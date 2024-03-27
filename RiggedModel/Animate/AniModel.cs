using Assimp;
using LSystem.Animate;
using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    class AniModel
    {
        private Entity _model;
        private Bone _rootBone;
        private int _jointCount;
        private Animator _animator;
        Matrix4x4f _rootBoneTransform;
        Matrix4x4f _bindShapeMatrix;
        XmlDae _xmlDae;

        public Bone GetBone(string boneName)
        {
            Bone bone = _rootBone;
            Stack<Bone> stack = new Stack<Bone>();
            stack.Push(bone);
            while (stack.Count > 0)
            {
                Bone b = stack.Pop();
                if (boneName == b.Name)
                {
                    return b;
                }
                foreach (Bone childBone in b.Childrens) stack.Push(childBone);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix4x4f BindShapeMatrix
        {
            get => _bindShapeMatrix;
            set => _bindShapeMatrix = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// 최상위 뼈의 포즈행렬을 가져온다.
        /// </summary>
        public Matrix4x4f RootBoneTransform => _rootBoneTransform;

        /// <summary>
        /// 
        /// </summary>
        public float MotionTime => _animator.MotionTime;

        /// <summary>
        /// 
        /// </summary>
        public Entity Entity => _model;

        /// <summary>
        /// 
        /// </summary>
        public Bone RootBone => _rootBone;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="xmlDae"></param>
        public AniModel(Entity model, XmlDae xmlDae)
        {
            _model = model;
            _xmlDae = xmlDae;
            _rootBone = xmlDae.RootBone;
            _jointCount = xmlDae.BoneCount;
            _animator = new Animator(this);
            _rootBoneTransform = xmlDae.RootMatirix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        public void SetMotion(string actionName)
        {
            Motion motion = _xmlDae.GetAnimation(actionName);
            if (motion == null) motion = _xmlDae.DefaultMotion;
            _animator.SetMotion(motion);
        }

        /// <summary>
        /// 업데이트를 통하여 애니메이션 행렬을 업데이트한다.
        /// </summary>
        /// <param name="deltaTime"></param>
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
                stack.Push(_rootBone.Childrens[0]);
                while(stack.Count > 0)
                {
                    Bone bone = stack.Pop();
                    if (bone.Index >= 0)
                        jointMatrices[bone.Index] = bone.AnimatedTransform * bone.InverseBindTransform;
                    foreach (Bone j in bone.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix4x4f AnimatedRootBone
        {
            get
            {
                Bone rbone = _rootBone.Childrens[0];
                return rbone.AnimatedTransform;
            }
        }

        /// <summary>
        /// * 애니매이션에서 뼈들의 뼈공간 ==> 캐릭터 공간으로의 변환 행렬<br/>
        /// * 뼈들의 포즈를 렌더링하기 위하여 사용할 수 있다.<br/>
        /// </summary>
        public Matrix4x4f[] BoneAnimationTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Bone> stack = new Stack<Bone>();
                stack.Push(_rootBone);

                while (stack.Count > 0)
                {
                    Bone bone = stack.Pop();
                    if (bone.Index >= 0)
                        jointMatrices[bone.Index] = bone.AnimatedTransform;
                    foreach (Bone j in bone.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }

        /// <summary>
        /// * 초기의 캐릭터공간에서의 바인드 포즈행렬을 가져온다. <br/>
        /// - 포즈행렬이란 한 뼈공간에서의 점의 상대적 좌표를 가져오는 행렬이다.<br/>
        /// </summary>
        public Matrix4x4f[] InverseBindPoseTransforms
        {
            get
            {
                Matrix4x4f[] jointMatrices = new Matrix4x4f[_jointCount];
                Stack<Bone> stack = new Stack<Bone>();
                stack.Push(_rootBone);
                while (stack.Count > 0)
                {
                    Bone bone = stack.Pop();
                    if (bone.Index >= 0)
                    {
                        jointMatrices[bone.Index] = bone.InverseBindTransform;
                    }
                    foreach (Bone j in bone.Childrens) stack.Push(j);
                }
                return jointMatrices;
            }
        }
    }
}
