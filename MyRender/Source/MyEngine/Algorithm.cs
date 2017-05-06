using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.Source.MyEngine
{
    class Algorithm
    {
        private static readonly Random _random = new Random();
        public static Random GetRandom
        {
            get { return _random; }
        }
    }
}
