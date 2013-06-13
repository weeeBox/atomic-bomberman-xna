using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine.Debugging;

namespace Bomberman.Network.Requests
{
    public enum NetworkMessageID
    {
        FieldStateRequest,
        FieldStateResponse,
    }

    //////////////////////////////////////////////////////////////////////////////

    public abstract class NetworkMessage
    {
        public NetworkMessageID id;

        protected NetworkMessage(NetworkMessageID id)
        {
            this.id = id;
        }   

        public abstract void Write(BitWriteBuffer writer);
        public abstract void Read(BitReadBuffer reader);
    }

    //////////////////////////////////////////////////////////////////////////////

    public class MsgFieldStateRequest : NetworkMessage
    {
        public Player player;

        public MsgFieldStateRequest()
            : base(NetworkMessageID.FieldStateRequest)
        {
        }

        public override void Write(BitWriteBuffer writer)
        {   
        }

        public override void Read(BitReadBuffer reader)
        {
        }
    }

    public class MsgFieldStateResponse : NetworkMessage
    {
        private const byte BLOCK_EMPTY = 0;
        private const byte BLOCK_SOLID = 1;
        private const byte BLOCK_BRICK = 2;

        public Field field;

        public MsgFieldStateResponse()
            : base(NetworkMessageID.FieldStateResponse)
        {
        }

        public override void Write(BitWriteBuffer writer)
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

        public override void Read(BitReadBuffer reader)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            Log.i("Field: " + width + "x" + height);
        }
    }
}
