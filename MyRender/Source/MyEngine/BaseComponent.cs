using OpenTK;
using System;

namespace MyRender.MyEngine
{
    class BaseComponent
    {
        protected WeakReference<Node> wNode;

        public BaseComponent(Node node)
        {
            wNode = new WeakReference<Node>(node);
        }

        virtual public void OnUpdate(FrameEventArgs e) { }

        virtual public void Release() { }

    }
}
