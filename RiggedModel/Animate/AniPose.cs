using System.Collections.Generic;
using System.Linq;

namespace LSystem
{
    public class AniPose
    {
        Dictionary<string, JointTransform> _pose;

        public AniPose()
        {
            _pose = new Dictionary<string, JointTransform>();
        }

        public JointTransform this[string jointName]
        {
            get => _pose[jointName];
            set => _pose[jointName] = value;
        }

        public string[] JointNames => _pose.Keys.ToArray();

    }
}
