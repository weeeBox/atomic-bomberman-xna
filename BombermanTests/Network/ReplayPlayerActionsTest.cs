using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay;
using Bomberman.Gameplay.Elements.Players;
using BombermanTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BombermanTests.Network
{
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
            const float FrameTime = 0.016f;

            Application.sharedApplication = new ApplicationMock();
            Application.frameTime = FrameTime;

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

            PlayerState[] states = new PlayerState[actions.Length];

            Player player = clientGame.GetPlayers().list[0];
            player.lockAnimations = true;
            for (int i = 0; i < actions.Length; ++i)
            {
                player.input.Force(CreateInputMask(actions[i]));
                player.Update(FrameTime);
                if (player.IsMoving())
                {
                    player.UpdateMoving(FrameTime);
                }
                player.FillState(ref states[i]);

                client.SendPlayingSendMessage();
            }

            int ackSeq = 0;
            client.channel.acknowledgedSequence = ackSeq;
            player.UpdateFromNetwork(ref states[ackSeq]);

            player.lockAnimations = true;
            client.ReplayPlayerActions(client.channel);

            PlayerState finalState = states[states.Length - 1];
            Assert.AreEqual(finalState.px, player.px);
            Assert.AreEqual(finalState.py, player.py);
            Assert.AreEqual(finalState.direction, player.direction);
            Assert.AreEqual(finalState.moving, player.IsMoving());
            Assert.AreEqual(finalState.speed, player.GetSpeed());
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
