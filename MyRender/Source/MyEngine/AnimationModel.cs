using MyRender.Debug;
using System.Collections.Generic;
using OpenTK;


namespace MyRender.MyEngine
{
    class AnimationModel
    {
        public List<Joint> JointHierarchy = new List<Joint>();
        public Animator animator;

        private List<Joint[]> hashJoint = new List<Joint[]>();
        public List<Joint[]> HashJoint
        {
            get { return hashJoint; }
        }

        public Animation AnimationData;

        public AnimationModel()
        {
            animator = new Animator(this);
        }

        public void CreateHashJoint(string[] jointSid)
        {
            var hash = new Joint[jointSid.Length];
            for (int i = 0; i < jointSid.Length; i++)
            {
                hash[i] = searchJoint(jointSid[i]);
            }
            hashJoint.Add(hash);
        }

        private Joint searchJoint(string sid)
        {
            foreach(var joint in JointHierarchy)
            {
                var result = findJoint(sid, joint);
                if(result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private Joint findJoint(string sid, Joint joint)
        {
            if(joint.sid == sid)
            {
                return joint;
            }

            foreach(var child in joint.children)
            {
                var result = findJoint(sid, child);
                if(result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
