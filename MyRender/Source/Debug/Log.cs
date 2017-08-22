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
#if DEBUG
            Console.WriteLine(str);
#endif
        }

        public static void Assert(string str)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(false, str);
#endif

        }
    }
}
