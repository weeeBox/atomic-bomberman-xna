using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Debugging;
using BomberEngine.Core.Events;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerList : IUpdatable, IEventHandler
    {
        public List<Player> list;

        private UpdatableList updatables;
        private EventHandlerList eventHandlers;

        public PlayerList(int capacity)
        {
            list = new List<Player>(capacity);
            
            updatables = new UpdatableList(capacity);
            eventHandlers = new EventHandlerList(capacity);
        }

        public void Update(float delta)
        {
            updatables.Update(delta);
        }

        public void Add(Player player)
        {
            Debug.Assert(!list.Contains(player));
            list.Add(player);

            updatables.Add(player);
            if (player.input is IEventHandler)
            {
                eventHandlers.Add(player.input as IEventHandler);
            }
        }

        public void Remove(Player player)
        {
            Remove(player, true);
        }

        public void Kill(Player player)
        {
            Remove(player, false);
        }

        private void Remove(Player player, bool removeFromList)
        {
            Debug.Assert(list.Contains(player));
            if (removeFromList)
            {
                list.Remove(player);
            }

            updatables.Remove(player);
            if (player.input is IEventHandler)
            {
                eventHandlers.Remove(player.input as IEventHandler);
            }
        }

        public Player TryGet(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < list.Count)
            {
                return list[playerIndex];
            }

            return null;
        }

        public Player Get(int playerIndex)
        {
            return list[playerIndex];
        }

        public int GetCount()
        {
            return list.Count;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IEventHandler

        public bool HandleEvent(Event evt)
        {
            return eventHandlers.HandleEvent(evt);
        }

        #endregion
    }
}
