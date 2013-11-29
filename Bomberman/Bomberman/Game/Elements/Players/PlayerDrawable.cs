using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Content;
using Bomberman.Gameplay.Elements.Cells;

namespace Bomberman.Gameplay.Elements.Players
{
    public class PlayerDrawable : IPlayerDrawable
    {
        private Player m_player;
        private PlayerAnimations m_animations;
        private AnimationInstance m_currentAnimation;

        private bool m_needUpdateAnimation;

        private bool m_pickingUpBomb;
        private bool m_punchingBomb;

        public PlayerDrawable(Player player, PlayerAnimations animations)
        {
            player.PlayerDrawable = this;

            m_player = player;
            m_currentAnimation = new AnimationInstance();
            m_animations = animations;

            Reset();
        }

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {
            if (m_needUpdateAnimation)
            {
                UpdateAnimation();
            }

            m_currentAnimation.Update(delta);
        }

        //////////////////////////////////////////////////////////////////////////////

        public void Draw(Context context, float x, float y)
        {
            m_currentAnimation.Draw(context, x, y);
        }

        //////////////////////////////////////////////////////////////////////////////

        public void Reset()
        {
            m_pickingUpBomb = false;
            m_punchingBomb = false;

            UpdateAnimation();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Animations

        public void SetNeedUpdateAnimation()
        {
            m_needUpdateAnimation = true;
        }

        private void UpdateAnimation()
        {
            PlayerAnimations.Id id;
            PlayerAnimations.Id currentId = (PlayerAnimations.Id)m_currentAnimation.id;
            AnimationInstance.Mode mode = AnimationInstance.Mode.Looped;

            if (m_player.IsAlive)
            {
                if (m_punchingBomb)
                {
                    if (currentId == PlayerAnimations.Id.PunchBomb)
                    {
                        return; // don't play animation again
                    }

                    id = PlayerAnimations.Id.PunchBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (m_pickingUpBomb)
                {
                    id = PlayerAnimations.Id.PickupBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (m_player.IsHoldingBomb())
                {
                    id = m_player.IsMoving() ? PlayerAnimations.Id.WalkBomb : PlayerAnimations.Id.StandBomb;
                    mode = AnimationInstance.Mode.Normal;
                }
                else if (m_player.IsMoving())
                {
                    id = PlayerAnimations.Id.Walk;
                    Animation newAnimation = m_animations.Find(id, m_player.direction);
                    if (m_currentAnimation.Animation == newAnimation)
                    {
                        return;
                    }
                }
                else
                {
                    id = PlayerAnimations.Id.Stand;
                }
            }
            else
            {
                id = PlayerAnimations.Id.Die;
                mode = AnimationInstance.Mode.Normal;
            }

            Animation animation = m_animations.Find(id, m_player.direction);
            m_currentAnimation.Init(animation, mode);
            m_currentAnimation.id = (int)id;
            m_currentAnimation.animationDelegate = AnimationFinishedCallback;

            m_needUpdateAnimation = false;
        }

        private void AnimationFinishedCallback(AnimationInstance animation)
        {
            PlayerAnimations.Id id = (PlayerAnimations.Id)animation.id;
            switch (id)
            {
                case PlayerAnimations.Id.PunchBomb:
                {
                    m_punchingBomb = false;
                    break;
                }

                case PlayerAnimations.Id.PickupBomb:
                {
                    m_punchingBomb = false;
                    break;
                }
            }

            m_player.OnAnimationFinished(id);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Player Player
        {
            get { return m_player;}
        }

        public PlayerAnimations Animations
        {
            get { return m_animations; }
            set { m_animations = value; }
        }

        public bool IsPickingUpBomb
        {
            get { return m_pickingUpBomb; }
            set 
            {
                m_needUpdateAnimation = m_pickingUpBomb != value;
                m_pickingUpBomb = value; 
            }
        }

        public bool IsPunchingBomb
        {
            get { return m_punchingBomb; }
            set 
            {
                m_needUpdateAnimation = m_punchingBomb != value;
                m_punchingBomb = value; 
            }
        }

        #endregion
    }
}
