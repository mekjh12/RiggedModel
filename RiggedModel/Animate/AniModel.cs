using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace LSystem
{
    class AniModel
    {
        private Entity _model;
        private Bone _rootJoint;
        private int _jointCount;
        private Animator _animator;
        Matrix4x4f _rootBoneTransform;
        Matrix4x4f bind_shape_matrix;

        public Matrix4x4f BindShapeMatrix
        {
            get => bind_shape_matrix;
            set => bind_shape_matrix = value;
        }

        public Animator Animator => _animator;

        /// <summary>
        /// 최상위 뼈의 포즈행렬을 가져온다.
        /// </summary>
        public Matrix4x4f RootBoneTransform => _rootBoneTransform;

        public float AnimationTime => _animator.AnimationTime;

        public Entity ModelEntity => _model;

        public Bone RootBone => _rootJoint;

        public AniModel(Entity model, Bone rootJoint, int jointCount, Matrix4x4f rootBoneTransform)
        {
            _model = model;
            _rootJoint = rootJoint;
            _jointCount = jointCount;
            _animator = new Animator(this);
            _rootBoneTransform = rootBoneTransform;
        }

        public void DoAnimation(Animation animation)
        {
            _animator.SetAnimation(animation);
        }

        public void Update(int deltaTime)
        {
            _animator.Update(0.001f * deltaTime);
        }

        /// <summary>
        /// * 캐릭터 공간에서의 애니메이션을 포즈행렬을 최종적으로 가져온다.<br/>
        /// * v' = Ma(i) Md^-1(i) v (Ma 애니메이션행렬, Md 바이딩포즈행렬)<br/>
        /// * 정점들을 바인딩포즈행렬을 이용하여 뼈 공간으로 정점을 변환 후, 애니메이션 행렬을 이용하여 뼈의 캐릭터 공간으로의 변환행렬을 가져온다.<br/>
        /// </summary>
        public Matrix4x4f[] BoneAnimationBindTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Bone> stack = new Stack<Bone>();
                stack.Push(_rootJoint.Childrens[0]);
                while(stack.Count > 0)
                {
                    Bone joint = stack.Pop();
                    if (joint.Index >= 0)
                    {
                        jointMatrices[joint.Index] = joint.AnimatedTransform * joint.InverseBindTransform;
                    }
                    foreach (Bone j in joint.Childrens) stack.Push(j);
                }

                // 이유를 찾지 못하였지만 최상위 hipBone의 수정이 필요하여~!
                Bone rbone = _rootJoint.Childrens[0];
                jointMatrices[0] = rbone.AnimatedTransform * rbone.InverseBindTransform;
                
                return jointMatrices;
            }
        }

        public Matrix4x4f AnimatedRootBone
        {
            get
            {
                Bone rbone = _rootJoint.Childrens[0];
                return rbone.AnimatedTransform;
            }
        }

        /// <summary>
        /// * 뼈들의 캐릭터 공간에서의 애니메이션 포즈행렬을 가져온다.<br/>
        /// * 뼈들의 포즈를 렌더링하기 위하여 사용할 수 있다.<br/>
        /// </summary>
        public Matrix4x4f[] BoneAnimationTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Bone> stack = new Stack<Bone>();
                stack.Push(_rootJoint);
                while (stack.Count > 0)
                {
                    Bone joint = stack.Pop();
                    if (joint.Index >= 0)
                    {
                        jointMatrices[joint.Index] = joint.AnimatedTransform;
                    }
                    foreach (Bone j in joint.Childrens) stack.Push(j);
                }

                // 이유를 찾지 못하였지만 최상위 hipBone의 수정이 필요하여~!
                Bone rbone = _rootJoint.Childrens[0];
                jointMatrices[0] = rbone.AnimatedTransform;

                return jointMatrices;
            }
        }

        /// <summary>
        /// * 초기의 캐릭터공간에서의 바인드 포즈행렬을 가져온다. <br/>
        /// * 포즈행렬이란 한 뼈공간에서의 점의 상대적 좌표를 가져오는 행렬이다.<br/>
        /// </summary>
        public Matrix4x4f[] BindPoseTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Bone> stack = new Stack<Bone>();
                stack.Push(_rootJoint);
                while (stack.Count > 0)
                {
                    Bone bone = stack.Pop();
                    jointMatrices[bone.Index] = bone.InverseBindTransform;
                    foreach (Bone j in bone.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }
    }
}
