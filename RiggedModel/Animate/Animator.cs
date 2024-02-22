using OpenGL;
using System.Collections.Generic;

namespace LSystem
{
    class Animator
    {
        private AnimatedModel animatedModel;
        private Animation currentAnimation;
        private float animationTime = 0.0f;

        public float AnimationTime => animationTime;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="entity"></param>
        public Animator(AnimatedModel animatedModel)
        {
            this.animatedModel = animatedModel;
        }

        /// <summary>
        /// 새로운 애니메이션을 실행하기 위하여 설정함
        /// </summary>
        /// <param name="animation"></param>
        public void SetAnimation(Animation animation)
        {
            this.animationTime = 0;
            this.currentAnimation = animation;
        }

        public void Update(float deltaTime)
        {
            if (currentAnimation == null) return;
            animationTime += deltaTime;
            animationTime = animationTime % currentAnimation.Length;
            Dictionary<string, Matrix4x4f> currentPose = this.CalculateCurrentAnimationPose();
            this.ApplyPoseToJoints(currentPose, animatedModel.RootJoint, Matrix4x4f.Identity);
        }

        private void ApplyPoseToJoints(Dictionary<string, Matrix4x4f> currentPose, Joint joint, Matrix4x4f parentTransform)
        {
            Matrix4x4f currentLocalTransform;

            // 만약 키가 존재하지 않으면 바인딩의 기본키를 사용한다.
            if (currentPose.ContainsKey(joint.Name))
            {
                currentLocalTransform = currentPose[joint.Name];
            }
            else
            {
                currentLocalTransform = joint.LocalBindTransform;
            }

            // 순서는 앞쪽부터 부모공간부터 변환이 되어야 한다. 
            Matrix4x4f currentTransform = currentLocalTransform * parentTransform;
            joint.AnimatedTransform = joint.BindTransform.Inverse * currentTransform;

            // 하위 노드 탐색을 위한 재귀 호출
            foreach (Joint childJoint in joint.Childrens)
                ApplyPoseToJoints(currentPose, childJoint, currentTransform);
        }

        /// <summary>
        /// 현재 시간의 애니메이션포즈를 가져온다. 딕셔너리는 jointName, Matrix4x4f이다.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Matrix4x4f> CalculateCurrentAnimationPose()
        {
            KeyFrame[] frames = this.GetPreviousAndNextFrames();
            float progression = this.CalculateProgression(frames[0], frames[1]);
            return this.InterpolatePoses(frames[0], frames[1], progression);
        }

        /// <summary>
        /// 현재 시간에서 가장 근접한 사이의 두 개의 프레임을 가져온다.
        /// </summary>
        /// <returns></returns>
        private KeyFrame[] GetPreviousAndNextFrames()
        {
            KeyFrame previousFrame = currentAnimation.Frame(0);
            KeyFrame nextFrame = currentAnimation.Frame(0);

            for (int i = 1; i < currentAnimation.KeyFrameCount; i++)
            {
                nextFrame = currentAnimation.Frame(i);
                if (nextFrame.TimeStamp >= animationTime)
                {
                    break;
                }
                previousFrame = currentAnimation.Frame(i);
            }
            return new KeyFrame[] { previousFrame, nextFrame };
        }

        /// <summary>
        /// 두 키프레임 사이의 진행률을 1로 보았을 때 현재 시간의 0과 1사이의 시간 값을 가져온다.
        /// </summary>
        /// <param name="previousFrame"></param>
        /// <param name="nextFrame"></param>
        /// <returns></returns>
        private float CalculateProgression(KeyFrame previousFrame, KeyFrame nextFrame)
        {
            float totalTime = nextFrame.TimeStamp - previousFrame.TimeStamp;
            float currentTime = animationTime - previousFrame.TimeStamp;
            return currentTime / totalTime;
        }

        /// <summary>
        /// 두 키프레임 사이의 보간된 포즈를 딕셔러리로 가져온다.
        /// jointName, Matrix4x4f 딕셔려리를 반환한다.
        /// </summary>
        /// <param name="previousFrame"></param>
        /// <param name="nextFrame"></param>
        /// <param name="progression"></param>
        /// <returns></returns>
        private Dictionary<string, Matrix4x4f> InterpolatePoses(KeyFrame previousFrame, KeyFrame nextFrame, float progression)
        {
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
