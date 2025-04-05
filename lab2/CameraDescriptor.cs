
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 1;
        const double DistanceScaleFactor = 1.1;
        const double AngleChangeStepSize = Math.PI / 180 * 5;
        const float MoveSize = 0.2f;

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        public Vector3D<float> Position
        {
            get
            {
                return Target - new Vector3D<float>(0, 0, 1) * (float)DistanceToOrigin;
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
            Target += new Vector3D<float>(0, 0, 1) * MoveSize;
        }

        public void MoveBackward()
        {
            Target += new Vector3D<float>(0, 0, -1) * MoveSize;
        }

        public void MoveRight()
        {
            Target += new Vector3D<float>(-1, 0, 0) * MoveSize;
        }

        public void MoveLeft()
        {
            Target = Target + new Vector3D<float>(1, 0, 0) * MoveSize;
        }

        public void MoveUp()
        {
            Target += new Vector3D<float>(0, 1, 0) * MoveSize;
        }

        public void MoveDown()
        {
            Target += new Vector3D<float>(0, -1, 0) * MoveSize;
        }

        private static Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Sin(angleToMinZYPlane);
            var z = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Cos(angleToMinZYPlane);
            var y = distanceToOrigin * Math.Sin(angleToMinZXPlane);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}
