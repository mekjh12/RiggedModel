using LSystem.Animate;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using static Assimp.Metadata;

namespace LSystem
{
    public partial class Form3D : Form
    {
        EngineLoop _gameLoop;
        List<Entity> entities;
        StaticShader _shader;
        AnimateShader _ashader;
        BoneWeightShader _bwShader;

        AniModel _aniModel;
        XmlDae xmlDae;
        int _boneIndex = 0;
        float _axisLength = 20.3f;
        float _drawThick = 1.0f;

        PolygonMode _polygonMode = PolygonMode.Fill;
        bool _isDraged = false;
        bool _isShifted = false;

        Vertex3f[] _point;
        Vertex3f _ikPoint;

        enum RenderingMode { Animation, BoneWeight, Static, None, Count };
        RenderingMode _renderingMode = RenderingMode.Animation;

        public Form3D()
        {
            InitializeComponent();
        }

        private void Form3D_Load(object sender, EventArgs e)
        {            
            // ### 초기화 ###
            IniFile.SetFileName("setup.ini");

            // ### 초기화 ###
            _gameLoop = new EngineLoop();
            _shader = new StaticShader();
            _ashader = new AnimateShader();
            _bwShader = new BoneWeightShader();
            entities = new List<Entity>();

            xmlDae = new XmlDae(EngineLoop.PROJECT_PATH + "\\Res\\Guybrush_final.dae", isLoadAnimation: false);
            string[] files = Directory.GetFiles(EngineLoop.PROJECT_PATH + "\\Res\\Action\\");
            foreach (string fn in files)
                this.cbAction.Items.Add(xmlDae.AddAction(fn));

            Entity daeEntity = new Entity(xmlDae.Model);
            daeEntity.Material = new Material();
            daeEntity.Position = new Vertex3f(0, 0, 0);
            daeEntity.IsAxisVisible = true;

            _aniModel = new AniModel(daeEntity, xmlDae);
            _aniModel.SetAnimation(xmlDae.DefaultAnimation.Name);

            // 설정 읽어오기
            // -------------------------------------------------------------------------------------------------------
            float cx = float.Parse(IniFile.GetPrivateProfileString("camera", "x", "0.0"));
            float cy = float.Parse(IniFile.GetPrivateProfileString("camera", "y", "0.0"));
            float cz = float.Parse(IniFile.GetPrivateProfileString("camera", "z", "0.0"));
            float yaw = float.Parse(IniFile.GetPrivateProfileString("camera", "yaw", "0.0"));
            float pitch = float.Parse(IniFile.GetPrivateProfileString("camera", "pitch", "0.0"));
            float distance = float.Parse(IniFile.GetPrivateProfileString("camera", "distance", "3.0"));
            float fov = float.Parse(IniFile.GetPrivateProfileString("camera", "fov", "90.0"));
            this.ckBoneBindPose.Checked = bool.Parse(IniFile.GetPrivateProfileString("control", "isvisibleBindBone", "true"));
            this.ckBoneVisible.Checked = bool.Parse(IniFile.GetPrivateProfileString("control", "isvisibleBone", "true"));
            _renderingMode = (RenderingMode)int.Parse(IniFile.GetPrivateProfileString("Rendering", "RenderingMode", "0"));
            _drawThick = float.Parse(IniFile.GetPrivateProfileString("Rendering", "drawThick", "1.0"));
            _axisLength = float.Parse(IniFile.GetPrivateProfileString("Rendering", "axisLength", "1.0"));
            // -------------------------------------------------------------------------------------------------------

            _gameLoop.Camera = new OrbitCamera("", cx, cy, cz, distance);
            _gameLoop.Camera.CameraPitch = pitch;
            _gameLoop.Camera.CameraYaw = yaw;
            _gameLoop.Camera.FOV = fov;
            this.trFov.Value = (int)fov;
            this.lblFov.Text = $"Fov={fov}";
            this.trTime.Maximum = (int)_aniModel.Animator.CurrentAnimation.Length * 100;
            this.trTime.Minimum = 0;
            this.trAxisLength.Value = (int)(_axisLength * 10.0f);

            // ### 주요로직 ###
            _gameLoop.UpdateFrame = (deltaTime) =>
            {
                float milliSecond = deltaTime * 0.001f;
                int w = this.glControl1.Width;
                int h = this.glControl1.Height;

                if (_gameLoop.Width * _gameLoop.Height == 0)
                {
                    _gameLoop.Init(w, h);
                    _gameLoop.Camera.Init(w, h);
                }

                _aniModel.Update(deltaTime);
                Matrix4x4f poseRootMatrix = _aniModel.RootBoneTransform * _aniModel.BindShapeMatrix;
                Bone bone = _aniModel.GetBone("mixamorig_LeftToeBase");
                _point = Kinetics.IKSolved2(_ikPoint, bone, 3, 100);

                Entity entity = entities.Count > 0 ? entities[0] : null;
                if (Keyboard.IsKeyDown(Key.D1)) entity.Roll(1);
                if (Keyboard.IsKeyDown(Key.D2)) entity.Roll(-1);
                if (Keyboard.IsKeyDown(Key.D3)) entity.Yaw(1);
                if (Keyboard.IsKeyDown(Key.D4)) entity.Yaw(-1);
                if (Keyboard.IsKeyDown(Key.D5)) entity.Pitch(1);
                if (Keyboard.IsKeyDown(Key.D6)) entity.Pitch(-1);

                OrbitCamera camera = _gameLoop.Camera as OrbitCamera;
                this.Text = $"{FramePerSecond.FPS}fps, t={FramePerSecond.GlobalTick} p={camera.Position}, distance={camera.Distance}";
                this.trTime.Value = (int)(_aniModel.AnimationTime * 100).Clamp(0, this.trTime.Maximum);
            };

            _gameLoop.RenderFrame = (deltaTime) =>
            {
                Camera camera = _gameLoop.Camera;

                Gl.Enable(EnableCap.CullFace);
                Gl.CullFace(CullFaceMode.Back);

                Gl.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                Gl.Enable(EnableCap.DepthTest);

                Gl.Enable(EnableCap.Blend);
                Gl.BlendEquation(BlendEquationMode.FuncAdd);
                Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                Gl.PolygonMode(MaterialFace.FrontAndBack, _polygonMode);
                Renderer.RenderAxis(_shader, camera);

                _aniModel.BindShapeMatrix = xmlDae.BindShapeMatrix;
                Matrix4x4f poseRootMatrix = _aniModel.RootBoneTransform * _aniModel.BindShapeMatrix;
                Matrix4x4f[] jointMatrix = _aniModel.BoneAnimationBindTransforms;

                if (_renderingMode == RenderingMode.Animation)
                    Renderer.Render(_ashader, jointMatrix, poseRootMatrix, _aniModel.Entity, camera);
                else if (_renderingMode == RenderingMode.BoneWeight)
                    Renderer.Render(_bwShader, poseRootMatrix, _boneIndex, _aniModel.Entity, camera);
                else if (_renderingMode == RenderingMode.Static)
                    Renderer.Render(_shader, _aniModel.Entity, camera, poseRootMatrix);

                if (this.ckBoneBindPose.Checked) // 정지 뼈대
                {
                    foreach (Matrix4x4f jointTransform in _aniModel.InverseBindPoseTransforms)
                        Renderer.RenderLocalAxis(_shader, camera, size: _axisLength, thick: _drawThick, jointTransform.Inverse);
                }
                
                if (this.ckBoneVisible.Checked) // 애니메이션 뼈대 렌더링
                {
                    foreach (Matrix4x4f jointTransform in _aniModel.BoneAnimationTransforms)
                        Renderer.RenderLocalAxis(_shader, camera, size: _axisLength, thick: _drawThick, jointTransform);
                }

                for (int i = 0; i < _point.Length; i++)
                    Renderer.RenderPoint(_shader, _point[i], camera, new Vertex4f(1, 0, 0, 1), size: 0.02f);
                Renderer.RenderPoint(_shader, _ikPoint, camera, new Vertex4f(1, 1, 0, 1), size: 0.02f);
            };
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Camera camera = _gameLoop.Camera;
            if (camera is FpsCamera) camera?.GoForward(0.02f * e.Delta);
            if (camera is OrbitCamera) (camera as OrbitCamera)?.FarAway(-0.005f * e.Delta);
        }

