using System.Collections.Generic;
using System.Linq;

namespace LSystem
{
    public class Animation
    {
        private string _name;
        private float _length;
        private Dictionary<float, KeyFrame> _keyframes;

        public Animation(string name, float lengthInSeconds)
        {
            _name = name;
            _length = lengthInSeconds;
            _keyframes = new Dictionary<float, KeyFrame>();
        }

        public KeyFrame FirstKeyFrame
        {
            get=> (_keyframes.Values.Count > 0) ? _keyframes.Values.ElementAt(0) : null;
        }

        public float Length => _length;

        public string Name => _name;

        public int KeyFrameCount => _keyframes.Count;

        public KeyFrame this[float time]
        {
            get
            {
                if (!_keyframes.ContainsKey(time))
                {
                    return FirstKeyFrame;
                }
                else
                {
                    return (_keyframes.Values.Count > 0) ? _keyframes[time] : null;
                }
            }
        }

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

        public void AddKeyFrames(float[] times)
        {
            for (int i = 0; i < times.Length; i++)
            {
                AddKeyFrame(times[i]);
            }
        }

    }
}
