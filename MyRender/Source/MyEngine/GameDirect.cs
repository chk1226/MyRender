using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRender.MyEngine
{
    class GameDirect
    {
        private GameDirect() { }

        static private GameDirect _instance;
        static public GameDirect Instance {
            get {
                if(_instance == null)
                {
                    _instance = new GameDirect();
                }
                return _instance;
            }
        }
    }
}