        private void glControl1_Render(object sender, GlControlEventArgs e)
        {
            int glLeft = this.Width - this.glControl1.Width;
            int glTop = this.Height - this.glControl1.Height;
            int glWidth = this.glControl1.Width;
            int glHeight = this.glControl1.Height;
            _gameLoop.DetectInput(this.Left + glLeft, this.Top + glTop, glWidth, glHeight);

            // 엔진 루프, 처음 로딩시 deltaTime이 커지는 것을 방지
            if (FramePerSecond.DeltaTime < 1000)
            {
                _gameLoop.Update(deltaTime: FramePerSecond.DeltaTime);
                _gameLoop.Render(deltaTime: FramePerSecond.DeltaTime);
            }
            FramePerSecond.Update();
        }

        private void WriteEnv()
        {
            // 종료 설정 저장
            IniFile.WritePrivateProfileString("camera", "x", _gameLoop.Camera.Position.x);
            IniFile.WritePrivateProfileString("camera", "y", _gameLoop.Camera.Position.y);
            IniFile.WritePrivateProfileString("camera", "z", _gameLoop.Camera.Position.z);
            IniFile.WritePrivateProfileString("camera", "yaw", _gameLoop.Camera.CameraYaw);
            IniFile.WritePrivateProfileString("camera", "pitch", _gameLoop.Camera.CameraPitch);
            IniFile.WritePrivateProfileString("camera", "distance", ((OrbitCamera)_gameLoop.Camera).Distance);
        }

