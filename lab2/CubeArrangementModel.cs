using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public double TopLayerRotationAngle { get; private set; } = 0;
        public bool IsRotatingTopLayer { get; private set; } = false;
        private double targetAngle = 0;
        private const double ROTATION_SPEED = 250.0;

        public void StartTopLayerRotationRight()
        {
            if (!IsRotatingTopLayer)
            {
                IsRotatingTopLayer = true;
                targetAngle = TopLayerRotationAngle + 90;
            }
        }

        public void StartTopLayerRotationLeft()
        {
            if (!IsRotatingTopLayer)
            {
                IsRotatingTopLayer = true;
                targetAngle = TopLayerRotationAngle - 90;
            }
        }

        internal void AdvanceTime(double deltaTime)
        {
            // we do not advance the simulation when animation is stopped
            if (!AnimationEnabled)
                return;

            // set a simulation time
            Time += deltaTime;

            if (IsRotatingTopLayer)
            {
                double step = ROTATION_SPEED * deltaTime;
                if (Math.Abs(targetAngle - TopLayerRotationAngle) <= step)
                {
                    TopLayerRotationAngle = targetAngle;
                    IsRotatingTopLayer = false;
                }
                else if (targetAngle > TopLayerRotationAngle)
                {
                    TopLayerRotationAngle += step;
                }
                else
                {
                    TopLayerRotationAngle -= step;
                }
            }

        }
    }
}
