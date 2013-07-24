using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;

namespace Bomberman.Content
{
    public class AnimationInstance : IUpdatable, IResettable
    {
        private Animation mAnimation;
        private AnimationGroup mGroup;

        private float mFrameTime;
        private int mFrameIndex;
        private float mSpeedMultiplier;

        public AnimationInstance(Animation animation)
        {
            mAnimation = animation;
            Reset();
        }

        public void Update(float delta)
        {
            mFrameTime += delta * mSpeedMultiplier;
            if (mFrameTime >= mGroup.frames[mFrameIndex].duration) // TODO: handle skipped frames
            {
                mFrameTime = 0.0f;
                mFrameIndex = (mFrameIndex + 1) % mGroup.frames.Length;
            }
        }

        public void Play(String name)
        {
            Reset();

            AnimationGroup group = mAnimation.Group(name);
            Debug.Assert(group != null, "Can't find group: " + name);
        }

        public void Reset()
        {
            mFrameIndex = 0;
            mFrameTime = 0.0f;
            mGroup = null;
            mSpeedMultiplier = 1.0f;
        }

        public AnimationGroup Group
        {
            get { return mGroup; }
        }

        public float FrameIndex
        {
            get { return mFrameIndex; }
        }

        public float SpeedMultiplier
        {
            get { return mSpeedMultiplier; }
            set { mSpeedMultiplier = value; }
        }
    }
}
