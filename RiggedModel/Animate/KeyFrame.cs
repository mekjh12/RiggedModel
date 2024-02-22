namespace LSystem
{
    public class KeyFrame
    {
        private AniPose _pose;
        private float _timeStamp;

        public KeyFrame(float timeStamp)
        {
            _timeStamp = timeStamp;
            _pose = new AniPose();
        }

        public AniPose Pose
        {
            get => _pose;
        }

        public float TimeStamp
        {
            get => _timeStamp;
        }

        public JointTransform this[string jointName]
        {
            get => _pose[jointName];
            set => _pose[jointName] = value;
        }


        public void AddJointTransform(string jointName, JointTransform jointTransform)
        {
            _pose[jointName] = jointTransform;
        }

    }
}
