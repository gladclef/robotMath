using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;
using robotMath.Expression;

namespace robotMath.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void MainTest()
        {
            SillyParser p = new SillyParser(null);
            Matrix A_0_1 = p.InterpretMatrix(
                "cos(t1),-sin(t1), 0, 0;" +
                "sin(t1),cos(t1),  0, 0;" +
                "0,      0,        1, d1;" +
                "0,      0,        0, 1");
            Matrix A_1_2 = p.InterpretMatrix(
                "0,  0,  1, 0;" +
                "-1, 0,  0, 0;" +
                "0,  -1, 0, d2;" +
                "0,  0,  0, 1");
            Matrix A_2_3 = p.InterpretMatrix(
                "0, -1, 0, 0;" +
                "1, 0,  0, 0;" +
                "0, 0,  1, d3;" +
                "0, 0,  0, 1");
            Matrix A_1_3 = p.InterpretMatrix(
                "0,  0, 1, d3;" +
                "0,  1, 0, 0;" +
                "-1, 0, 0, d2;" +
                "0,  0, 0, 1");
            Matrix A_0_3 = p.InterpretMatrix(
                "0,  -sin(t1), cos(t1), *(d3,cos(t1));" +
                "0,  cos(t1),  sin(t1), *(d3,sin(t1));" +
                "-1, 0,        0,       +(d1,d2);" +
                "0,  0,        0,       1");

            Assert.AreEqual(A_1_3.PrettyPrint(), A_1_2.DotProduct(A_2_3).Simplify().PrettyPrint());
            Assert.AreEqual(A_0_3, A_0_1.DotProduct(A_1_3).Simplify());
        }
    }
}