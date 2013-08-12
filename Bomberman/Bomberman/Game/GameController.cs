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

    public class GameController : BmController
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

            RegisterNotification(GameNotifications.RoundEnded, RoundEndedNotification);
            RegisterNotification(GameNotifications.GameEnded, GameEndedNotification);
        }

        protected override void OnStop()
        {
            GetConsole().UnregisterCommands(gameCommands);
            base.OnStop();
        }

        protected void Stop(ExitCode exitCode, Object data = null)
        {
            Stop((int)exitCode, data);
        }

        public void Restart()
        {
            game.Restart();
            GetConsole().TryExecuteCommand("exec game.cfg");
        }

        protected void StartNextRound()
        {   
            game.StartNextRound();
            GetConsole().TryExecuteCommand("exec game.cfg");
        }

        protected void LoadField(Scheme scheme)
        {   
            game.LoadField(scheme); 
        }

        protected void SetupField(Scheme scheme)
        {
            game.SetupField(scheme);
        }

        public void ShowPauseScreen()
        {
            Debug.Assert(CurrentScreen() is GameScreen);
            StartNextScreen(new PauseScreen(OnPauseScreenButtonPress));
        }

        public void HidePauseScreen()
        {
            if (CurrentScreen() is PauseScreen)
            {
                CurrentScreen().Finish();
            }
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

        //////////////////////////////////////////////////////////////////////////////

        #region Game notifications

        private void RoundEndedNotification(Notification notification)
        {
            Log.d("Round ended");
            OnRoundEnded();
        }

        private void GameEndedNotification(Notification notification)
        {
            Log.d("Game ended");
            OnGameEnded();
        }

        private void RoundResultScreenButtonDelegate(Button button)
        {
            RoundResultScreen.ButtonId buttonId = (RoundResultScreen.ButtonId)button.id;

            RoundResultScreen resultScreen = CurrentScreen() as RoundResultScreen;
            Debug.Assert(resultScreen != null);

            switch (buttonId)
            {
                case RoundResultScreen.ButtonId.Continue:
                    RoundResultScreenAccepted(resultScreen);
                    break;
                case RoundResultScreen.ButtonId.Exit:
                    RoundResultScreenDismissed(resultScreen);
                    break;
            }
        }

        private void GameResultScreenButtonDelegate(Button button)
        {
            GameResultScreen.ButtonId buttonId = (GameResultScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case GameResultScreen.ButtonId.Exit:
                    Stop(ExitCode.Exit);
                    break;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected virtual void OnRoundEnded()
        {
            StartNextScreen(new RoundResultScreen(RoundResultScreenButtonDelegate));
        }

        protected virtual void OnGameEnded()
        {
            StartNextScreen(new GameResultScreen(GameResultScreenButtonDelegate));
        }

        protected virtual void RoundResultScreenAccepted(RoundResultScreen screen)
        {
            screen.Finish();
            StartNextRound();
        }

        protected virtual void RoundResultScreenDismissed(RoundResultScreen screen)
        {
            Stop(ExitCode.Exit);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        protected Field Field
        {
            get { return Game.Current.Field; }
        }

        #endregion
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

            game = new Game(MultiplayerMode.None);

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
