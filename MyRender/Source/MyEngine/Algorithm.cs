using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.MyEngine
{
    class Algorithm
    {
        private static readonly Random _random = new Random();
        public static Random GetRandom
        {
            get { return _random; }
        }

        private static float radin = (float)Math.PI / 180.0f;
        public static float Radin { get { return radin; } }
    }
}
