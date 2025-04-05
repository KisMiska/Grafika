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
        public double MiddleLayerRotationAngle { get; private set; } = 0;
        public double BottomLayerRotationAngle { get; private set; } = 0;

        private bool isRotatingTop = false;
        private bool isRotatingMiddle = false;
        private bool isRotatingBottom = false;

        private double targetAngleTop = 0;
        private double targetAngleMiddle = 0;
        private double targetAngleBottom = 0;

        public double LeftLayerRotationAngle { get; private set; } = 0;
        public double CenterLayerRotationAngle { get; private set; } = 0;
        public double RightLayerRotationAngle { get; private set; } = 0;


        private bool isRotatingLeft = false;
        private bool isRotatingCenter = false;
        private bool isRotatingRight = false;

        private double targetAngleLeft = 0;
        private double targetAngleCenter = 0;
        private double targetAngleRight = 0;



        private const double ROTATION_SPEED = 250.0;

        public void StartTopLayerRotationRight()
        {
            if (!isRotatingTop)
            {
                isRotatingTop = true;
                targetAngleTop = TopLayerRotationAngle + 90;
            }
        }

        public void StartTopLayerRotationLeft()
        {
            if (!isRotatingTop)
            {
                isRotatingTop = true;
                targetAngleTop = TopLayerRotationAngle - 90;
            }
        }

        public void StartMiddleLayerRotationRight()
        {
            if (!isRotatingMiddle)
            {
                isRotatingMiddle = true;
                targetAngleMiddle = MiddleLayerRotationAngle + 90;
            }
        }

        public void StartMiddleLayerRotationLeft()
        {
            if (!isRotatingMiddle)
            {
                isRotatingMiddle = true;
                targetAngleMiddle = MiddleLayerRotationAngle - 90;
            }
        }

        public void StartBottomLayerRotationRight()
        {
            if (!isRotatingBottom)
            {
                isRotatingBottom = true;
                targetAngleBottom = BottomLayerRotationAngle + 90;
            }
        }

        public void StartBottomLayerRotationLeft()
        {
            if (!isRotatingBottom)
            {
                isRotatingBottom = true;
                targetAngleBottom = BottomLayerRotationAngle - 90;
            }
        }



        public void StartLeftLayerRotationUp()
        {
            if (!isRotatingLeft)
            {
                isRotatingLeft = true;
                targetAngleLeft = LeftLayerRotationAngle + 90;
            }
        }

        public void StartLeftLayerRotationDown()
        {
            if (!isRotatingLeft)
            {
                isRotatingLeft = true;
                targetAngleLeft = LeftLayerRotationAngle - 90;
            }
        }

        public void StartCenterLayerRotationUp()
        {
            if (!isRotatingCenter)
            {
                isRotatingCenter = true;
                targetAngleCenter = CenterLayerRotationAngle + 90;
            }
        }

        public void StartCenterLayerRotationDown()
        {
            if (!isRotatingCenter)
            {
                isRotatingCenter = true;
                targetAngleCenter = CenterLayerRotationAngle - 90;
            }
        }

        public void StartRightLayerRotationUp()
        {
            if (!isRotatingRight)
            {
                isRotatingRight = true;
                targetAngleRight = RightLayerRotationAngle + 90;
            }
        }

        public void StartRightLayerRotationDown()
        {
            if (!isRotatingRight)
            {
                isRotatingRight = true;
                targetAngleRight = RightLayerRotationAngle - 90;
            }
        }

        internal void AdvanceTime(double deltaTime)
        {
            if (!AnimationEnabled)
                return;

            Time += deltaTime;

            double topAngle = TopLayerRotationAngle;
            double middleAngle = MiddleLayerRotationAngle;
            double bottomAngle = BottomLayerRotationAngle;

            double leftAngle = LeftLayerRotationAngle;
            double centerAngle = CenterLayerRotationAngle;
            double rightAngle = RightLayerRotationAngle;

            UpdateLayerRotation(deltaTime, ref isRotatingTop, ref topAngle, ref targetAngleTop);
            UpdateLayerRotation(deltaTime, ref isRotatingMiddle, ref middleAngle, ref targetAngleMiddle);
            UpdateLayerRotation(deltaTime, ref isRotatingBottom, ref bottomAngle, ref targetAngleBottom);

            UpdateLayerRotation(deltaTime, ref isRotatingLeft, ref leftAngle, ref targetAngleLeft);
            UpdateLayerRotation(deltaTime, ref isRotatingCenter, ref centerAngle, ref targetAngleCenter);
            UpdateLayerRotation(deltaTime, ref isRotatingRight, ref rightAngle, ref targetAngleRight);


            TopLayerRotationAngle = topAngle;
            MiddleLayerRotationAngle = middleAngle;
            BottomLayerRotationAngle = bottomAngle;

            LeftLayerRotationAngle = leftAngle;
            CenterLayerRotationAngle = centerAngle;
            RightLayerRotationAngle = rightAngle;
        }

        private void UpdateLayerRotation(double deltaTime, ref bool isRotating, ref double currentAngle, ref double targetAngle)
        {
            if (!isRotating) return;

            double step = ROTATION_SPEED * deltaTime;
            if (Math.Abs(targetAngle - currentAngle) <= step)
            {
                currentAngle = targetAngle;
                isRotating = false;
            }
            else if (targetAngle > currentAngle)
            {
                currentAngle += step;
            }
            else
            {
                currentAngle -= step;
            }
        }

    }
}