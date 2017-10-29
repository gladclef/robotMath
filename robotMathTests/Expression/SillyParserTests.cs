using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;

namespace robotMath.Expression.Tests
{
    [TestClass()]
    public class SillyParserTests
    {
        [TestMethod()]
        public void interpretMatrixTest()
        {
            SillyParser p = new SillyParser(null);
            Node[,] values = new Node[2, 2];
            values[0, 0] = p.m(1);
            values[0, 1] = p.m(2);
            values[1, 0] = p.m(3);
            values[1, 1] = p.m(4);
            Matrix expected = new Matrix(values);
            Assert.AreEqual(expected, p.InterpretMatrix("1, 2; 3, 4"));
        }

        [TestMethod()]
        public void interpretNodeTest_simple()
        {
            SillyParser p = new SillyParser(null);
            Assert.AreEqual(p.m(1d), p.InterpretNode("1"));
            Assert.AreEqual(p.m("a"), p.InterpretNode("a"));
            Assert.AreEqual(p.m("a"), p.InterpretNode("(a)"));
            Assert.AreEqual(p.m("sin", 1d), p.InterpretNode("sin(1)"));
        }

        [TestMethod()]
        public void interpretNodeTest_multiplyTwoDoubles()
        {
            SillyParser p = new SillyParser(null);
            Node actual = p.InterpretNode("2*-1");
            Assert.AreEqual(p.m("*", 2d, -1d), actual);
        }

        [TestMethod()]
        public void interpretNodeTest_addNegOne()
        {
            SillyParser p = new SillyParser(null);
            Node actual = p.InterpretNode("2+-1");
            Assert.AreEqual(p.m("+", 2d, -1d), actual);
        }

        [TestMethod()]
        public void interpretNodeTest_multiplyTwoExpressions()
        {
            SillyParser p = new SillyParser(null);
            Node mul = p.m("*", p.m(2d), p.m(1d));
            Node atan2 = p.m("atan2", p.m(3d), p.m(4d));
            Node add = p.m("+", mul, p.m(5d));
            Assert.AreEqual(mul, p.InterpretNode("*(2,1)"));
            Assert.AreEqual(atan2, p.InterpretNode("atan2(3,4)"));
            Assert.AreEqual(add, p.InterpretNode("5+atan2(3,4)"));
            Assert.AreEqual(p.m("+", add, atan2), p.InterpretNode("*(2,1)+5+atan2(3,4)"));
        }

        [TestMethod()]
        public void interpretListOfNodesTest()
        {
            SillyParser p = new SillyParser(null);
            Assert.AreEqual(p.m("atan2", 1d, 2d), p.InterpretNode("atan2(1, 2)"));
            Node inner1 = p.m("atan2", 1d, 2d);
            Node inner2 = p.m("atan2", 3d, 4d);
            Assert.AreEqual(inner1, p.InterpretNode("atan2(1, 2)"));
            Assert.AreEqual(inner2, p.InterpretNode("atan2(3, 4)"));
            Assert.AreEqual(p.m("atan2", inner1, inner2), p.InterpretNode("atan2(atan2(1, 2), atan2(3, 4))"));
        }

        [TestMethod()]
        public void pemdas()
        {
            SillyParser p = new SillyParser(null);
            Node atan2 = p.m("atan2", 5d, 6d);
            Node minParen = p.m("-", -1d, 2d);
            Node sin = p.m("sin", 3d);
            Node mid = p.m("/", p.m("*", sin, minParen), atan2);
            Node firstHalf = p.m("+", 2d, mid);
            Node end1 = p.m("+", firstHalf, 2d);
            Node end2 = p.m("-", end1, p.m("*", 5d, 7d));
            Node whole = p.m("+", end2, 3d);
            Assert.AreEqual(whole, p.InterpretNode("2+sin(3)*(-1-2)/atan2(5,6)+2-5*7+3"));
        }

        [TestMethod()]
        public void interpretMatrix()
        {
            double pi = Math.PI;
            double thpi = 1.5 * pi; // three halves pi
            double ohpi = 0.5 * pi; // one halves pi
            SillyParser p = new SillyParser(null);
            Matrix matrix = p.InterpretMatrix(
                $"0,  0,         d1,  th1;" +
                $"0,  1.5*{pi},  d2,  1.5*{pi};" +
                $"0,  0,         d3,  0.5*{pi}");
            matrix = matrix.Simplify();
            Assert.AreEqual(p.m(0), matrix[0, 0]);
            Assert.AreEqual(p.m(0), matrix[0, 1]);
            Assert.AreEqual(p.m("d1"), matrix[0, 2]);
            Assert.AreEqual(p.m("th1"), matrix[0, 3]);
            Assert.AreEqual(p.m(0), matrix[1, 0]);
            Assert.AreEqual(p.m(thpi), matrix[1, 1]);
            Assert.AreEqual(p.m("d2"), matrix[1, 2]);
            Assert.AreEqual(p.m(thpi), matrix[1, 3]);
            Assert.AreEqual(p.m(0), matrix[2, 0]);
            Assert.AreEqual(p.m(0), matrix[2, 1]);
            Assert.AreEqual(p.m("d3"), matrix[2, 2]);
            Assert.AreEqual(p.m(ohpi), matrix[2, 3]);
        }
    }
}