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
            Node actual = p.InterpretNode("2*1");
            Assert.AreEqual(p.m("*", p.m(2d), p.m(1d)), actual);
        }

        [TestMethod()]
        public void interpretNodeTest_multiplyTwoExpressions()
        {
            SillyParser p = new SillyParser(null);
            Node mul = p.m("*", p.m(2d), p.m(1d));
            Node atan2 = p.m("atan2", p.m(3d), p.m(4d));
            Node add = p.m("+", p.m(5d), atan2);
            Assert.AreEqual(mul, p.InterpretNode("*(2,1)"));
            Assert.AreEqual(atan2, p.InterpretNode("atan2(3,4)"));
            Assert.AreEqual(add, p.InterpretNode("5+atan2(3,4)"));
            Assert.AreEqual(p.m("+", mul, add), p.InterpretNode("*(2,1)+5+atan2(3,4)"));
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
    }
}