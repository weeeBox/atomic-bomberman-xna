using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Network.Requests
{
    public enum NetworkMessageID
    {
        FieldState,
    }

    //////////////////////////////////////////////////////////////////////////////

    public abstract class NetworkMessage
    {
        public NetworkMessageID id;

        protected NetworkMessage(NetworkMessageID id)
        {
            this.id = id;
        }   

        public void Write(BufferWriter writer)
        {
            writer.Write((byte)id);
            WriteData(writer);
        }

        public void Read(BufferReader reader)
        {
            ReadData(reader);
        }

        protected abstract void WriteData(BufferWriter writer);
        protected abstract void ReadData(BufferReader reader);
    }

    //////////////////////////////////////////////////////////////////////////////

    public class MsgFieldState : NetworkMessage
    {
        private const byte BLOCK_EMPTY = 0;
        private const byte BLOCK_SOLID = 1;
        private const byte BLOCK_BRICK = 2;

        public Field field;

        public MsgFieldState()
            : base((byte)NetworkMessageID.FieldState)
        {
        }

        protected override void WriteData(BufferWriter writer)
        {
            FieldCellArray cells = field.GetCells();
            int width = cells.GetWidth();
            int height = cells.GetHeight();

            writer.Write(width);
            writer.Write(height);

            FieldCellSlot[] slots = cells.slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCell staticCell = slots[i].staticCell;
                if (staticCell != null)
                {
                    if (staticCell.IsSolid())
                    {
                        writer.Write(BLOCK_SOLID);
                    }
                    else if (staticCell.IsBrick())
                    {
                        BrickCell brick = staticCell.AsBrick();
                        byte powerup = brick.powerup != Powerups.None ? (byte)brick.powerup : (byte)0xff;

                        writer.Write(BLOCK_BRICK);
                        writer.Write(powerup);
                    }
                }
                else
                {
                    writer.Write(BLOCK_EMPTY);
                }
            }
        }

        protected override void ReadData(BufferReader reader)
        {
        }
    }
}
