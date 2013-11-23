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

namespace BombermanTests.Network
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

            GameControllerNetworkMock server = new GameControllerNetworkMock();

            Game serverGame = server.game;
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
            server.WriteFieldState(buffer, channel);

            GameControllerNetworkMock client = new GameControllerNetworkMock();
            Game clientGame = client.game;
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard1));
            clientGame.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard2));
            clientGame.SetupField(scheme);

            client.ReadFieldState(buffer);

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

        [TestMethod]
        public void TestPlayingMessage()
        {
            GameControllerServer server = new GameControllerServerMock();
            GameControllerClientMock client = new GameControllerClientMock();

            Game serverGame = server.game;
            serverGame.AddPlayer(new Player(0), new PlayerKeyInput());
            serverGame.AddPlayer(new Player(1), new PlayerNetworkInput());

            Player svLocalPlayer = serverGame.GetPlayersList()[0];
            Player svRemotePlayer = serverGame.GetPlayersList()[1];

            Game clientGame = client.game;
            clientGame.AddPlayer(new Player(0), new PlayerNetworkInput());
            clientGame.AddPlayer(new Player(1), new PlayerKeyInput());

            Player clRemotePlayer = clientGame.GetPlayersList()[0];
            Player clLocalPlayer = clientGame.GetPlayersList()[1];

            NetChannel svChannel = new NetChannel(null, svRemotePlayer);
            NetChannel clChannel = new NetChannel(null, clLocalPlayer);

            clLocalPlayer.input.SetActionPressed(PlayerAction.Down, true);
            clLocalPlayer.input.SetActionPressed(PlayerAction.Left, true);

            NetBuffer buffer = new NetBuffer();
            client.WritePlayingMessage(buffer, clChannel);

            server.ReadPlayingMessage(buffer, svChannel);

            Assert.AreEqual(svLocalPlayer.input.mask, clRemotePlayer.input.mask);
        }
    }

    class GameControllerNetworkMock : GameControllerNetwork
    {
        public GameControllerNetworkMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }

    class GameControllerClientMock : GameControllerClient
    {
        public GameControllerClientMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }

    class GameControllerServerMock : GameControllerServer
    {
        public GameControllerServerMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }
}
