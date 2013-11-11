using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Gameplay.Multiplayer;
using Bomberman.Gameplay;
using BombermanTests.Mocks;
using Lidgren.Network;
using Bomberman.Networking;
using Bomberman.Gameplay.Elements.Players;
using BombermanCommon.Resources;
using Bomberman.Content;
using BombermanCommon.Resources.Scheme;
using Bomberman.Gameplay.Elements.Fields;
using BomberEngine;

namespace BombermanTests
{
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
    using Bomberman.Gameplay.Elements;

    [TestClass]
    public class GameControllerNetworkTest
    {
        struct PlayerPos
        {
            public float x;
            public float y;

            public PlayerPos(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            new InputMapping();
            MathHelp.InitRandom(0);
        }

        [TestMethod]
        public void TestBitPacking()
        {
            NetBuffer buffer = new NetBuffer();
            for (int i = 0; i < 10; ++i)
            {
                buffer.WritePlayerIndex(i);
            }

            for (int cx = 0; cx < 15; ++cx)
            {
                for (int cy = 0; cy < 11; ++cy)
                {
                    buffer.WriteCellCord(cx);
                    buffer.WriteCellCord(cy);
                }
            }

            FieldCellType[] types = { FieldCellType.Solid, FieldCellType.Brick, FieldCellType.Powerup, FieldCellType.Flame };
            for (int i = 0; i < types.Length; ++i)
            {
                buffer.WriteStaticCellType(types[i]);
            }

            Direction[] directions = { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT };
            for (int i = 0; i < directions.Length; ++i)
            {
                buffer.Write(directions[i]);
            }

            // check

            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(i, buffer.ReadPlayerIndex());
            }

            for (int cx = 0; cx < 15; ++cx)
            {
                for (int cy = 0; cy < 11; ++cy)
                {
                    Assert.AreEqual(cx, buffer.ReadCellCord());
                    Assert.AreEqual(cy, buffer.ReadCellCord());
                }
            }

            for (int i = 0; i < types.Length; ++i)
            {
                Assert.AreEqual(types[i], buffer.ReadStaticCellType());
            }

            for (int i = 0; i < directions.Length; ++i)
            {
                Assert.AreEqual(directions[i], buffer.ReadDirection());
            }
        }

        [TestMethod]
        public void TestFieldState()
        {
            Scheme scheme = new SchemeMock("Test", 90);
            GameSettings settings = new GameSettings(scheme);

            GameControllerNetworkMock serverController = CreateNetworkController(settings);

            Game serverGame = serverController.game;
            serverGame.AddPlayer(new Player(0));
            serverGame.AddPlayer(new Player(1));
            serverGame.AddPlayer(new Player(2));
            serverGame.AddPlayer(new Player(3));

            serverGame.LoadField(scheme);
            List<Player> localPlayers = new List<Player>();
            localPlayers.Add(serverGame.GetPlayersList()[2]);
            localPlayers.Add(serverGame.GetPlayersList()[3]);

            NetChannel channel = new NetChannel(null, localPlayers);

            NetBuffer buffer = new NetBuffer();
            serverController.WriteFieldState(buffer, channel);

            GameControllerNetworkMock clientController = CreateNetworkController(settings);
            Game clientGame = clientController.game;
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard1));
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard2));
            clientGame.SetupField(scheme);

            clientController.ReadFieldState(buffer);

            // check field
            FieldCellSlot[] serverSlots = serverGame.Field.GetSlots();
            FieldCellSlot[] clientSlots = clientGame.Field.GetSlots();

            Assert.AreEqual(serverSlots.Length, clientSlots.Length);
            for (int i = 0; i < serverSlots.Length; ++i)
            {
                FieldCell servereCell = serverSlots[i].staticCell;
                FieldCell clientCell = clientSlots[i].staticCell;

                Assert.IsTrue(servereCell == null && clientCell == null || servereCell.EqualsTo(clientCell));
            }

            List<Player> serverPlayers = serverGame.GetPlayersList();
            List<Player> clientPlayers = clientGame.GetPlayersList();

            // check positions
            Assert.AreEqual(serverPlayers.Count, clientPlayers.Count);
            for (int i = 0; i < serverPlayers.Count; ++i)
            {
                Assert.IsTrue(serverPlayers[i].EqualsTo(clientPlayers[i]));
            }

            // check local/multiplayer
            Assert.IsTrue(clientPlayers[0].IsNetworkPlayer);
            Assert.IsTrue(clientPlayers[1].IsNetworkPlayer);
            Assert.IsFalse(clientPlayers[2].IsNetworkPlayer);
            Assert.IsFalse(clientPlayers[3].IsNetworkPlayer);
        }

        #region Helpers

        private GameControllerNetworkMock CreateNetworkController(GameSettings settings)
        {
            Scheme scheme = settings.scheme;
            FieldData fieldData = scheme.fieldData;

            GameMock game = new GameMock(fieldData.GetWidth(), fieldData.GetHeight());
            return new GameControllerNetworkMock(game, settings);
        }

        private Scheme Create(int[] values, int width, int height)
        {
            FieldBlocks[] blocks = new FieldBlocks[width * height];
            for (int i = 0; i < values.Length; ++i)
            {
                blocks[i] = (FieldBlocks)values[i];
            }

            Scheme scheme = new Scheme();
            scheme.fieldData = new FieldData(width, height, blocks);

            return scheme;
        }

        #endregion
    }

    class GameControllerNetworkMock : GameControllerNetwork
    {
        public GameControllerNetworkMock(Game game, GameSettings settings)
            : base(game, settings)
        {
        }

        public new void WriteFieldState(NetBuffer buffer, NetChannel channel)
        {
            base.WriteFieldState(buffer, channel);
        }

        public new void ReadFieldState(NetBuffer buffer)
        {
            base.ReadFieldState(buffer);
        }
    }
}
