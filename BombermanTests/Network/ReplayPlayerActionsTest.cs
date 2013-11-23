using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Content;
using BombermanTests.Mocks;
using Bomberman.Gameplay;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Networking;
using Lidgren.Network;
using Bomberman.Gameplay.Elements.Fields;

namespace BombermanTests.Network
{
    [TestClass]
    public class ReplayPlayerActionsTest
    {
        [TestMethod]
        public void TestReplayActions()
        {
            Scheme scheme = new EmptySchemeMock("Test", 90);
            GameSettings settings = new GameSettings(scheme);

            GameControllerClientMock client = new GameControllerClientMock();
            Game clientGame = client.game;
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard1));
            clientGame.SetupField(scheme);

            client.CreateNetChannel();

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
            
            for (int i = 0; i < actions.Length; ++i)
            {
                PlayerAction[] arr = actions[i];
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
