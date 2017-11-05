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
    public class UnaryTests
    {
        [TestMethod()]
        public void Simplify_NegativeNegations_success()
        {
            SillyParser p = new SillyParser(null);
            Unary node = (Unary) p.m("-", p.m("a"));
            Unary neg = (Unary) node.Negate();
            Leaf negSimp = (Leaf)neg.Simplify();

            Assert.AreEqual("--a", neg.Unparse());
            Assert.AreEqual("a", negSimp.Unparse());
            p.SetVar("a", 2);
            Assert.AreEqual(-2, node.Eval());
            Assert.AreEqual(2, neg.Eval());
            Assert.AreEqual(2, negSimp.Eval());
        }
    }
}