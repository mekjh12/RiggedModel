using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    /// <summary>
    /// Bone과 동일하게 뼈로서 부모와 자식을 연결하여 Armature를 구성하는 요소이다.
    /// </summary>
    public class Bone
    {
        private int _index;
        private string _name;
        private List<Bone> _children = new List<Bone>();
        private Bone _parent;

        private Matrix4x4f _animatedTransform = Matrix4x4f.Identity;
        private Matrix4x4f _bindTransform = Matrix4x4f.Identity;
        private Matrix4x4f _inverseBindTransform = Matrix4x4f.Identity;
        private Matrix4x4f _localAnimationTransform = Matrix4x4f.Identity;

        public bool IsLeaf => _children.Count == 0;

        public Bone Parent
        {
            get => _parent; 
            set => _parent = value;
        }

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public Vertex3f PivotPosition => _animatedTransform.Column3.Vertex3f();

        public List<Bone> Childrens => _children;

        /// <summary>
        /// 캐릭터공간에서의 변환행렬
        /// </summary>
        public Matrix4x4f AnimatedTransform 
        {
            get => _animatedTransform;
            set => _animatedTransform = value;
        }

        /// <summary>
        /// 뼈공간에서의 변환행렬
        /// </summary>
        public Matrix4x4f LocalAnimationTransform
        {
            get => _localAnimationTransform;
            set => _localAnimationTransform = value;
        }

        /// <summary>
        /// 바인딩포즈의 뼈공간의 역행렬
        /// </summary>
        public Matrix4x4f InverseBindTransform
        {
            get => _inverseBindTransform;
            set => _inverseBindTransform = value;
        }

        /// <summary>
        /// 바인딩 포즈의 변환행렬
        /// </summary>
        public Matrix4x4f BindTransform
        {
            get => _bindTransform;
            set => _bindTransform = value;
        }

        public Bone(string name, int index, Matrix4x4f bindTransform)
        {
            _name = name;
            _index = index;
            _bindTransform = bindTransform;
        }

        public void AddChild(Bone child)
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
                    + ((i < 3) ? " / ": "");
            }
            return $"[{_index},{_name}] BindMatrix {txt}";

            float Cut(float a) => ((float)Math.Abs(a) < 0.000001f) ? 0.0f : a;
        }
    }
}
