using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework;

namespace Bomberman.Content
{
    public class AnimationInstance : IUpdatable, IResettable // TODO: make it a View subclass
    {
        public enum Mode
        {
            Normal,
            Looped,
        }

        private Animation m_animation;

        private float m_frameTime;
        private int m_frameIndex;
        private float m_speedMultiplier;

        private Mode m_mode;

        public void Init(Animation animation)
        {
            Reset();
            m_animation = animation;
        }

        public void Update(float delta)
        {
            m_frameTime += delta * m_speedMultiplier;
            if (m_frameTime >= m_animation.frames[m_frameIndex].duration) // TODO: handle skipped frames
            {
                m_frameTime = 0.0f;
                if (m_frameIndex == m_animation.frames.Length - 1)
                {
                    if (m_mode == Mode.Looped)
                        m_frameIndex = 0;
                }
                else
                {
                    ++m_frameIndex;
                }
            }
        }

        public void Draw(Context context, float x, float y)
        {
            m_animation.Draw(context, m_frameIndex, x, y);
        }

        public void Reset()
        {
            m_frameIndex = 0;
            m_frameTime = 0.0f;
            m_animation = null;
            m_speedMultiplier = 1.0f;
            m_mode = Mode.Looped;
        }

        public Animation Animation
        {
            get { return m_animation; }
        }

        public TextureImage Texture
        {
            get { return m_animation.texture; }
        }

        public int FrameIndex
        {
            get { return m_frameIndex; }
        }

        public float SpeedMultiplier
        {
            get { return m_speedMultiplier; }
            set { m_speedMultiplier = value; }
        }

        public Mode mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }
    }
}
