using Silk.NET.Maths;

namespace GrafikaSzeminarium
{
    internal class ExplosionEffect
    {
        public Vector3D<float> Position { get; private set; }
        public float Scale { get; private set; }
        public float Alpha { get; private set; }
        public float Duration { get; private set; }
        public Vector3D<float> Color { get; private set; }
        public bool IsActive => Duration > 0;

        public ExplosionEffect(Vector3D<float> position)
        {
            Position = position;
            Scale = 2.0f;
            Alpha = 1.0f;
            Duration = 1.0f;
            Color = new Vector3D<float>(1.0f, 0.5f, 0.0f);
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            Duration -= deltaTime;
            Scale += deltaTime * 1.0f;
            Alpha = Duration;
        }
    }
}