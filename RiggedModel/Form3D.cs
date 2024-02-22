using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace LSystem
{
    public partial class Form3D : Form
    {
        EngineLoop _gameLoop;
        List<Entity> entities;
        StaticShader _shader;
        AnimateShader _ashader;
        AnimatedModel _animatedModel;
        ModelDae daeModel1;
        PolygonMode _polygonMode = PolygonMode.Fill;

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
            entities = new List<Entity>();

            string fileName = EngineLoop.PROJECT_PATH + "\\Res\\test.dae";
            daeModel1 = new ModelDae(fileName);
            Entity daeEntity = new Entity(daeModel1.Model);
            daeEntity.Material = new Material();
            daeEntity.Position = new Vertex3f(0, 0, 0);
            daeEntity.Scaled(1, 1, 1);
            _animatedModel = new AnimatedModel(daeEntity, daeModel1.RootBone, daeModel1.BoneCount);
            _animatedModel.DoAnimation(daeModel1.Animation);
            entities.Add(daeEntity);

            // 카메라 설정
            float cx = float.Parse(IniFile.GetPrivateProfileString("camera", "x", "0.0"));
            float cy = float.Parse(IniFile.GetPrivateProfileString("camera", "y", "0.0"));
            float cz = float.Parse(IniFile.GetPrivateProfileString("camera", "z", "0.0"));
            float yaw = float.Parse(IniFile.GetPrivateProfileString("camera", "yaw", "0.0"));
            float pitch = float.Parse(IniFile.GetPrivateProfileString("camera", "pitch", "0.0"));
            _gameLoop.Camera = new FpsCamera("", cx, cy, cz, yaw, pitch);

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

                Entity entity = entities.Count > 0 ? entities[0] : null;
                _animatedModel.Update(deltaTime);
                Console.WriteLine(_animatedModel.AnimationTime);

                if (Keyboard.IsKeyDown(Key.D1)) entity.Roll(1);
                if (Keyboard.IsKeyDown(Key.D2)) entity.Roll(-1);
                if (Keyboard.IsKeyDown(Key.D3)) entity.Yaw(1);
                if (Keyboard.IsKeyDown(Key.D4)) entity.Yaw(-1);
                if (Keyboard.IsKeyDown(Key.D5)) entity.Pitch(1);
                if (Keyboard.IsKeyDown(Key.D6)) entity.Pitch(-1);

                Camera camera = _gameLoop.Camera;
                this.Text = $"{FramePerSecond.FPS}fps, t={FramePerSecond.GlobalTick} p={camera.Position}";
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
                foreach (Entity entity in entities)
                {
                    Renderer.Render(_shader, entity, camera);
                }

                Renderer.RenderAxis(_shader, camera);
            };
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Camera camera = _gameLoop.Camera;
            if (camera is FpsCamera) camera?.GoForward(0.02f * e.Delta);
            if (camera is OrbitCamera) (camera as OrbitCamera)?.FarAway(-0.005f * e.Delta);
        }

        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Mouse.CurrentPosition = new Vertex2i(e.X, e.Y);
            Camera camera = _gameLoop.Camera;
            Vertex2i delta = Mouse.DeltaPosition;
            camera?.Yaw(-delta.x);
            camera?.Pitch(-delta.y);
            Mouse.PrevPosition = new Vertex2i(e.X, e.Y);
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

        private void glControl1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (MessageBox.Show("정말로 끝내시겠습니까?", "종료", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { 
                    // 종료 설정 저장
                    IniFile.WritePrivateProfileString("camera", "x", _gameLoop.Camera.Position.x);
                    IniFile.WritePrivateProfileString("camera", "y", _gameLoop.Camera.Position.y);
                    IniFile.WritePrivateProfileString("camera", "z", _gameLoop.Camera.Position.z);
                    IniFile.WritePrivateProfileString("camera", "yaw", _gameLoop.Camera.CameraYaw);
                    IniFile.WritePrivateProfileString("camera", "pitch", _gameLoop.Camera.CameraPitch);
                    Application.Exit();
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                _polygonMode = (_polygonMode == PolygonMode.Fill) ?
                    PolygonMode.Line : PolygonMode.Fill;
            }
        }
    }
}
