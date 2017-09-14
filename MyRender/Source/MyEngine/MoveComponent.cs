
using OpenTK;
using System;

namespace MyRender.MyEngine
{
    class MoveComponent : BaseComponent
    {
        private float delayTime;
        private float animationTime;
        private float elapsedTime = 0;
        private bool start = false;
        private bool doBack = false;
        private Vector3 destPos;
        private Vector3 backPos;

        public MoveComponent(float delay, float time, Vector3 dest, Node node) : base(node)
        {
            delayTime = delay;
            destPos = dest;
            animationTime = time;

            if(delay <= 0)
            {
                start = true;
            }

            if (node != null)
            {
                backPos = node.LocalPosition;
            }
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            float time = (float)e.Time;

            if(delayTime > 0)
            {
                delayTime -= time;
                if (delayTime <= 0)
                {
                    start = true;
                }

            }

            if(start)
            {
                //var offset = direction * velocity * time;

                // plus position
                Node node;
                if (wNode != null &&
                    wNode.TryGetTarget(out node))
                {
                    var pos = node.LocalPosition;

                    elapsedTime = Math.Min(elapsedTime + time, animationTime);
                    float rate = Algorithm.SmootherStep(0, 1, elapsedTime / animationTime);

                    Vector3 dir;
                    if (!doBack)
                    {
                        dir = (destPos - backPos) * rate;
                        node.LocalPosition = backPos + dir;

                    }
                    else
                    {
                        dir = (backPos - destPos) * rate;
                        node.LocalPosition = destPos + dir;
                    }

                    if(elapsedTime == animationTime)
                    {
                        elapsedTime = 0;
                        doBack = !doBack;
                    }
                    
                }


            }


        }

    }
}
