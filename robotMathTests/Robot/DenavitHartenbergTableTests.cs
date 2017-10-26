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
        public void IntermediateHomogeneousTransformationsTest()
        {
            double pi = Math.PI;
            SillyParser p = (SillyParser) SillyParser.GetInstance();
            DenavitHartenbergTable table = new DenavitHartenbergTable(p.InterpretMatrix(
                $"0,  0,            d1,  th1;"         +
                $"0,  *(1.5,{pi}),  d2,  *(1.5,{pi});" +
                $"0,  0,            d3,  *(0.5,{pi})" ));

            Matrix A_0_1 = p.InterpretMatrix(
                "cos(th1), -sin(th1), 0, 0;" +
                "sin(th1), cos(th1),  0, 0;" +
                "0,        0,         1, d1;" +
                "0,        0,         0, 1");
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