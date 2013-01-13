using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Elements.Fields
{
    public class Field : Updatable
    {   
        private FieldCellArray cells;

        private PlayerArray players;

        public Field(int width, int height)
        {
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
    }
}
