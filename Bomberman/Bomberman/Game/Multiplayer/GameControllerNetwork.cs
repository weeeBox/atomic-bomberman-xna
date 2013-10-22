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
using Bomberman.Game.Screens;
using BomberEngine.Core.Visual;
using System.Text;

namespace Bomberman.Game.Multiplayer
{
    public struct ClientPacket
    {
        public int id;
        public int lastAckServerPacketId;
        public int actions;
    }

    public struct ServerPacket
    {
        public int id;
        public int lastAckClientPacketId;
    }

    public abstract class GameControllerNetwork : GameController, IPeerListener
    {
        protected enum State
        {
            Undefined,
            RoundStart,
            Playing,
            RoundEnd,
        }

        public enum PeerMessageId
        {
            RoundStart,
            Playing,
            RoundEnd,
        }

        private State m_state;

        public GameControllerNetwork(GameSettings settings)
            : base(settings)
        {
            m_state = State.Undefined;
        }

        protected override void OnStart()
        {
            base.OnStart();

            GetPeer().SetPeerListener(this);
            SetState(State.RoundStart);

            AddDebugView(new NetworkControllerDebugView(this));
        }

        protected override void OnStop()
        {
            GetPeer().SetPeerListener(null);
            StopNetworkPeer();

            base.OnStop();
        }

        //////////////////////////////////////////////////////////////////////////////

        private const byte CELL_FLAME   = 0;
        private const byte CELL_BRICK   = 1;
        private const byte CELL_POWERUP = 2;

        private static readonly int BITS_FOR_STATIC_CELL = NetUtility.BitsToHoldUInt(2);
        private static readonly int BITS_FOR_POWERUP = NetUtility.BitsToHoldUInt(Powerups.Count - 1);

        /* a client sends data to the server */
        protected void WriteClientPacket(NetBuffer buffer, ref ClientPacket packet)
        {
            buffer.Write(packet.id);
            buffer.Write(packet.lastAckServerPacketId);
            buffer.Write(packet.actions, (int)PlayerAction.Count);
        }

        /* the server reads data from a client */
        protected ClientPacket ReadClientPacket(NetBuffer msg)
        {
            ClientPacket packet;
            packet.id = msg.ReadInt32();
            packet.lastAckServerPacketId = msg.ReadInt32();
            packet.actions = msg.ReadInt32((int)PlayerAction.Count);

            return packet;
        }

        #region "RoundStart" message

        protected void WriteFieldState(NetBuffer buffer, Player player)
        {   
            // bricks & powerups
            FieldCellSlot[] slots = game.Field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    bool hasPowerup = brick.HasPowerup();
                    buffer.Write(hasPowerup);
                    if (hasPowerup)
                    {
                        buffer.Write((byte)brick.powerup);
                    }
                }
            }

            // players
            List<Player> players = game.GetPlayers().list;

            int senderIndex = players.IndexOf(player);
            Debug.Assert(senderIndex != -1);

