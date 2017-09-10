using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics;
using System;

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
        private float distance;

        // x is current, y is next
        private Vector2 texCoordField;
        // x is width field, y is height field, 
        private Vector2 textureInfo;
        private float blendAnimationFactor;

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

        public float Scale
        {
            get
            {
                return scale;
            }

            private set
            {
                scale = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }

            private set
            {
                rotation = value;
            }
        }

        public float ElapsedTime
        {
            get
            {
                return elapsedTime;
            }

            private set
            {
                elapsedTime = value;
            }
        }

        public float BlendAnimationFactor
        {
            get
            {
                return blendAnimationFactor;
            }

            private set
            {
                blendAnimationFactor = value;
            }
        }

        public float Distance
        {
            get
            {
                return distance;
            }

            private set
            {
                distance = value;
            }
        }

        /// <summary>
        /// return value - x,y is current texture coordinate, z,w is next texture coordinate
        /// </summary>
        public Vector4 GetTextureCoord()
        {
            float dx = 1 / textureInfo.X;
            float dy = 1 / textureInfo.Y;

            Vector4 result = Vector4.Zero;
            //current
            result.X = ((int)texCoordField.X % (int)textureInfo.X) * dx;
            result.Y = 1 - ((int)texCoordField.X / (int)textureInfo.X) * dy; //flip

            //next
            result.Z = ((int)texCoordField.Y % (int)textureInfo.X) * dx;
            result.W = 1 - ((int)texCoordField.Y / (int)textureInfo.X) * dy; //flip

            //Log.Print(result.ToString());

            return result;
        }

        public Particle(Vector3 pos, Vector3 velocity, float life, float gravity, float rotation, float scale, Vector2 textureInfo)
        {
            this.textureInfo = textureInfo;
            ResetParticle(pos, velocity, life, gravity, rotation, scale);
        }

        public void Update(float deltaTime, ref Matrix4 modelMat)
        {
            velocity.Y += gravity * deltaTime;
            var change = velocity * deltaTime;
            Position += change;
            ElapsedTime += deltaTime;

            updateTextureFieldAndBlend();
            updateDistance(ref modelMat);

            if(ElapsedTime >= life)
            {
                isLife = false;
            }
        }

        private void updateTextureFieldAndBlend()
        {
            var process = ElapsedTime / life;
            process = Math.Min(process, 1.0f);

            var totalField = (int)(textureInfo.X * textureInfo.Y);
            float atlasProcess = process * totalField;

            texCoordField.X = (int)Math.Floor(atlasProcess);
            texCoordField.Y = (texCoordField.X < totalField - 1) ? texCoordField.X + 1 : texCoordField.X;

            BlendAnimationFactor = atlasProcess - texCoordField.X;

            //Log.Print(texCoordField.ToString() + "   " + BlendAnimationFactor);
        }

        private void updateDistance(ref Matrix4 modelMat)
        {
            var pos = modelMat * new Vector4(position, 1);
            distance = (pos.Xyz - GameDirect.Instance.MainScene.MainCamera.eye).LengthFast;
        }

        public void ResetParticle(Vector3 pos, Vector3 velocity, float life, float gravity, float rotation, float scale)
        {
            Position = pos;
            this.velocity = velocity;
            this.life = Math.Max(life, 0.01f);
            this.gravity = gravity;
            Rotation = rotation;
            Scale = scale;

            texCoordField = Vector2.Zero;
            BlendAnimationFactor = 0;

            isLife = true;
            ElapsedTime = 0;
        }

    }
}
