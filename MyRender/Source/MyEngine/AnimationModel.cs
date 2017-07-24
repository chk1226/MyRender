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
        private float[] animation_mat;

        public AnimationModel()
        {
            animator = new Animator(this);
        }

        public float[] HashJointToArray(uint layer)
        {
            if(layer >= hashJoint.Count)
            {
                return null;
            }

            var joints = hashJoint[(int)layer];
            int stride = 16;

            if(animation_mat == null)
            {
                animation_mat = new float[joints.Length * stride];
            }

            for(int i = 0; i < joints.Length; i++)
            {
                if (joints[i] == null)
                {
                    var mat= Matrix4.Identity;
                    Algorithm.Matrix4ToArray(ref mat, animation_mat, i * stride);
                }
                else
                {
                    var mat = joints[i].animatedTransform;
                    mat.Transpose();
                    Algorithm.Matrix4ToArray(ref mat, animation_mat, i * stride);
                }
            }

            return animation_mat;
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
                return findJoint(sid, joint);
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
