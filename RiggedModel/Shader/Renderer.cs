using OpenGL;

namespace LSystem
{
    public class Renderer
    {
        public static RawModel3d Line = Loader3d.LoadLine(0, 0, 0, 1, 0, 0);
        public static RawModel3d Cube = Loader3d.LoadCube();
        public static RawModel3d Cone = Loader3d.LoadCone(4, 1.0f, 3.0f, false);
        public static RawModel3d Sphere = Loader3d.LoadSphere(r: 1, piece: 6);
        public static RawModel3d Rect = Loader3d.LoadPlane();

        public static void RenderAxis(StaticShader shader, Camera camera)
        {
            Gl.Disable(EnableCap.DepthTest);
            shader.Bind();

            shader.LoadProjMatrix(camera.ProjectiveMatrix);
            shader.LoadViewMatrix(camera.ViewMatrix);

            Gl.BindVertexArray(Line.VAO);
            Gl.EnableVertexAttribArray(0);

            shader.LoadIsTextured(false);

            // positive axis
            Gl.LineWidth(1.0f);
            shader.LoadModelMatrix(Matrix4x4f.Identity * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(1, 0, 0, 1)); // red
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            shader.LoadModelMatrix(Matrix4x4f.RotatedZ(90) * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(0, 1, 0, 1)); // green
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            shader.LoadModelMatrix(Matrix4x4f.RotatedY(-90) * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(0, 0, 1, 1)); // blue
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            // negative axis
            Gl.LineStipple(2, 0xAAAA);
            Gl.Enable(EnableCap.LineStipple);
            Gl.LineWidth(1.0f);

            shader.LoadModelMatrix(Matrix4x4f.RotatedY(180) * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(1, 0, 0, 1)); // red
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            shader.LoadModelMatrix(Matrix4x4f.RotatedZ(-90) * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(0, 1, 0, 1)); // green
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            shader.LoadModelMatrix(Matrix4x4f.RotatedY(90) * Matrix4x4f.Scaled(100, 100, 100));
            shader.LoadObjectColor(new Vertex4f(0, 0, 1, 1)); // blue
            Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
            Gl.Disable(EnableCap.LineStipple);

            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);

            shader.Unbind();
            Gl.Enable(EnableCap.DepthTest);
            shader.LoadIsTextured(true);

        }

        public void Render(AnimateShader shader, Matrix4x4f[] jointTransforms, Entity entity, Camera camera)
        {
            shader.Bind();

            shader.LoadModelMatrix(entity.ModelMatrix);
            shader.LoadViewMatrix(camera.ViewMatrix);
            shader.LoadProjMatrix(camera.ProjectiveMatrix);

            for (int i = 0; i < jointTransforms.Length; i++)
            {
                shader.PushBoneMatrix(i, jointTransforms[i]);
            }

            Gl.BindVertexArray(entity.Model.VAO);
            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);
            Gl.EnableVertexAttribArray(2);
            Gl.EnableVertexAttribArray(3);
            Gl.EnableVertexAttribArray(4);

            TexturedModel modelTextured = (TexturedModel)(entity.Model);

            if (modelTextured.Texture != null)
            {
                shader.LoadTexture("diffuseMap", TextureUnit.Texture0, modelTextured.Texture.TextureID);
            }

            Gl.DrawArrays(PrimitiveType.Triangles, 0, entity.Model.VertexCount);

            Gl.DisableVertexAttribArray(0);
            Gl.DisableVertexAttribArray(1);
            Gl.DisableVertexAttribArray(2);
            Gl.DisableVertexAttribArray(3);
            Gl.DisableVertexAttribArray(4);
            Gl.BindVertexArray(0);

            shader.Unbind();
        }

        public static void Render(StaticShader shader, Entity entity, Camera camera)
        {
            Gl.Enable(EnableCap.Blend);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader.Bind();

            Gl.BindVertexArray(entity.Model.VAO);
            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);
            Gl.EnableVertexAttribArray(2);

            if (entity.IsTextured)
            {
                shader.LoadIsTextured(true);
                TexturedModel modelTextured = entity.Model as TexturedModel;
                shader.SetInt("modelTexture", 0);
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.Texture2d, modelTextured.Texture.TextureID);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            }

            shader.LoadObjectColor(entity.Material.Ambient);
            shader.LoadProjMatrix(camera.ProjectiveMatrix);
            shader.LoadViewMatrix(camera.ViewMatrix);
            shader.LoadModelMatrix(entity.ModelMatrix);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, entity.Model.VertexCount);

            Gl.DisableVertexAttribArray(2);
            Gl.DisableVertexAttribArray(1);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);

            if (entity.IsAxisVisible)
            {
                Gl.BindVertexArray(Renderer.Line.VAO);
                Gl.EnableVertexAttribArray(0);
                shader.LoadIsTextured(false);
                shader.LoadProjMatrix(camera.ProjectiveMatrix);
                shader.LoadViewMatrix(camera.ViewMatrix);

                shader.LoadObjectColor(new Vertex4f(1, 0, 0, 1));
                shader.LoadModelMatrix(entity.ModelMatrix * Matrix4x4f.Scaled(3, 3, 3));
                Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

                shader.LoadObjectColor(new Vertex4f(0, 1, 0, 1));
                shader.LoadModelMatrix(entity.ModelMatrix * Matrix4x4f.RotatedZ(90) * Matrix4x4f.Scaled(3, 3, 3));
                Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

                shader.LoadObjectColor(new Vertex4f(0, 0, 1, 1));
                shader.LoadModelMatrix(entity.ModelMatrix * Matrix4x4f.RotatedY(-90) * Matrix4x4f.Scaled(3, 3, 3));
                Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

                Gl.DisableVertexAttribArray(0);
                Gl.BindVertexArray(0);
            }

            shader.Unbind();
        }

    }
}
