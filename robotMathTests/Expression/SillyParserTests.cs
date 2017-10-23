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
            Assert.AreEqual(expected, p.interpretMatrix("1, 2; 3, 4"));
        }

        [TestMethod()]
        public void interpretNodeTest()
        {
            SillyParser p = new SillyParser(null);
            Assert.AreEqual(p.m(1d), p.interpretNode("1"));
            Assert.AreEqual(p.m("a"), p.interpretNode("a"));
            Assert.AreEqual(p.m("a"), p.interpretNode("(a)"));
            Assert.AreEqual(p.m("sin", 1d), p.interpretNode("sin(1)"));
        }

        [TestMethod()]
        public void interpretListOfNodesTest()
        {
            SillyParser p = new SillyParser(null);
            Assert.AreEqual(p.m("atan2", 1d, 2d), p.interpretNode("atan2(1, 2)"));
            Assert.AreEqual(p.m("atan2", 
                p.m("atan2", 1d, 2d),
                p.m("atan2", 3d, 4d)), p.interpretNode("atan2(atan2(1, 2), atan2(3, 4))"));
        }
    }
}