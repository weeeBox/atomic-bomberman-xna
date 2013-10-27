
namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerNetworkInput : PlayerInput
    {
        private int m_netActionBits;

        public override void Update(float delta)
        {
            base.Update(delta);

            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                SetActionPressed(i, (m_netActionBits & (1 << i)) != 0);
            }
        }

        public void SetNetActionBits(int bits)
        {
            m_netActionBits = bits;
        }
    }
}
