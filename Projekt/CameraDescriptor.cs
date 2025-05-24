
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 1;

        public double AngleToZYPlane { get; private set; } = 0;

        public double AngleToZXPlane { get; private set; } = 0;

        const double DistanceScaleFactor = 1.1;

        const double AngleChangeStepSize = Math.PI / 180 * 5;

        public enum CameraMode
        {
            Free,
            ThirdPerson,
            FirstPerson
        }

        public CameraMode Mode { get; set; } = CameraMode.ThirdPerson;
        private const float ThirdPersonDistance = 4.5f;
        private const float ThirdPersonHeight = 3.0f;
        private const float FirstPersonHeight = 1.2f;
        private const float FirstPersonForwardOffset = 0.5f;

        // <summary>
        // Gets the position of the camera.
        // </summary>
        public Vector3D<float> Position
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.Free:
                        return GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane);

                    case CameraMode.ThirdPerson:
                        return GetThirdPersonPosition();

                    case CameraMode.FirstPerson:
                        return GetFirstPersonPosition();

                    default:
                        return GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane);
                }
            }
        }

        // <summary>
        // Gets the up vector of the camera.
        // </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.Free:
                        return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + Math.PI / 2));

                    case CameraMode.ThirdPerson:
                    case CameraMode.FirstPerson:
                        return Vector3D<float>.UnitY;

                    default:
                        return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + Math.PI / 2));
                }
            }
        }

        // <summary>
        // Gets the target point of the camera view.
        // </summary>
        public Vector3D<float> Target
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.Free:
                        return Vector3D<float>.Zero;

                    case CameraMode.ThirdPerson:
                        return GetThirdPersonTarget();

                    case CameraMode.FirstPerson:
                        return GetFirstPersonTarget();

                    default:
                        return Vector3D<float>.Zero;
                }
            }
        }

        private CubeArrangementModel vehicleModel;

        private Vector3D<float> GetThirdPersonPosition()
        {
            if (vehicleModel == null) return Vector3D<float>.Zero;


            Vector3D<float> vehiclePos = vehicleModel.Position;
            float vehicleRotation = vehicleModel.Rotation;

            Vector3D<float> backward = new Vector3D<float>(
                -(float)Math.Sin(vehicleRotation),
                0,
                -(float)Math.Cos(vehicleRotation)
            );

            Vector3D<float> cameraPosition = vehiclePos +
                                           backward * ThirdPersonDistance +
                                           Vector3D<float>.UnitY * ThirdPersonHeight;

            return cameraPosition;
        }

        private Vector3D<float> GetThirdPersonTarget()
        {
            if (vehicleModel == null) return Vector3D<float>.Zero;

            Vector3D<float> vehiclePos = vehicleModel.Position;
            float vehicleRotation = vehicleModel.Rotation;

            Vector3D<float> forward = new Vector3D<float>(
                (float)Math.Sin(vehicleRotation),
                0,
                (float)Math.Cos(vehicleRotation)
            );

            return vehiclePos + forward * 2.0f + Vector3D<float>.UnitY * 1.0f;
        }

        private Vector3D<float> GetFirstPersonPosition()
        {
            if (vehicleModel == null) return Vector3D<float>.Zero;

            Vector3D<float> vehiclePos = vehicleModel.Position;
            float vehicleRotation = vehicleModel.Rotation;
            float scale = vehicleModel.IsBoosted ? 1.5f : 1.0f;

            Vector3D<float> forward = new Vector3D<float>(
                (float)Math.Sin(vehicleRotation),
                0,
                (float)Math.Cos(vehicleRotation)
            );

            return vehiclePos + forward * (FirstPersonForwardOffset * scale) + Vector3D<float>.UnitY * (FirstPersonHeight * scale);
        }

        private Vector3D<float> GetFirstPersonTarget()
        {
            if (vehicleModel == null) return Vector3D<float>.Zero;

            Vector3D<float> vehiclePos = vehicleModel.Position;
            float vehicleRotation = vehicleModel.Rotation;

            Vector3D<float> forward = new Vector3D<float>(
                (float)Math.Sin(vehicleRotation),
                0,
                (float)Math.Cos(vehicleRotation)
            );

            return vehiclePos + forward * 100.0f + Vector3D<float>.UnitY * FirstPersonHeight;
        }

        public void SetVehicleReference(CubeArrangementModel vehicle)
        {
            vehicleModel = vehicle;
        }

        public void IncreaseZXAngle()
        {
            if (Mode == CameraMode.Free)
                AngleToZXPlane += AngleChangeStepSize;
        }

        public void DecreaseZXAngle()
        {
            if (Mode == CameraMode.Free)
                AngleToZXPlane -= AngleChangeStepSize;
        }

        public void IncreaseZYAngle()
        {
            if (Mode == CameraMode.Free)
                AngleToZYPlane += AngleChangeStepSize;
        }

        public void DecreaseZYAngle()
        {
            if (Mode == CameraMode.Free)
                AngleToZYPlane -= AngleChangeStepSize;
        }

        public void IncreaseDistance()
        {
            if (Mode == CameraMode.Free)
                DistanceToOrigin = DistanceToOrigin * DistanceScaleFactor;
        }

        public void DecreaseDistance()
        {
            if (Mode == CameraMode.Free)
                DistanceToOrigin = DistanceToOrigin / DistanceScaleFactor;
        }

        private static Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Sin(angleToMinZYPlane);
            var z = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Cos(angleToMinZYPlane);
            var y = distanceToOrigin * Math.Sin(angleToMinZXPlane);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }

        public void SetCameraMode(CameraMode mode)
        {
            Mode = mode;
        }
    }
}

