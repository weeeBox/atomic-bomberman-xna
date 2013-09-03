using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Util;
using Bomberman.Game;

namespace BombermanTests
{
    [TestClass]
    public class FieldCellTests
    {
        [TestMethod]
        public void TestCell2CellCollision()
        {
            DummyCell cell1 = new DummyCell(0, 0);
            DummyCell cell2 = new DummyCell(0, 0);

            Assert.IsTrue(cell1.Collides(cell2));
        }

        [TestMethod]
        public void TestBounds2CellCollision()
        {
            DummyCell cell1 = new DummyCell(2, 2);
            DummyCell cell2 = new DummyCell(2, 2);

            Assert.IsTrue(cell1.CheckBounds2CellCollision(cell2));

            float dx = 0.25f * Constant.CELL_WIDTH;
            float dy = 0.25f * Constant.CELL_HEIGHT;

            AssertBounds2CellCollides(cell1, cell2, dx, dy, true);

            dx = 0.5f * Constant.CELL_WIDTH;
            dy = 0.5f * Constant.CELL_HEIGHT;

            AssertBounds2CellCollides(cell1, cell2, dx, dy, true);

            dx = 0.75f * Constant.CELL_WIDTH;
            dy = 0.75f * Constant.CELL_HEIGHT;

            AssertBounds2CellCollides(cell1, cell2, dx, dy, true);

            dx = Constant.CELL_WIDTH;
            dy = Constant.CELL_HEIGHT;

            AssertBounds2CellCollides(cell1, cell2, dx, dy, false);

            dx = 1.25f * Constant.CELL_WIDTH;
            dy = 1.25f * Constant.CELL_HEIGHT;

            AssertBounds2CellCollides(cell1, cell2, dx, dy, false);
        }

        private void AssertBounds2CellCollides(DummyCell cell1, DummyCell cell2, float dx, float dy, bool collides)
        {
            float px = cell1.px;
            float py = cell2.py;

            cell1.SetPos(px - dx, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px + dx, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px + dx, py);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px + dx, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px - dx, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px - dx, py);
            Assert.AreEqual(collides, cell1.CheckBounds2CellCollision(cell2));

            cell1.SetPos(px, py);
        }

        private void AssertBounds2BoundsCollides(DummyCell cell1, DummyCell cell2, float dx, float dy, bool collides)
        {
            float px = cell1.px;
            float py = cell2.py;

            cell1.SetPos(px - dx, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px + dx, py - dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px + dx, py);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px + dx, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px - dx, py + dy);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px - dx, py);
            Assert.AreEqual(collides, cell1.CheckBounds2BoundsCollision(cell2));

            cell1.SetPos(px, py);
        }

        class DummyCell : FieldCell
        {
            public DummyCell(int cx, int cy)
                : base(FieldCellType.Count, cx, cy)
            {
            }

            public new bool CheckCell2CellCollision(FieldCell other)
            {
                return base.CheckCell2CellCollision(other);
            }

            public new bool CheckBounds2CellCollision(FieldCell other)
            {
                return base.CheckBounds2CellCollision(other);
            }

            public new bool CheckBounds2BoundsCollision(FieldCell other)
            {
                return base.CheckBounds2BoundsCollision(other);
            }
        }
    }
}
