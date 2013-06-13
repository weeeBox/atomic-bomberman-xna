using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;

namespace Bomberman.Network
{
    public class GameNetwork
    {
        private const byte BLOCK_EMPTY = 0;
        private const byte BLOCK_SOLID = 1;
        private const byte BLOCK_BRICK = 2;

        private const byte NO_POWERUP = (byte)0xff;

        public static void WriteFieldState(BitWriteBuffer buffer)
        {
            Field field = Field.Current();

            FieldCellArray cells = field.GetCells();
            int width = cells.GetWidth();
            int height = cells.GetHeight();

            buffer.Write(width);
            buffer.Write(height);

            FieldCellSlot[] slots = cells.slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCell staticCell = slots[i].staticCell;
                if (staticCell != null)
                {
                    if (staticCell.IsSolid())
                    {
                        buffer.Write(BLOCK_SOLID);
                    }
                    else if (staticCell.IsBrick())
                    {
                        BrickCell brick = staticCell.AsBrick();
                        byte powerup = brick.powerup != Powerups.None ? (byte)brick.powerup : NO_POWERUP;

                        buffer.Write(BLOCK_BRICK);
                        buffer.Write(powerup);
                    }
                }
                else
                {
                    buffer.Write(BLOCK_EMPTY);
                }
            }
        }

        public static void ReadFieldState(BitReadBuffer buffer)
        {
            Field field = Field.Current();

            int width = buffer.ReadInt32();
            int height = buffer.ReadInt32();

            int blocksCount = width * height;
            for (int i = 0; i < blocksCount; ++i)
            {
                byte blockType = buffer.ReadByte();
                if (blockType == BLOCK_EMPTY)
                {

                }
                else if (blockType == BLOCK_BRICK)
                {
                    byte powerup = buffer.ReadByte();
                    if (powerup != NO_POWERUP)
                    {

                    }
                }
                else if (blockType == BLOCK_SOLID)
                {

                }
            }
        }
    }
}
