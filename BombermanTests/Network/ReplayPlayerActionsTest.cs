using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay;
using Bomberman.Gameplay.Elements.Players;
using BombermanTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BombermanTests.Network
{
    [TestClass]
    public class ReplayPlayerActionsTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            new InputMapping();
            MathHelp.InitRandom(0);
        }

        [TestMethod]
        public void TestReplayActions()
        {
            Application.sharedApplication = new ApplicationMock();
            Application.frameTime = 0.016f;

            Scheme scheme = new EmptySchemeMock("Test", 90);
            GameSettings settings = new GameSettings(scheme);

            GameControllerClientMock client = new GameControllerClientMock();
            Game clientGame = client.game;
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard1));
            clientGame.SetupField(scheme);

            client.CreateNetChannel(null, clientGame.GetPlayers().list);

            PlayerAction[][] actions =
            {
                new PlayerAction[] {},
                new PlayerAction[] { PlayerAction.Down },
                new PlayerAction[] { PlayerAction.Down },
                new PlayerAction[] { PlayerAction.Down },
                new PlayerAction[] {},
                new PlayerAction[] {},
                new PlayerAction[] { PlayerAction.Down },
                new PlayerAction[] { PlayerAction.Down },
            };

            Player player = clientGame.GetPlayers().list[0];
            for (int i = 0; i < actions.Length; ++i)
            {
                player.input.Force(CreateInputMask(actions[i]));
                player.Update(0.016f);
                client.SendPlayingSendMessage();
            }
        }

        private int CreateInputMask(params PlayerAction[] actions)
        {
            int mask = 0;
            for (int i = 0; i < actions.Length; ++i)
            {
                mask |= 1 << (int)actions[i];
            }
            return mask;
        }
    }
}
