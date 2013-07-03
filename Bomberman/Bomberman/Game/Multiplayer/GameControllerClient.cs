using System;
using System.Collections.Generic;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;
using BomberEngine.Debugging;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerClient : GameControllerNetwork, IClientListener
    {
        private const int SENT_HISTORY_SIZE = 32;
        private const int SENT_HISTORY_MASK = SENT_HISTORY_SIZE - 1;

        private Player localPlayer;
        private ClientPacket[] sentPackets;
        private int nextPacketId;

        public GameControllerClient(GameSettings settings)
            : base(settings)
        {
            sentPackets = new ClientPacket[SENT_HISTORY_SIZE];
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Client");

            GetMultiplayerManager().SetClientListener(this);
            RequestFieldState();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (game != null)
            {
                SendClientPacket(localPlayer);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                {
                    game = new Game();

                    SetupField(settings.scheme);

                    ReadFieldState(message);

                    gameScreen = new GameScreen();

                    localPlayer = null;
                    List<Player> players = game.GetPlayers().list;
                    for (int i = 0; i < players.Count; ++i)
                    {
                        if (players[i].input == null)
                        {
                            localPlayer = players[i];
                            break;
                        }
                    }

                    // input
                    PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
                    keyboardInput1.Map(KeyCode.W, PlayerAction.Up);
                    keyboardInput1.Map(KeyCode.A, PlayerAction.Left);
                    keyboardInput1.Map(KeyCode.S, PlayerAction.Down);
                    keyboardInput1.Map(KeyCode.D, PlayerAction.Right);
                    keyboardInput1.Map(KeyCode.OemCloseBrackets, PlayerAction.Bomb);
                    keyboardInput1.Map(KeyCode.OemOpenBrackets, PlayerAction.Special);
                    gameScreen.AddKeyListener(keyboardInput1);

                    localPlayer.SetPlayerInput(keyboardInput1);
                    localPlayer.connection = client.GetServerConnection();

                    StartScreen(gameScreen);
                    break;
                }

                case NetworkMessageId.ServerPacket:
                {
                    if (game != null)
                    {
                        ReadServerPacket(message);
                    }
                    break;
                }
            }
        }

        public void OnConnectedToServer(Client client, NetConnection serverConnection)
        {
        }

        public void OnDisconnectedFromServer(Client client)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void SendClientPacket(Player player)
        {
            int actions = 0;
            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                if (player.input.IsActionPressed(i))
                {
                    actions |= 1 << i;
                }
            }

            ClientPacket packet;
            packet.id = nextPacketId;
            packet.timeStamp = (float)NetTime.Now;
            packet.actions = actions;

            ++nextPacketId;

            NetOutgoingMessage msg = CreateMessage(NetworkMessageId.ClientPacket);
            WriteClientPacket(msg, ref packet);
            SendMessage(msg);

            PushPacket(ref packet);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void RequestFieldState()
        {
            SendMessage(NetworkMessageId.FieldState, NetDeliveryMethod.ReliableOrdered);
            StartConnectionScreen(OnRequestFieldStateCancelled, "Waiting for the server...");
        }

        private void OnRequestFieldStateCancelled()
        {
            throw new NotImplementedException();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Packet history

        private void PushPacket(ref ClientPacket packet)
        {
            int index = packet.id & SENT_HISTORY_MASK;
            sentPackets[index] = packet;
        }

        private ClientPacket GetPacket(int id)
        {
            int index = id & SENT_HISTORY_MASK;
            Debug.Assert(sentPackets[index].id == id);
            return sentPackets[index];
        }

        #endregion
    }
}
