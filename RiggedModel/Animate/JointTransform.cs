﻿using Assimp;
using OpenGL;

namespace LSystem
{
    public class JointTransform
    {
        Vertex3f _scaling;
        Vertex3f _position;
        Quaternion _rotation;

        public Vertex3f Scaling
        {
            get=>_scaling; 
            set=> _scaling = value;
        }

        public Vertex3f Position
        {
            get => _position;
            set => _position = value;
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public JointTransform(Vertex3f position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
            _scaling = Vertex3f.One;
        }

        public JointTransform(Vector3D position, Quaternion rotation, Vector3D scaling)
        {
            _position = new Vertex3f((float)position.X, (float)position.Y, (float)position.Z);
            _rotation = rotation;
            _scaling = new Vertex3f((float)scaling.X, (float)scaling.Y, (float)scaling.Z);
        }

        public JointTransform()
        {
            throw new System.NotImplementedException();
        }

        public Matrix4x4f LocalTransform
        {
            get
            {
                Matrix4x4f T = Matrix4x4f.Translated(_position.x, _position.y, _position.z);
                Matrix4x4f R = (Matrix4x4f)_rotation;
                Matrix4x4f S = Matrix4x4f.Scaled(_scaling.x, _scaling.y, _scaling.z);
                return S * R * T;
            }
        }

        public static JointTransform InterpolateSlerp(JointTransform frameA, JointTransform frameB, float progression)
        {
            Vertex3f pos = InterpolateLerp(frameA.Position, frameB.Position, progression);
            Quaternion rot = frameA.Rotation.Interpolate(frameB.Rotation, progression);
            return new JointTransform(pos, rot);
        }

        private static Vertex3f InterpolateLerp(Vertex3f start, Vertex3f end, float progression)
        {
            float x = start.x + (end.x - start.x) * progression;
            float y = start.y + (end.y - start.y) * progression;
            float z = start.z + (end.z - start.z) * progression;
            return new Vertex3f(x, y, z);
        }

    }
}