using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;
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

        private CCommand[] gameCommands;

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
            Assert.IsNull(gameCommands);
            gameCommands = CreateCommands();

            GetConsole().RegisterCommands(gameCommands);

            RegisterNotification(GameNotifications.RoundEnded, RoundEndedNotification);
            RegisterNotification(GameNotifications.GameEnded, GameEndedNotification);
            RegisterNotification(GameNotifications.RoundRestarted, RoundRestartedNotification);
        }

        protected override void OnStop()
        {
            Assert.IsNotNull(gameCommands);
            GetConsole().UnregisterCommands(gameCommands);
            gameCommands = null;

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
            Assert.IsTrue(CurrentScreen() is GameScreen);
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

        #region Commands

        private void P1_Up_Down()       { ActionDown(0, PlayerAction.Up); }
        private void P1_Up_Up()         {   ActionUp(0, PlayerAction.Up); }
        private void P1_Down_Down()     { ActionDown(0, PlayerAction.Down); }
        private void P1_Down_Up()       {   ActionUp(0, PlayerAction.Down); }
        private void P1_Left_Down()     { ActionDown(0, PlayerAction.Left); }
        private void P1_Left_Up()       {   ActionUp(0, PlayerAction.Left); }
        private void P1_Right_Down()    { ActionDown(0, PlayerAction.Right); }
        private void P1_Right_Up()      {   ActionUp(0, PlayerAction.Right); }
        private void P1_Bomb_Down()     { ActionDown(0, PlayerAction.Bomb); }
        private void P1_Bomb_Up()       {   ActionUp(0, PlayerAction.Bomb); }
        private void P1_Special_Down()  { ActionDown(0, PlayerAction.Special); }
        private void P1_Special_Up()    {   ActionUp(0, PlayerAction.Special); }

        private void P2_Up_Down()       { ActionDown(1, PlayerAction.Up); }
        private void P2_Up_Up()         {   ActionUp(1, PlayerAction.Up); }
        private void P2_Down_Down()     { ActionDown(1, PlayerAction.Down); }
        private void P2_Down_Up()       {   ActionUp(1, PlayerAction.Down); }
        private void P2_Left_Down()     { ActionDown(1, PlayerAction.Left); }
        private void P2_Left_Up()       {   ActionUp(1, PlayerAction.Left); }
        private void P2_Right_Down()    { ActionDown(1, PlayerAction.Right); }
        private void P2_Right_Up()      {   ActionUp(1, PlayerAction.Right); }
        private void P2_Bomb_Down()     { ActionDown(1, PlayerAction.Bomb); }
        private void P2_Bomb_Up()       {   ActionUp(1, PlayerAction.Bomb); }
        private void P2_Special_Down()  { ActionDown(1, PlayerAction.Special); }
        private void P2_Special_Up()    {   ActionUp(1, PlayerAction.Special); }

        private void P3_Up_Down()       { ActionDown(2, PlayerAction.Up); }
        private void P3_Up_Up()         {   ActionUp(2, PlayerAction.Up); }
        private void P3_Down_Down()     { ActionDown(2, PlayerAction.Down); }
        private void P3_Down_Up()       {   ActionUp(2, PlayerAction.Down); }
        private void P3_Left_Down()     { ActionDown(2, PlayerAction.Left); }
        private void P3_Left_Up()       {   ActionUp(2, PlayerAction.Left); }
        private void P3_Right_Down()    { ActionDown(2, PlayerAction.Right); }
        private void P3_Right_Up()      {   ActionUp(2, PlayerAction.Right); }
        private void P3_Bomb_Down()     { ActionDown(2, PlayerAction.Bomb); }
        private void P3_Bomb_Up()       {   ActionUp(2, PlayerAction.Bomb); }
        private void P3_Special_Down()  { ActionDown(2, PlayerAction.Special); }
        private void P3_Special_Up()    {   ActionUp(2, PlayerAction.Special); }

        private void P4_Up_Down()       { ActionDown(3, PlayerAction.Up); }
        private void P4_Up_Up()         {   ActionUp(3, PlayerAction.Up); }
        private void P4_Down_Down()     { ActionDown(3, PlayerAction.Down); }
        private void P4_Down_Up()       {   ActionUp(3, PlayerAction.Down); }
        private void P4_Left_Down()     { ActionDown(3, PlayerAction.Left); }
        private void P4_Left_Up()       {   ActionUp(3, PlayerAction.Left); }
        private void P4_Right_Down()    { ActionDown(3, PlayerAction.Right); }
        private void P4_Right_Up()      {   ActionUp(3, PlayerAction.Right); }
        private void P4_Bomb_Down()     { ActionDown(3, PlayerAction.Bomb); }
        private void P4_Bomb_Up()       {   ActionUp(3, PlayerAction.Bomb); }
        private void P4_Special_Down()  { ActionDown(3, PlayerAction.Special); }
        private void P4_Special_Up()    {   ActionUp(3, PlayerAction.Special); }

        private void P5_Up_Down()       { ActionDown(4, PlayerAction.Up); }
        private void P5_Up_Up()         {   ActionUp(4, PlayerAction.Up); }
        private void P5_Down_Down()     { ActionDown(4, PlayerAction.Down); }
        private void P5_Down_Up()       {   ActionUp(4, PlayerAction.Down); }
        private void P5_Left_Down()     { ActionDown(4, PlayerAction.Left); }
        private void P5_Left_Up()       {   ActionUp(4, PlayerAction.Left); }
        private void P5_Right_Down()    { ActionDown(4, PlayerAction.Right); }
        private void P5_Right_Up()      {   ActionUp(4, PlayerAction.Right); }
        private void P5_Bomb_Down()     { ActionDown(4, PlayerAction.Bomb); }
        private void P5_Bomb_Up()       {   ActionUp(4, PlayerAction.Bomb); }
        private void P5_Special_Down()  { ActionDown(4, PlayerAction.Special); }
        private void P5_Special_Up()    {   ActionUp(4, PlayerAction.Special); }

        private void P6_Up_Down()       { ActionDown(5, PlayerAction.Up); }
        private void P6_Up_Up()         {   ActionUp(5, PlayerAction.Up); }
        private void P6_Down_Down()     { ActionDown(5, PlayerAction.Down); }
        private void P6_Down_Up()       {   ActionUp(5, PlayerAction.Down); }
        private void P6_Left_Down()     { ActionDown(5, PlayerAction.Left); }
        private void P6_Left_Up()       {   ActionUp(5, PlayerAction.Left); }
        private void P6_Right_Down()    { ActionDown(5, PlayerAction.Right); }
        private void P6_Right_Up()      {   ActionUp(5, PlayerAction.Right); }
        private void P6_Bomb_Down()     { ActionDown(5, PlayerAction.Bomb); }
        private void P6_Bomb_Up()       {   ActionUp(5, PlayerAction.Bomb); }
        private void P6_Special_Down()  { ActionDown(5, PlayerAction.Special); }
        private void P6_Special_Up()    {   ActionUp(5, PlayerAction.Special); }

        private CCommand[] CreateCommands()
        {
            return new CCommand[]
            {
                new Cmd_infect(),
                new Cmd_add(),
                new Cmd_map(),
                new Cmd_map_restart(),

                new Cmd_action("+up",         P1_Up_Down),
                new Cmd_action("-up",         P1_Up_Up),
                new Cmd_action("+down",       P1_Down_Down),
                new Cmd_action("-down",       P1_Down_Up),
                new Cmd_action("+left",       P1_Left_Down),
                new Cmd_action("-left",       P1_Left_Up),
                new Cmd_action("+right",      P1_Right_Down),
                new Cmd_action("-right",      P1_Right_Up),
                new Cmd_action("+bomb",       P1_Bomb_Down),
                new Cmd_action("-bomb",       P1_Bomb_Up),
                new Cmd_action("+special",    P1_Special_Down),
                new Cmd_action("-special",    P1_Special_Up),

                new Cmd_action("+up2",        P2_Up_Down),
                new Cmd_action("-up2",        P2_Up_Up),
                new Cmd_action("+down2",      P2_Down_Down),
                new Cmd_action("-down2",      P2_Down_Up),
                new Cmd_action("+left2",      P2_Left_Down),
                new Cmd_action("-left2",      P2_Left_Up),
                new Cmd_action("+right2",     P2_Right_Down),
                new Cmd_action("-right2",     P2_Right_Up),
                new Cmd_action("+bomb2",      P2_Bomb_Down),
                new Cmd_action("-bomb2",      P2_Bomb_Up),
                new Cmd_action("+special2",   P2_Special_Down),
                new Cmd_action("-special2",   P2_Special_Up),

                new Cmd_action("+up3",        P3_Up_Down),
                new Cmd_action("-up3",        P3_Up_Up),
                new Cmd_action("+down3",      P3_Down_Down),
                new Cmd_action("-down3",      P3_Down_Up),
                new Cmd_action("+left3",      P3_Left_Down),
                new Cmd_action("-left3",      P3_Left_Up),
                new Cmd_action("+right3",     P3_Right_Down),
                new Cmd_action("-right3",     P3_Right_Up),
                new Cmd_action("+bomb3",      P3_Bomb_Down),
                new Cmd_action("-bomb3",      P3_Bomb_Up),
                new Cmd_action("+special3",   P3_Special_Down),
                new Cmd_action("-special3",   P3_Special_Up),

                new Cmd_action("+up4",        P4_Up_Down),
                new Cmd_action("-up4",        P4_Up_Up),
                new Cmd_action("+down4",      P4_Down_Down),
                new Cmd_action("-down4",      P4_Down_Up),
                new Cmd_action("+left4",      P4_Left_Down),
                new Cmd_action("-left4",      P4_Left_Up),
                new Cmd_action("+right4",     P4_Right_Down),
                new Cmd_action("-right4",     P4_Right_Up),
                new Cmd_action("+bomb4",      P4_Bomb_Down),
                new Cmd_action("-bomb4",      P4_Bomb_Up),
                new Cmd_action("+special4",   P4_Special_Down),
                new Cmd_action("-special4",   P4_Special_Up),

                new Cmd_action("+up5",        P5_Up_Down),
                new Cmd_action("-up5",        P5_Up_Up),
                new Cmd_action("+down5",      P5_Down_Down),
                new Cmd_action("-down5",      P5_Down_Up),
                new Cmd_action("+left5",      P5_Left_Down),
                new Cmd_action("-left5",      P5_Left_Up),
                new Cmd_action("+right5",     P5_Right_Down),
                new Cmd_action("-right5",     P5_Right_Up),
                new Cmd_action("+bomb5",      P5_Bomb_Down),
                new Cmd_action("-bomb5",      P5_Bomb_Up),
                new Cmd_action("+special5",   P5_Special_Down),
                new Cmd_action("-special5",   P5_Special_Up),

                new Cmd_action("+up6",        P6_Up_Down),
                new Cmd_action("-up6",        P6_Up_Up),
                new Cmd_action("+down6",      P6_Down_Down),
                new Cmd_action("-down6",      P6_Down_Up),
                new Cmd_action("+left6",      P6_Left_Down),
                new Cmd_action("-left6",      P6_Left_Up),
                new Cmd_action("+right6",     P6_Right_Down),
                new Cmd_action("-right6",     P6_Right_Up),
                new Cmd_action("+bomb6",      P6_Bomb_Down),
                new Cmd_action("-bomb6",      P6_Bomb_Up),
                new Cmd_action("+special6",   P6_Special_Down),
                new Cmd_action("-special6",   P6_Special_Up),

                new Cmd_action("test1", RunTest1),
                new Cmd_action("test2", RunTest2),
                new Cmd_action("test3", RunTest3),
                new Cmd_action("test4", RunTest4),
                new Cmd_action("test5", RunTest5),
            };
        }

        private void ActionDown(int playerIndex, PlayerAction action)
        {
            InputMapping.SetKeyboardAction(playerIndex, action, true);
        }

        private void ActionUp(int playerIndex, PlayerAction action)
        {
            InputMapping.SetKeyboardAction(playerIndex, action, false);
        }

        protected PlayerInput CreatePlayerInput(InputType type)
        {
            return InputMapping.CreatePlayerInput(type);
        }

        #endregion

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
            Assert.IsTrue(resultScreen != null);

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
