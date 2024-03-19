using Assimp;
using OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LSystem.Animate
{
    public class XmlDae
    {
        string _filename; // dae
        string _diffuseFileName;
        
        Dictionary<string, Animation> _animations;
        TexturedModel _model;
        RawModel3d _rawModel;
        Bone _rootBone;
        Dictionary<TextureType, Texture> _textures;
        Dictionary<string, int> _dicBoneIndex;
        Matrix4x4f[] _invBindPoses;
        string[] _boneNames;
        Matrix4x4f _rootMatrix;
        Matrix4x4f bind_shape_matrix;

        public Matrix4x4f BindShapeMatrix => bind_shape_matrix;

        public Matrix4x4f RootMatirix => _rootBone.BindTransform;

        public Bone RootBone => _rootBone;

        public int BoneCount => _boneNames.Length;

        public Matrix4x4f[] BoneMatrices => _invBindPoses;

        public TexturedModel Model => _model;

        public Animation Animation(string animationName)
        {
            return (_animations.ContainsKey(animationName))? _animations[animationName] : null;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="filename"></param>
        public XmlDae(string filename)
        {
            _filename = filename;
            _textures = new Dictionary<TextureType, Texture>();
            _model = Load(filename);
        }

        private void PrintBoneStructure()
        {
            Stack<Bone> stack = new Stack<Bone>();
            stack.Push(_rootBone);
            while (stack.Count > 0)
            {
                Bone bone = stack.Pop();
                if (bone.IsLeaf)
                {
                    Bone parent = bone;
                    string txt = "";
                    while (parent != null)
                    {
                        txt = parent.Name + "->" + txt;
                        parent = parent?.Parent;
                    }
                    Console.WriteLine(txt);
                }                
                foreach (Bone child in bone.Childrens) stack.Push(child);
            }
        }

        public TexturedModel Load(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

            // (1) library_images = textures
            LibraryImages(xml);

            // (2) library_geometries = position, normal, texcoord, color
            LibraryGeometris(xml, out List<uint> lstVertexIndices, out List<uint> normalIndices, out List<uint> texcoordIndices, out List<uint> colorIndices,
                out List<Vertex3f> lstPositions, out List<Vertex2f> lstTexCoord);

            // (3) library_controllers = boneIndex, boneWeight
            LibraryController(xml, out List<string> boneNames, out Dictionary<string, Matrix4x4f> invBindPoses, out List<Vertex4i> lstBoneIndex, out List<Vertex4f> lstBoneWeight);
            
            _boneNames = new string[boneNames.Count];
            _dicBoneIndex = new Dictionary<string, int>();
            for (int i = 0; i < boneNames.Count; i++)
            {
                _boneNames[i] = boneNames[i].Trim();
                _dicBoneIndex.Add(_boneNames[i], i);
            }

            _invBindPoses = new Matrix4x4f[invBindPoses.Values.Count];
            foreach (KeyValuePair<string, Matrix4x4f> item in invBindPoses)
            {
                int index = _dicBoneIndex[item.Key];
                _invBindPoses[index] = item.Value;
            }

            // (4) library_animations
            _animations = LibraryAnimations(xml);

            // (5) library_visual_scenes = bone hierarchy + rootBone
            _rootBone = LibraryVisualScenes(xml, boneNames, invBindPoses);

            // 읽어온 정보의 인덱스를 이용하여 배열을 만든다.
            int count = lstVertexIndices.Count;
            float[] postions = new float[count * 3];
            float[] texcoords = new float[count * 2];
            uint[] boneIndices = new uint[count * 4];
            float[] boneWeights = new float[count * 4];
            for (int i = 0; i < count; i++)
            {
                int idx = (int)lstVertexIndices[i];
                int tidx = (int)texcoordIndices[i];
                postions[3 * i + 0] = lstPositions[idx].x;
                postions[3 * i + 1] = lstPositions[idx].y;
                postions[3 * i + 2] = lstPositions[idx].z;

                texcoords[2 * i + 0] = lstTexCoord[tidx].x;
                texcoords[2 * i + 1] = lstTexCoord[tidx].y;

                boneIndices[4 * i + 0] = (uint)lstBoneIndex[idx].x;
                boneIndices[4 * i + 1] = (uint)lstBoneIndex[idx].y;
                boneIndices[4 * i + 2] = (uint)lstBoneIndex[idx].z;
                boneIndices[4 * i + 3] = (uint)lstBoneIndex[idx].w;

                boneWeights[4 * i + 0] = (float)lstBoneWeight[idx].x;
                boneWeights[4 * i + 1] = (float)lstBoneWeight[idx].y;
                boneWeights[4 * i + 2] = (float)lstBoneWeight[idx].z;
                boneWeights[4 * i + 3] = (float)lstBoneWeight[idx].w;
            }

            PrintBoneStructure();

            // VAO, VBO로 Raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            GpuLoader.StoreDataInAttributeList(0, 3, postions, BufferUsage.StaticDraw);
            GpuLoader.StoreDataInAttributeList(1, 2, texcoords, BufferUsage.StaticDraw);
            GpuLoader.StoreDataInAttributeList(3, 4, boneIndices, BufferUsage.StaticDraw);
            GpuLoader.StoreDataInAttributeList(4, 4, boneWeights, BufferUsage.StaticDraw);
            //GpuLoader.BindIndicesBuffer(lstVertexIndices.ToArray());
            Gl.BindVertexArray(0);
            _rawModel = new RawModel3d(vao, postions);

            TexturedModel texturedModel = new TexturedModel(_rawModel, _textures[TextureType.Diffuse]);
            texturedModel.IsDrawElement = false;
            return texturedModel;
        }


        /// <summary>
        /// * TextureStorage에 텍스처를 로딩한다. <br/>
        /// - 딕셔너리의 키는 전체파일명으로 한다.<br/>
        /// </summary>
        /// <param name="xml"></param>
        private void LibraryImages(XmlDocument xml)
        {
            XmlNodeList libraryImagesNode = xml.GetElementsByTagName("library_images");
            if (libraryImagesNode.Count > 0)
            {
                if (libraryImagesNode[0].HasChildNodes)
                {
                    _diffuseFileName = Path.GetDirectoryName(_filename) + "\\" + libraryImagesNode[0]["image"]["init_from"].InnerText;
                    _diffuseFileName = _diffuseFileName.Replace("%20", " ");

                    if (!File.Exists(_diffuseFileName))
                    {
                        Console.WriteLine($"[로딩에러] library_image가 존재하지 않습니다. {_diffuseFileName}");
                    }

                    if (TextureStorage.TexturesLoaded.ContainsKey(_diffuseFileName)) // 로드한 적이 있음
                    {                         
                        _textures[TextureType.Diffuse] = TextureStorage.TexturesLoaded[_diffuseFileName];
                    }
                    else  // 로드한 적이 없음
                    {
                        Texture texture = new Texture(_diffuseFileName);
                        _textures.Add(TextureType.Diffuse, texture);
                        TextureStorage.TexturesLoaded.Add(_diffuseFileName, texture);
                    }
                }
            }
        }

        private void LibraryGeometris(XmlDocument xml, 
            out List<uint> lstVertexIndices, out List<uint> normalIndices, out List<uint> texcoordIndices, out List<uint> colorIndices,
            out List<Vertex3f> lstPositions, out List<Vertex2f> lstTexCoord)
        {
            lstVertexIndices = new List<uint>();
            normalIndices = new List<uint>();
            texcoordIndices = new List<uint>();
            colorIndices = new List<uint>();
            lstPositions = new List<Vertex3f>();
            lstTexCoord = new List<Vertex2f>();

            XmlNodeList libraryGeometries = xml.GetElementsByTagName("library_geometries");

            if (libraryGeometries.Count > 0)
            {
                XmlNode library_geometries = libraryGeometries[0];
                XmlNode geometry = library_geometries["geometry"];
                XmlNode vertices = geometry["mesh"]["vertices"];
                string positionName = vertices["input"].Attributes["source"].Value;
                XmlNode triangles = geometry["mesh"]["triangles"];
                string vertexName = "";
                string normalName = "";
                string texcoordName = "";
                string colorName = "";

                int vertexOffset = -1;
                int normalOffset = -1;
                int texcoordOffset = -1;
                int colorOffset = -1;

                // 인덱스를 읽어온다. pos, tex, nor, color indices list
                #region 인덱스
                foreach (XmlNode input in triangles.ChildNodes)
                {
                    if (input.Name == "input")
                    {
                        if (input.Attributes["semantic"].Value == "VERTEX")
                        {
                            vertexName = input.Attributes["source"].Value;
                            vertexOffset = int.Parse(input.Attributes["offset"].Value);
                        }
                        if (input.Attributes["semantic"].Value == "NORMAL")
                        {
                            normalName = input.Attributes["source"].Value;
                            normalOffset = int.Parse(input.Attributes["offset"].Value);
                        }
                        if (input.Attributes["semantic"].Value == "TEXCOORD")
                        {
                            texcoordName = input.Attributes["source"].Value;
                            texcoordOffset = int.Parse(input.Attributes["offset"].Value);
                        }
                        if (input.Attributes["semantic"].Value == "COLOR")
                        {
                            colorName = input.Attributes["source"].Value;
                            colorOffset = int.Parse(input.Attributes["offset"].Value);
                        }
                    }
                }

                XmlNode p = geometry["mesh"]["triangles"]["p"];
                string[] values = p.InnerText.Split(new char[] { ' ' });
                int total = (vertexOffset >= 0 ? 1 : 0) + (normalOffset >= 0 ? 1 : 0)
                    + (texcoordOffset >= 0 ? 1 : 0) + (colorOffset >= 0 ? 1 : 0);

                for (int i = 0; i < values.Length; i += total)
                {
                    if (vertexOffset >= 0) lstVertexIndices.Add(uint.Parse(values[i + vertexOffset]));
                    if (normalOffset >= 0) normalIndices.Add(uint.Parse(values[i + normalOffset]));
                    if (texcoordOffset >= 0) texcoordIndices.Add(uint.Parse(values[i + texcoordOffset]));
                    if (colorOffset >= 0) colorIndices.Add(uint.Parse(values[i + colorOffset]));
                }
                #endregion

                // 기본데이터를 읽어옴.
                foreach (XmlNode source in geometry["mesh"].ChildNodes)
                {
                    if (source.Name != "source") continue;

                    // 소스텍스트로부터 실수 배열을 만든다.
                    string sourcesId = source.Attributes["id"].Value;
                    string[] value = source["float_array"].InnerText.Split(' ');
                    float[] items = new float[value.Length];
                    for (int i = 0; i < value.Length; i++) items[i] = float.Parse(value[i]);

                    if ("#" + sourcesId == positionName)
                    {
                        for (int i = 0; i < items.Length; i += 3)
                        {
                            lstPositions.Add(new Vertex3f(1 * items[i], 1 * items[i + 1], 1 * items[i + 2]));
                        }
                    }
                    else if ("#" + sourcesId == normalName)
                    {
                    }
                    else if ("#" + sourcesId == texcoordName)
                    {
                        for (int i = 0; i < items.Length; i += 2)
                        {
                            lstTexCoord.Add(new Vertex2f(items[i], 1.0f - items[i + 1]));
                        }
                    }
                    else if ("#" + sourcesId == colorName)
                    {

                    }
                }
            }
        }

        private void LibraryController(XmlDocument xml, out List<string> boneNames, out Dictionary<string, Matrix4x4f> invBindPoses,
            out List<Vertex4i> lstBoneIndex, out List<Vertex4f> lstBoneWeight)
        {
            lstBoneIndex = new List<Vertex4i>();
            lstBoneWeight = new List<Vertex4f>();

            string jointsName = "";
            string inverseBindMatrixName = "";
            string weightName = "";
            int jointsOffset = -1;
            int weightOffset = -1;
            invBindPoses = new Dictionary<string, Matrix4x4f>();
            boneNames = new List<string>();

            List<float> weightList = new List<float>();

            XmlNodeList libraryControllers = xml.GetElementsByTagName("library_controllers");
            if (libraryControllers.Count > 0)
            {
                XmlNode libraryController = libraryControllers[0];
                XmlNode geometry = libraryController["controller"];
                XmlNode joints = geometry["skin"]["joints"];
                XmlNode vertex_weights = geometry["skin"]["vertex_weights"];

                string[] eles = geometry["skin"]["bind_shape_matrix"].InnerText.Split(' ');
                float[] eleValues = new float[eles.Length];
                for (int i = 0; i < eles.Length; i++)
                    eleValues[i] = float.Parse(eles[i]);
                bind_shape_matrix = new Matrix4x4f(eleValues).Transposed;

                // joints 읽어옴.
                foreach (XmlNode input in joints.ChildNodes)
                {
                    if (input.Name == "input")
                    {
                        // name 가져오기
                        if (input.Attributes["semantic"].Value == "JOINT")
                        {
                            jointsName = input.Attributes["source"].Value;
                        }
                        if (input.Attributes["semantic"].Value == "INV_BIND_MATRIX")
                        {
                            inverseBindMatrixName = input.Attributes["source"].Value;
                        }

                        foreach (XmlNode source in geometry["skin"].ChildNodes)
                        {
                            if (source.Name == "source")
                            {
                                string sourcesId = source.Attributes["id"].Value;

                                if (source["Name_array"] != null)
                                {
                                    string[] value = source["Name_array"].InnerText.Split(' ');

                                    // BoneName가져오기
                                    if ("#" + sourcesId == jointsName)
                                    {
                                        boneNames.Clear();
                                        boneNames.AddRange(value);
                                    }
                                }

                                if (source["float_array"] != null)
                                {
                                    string[] value = source["float_array"].InnerText.Split(' ');
                                    float[] items = new float[value.Length];
                                    for (int i = 0; i < value.Length; i++)
                                        items[i] = float.Parse(value[i]);

                                    // INV_BIND_MATRIX
                                    if ("#" + sourcesId == inverseBindMatrixName)
                                    {
                                        for (int i = 0; i < items.Length; i += 16)
                                        {
                                            List<float> mat = new List<float>();
                                            for (int j = 0; j < 16; j++) mat.Add(items[i + j]);
                                            Matrix4x4f bindpose = new Matrix4x4f(mat.ToArray());
                                            invBindPoses.Add(boneNames[i / 16], bindpose.Transposed);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // vertex_weights 읽어옴.
                foreach (XmlNode input in vertex_weights.ChildNodes)
                {
                    if (input.Name == "input")
                    {
                        // name 가져오기
                        if (input.Attributes["semantic"].Value == "WEIGHT") weightName = input.Attributes["source"].Value;
                        foreach (XmlNode source in geometry["skin"].ChildNodes)
                        {
                            if (source.Name == "source")
                            {
                                string sourcesId = source.Attributes["id"].Value;
                                if (source["float_array"] != null)
                                {
                                    string[] value = source["float_array"].InnerText.Split(' ');
                                    float[] items = new float[value.Length];
                                    for (int i = 0; i < value.Length; i++)
                                        items[i] = float.Parse(value[i]);

                                    // WEIGHT
                                    if ("#" + sourcesId == weightName) weightList.AddRange(items);
                                }
                            }
                        }
                    }
                }

                // vertex_weights - vcount, v 읽어옴.
                XmlNode vcount = geometry["skin"]["vertex_weights"]["vcount"];
                XmlNode v = geometry["skin"]["vertex_weights"]["v"];
                string[] vcountArray = vcount.InnerText.Trim().Split(' ');
                int[] vcountIntArray = new int[vcountArray.Length];
                uint total = 0;
                for (int i = 0; i < vcountArray.Length; i++)
                {
                    vcountIntArray[i] = int.Parse(vcountArray[i].Trim());
                    total += (uint)vcountIntArray[i];
                }

                foreach (XmlNode input in geometry["skin"]["vertex_weights"].ChildNodes)
                {
                    if (input.Name == "input")
                    {
                        if (input.Attributes["semantic"].Value == "JOINT") jointsOffset = int.Parse(input.Attributes["offset"].Value);
                        if (input.Attributes["semantic"].Value == "WEIGHT") weightOffset = int.Parse(input.Attributes["offset"].Value);
                    }
                }

                string[] vArray = v.InnerText.Split(' ');
                int sum = 0;
                for (int i = 0; i < vcountIntArray.Length; i++)
                {
                    int vertexCount = vcountIntArray[i];
                    List<int> boneIndexList = new List<int>();
                    List<int> boneWeightList = new List<int>();

                    for (int j = 0; j < vertexCount; j++)
                    {
                        if (jointsOffset >= 0)
                            boneIndexList.Add(int.Parse(vArray[sum + 2 * j + jointsOffset].Trim()));
                        if (weightOffset >= 0)
                            boneWeightList.Add(int.Parse(vArray[sum + 2 * j + weightOffset].Trim()));
                    }

                    Vertex4i jointId = Vertex4i.Zero;
                    if (boneIndexList.Count == 0) jointId = new Vertex4i(-1, -1, -1, -1);
                    if (boneIndexList.Count == 1) jointId = new Vertex4i(boneIndexList[0], -1, -1, -1);
                    if (boneIndexList.Count == 2) jointId = new Vertex4i(boneIndexList[0], boneIndexList[1], -1, -1);
                    if (boneIndexList.Count == 3) jointId = new Vertex4i(boneIndexList[0], boneIndexList[1], boneIndexList[2], -1);
                    if (boneIndexList.Count >= 4) jointId = new Vertex4i(boneIndexList[0], boneIndexList[1], boneIndexList[2], boneIndexList[3]);

                    float bx = boneWeightList.Count > 0 ? weightList[boneWeightList[0]] : 0.0f;
                    float by = boneWeightList.Count > 1 ? weightList[boneWeightList[1]] : 0.0f;
                    float bz = boneWeightList.Count > 2 ? weightList[boneWeightList[2]] : 0.0f;
                    float bw = boneWeightList.Count > 3 ? weightList[boneWeightList[3]] : 0.0f;
                    Vertex4f weight = new Vertex4f(bx, by, bz, bw);

                    lstBoneIndex.Add(jointId);
                    lstBoneWeight.Add(weight);
                    sum += 2 * vertexCount;
                }
            }
        }

        private Dictionary<string, Animation> LibraryAnimations(XmlDocument xml)
        {
            XmlNodeList libraryAnimations = xml.GetElementsByTagName("library_animations");
            if (libraryAnimations.Count == 0)
            {
                Console.WriteLine($"[에러] dae파일구조에서 애니메이션구조를 읽어올 수 없습니다.");
                return null;
            }

            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();

            Dictionary<string, Dictionary<float, Matrix4x4f>> ani = new Dictionary<string, Dictionary<float, Matrix4x4f>>();
            foreach (XmlNode libraryAnimation in libraryAnimations[0])
            {
                string animationName = libraryAnimation.Attributes["name"].Value;
                float maxTimeLength = 0.0f;

                // bone마다 순회
                foreach (XmlNode boneAnimation in libraryAnimation.ChildNodes)
                {
                    string boneName = boneAnimation.Attributes["id"].Value.Substring(animationName.Length + 1);
                    int fIdx = boneName.IndexOf("_");
                    string actionName = (fIdx >= 0) ? boneName.Substring(0, fIdx) : "";
                    boneName = (fIdx >= 0) ? boneName.Substring(fIdx + 1) : boneName;
                    boneName = boneName.Replace("_pose_matrix", "");

                    List<float> sourceInput = new List<float>(); // time interval
                    List<Matrix4x4f> sourceOutput = new List<Matrix4x4f>();
                    List<string> interpolationInput = new List<string>();

                    XmlNode channel = boneAnimation["channel"];
                    string channelName = channel.Attributes["source"].Value;

                    XmlNode sampler = boneAnimation["sampler"];
                    if (channelName != "#" + sampler.Attributes["id"].Value) continue;

                    string inputName = "";
                    string outputName = "";
                    string interpolationName = "";

                    // semantic의 Name을 읽어옴.
                    foreach (XmlNode input in sampler.ChildNodes)
                    {
                        if (input.Attributes["semantic"].Value == "INPUT") inputName = input.Attributes["source"].Value;
                        if (input.Attributes["semantic"].Value == "OUTPUT") outputName = input.Attributes["source"].Value;
                        if (input.Attributes["semantic"].Value == "INTERPOLATION") interpolationName = input.Attributes["source"].Value;
                    }

                    // bone의 애니메이션 소스를 읽어온다.
                    foreach (XmlNode source in boneAnimation.ChildNodes)
                    {
                        if (source.Name == "source")
                        {
                            string sourcesId = source.Attributes["id"].Value;
                            if ("#" + sourcesId == inputName)
                            {
                                string[] value = source["float_array"].InnerText.Split(' ');
                                float[] items = new float[value.Length];
                                for (int i = 0; i < value.Length; i++)
                                {
                                    items[i] = float.Parse(value[i]);
                                    maxTimeLength = Math.Max(items[i], maxTimeLength);
                                }
                                sourceInput.AddRange(items);
                            }

                            if ("#" + sourcesId == outputName)
                            {
                                string[] value = source["float_array"].InnerText.Split(' ');
                                float[] items = new float[value.Length];
                                for (int i = 0; i < value.Length; i++) items[i] = float.Parse(value[i]);
                                for (int i = 0; i < value.Length; i += 16)
                                {
                                    List<float> mat = new List<float>();
                                    for (int j = 0; j < 16; j++) mat.Add(items[i + j]);
                                    Matrix4x4f matrix = new Matrix4x4f(mat.ToArray());
                                    sourceOutput.Add(matrix.Transposed);
                                }
                            }

                            if ("#" + sourcesId == interpolationName)
                            {
                                string[] value = source["Name_array"].InnerText.Split(' ');
                                interpolationInput.AddRange(value);
                            }
                        }
                    }

                    // 가져온 소스로 키프레임을 만든다.
                    Dictionary<float, Matrix4x4f> keyframe = new Dictionary<float, Matrix4x4f>();
                    for (int i = 0; i < sourceInput.Count; i++)
                    {
                        keyframe.Add(sourceInput[i], sourceOutput[i]);
                    }

                    ani.Add(boneName, keyframe);
                }

                Animation animation = new Animation(animationName, maxTimeLength);
                if (maxTimeLength > 0)
                {
                    foreach (KeyValuePair<string, Dictionary<float, Matrix4x4f>> item in ani)
                    {
                        string boneName = item.Key;
                        Dictionary<float, Matrix4x4f> source = item.Value;
                        foreach (KeyValuePair<float, Matrix4x4f> subsource in source)
                        {
                            float time = subsource.Key;
                            Matrix4x4f mat = subsource.Value;
                            animation.AddKeyFrame(time);

                            Quaternion q = ToQuaternion(mat);
                            q.Normalize();
                            BonePose bonePose = new BonePose();
                            bonePose.Position = new Vertex3f(mat[3, 0], mat[3, 1], mat[3, 2]);
                            bonePose.Rotation = q;
                            animation[time].AddBoneTransform(boneName, bonePose);
                        }
                    }
                }
                animations.Add(animationName, animation);
            }

            return animations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private Bone LibraryVisualScenes(XmlDocument xml, List<string> boneNames, Dictionary<string, Matrix4x4f> invBindPoses)
        {
            XmlNodeList library_visual_scenes = xml.GetElementsByTagName("library_visual_scenes");
            if (library_visual_scenes.Count == 0)
            {
                Console.WriteLine($"[에러] dae파일구조에서 뼈대구조를 읽어올 수 없습니다.");
                return null;
            }

            Stack<XmlNode> nStack = new Stack<XmlNode>();
            Stack<Bone> bStack = new Stack<Bone>();
            XmlNode nodes = library_visual_scenes[0]["visual_scene"];
            XmlNode rootNode = null;

            // Find Root Node
            foreach (XmlNode item in nodes) 
                if (item.Attributes["id"].Value == "Armature") rootNode = item;
            if (rootNode == null) return null;

            nStack.Push(rootNode);
            Bone rootBone = new Bone("Armature", 0, Matrix4x4f.Identity);
            bStack.Push(rootBone);
            while (nStack.Count > 0)
            {
                XmlNode node = nStack.Pop();
                Bone bone = bStack.Pop();

                string[] value = node["matrix"].InnerText.Split(' ');
                float[] items = new float[value.Length];
                for (int i = 0; i < value.Length; i++) items[i] = float.Parse(value[i]);
                Matrix4x4f mat = new Matrix4x4f(items).Transposed;

                string boneName = node.Attributes["sid"]?.Value;
                if (boneName == null)
                {
                    if (node.Attributes["name"].Value == "Armature")
                    {
                        bone.Name = "Armature";
                        bone.BindTransform = mat;
                        bone.Index = 0;
                    }
                }
                else
                {
                    bone.Name = boneName;
                    bone.BindTransform = mat;
                    if (_dicBoneIndex.ContainsKey(boneName))
                        bone.Index = _dicBoneIndex[boneName];
                }

                if (invBindPoses.ContainsKey(bone.Name))
                {
                    bone.InverseBindTransform = invBindPoses[bone.Name];
                }

                // 하위 노드를 순회한다.
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name != "node") continue;
                    nStack.Push(child);
                    Bone childBone = new Bone("", 0, Matrix4x4f.Identity);
                    childBone.Parent = bone;
                    bone.AddChild(childBone);
                    bStack.Push(childBone);
                }

            }

            return rootBone;
        }

        /// <summary>
        /// Quaternions for Computer Graphics by John Vince. p199 참고
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private Quaternion ToQuaternion(Matrix4x4f mat)
        {
            Quaternion q = Quaternion.Identity;
            float a11 = mat[0, 0];
            float a12 = mat[1, 0];
            float a13 = mat[2, 0];

            float a21 = mat[0, 1];
            float a22 = mat[1, 1];
            float a23 = mat[2, 1];

            float a31 = mat[0, 2];
            float a32 = mat[1, 2];
            float a33 = mat[2, 2];

            float trace = a11 + a22 + a33;
            if (trace >= -1)
            {
                // I changed M_EPSILON to 0
                float s = 0.5f / (float)Math.Sqrt(trace + 1.0f);
                q.W = 0.25f / s;
                q.X = (a32 - a23) * s;
                q.Y = (a13 - a31) * s;
                q.Z = (a21 - a12) * s;
            }
            else
            {
                if (1 + a11 - a22 - a33 >= 0)
                {
                    float s = 2.0f * (float)Math.Sqrt(1.0f + a11 - a22 - a33);
                    q.X = 0.25f * s;
                    q.Y = (a12 + a21) / s;
                    q.Z = (a13 + a31) / s;
                    q.W = (a32 - a23) / s;
                }
                else if (1 - a11 + a22 - a33 >= 0)
                {
                    float s = 2.0f * (float)Math.Sqrt(1 - a11 + a22 - a33);
                    q.Y = 0.25f * s;
                    q.X = (a12 + a21) / s;
                    q.Z = (a23 + a32) / s;
                    q.W = (a13 - a31) / s;
                }
                else
                {
                    float s = 2.0f * (float)Math.Sqrt(1 - a11 - a22 + a33);
                    q.Z = 0.25f * s;
                    q.X = (a13 + a31) / s;
                    q.Y = (a23 + a32) / s;
                    q.W = (a21 - a12) / s;
                }
            }
            return q;
        }

    }
}
