using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    class Joint
    {
        private int _index;
        private string _name;
        private List<Joint> _children = new List<Joint>();
        private Matrix4x4f _animatedTransform = Matrix4x4f.Identity;
        private Matrix4x4f _localBindTransform = Matrix4x4f.Identity;
        private Matrix4x4f _bindTransform = Matrix4x4f.Identity;

        public int Index => _index;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public List<Joint> Childrens => _children;

        public Matrix4x4f AnimatedTransform 
        {
            get => _animatedTransform;
            set => _animatedTransform = value;
        }

        public Matrix4x4f BindTransform
        {
            get => _bindTransform;
            set => _bindTransform = value;
        }

        public Matrix4x4f LocalBindTransform
        {
            get => _localBindTransform;
            set => _localBindTransform = value;
        }

        public Joint(string name, int index, Matrix4x4f bindTransform)
        {
            _name = name;
            _index = index;
            _bindTransform = bindTransform;
        }

        public void AddChild(Joint child)
        {
            _children.Add(child);
        }

        public override string ToString()
        {
            string txt = "";
            Matrix4x4f m = _bindTransform;
            for (uint i = 0; i < 4; i++)
            {
                txt += $"{Cut(m[0, i])} {Cut(m[1, i])} {Cut(m[2, i])} {Cut(m[3, i])}" 
                    + ((i < 3) ? "\r\n": "");
            }
            float Cut(float a) => ((float)Math.Abs(a) < 0.000001f) ? 0.0f : a;
            return $"[{_index},{_name}] Matrix4x4f \r\n{txt}";
        }
    }
}
