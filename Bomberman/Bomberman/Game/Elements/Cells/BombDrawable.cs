using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using Bomberman.Content;
using Microsoft.Xna.Framework;

namespace Bomberman.Gameplay.Elements.Cells
{
    public class BombDrawable : IBombDrawable, IUpdatable
    {
        private Bomb m_bomb;

        private BombAnimations m_animations;
        private AnimationInstance m_currentAnimation;
        private bool m_needsUpdateAnimation;

        public BombDrawable(Bomb bomb, BombAnimations animations)
        {
            bomb.BombDrawable = this;

            m_bomb = bomb;
            m_currentAnimation = new AnimationInstance();
            m_animations = animations;
            Reset();
        }

        public void Update(float delta)
        {
            if (m_needsUpdateAnimation)
            {
                UpdateAnimation();
            }
            m_currentAnimation.Update(delta);
        }

        public void Draw(Context context, float x, float y)
        {
            m_currentAnimation.Draw(context, x, y, m_bomb.IsBlocked ? Color.Red : Color.White);
        }

        public void Reset()
        {
            UpdateAnimation();
        }

        public void SetNeedUpdateAnimation()
        {
            m_needsUpdateAnimation = true;
        }

        private void UpdateAnimation()
        {
            BombAnimations.AnimationType type;
            if (m_bomb.isTrigger)
            {
                type = BombAnimations.AnimationType.Trigger;
            }
            else if (m_bomb.IsJelly())
            {
                type = BombAnimations.AnimationType.Jelly;
            }
            else
            {
                type = BombAnimations.AnimationType.Default;
            }

            Animation animation = m_animations.Find(type);
            m_currentAnimation.Init(animation);

            m_needsUpdateAnimation = false;
        }

        public BombAnimations Animations
        {
            get { return m_animations; }
            set { m_animations = value; }
        }
    }
}
