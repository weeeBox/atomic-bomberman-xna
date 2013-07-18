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
using Bomberman.Game.Multiplayer;

namespace Bomberman.Game
{
    public class GameSettings
    {
        public struct InputEntry
        {
            public int playerIndex;
            public PlayerInput input;

            public InputEntry(int playerIndex, PlayerInput input)
            {
                this.playerIndex = playerIndex;
                this.input = input;
            }
        }

        public Scheme scheme;
        public ServerInfo serverInfo;

        public InputEntry[] inputEntries;

        public GameSettings(Scheme scheme)
        {
            Debug.CheckArgumentNotNull("scheme", scheme);
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
            return new GameControllerClient(settings);
        }

        public static GameController Server(GameSettings settings)
        {
            return new GameControllerServer(settings);
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
            Debug.CheckArgumentNotNull(settings.inputEntries);
            Debug.CheckArgument(settings.inputEntries.Length > 0);
        }

        protected override void OnStart()
        {
            base.OnStart();

            game = new Game();
            GameSettings.InputEntry[] entries = settings.inputEntries;
            for (int i = 0; i < entries.Length; ++i)
            {
                Player player = new Player(entries[i].playerIndex);
                player.SetPlayerInput(entries[i].input);
                game.AddPlayer(player);
            }

            LoadField(settings.scheme);

            gameScreen = new GameScreen();

            StartScreen(gameScreen);

            GetConsole().TryExecuteCommand("exec game.cfg");
        }
    }

    #endregion
}
