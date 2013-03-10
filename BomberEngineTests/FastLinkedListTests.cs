using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BomberEngine.Util;

namespace BomberEngineTests
{
    [TestClass]
    public class FastLinkedListTests
    {
        [TestMethod]
        public void TestAddFirst()
        {
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            Node node = new Node(1);
            list.AddFirst(node);

            Assert.AreEqual(list.GetListSize(), 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.listPrev);
            Assert.IsNull(list.listFirst.listNext);

            Assert.IsNull(list.listLast.listPrev);
            Assert.IsNull(list.listLast.listNext);
        }

        [TestMethod]
        public void TestAddLast()
        {
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            Node node = new Node(1);
            list.AddLast(node);

            Assert.AreEqual(list.GetListSize(), 1);

            Assert.AreEqual(list.listFirst, node);
            Assert.AreEqual(list.listLast, node);

            Assert.IsNull(list.listFirst.listPrev);
            Assert.IsNull(list.listFirst.listNext);

            Assert.IsNull(list.listLast.listPrev);
            Assert.IsNull(list.listLast.listNext);
        }

        [TestMethod]
        public void TestAdd1()
        {
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            Node node1 = new Node(1);
            list.AddLast(node1);

            Node node2 = new Node(2);
            list.AddLast(node2);

            Assert.AreEqual(list.GetListSize(), 2);

            Assert.AreEqual(list.listFirst, node1);
            Assert.AreEqual(list.listLast, node2);

            Assert.IsNull(list.listFirst.listPrev);
            Assert.AreEqual(list.listFirst.listNext, node2);

            Assert.AreEqual(list.listLast.listPrev, node1);
            Assert.IsNull(list.listLast.listNext);
        }

        [TestMethod]
        public void TestAdd2()
        {
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            Node node1 = new Node(1);
            list.AddFirst(node1);

            Node node2 = new Node(2);
            list.AddFirst(node2);

            Assert.AreEqual(list.GetListSize(), 2);

            Assert.AreEqual(list.listFirst, node2);
            Assert.AreEqual(list.listLast, node1);

            Assert.IsNull(list.listFirst.listPrev);
            Assert.AreEqual(list.listFirst.listNext, node1);

            Assert.AreEqual(list.listLast.listPrev, node2);
            Assert.IsNull(list.listLast.listNext);
        }

        [TestMethod]
        public void TestAdd3()
        {
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddLast(new Node(values[i]));
            }

            Assert.AreEqual(list.GetListSize(), 3);

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
            FastLinkedList<Node> list = new FastLinkedList<Node>();

            int[] values = { 1, 2, 3 };

            for (int i = 0; i < values.Length; ++i)
            {
                list.AddFirst(new Node(values[i]));
            }

            Assert.AreEqual(list.GetListSize(), 3);

            Node node = list.listLast;
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(values[i], node.value);
                node = node.listPrev;
            }

            Assert.IsNull(node);
        }

    }

    /*
      [TestMethod]
        public void TestAdd()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            Node node = root;
            for (int i = values.Length - 1; i >= 0; --i)
            {
                Assert.IsNotNull(node);
                Assert.AreEqual(node.value, values[i]);
                node = node.listNext;
            }

            Assert.IsNull(node);
        }

        [TestMethod]
        public void TestFind()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            for (int i = 0; i < values.Length; ++i)
            {
                int value = values[i];
                Assert.AreEqual(value, Find(root, value).value);
            }

            Assert.IsNull(Find(root, 4));
        }

        [TestMethod]
        public void TestRemove1()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            Node node = Find(root, 1);
            root = ListUtils.Remove(root, node);

            ArraysEqual(GetValues(root), 3, 2);
        }

        [TestMethod]
        public void TestRemove2()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            Node node = Find(root, 2);
            root = ListUtils.Remove(root, node);

            ArraysEqual(GetValues(root), 3, 1);
        }

        [TestMethod]
        public void TestRemove3()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            Node node = Find(root, 3);
            root = ListUtils.Remove(root, node);

            ArraysEqual(GetValues(root), 2, 1);
        }

        [TestMethod]
        public void TestRemove4()
        {
            int[] values = { 1, 2, 3 };

            Node root = null;
            for (int i = 0; i < values.Length; ++i)
            {
                root = ListUtils.Add(root, new Node(values[i]));
            }

            Node node = Find(root, 1);
            root = ListUtils.Remove(root, node);

            node = Find(root, 2);
            root = ListUtils.Remove(root, node);

            node = Find(root, 3);
            root = ListUtils.Remove(root, node);

            Assert.IsNull(root);
        }

        private Node Find(Node root, int value)
        {
            for (Node node = root; node != null; node = node.listNext)
            {
                if (node.value == value)
                {
                    return node;
                }
            }

            return null;
        }

        private int[] GetValues(Node root)
        {
            int count = 0;
            for (Node node = root; node != null; node = node.listNext)
            {
                ++count;
            }

            int[] values = new int[count];
            int index = 0;

            for (Node node = root; node != null; node = node.listNext)
            {
                values[index++] = node.value;
            }

            return values;
        }

        private void ArraysEqual(int[] a, params int[] b)
        {
            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; ++i)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }
    }

    public class Node : ListNode<Node>
    {
        public int value;

        public Node(int value)
        {
            this.value = value;
        }
    }   
     */

    class Node : ListNode<Node>
    {
        public int value;

        public Node(int value)
        {
            this.value = value;
        }
    }
}
