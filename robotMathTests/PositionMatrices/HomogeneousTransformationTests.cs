using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotMath.Robot.FrameUtil;
using robotMath.Robot.PositionMatrices;
using RobotMath.Orientation;
using RobotMath.positionMatrices;
using RobotMath.PositionMatrices;

namespace RobotMathTests.PositionMatrices
{
    [TestClass()]
    public class HomogeneousTransformationTests
    {
        [TestMethod()]
        public void TranslateByCurrentOrientationTest()
        {
            Frame a = new Frame("a");
            Frame b = new Frame("b");
            Frame c = new Frame("c");

            RotationMatrix rotationMatrix = new RotationMatrix(CartesianDimension.Z, Math.PI / 2, a, b);
            HomogeneousTransformation originalTransformation = new HomogeneousTransformation(rotationMatrix);
            TranslationVector translation = new TranslationVector(new double[,] { { 1, 0, 0 } }, b, c);
            HomogeneousTransformation result = originalTransformation.TranslateByCurrentOrientation(translation);

            TranslationVector expectedOrigin = new TranslationVector(new double[,] { { 0, 1, 0 } }, a, c).Transform();
            RotationMatrix expectedRotation = new RotationMatrix(rotationMatrix, a, c);
            expectedOrigin.Equals(result.TranslationVector);
            Assert.AreEqual(expectedOrigin, result.TranslationVector);
            Assert.AreEqual(new HomogeneousTransformation(expectedRotation, expectedOrigin), result);
        }
    }
}