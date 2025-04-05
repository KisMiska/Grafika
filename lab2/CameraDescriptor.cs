
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 2;
        const double DistanceScaleFactor = 1.1;
        const double AngleChangeStepSize = Math.PI / 180 * 5;
        const float MoveSize = 0.2f;

        public static double Yaw { get; private set; } = 0;
        public static double Pitch { get; private set; } = 0;

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        public Vector3D<float> Position
        {
            get
            {
                Vector3D<float> direction = GetPointFromAngles();
                return Target - direction * (float)DistanceToOrigin;
            }
        }

        /// <summary>
        /// Gets the up vector of the camera.
        /// </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                return Vector3D<float>.UnitY;
            }
        }

        /// <summary>
        /// Gets the target point of the camera view.
        /// </summary>
        public Vector3D<float> Target { get; private set; } = Vector3D<float>.Zero;

        public void IncreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin * DistanceScaleFactor;
        }

        public void DecreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin / DistanceScaleFactor;
        }

        public void MoveForward()
        {
            Target += GetPointFromAngles() * MoveSize;
        }

        public void MoveBackward()
        {
            Target -= GetPointFromAngles() * MoveSize;
        }

        public void MoveRight()
        {
            Target += Vector3D.Cross(GetPointFromAngles(), UpVector) * MoveSize;
        }

        public void MoveLeft()
        {
            Target -= Vector3D.Cross(GetPointFromAngles(), UpVector) * MoveSize;
        }

        public void MoveUp()
        {
            Target += UpVector * MoveSize;
        }

        public void MoveDown()
        {
            Target -= UpVector * MoveSize;
        }

        public void IncreaseYaw()
        {
            Yaw += AngleChangeStepSize;
        }

        public void DecreaseYaw()
        {
            Yaw -= AngleChangeStepSize;
        }

        public void IncreasePitch()
        {
            Pitch += AngleChangeStepSize;
        }

        public void DecreasePitch()
        {
            Pitch -= AngleChangeStepSize;
        }

        private static Vector3D<float> GetPointFromAngles()
        {
            float x = (float)(Math.Cos(Pitch) * Math.Sin(Yaw));
            float y = (float)(Math.Sin(Pitch));
            float z = (float)(Math.Cos(Pitch) * Math.Cos(Yaw));
            return Vector3D.Normalize(new Vector3D<float>(x, y, z));

        }
    }
}
