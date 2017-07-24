using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.Debug
{
    class Log
    {
        public static void Print(string str)
        {
            Console.WriteLine(str);
        }

        public static void Assert(string str)
        {
            System.Diagnostics.Debug.Assert(false, str);
        }
    }
}
