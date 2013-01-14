using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Cells;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {   
        private FieldCellArray cells;

        private PlayerArray players;

        private static Field currentField;

        public Field(int width, int height)
        {
            currentField = this;

            cells = new FieldCellArray(width, height);
            players = new PlayerArray();
        }

        public void Update(float delta)
        {
            players.Update(delta);
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public PlayerArray GetPlayers()
        {
            return players;
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

        public void CellPosChanged(MovableCell cell, float oldPx, float oldPy)
        {
            float px = cell.GetPx();
            float py = cell.GetPy();

            Direction direction = cell.GetDirection();
            switch (direction)
            {
                case Direction.LEFT:
                {
                    float minX = 0.0f;
                    if (px < minX)
                    {
                        cell.SetPosX(minX);
                    }
                    break;
                }

                case Direction.RIGHT:
                {
                    float maxX = Constant.FIELD_WIDTH - Constant.CELL_WIDTH;
                    if (px > maxX)
                    {
                        cell.SetPosX(maxX);
                    }
                    break;
                }

                case Direction.UP:
                {
                    float minY = 0.0f;
                    if (py < minY)
                    {
                        cell.SetPosY(minY);
                    }
                    break;
                }

                case Direction.DOWN:
                {
                    float maxY = Constant.FIELD_HEIGHT - Constant.CELL_HEIGHT;
                    if (py > maxY)
                    {
                        cell.SetPosY(maxY);
                    }
                    break;
                }
            }
        }

        public void CellChanged(MovableCell cell, int oldX, int oldY)
        {   
        }

        public static Field Current()
        {
            return currentField;
        }
    }
}
