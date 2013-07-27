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
using Bomberman.Game.Elements;

namespace Bomberman.Game.Multiplayer
{
    public struct ClientPacket
    {
        public int id;
        public int lastAckId;
        public float timeStamp;
        public int actions;
    }

    public struct ServerPacket
    {
        public int id;
        public int lastAckId;
        public float timeStamp;
    }

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

        private const byte CELL_FLAME   = 0;
        private const byte CELL_BRICK   = 1;
        private const byte CELL_POWERUP = 2;

        private static readonly int BITS_FOR_STATIC_CELL = NetUtility.BitsToHoldUInt(2);
        private static readonly int BITS_FOR_POWERUP = NetUtility.BitsToHoldUInt(Powerups.Count - 1);

        protected void WriteFieldState(NetOutgoingMessage response, Player senderPlayer)
        {
            Field field = game.Field;
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
            Field field = game.Field;
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

        protected void WriteClientPacket(NetOutgoingMessage msg, ref ClientPacket packet)
        {
            msg.Write(packet.id);
            msg.Write(packet.lastAckId);
            msg.WriteTime(packet.timeStamp, false);
            msg.Write(packet.actions, (int)PlayerAction.Count);
        }

        protected ClientPacket ReadClientPacket(NetIncomingMessage msg)
        {
            ClientPacket packet;
            packet.id = msg.ReadInt32();
            packet.lastAckId = msg.ReadInt32();
            packet.timeStamp = (float)msg.ReadTime(false);
            packet.actions = msg.ReadInt32((int)PlayerAction.Count);

            return packet;
        }

        protected void WriteServerPacket(NetBuffer buffer)
        {
            buffer.WriteTime(false);

            WriteServerPacket(buffer, game.Field);

            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                WriteServerPacket(buffer, players[i]);
            }
        }

