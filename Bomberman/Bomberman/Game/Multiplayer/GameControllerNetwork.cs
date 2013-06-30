using System;
using System.Collections.Generic;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Multiplayer;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Game.Multiplayer
{
    public abstract class GameControllerNetwork : GameController
    {
        public GameControllerNetwork(GameSettings settings)
            : base(settings)
        {
        }

        protected override void OnStop()
        {
            StopNetworkPeer();
            base.OnStop();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Protocol

        protected void WriteFieldState(NetOutgoingMessage response, Player senderPlayer)
        {
            Field field = game.field;
            Debug.Assert(field != null);

            // powerups
            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    response.Write((byte)brick.powerup);
                }
            }

            // players
            List<Player> players = game.GetPlayers().list;

            int senderIndex = players.IndexOf(senderPlayer);
            Debug.Assert(senderIndex != -1);
            response.Write((byte)senderIndex);

            response.Write((byte)players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                Player player = players[i];
                response.Write((byte)player.cx);
                response.Write((byte)player.cy);
            }
        }

        protected void ReadFieldState(NetIncomingMessage response)
        {
            Field field = game.field;
            Debug.Assert(field != null);

            // powerups
            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    brick.powerup = response.ReadByte();
                }
            }

            // players
            int senderIndex = response.ReadByte();
            int playersCount = response.ReadByte();
            for (int i = 0; i < playersCount; ++i)
            {
                Player player = new Player(i);
                int cx = response.ReadByte();
                int cy = response.ReadByte();
                player.SetCell(cx, cy);
                if (senderIndex != i)
                {
                    player.SetPlayerInput(new PlayerNetworkInput());
                }

                game.AddPlayer(player);
            }
        }

        protected void WritePlayerActions(NetOutgoingMessage response, PlayerInput input)
        {
            int mask = 0;
            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                if (input.IsActionPressed(i))
                {
                    mask |= 1 << i;
                }
            }
            response.Write(mask, actionsCount);
        }

        protected void ReadPlayerActions(NetIncomingMessage response, PlayerNetworkInput input)
        {
            int actionsCount = (int)PlayerAction.Count;
            int mask = response.ReadInt32(actionsCount);
            for (int i = 0; i < actionsCount; ++i)
            {
                PlayerAction action = (PlayerAction)i;
                if ((mask & (1 << i)) == 0)
                {
                    input.OnActionReleased(action);
                }
            }
            for (int i = 0; i < actionsCount; ++i)
            {
                PlayerAction action = (PlayerAction)i;
                if ((mask & (1 << i)) != 0)
                {
                    input.OnActionPressed(action);
                }
            }
        }

        protected void WritePlayersPositions(NetOutgoingMessage msg, List<Player> players)
        {
            int count = players.Count;
            msg.Write((byte)count);
            for (int i = 0; i < count; ++i)
            {
                Player player = players[i];
                msg.Write(player.px);
                msg.Write(player.py);
            }
        }

        protected void ReadPlayersPositions(NetIncomingMessage msg, List<Player> players)
        {
            int count = msg.ReadByte();
            Debug.Assert(players.Count == count);

            for (int i = 0; i < count; ++i)
            {
                Player player = players[i];

                float px = msg.ReadFloat();
                float py = msg.ReadFloat();

                player.SetPos(px, py);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Network helpers

        protected NetOutgoingMessage CreateMessage(NetworkMessageId messageId)
        {
            return GetMultiplayerManager().CreateMessage(messageId);
        }

        protected void SendMessage(NetOutgoingMessage message, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(message, recipient, method);
        }

        protected void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(messageId, method);
        }

        protected void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(message, method);
        }

        protected void SendMessage(NetworkMessageId messageId, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(messageId, recipient, method);
        }

        protected void StopNetworkPeer()
        {
            GetMultiplayerManager().Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Connection screen

        protected void StartConnectionScreen(ConnectionCancelCallback cancelCallback, String message)
        {
            NetworkConnectionScreen screen = new NetworkConnectionScreen(message);
            screen.cancelCallback = cancelCallback;
            StartNextScreen(screen);
        }

        protected void HideConnectionScreen()
        {
            NetworkConnectionScreen screen = CurrentScreen() as NetworkConnectionScreen;
            if (screen != null)
            {
                screen.cancelCallback = null;
                screen.Finish();
            }
        }

        #endregion
    }
}
