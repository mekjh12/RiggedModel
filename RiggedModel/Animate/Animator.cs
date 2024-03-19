using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    class Animator
    {
        AniModel _animatedModel;
        Animation _currentAnimation;
        float _animationTime = 0.0f;
        bool _isPlaying = false;

        public Animation CurrentAnimation => _currentAnimation;

        public float AnimationTime => _animationTime;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="entity"></param>
        public Animator(AniModel animatedModel)
        {
            _animatedModel = animatedModel;
        }

        /// <summary>
        /// 애니메이션을 지정한다.
        /// </summary>
        /// <param name="animation"></param>
        public void SetAnimation(Animation animation)
        {
            _animationTime = 0;
            _currentAnimation = animation;
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Toggle()
        {
            _isPlaying  = !_isPlaying;
        }

        public void Update(float deltaTime)
        {
            if (_currentAnimation == null) return;

            // 애니메이션 시간을 업데이트한다.
            if (_isPlaying)
            {
                _animationTime += deltaTime;
                _animationTime = _animationTime % _currentAnimation.Length;
            }

            // 키프레임으로부터 현재의 로컬포즈행렬을 가져온다.(bone name, mat4x4f)
            Dictionary<string, Matrix4x4f> currentPose = this.CalculateCurrentAnimationPose();

            // 로컬 포즈행렬로부터 캐릭터공간의 포즈행렬을 얻는다.
            Stack<Bone> stack = new Stack<Bone>();
            Stack<Matrix4x4f> mStack = new Stack<Matrix4x4f>();
            stack.Push(_animatedModel.RootBone);
            mStack.Push(Matrix4x4f.Identity);
            while (stack.Count > 0)
            {
                Bone joint = stack.Pop();
                Matrix4x4f parentTransform = mStack.Pop();

                Matrix4x4f boneLocalTransform = (currentPose.ContainsKey(joint.Name)) ?
                    currentPose[joint.Name] : joint.BindTransform; // 로컬포즈행렬이 없으면 기본바인딩행렬로 가져온다.

                joint.AnimatedTransform = parentTransform * boneLocalTransform; // 순서는 자식부터  v' = ... P2 P1 L v

                foreach (Bone childJoint in joint.Childrens) // 순회를 위한 스택 입력
                {
                    stack.Push(childJoint);
                    mStack.Push(joint.AnimatedTransform);
                }
            }
        }

        /// <summary>
        /// * 현재 시간의 애니메이션포즈를 가져온다. <br/>
        /// * 반환값의 딕셔너리는 jointName, Matrix4x4f이다.<br/>
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Matrix4x4f> CalculateCurrentAnimationPose()
        {
            // 현재 시간에서 가장 근접한 사이의 두 개의 프레임을 가져온다.
            KeyFrame previousFrame = _currentAnimation.FirstFrame; 
            KeyFrame nextFrame = _currentAnimation.FirstFrame;
            float firstTime = _currentAnimation.FirstFrame.TimeStamp;
            for (int i = 1; i < _currentAnimation.KeyFrameCount; i++)
            {
                nextFrame = _currentAnimation.Frame(i);
                if (nextFrame.TimeStamp >= _animationTime - firstTime)
                {
                    break;
                }
                previousFrame = _currentAnimation.Frame(i);
            }

            // 현재 진행률을 계산한다.
            float totalTime = nextFrame.TimeStamp - previousFrame.TimeStamp;
            float currentTime = _animationTime - previousFrame.TimeStamp;
            float progression = currentTime / totalTime;

            // 두 키프레임 사이의 보간된 포즈를 딕셔러리로 가져온다.
            Dictionary<string, Matrix4x4f> currentPose = new Dictionary<string, Matrix4x4f>();
            foreach (string jointName in previousFrame.Pose.JointNames)
            {
                BonePose previousTransform = previousFrame[jointName];
                BonePose nextTransform = nextFrame[jointName];
                BonePose currentTransform = BonePose.InterpolateSlerp(previousTransform, nextTransform, progression);
                currentPose[jointName] = currentTransform.LocalTransform;
            }

            return currentPose;
        }

    }
}
