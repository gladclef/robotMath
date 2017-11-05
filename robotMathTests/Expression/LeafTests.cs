using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotMath.Expression.Tests
{
    [TestClass()]
    public class LeafTests
    {
        [TestMethod()]
        public void Eval_PositiveLiteral_success()
        {
            SillyParser p = new SillyParser(null);
            Node n = p.m(2);
            Assert.AreEqual(2, n.Eval());
        }

        [TestMethod()]
        public void Eval_NegateLiteral_ReturnsNegativeValue()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf) p.m(-2);
            Assert.AreEqual(-2, n.Eval());
            Assert.AreEqual(2, n.Value);
            Assert.IsTrue(n.IsNegative);
        }

        [TestMethod()]
        public void Simplify_NegateLiteral_ReturnsNegativeValue()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m(-2).Simplify();
            Assert.AreEqual(-2, n.Eval());
        }

        [TestMethod()]
        public void Simplify_NamedVariable_ReturnsName()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m("a").Simplify();
            Assert.AreEqual("a", n.Unparse());
        }

        [TestMethod()]
        [ExpectedException(typeof(Parser.NoSetValueException))]
        public void Eval_NamedVariable_ThrowsException()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m("a");
            n.Eval();
        }

        [TestMethod()]
        public void Unparse_NegatedNamedVariable_ReturnsNegatedName()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m("a").Negate();
            Assert.AreEqual("-a", n.Unparse());
        }

        [TestMethod()]
        public void Simplify_NamedVariableWithVal_ReturnsVal()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m("a");
            p.SetVar("a", 2);
            Assert.AreEqual(2, n.Eval());
        }

        [TestMethod()]
        public void SetValueTwice_NamedVariable_ReturnsSecondVal()
        {
            SillyParser p = new SillyParser(null);
            Leaf n = (Leaf)p.m("a");
            p.SetVar("a", 2);
            Assert.AreEqual(2, n.Eval());
            p.SetVar("a", 3);
            Assert.AreEqual(3, n.Eval());
        }
    }
}