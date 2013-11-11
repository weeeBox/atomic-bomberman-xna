using System.Collections.Generic;
using System.Text;
using BomberEngine;
using Bomberman.Gameplay.Elements;
using Bomberman.Gameplay.Elements.Cells;
using Bomberman.Gameplay.Elements.Fields;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Networking;
using Lidgren.Network;
using System;

namespace Bomberman.Gameplay.Multiplayer
{
    public struct ClientPacket
    {
        public int id;
        public int actions;
        public bool replayed;
    }

    public struct ServerPacket
    {
        public int id;
    }

    public abstract class GameControllerNetwork : GameController, IPeerListener
    {
        protected enum State
        {
            Undefined,
            RoundStart,
            Playing,
            RoundEnd,
            GameEnd,
        }

        public enum PeerMessageId
        {
            RoundStart,
            Playing,
            RoundEnd,
        }

        private State m_state;

        public GameControllerNetwork(Game game, GameSettings settings)
            : base(game, settings)
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

        #region "RoundStart" message

        internal void WriteFieldState(NetBuffer buffer, NetChannel channel)
        {
            // field
            WriteFieldState(buffer, game.Field);

            // players
            List<Player> players = game.GetPlayers().list;
            buffer.Write((byte)players.Count);
            for (int i = 0; i < players.Count; ++i)
            {
                Player p = players[i];
                buffer.WriteCellCords(p);
                buffer.Write(p.NetChannel == channel);
            }
        }

