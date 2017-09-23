using System;
using RobotMath.Robot.FrameUtil;
using RobotMath.LinearAlgebra;

namespace RobotMath.Robot.PositionMatrices
{
    public class TranslationVector : FrameTransformationPosition
    {
        public TranslationVector(Matrix values) : this(values.Values, null, null)
        {
        }

        public TranslationVector(FrameTransformationMatrix values) : this(values.Values, values.BaseFrame, values.ToFrame)
        {
        }

        public TranslationVector(Matrix values, Frame baseFrame, Frame toFrame) : this(values.Values, baseFrame, toFrame)
        {
        }

        public TranslationVector(double[,] values) : this(values, null, null)
        {
        }

        public TranslationVector(double[,] values, Frame baseFrame, Frame toFrame) : base(values, baseFrame, toFrame)
        {
        }

        public TranslationVector Sum(TranslationVector other)
        {
            Matrix newMatrix = base.Sum(other);
            return new TranslationVector(newMatrix.Values, BaseFrame, ToFrame);
        }

        public new TranslationVector Multiply(double scalar)
        {
            return new TranslationVector(base.Multiply(scalar), BaseFrame, ToFrame);
        }

        public new TranslationVector Transform()
        {
            return new TranslationVector(base.Transform().Values, BaseFrame, ToFrame);
        }

        public override string ToString()
        {
            return "Trans" + base.ToString();
        }

        public new string PrettyPrint()
        {
            return "Translation Vector " + base.PrettyPrint();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return (o is TranslationVector) &&
                   base.Equals(o);
        }

        public static TranslationVector operator +(TranslationVector left, TranslationVector right)
        {
            return left.Sum(right);
        }

        public static TranslationVector operator -(TranslationVector left, TranslationVector right)
        {
            return left.Sum(right * -1);
        }

        public static TranslationVector operator *(TranslationVector left, double right)
        {
            return left.Multiply(right);
        }

        public static TranslationVector operator *(double left, TranslationVector right)
        {
            return right.Multiply(left);
        }
    }
}
