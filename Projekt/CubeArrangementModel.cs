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
        public bool AnimationEnabled { get; set; } = false;

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
        private const float RotationSpeed = 2.0f;

        public bool IsMovingForward { get; set; } = false;
        public bool IsMovingBackward { get; set; } = false;
        public bool IsTurningLeft { get; set; } = false;
        public bool IsTurningRight { get; set; } = false;

        internal void AdvanceTime(double deltaTime)
        {
            // we do not advance the simulation when animation is stopped
            if (!AnimationEnabled)
                return;

            // set a simulation time
            Time += deltaTime;

            // lets produce an oscillating scale in time
            CenterCubeScale = 1 + 0.2 * Math.Sin(1.5 * Time);
        }

        public void UpdateMovement(float deltaTime)
        {
            // Handle rotation first
            if (IsTurningLeft)
            {
                Rotation += RotationSpeed * deltaTime;
            }
            if (IsTurningRight)
            {
                Rotation -= RotationSpeed * deltaTime;
            }

            // Keep rotation in 0-2π range
            while (Rotation > Math.PI * 2) Rotation -= (float)(Math.PI * 2);
            while (Rotation < 0) Rotation += (float)(Math.PI * 2);

            // Calculate movement vectors based on current rotation
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

            // Calculate movement delta
            Vector3D<float> movementDelta = Vector3D<float>.Zero;

            if (IsMovingForward)
            {
                movementDelta += forward * MovementSpeed * deltaTime;
            }
            if (IsMovingBackward)
            {
                movementDelta -= forward * MovementSpeed * deltaTime;
            }

            // Apply movement
            Position += movementDelta;
        }

        public Matrix4X4<float> GetTransformMatrix()
        {
            return Matrix4X4.CreateScale((float)CenterCubeScale) *
                   Matrix4X4.CreateRotationY(Rotation) *
                   Matrix4X4.CreateTranslation(Position);
        }

        public void Reset()
        {
            Position = Vector3D<float>.Zero;
            Rotation = 0f;
            IsMovingForward = false;
            IsMovingBackward = false;
            IsTurningLeft = false;
            IsTurningRight = false;
        }

    }
}
