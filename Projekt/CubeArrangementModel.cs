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

        private const float MovementSpeed = 2.0f;

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

        public void MoveForward(float deltaTime)
        {
            Position = new Vector3D<float>(Position.X, Position.Y, Position.Z + MovementSpeed * deltaTime);
        }

        public void MoveBackward(float deltaTime)
        {
            Position = new Vector3D<float>(Position.X, Position.Y, Position.Z - MovementSpeed * deltaTime);
        }

        public void MoveLeft(float deltaTime)
        {
            Position = new Vector3D<float>(Position.X - MovementSpeed * deltaTime, Position.Y, Position.Z);
        }

        public void MoveRight(float deltaTime)
        {
            Position = new Vector3D<float>(Position.X + MovementSpeed * deltaTime, Position.Y, Position.Z);
        }

    }
}
