using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;

namespace Bomberman.Gameplay.Elements.Players
{
    public abstract class PlayerBitArrayInput : PlayerInput
    {
        private BitArray m_actionsArray;

        public PlayerBitArrayInput()
        {
            m_actionsArray = new BitArray((int)PlayerAction.Count);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            for (int i = 0; i < m_actionsArray.length; ++i)
            {
                SetActionPressed(i, m_actionsArray[i]);
            }
        }

        public override void Reset()
        {
            base.Reset();
            m_actionsArray.Clear();
        }

        public override void Reset(int mask)
        {
            base.Reset(mask);
            m_actionsArray.value = mask;
        }

        public override void Force(int mask)
        {
            base.Force(mask);
            m_actionsArray.value = mask;
        }

        internal BitArray actionsArray
        {
            get { return m_actionsArray; }
        }
    }
}
