﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotMath.LinearAlgebra;
using RobotMath.Robot.PositionMatrices;

namespace robotMathTests.Robot.PositionMatrices
{
    [TestClass()]
    public class RotationMatrixTests
    {
        [TestMethod()]
        public void RotationMatrixTest_createByDimensionsConstructor()
        {
            double ang = Math.PI / 2;
            RotationMatrix a = new RotationMatrix(CartesianDimension.Y, ang);
            Matrix expected = new Matrix(new double[,]
            {
                { Math.Cos(ang), 0, Math.Sin(ang)},
                { 0, 1, 0 },
                { -Math.Sin(ang), 0, Math.Cos(ang)},
            });
            Assert.AreEqual(expected, a);
        }
    }
}