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
    }
}
