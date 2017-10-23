using System;
using System.Text;
using robotMath.Robot.FrameUtil;
using robotMath.LinearAlgebra;
using robotMath.Expression;

namespace robotMath.Robot.PositionMatrices
{
    public class RotationMatrix : FrameTransformationMatrix
    {
        public RotationMatrix(Node[,] values) : this(values, null, null)
        {
        }

        public RotationMatrix(Node[,] values, Frame baseFrame, Frame toFrame) : base(values, baseFrame, toFrame)
        {
            if (values.GetLength(0) != 3 || values.GetLength(1) != 3)
            {
                throw new ArgumentException("array must be a 3x3 matrix, is a " + 
                    values.GetLength(0) + "x" + values.GetLength(1) + " matrix", nameof(values));
            }
        }

        public RotationMatrix(Matrix sourceMatrix) : this(sourceMatrix.Values, null, null)
        {
        }

        public RotationMatrix(Matrix sourceMatrix, Frame baseFrame, Frame toFrame) : this(sourceMatrix.Values, baseFrame, toFrame)
        {
        }

        public RotationMatrix(FrameTransformationMatrix values) : this(values.Values, values.BaseFrame, values.ToFrame)
        {
        }

        public RotationMatrix(CartesianDimension dimension, double radians) : this(dimension, radians, null, null)
        {
        }

        public RotationMatrix(CartesianDimension dimension, double radians, Frame baseFrame, Frame toFrame) : base(
            BuildRotationMatrix(dimension, radians), baseFrame, toFrame)
        {
            if (radians.Equals(double.NaN) || radians.Equals(double.NegativeInfinity) ||
                radians.Equals(double.PositiveInfinity))
            {
                throw new ArgumentException("must be a valid double", nameof(radians));
            }
        }

        public RotationMatrix DotProduct(RotationMatrix other)
        {
            Matrix result = base.DotProduct(other);
            return new RotationMatrix(result.Values);
        }

        public new RotationMatrix Simplify()
        {
            return new RotationMatrix(base.Simplify(), BaseFrame, ToFrame);
        }

        internal static Node[,] BuildRotationMatrix(CartesianDimension dimension, double radians)
        {
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            switch (dimension)
            {
                case CartesianDimension.X:
                    return genNodes(new double[,]
                    {
                        {1, 0, 0},
                        {0, cos, -sin},
                        {0, sin, cos}
                    });
                case CartesianDimension.Y:
                    return genNodes(new double[,]
                    {
                        {cos, 0, sin},
                        {0, 1, 0},
                        {-sin, 0, cos}
                    });
                case CartesianDimension.Z:
                    return genNodes(new double[,]
                    {
                        {cos, -sin, 0},
                        {sin, cos, 0},
                        {0, 0, 1}
                    });
                default:
                    return null;
            }
        }

        public RotationMatrix Sum(RotationMatrix other)
        {
            Matrix newMatrix = base.Sum(other);
            return new RotationMatrix(newMatrix.Values, BaseFrame, ToFrame);
        }

        public new RotationMatrix Multiply(double scalar)
        {
            return new RotationMatrix(base.Multiply(scalar), BaseFrame, ToFrame);
        }

        public new RotationMatrix Transform()
        {
            return new RotationMatrix(base.Transform().Values, BaseFrame, ToFrame);
        }

        public override string ToString()
        {
            return "Rot" + base.ToString();
        }

        public new string PrettyPrint()
        {
            return "Rotation Matrix " + base.PrettyPrint();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return (o is RotationMatrix) &&
                   base.Equals(o);
        }

        public static RotationMatrix operator +(RotationMatrix left, RotationMatrix right)
        {
            return left.Sum(right);
        }

        public static RotationMatrix operator -(RotationMatrix left, RotationMatrix right)
        {
            return left.Sum(right * -1);
        }

        public static RotationMatrix operator *(RotationMatrix left, RotationMatrix right)
        {
            return left.DotProduct(right);
        }

        public static RotationMatrix operator *(RotationMatrix left, double right)
        {
            return left.Multiply(right);
        }

        public static RotationMatrix operator *(double left, RotationMatrix right)
        {
            return right.Multiply(left);
        }
    }
}
