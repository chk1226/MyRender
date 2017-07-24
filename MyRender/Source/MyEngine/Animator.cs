using OpenTK;

namespace MyRender.MyEngine
{
    class Animator
    {
        private AnimationModel entity;

        public Animator(AnimationModel animationModel)
        {
            entity = animationModel;
        }

        public void Update()
        {
        }

        private void applyPoseToJoints(Joint joint)
        {
            foreach(var child in joint.children)
            {
                applyPoseToJoints(child);
            }

            joint.animatedTransform = Matrix4.Invert(joint.inverseBindTransform) * joint.inverseBindTransform;
        }
    }
}
