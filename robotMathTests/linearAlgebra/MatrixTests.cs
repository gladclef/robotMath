using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMath.LinearAlgebra
{
    [TestClass()]
    public class MatrixTests
    {
        [TestMethod()]
        public void dotProductTest1_success()
        {
            Matrix a = new Matrix(new double[,] { { 1, 2 } });
            Matrix b = new Matrix(new double[,] { { 1 }, { 2 } });
            Matrix c = a.DotProduct(b);
            Matrix expected = new Matrix(new double[,] { { 5 } });
            expected.Equals(c);
            Assert.AreEqual(expected, c);
        }

        [TestMethod()]
        public void dotProductTest2_success()
        {
            Matrix a = new Matrix(new double[,] { { 1, 2 }, { 2, 3 }, { 3, 4 } });
            Matrix b = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
            Matrix c = a.DotProduct(b);
            Matrix expected = new Matrix(new double[,] { { 1+6, 2+8 }, { 2+9, 4+12 }, { 3+12, 6+16 } });
            Assert.AreEqual(c, expected);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void dotProductTest_null_exception()
        {
            Matrix a = new Matrix(new double[,] { { 1 } });
            a.DotProduct(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void dotProductTest_badDimensions1_exception()
        {
            Matrix a = new Matrix(new double[,] { { 1 } });
            Matrix b = new Matrix(new double[,] { { 1 }, { 2 } });
            a.DotProduct(b);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void dotProductTest_badDimensions2_exception()
        {
            Matrix a = new Matrix(new double[,] { { 1 }, { 2 }, { 3 } });
            Matrix b = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
            a.DotProduct(b);
        }
    }
}