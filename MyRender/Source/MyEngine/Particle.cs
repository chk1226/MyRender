using OpenTK;
using OpenTK.Graphics;

namespace MyRender.MyEngine
{
    class Particle
    {
        private Vector3 position;
        private Vector3 velocity;
        private float life;
        private float gravity;
        private float rotation;
        private float scale;
        private float elapsedTime = 0;

        private bool isLife = true;
        public bool IsLife
        {
            get
            {
                return isLife;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            private set
            {
                position = value;
            }
        }

        public Particle(Vector3 pos, Vector3 velocity, float life, float gravity, float rotation, float scale)
        {
            ResetParticle(pos, velocity, life, gravity, rotation, scale);
        }

        public void Update(float deltaTime)
        {
            velocity.Y += gravity * deltaTime;
            var change = velocity * deltaTime;
            Position += change;
            elapsedTime += deltaTime;

            if(elapsedTime >= life)
            {
                isLife = false;
            }
        }

        public void ResetParticle(Vector3 pos, Vector3 velocity, float life, float gravity, float rotation, float scale)
        {
            Position = pos;
            this.velocity = velocity;
            this.life = life;
            this.gravity = gravity;
            this.rotation = rotation;
            this.scale = scale;

            isLife = true;
            elapsedTime = 0;
        }

    }
}
