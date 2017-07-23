using System.Collections.Generic;

namespace MyRender.MyEngine
{
    class KeyFrame
    {
        public float timeStamp;
        // joint name
        public Dictionary<string, JointTransform> pose = new Dictionary<string, JointTransform>();
    }
}
