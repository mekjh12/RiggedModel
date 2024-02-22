using OpenGL;

namespace LSystem
{
    public class AnimateShader : ShaderProgram
    {
        private static int MAX_JOINTS = 50;
        const string VERTEX_FILE = @"\Shader\ani.vert";
        const string FRAGMENT_FILE = @"\Shader\ani.frag";
        const string GEOMETRY_FILE = @"\Shader\ani.geom";
        private int[] loc_boneMatrix;

        public AnimateShader() : base(EngineLoop.PROJECT_PATH + VERTEX_FILE,
            EngineLoop.PROJECT_PATH + FRAGMENT_FILE, 
            EngineLoop.PROJECT_PATH + GEOMETRY_FILE)
        {

        }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "in_position");
            base.BindAttribute(1, "in_textureCoords");
            base.BindAttribute(2, "in_normal");
            base.BindAttribute(3, "in_jointIndices");
            base.BindAttribute(4, "in_weights");
        }

        protected override void GetAllUniformLocations()
        {
            UniformLocations("model", "view", "proj");
            UniformLocations("lightDirection");

            loc_boneMatrix = new int[MAX_JOINTS];
            for (int i = 0; i < MAX_JOINTS; i++)
            {
                loc_boneMatrix[i] = base.GetUniformLocation($"jointTransforms[{i}]");
            }
        }

        public void LoadTexture(string textureUniformName, TextureUnit textureUnit, uint texture)
        {
            base.LoadInt(_location[textureUniformName], textureUnit - TextureUnit.Texture0);
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(TextureTarget.Texture2d, texture);
        }

        public void PushBoneMatrix(int index, Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["bone" + index], matrix);
        }

        public void LoadProjMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["proj"], matrix);
        }

        public void LoadViewMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["view"], matrix);
        }

        public void LoadModelMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["model"], matrix);
        }
    }
}
