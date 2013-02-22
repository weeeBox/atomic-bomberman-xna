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

        public GameController()
        {
            game = new Game();
            game.AddPlayer(new Player(0));
            game.AddPlayer(new Player(1));
            game.AddPlayer(new Player(2));

            InitField(A.sch_X);

            gameScene = new GameScene();

            InitPlayers();
            InitCheats();
        }

        private void InitPlayers()
        {
            List<Player> players = game.GetPlayers().list;

            PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
            keyboardInput1.Map(Keys.W, PlayerAction.Up);
            keyboardInput1.Map(Keys.A, PlayerAction.Left);
            keyboardInput1.Map(Keys.S, PlayerAction.Down);
            keyboardInput1.Map(Keys.D, PlayerAction.Right);
            keyboardInput1.Map(Keys.OemCloseBrackets, PlayerAction.Bomb);
            keyboardInput1.Map(Keys.OemOpenBrackets, PlayerAction.Special);
            gameScene.AddKeyboardListener(keyboardInput1);

            players[0].SetPlayerInput(keyboardInput1);

            PlayerGamePadInput gamePadInput = new PlayerGamePadInput(0);
            gamePadInput.Map(Buttons.A, PlayerAction.Bomb);
            gamePadInput.Map(Buttons.B, PlayerAction.Special);
            gameScene.AddGamePadListener(gamePadInput);
            gameScene.AddUpdatabled(gamePadInput);

            players[1].SetPlayerInput(gamePadInput);
        }

        private void InitCheats()
        {
            cheats = new GameCheats(game);
            gameScene.AddKeyboardListener(cheats);
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
