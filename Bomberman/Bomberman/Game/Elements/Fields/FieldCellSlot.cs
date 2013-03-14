using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine;
using BomberEngine.Debugging;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCellSlot
    {
        private static FieldCellType[] STATIC_CELL_TYPES = 
        {
            FieldCellType.Solid,
            FieldCellType.Brick,
            FieldCellType.Powerup,
        };

        public int cx;
        public int cy;

        public FieldCell cell;
        public LinkedList<Player> players;

        public FieldCellSlot(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;

            players = new LinkedList<Player>();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Add/Remove

        public void AddCell(FieldCell c)
        {
            if (c.IsPlayer())
            {
                Debug.Assert(Debug.flag && !players.Contains(c));
                players.AddLast(c.AsPlayer());
            }
            else
            {
                Debug.Assert(cell == null);
                cell = c;
            }
        }

        public void RemoveCell(FieldCell c)
        {
            if (c.IsPlayer())
            {
                Debug.Assert(Debug.flag && players.Contains(c));
                players.Remove(c.AsPlayer());
            }
            else
            {
                Debug.Assert(cell != null);
                cell = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        public int Size()
        {
            return players.Count + (cell != null ? 1 : 0);
        }

        public int PlayersCount()
        {
            return players.Count;
        }

        //public bool ContainsSolid()
        //{
        //    return Contains(FieldCellType.Solid);
        //}

        public SolidCell GetSolid()
        {
            return cell != null ? cell.AsSolid() : null;
        }

        //public bool ContainsBrick()
        //{
        //    return Contains(FieldCellType.Brick);
        //}

        public BrickCell GetBrick()
        {
            return cell != null ? cell.AsBrick() : null;
        }

        //public bool ContainsPowerup()
        //{
        //    return Contains(FieldCellType.Powerup);
        //}

        public PowerupCell GetPowerup()
        {
            return cell != null ? cell.AsPowerup() : null;
        }

        //public bool ContainsFlame()
        //{
        //    return Contains(FieldCellType.Flame);
        //}

        public FlameCell GetFlame()
        {
            return cell != null ? cell.AsFlame() : null;
        }

        //public bool ContainsBomb()
        //{
        //    return Contains(FieldCellType.Bomb);
        //}

        public Bomb GetBomb()
        {
            return cell != null ? cell.AsBomb() : null;
        }

        //public bool ContainsPlayer()
        //{
        //    return Contains(FieldCellType.Player);
        //}

        //public Player GetPlayer()
        //{
        //    return (Player)Get(FieldCellType.Player);
        //}

        public bool Contains(FieldCell c)
        {
            if (cell != null)
            {
                if (cell == c)
                {
                    return true;
                }

                if (c.IsPlayer() && players.Contains(c.AsPlayer()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsObstacle()
        {
            return cell != null && cell.IsObstacle();
        }

        public FieldCell GetStaticCell()
        {
            if (cell != null)
            {
                for (int i = 0; i < STATIC_CELL_TYPES.Length; ++i)
                {
                    if (cell.type == STATIC_CELL_TYPES[i])
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
