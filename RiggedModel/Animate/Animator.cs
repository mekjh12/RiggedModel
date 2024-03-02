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

        public void Update(float deltaTime, Matrix4x4f rootMatrix)
        {
            if (_currentAnimation == null) return;

            // 애니메이션 시간을 업데이트한다.
            if (_isPlaying)
            {
                _animationTime += deltaTime;
                _animationTime = _animationTime % _currentAnimation.Length;
            }

            // 현재포즈를 가져온다.(bone name, mat4x4f)
            Dictionary<string, Matrix4x4f> currentPose = this.CalculateCurrentAnimationPose();

            // 
            Stack<Joint> stack = new Stack<Joint>();
            Stack<Matrix4x4f> mStack = new Stack<Matrix4x4f>();
            stack.Push(_animatedModel.RootJoint);
            mStack.Push(rootMatrix.Inverse);
            while (stack.Count > 0)
            {
                Joint joint = stack.Pop();
                Matrix4x4f parentTransform = mStack.Pop();

                Matrix4x4f currentLocalTransform = (currentPose.ContainsKey(joint.Name)) ?
                    currentPose[joint.Name] : joint.BindTransform;

                // 순서는 앞쪽부터 부모공간부터 변환이 되어야 한다. 
                joint.AnimatedTransform = parentTransform * currentLocalTransform;

                foreach (Joint childJoint in joint.Childrens) // 순회를 위한 스택 입력
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
                if (nextFrame.TimeStamp >= _animationTime)
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
                JointTransform previousTransform = previousFrame[jointName];
                JointTransform nextTransform = nextFrame[jointName];
                JointTransform currentTransform = JointTransform.InterpolateSlerp(previousTransform, nextTransform, progression);
                currentPose[jointName] = currentTransform.LocalTransform;
            }

            return currentPose;
        }

    }
}
