using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyRender.MyEngine;
using OpenTK;
using MyRender.Debug;

namespace MyRender.Game
{
    class BaseRenderScene : Scene
    {
        public override void OnStart()
        {
            base.OnStart();

            Node node;
            node = new Node();
            AddChild(node);
            AddChild(new Node());
            node.AddChild(new Node());
            node.AddChild(new Node());
            node.AddChild(new Node());

        }


        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            //Log.Print("BaseRenderScene-OnUpdate");
        }
    }
}
