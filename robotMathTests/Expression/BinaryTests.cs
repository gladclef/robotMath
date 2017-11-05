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
    public class BinaryTests
    {
        [TestMethod()]
        public void Simplify_NegatedExpression_NegativeMovedInside()
        {
            SillyParser p = new SillyParser(null);
            Node n = p.m("*", p.m("a"), p.m("b"));
            Assert.AreEqual("(a * b)", n.Unparse());

            n = n.Negate();
            Assert.AreEqual("-(a * b)", n.Unparse());

            Node nSimp = n.Simplify();
            Assert.AreEqual("(-a * -b)", nSimp.Unparse());
        }

        [TestMethod()]
        public void Simplify_NegatedSubtraction_NegativeMovedInside()
        {
            SillyParser p = new SillyParser(null);
            Node n = p.m("-", p.m("a"), p.m("b"));
            Assert.AreEqual("(a - b)", n.Unparse());

            n = n.Negate();
            Assert.AreEqual("-(a - b)", n.Unparse());

            Node nSimp = n.Simplify();
            Assert.AreEqual("(b - a)", nSimp.Unparse());
        }
    }
}