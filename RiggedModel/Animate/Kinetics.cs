using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace LSystem.Animate
{
    public class Kinetics
    {

        private static void Rotate(Vertex3f grabTarget, Bone bone, Vertex3f endTarget)
        {
            Vertex3f G = grabTarget;
            Vertex3f T = endTarget; // bone.AnimatedTransform.Column3.Vertex3f();
            Vertex3f P = bone.Parent.AnimatedTransform.Column3.Vertex3f();
            Vertex3f t = T - P;
            Vertex3f g = G - P;
            Vertex3f r = t.Cross(g).Normalized;
            float tf = t.Norm();
            float gf = g.Norm();
            float ef = (t - g).Norm();
            float cos2 = (tf * tf + gf * gf - ef * ef) / (2 * tf * gf);
            float theta = (-1 <= cos2 && cos2 <= 1) ? ((float)Math.Acos(cos2)).ToDegree() : 0.0f;

            Quaternion q = new Quaternion(r, theta);
            q.Normalize();
            Matrix4x4f Mq = ((Matrix4x4f)q);

            Bone Parent = bone.Parent;

            Vertex4f p = Parent.AnimatedTransform.Column3;

            Parent.AnimatedTransform = Mq * Parent.AnimatedTransform;
            Matrix4x4f m = Matrix4x4f.Translated(p.x, p.y, p.z) * Parent.AnimatedTransform;
            m[3, 0] = p.x;
            m[3, 1] = p.y;
            m[3, 2] = p.z;
            Parent.AnimatedTransform = m;

            Stack<Bone> stack = new Stack<Bone>();
            foreach (Bone child in Parent.Childrens) stack.Push(child);
            while (stack.Count > 0)
            {
                Bone b = stack.Pop();
                b.AnimatedTransform = b.Parent.AnimatedTransform * b.LocalAnimationTransform;
                foreach (Bone child in b.Childrens) stack.Push(child);
            }
        }

        public static Vertex3f[] IKSolved2(Vertex3f grabTarget, Bone bone, int chainLength = 2, int iternations = 10, float epsilon = 0.01f)
        {
            Vertex3f G = grabTarget;

            // 말단뼈로부터 최상위 뼈까지 리스트를 만들고 Chain Length를 구함.
            List<Bone> bones = new List<Bone>();
            Bone parent = bone;
            bones.Add(parent);
            while (parent.Parent != null)
            {
                bones.Add(parent.Parent);
                parent = parent.Parent;
            }

            // 가능한 Chain Length
            int rootChainLength = bones.Count;
            int N = Math.Min(chainLength, rootChainLength);

            // 뼈의 리스트 (말단의 뼈로부터 최상위 뼈로의 순서로)
            // 0번째가 말단뼈 --> ... --> N-1이 최상위 뼈
            Bone[] Bn = new Bone[N];

            // [초기값 설정] 캐릭터 공간 행렬과 뼈 공간 행렬을 만듦 
            for (int i = 0; i < N; i++) Bn[i] = bones[i];

            int iter = 0;

            while(iter < iternations)
            {
                for (int i = 0; i < N; i++)
                {
                    Vertex3f T = Bn[0].AnimatedTransform.Column3.Vertex3f();
                    Rotate(G, Bn[i], T);
                }

                Vertex3f T0 = Bn[0].AnimatedTransform.Column3.Vertex3f();
                Rotate(G, Bn[0], T0);

                iter++;
            }

            List<Vertex3f> list = new List<Vertex3f>();
            list.Add(bone.AnimatedTransform.Column3.Vertex3f());
            return list.ToArray();
        }

        public static Vertex3f IKSolved(Vertex3f grabTarget, Bone bone, int chainLength = 2, int iternations = 10, float epsilon = 0.01f)
        {
            Vertex3f G = grabTarget;

            // 말단뼈로부터 최상위 뼈까지 리스트를 만들고 Chain Length를 구함.
            List<Bone> bones = new List<Bone>();
            Bone parent = bone;
            bones.Add(parent);
            while ( parent.Parent != null)
            {
                bones.Add(parent.Parent);
                parent = parent.Parent;
            }

            // 가능한 Chain Length
            int rootChainLength = bones.Count;
            int N = Math.Min(chainLength, rootChainLength);

            // 뼈의 리스트 (말단의 뼈로부터 최상위 뼈로의 순서로)
            // 0번째가 말단뼈 --> ... --> N-1이 최상위 뼈
            Bone[] Bn = new Bone[N]; 

            // [초기값 설정] 캐릭터 공간 행렬과 뼈 공간 행렬을 만듦 
            for (int i = 0; i < N; i++) Bn[i] = bones[i];

            int iter = 0;
            Vertex3f T = Vertex3f.Zero;
            float err = float.MaxValue;

            for (int i = 0; i< N; i++)
            {
                T = Bn[0].AnimatedTransform.Column3.Vertex3f();

            }

            return Vertex3f.Zero;
        }
    }
}