            buffer.Write((byte)senderIndex);
            buffer.Write((byte)players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                Player p = players[i];
                buffer.Write((byte)p.cx);
                buffer.Write((byte)p.cy);
            }
        }

        protected void ReadFieldState(NetBuffer buffer)
        {
            // bricks & powerups
            FieldCellSlot[] slots = game.Field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    bool hasPowerup = buffer.ReadBoolean();
                    if (hasPowerup)
                    {
                        brick.powerup = buffer.ReadByte();
                    }
                }
            }

            // players
            int senderIndex = buffer.ReadByte();
            int playersCount = buffer.ReadByte();
            for (int i = 0; i < playersCount; ++i)
            {
                Player player = new Player(i);
                int cx = buffer.ReadByte();
                int cy = buffer.ReadByte();
                player.SetCell(cx, cy);
                if (i != senderIndex)
                {
                    player.SetPlayerInput(new PlayerNetworkInput());
                }

                game.AddPlayer(player);
            }
        }

        #endregion

        #region "Ready"

        protected void WriteReadyFlags(NetBuffer buffer)
        {
            List<Player> players = game.GetPlayers().list;
            buffer.Write((byte)players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                buffer.Write(players[i].IsReady);
            }
        }

        protected void ReadReadyFlags(NetBuffer buffer)
        {
            List<Player> players = game.GetPlayers().list;
            int playersCount = buffer.ReadByte();
            Debug.Assert(players.Count == playersCount);

            for (int i = 0; i < playersCount; ++i)
            {   
                Player player = players[i];

                bool isReady = buffer.ReadBoolean();
                if (player.IsNetworkPlayer)
                {
                    player.IsReady = isReady;
                }
            }
        }

        protected void SkipReadyFlags(NetBuffer buffer)
        {
            int playersCount = buffer.ReadByte();
            buffer.ReadInt64(playersCount);
        }

        #endregion

        #region "Playing" message

        /* the server sends data to a client */
        protected void WritePlayingMessage(NetBuffer buffer)
        {
            WriteFieldState(buffer, game.Field);
             
            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                WritePlayerState(buffer, players[i]);
            }
        }

        private void WriteFieldState(NetBuffer buffer, Field field)
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
                            buffer.Write(CELL_BRICK, BITS_FOR_STATIC_CELL);
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

        private void WritePlayerState(NetBuffer buffer, Player p)
        {
            bool alive = p.IsAlive;
            buffer.Write(alive);
            if (alive)
            {
                buffer.Write(p.px);
                buffer.Write(p.py);
                buffer.Write(p.IsMoving());
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
                int diseasesCount = DiseaseList.diseaseArray.Length;
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
                WriteBombState(buffer, bombs[i]);
            }
        }

        private void WriteBombState(NetBuffer msg, Bomb b)
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

        /* a client reads data from the server */
        protected void ReadPlayingMessage(NetIncomingMessage msg)
        {
            ReadFieldState(msg, game.Field);

            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                ReadPlayerState(msg, players[i]);
            }
        }

        private void ReadFieldState(NetBuffer msg, Field field)
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
                            Debug.Assert(slot.ContainsBrick());
                            break;
                        }

                        case CELL_POWERUP:
                        {
                            int powerup = msg.ReadInt32(BITS_FOR_POWERUP);
                            if (slot.staticCell == null)
                            {
                                field.AddCell(new PowerupCell(powerup, slot.cx, slot.cy));
                            }
                            else if (!slot.staticCell.IsPowerup())
                            {
                                field.RemoveCell(slot.staticCell);
                                field.AddCell(new PowerupCell(powerup, slot.cx, slot.cy));
                            }
                            break;
                        }

                        case CELL_FLAME:
                        {
                            int playerIndex = msg.ReadInt32(bitsForPlayerIndex);
                            if (slot.staticCell == null)
                            {
                                Player player = field.GetPlayers().Get(playerIndex);
                                field.AddCell(new FlameCell(player, slot.cx, slot.cy));
                            }
                            else if (!slot.staticCell.IsFlame())
                            {
                                Player player = field.GetPlayers().Get(playerIndex);
                                field.RemoveCell(slot.staticCell);
                                field.AddCell(new FlameCell(player, slot.cx, slot.cy));
                            }
                            break;
                        }
                    }
                }
                else if (slot.staticCell != null && !slot.staticCell.IsSolid())
                {
                    field.RemoveCell(slot.staticCell);
                }
            }
        }

        private void ReadPlayerState(NetIncomingMessage msg, Player p)
        {
            bool alive = msg.ReadBoolean();
            if (alive)
            {
                float px = msg.ReadFloat();
                float py = msg.ReadFloat();
                bool moving = msg.ReadBoolean();
                Direction direction = (Direction)msg.ReadByte();
                float speed = msg.ReadFloat();

                // player state
                p.UpdateFromNetwork(px, py, moving, direction, speed);

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
                int diseasesCount = DiseaseList.diseaseArray.Length;
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
            else if (p.IsAlive)
            {
                Field.KillPlayer(p, p); // TODO: report killer
            }

            Bomb[] bombs = p.bombs.array;
            for (int i = 0; i < bombs.Length; ++i)
            {
                ReadBombState(msg, p, bombs[i]);
            }
        }

        private void ReadBombState(NetIncomingMessage msg, Player p, Bomb b)
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
            else if (b.isActive)
            {
                b.Deactivate();
                b.RemoveFromField();
            }
        }

        #endregion

        #region "RoundEnd" message

        protected void WriteRoundResults(NetBuffer buffer)
        {
            Debug.Assert(game != null);

            buffer.Write((byte)game.roundIndex);
            buffer.Write((byte)game.roundsCount);

            List<Player> players = game.GetPlayersList();

            for (int i = 0; i < players.Count; ++i)
            {
                Player p = players[i];
                buffer.Write((byte)p.statistics.winsCount);
                buffer.Write((byte)p.statistics.suicidesCount);
            }
        }

        protected void ReadRoundResults(NetBuffer buffer)
        {
            Debug.Assert(game != null);

            game.roundIndex = buffer.ReadByte();
            game.roundsCount = buffer.ReadByte();

            List<Player> players = game.GetPlayersList();

            for (int i = 0; i < players.Count; ++i)
            {
                Player p = players[i];
                p.statistics.winsCount = buffer.ReadByte();
                p.statistics.suicidesCount = buffer.ReadByte();
            }
        }

        #endregion
        
        //////////////////////////////////////////////////////////////////////////////

        #region State management

        protected void SetState(State state)
        {
            if (state != m_state)
            {
                State oldState = m_state;
                m_state = state;
                Log.d("Set state: " + state + " was " + oldState);
                OnStateChanged(oldState, state);
            }
        }

        protected State GetState()
        {
            return m_state;
        }

        protected virtual void OnStateChanged(State oldState, State newState)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Network helpers

        protected NetOutgoingMessage CreateMessage()
        {
            return GetNetwork().CreateMessage();
        }

        protected NetOutgoingMessage CreateMessage(PeerMessageId messageId)
        {
            NetOutgoingMessage msg = CreateMessage();
            msg.Write((byte)messageId);
            return msg;
        }

        protected void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            GetNetwork().SendMessage(message, recipient);
        }

        protected void SendMessage(NetOutgoingMessage message)
        {
            GetNetwork().SendMessage(message);
        }

        protected void RecycleMessage(NetOutgoingMessage message)
        {
            GetNetwork().RecycleMessage(message);
        }

        protected void StopNetworkPeer()
        {
            GetNetwork().Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IPeerListener

        public virtual void OnPeerMessageReceived(Peer peer, NetIncomingMessage msg)
        {
            PeerMessageId id = (PeerMessageId) msg.ReadByte();
            ReadPeerMessage(peer, id, msg);
        }

        protected virtual void ReadPeerMessage(Peer peer, PeerMessageId id, NetIncomingMessage msg)
        {   
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        protected Server GetServer()
        {
            return GetNetwork().GetServer();
        }

        protected Client GetClient()
        {
            return GetNetwork().GetClient();
        }

        protected Peer GetPeer()
        {
            return GetNetwork().GetPeer();
        }

        protected void SetPlayersReady(bool ready)
        {
            game.GetPlayers().SetPlayersReady(ready);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private class NetworkControllerDebugView : View
        {
            private GameControllerNetwork m_controller;
            private TextView m_textView;

            public NetworkControllerDebugView(GameControllerNetwork controller)
            {
                m_controller = controller;

                m_textView = new TextView(Helper.fontSystem, null);
                UpdateState();

                AddView(m_textView);
                ResizeToFitViews();

                height = 100;
            }

            public override void Update(float delta)
            {
                UpdateState();
            }

            private void UpdateState()
            {
                StringBuilder buf = new StringBuilder();
                buf.Append("State: " + m_controller.GetState());

                if (m_controller.game != null)
                {
                    List<Player> players = m_controller.game.GetPlayersList();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        Player p = players[i];
                        buf.Append("\n" + i + ": isReady=" + p.IsReady +
                            " net=" + p.IsNetworkPlayer +
                            " needsFieldState=" + p.needsFieldState + 
                            " needsRoundResults=" + p.needsRoundResults);
                    }
                }

                m_textView.SetText(buf.ToString());
            }
        }
    }
}
