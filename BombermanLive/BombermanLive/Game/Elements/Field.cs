﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

namespace Bomberman.Game.Elements
{
    public class Field : Drawable, Updatable
    {   
        private FieldCellArray cells;

        public Field(int width, int height)
        {
            cells = new FieldCellArray(width, height);
        }

        public void Draw(Context context)
        {   
        }

        public void Update(float delta)
        {   
        }

        public int GetWidth()
        {
            return cells.GetWidth();
        }

        public int GetHeight()
        {
            return cells.GetHeight();
        }
    }
}
