using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanCommon.Resources.Scheme
{
    public class FieldData
    {
        private EnumBlocks[] data;
        private int width;
        private int height;

        public FieldData(int width, int height)
        {
            this.width = width;
            this.height = height;

            data = new EnumBlocks[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = EnumBlocks.BLOCK_BLANK;
            }
        }

        public EnumBlocks[] Data
        {
            get { return data; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public void Set(int x, int y, EnumBlocks block)
        {
            int index = y * width + x;
            data[index] = block;
        }
    }
}
