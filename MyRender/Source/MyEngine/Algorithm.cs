using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace MyRender.MyEngine
{
    partial class Algorithm
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        public static Random GetRandom
        {
            get { return _random; }
        }

        private static float radin = (float)Math.PI / 180.0f;
        public static float Radin { get { return radin; } }

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
    }
}
