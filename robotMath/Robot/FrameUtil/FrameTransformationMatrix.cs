using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Util;
using RobotMath.linearAlgebra;
using RobotMath.Robot.FrameUtil;

namespace robotMath.Robot.FrameUtil
{
    public class FrameTransformationMatrix : Matrix, IFrameTransformation, PrettyPrintInterface
    {
        private readonly FrameTransformationHelper fth;
        public Frame BaseFrame => fth.BaseFrame;
        public Frame ToFrame => fth.ToFrame;

        public FrameTransformationMatrix(double[,] values, Frame baseFrame, Frame toFrame) : base(values)
        {
            fth = new FrameTransformationHelper(baseFrame, toFrame);
        }

        public FrameTransformationMatrix(Matrix values, Frame baseFrame, Frame toFrame) : base(values.Values)
        {
            fth = new FrameTransformationHelper(baseFrame, toFrame);
        }

        public FrameTransformationMatrix(FrameTransformationMatrix values) : this(values, values.BaseFrame, values.ToFrame)
        {
        }

        public void FrameEqualityCheck(IFrameTransformation other)
        {
            fth.FrameEqualityCheck(other);
        }

        public void FrameTransitionCheck(IFrameTransformation other)
        {
            fth.FrameTransitionCheck(other);
        }

        public FrameTransformationMatrix DotProduct(FrameTransformationMatrix other)
        {
            FrameTransitionCheck(other);
            return new FrameTransformationMatrix(base.DotProduct(other), BaseFrame, other.ToFrame);
        }

        public FrameTransformationMatrix Sum(FrameTransformationMatrix other)
        {
            FrameTransitionCheck(other);
            return new FrameTransformationMatrix(base.Sum(other), BaseFrame, other.ToFrame);
        }

        public override string ToString()
        {
            StringBuilder retval = new StringBuilder();
            fth.ToString(retval);
            retval.Append(base.ToString());
            return retval.ToString();
        }

        public new string PrettyPrint()
        {
            StringBuilder retval = new StringBuilder();
            fth.PrettyPrint(retval);
            retval.Append(base.ToString());
            return retval.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode()
                   ^ fth.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return fth.Equals((IFrameTransformation)o) &&
                   base.Equals(o);
        }
    }
}
