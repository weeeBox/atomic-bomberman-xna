using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Content
{
    public class AnimationInstance : IUpdatable, IResettable
    {
        public enum Mode
        {
            Normal,
            Looped,
        }

        private AnimationGroup m_Group;
        private TextureImage m_Texture;

        private float m_FrameTime;
        private int m_FrameIndex;
        private float m_SpeedMultiplier;

        private Mode m_Mode;

        public void Init(AnimationGroup group, TextureImage texture)
        {
            Reset();

            m_Group = group;
            m_Texture = texture;
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
                else
                {
                    ++m_FrameIndex;
                }
            }
        }

        public void Reset()
        {
            m_FrameIndex = 0;
            m_FrameTime = 0.0f;
            m_Group = null;
            m_Texture = null;
            m_SpeedMultiplier = 1.0f;
            m_Mode = Mode.Looped;
        }

        public AnimationGroup Group
        {
            get { return m_Group; }
        }

        public TextureImage Texture
        {
            get { return m_Texture; }
        }

        public int FrameIndex
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
