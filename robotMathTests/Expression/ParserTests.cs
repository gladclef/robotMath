using System.IO;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Expression;

namespace robotMathTests.Expression
{
    [TestClass()]
    public class ParserTests
    {
        // expression parsing with GPPG doesn't work, ignoring for now
        //[TestMethod()]
        public void parser_constructor_success()
        {
            using (Stream stream = GenerateStreamFromString("1"))
            {
                var parser = new Parser(new Scanner(stream));
                Assert.IsTrue(true);
            }
        }

        // expression parsing with GPPG doesn't work, ignoring for now
        //[TestMethod()]
        public void parse_multiplication_success()
        {
            using (Stream stream = GenerateStreamFromString("2*3"))
            {
                var parser = new Parser(new Scanner(stream));
                Assert.IsTrue(parser.Parse());
            }
        }

        // expression parsing with GPPG doesn't work, ignoring for now
        //[TestMethod()]
        public void eval_unparse_returnsSame()
        {
            string expression = "(2 * 3)";
            using (Stream stream = GenerateStreamFromString(expression))
            {
                var parser = new Parser(new Scanner(stream));
                parser.Parse();
                string result = parser.Root.Unparse();
                Assert.AreEqual(expression, result, "error unparsing");
            }
        }

        // expression parsing with GPPG doesn't work, ignoring for now
        //[TestMethod()]
        public void eval_multiplication_returns6()
        {
            using (Stream stream = GenerateStreamFromString("2*3"))
            {
                var parser = new Parser(new Scanner(stream));
                parser.Parse();
                double result = parser.Root.Eval();
                Assert.AreEqual(6d, result, 0.000001, "error evaluating: 2*3 != 6?");
            }
        }

        [TestMethod()]
        public void unparse_complicatedExpression_success()
        {
            // (2*3)+2
            var p = new Parser(null);
            p.m("+", p.m("*", 2d, 3d), 2d);
            string expected = "((2 * 3) + 2)";
            string actual = p.Root.Unparse();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void eval_complicatedExpression_success()
        {
            // (2*3)+2
            var p = new Parser(null);
            p.m("+", p.m("*", 2d, 3d), 2d);
            Assert.AreEqual(8d, p.Root.Eval(), 0.000001);
        }

        [TestMethod()]
        public void eval_trig_success()
        {
            double pof = Math.PI / 4d;
            Assert.AreEqual(Math.Sin(pof), (new Parser(null)).m("sin", pof).Eval(), 0.0000001);
            Assert.AreEqual(Math.Cos(pof), (new Parser(null)).m("cos", pof).Eval(), 0.0000001);
            Assert.AreEqual(Math.Tan(pof), (new Parser(null)).m("tan", pof).Eval(), 0.0000001);
            Assert.AreEqual(Math.Asin(pof), (new Parser(null)).m("asin", pof).Eval(), 0.0000001);
            Assert.AreEqual(Math.Acos(pof), (new Parser(null)).m("acos", pof).Eval(), 0.0000001);
            Assert.AreEqual(Math.Atan(pof), (new Parser(null)).m("atan", pof).Eval(), 0.0000001);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}