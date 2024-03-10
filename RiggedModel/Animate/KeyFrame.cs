namespace LSystem
{
    public class KeyFrame
    {
        private ArmaturePose _pose;
        private float _timeStamp;

        public KeyFrame(float timeStamp)
        {
            _timeStamp = timeStamp;
            _pose = new ArmaturePose();
        }

        public ArmaturePose Pose => _pose;

        public float TimeStamp => _timeStamp;

        public BonePose this[string jointName]
        {
            get => _pose[jointName];
            set => _pose[jointName] = value;
        }

        public void AddBoneTransform(string jointName, BonePose jointTransform)
        {
            _pose[jointName] = jointTransform;
        }

    }
}
