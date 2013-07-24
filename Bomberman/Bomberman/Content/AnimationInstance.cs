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

        private Animation m_Animation;

        private float m_FrameTime;
        private int m_FrameIndex;
        private float m_SpeedMultiplier;

        private Mode m_Mode;

        public void Init(Animation animation)
        {
            Reset();
            m_Animation = animation;
        }

        public void Update(float delta)
        {
            m_FrameTime += delta * m_SpeedMultiplier;
            if (m_FrameTime >= m_Animation.frames[m_FrameIndex].duration) // TODO: handle skipped frames
            {
                m_FrameTime = 0.0f;
                if (m_FrameIndex == m_Animation.frames.Length - 1)
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
            m_Animation = null;
            m_SpeedMultiplier = 1.0f;
            m_Mode = Mode.Looped;
        }

        public Animation Animation
        {
            get { return m_Animation; }
        }

        public TextureImage Texture
        {
            get { return m_Animation.texture; }
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
