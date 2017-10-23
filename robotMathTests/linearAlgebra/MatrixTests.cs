using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.LinearAlgebra;
using robotMath.Expression;

namespace robotMathTests.LinearAlgebra
{
    [TestClass()]
    public class MatrixTests
    {
        public static Node[,] genNodes(double[,] vals)
        {
            return Matrix.genNodes(vals);
        }

        [TestMethod()]
        public void dotProductTest1_success()
        {
            Matrix a = new Matrix(genNodes(new double[,] { { 1, 2 } }));
            Matrix b = new Matrix(genNodes(new double[,] { { 1 }, { 2 } }));
            Matrix c = a.DotProduct(b).Simplify();
            Matrix expected = new Matrix(genNodes(new double[,] { { 5 } }));
            Assert.AreEqual(expected, c);
        }

        [TestMethod()]
        public void dotProductTest2_success()
        {
            Matrix a = new Matrix(genNodes(new double[,] { { 1, 2 }, { 2, 3 }, { 3, 4 } }));
            Matrix b = new Matrix(genNodes(new double[,] { { 1, 2 }, { 3, 4 } }));
            Matrix c = a.DotProduct(b).Simplify();
            Matrix expected = new Matrix(genNodes(new double[,] { { 1 + 6, 2 + 8 }, { 2 + 9, 4 + 12 }, { 3 + 12, 6 + 16 } }));
            Assert.AreEqual(c, expected);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void dotProductTest_null_exception()
        {
            Matrix a = new Matrix(genNodes(new double[,] { { 1 } }));
            a.DotProduct(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void dotProductTest_badDimensions1_exception()
        {
            Matrix a = new Matrix(genNodes(new double[,] { { 1 } }));
            Matrix b = new Matrix(genNodes(new double[,] { { 1 }, { 2 } }));
            a.DotProduct(b);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void dotProductTest_badDimensions2_exception()
        {
            Matrix a = new Matrix(genNodes(new double[,] { { 1 }, { 2 }, { 3 } }));
            Matrix b = new Matrix(genNodes(new double[,] { { 1, 2 }, { 3, 4 } }));
            a.DotProduct(b);
        }
    }
}