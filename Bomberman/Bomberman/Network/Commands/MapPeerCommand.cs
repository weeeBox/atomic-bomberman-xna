using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;

namespace Bomberman.Network.Requests
{
    public class MapPeerCommand : PeerCommand
    {
        private const byte BLOCK_EMPTY = 0;
        private const byte BLOCK_SOLID = 1;
        private const byte BLOCK_BRICK = 2;

        public Field field;

        public MapPeerCommand()
            : base((byte)PeerCommandID.Map)
        {
        }

        protected override void WriteData(NetBuffer stream)
        {
            FieldCellArray cells = field.GetCells();
            int width = cells.GetWidth();
            int height = cells.GetHeight();

            stream.Write(width);
            stream.Write(height);

            FieldCellSlot[] slots = cells.slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCell staticCell = slots[i].staticCell;
                if (staticCell != null)
                {
                    if (staticCell.IsSolid())
                    {
                        stream.Write(BLOCK_SOLID);
                    }
                    else if (staticCell.IsBrick())
                    {   
                        BrickCell brick = staticCell.AsBrick();
                        byte powerup = brick.powerup != Powerups.None ? (byte)brick.powerup : (byte)0xff;

                        stream.Write(BLOCK_BRICK);
                        stream.Write(powerup);
                    }
                }
                else
                {
                    stream.Write(BLOCK_EMPTY);
                }
            }
        }

        protected override void ReadData(NetBuffer stream)
        {
        }
    }
}