        private void WriteServerPacket(NetBuffer buffer, Field field)
        {
            int bitsForPlayerIndex = NetUtility.BitsToHoldUInt((uint)(field.GetPlayers().GetCount()-1));

            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCell staticCell = slots[i].staticCell;
                bool shouldWrite = staticCell != null && !staticCell.IsSolid();
                buffer.Write(shouldWrite);
                if (shouldWrite)
                {   
                    switch (staticCell.type)
                    {
                        case FieldCellType.Brick:
                        {
                            BrickCell brick = staticCell.AsBrick();
                            bool hasPowerup = brick.powerup != Powerups.None;

                            buffer.Write(CELL_BRICK, BITS_FOR_STATIC_CELL);
                            buffer.Write(hasPowerup);
                            if (hasPowerup)
                            {
                                buffer.Write(brick.powerup, BITS_FOR_POWERUP);
                            }
                            break;
                        }
                        case FieldCellType.Powerup:
                        {
                            PowerupCell powerup = staticCell.AsPowerup();
                            buffer.Write(CELL_POWERUP, BITS_FOR_STATIC_CELL);
                            buffer.Write(powerup.powerup, BITS_FOR_POWERUP);
                            break;
                        }
                        case FieldCellType.Flame:
                        {
                            FlameCell flame = staticCell.AsFlame();
                            buffer.Write(CELL_FLAME, BITS_FOR_STATIC_CELL);
                            buffer.Write(flame.player.GetIndex(), bitsForPlayerIndex);
                            break;
                        }
                    }
                }
            }
        }

        private void WriteServerPacket(NetBuffer buffer, Player p)
        {
            bool alive = p.IsAlive();
            buffer.Write(alive);
            if (alive)
            {
                buffer.Write(p.px);
                buffer.Write(p.py);
                buffer.Write((byte)p.direction);
                buffer.Write(p.GetSpeed());

                // powerups
                int powerupsCount = (int)Powerups.Count;
                for (int i = 0; i < powerupsCount; ++i)
                {
                    bool hasPower = p.powerups.HasPowerup(i);
                    buffer.Write(hasPower);
                    if (hasPower)
                    {   
                        buffer.Write((byte)p.powerups.GetCount(i));
                    }
                }

                // diseases
                int diseasesCount = (int)Diseases.Count;
                for (int i = 0; i < diseasesCount; ++i)
                {
                    bool infected = p.diseases.IsInfected(i);
                    buffer.Write(infected);
                    if (infected)
                    {
                        buffer.WriteTime(NetTime.Now + p.diseases.GetInfectedRemains(i), false);
                    }
                }
            }

            Bomb[] bombs = p.bombs.array;
            for (int i = 0; i < bombs.Length; ++i)
            {
                WriteServerPacket(buffer, bombs[i]);
            }
        }

        private void WriteServerPacket(NetBuffer msg, Bomb b)
        {
            msg.Write(b.isActive);
            if (b.isActive)
            {
                msg.WriteTime(NetTime.Now + b.timeRemains, false);
                msg.Write(b.px);
                msg.Write(b.py);
                msg.Write((byte)b.direction);
                msg.Write(b.GetSpeed());
                msg.Write(b.IsJelly());
                msg.Write(b.IsTrigger());
            }
        }

        protected ServerPacket ReadServerPacket(NetIncomingMessage msg)
        {
            ServerPacket packet;
            packet.id = msg.ReadInt32();
            packet.lastAckId = msg.ReadInt32();
            packet.timeStamp = (float)msg.ReadTime(false);

            ReadServerPacket(msg, game.Field);

            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                ReadServerPacket(msg, players[i]);
            }

            return packet;
        }

        private void ReadServerPacket(NetIncomingMessage msg, Field field)
        {
            int bitsForPlayerIndex = NetUtility.BitsToHoldUInt((uint)(field.GetPlayers().GetCount() - 1));

            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCellSlot slot = slots[i];

                bool hasStaticCell = msg.ReadBoolean();
                if (hasStaticCell)
                {   
                    byte type = msg.ReadByte(BITS_FOR_STATIC_CELL);
                    switch (type)
                    {
                        case CELL_BRICK:
                        {
                            bool hasPowerup = msg.ReadBoolean();
                            int powerup = hasPowerup ? msg.ReadInt32(BITS_FOR_POWERUP) : Powerups.None;
                            break;
                        }

                        case CELL_POWERUP:
                        {
                            int powerup = msg.ReadInt32(BITS_FOR_POWERUP);
                            break;
                        }

                        case CELL_FLAME:
                        {
                            int playerIndex = msg.ReadInt32(bitsForPlayerIndex);
                            break;
                        }
                    }
                }
                else if (slot.staticCell != null && !slot.staticCell.IsSolid())
                {
                    slot.RemoveCell(slot.staticCell);
                }
            }
        }

        private void ReadServerPacket(NetIncomingMessage msg, Player p)
        {
            bool alive = msg.ReadBoolean();
            if (alive)
            {
                float px = msg.ReadFloat();
                float py = msg.ReadFloat();
                Direction direction = (Direction)msg.ReadByte();
                float speed = msg.ReadFloat();

                p.SetPos(px, py);
                p.SetSpeed(speed);
                p.SetDirection(direction);

                // powerups
                int powerupsCount = (int)Powerups.Count;
                for (int i = 0; i < powerupsCount; ++i)
                {
                    bool hasPower = msg.ReadBoolean();
                    if (hasPower)
                    {
                        int count = msg.ReadByte();
                        p.powerups.SetCount(i, count);
                    }
                    else
                    {
                        p.powerups.SetCount(i, 0);
                    }
                }

                // diseases
                int diseasesCount = (int)Diseases.Count;
                for (int i = 0; i < diseasesCount; ++i)
                {
                    bool infected = msg.ReadBoolean();
                    if (infected)
                    {
                        float remains = (float)(msg.ReadTime(false) - NetTime.Now);
                        p.diseases.TryInfect(i);
                        p.diseases.SetInfectedRemains(i, remains);
                    }
                    else
                    {
                        p.diseases.TryCure(i);
                    }
                }
            }
            else if (p.IsAlive())
            {
                p.Kill();
            }

            Bomb[] bombs = p.bombs.array;
            for (int i = 0; i < bombs.Length; ++i)
            {
                ReadServerPacket(msg, p, bombs[i]);
            }
        }

        private void ReadServerPacket(NetIncomingMessage msg, Player p, Bomb b)
        {
            bool active = msg.ReadBoolean();
            if (active)
            {
                float remains = (float)(msg.ReadTime(false) - NetTime.Now);
                float px = msg.ReadFloat();
                float py = msg.ReadFloat();
                Direction dir = (Direction)msg.ReadByte();
                float speed = msg.ReadFloat();
                bool jelly = msg.ReadBoolean();
                bool trigger = msg.ReadBoolean();

                if (!b.isActive)
                {
                    b.player = p;
                    b.Activate();
                    game.Field.SetBomb(b);
                }

                b.timeRemains = remains;
                b.SetPos(px, py);
                b.SetSpeed(speed);
                b.SetJelly(jelly);
                b.SetTrigger(trigger);
                // TODO: jelly & trigger
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Network helpers

        protected NetOutgoingMessage CreateMessage()
        {
            return GetMultiplayerManager().CreateMessage();
        }

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

        protected void RecycleMessage(NetOutgoingMessage msg)
        {
            GetMultiplayerManager().RecycleMessage(msg);
        }

        protected void RecycleMessage(NetIncomingMessage msg)
        {
            GetMultiplayerManager().RecycleMessage(msg);
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
