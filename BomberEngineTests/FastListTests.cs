using BomberEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    [TestClass]
    public class FastListTests
    {
        [TestMethod]
        public void TestAddFirst()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.AddFirstItem(node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestAddLast()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.AddLastItem(node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestAdd1()
        {
            FastList<Node> list = new FastList<Node>();

            Node node1 = new Node(1);
            list.AddLastItem(node1);

            Node node2 = new Node(2);
            list.AddLastItem(node2);

            Assert.AreEqual(list.size, 2);

            Assert.AreEqual(list.listFirst, node1);
            Assert.AreEqual(list.listLast, node2);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.AreEqual(list.listFirst.m_listNext, node2);

            Assert.AreEqual(list.listLast.m_listPrev, node1);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestAdd2()
        {
            FastList<Node> list = new FastList<Node>();

            Node node1 = new Node(1);
            list.AddFirstItem(node1);

            Node node2 = new Node(2);
            list.AddFirstItem(node2);

            Assert.AreEqual(list.size, 2);

            Assert.AreEqual(list.listFirst, node2);
            Assert.AreEqual(list.listLast, node1);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.AreEqual(list.listFirst.m_listNext, node1);

            Assert.AreEqual(list.listLast.m_listPrev, node2);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestAdd3()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Assert.AreEqual(list.size, 3);

            Node node = list.listFirst;
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(values[i], node.value);
                node = node.listNext;
            }

            Assert.IsNull(node);
        }

        [TestMethod]
        public void TestAdd4()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddFirstItem(new Node(values[i]));
            }

            Assert.AreEqual(list.size, 3);

            Node node = list.listLast;
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(values[i], node.value);
                node = node.listPrev;
            }

            Assert.IsNull(node);
        }

        [TestMethod]
        public void TestInsert1()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.InsertBeforeItem(list.listFirst, node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestInsert2()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.InsertAfterItem(list.listFirst, node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestInsert3()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.InsertBeforeItem(list.listLast, node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestInsert4()
        {
            FastList<Node> list = new FastList<Node>();

            Node node = new Node(1);
            list.InsertAfterItem(list.listLast, node);

            Assert.AreEqual(list.size, 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.m_listPrev);
            Assert.IsNull(list.listFirst.m_listNext);

            Assert.IsNull(list.listLast.m_listPrev);
            Assert.IsNull(list.listLast.m_listNext);
        }

        [TestMethod]
        public void TestInsert5()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = new Node(4);
            list.InsertBeforeItem(list.listFirst, node);

            AssertValues(list, 4, 1, 2, 3);
        }

        [TestMethod]
        public void TestInsert6()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = new Node(4);
            list.InsertAfterItem(list.listFirst, node);

            AssertValues(list, 1, 4, 2, 3);
        }

        [TestMethod]
        public void TestInsert7()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = new Node(4);
            list.InsertAfterItem(list.listFirst, node);

            AssertValues(list, 1, 4, 2, 3);
        }

        [TestMethod]
        public void TestInsert8()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node insertNode = Find(list, 2);

            Node node = new Node(4);
            list.InsertBeforeItem(insertNode, node);

            AssertValues(list, 1, 4, 2, 3);
        }

        [TestMethod]
        public void TestInsert9()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node insertNode = Find(list, 2);

            Node node = new Node(4);
            list.InsertAfterItem(insertNode, node);

            AssertValues(list, 1, 2, 4, 3);
        }

        [TestMethod]
        public void TestInsert10()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node insertNode = Find(list, 3);

            Node node = new Node(4);
            list.InsertBeforeItem(insertNode, node);

            AssertValues(list, 1, 2, 4, 3);
        }

        [TestMethod]
        public void TestInsert11()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node insertNode = Find(list, 3);

            Node node = new Node(4);
            list.InsertAfterItem(insertNode, node);

            AssertValues(list, 1, 2, 3, 4);
        }

        [TestMethod]
        public void TestInsert12()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = new Node(4);
            list.InsertBeforeItem(list.listLast, node);

            AssertValues(list, 1, 2, 4, 3);
        }

        [TestMethod]
        public void TestInsert13()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = new Node(4);
            list.InsertAfterItem(list.listLast, node);

            AssertValues(list, 1, 2, 3, 4);
        }

        [TestMethod]
        public void TestRemove1()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 1);
            list.RemoveItem(node);

            AssertValues(list, 2, 3);
        }

        [TestMethod]
        public void TestRemove2()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 2);
            list.RemoveItem(node);

            AssertValues(list, 1, 3);
        }

        [TestMethod]
        public void TestRemove3()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 1);
            list.RemoveItem(node);

            AssertValues(list, 2, 3);

            node = Find(list, 2);
            list.RemoveItem(node);

            AssertValues(list, 3);

            node = Find(list, 3);
            list.RemoveItem(node);

            AssertValues(list);
        }

        [TestMethod]
        public void TestRemove4()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 3);
            list.RemoveItem(node);

            AssertValues(list, 1, 2);
        }

        [TestMethod]
        public void TestRemove5()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            list.RemoveFirstItem();

            AssertValues(list, 2, 3);
        }

        [TestMethod]
        public void TestRemove6()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            list.RemoveFirstItem();

            AssertValues(list, 2, 3);
        }

        [TestMethod]
        public void TestRemove7()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            list.RemoveLastItem();

            AssertValues(list, 1, 2);
        }

        [TestMethod]
        public void TestRemove8()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            list.RemoveFirstItem();
            list.RemoveFirstItem();
            list.RemoveFirstItem();

            AssertValues(list);
        }

        [TestMethod]
        public void TestRemove9()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 2);
            list.RemoveItem(node);

            Assert.IsNull(node.m_listPrev);
            Assert.IsNull(node.m_listNext);
        }

        [TestMethod]
        public void TestMix1()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            Node node = Find(list, 2);
            list.InsertBeforeItem(node, new Node(4));
            list.RemoveItem(node);

            AssertValues(list, 1, 4, 3);
        }

        [TestMethod]
        public void TestMix2()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            list.RemoveFirstItem();
            list.RemoveLastItem();

            list.AddFirstItem(new Node(4));
            list.AddLastItem(new Node(5));

            AssertValues(list, 4, 2, 5);
        }

        [TestMethod]
        public void TestMix3()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            while (list.size > 0)
            {
                list.RemoveFirstItem();
            }

            AssertValues(list);
        }

        [TestMethod]
        public void TestMix4()
        {
            FastList<Node> list = new FastList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLastItem(new Node(values[i]));
            }

            while (list.size > 0)
            {
                list.RemoveLastItem();
            }

            AssertValues(list);
        }

        private void AssertValues(FastList<Node> list, params int[] values)
        {
            Assert.AreEqual(list.size, values.Length);

            Node node = list.listFirst;
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(node.value, values[i]);
                node = node.listNext;
            }

            Assert.IsNull(node);
        }

        private Node Find(FastList<Node> list, int value)
        {
            for (Node node = list.listFirst; node != null; node = node.listNext)
            {
                if (node.value == value)
                {
                    return node;
                }
            }

            return null;
        }
    }

    class Node : FastListNode
    {
        public int value;

        public Node(int value)
        {
            this.value = value;
        }

        public new Node listPrev
        {
            get { return (Node)base.listPrev; }
        }

        public new Node listNext
        {
            get { return (Node)base.listNext; }
        }
    }
}