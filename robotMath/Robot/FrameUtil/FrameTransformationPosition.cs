using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotMath.Util;
using RobotMath.LinearAlgebra;
using RobotMath.Robot.PositionMatrices;

namespace RobotMath.Robot.FrameUtil
{
    public class FrameTransformationPosition : FrameTransformationVector, IPosition
    {
        public double X => this[0];
        public double Y => this[1];
        public double Z => this[2];

        public FrameTransformationPosition(double[,] values, Frame baseFrame, Frame toFrame) : base(values, baseFrame, toFrame)
        {
            CheckValidDimensions(values.GetLength(0), values.GetLength(1));
        }

        public FrameTransformationPosition(Matrix values, Frame baseFrame, Frame toFrame) : this(values.Values, null, null)
        {
        }

        public FrameTransformationPosition(FrameTransformationPosition values) : this(values, values.BaseFrame, values.ToFrame)
        {
        }

        public FrameTransformationPosition DotProduct(FrameTransformationPosition other)
        {
            return new FrameTransformationPosition(base.DotProduct(other), BaseFrame, other.ToFrame);
        }

        public FrameTransformationPosition Sum(FrameTransformationPosition other)
        {
            return new FrameTransformationPosition(base.Sum(other), BaseFrame, other.ToFrame);
        }

        public void CheckValidDimensions(int rows, int cols)
        {
            if (rows != 3 && cols != 3)
            {
                throw new ArgumentOutOfRangeException("values", "the dimensions of this vector must be either 1x3 or 3x1");
            }
        }
    }
}
