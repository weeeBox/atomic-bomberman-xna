using Bomberman.Gameplay.Elements;
using Bomberman.Gameplay.Elements.Cells;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BombermanTests
{
    [TestClass]
    public class FlameCellTest
    {
        [TestMethod]
        public void TestFlags()
        {
            FlameCell cell = new FlameCell(null, 0, 0);

            cell.isCap = true;
            cell.isCenter = true;
            cell.isShort = true;
            cell.isGolden = true;

            cell.direction = Direction.DOWN;

            Assert.IsTrue(cell.isCap);
            Assert.IsTrue(cell.isCenter);
            Assert.IsTrue(cell.isShort);
            Assert.IsTrue(cell.isGolden);

            Assert.AreEqual(cell.direction, Direction.DOWN);

            cell.isCap = false;

            Assert.IsFalse(cell.isCap);
            Assert.IsTrue(cell.isCenter);
            Assert.IsTrue(cell.isShort);
            Assert.IsTrue(cell.isGolden);

            cell.isCenter = false;

            Assert.IsFalse(cell.isCap);
            Assert.IsFalse(cell.isCenter);
            Assert.IsTrue(cell.isShort);
            Assert.IsTrue(cell.isGolden);

            cell.isShort = false;

            Assert.IsFalse(cell.isCap);
            Assert.IsFalse(cell.isCenter);
            Assert.IsFalse(cell.isShort);
            Assert.IsTrue(cell.isGolden);

            cell.isGolden = false;

            Assert.IsFalse(cell.isCap);
            Assert.IsFalse(cell.isCenter);
            Assert.IsFalse(cell.isShort);
            Assert.IsFalse(cell.isGolden);

            Assert.AreEqual(cell.direction, Direction.DOWN);

            cell.direction = Direction.UP;
            Assert.AreEqual(cell.direction, Direction.UP);

            cell.direction = Direction.LEFT;
            Assert.AreEqual(cell.direction, Direction.LEFT);

            cell.direction = Direction.RIGHT;
            Assert.AreEqual(cell.direction, Direction.RIGHT);
        }
    }
}
