using OpenGL;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace LSystem
{
    class AniModel
    {
        private Entity _model;
        private Joint _rootJoint;
        private int _jointCount;
        private Animator _animator;
        Matrix4x4f _rootMatrix4x4;

        public Animator Animator => _animator;

        public Matrix4x4f RootMatrix4x4f => _rootMatrix4x4;

        public float AnimationTime => _animator.AnimationTime;

        public Entity ModelEntity => _model;

        public Joint RootJoint
        {
            get =>_rootJoint;
        }

        public AniModel(Entity model, Joint rootJoint, int jointCount, Matrix4x4f rootMatrix4x4)
        {
            _model = model;
            _rootJoint = rootJoint;
            _jointCount = jointCount;
            _animator = new Animator(this);
            _rootMatrix4x4 = rootMatrix4x4;
            //_rootJoint.CalcInverseBindTransform(Matrix4x4f.Identity);
        }

        public void DoAnimation(Animation animation)
        {
            _animator.SetAnimation(animation);
        }

        public void Update(int deltaTime)
        {
            _animator.Update(0.001f * deltaTime, RootMatrix4x4f);
        }

        public Matrix4x4f[] JointTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Joint> stack = new Stack<Joint>();
                stack.Push(_rootJoint);
                while(stack.Count > 0)
                {
                    Joint joint = stack.Pop();
                    jointMatrices[joint.Index] = joint.AnimatedTransform * joint.InverseBindTransform;
                    foreach (Joint j in joint.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }

        public Matrix4x4f[] BindPoseTransform
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Joint> stack = new Stack<Joint>();
                stack.Push(_rootJoint);
                while (stack.Count > 0)
                {
                    Joint joint = stack.Pop();
                    jointMatrices[joint.Index] = joint.InverseBindTransform.Inverse;
                    foreach (Joint j in joint.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }
    }
}
