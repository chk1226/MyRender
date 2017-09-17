using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using MyRender.Debug;

namespace MyRender.MyEngine
{
    partial class Algorithm
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        public static Random GetRandom
        {
            get { return _random; }
        }

        private static readonly Vector4 zeroVector = new Vector4(0, 0, 0, 1);
        public static Vector4 ZeroVector { get { return zeroVector; } }

        // reference https://stackoverflow.com/questions/4436764/rotating-a-quaternion-on-1-axis
        public static Quaternion CreateFromAxisAngle(float x, float y, float z, float a)
        {
            // Here we calculate the sin( theta / 2) once for optimization
            var factor = (float)Math.Sin(a / 2.0);

            // Calculate the x, y and z of the quaternion
            x *= factor;
            y *= factor;
            z *= factor;

            // Calcualte the w value by cos( theta / 2 )
            var w = (float)Math.Cos(a / 2.0);

            return new Quaternion(x, y, z, w);
        }

        public static void Matrix4ToArray(ref Matrix4 mat, float[] array, int index)
        {
            array[index] = mat.M11;
            array[index + 1] = mat.M12;
            array[index + 2] = mat.M13;
            array[index + 3] = mat.M14;
            array[index + 4] = mat.M21;
            array[index + 5] = mat.M22;
            array[index + 6] = mat.M23;
            array[index + 7] = mat.M24;
            array[index + 8] = mat.M31;
            array[index + 9] = mat.M32;
            array[index + 10] = mat.M33;
            array[index + 11] = mat.M34;
            array[index + 12] = mat.M41;
            array[index + 13] = mat.M42;
            array[index + 14] = mat.M13;
            array[index + 15] = mat.M14;
        }

        public static Color4 ColorNormalize(ref Color4 color)
        {
            Color4 newColor;

            newColor.R = color.R / 255;
            newColor.G = color.G / 255;
            newColor.B = color.B / 255;
            newColor.A = color.A / 255;

            return newColor;
        }

        public static Color4 ColorLearp(ref Color4 from, ref Color4 to, float blend)
        {
            Color4 color;
            color.R = to.R * blend + from.R * (1.0f - blend);
            color.G = to.G * blend + from.G * (1.0f - blend);
            color.B = to.B * blend + from.B * (1.0f - blend);
            color.A = to.A * blend + from.A * (1.0f - blend);

            return color;
        }

        public static float Lerp(float from, float to, float progression)
        {
            return from + progression * (to - from);
        }

        public static float SmootherStep(float edge0, float edge1, float x)
        {
            // Scale, and clamp x to 0..1 range
            x = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            // Evaluate polynomial
            return x * x * x * (x * (x * 6 - 15) + 10);
        }


        #region perlin noise

        private static readonly int octaves = 4;
        private static readonly float persistence = 0.3f;
#if DEBUG
        private static float noiseScale = 10;
#else
        private static float noiseScale = 18;
#endif

        //reference 
        // https://www.shadertoy.com/view/Mls3RS
        // http://flafla2.github.io/2014/08/09/perlinnoise.html
        static float noise(int x, int y)
        {
            float dot = Vector2.Dot(new Vector2(x, y), new Vector2(12.9898f, 78.233f));
#if DEBUG
            dot = (float)Math.Sin(dot) * 36875.5453f;
#else
            dot = (float)Math.Sin(dot) * 46500.5453f;
#endif
            dot -= (int)dot;

            return dot;
        }

        static float smoothNoise(int x, int y)
        {
            return noise(x, y) / 4.0f + 
                (noise(x + 1, y) + noise(x - 1, y) + noise(x, y + 1) + noise(x, y - 1)) / 8.0f + 
                (noise(x + 1, y + 1) + noise(x + 1, y - 1) + noise(x - 1, y + 1) + noise(x - 1, y - 1)) / 16.0f;
        }

        static float cosInterpolation(float x, float y, float n)
        {
            float f = (1.0f - (float)Math.Cos(n * MathHelper.Pi)) * 0.5f;
            return x * (1.0f - f) + y * f;
        }

        static float interpolationNoise(float x, float y)
        {
            int ix = (int)x;
            int iy = (int)y;
            float fracx = x - (int)x;
            float fracy = y - (int)y;

            float v1 = smoothNoise(ix, iy);
            float v2 = smoothNoise(ix + 1, iy);
            float v3 = smoothNoise(ix, iy + 1);
            float v4 = smoothNoise(ix + 1, iy + 1);

            float i1 = cosInterpolation(v1, v2, fracx);
            float i2 = cosInterpolation(v3, v4, fracx);

            return cosInterpolation(i1, i2, fracy);

        }

        public static float PerlinNoise2D(float x, float y)
        {
            float sum = 0;
            float d = (float)Math.Pow(2, octaves - 1);
            for (int i = 0; i < octaves; i++)
            {
                float frequency = (float)Math.Pow(2.0, i) / d;
                float amplitude = (float)Math.Pow(persistence, i) * noiseScale; 
                sum = sum + interpolationNoise(x * frequency, y * frequency) * amplitude;
            }
            //Log.Print(sum.ToString());

            return sum;
        }

#endregion





    }
}
