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
    public static class Kinetics
    {
        #region 벡터연산 구현 부분
        public static Vertex3f Cross(this Vertex3f a, Vertex3f b)
        {
            //외적의 방향은 왼손으로 감는다.
            return new Vertex3f(a.y * b.z - a.z * b.y, -a.x * b.z + a.z * b.x, a.x * b.y - a.y * b.x);
        }

        public static float Norm(this Vertex3f vec)
        {
            return (float)Math.Sqrt(vec.Dot(vec));
        }

        public static Vertex3f Vertex3f(this Vertex4f vec)
        {
            return new Vertex3f(vec.x, vec.y, vec.z);
        }
        #endregion

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grabTarget"></param>
        /// <param name="bone"></param>
        /// <param name="chainLength"></param>
        /// <param name="iternations"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vertex3f[] IKSolved(Vertex3f grabTarget, Bone bone, int chainLength = 2, int iternations = 10, float epsilon = 0.05f)
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
            for (int i = 0; i < N; i++)
            {
                Bn[i] = bones[i];
            }

            // 반복횟수와 오차범위안에서 반복하여 최적의 해를 찾는다.
            int iter = 0;
            float err = float.MaxValue;

            while(iter < iternations &&  err > epsilon)
            {
                // 최말단뼈부터 시작하여 최상위 뼈까지 회전을 적용한다.
                for (int i = 0; i < N; i++)
                {
                    Vertex3f T = Bn[0].AnimatedTransform.Column3.Vertex3f();
                    err = (T - G).Norm();
                    Rotate(G, Bn[i], T);
                }

                // 최종적으로 최말단뼈의 회전을 적용한다.
                Vertex3f T0 = Bn[0].AnimatedTransform.Column3.Vertex3f();
                Rotate(G, Bn[0], T0);

                iter++;
            }

            Console.WriteLine($"{iter}회 에러={err}");
            List<Vertex3f> vertices = new List<Vertex3f>();
            vertices.Add(bone.AnimatedTransform.Column3.Vertex3f());
            return vertices.ToArray();
        }


        public static Vertex3f[] IKSolvedInv(Vertex3f grabTarget, Bone bone, int chainLength = 2, int iternations = 10, float epsilon = 0.05f)
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
            for (int i = 0; i < N; i++)
            {
                Bn[i] = bones[i];
            }

            // 반복횟수와 오차범위안에서 반복하여 최적의 해를 찾는다.
            int iter = 0;
            float err = float.MaxValue;

            while (iter < iternations && err > epsilon)
            {
                // 최말단뼈부터 시작하여 최상위 뼈까지 회전을 적용한다.
                for (int i = N - 1; i >= 0; i--)
                {
                    Vertex3f T = Bn[0].AnimatedTransform.Column3.Vertex3f();
                    err = (T - G).Norm();
                    Rotate(G, Bn[i], T);
                }


                iter++;
            }

            Console.WriteLine($"{iter}회 에러={err}");
            List<Vertex3f> vertices = new List<Vertex3f>();
            vertices.Add(bone.AnimatedTransform.Column3.Vertex3f());
            return vertices.ToArray();
        }

    }
}
