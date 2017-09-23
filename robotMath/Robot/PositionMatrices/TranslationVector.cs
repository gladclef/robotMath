using System;
using robotMath.Robot.FrameUtil;
using RobotMath.linearAlgebra;
using RobotMath.Robot.FrameUtil;

namespace robotMath.Robot.PositionMatrices
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
            if ((values.GetLength(0) != 1 && values.GetLength(1) != 1) ||
                (values.GetLength(0) != 3 && values.GetLength(1) != 3))
            {
                throw new ArgumentOutOfRangeException(nameof(values), "must be a 1x3 or 3x1 matrix");
            }
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
