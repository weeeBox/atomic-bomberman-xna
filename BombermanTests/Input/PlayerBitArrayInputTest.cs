using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bomberman.Gameplay.Elements.Players;

namespace BombermanTests.Input
{
    [TestClass]
    public class PlayerBitArrayInputTest
    {
        [TestMethod]
        public void TestPressAndRelease()
        {
            PlayerBitArrayInput input = new DummyBitArrayInput();

            input.Update(0.016f);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
                Assert.IsFalse(input.IsActionPressed(i));
            }

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                input.actionsArray[i] = true;
            }

            input.Update(0.016f);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsTrue(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
                Assert.IsTrue(input.IsActionPressed(i));
            }

            input.Update(0.016f);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
                Assert.IsTrue(input.IsActionPressed(i));
            }

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                input.actionsArray[i] = false;
            }

            input.Update(0.016f);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsTrue(input.IsActionJustReleased(i));
                Assert.IsFalse(input.IsActionPressed(i));
            }

            input.Update(0.016f);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
                Assert.IsFalse(input.IsActionPressed(i));
            }

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                input.actionsArray[i] = true;
            }

            input.Update(0.016f);
            input.Reset();

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
                Assert.IsFalse(input.IsActionPressed(i));
            }
        }

        [TestMethod]
        public void TestPressedCount()
        {
            PlayerBitArrayInput input = new DummyBitArrayInput();

            int pressedCount = 0;
            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                input.actionsArray[i] = pressed;

                if (pressed) ++pressedCount;
            }

            input.Update(0.016f);
            Assert.AreEqual(pressedCount, input.GetPressedActionCount());

            input.Reset();
            Assert.AreEqual(0, input.GetPressedActionCount());
        }

        [TestMethod]
        public void TestMaskReset()
        {
            int mask = 0;
            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                if (pressed)
                {
                    mask |= 1 << i;
                }
            }

            PlayerBitArrayInput input = new DummyBitArrayInput();
            input.Update(0.016f);

            input.Reset(mask);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                Assert.AreEqual(pressed, input.IsActionPressed(i));
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
            }

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                input.actionsArray[i] = true;
            }

            input.Reset(mask);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                Assert.AreEqual(pressed, input.IsActionPressed(i));
                Assert.IsFalse(input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
            }
        }

        [TestMethod]
        public void TestForceMask()
        {
            int mask = 0;
            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                if (pressed)
                {
                    mask |= 1 << i;
                }
            }

            PlayerBitArrayInput input = new DummyBitArrayInput();
            input.Update(0.016f);

            input.Force(mask);

            for (int i = 0; i < (int)PlayerAction.Count; ++i)
            {
                bool pressed = i % 2 == 0;
                Assert.AreEqual(pressed, input.IsActionPressed(i));
                Assert.AreEqual(pressed, input.IsActionJustPressed(i));
                Assert.IsFalse(input.IsActionJustReleased(i));
            }
        }
    }

    public class DummyBitArrayInput : PlayerBitArrayInput
    {
        public override bool IsLocal
        {
            get { return true; }
        }
    }
}
