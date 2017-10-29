using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Expression;
using robotMath.LinearAlgebra;
using robotMath.Robot.PositionMatrices;

namespace robotMath.Robot.Tests
{
    [TestClass()]
    public class DenavitHartenbergTableTests
    {
        [TestMethod()]
        public void DotProductTest()
        {
            SillyParser p = new SillyParser(null);
            Matrix A_0_1 = p.InterpretMatrix(
                "cos(t1), -sin(t1), 0, 0;" +
                "sin(t1), cos(t1),  0, 0;" +
                "0,       0,        1, d1;" +
                "0,       0,        0, 1");
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
                "0,  -sin(t1), cos(t1), d3 * cos(t1);" +
                "0,  cos(t1),  sin(t1), d3 * sin(t1);" +
                "-1, 0,        0,       d1 + d2;" +
                "0,  0,        0,       1");

            Assert.AreEqual(A_1_3.PrettyPrint(), A_1_2.DotProduct(A_2_3).Simplify().PrettyPrint());
            Matrix dot2 = A_0_1.DotProduct(A_1_3).Simplify();
            Assert.AreEqual(A_0_3, dot2, $"Expected\n{A_0_3.PrettyPrint()}\nbut was\n{dot2.PrettyPrint()}");
        }

        [TestMethod()]
        public void DHTableMatrixTest()
        {
            double pi = Math.PI;
            double thpi = 1.5 * pi; // three halves pi
            double zhpi = 0.5 * pi; // zero halves pi
            SillyParser p = (SillyParser)SillyParser.GetInstance();
            Matrix matrix = p.InterpretMatrix(
                $"0,  0,         d1,  th1;" +
                $"0,  1.5*{pi},  d2,  1.5*{pi};" +
                $"0,  0,         d3,  0.5*{pi}");
            matrix = matrix.Simplify();

            Assert.AreEqual(p.m(0),     matrix[0, 0]);
            Assert.AreEqual(p.m(0),     matrix[0, 1]);
            Assert.AreEqual(p.m("d1"),  matrix[0, 2]);
            Assert.AreEqual(p.m("th1"), matrix[0, 3]);
            Assert.AreEqual(p.m(0),     matrix[1, 0]);
            Assert.AreEqual(p.m(thpi),  matrix[1, 1]);
            Assert.AreEqual(p.m("d2"),  matrix[1, 2]);
            Assert.AreEqual(p.m(thpi),  matrix[1, 3]);
            Assert.AreEqual(p.m(0),     matrix[2, 0]);
            Assert.AreEqual(p.m(0),     matrix[2, 1]);
            Assert.AreEqual(p.m("d3"),  matrix[2, 2]);
            Assert.AreEqual(p.m(zhpi),  matrix[2, 3]);
        }

        [TestMethod()]
        public void IntermediateHomogeneousTransformationsTest()
        {
            double pi = Math.PI;
            SillyParser p = (SillyParser) SillyParser.GetInstance();
            DenavitHartenbergTable table = new DenavitHartenbergTable(p.InterpretMatrix(
                $"0,  0,         d1,  th1;"      +
                $"0,  1.5*{pi},  d2,  1.5*{pi};" +
                $"0,  0,         d3,  0.5*{pi}"  ));

            Matrix A_0_1 = p.InterpretMatrix(
                "cos(th1), -sin(th1), 0, 0;"  +
                "sin(th1), cos(th1),  0, 0;"  +
                "0,        0,         1, d1;" +
                "0,        0,         0, 1"   );
            Matrix A_1_2 = p.InterpretMatrix(
                "0,  0,  1, 0;"  +
                "-1, 0,  0, 0;"  +
                "0,  -1, 0, d2;" +
                "0,  0,  0, 1"   );
            Matrix A_2_3 = p.InterpretMatrix(
                "0, -1, 0, 0;"  +
                "1, 0,  0, 0;"  +
                "0, 0,  1, d3;" +
                "0, 0,  0, 1"   );

            HomogeneousTransformation[] HTs = table.IntermediateHomogeneousTransformations();
            Matrix[] HTMs = new Matrix[HTs.Length];
            for (int i = 0; i < HTs.Length; i++)
            {
                HTMs[i] = new Matrix(HTs[i].SubMatrix(0, 4, 0, 4));
                HTMs[i] = HTMs[i].Simplify();
            }
            Assert.AreEqual(A_0_1, HTMs[0], $"Expected \n{A_0_1.PrettyPrint()}\nbut was\n{HTMs[0].PrettyPrint()}");
            Assert.AreEqual(A_1_2, HTMs[1], $"Expected \n{A_1_2.PrettyPrint()}\nbut was\n{HTMs[1].PrettyPrint()}");
            Assert.AreEqual(A_2_3, HTMs[2], $"Expected \n{A_2_3.PrettyPrint()}\nbut was\n{HTMs[2].PrettyPrint()}");
        }
    }
}