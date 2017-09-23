using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotMath.Util;
using RobotMath.LinearAlgebra;
using RobotMath.PositionMatrices;
using RobotMath.Robot.FrameUtil;

namespace RobotMath.Robot.FrameUtil
{
    public abstract class FrameTransformationPosition : Position, IFrameTransformation, PrettyPrintInterface
    {
        public readonly FrameTransformationHelper fth;
        public Frame BaseFrame => fth.BaseFrame;
        public Frame ToFrame => fth.ToFrame;

        protected FrameTransformationPosition(double[,] values, Frame baseFrame, Frame toFrame) : base(values)
        {
            fth = new FrameTransformationHelper(baseFrame, toFrame);
        }

        protected FrameTransformationPosition(Matrix values, Frame baseFrame, Frame toFrame) : base(values.Values)
        {
            fth = new FrameTransformationHelper(baseFrame, toFrame);
        }

        protected FrameTransformationPosition(FrameTransformationMatrix values) : this(values, values.BaseFrame, values.ToFrame)
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
