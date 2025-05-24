using Silk.NET.Maths;
using System;
using System.Collections.Generic;

namespace Szeminarium
{
    internal class BotsDescriptor
    {
        private List<Bot> bots;
        private bool animationEnabled = true;

        public BotsDescriptor()
        {
            bots = new List<Bot>();
            InitializeBots();
        }

        private void InitializeBots()
        {
            var paths = new List<List<Vector3D<float>>>
            {
                CreateCircularPath(new Vector3D<float>(20, 0, 20), 15.0f, 16),

                CreateFigureEightPath(new Vector3D<float>(-20, 0, 20), 12.0f, 20),

                CreateLinearPath(new Vector3D<float>(0, 0, -40), new Vector3D<float>(0, 0, 40), 115),

                CreateSquarePath(new Vector3D<float>(-25, 0, -20), 20.0f, 16),

                CreateSpiralPath(new Vector3D<float>(20, 0, 0), 15.0f, 0.0f, 24),

                CreateCircularPath(new Vector3D<float>(-20, 0, -20), 18.0f, 16),

                CreateFigureEightPath(new Vector3D<float>(20, 0, -20), 15.0f, 20),

                CreateLinearPath(new Vector3D<float>(-40, 0, 0), new Vector3D<float>(40, 0, 0), 115),

                CreateSquarePath(new Vector3D<float>(25, 0, 20), 25.0f, 16),

                CreateSpiralPath(new Vector3D<float>(-20, 0, 0), 18.0f, 0.0f, 24)
            };

            float[] speeds = { 1.5f, 1.0f, 2.0f, 2.5f, 1.2f, 2.0f, 1.8f, 1.5f, 2.2f, 1.7f };

            for (int i = 0; i < 5; i++)
            {
                var bot = new Bot(
                    $"hydra_flak_{i}",
                    paths[i],
                    speeds[i],
                    i * 0.2f
                );
                bots.Add(bot);
            }

            for (int i = 5; i < 10; i++)
            {
                var bot = new Bot(
                    $"hammerhead_{i}",
                    paths[i],
                    speeds[i],
                    i * 0.2f
                );
                bot.IsHammerhead = true;
                bots.Add(bot);
            }
        }

        private List<Vector3D<float>> CreateCircularPath(Vector3D<float> center, float radius, int points)
        {
            var path = new List<Vector3D<float>>();
            for (int i = 0; i < points; i++)
            {
                float angle = (float)(i * 2 * Math.PI / points);
                path.Add(new Vector3D<float>(
                    center.X + radius * (float)Math.Cos(angle),
                    center.Y,
                    center.Z + radius * (float)Math.Sin(angle)
                ));
            }
            return path;
        }

        private List<Vector3D<float>> CreateFigureEightPath(Vector3D<float> center, float size, int points)
        {
            var path = new List<Vector3D<float>>();
            for (int i = 0; i < points; i++)
            {
                float t = (float)(i * 2 * Math.PI / points);
                path.Add(new Vector3D<float>(
                    center.X + size * (float)Math.Sin(t),
                    center.Y,
                    center.Z + size * (float)Math.Sin(t) * (float)Math.Cos(t)
                ));
            }
            return path;
        }

        private List<Vector3D<float>> CreateLinearPath(Vector3D<float> start, Vector3D<float> end, int points)
        {
            var path = new List<Vector3D<float>>();
            for (int i = 0; i < points; i++)
            {
                float t = (float)i / (points - 1);
                if (i >= points / 2)
                    t = 1.0f - (float)(i - points / 2) / (points / 2 - 1);

                path.Add(Vector3D.Lerp(start, end, t));
            }
            return path;
        }

        private List<Vector3D<float>> CreateSquarePath(Vector3D<float> center, float size, int points)
        {
            var path = new List<Vector3D<float>>();
            var corners = new Vector3D<float>[]
            {
                center + new Vector3D<float>(-size/2, 0, -size/2),
                center + new Vector3D<float>(size/2, 0, -size/2),
                center + new Vector3D<float>(size/2, 0, size/2),
                center + new Vector3D<float>(-size/2, 0, size/2)
            };

            int pointsPerSide = points / 4;
            for (int side = 0; side < 4; side++)
            {
                var start = corners[side];
                var end = corners[(side + 1) % 4];

                for (int i = 0; i < pointsPerSide; i++)
                {
                    float t = (float)i / pointsPerSide;
                    path.Add(Vector3D.Lerp(start, end, t));
                }
            }
            return path;
        }

        private List<Vector3D<float>> CreateSpiralPath(Vector3D<float> center, float maxRadius, float height, int points)
        {
            var path = new List<Vector3D<float>>();
            for (int i = 0; i < points; i++)
            {
                float t = (float)i / points;
                float angle = t * 4 * (float)Math.PI;
                float radius = maxRadius * t;

                float y = center.Y;

                path.Add(new Vector3D<float>(
                    center.X + radius * (float)Math.Cos(angle),
                    y,
                    center.Z + radius * (float)Math.Sin(angle)
                ));
            }
            return path;
        }
        public void Update(float deltaTime)
        {
            if (!animationEnabled) return;

            foreach (var obj in bots)
            {
                obj.Update(deltaTime);
            }
        }

        public List<Bot> GetBots()
        {
            return bots;
        }

        public void SetAnimationEnabled(bool enabled)
        {
            animationEnabled = enabled;
        }
        public bool IsAnimationEnabled => animationEnabled;

    }

    internal class Bot
    {
        public string ModelName { get; private set; }
        public Vector3D<float> Position { get; private set; }
        public Vector3D<float> Rotation { get; private set; }
        public float Scale { get; set; } = 1.0f;

        public bool IsHammerhead { get; set; } = false;

        private List<Vector3D<float>> path;
        private float speed;
        private float currentPathTime;
        private int currentPathIndex;
        private float pathTimeOffset;

        public Bot(string modelName, List<Vector3D<float>> path, float speed, float timeOffset = 0f)
        {
            ModelName = modelName;
            this.path = path;
            this.speed = speed;
            this.pathTimeOffset = timeOffset;
            currentPathTime = timeOffset;
            currentPathIndex = 0;

            if (path.Count > 0) Position = path[0];
        }

        public void Update(float deltaTime)
        {
            if (path.Count < 2) return;

            currentPathTime += deltaTime * speed;

            float totalTime = path.Count - 1;
            float normalizedTime = currentPathTime % totalTime;
            currentPathIndex = (int)Math.Floor(normalizedTime);
            float segmentProgress = normalizedTime - currentPathIndex;

            if (currentPathIndex >= path.Count - 1)
            {
                currentPathIndex = path.Count - 2;
                segmentProgress = 1.0f;
            }

            Vector3D<float> currentPoint = path[currentPathIndex];
            Vector3D<float> nextPoint = path[(currentPathIndex + 1) % path.Count];

            Position = Vector3D.Lerp(currentPoint, nextPoint, segmentProgress);

            Vector3D<float> direction = Vector3D.Normalize(nextPoint - currentPoint);
            if (direction.Length > 0.01f)
            {
                float yRotation = (float)Math.Atan2(direction.X, direction.Z);
                Rotation = new Vector3D<float>(0, yRotation, 0);
            }
        }

        public Matrix4X4<float> GetTransformMatrix()
        {
            return Matrix4X4.CreateScale(Scale) * Matrix4X4.CreateRotationY(Rotation.Y) * Matrix4X4.CreateTranslation(Position);
        }

        public void Reset()
        {
            currentPathTime = pathTimeOffset;
            currentPathIndex = 0;
            if (path.Count > 0) Position = path[0];
        }

    }
}