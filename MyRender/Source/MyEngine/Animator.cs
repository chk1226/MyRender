using MyRender.Debug;
using OpenTK;
using System.Collections.Generic;

namespace MyRender.MyEngine
{
    class Animator
    {
        private AnimationModel entity;

        private float animationTime = 0;
        private Animation currentAnimation;

        public Animator(AnimationModel animationModel)
        {
            entity = animationModel;
        }

        public void DoAnimation(Animation doAnimation)
        {
            animationTime = 0;
            currentAnimation = doAnimation;
        }

        public void Update(float delta)
        {
            if (currentAnimation == null) return;

            increaseAnimationTime(delta);
            var currentPos = calculateCurrentAnimationPose();
            foreach(var joint in entity.JointHierarchy)
            {
                applyPoseToJoints(joint, currentPos, Matrix4.Identity);
            }

        }

        private void applyPoseToJoints(Joint joint, Dictionary<string, Matrix4> currentPos, Matrix4 parentTransform)
        {
            Matrix4 currentLocalTransform = Matrix4.Identity;
            if (currentPos.ContainsKey(joint.id))
            {
                currentLocalTransform = currentPos[joint.id];
            }
            else
            {
                currentLocalTransform = joint.localBindTransform;
            }

            currentLocalTransform = parentTransform * currentLocalTransform;

            foreach (var child in joint.children)
            {
                applyPoseToJoints(child, currentPos, currentLocalTransform);
            }


            joint.animatedTransform = currentLocalTransform * joint.inverseBindTransform;


        }

        private void increaseAnimationTime(float deltaTime)
        {
            animationTime += deltaTime;
            if(animationTime > currentAnimation.length)
            {
                animationTime %= currentAnimation.length;
            }
        }

        private Dictionary<string, Matrix4> calculateCurrentAnimationPose()
        {
            // get pre and next frame
            var allFrame = currentAnimation.keyFrames;
            var preFrame = allFrame[0];
            var nextFrame = allFrame[0];

            for (int i = 0; i < allFrame.Length; i++)
            {
                nextFrame = allFrame[i];
                if (nextFrame.timeStamp > animationTime)
                {
                    break;
                }
                preFrame = allFrame[i];
            }

            // calculate progression
            float delta = nextFrame.timeStamp - preFrame.timeStamp;
            float progress = (animationTime - preFrame.timeStamp) / delta;

            // interpolate poses
            var currentPose = new Dictionary<string, Matrix4>();

            foreach(var keyValue in preFrame.pose)
            {
                var preTransform = preFrame.pose[keyValue.Key];
                var nextTransform = nextFrame.pose[keyValue.Key];
                var currentTransform = JointTransform.Interpolate(preTransform, nextTransform, progress);
                currentPose.Add(keyValue.Key, currentTransform.GetLocalTransform());
            }

            return currentPose;
        }
    }
}
