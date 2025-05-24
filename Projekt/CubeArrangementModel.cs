using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CubeArrangementModel
    {
        /// <summary>
        /// Gets or sets wheather the animation should run or it should be frozen.
        /// </summary>
        public bool AnimationEnabled { get; set; } = true;

        /// <summary>
        /// The time of the simulation. It helps to calculate time dependent values.
        /// </summary>
        private double Time { get; set; } = 0;

        /// <summary>
        /// The value by which the center cube is scaled. It varies between 0.8 and 1.2 with respect to the original size.
        /// </summary>
        public double CenterCubeScale { get; private set; } = 1;

        public Vector3D<float> Position { get; private set; } = Vector3D<float>.Zero;
        public float Rotation { get; private set; } = 0f;

        private const float MovementSpeed = 3.0f;
        private const float RotationSpeed = 1.5f;

        public bool IsMovingForward { get; set; } = false;
        public bool IsMovingBackward { get; set; } = false;
        public bool IsTurningLeft { get; set; } = false;
        public bool IsTurningRight { get; set; } = false;

        private double boostEndTime = 0;
        private const double BoostDuration = 10.0;
        private const float BoostSizeMultiplier = 1.5f;
        private const float CollectionRadius = 2.0f;
        public bool IsBoosted => Time <= boostEndTime;

        internal void AdvanceTime(double deltaTime)
        {
            // we do not advance the simulation when animation is stopped
            if (!AnimationEnabled)
                return;

            // set a simulation time
            Time += deltaTime;


            if (IsBoosted)
            {
                CenterCubeScale = BoostSizeMultiplier;
            }
            else
            {
                CenterCubeScale = 1.0;
            }
        }

        public void UpdateMovement(float deltaTime)
        {
            if (IsTurningLeft)
            {
                Rotation += RotationSpeed * deltaTime;
            }
            if (IsTurningRight)
            {
                Rotation -= RotationSpeed * deltaTime;
            }

            while (Rotation > Math.PI * 2) Rotation -= (float)(Math.PI * 2);
            while (Rotation < 0) Rotation += (float)(Math.PI * 2);

            Vector3D<float> forward = new Vector3D<float>(
                (float)Math.Sin(Rotation),
                0,
                (float)Math.Cos(Rotation)
            );

            Vector3D<float> right = new Vector3D<float>(
                (float)Math.Cos(Rotation),
                0,
                -(float)Math.Sin(Rotation)
            );

            Vector3D<float> movementDelta = Vector3D<float>.Zero;

            if (IsMovingForward)
            {
                movementDelta += forward * MovementSpeed * deltaTime;
            }
            if (IsMovingBackward)
            {
                movementDelta -= forward * MovementSpeed * deltaTime;
            }

            Position += movementDelta;
        }

        public Matrix4X4<float> GetTransformMatrix()
        {
            return Matrix4X4.CreateScale((float)CenterCubeScale) * Matrix4X4.CreateRotationY(Rotation) * Matrix4X4.CreateTranslation(Position);
        }

        public void Reset()
        {
            Position = Vector3D<float>.Zero;
            Rotation = 0f;
            IsMovingForward = false;
            IsMovingBackward = false;
            IsTurningLeft = false;
            IsTurningRight = false;
            boostEndTime = 0;
        }

        public void CollectBoost()
        {
            boostEndTime = Time + BoostDuration;
        }

        public bool CanCollectBoost(Vector3D<float> boostPosition)
        {
            float distance = Vector3D.Distance(Position, boostPosition);
            return distance <= CollectionRadius;
        }

        public double GetRemainingBoostTime()
        {
            if (!IsBoosted) return 0;
            return boostEndTime - Time;
        }
        


    }
}
