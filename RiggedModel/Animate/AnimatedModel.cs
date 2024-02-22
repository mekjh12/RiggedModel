using OpenGL;
using System.Drawing.Drawing2D;

namespace LSystem
{
    class AnimatedModel
    {
        private Entity _model;
        private Joint _rootJoint;
        private int _jointCount;
        private Animator _animator;
        private float velocity = 1.0f;

        public float AnimationTime => _animator.AnimationTime;

        public Joint RootJoint
        {
            get =>_rootJoint;
        }

        public AnimatedModel(Entity model, Joint rootJoint, int jointCount)
        {
            _model = model;
            _rootJoint = rootJoint;
            _jointCount = jointCount;
            _animator = new Animator(this);
            //rootJoint.CalcInverseBindTransform(Matrix4x4f.Identity);
        }

        public void Delete()
        {
            //model.delete();
            //texture.delete();
        }

        public void DoAnimation(Animation animation)
        {
            _animator.SetAnimation(animation);
        }

        public void Update(int deltaTime)
        {
            // 렌더링
            float movedDistance = 0.001f * velocity * (float)deltaTime;
            //if (movedDistance>0) model.GoForward(movedDistance);

            float px = _model.Position.x;
            float pz = _model.Position.y;

            _model.Position = new Vertex3f(px, 0, pz);

            _animator.Update(0.001f * deltaTime);
        }

        public Matrix4x4f[] JointTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                this.AddJointsToArray(_rootJoint, jointMatrices);
                return jointMatrices;
            }
        }

        private void AddJointsToArray(Joint headJoint, Matrix4x4f[] jointMatrices)
        {
            jointMatrices[headJoint.Index] = headJoint.AnimatedTransform;
            foreach (Joint childJoint in headJoint.Childrens)
            {
                this.AddJointsToArray(childJoint, jointMatrices);
            }
        }
    }
}
