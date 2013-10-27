using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Multiplayer;
using Bomberman.Game.Screens;
using Bomberman.Multiplayer;

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
        
        private Game m_game;

        protected GameSettings settings;

        private CCommand[] gameCommands = 
        {
            new Cmd_infect(),
            new Cmd_add(),
            new Cmd_map(),
            new Cmd_map_restart(),

            new Cmd_test("test1", RunTest1),
            new Cmd_test("test2", RunTest2),
            new Cmd_test("test3", RunTest3),
            new Cmd_test("test4", RunTest4),
            new Cmd_test("test5", RunTest5),
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

        public override void Destroy()
        {
            game = null;
            base.Destroy();
        }

        protected override void OnStart()
        {
            GetConsole().RegisterCommands(gameCommands);

            RegisterNotification(GameNotifications.RoundEnded, RoundEndedNotification);
            RegisterNotification(GameNotifications.GameEnded, GameEndedNotification);
            RegisterNotification(GameNotifications.RoundRestarted, RoundRestartedNotification);
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

        private void RoundRestartedNotification(Notification notification)
        {
            Log.d("Round restarted");
            OnRoundRestarted();
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
            StartRoundResultScreen();
        }

        protected virtual void OnRoundRestarted()
        {
        }

        protected virtual void OnGameEnded()
        {
            StartGameResultScreen();
        }

        protected void StartRoundResultScreen()
        {
            StartScreen(new RoundResultScreen(game, RoundResultScreenButtonDelegate));
        }

        protected void StartGameResultScreen()
        {
            StartScreen(new GameResultScreen(game, GameResultScreenButtonDelegate));
        }

        protected virtual void RoundResultScreenAccepted(RoundResultScreen screen)
        {
            StartNextRound();

            gameScreen = new GameScreen();
            StartScreen(gameScreen);
        }

        protected virtual void RoundResultScreenDismissed(RoundResultScreen screen)
        {
            Stop(ExitCode.Exit);
        }

        //////////////////////////////////////////////////////////////////////////////

        public static void RunTest1()
        {
            Field.Current().RunTest1();
        }

        public static void RunTest2()
        {
            Field.Current().RunTest2();
        }

        public static void RunTest3()
        {
            Field.Current().RunTest3();
        }

        public static void RunTest4()
        {
            Field.Current().RunTest4();
        }

        public static void RunTest5()
        {
            Field.Current().RunTest5();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        protected Field Field
        {
            get { return Game.Current.Field; }
        }

        protected List<Player> GetPlayerList()
        {
            return game.GetPlayersList();
        }

        protected PlayerList GetPlayers()
        {
            return game.GetPlayers();
        }

        public Game game
        {
            get { return m_game; }
            protected set
            {
                if (m_game != value && m_game != null)
                {
                    m_game.Destroy();
                }
                m_game = value;
            }
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
