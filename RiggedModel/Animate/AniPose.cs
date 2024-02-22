using System.Collections.Generic;
using System.Linq;

namespace LSystem
{
    public class AniPose
    {
        private Dictionary<string, JointTransform> pose;

        public AniPose()
        {
            this.pose = new Dictionary<string, JointTransform>();
        }

        public JointTransform GetJointTransform(int index)
        {
            int num = 0;
            foreach (KeyValuePair<string, JointTransform> item in pose)
            {
                if (index == num) return item.Value;
                num++;
            }
            return null;
        }

        public JointTransform this[string jointName]
        {
            get
            {
                return pose[jointName];
            }

            set
            {
                pose[jointName] = value;
            }
        }

        public string[] JointNames
        {
            get
            {
                return pose.Keys.ToArray();
            }
        }


    }
}
