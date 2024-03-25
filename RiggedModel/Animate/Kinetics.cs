using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem.Animate
{
    public class Kinetics
    {
        public static float angle = 0;

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
            Bone[] Lb = new Bone[N]; 
            Matrix4x4f[] Mb = new Matrix4x4f[N]; // 뼈 공간
            Matrix4x4f[] Mc = new Matrix4x4f[N]; // 캐릭터 공간

            // [초기값 설정] 캐릭터 공간 행렬과 뼈 공간 행렬을 만듦            
            for (int i = 0; i < N; i++)
            {
                Lb[i] = bones[i]; // 0번째가 말단뼈 --> ... --> N-1이 최상위 뼈
                Mb[i] = Lb[i].LocalAnimationTransform;
                Mc[i] = Lb[i].AnimatedTransform;
            }

            int iter = 0;
            Vertex3f T = Vertex3f.Zero;
            float err = float.MaxValue;

            while (err >= epsilon && iter < iternations)
            {                
                T = Mc[0].Column3.Vertex3f();
                err = (G - T).Norm();

                // 말단뼈부터 최상위 뼈까지 IK를 적용하여 뼈공간에 회전을 적용함
                for (int i = 1; i < N; i++)
                {
                    ApplyModifyBone(i);
                }

                ApplyModifyBone(1);

                T = Mc[0].Column3.Vertex3f();
                err = (G - T).Norm();
                Console.WriteLine($"{iter}회, 오차범위={err}");

                iter++;
            }

            //Console.WriteLine("----------------");

            T = Mc[0].Column3.Vertex3f(); // 0번째가 말단뼈
            return T;

            void ApplyModifyBone(int i)
            {
                Matrix4x4f Mq = GetRotationAngle(i, G, T);
                Mc[i] = Mc[i] * Mq;
                Lb[i].AnimatedTransform = Mc[i];

                // 적용한 뼈공간의 행렬로부터 캐릭터 공간의 행렬을 적용함.
                for (int k = i; k >= 0; k--)
                {
                    if (i == k)
                    {
                        Bone b = Lb[i].Parent;
                        Mc[k] = b.AnimatedTransform * Mb[k];
                        Lb[k].AnimatedTransform = Mc[k];
                    }
                    else
                    {
                        Mc[k] = Mc[k + 1] * Mb[k];
                        Lb[k].AnimatedTransform = Mc[k];
                    }
                }
            }

            Matrix4x4f GetRotationAngle(int i, Vertex3f grab, Vertex3f target, bool inverse = false)
            {
                Vertex3f Position = Mc[i].Column3.Vertex3f();
                Vertex3f g = grab - Position;
                Vertex3f t = target - Position;
                Vertex3f r = t.Cross(g);
                float tf = t.Norm();
                float gf = g.Norm();
                float ef = (t - g).Norm();

                float rNorm = r.Norm();
                if (rNorm <= 0.0f) return Matrix4x4f.Identity;

                float cos2 = (tf * tf + gf * gf - ef * ef) / (2 * tf * gf);
                float theta = 0.0f;
                if (-1 <= cos2 && cos2 <= 1)
                {
                    theta = ((float)Math.Acos(cos2)).ToDegree();
                }

                Quaternion q = inverse ? new Quaternion(r.Normalized, theta) : new Quaternion(r.Normalized, -theta);
                q.Normalize();
                Matrix4x4f Mq = ((Matrix4x4f)q);


                return Mq;
            }
        }
    }
}
