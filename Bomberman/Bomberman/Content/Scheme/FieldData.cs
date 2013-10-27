using System;

namespace BombermanCommon.Resources.Scheme
{
    public class FieldData
    {
        private FieldBlocks[] data;
        private int width;
        private int height;

        public FieldData(int width, int height)
            : this(width, height, new FieldBlocks[width * height])
        {  
        }

        public FieldData(int width, int height, FieldBlocks[] data)
        {
            if (data.Length != width * height)
            {
                throw new ArgumentException("Invalid data array size");
            }

            this.width = width;
            this.height = height;

            this.data = data;
        }

        public FieldBlocks Get(int x, int y)
        {
            int index = y * width + x;
            return data[index];
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
