using OpenTK;

namespace MyRender.MyEngine
{
    class JointTransform
    {
        // Quaternion no has position information, so need position variable
        public Vector3 position;
        public Quaternion ActionValue;

        public Matrix4 GetLocalTransform()
        {

            var mat = Matrix4.CreateFromQuaternion(ActionValue);
            mat.M31 = position.X;
            mat.M32 = position.Y;
            mat.M33 = position.Z;

            //var mat = Matrix4.CreateTranslation(position.X, position.Y, position.Z) * Matrix4.CreateFromQuaternion(ActionValue);

            return mat;
        }

        public static JointTransform Interpolate(JointTransform fram, JointTransform to, float progression)
        {
            var newJoint = new JointTransform();
            newJoint.position = Vector3.Lerp(fram.position, to.position, progression);
            newJoint.ActionValue = Quaternion.Slerp(fram.ActionValue, to.ActionValue, progression);

            return newJoint;
        }

    }

}
