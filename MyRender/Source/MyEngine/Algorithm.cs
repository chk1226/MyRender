using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.MyEngine
{
    partial class Algorithm
    {
        private static readonly Random _random = new Random();
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
    }
}
