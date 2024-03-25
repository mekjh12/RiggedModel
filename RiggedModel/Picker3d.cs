using OpenGL;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using LSystem;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LSystem
{
    class Picker3d
    {
        /// <summary>
        /// * Depth Buffer를 이용하여 p-> projInverse -> p/p.w -> viewInverse -> p <br/>
        /// * 얻는 방법이나 깊이버퍼에 기록되는 부동소숫점의 영향으로 깊이를 정밀하게 반영하지 못한다.<br/>
        /// * 가까운 물체에서는 어느정도 작동하나 조금 멀리 떨어진 곳에서는 작동이 잘 되지 않는다.<br/>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vertex3f PickUpPoint(Camera camera, int px, int py, int width, int height)
        {
            float x = (px - ((float)width / 2.0f)) / ((float)width / 2.0f);
            float y = (((float)height / 2.0f) - py) / ((float)height / 2.0f);

            Console.WriteLine($"picker click ({x}, {y})");
            Matrix4x4f proj = camera.ProjectiveMatrix;
            Matrix4x4f view = camera.ViewMatrix;
            Vertex4f at = GetWorldLocation(new Vertex4f(x, y, 0.99f, 1.0f), proj, view);
            Vertex4f eye = GetWorldLocation(new Vertex4f(x, y, 0.1f, 1.0f), proj, view);
            Vertex3f f = (Vertex3f)(at - eye).Normalized;
            Vertex3f p = (Vertex3f)eye;
            float t = -p.y / f.y;
            return new Vertex3f(p.x + t * f.x, 0.0f, p.z + t * f.z);

        }

        private static Vertex4f GetWorldLocation(Vertex4f screenCoord, Matrix4x4f proj, Matrix4x4f view)
        {
            Vertex4f projCoord = proj.Inverse * screenCoord;
            Vertex4f viewCoord = new Vertex4f(projCoord.x / projCoord.w, projCoord.y / projCoord.w, projCoord.z / projCoord.w, 1.0f);
            Vertex4f worldCoord = view.Inverse * viewCoord;
            return worldCoord;
        }

    }
}
