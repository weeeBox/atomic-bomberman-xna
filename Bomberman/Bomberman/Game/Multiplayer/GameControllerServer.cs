﻿using System;
using System.Collections.Generic;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;
using BomberEngine.Core;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using Bomberman.Game.Elements.Fields;
using Bomberman.Multiplayer;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerServer : GameControllerNetwork
    {
        private IDictionary<NetConnection, Player> networkPlayersLookup;
        private List<Player> networkPlayers;

        private int nextPacketId;

        public GameControllerServer(GameSettings settings) :
            base(settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Server");

            game = new Game(MultiplayerMode.Server);

            int playerIndex = 0;
            GameSettings.InputEntry[] entries = settings.inputEntries;
            for (; playerIndex < entries.Length; ++playerIndex)
            {
                Player player = new Player(entries[playerIndex].playerIndex);
                player.SetPlayerInput(entries[playerIndex].input);
                game.AddPlayer(player);
            }

            networkPlayersLookup = new Dictionary<NetConnection, Player>();
            networkPlayers = new List<Player>();

            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(playerIndex + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                game.AddPlayer(player);

                AddPlayerConnection(connections[i], player);
            }

            LoadField(settings.scheme);

            gameScreen = new GameScreen();
            StartScreen(gameScreen);

            GetConsole().TryExecuteCommand("exec game.cfg");

            SetPacketRate(CVars.sv_packetRate.intValue);

            RegisterNotification(Notifications.ConsoleVariableChanged, ServerRateVarChangedCallback);
            RegisterNotification(NetworkNotifications.ClientConnected, ClientConnectedNotification);
            RegisterNotification(NetworkNotifications.ClientDisconnected, ClientDisconnectedNotification);

            GetNetwork().StartListeningMessages(NetworkMessageId.Request, OnClientRequest);
        }

        protected override void OnStop()
        {
            networkPlayersLookup = null;
            networkPlayers = null;
            game.Field.CancelAllTimers(this);

            base.OnStop();
        }

        private void SetPacketRate(int packetRate)
        {
            float packetDelay = 1.0f / packetRate;
            game.Field.CancelTimer(SendServerPacketCallback);
            game.Field.ScheduleTimer(SendServerPacketCallback, packetDelay, true);
        }

        private void ServerRateVarChangedCallback(Notification notification)
        {
            CVar var = notification.GetNotNullData<CVar>();
            SetPacketRate(var.intValue);
        }

        private void SendServerPacketCallback(Timer timer)
        {
            NetOutgoingMessage payloadMessage = CreateMessage();
            WriteServerPacket(payloadMessage);

            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                Player player = networkPlayers[i];

                NetOutgoingMessage message = CreateMessage(NetworkMessageId.ServerPacket);
                message.Write(nextPacketId);
                message.Write(player.lastAckPacketId);
                message.Write(payloadMessage);

                NetConnection connection = player.connection;
                Debug.Assert(connection != null);

                SendMessage(message, connection);
            }

            RecycleMessage(payloadMessage);

            ++nextPacketId;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Server listener

        private void OnClientRequest(Peer server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            NetworkRequestId requestId = (NetworkRequestId) message.ReadByte();
            switch (requestId)
            {
                case NetworkRequestId.RoundStart:
                {
                    Player player = FindPlayer(message.SenderConnection);
                    Debug.Assert(player != null);

                    NetOutgoingMessage response = CreateMessage(NetworkMessageId.Response);
                    response.Write((byte)requestId);

                    WriteFieldState(response, player);
                    SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableSequenced);

                    GetNetwork().StartListeningMessages(NetworkMessageId.ClientPacket, OnClientPacketReceived);
                    break;
                }

                case NetworkRequestId.RoundEnd:
                {
                    NetOutgoingMessage response = CreateMessage(NetworkMessageId.Response);
                    response.Write((byte)requestId);

                    SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableSequenced);
                    break;
                }

                default:
                    Debug.Fail("Unexpected request id: " + requestId);
                    break;
            }
        }

        private void OnClientPacketReceived(Peer server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            Player player = FindPlayer(message.SenderConnection);
            Debug.Assert(player != null);

            ClientPacket packet = ReadClientPacket(message);
            player.lastAckPacketId = packet.id;

            PlayerNetworkInput input = player.input as PlayerNetworkInput;
            Debug.Assert(input != null);

            input.SetNetActionBits(packet.actions);
        }

        private void ClientConnectedNotification(Notification notification)
        {
            NetConnection connection = notification.GetData2<NetConnection>();
            String name = notification.GetData3<String>();

            throw new NotImplementedException(); // clients can't join in progress game yet
        }

        private void ClientDisconnectedNotification(Notification notification)
        {
            NetConnection connection = notification.GetData2<NetConnection>();

            Player player = FindPlayer(connection);
            Debug.Assert(player != null);

            RemovePlayerConnection(connection);
            // TODO: notify gameplay

            if (CVars.g_startupMultiplayerMode.value != "server")
            {
                Application.sharedApplication.Stop();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player connection

        private void AddPlayerConnection(NetConnection connection, Player player)
        {
            Debug.Assert(!networkPlayersLookup.ContainsKey(connection));
            networkPlayersLookup.Add(connection, player);

            Debug.Assert(player.connection == null);
            player.connection = connection;

            Debug.Assert(!networkPlayers.Contains(player));
            networkPlayers.Add(player);
        }

        private void RemovePlayerConnection(NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Debug.Assert(player != null && player.connection == connection);
            player.connection = null;

            networkPlayersLookup.Remove(connection);

            Debug.Assert(networkPlayers.Contains(player));
            networkPlayers.Remove(player);
        }

        private Player FindPlayer(NetConnection connection)
        {
            Player player;
            if (networkPlayersLookup.TryGetValue(connection, out player))
            {
                return player;
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnRoundEnded()
        {
            base.OnRoundEnded();
            SendRequest(NetworkRequestId.RoundEnd, null, "Waiting for clients");
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Game state

        private void FillGameState(GameStateSnapshot state)
        {   
            state.SetFrom(game.GetPlayers().list);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private Server GetServer()
        {
            return GetNetwork().GetServer();
        }

        #endregion
    }
}
