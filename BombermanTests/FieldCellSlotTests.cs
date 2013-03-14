using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;

namespace BombermanTests
{
    [TestClass]
    public class FieldCellSlotTests
    {
        [TestMethod]
        public void TestAdd1()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb = new Bomb(player);

            slot.AddCell(player);
            slot.AddCell(bomb);

            AssertNodes(slot, bomb, player);
        }

        [TestMethod]
        public void TestAdd2()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb = new Bomb(player);

            slot.AddCell(bomb);
            slot.AddCell(player);
            
            AssertNodes(slot, bomb, player);
        }

        [TestMethod]
        public void TestAdd3()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.AddCell(bomb1);
            slot.AddCell(bomb2);
            slot.AddCell(player);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd4()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.AddCell(bomb1);
            slot.AddCell(player);
            slot.AddCell(bomb2);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd5()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.AddCell(player);
            slot.AddCell(bomb1);
            slot.AddCell(bomb2);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd6()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);
            SolidCell solid = new SolidCell(0, 0);

            slot.AddCell(player);
            slot.AddCell(bomb1);
            slot.AddCell(bomb2);
            slot.AddCell(solid);

            AssertNodes(slot, solid, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd7()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            SolidCell solid = new SolidCell(0, 0);

            slot.AddCell(solid);
            slot.AddCell(player1);
            slot.AddCell(bomb1);
            slot.AddCell(bomb2);
            slot.AddCell(player2);

            AssertNodes(slot, solid, bomb1, bomb2, player1, player2);
        }

        [TestMethod]
        public void TestAdd8()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            slot.AddCell(player1);
            slot.AddCell(bomb1);
            slot.AddCell(bomb2);
            slot.AddCell(player2);

            AssertNodes(slot, bomb1, bomb2, player1, player2);
        }

        [TestMethod]
        public void TestMix1()
        {
            FieldCellSlot slot = new FieldCellSlot(0, 0);

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            SolidCell solid = new SolidCell(0, 0);

            slot.AddCell(solid);
            slot.AddCell(player1);
            slot.AddCell(bomb1);
            slot.AddCell(bomb2);
            slot.RemoveCell(player1);
            slot.AddCell(player2);

            AssertNodes(slot, solid, bomb1, bomb2, player2);
        }

        private void AssertNodes(FieldCellSlot slot, params FieldCell[] nodes)
        {
            //Assert.AreEqual(slot.Size(), nodes.Length);

            //LinkedListNode<FieldCell> node = slot.Cells().First;
            //for (int i = 0; i < nodes.Length; ++i)
            //{
            //    Assert.AreEqual(node.Value, nodes[i]);
            //    node = node.Next;
            //}

            //Assert.IsNull(node);
        }
    }
}
