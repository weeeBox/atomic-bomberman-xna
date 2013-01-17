using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanCommon.Resources.Scheme
{
    public class FieldData
    {
        private FieldBlocks[] data;
        private int width;
        private int height;

        public FieldData(int width, int height)
        {
            this.width = width;
            this.height = height;

            data = new FieldBlocks[width * height];
        }

        public void Set(int x, int y, FieldBlocks block)
        {
            int index = y * width + x;
            data[index] = block;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public FieldBlocks[] GetDataArray()
        {
            return data;
        }
    }
}
