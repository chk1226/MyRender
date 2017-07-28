using OpenTK;

namespace MyRender.MyEngine
{
    class JointTransform
    {
        // Quaternion no has position information, so need position variable
        //public Vector3 position;
        //public Quaternion ActionValue;

        public Matrix4 Action = Matrix4.Identity;

        public Matrix4 GetLocalTransform()
        {
            return Action;
        }

        public static JointTransform Interpolate(JointTransform from, JointTransform to, float progression)
        {
            var newJoint = new JointTransform();
            //newJoint.position = Vector3.Lerp(from.position, to.position, progression);
            //newJoint.ActionValue = Quaternion.Slerp(from.ActionValue, to.ActionValue, progression);

            newJoint.Action = to.Action * progression + from.Action * (1.0f - progression);

            return newJoint;
        }

    }

}
