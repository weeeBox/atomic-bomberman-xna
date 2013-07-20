using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerList : IUpdatable, IKeyInputListener
    {
        public List<Player> list;

        private KeyInputListenerList keyListeners;
        private UpdatableList updatables;

        public PlayerList(int capacity)
        {
            list = new List<Player>(capacity);
            keyListeners = new KeyInputListenerList(capacity);
            updatables = new UpdatableList(capacity);
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
            if (player.input is IKeyInputListener)
            {
                keyListeners.Add(player.input as IKeyInputListener);
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
            if (player.input is IKeyInputListener)
            {
                keyListeners.Remove(player.input as IKeyInputListener);
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

        #region IKeyInputListener

        public bool OnKeyPressed(KeyEventArg arg)
        {
            return keyListeners.OnKeyPressed(arg);
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            return keyListeners.OnKeyRepeated(arg);
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            return keyListeners.OnKeyReleased(arg);
        }

        #endregion
    }
}