        private void glControl1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Shift) _isShifted = true;

            if (e.KeyCode == Keys.Escape)
            {
                if (MessageBox.Show("정말로 끝내시겠습니까?", "종료", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WriteEnv();
                    Application.Exit();
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                _polygonMode++;
                if (_polygonMode == (PolygonMode)6915) _polygonMode = (PolygonMode)6912;
            }
            else if (e.KeyCode == Keys.Space)
            {
                _aniModel.Animator.Toggle();
            }
            else if (e.KeyCode == Keys.Oemplus)
            {
                _boneIndex++;
                Console.WriteLine(_boneIndex);
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                _boneIndex--;
                Console.WriteLine(_boneIndex);
            }
            else if (e.KeyCode == Keys.R)
            {
                _renderingMode++;
                if (_renderingMode == RenderingMode.Count) _renderingMode = 0;
                IniFile.WritePrivateProfileString("Rendering", "RenderingMode", (int)_renderingMode);
            }
        }

        private void ckBoneVisible_CheckedChanged(object sender, EventArgs e)
        {
            IniFile.WritePrivateProfileString("control", "isvisibleBone", this.ckBoneVisible.Checked.ToString());
        }

        private void glControl1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Camera camera = _gameLoop.Camera;            
            camera.GoTo(_aniModel.AnimatedRootBone.Position);
            if (camera is OrbitCamera) (camera as OrbitCamera).Distance = 5.0f;
        }

        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isDraged = true;
            if (e.Button == MouseButtons.Left)
            {
                Camera camera = _gameLoop.Camera;
                _ikPoint = Picker3d.PickUpPoint(camera, e.X, e.Y, glControl1.Width, glControl1.Height);
                this.lbPrint.Text = _ikPoint.ToString();
            }
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _isDraged = false;
        }

        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Mouse.CurrentPosition = new Vertex2i(e.X, e.Y);

            if (e.Button == MouseButtons.Middle && _isShifted)
            {
                Camera camera = _gameLoop.Camera;
                float sensity = 0.3f;
                Vertex2i delta = Mouse.DeltaPosition;
                camera?.GoRight(-sensity * delta.x);
                camera?.GoForward(sensity * delta.y);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                Camera camera = _gameLoop.Camera;
                Vertex2i delta = Mouse.DeltaPosition;
                camera?.Yaw(-delta.x);
                camera?.Pitch(delta.y);
            }

            Mouse.PrevPosition = new Vertex2i(e.X, e.Y);
        }

        private void trFov_Scroll(object sender, EventArgs e)
        {
            Camera camera = _gameLoop.Camera;
            camera.FOV = trFov.Value;
            this.lblFov.Text = "Fov=" + camera.FOV;
            IniFile.WritePrivateProfileString("camera", "fov", camera.FOV);
        }

        private void trTime_ValueChanged(object sender, EventArgs e)
        {
            lblTime.Text = $"Time={_aniModel.Animator.AnimationTime}s";
        }

        private void glControl1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!e.Shift) _isShifted = false;
        }

        private void trAxisLength_Scroll(object sender, EventArgs e)
        {
            _axisLength = trAxisLength.Value * 0.1f;
            IniFile.WritePrivateProfileString("Rendering", "axisLength", _axisLength);
        }

        private void cbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.trTime.Maximum = (int)_aniModel.Animator.CurrentAnimation.Length * 100;
            this.trTime.Minimum = 0; _aniModel.SetAnimation(cbAction.Text);
        }

        private void btnIKSolved_Click(object sender, EventArgs e)
        {
        }

        private void trThick_Scroll(object sender, EventArgs e)
        {
            _drawThick = (float)trThick.Value / 100.0f;
            IniFile.WritePrivateProfileString("Rendering", "drawThick", _drawThick);
        }

        private void ckBoneBindPose_CheckedChanged(object sender, EventArgs e)
        {
            IniFile.WritePrivateProfileString("control", "isvisibleBindBone", this.ckBoneBindPose.Checked.ToString());
        }

        private void glControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
