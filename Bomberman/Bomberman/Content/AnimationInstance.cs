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
    public delegate void AnimationInstanceDelegate(AnimationInstance instance);

    public class AnimationInstance : IUpdatable, IResettable // TODO: make it a View subclass
    {
        public enum Mode
        {
            Normal,
            Looped,
        }

        private int m_id;
        private Animation m_animation;

        private float m_frameTime;
        private int m_frameIndex;
        private float m_speedMultiplier;

        private Mode m_mode;
        private AnimationInstanceDelegate m_delegate;
        private Object m_userData;

        public void Init(Animation animation, Mode mode = Mode.Looped)
        {
            Reset();
            m_animation = animation;
            m_mode = mode;
        }

        public void Update(float delta)
        {
            m_frameTime += delta * m_speedMultiplier;
            if (m_frameTime >= m_animation.frames[m_frameIndex].duration) // TODO: handle skipped frames
            {
                m_frameTime = 0.0f;
                if (m_frameIndex == m_animation.frames.Length - 1)
                {
                    if (m_mode == Mode.Normal)
                    {
                        if (m_delegate != null)
                        {
                            m_delegate(this);
                        }
                    }
                    if (m_mode == Mode.Looped)
                    {
                        m_frameIndex = 0;
                    }
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
            m_delegate = null;
            m_id = 0;
            m_userData = null;
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
        }

        public AnimationInstanceDelegate animationDelegate
        {
            get { return m_delegate; }
            set { m_delegate = value; }
        }

        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public Object userData
        {
            get { return m_userData; }
            set { m_userData = value; }
        }
    }
}
