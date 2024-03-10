using Assimp;
using OpenGL;

namespace LSystem
{
    public class BonePose
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

        public BonePose(Vertex3f position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
            _scaling = Vertex3f.One;
        }

        public BonePose(Vector3D position, Quaternion rotation, Vector3D scaling)
        {
            _position = new Vertex3f((float)position.X, (float)position.Y, (float)position.Z);
            _rotation = rotation;
            _scaling = new Vertex3f((float)scaling.X, (float)scaling.Y, (float)scaling.Z);
        }

        public override string ToString()
        {
            string txt = "";
            txt += " scale=" + _scaling.ToString();
            txt += " pos=" + _position.ToString();
            txt += " rot=" + _rotation.ToString();
            return txt;
        }

        public BonePose()
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
                return T * R * S;
            }
        }

        public static BonePose InterpolateSlerp(BonePose frameA, BonePose frameB, float progression)
        {
            Vertex3f pos = InterpolateLerp(frameA.Position, frameB.Position, progression);
            Quaternion rot = frameA.Rotation.Interpolate(frameB.Rotation, progression);
            return new BonePose(pos, rot);
        }

        private static Vertex3f InterpolateLerp(Vertex3f start, Vertex3f end, float progression)
        {
            return start + (end - start) * progression;
        }

    }
}
