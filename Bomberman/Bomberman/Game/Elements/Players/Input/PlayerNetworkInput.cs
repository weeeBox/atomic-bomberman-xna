
namespace Bomberman.Gameplay.Elements.Players
{
    public class PlayerNetworkInput : PlayerBitArrayInput
    {
        public void SetNetActionBits(int value)
        {
            actionsArray.value = value;
        }

        public override bool IsLocal
        {
            get { return false; }
        }
    }
}
