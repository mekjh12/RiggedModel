using OpenGL;

namespace LSystem
{
    public class BoneWeightShader : ShaderProgram
    {
        private static int MAX_JOINTS = 128;
        const string VERTEX_FILE = @"\Shader\bw.vert";
        const string FRAGMENT_FILE = @"\Shader\bw.frag";

        public BoneWeightShader() : base(EngineLoop.PROJECT_PATH + VERTEX_FILE, EngineLoop.PROJECT_PATH + FRAGMENT_FILE, "")
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
            UniformLocations("model", "view", "proj", "boneIndex");
        }

        public void LoadTexture(string textureUniformName, TextureUnit textureUnit, uint texture)
        {
            base.LoadInt(_location[textureUniformName], textureUnit - TextureUnit.Texture0);
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(TextureTarget.Texture2d, texture);
        }

        public void LoadBoneIndex(int boneIndex)
        {
            base.LoadInt(_location["boneIndex"], boneIndex);
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
