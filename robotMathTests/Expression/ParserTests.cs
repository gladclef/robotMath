using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Expression;

namespace robotMathTests.Expression
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void parser_constructor_success()
        {
            using (Stream stream = GenerateStreamFromString("1"))
            {
                var parser = new Parser(new Scanner(stream));
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void parse_multiplication_success()
        {
            using (Stream stream = GenerateStreamFromString("2*3"))
            {
                var parser = new Parser(new Scanner(stream));
                Assert.IsTrue(parser.Parse());
            }
        }

        [TestMethod()]
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

        [TestMethod()]
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