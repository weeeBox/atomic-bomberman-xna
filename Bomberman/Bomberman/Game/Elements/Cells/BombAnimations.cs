using Assets;
using BomberEngine;
using Bomberman.Content;

namespace Bomberman.Game.Elements.Cells
{
    public class BombAnimations
    {
        public enum AnimationType
        {
            Default,
            Jelly,
            Trigger,
            Dud,

            Count
        }

        private Animation[] m_Animations;

        public BombAnimations()
        {
            InitAnimations();
        }

        public Animation Find(AnimationType type)
        {
            return m_Animations[(int)type];
        }

        private void InitAnimations()
        {
            m_Animations = new Animation[(int)AnimationType.Count];
            m_Animations[(int)AnimationType.Default] = GetAnimation(A.anim_bomb_regular_green);
            m_Animations[(int)AnimationType.Jelly]   = GetAnimation(A.anim_bomb_jelly_green);
            m_Animations[(int)AnimationType.Trigger] = GetAnimation(A.anim_bomb_trigger_green);
            m_Animations[(int)AnimationType.Dud]     = GetAnimation(A.anim_bomb_regular_green_dud);
        }

        private Animation GetAnimation(int id)
        {
            Animation anim = BmApplication.Assets().GetAnimation(id);
            Assert.IsTrue(anim != null);
            return anim;
        }
    }
}
