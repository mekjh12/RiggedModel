using System.Collections.Generic;
using System.Linq;

namespace LSystem
{
    public class Motion
    {
        private string _animationName;
        private float _length;
        private Dictionary<float, KeyFrame> _keyframes;

        public KeyFrame FirstKeyFrame => (_keyframes.Values.Count > 0) ? _keyframes.Values.ElementAt(0) : null;

        public float Length => _length;

        public string Name => _animationName;

        public int KeyFrameCount => _keyframes.Count;

        public Motion(string name, float lengthInSeconds)
        {
            _animationName = name;
            _length = lengthInSeconds;
            _keyframes = new Dictionary<float, KeyFrame>();
        }

        public KeyFrame this[float time]
        {
            get
            {
                if (_keyframes.ContainsKey(time))
                {
                    return (_keyframes.Values.Count > 0) ? _keyframes[time] : null;
                }
                else
                {
                    return FirstKeyFrame;
                }
            }
        }

        public KeyFrame FirstFrame => _keyframes.Values.ElementAt(0);

        public KeyFrame Frame(int index)
        {
            return _keyframes.Values.ElementAt(index);
        }

        public void AddKeyFrame(float time)
        {
            if (!_keyframes.ContainsKey(time))
            {
                _keyframes[time] = new KeyFrame(time);
            }
        }

    }
}