        internal void ReadFieldState(NetBuffer buffer)
        {
            // field
            ReadFieldState(buffer, game.Field);

            // sync players list with server
            List<Player> localPlayers = new List<Player>(game.GetPlayersList());
            game.GetPlayers().Clear();

            // players
            int playersCount = buffer.ReadByte();
            for (int playerIndex = 0, localIndex = 0; playerIndex < playersCount; ++playerIndex)
            {
                int cx = buffer.ReadCellCord();
                int cy = buffer.ReadCellCord();
                bool isLocal = buffer.ReadBoolean();

                Player player;

                // input
                if (isLocal)
                {
                    Assert.IsRange(localIndex, 0, localPlayers.Count - 1);
                    player = localPlayers[localIndex++];

                    Assert.IsNotNull(player.input);         // player should have an input
                    Assert.IsTrue(player.input.IsLocal);    // and it should be local
                    Assert.AreEqual(-1, player.index);      // no index should be assigned yet

                    player.index = playerIndex;
                }
                else
                {
                    player = new Player(playerIndex);
                    player.SetPlayerInput(new PlayerNetworkInput());
                }

                // position
                player.SetCell(cx, cy);
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
            Assert.IsTrue(players.Count == playersCount);

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
            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCell staticCell = slots[i].staticCell;

                bool shouldWrite = staticCell != null && !staticCell.IsSolid();
                buffer.Write(shouldWrite);

                if (shouldWrite)
                {
                    buffer.WriteStaticCellType(staticCell.type);
                    switch (staticCell.type)
                    {
                        case FieldCellType.Brick:
                        {
                            BrickCell brick = staticCell.AsBrick();
                            buffer.WritePowerup(brick.powerup, true);
                            break;
                        }

                        case FieldCellType.Powerup:
                        {
                            PowerupCell powerup = staticCell.AsPowerup();
                            buffer.WritePowerup(powerup.powerup);
                            break;
                        }

                        case FieldCellType.Flame:
                        {
                            FlameCell flame = staticCell.AsFlame();
                            buffer.WritePlayerIndex(flame.player);
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
                buffer.Write(p.direction);
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
                msg.Write(b.direction);
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

        private void ReadFieldState(NetBuffer buffer, Field field)
        {
            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                FieldCellSlot slot = slots[i];

                bool hasStaticCell = buffer.ReadBoolean();
                if (hasStaticCell)
                {
                    FieldCellType type = buffer.ReadStaticCellType();
                    switch (type)
                    {
                        case FieldCellType.Brick:
                        {
                            BrickCell brick = slot.GetBrick();
                            Assert.IsNotNull(brick);

                            brick.powerup = buffer.ReadPowerup(true);
                            break;
                        }

                        case FieldCellType.Powerup:
                        {
                            int powerup = buffer.ReadPowerup();
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

                        case FieldCellType.Flame:
                        {
                            int playerIndex = buffer.ReadPlayerIndex();
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
                Direction direction = msg.ReadDirection();
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
                Direction dir = msg.ReadDirection();
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
            Assert.IsTrue(game != null);

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
            Assert.IsTrue(game != null);

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

            #if DEBUG
            if (CVars.d_closeInnactive.floatValue > 0)
            {
                Application.CancelTimer(CloseInnactive);
                Application.ScheduleTimer(CloseInnactive, CVars.d_closeInnactive.floatValue);
            }
            #endif
        }

        protected virtual void ReadPeerMessage(Peer peer, PeerMessageId id, NetIncomingMessage msg)
        {   
        }

        private void CloseInnactive()
        {
            Application.sharedApplication.Stop();
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

        protected bool AllPlayersAreReady()
        {
            return game.GetPlayers().AllPlayersAreReady();
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
                //StringBuilder buf = new StringBuilder();
                //buf.Append("State: " + m_controller.GetState());

                //if (m_controller.game != null)
                //{
                //    List<Player> players = m_controller.game.GetPlayersList();
                //    for (int i = 0; i < players.Count; ++i)
                //    {
                //        Player p = players[i];
                //        buf.Append("\n" + i + ": isReady=" + p.IsReady +
                //            " net=" + p.IsNetworkPlayer +
                //            " needsFieldState=" + p.needsFieldState + 
                //            " needsRoundResults=" + p.needsRoundResults);
                //    }
                //}

                //m_textView.SetText(buf.ToString());
            }
        }
    }

    public static class CellNetBuffer
    {
        private static readonly uint MaxStaticCellType  = (uint)FieldCellType.Flame;
        private static readonly uint MaxCellCord        = 15;
        private static readonly uint MaxPlayerIndex     = 15;
        private static readonly uint MaxPowerupValue    = Powerups.Count - 1;
        private static readonly uint MaxDirectionValue  = (uint)Direction.Count - 1;

        private static readonly int CellStaticTypeBitsCount = NetUtility.BitsToHoldUInt(MaxStaticCellType);
        private static readonly int CellCordBitsCount       = NetUtility.BitsToHoldUInt(MaxCellCord);

        private static readonly int PlayerIndexBitsCount    = NetUtility.BitsToHoldUInt(MaxPlayerIndex);
        private static readonly int PowerupValueBitsCount   = NetUtility.BitsToHoldUInt(MaxPowerupValue);

        private static readonly int DirectionBitsCount      = NetUtility.BitsToHoldUInt(MaxDirectionValue);

        // TODO: use bit packing

        public static void WriteCellCords(this NetBuffer buffer, FieldCell cell)
        {
            buffer.WriteCellCord(cell.cx); 
            buffer.WriteCellCord(cell.cy);
        }

        public static void WriteCellCord(this NetBuffer buffer, int value)
        {
            Assert.IsRange(value, 0, MaxCellCord);
            buffer.Write((byte)value, CellCordBitsCount);
        }

        public static int ReadCellCord(this NetBuffer buffer)
        {
            return buffer.ReadByte(CellCordBitsCount);
        }

        public static void WriteStaticCellType(this NetBuffer buffer, FieldCellType type)
        {
            Assert.IsRange((int)type, 0, MaxStaticCellType);
            buffer.Write((byte)type, CellStaticTypeBitsCount);
        }

        public static FieldCellType ReadStaticCellType(this NetBuffer buffer)
        {
            return (FieldCellType)buffer.ReadByte(CellStaticTypeBitsCount);
        }

        public static void WritePowerup(this NetBuffer buffer, int value, bool writeFlag = false)
        {
            if (writeFlag)
            {
                bool valid = value != Powerups.None;
                buffer.Write(valid);

                if (valid)
                {
                    Assert.IsRange(value, 0, MaxPowerupValue);
                    buffer.Write((byte)value, PowerupValueBitsCount);
                }
            }
            else
            {
                Assert.IsRange(value, 0, MaxPowerupValue);
                buffer.Write((byte)value, PowerupValueBitsCount);
            }
        }

        public static int ReadPowerup(this NetBuffer buffer, bool readFlag = false)
        {
            if (readFlag)
            {
                bool valid = buffer.ReadBoolean();
                if (!valid)
                {
                    return Powerups.None;
                }
            }
            return buffer.ReadByte(PowerupValueBitsCount);
        }

        public static void WritePlayerIndex(this NetBuffer buffer, Player player)
        {   
            buffer.WritePlayerIndex(player.GetIndex());
        }

        public static void WritePlayerIndex(this NetBuffer buffer, int value)
        {
            Assert.IsRange(value, 0, MaxPlayerIndex);
            buffer.Write((byte)value, PlayerIndexBitsCount);
        }

        public static int ReadPlayerIndex(this NetBuffer buffer)
        {
            return buffer.ReadByte(PlayerIndexBitsCount);
        }

        public static void Write(this NetBuffer buffer, Direction direction)
        {
            Assert.IsRange((int)direction, 0, MaxDirectionValue);
            buffer.Write((byte)direction, DirectionBitsCount);
        }

        public static Direction ReadDirection(this NetBuffer buffer)
        {
            return (Direction)buffer.ReadByte(DirectionBitsCount);
        }
    }
}
