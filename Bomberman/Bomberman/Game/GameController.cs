using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Game.Scenes;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Microsoft.Xna.Framework.Input;
using BombermanCommon.Resources;
using Bomberman.Content;
using Assets;
using BomberEngine.Core.Input;

namespace Bomberman.Game
{
    public class GameController : Controller
    {
        private GameScene gameScene;
        private GameCheats cheats;

        private Game game;
        private KeyboardListenerList keyboardListeners;

        public GameController()
        {
            keyboardListeners = new KeyboardListenerList();
            Application.Input().SetKeyboardListener(keyboardListeners);

            game = new Game();
            InitPlayers();
            InitField(A.sch_X);

            gameScene = new GameScene();
        }

        private void InitPlayers()
        {
            KeyboardPlayerInput keyboardInput1 = new KeyboardPlayerInput();
            keyboardInput1.Map(Keys.W, PlayerAction.Up);
            keyboardInput1.Map(Keys.A, PlayerAction.Left);
            keyboardInput1.Map(Keys.S, PlayerAction.Down);
            keyboardInput1.Map(Keys.D, PlayerAction.Right);
            keyboardInput1.Map(Keys.OemCloseBrackets, PlayerAction.Bomb);
            keyboardInput1.Map(Keys.OemOpenBrackets, PlayerAction.Special);
            keyboardListeners.Add(keyboardInput1);

            Player player1 = new Player(0, keyboardInput1);
            game.AddPlayer(player1);

            KeyboardPlayerInput keyboardInput2 = new KeyboardPlayerInput();
            keyboardInput2.Map(Keys.Up, PlayerAction.Up);
            keyboardInput2.Map(Keys.Left, PlayerAction.Left);
            keyboardInput2.Map(Keys.Down, PlayerAction.Down);
            keyboardInput2.Map(Keys.Right, PlayerAction.Right);
            keyboardListeners.Add(keyboardInput2);

            Player player2 = new Player(1, keyboardInput2);
            game.AddPlayer(player2);

            cheats = new GameCheats(game);
            keyboardListeners.Add(cheats);
        }

        private void InitField(int schemeId)
        {
            Scheme scheme = Helper.GetScheme(schemeId);
            game.LoadField(scheme); 
        }

        protected override void OnStart()
        {
            StartScene(gameScene);
        }
    }
}
