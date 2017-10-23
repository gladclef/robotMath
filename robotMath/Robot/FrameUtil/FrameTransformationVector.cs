using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Util;
using robotMath.LinearAlgebra;
using robotMath.Robot.PositionMatrices;
using robotMath.Expression;

namespace robotMath.Robot.FrameUtil
{
    public class FrameTransformationVector : FrameTransformationMatrix, IVector
    {
        public VectorHelper Helper { get; }
        public Node this[int a] => Helper[a];

        public FrameTransformationVector(Node[,] values, Frame baseFrame, Frame toFrame) : base(values, baseFrame, toFrame)
        {
            Helper = new VectorHelper(this);
        }

        public FrameTransformationVector(Matrix values, Frame baseFrame, Frame toFrame) : this(values.Values, null, null)
        {
        }

        public FrameTransformationVector(FrameTransformationVector values) : this(values, values.BaseFrame, values.ToFrame)
        {
        }

        public FrameTransformationVector DotProduct(FrameTransformationVector other)
        {
            return new FrameTransformationVector(base.DotProduct(other), BaseFrame, other.ToFrame);
        }

        public FrameTransformationVector Sum(FrameTransformationVector other)
        {
            return new FrameTransformationVector(base.Sum(other), BaseFrame, other.ToFrame);
        }
    }
}
