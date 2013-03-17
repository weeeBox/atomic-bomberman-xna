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
    public class MovableCellListTests
    {
        [TestMethod]
        public void TestAdd1()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb = new Bomb(player);

            slot.Add(player);
            slot.Add(bomb);

            AssertNodes(slot, bomb, player);
        }

        [TestMethod]
        public void TestAdd2()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb = new Bomb(player);

            slot.Add(bomb);
            slot.Add(player);
            
            AssertNodes(slot, bomb, player);
        }

        [TestMethod]
        public void TestAdd3()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.Add(bomb1);
            slot.Add(bomb2);
            slot.Add(player);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd4()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.Add(bomb1);
            slot.Add(player);
            slot.Add(bomb2);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd5()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.Add(player);
            slot.Add(bomb1);
            slot.Add(bomb2);

            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd6()
        {
            MovableCellList slot = new MovableCellList();

            Player player = new Player(0);
            Bomb bomb1 = new Bomb(player);
            Bomb bomb2 = new Bomb(player);

            slot.Add(player);
            slot.Add(bomb1);
            slot.Add(bomb2);
            
            AssertNodes(slot, bomb1, bomb2, player);
        }

        [TestMethod]
        public void TestAdd7()
        {
            MovableCellList slot = new MovableCellList();

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            slot.Add(player1);
            slot.Add(bomb1);
            slot.Add(bomb2);
            slot.Add(player2);

            AssertNodes(slot, bomb1, bomb2, player1, player2);
        }

        [TestMethod]
        public void TestAdd8()
        {
            MovableCellList slot = new MovableCellList();

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            slot.Add(player1);
            slot.Add(bomb1);
            slot.Add(bomb2);
            slot.Add(player2);

            AssertNodes(slot, bomb1, bomb2, player1, player2);
        }

        [TestMethod]
        public void TestMix1()
        {
            MovableCellList slot = new MovableCellList();

            Player player1 = new Player(0);
            Player player2 = new Player(1);

            Bomb bomb1 = new Bomb(player1);
            Bomb bomb2 = new Bomb(player2);

            slot.Add(player1);
            slot.Add(bomb1);
            slot.Add(bomb2);
            slot.Remove(player1);
            slot.Add(player2);

            AssertNodes(slot, bomb1, bomb2, player2);
        }

        private void AssertNodes(MovableCellList slot, params MovableCell[] array)
        {
            Assert.AreEqual(slot.Size(), array.Length);

            LinkedListNode<MovableCell> node = slot.list.First;
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.AreEqual(node.Value, array[i]);
                node = node.Next;
            }

            Assert.IsNull(node);
        }
    }
}
