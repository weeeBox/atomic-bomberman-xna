using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using Bomberman.Content;

namespace Bomberman.Gameplay.Elements.Cells
{
    public class BombDrawable : IUpdatable, IDrawable
    {
        private Bomb m_bomb;
        private BombAnimations m_animations;
        private AnimationInstance m_currentAnimation;

        public BombDrawable(Bomb bomb)
        {   
            m_bomb = bomb;
            m_currentAnimation = new AnimationInstance();
        }

        public void Update(float delta)
        {
            m_currentAnimation.Update(delta);
        }

        public void Draw(Context context)
        {
            throw new NotImplementedException();
        }
    }
}
