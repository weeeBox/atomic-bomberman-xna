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
            //settings.inputEntries = new GameSettings.InputEntry[] {
            //    new GameSettings.InputEntry(0, InputMapping.CreatePlayerInput(InputType.Keyboard1)),
            //    new GameSettings.InputEntry(1, InputMapping.CreatePlayerInput(InputType.Keyboard2))
            //};

            Assert.Fail();

            GameControllerNetworkMock controller1 = CreateNetworkController(settings);

            Game game1 = controller1.game;
            game1.AddPlayer(new Player(0));
            game1.AddPlayer(new Player(1));
            game1.AddPlayer(new Player(2));
            game1.AddPlayer(new Player(3));

            game1.LoadField(scheme);
            List<Player> localPlayers = new List<Player>();
            localPlayers.Add(game1.GetPlayersList()[2]);
            localPlayers.Add(game1.GetPlayersList()[3]);

            NetChannel channel = new NetChannel(null, localPlayers);

            NetBuffer buffer = new NetBuffer();
            controller1.WriteFieldState(buffer, channel);

            GameControllerNetworkMock controller2 = CreateNetworkController(settings);
            Game game2 = controller2.game;
            game2.SetupField(scheme);

            controller2.ReadFieldState(buffer);

            // check field
            FieldCellSlot[] slots1 = game1.Field.GetSlots();
            FieldCellSlot[] slots2 = game2.Field.GetSlots();

            Assert.AreEqual(slots1.Length, slots2.Length);
            for (int i = 0; i < slots1.Length; ++i)
            {
                FieldCell cell1 = slots1[i].staticCell;
                FieldCell cell2 = slots2[i].staticCell;

                Assert.IsTrue(cell1 == null && cell2 == null || cell1.EqualsTo(cell2));
            }

            List<Player> players1 = game1.GetPlayersList();
            List<Player> players2 = game2.GetPlayersList();

            // check positions
            Assert.AreEqual(players1.Count, players2.Count);
            for (int i = 0; i < players1.Count; ++i)
            {
                Assert.IsTrue(players1[i].EqualsTo(players2[i]));
            }

            // check local/multiplayer
            Assert.IsTrue(players2[0].IsNetworkPlayer);
            Assert.IsTrue(players2[1].IsNetworkPlayer);
            Assert.IsFalse(players2[2].IsNetworkPlayer);
            Assert.IsFalse(players2[3].IsNetworkPlayer);
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
            : base(settings)
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
