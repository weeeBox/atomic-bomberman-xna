using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Game.Screens;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Microsoft.Xna.Framework.Input;
using BombermanCommon.Resources;
using Bomberman.Content;
using Assets;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;

namespace Bomberman.Game
{
    public class GameController : Controller, IButtonDelegate
    {
        private const int SCREEN_GAME = 1;
        private const int SCREEN_PAUSE = 2;

        private GameScreen gameScreen;
        private Game game;

        private CCommand[] gameCommands = 
        {
            new Cmd_infect(),
            new Cmd_add(),
            new Cmd_map(),
            new Cmd_map_restart(),
        };

        public GameController()
        {
            game = new Game();
            game.AddPlayer(new Player(0));
            game.AddPlayer(new Player(1));
            game.AddPlayer(new Player(2));

            InitField(A.sch_X);

            gameScreen = new GameScreen();
            gameScreen.id = SCREEN_GAME;

            InitPlayers();
        }

        protected override void OnStart()
        {
            Console().RegisterCommands(gameCommands);
            StartScreen(gameScreen);
        }

        protected override void OnStop()
        {
            Console().UnregisterCommands(gameCommands);
        }

        private void InitPlayers()
        {
            List<Player> players = game.GetPlayers().list;

            PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
            keyboardInput1.Map(KeyCode.KB_W, PlayerAction.Up);
            keyboardInput1.Map(KeyCode.KB_A, PlayerAction.Left);
            keyboardInput1.Map(KeyCode.KB_S, PlayerAction.Down);
            keyboardInput1.Map(KeyCode.KB_D, PlayerAction.Right);
            keyboardInput1.Map(KeyCode.KB_OemCloseBrackets, PlayerAction.Bomb);
            keyboardInput1.Map(KeyCode.KB_OemOpenBrackets, PlayerAction.Special);
            gameScreen.AddKeyListener(keyboardInput1);

            players[0].SetPlayerInput(keyboardInput1);

            PlayerKeyboardInput keyboardInput2 = new PlayerKeyboardInput();
            keyboardInput2.Map(KeyCode.KB_Up, PlayerAction.Up);
            keyboardInput2.Map(KeyCode.KB_Left, PlayerAction.Left);
            keyboardInput2.Map(KeyCode.KB_Down, PlayerAction.Down);
            keyboardInput2.Map(KeyCode.KB_Right, PlayerAction.Right);
            gameScreen.AddKeyListener(keyboardInput2);

            players[1].SetPlayerInput(keyboardInput2);

            PlayerGamePadInput gamePadInput = new PlayerGamePadInput(0);
            gamePadInput.Map(KeyCode.GP_A, PlayerAction.Bomb);
            gamePadInput.Map(KeyCode.GP_B, PlayerAction.Special);
            gameScreen.AddKeyListener(gamePadInput);
            gameScreen.AddUpdatabled(gamePadInput);

            players[2].SetPlayerInput(gamePadInput);
        }

        private void InitField(int schemeId)
        {
            Scheme scheme = Helper.GetScheme(schemeId);
            game.LoadField(scheme); 
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
                if (OnKeyPressed(keyEvent.arg))
                {
                    return true;
                }
            }

            return false;
        }

        private bool OnKeyPressed(KeyEventArg e)
        {
            if (e.key == KeyCode.KB_Escape)
            {
                Screen screen = CurrentScreen();
                if (screen.id == SCREEN_GAME)
                {
                    PauseScreen pauseScreen = new PauseScreen(this);
                    pauseScreen.id = SCREEN_PAUSE;
                    StartNextScreen(pauseScreen);
                    return true;
                }

                if (screen.id == SCREEN_PAUSE)
                {
                    screen.Finish();
                    return true;
                }
            }

            return false;
        }

        public void OnButtonPress(AbstractButton button)
        {
            switch (button.id)
            {
                case PauseScreen.BUTTON_RESUME:
                    CurrentScreen().Finish();
                    break;

                case PauseScreen.BUTTON_EXIT:
                    Stop();
                    break;
            }
        }
    }
}
