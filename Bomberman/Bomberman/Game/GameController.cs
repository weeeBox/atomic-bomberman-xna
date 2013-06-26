using System;
using System.Collections.Generic;
using BomberEngine.Game;
using Bomberman.Game.Screens;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Content;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;
using BomberEngine.Core;
using Bomberman.Multiplayer;
using Lidgren.Network;

namespace Bomberman.Game
{
    public class GameSettings
    {   
        public Scheme scheme;
        public ServerInfo serverInfo;

        public GameSettings(Scheme scheme)
        {
            this.scheme = scheme;
        }
    }

    public class GameController : BombermanController
    {
        public enum ExitCode
        {
            StopServer,
            StopClient,
            Exit
        }

        protected GameScreen gameScreen;
        protected PauseScreen pauseScreen;
        
        protected Game game;

        protected GameSettings settings;

        private CCommand[] gameCommands = 
        {
            new Cmd_infect(),
            new Cmd_add(),
            new Cmd_map(),
            new Cmd_map_restart(),
        };

        public static GameController Client(GameSettings settings)
        {
            return new ClientGameController(settings);
        }

        public static GameController Server(GameSettings settings)
        {
            return new ServerGameController(settings);
        }

        public static GameController Local(GameSettings settings)
        {
            return new LocalGameController(settings);
        }

        protected GameController(GameSettings settings)
        {
            this.settings = settings;
        }

        protected override void OnStart()
        {
            GetConsole().RegisterCommands(gameCommands);
        }

        protected override void OnStop()
        {   
            GetConsole().UnregisterCommands(gameCommands);
        }

        protected void Stop(ExitCode exitCode, Object data = null)
        {
            Stop((int)exitCode, data);
        }

        protected void InitPlayers()
        {
            List<Player> players = game.GetPlayers().list;

            PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
            keyboardInput1.Map(KeyCode.W, PlayerAction.Up);
            keyboardInput1.Map(KeyCode.A, PlayerAction.Left);
            keyboardInput1.Map(KeyCode.S, PlayerAction.Down);
            keyboardInput1.Map(KeyCode.D, PlayerAction.Right);
            keyboardInput1.Map(KeyCode.OemCloseBrackets, PlayerAction.Bomb);
            keyboardInput1.Map(KeyCode.OemOpenBrackets, PlayerAction.Special);
            gameScreen.AddKeyListener(keyboardInput1);

            players[0].SetPlayerInput(keyboardInput1);
        }

        protected void LoadField(Scheme scheme)
        {   
            game.LoadField(scheme); 
        }

        protected void SetupField(Scheme scheme)
        {
            game.SetupField(scheme);
        }

        public override bool HandleEvent(Event evt)
        {
            if (base.HandleEvent(evt))
            {
                return true;
            }

            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.state == KeyState.Pressed && OnKeyPressed(keyEvent.arg))
                {
                    return true;
                }
            }

            return false;
        }

        private bool OnKeyPressed(KeyEventArg e)
        {
            if (e.key == KeyCode.Escape)
            {
                Screen screen = CurrentScreen();
                if (screen is GameScreen)
                {
                    StartNextScreen(new PauseScreen(OnPauseScreenButtonPress));
                    return true;
                }

                if (screen is PauseScreen)
                {
                    screen.Finish();
                    return true;
                }
            }

            return false;
        }

        private void OnPauseScreenButtonPress(Button button)
        {
            PauseScreen.ButtonId buttonId = (PauseScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case PauseScreen.ButtonId.Exit:
                    Stop(ExitCode.Exit);
                    break;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    #region Local game

    internal class LocalGameController : GameController
    {
        public LocalGameController(GameSettings settings)
            : base(settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            game = new Game();
            game.AddPlayer(new Player(0));

            LoadField(settings.scheme);

            gameScreen = new GameScreen();

            InitPlayers();

            StartScreen(gameScreen);
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////

    #region Network game

    internal abstract class NetworkGameController : GameController
    {
        public NetworkGameController(GameSettings settings)
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

    #endregion

    //////////////////////////////////////////////////////////////////////////////

    #region Client side game

    internal class ClientGameController : NetworkGameController, ClientListener
    {
        private Player localPlayer;

        public ClientGameController(GameSettings settings)
            : base(settings)
        {
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
                SendActions(localPlayer);
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

        private void SendActions(Player player)
        {
            NetOutgoingMessage msg = CreateMessage(NetworkMessageId.PlayerActions);
            WritePlayerActions(msg, player.input);
            SendMessage(msg, player.connection);
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////

    #region Server side game

    internal class ServerGameController : NetworkGameController, ServerListener
    {
        private IDictionary<NetConnection, Player> playersLookup;

        public ServerGameController(GameSettings settings) :
            base(settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Server");

            GetMultiplayerManager().SetServerListener(this);
            
            game = new Game();
            game.AddPlayer(new Player(0));

            playersLookup = new Dictionary<NetConnection, Player>();

            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(i + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                game.AddPlayer(player);

                AddPlayerConnection(connections[i], player);
            }

            LoadField(settings.scheme);

            gameScreen = new GameScreen();

            InitPlayers();

            StartScreen(gameScreen);
        }

        protected override void OnStop()
        {
            playersLookup = null;
            base.OnStop();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                {
                    Player player = FindPlayer(message.SenderConnection);
                    Debug.Assert(player != null);

                    NetOutgoingMessage response = CreateMessage(NetworkMessageId.FieldState);
                    WriteFieldState(response, player);
                    SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                }

                case NetworkMessageId.PlayerActions:
                {
                    Player player = FindPlayer(message.SenderConnection);
                    Debug.Assert(player != null);

                    PlayerNetworkInput input = player.input as PlayerNetworkInput;
                    Debug.Assert(input != null);

                    ReadPlayerActions(message, input);
                    break;
                }
            }
        }

        public void OnClientConnected(Server server, string name, NetConnection connection)
        {
            throw new NotImplementedException(); // clients can't join in progress game yet
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Debug.Assert(player != null);

            RemovePlayerConnection(connection);
            // TODO: notify gameplay
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player connection

        private void AddPlayerConnection(NetConnection connection, Player player)
        {
            Debug.Assert(!playersLookup.ContainsKey(connection));
            playersLookup.Add(connection, player);

            Debug.Assert(player.connection == null);
            player.connection = connection;
        }

        private void RemovePlayerConnection(NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Debug.Assert(player != null && player.connection == connection);
            player.connection = null;

            playersLookup.Remove(connection);
        }

        private Player FindPlayer(NetConnection connection)
        {
            Player player;
            if (playersLookup.TryGetValue(connection, out player))
            {
                return player;
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private Server GetServer()
        {
            return GetMultiplayerManager().GetServer();
        }

        #endregion
    }

    #endregion
}
