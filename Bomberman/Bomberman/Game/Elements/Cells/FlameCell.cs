using BomberEngine;
using Bomberman.Gameplay.Elements.Fields;
using Bomberman.Gameplay.Elements.Players;

namespace Bomberman.Gameplay.Elements.Cells
{
    public class FlameCell : FieldCell
    {
        private const int BitIsCenter   = 0;
        private const int BitIsCap      = 1;
        private const int BitIsShort    = 2;
        private const int BitIsGolden   = 3;

        private const int ShiftDirection = 4;
        private const int MaskDirection  = 0x7; // binary: 111 (3 bits to hold 4 direction values)

        private float m_remains;
        private Player m_player;

        private int m_flag;

        public FlameCell(Player player, int cx, int cy)
            : base(FieldCellType.Flame, cx, cy)
        {
            m_player = player;
            m_remains = CVars.cg_timeFlame.intValue * 0.001f;
        }

        public override void Update(float delta)
        {
            m_remains -= delta;
            if (m_remains <= 0)
            {
                GetField().RemoveCell(this);
            }
        }

        public override bool IsFlame()
        {
            return true;
        }

        public override FlameCell AsFlame()
        {
            return this;
        }

        public float remains
        {
            get { return m_remains; }
        }

        public Player player
        {
            get { return m_player; }
        }

        #region Flags

        public bool isCenter
        {
            get { return BitUtils.GetBit(m_flag, BitIsCenter); }
            set { m_flag = BitUtils.SetBit(m_flag, BitIsCenter, value); }
        }

        public bool isCap
        {
            get { return BitUtils.GetBit(m_flag, BitIsCap); }
            set { m_flag = BitUtils.SetBit(m_flag, BitIsCap, value); }
        }

        public bool isShort
        {
            get { return BitUtils.GetBit(m_flag, BitIsShort); }
            set { m_flag = BitUtils.SetBit(m_flag, BitIsShort, value); }
        }

        public bool isGolden
        {
            get { return BitUtils.GetBit(m_flag, BitIsGolden); }
            set { m_flag = BitUtils.SetBit(m_flag, BitIsGolden, value); }
        }

        public Direction direction
        {
            get 
            {
                int intValue = (m_flag >> ShiftDirection) & MaskDirection;
                return (Direction)intValue;
            }
            set 
            {
                int intValue = (int)value;
                m_flag &= ~(MaskDirection << ShiftDirection);           // clear old value
                m_flag |= (intValue & MaskDirection) << ShiftDirection; // set new value
            }
        }

        #endregion
    }
}
