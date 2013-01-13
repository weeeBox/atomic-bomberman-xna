using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Game;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {   
        private FieldCellArray cells;

        public Field(int width, int height)
        {
            cells = new FieldCellArray(width, height);
        }

        public void Update(float delta)
        {   
        }

        public FieldCellArray GetCells()
        {
            return cells;
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
