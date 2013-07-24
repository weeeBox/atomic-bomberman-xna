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
        public enum Mode
        {
            Normal,
            Looped,
        }

        private Animation m_Animation;
        private AnimationGroup m_Group;

        private float m_FrameTime;
        private int m_FrameIndex;
        private float m_SpeedMultiplier;

        private Mode m_Mode;

        public void Init(Animation animation)
        {
            m_Animation = animation;
            Reset();
        }

        public void Update(float delta)
        {
            m_FrameTime += delta * m_SpeedMultiplier;
            if (m_FrameTime >= m_Group.frames[m_FrameIndex].duration) // TODO: handle skipped frames
            {
                m_FrameTime = 0.0f;
                if (m_FrameIndex == m_Group.frames.Length - 1)
                {
                    if (m_Mode == Mode.Looped)
                        m_FrameIndex = 0;
                }
            }
        }

        public void Play(String name)
        {
            Reset();

            AnimationGroup group = m_Animation.Group(name);
            Debug.Assert(group != null, "Can't find group: " + name);
        }

        public void Reset()
        {
            m_FrameIndex = 0;
            m_FrameTime = 0.0f;
            m_Group = null;
            m_SpeedMultiplier = 1.0f;
            m_Mode = Mode.Normal;
        }

        public AnimationGroup Group
        {
            get { return m_Group; }
        }

        public float FrameIndex
        {
            get { return m_FrameIndex; }
        }

        public float SpeedMultiplier
        {
            get { return m_SpeedMultiplier; }
            set { m_SpeedMultiplier = value; }
        }

        public Mode mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
    }
}
