using System;
using System.Data;
using System.Text;
using robotMath.Robot.FrameUtil;

namespace RobotMath.Robot.FrameUtil
{
    public interface IFrameTransformation
    {
        Frame BaseFrame { get; }
        Frame ToFrame { get; }
    }

    public class FrameTransformationHelper : IFrameTransformation
    {
        public Frame BaseFrame { get; }
        public Frame ToFrame { get; }

        public FrameTransformationHelper(Frame baseFrame, Frame toFrame)
        {
            BaseFrame = DetermineBaseFrame(baseFrame, toFrame);
            ToFrame = DetermineToFrame(baseFrame, toFrame);
        }

        public static Frame DetermineBaseFrame(Frame baseFrame, Frame toFrame)
        {
            return baseFrame ?? (toFrame == null ? null : Frame.ReferenceFrame);
        }

        public static Frame DetermineToFrame(Frame baseFrame, Frame toFrame)
        {
            return toFrame ?? (baseFrame == null ? null : Frame.ReferenceFrame);
        }

        public void CheckForValidTransformation(Frame baseFrame, Frame toFrame)
        {
            if (!IsValidTransformation(baseFrame, toFrame))
            {
                throw new InvalidOperationException("Invalid frame transformation from base frame \"" +
                                                    baseFrame + "\" to frame \"" + toFrame + "\"");
            }
        }

        public bool IsValidTransformation(IFrameTransformation obj)
        {
            return IsValidTransformation(obj.BaseFrame, obj.ToFrame);
        }

        public bool IsValidTransformation(Frame baseFrame, Frame toFrame)
        {
            if (baseFrame == toFrame && baseFrame != null)
            {
                return false;
            }
            return true;
        }

        public int GetHashCode(IFrameTransformation transformation)
        {
            return transformation.BaseFrame.GetHashCode()
                   ^ transformation.ToFrame.GetHashCode();
        }

        internal void ToString(StringBuilder retval)
        {
            if (BaseFrame != null || ToFrame != null)
            {
                retval.Append("<");
                retval.Append(BaseFrame);
                retval.Append(",");
                retval.Append(ToFrame);
                retval.Append(">:");
            }
            else
            {
                retval.Append("<>");
            }
        }

        public void PrettyPrint(StringBuilder retval)
        {
            if (BaseFrame != Frame.ReferenceFrame || ToFrame != Frame.ReferenceFrame)
            {
                retval.Append("from Base Frame \"");
                retval.Append(BaseFrame);
                retval.Append("\" To Frame \"");
                retval.Append(ToFrame);
                retval.Append("\"\n");
            }
            else
            {
                retval.Append("<>");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IFrameTransformation))
            {
                return false;
            }
            IFrameTransformation other = (IFrameTransformation) obj;
            return ((BaseFrame == other.BaseFrame || BaseFrame.Equals(other.BaseFrame)) &&
                (ToFrame == other.ToFrame     || ToFrame.Equals(other.ToFrame)));
        }

        public static void FrameEqualityCheck(Frame a, Frame b)
        {
            if (a != b && !a.Equals(b))
            {
                throw new InvalidExpressionException(
                    "frames must match but are instead \"" + a + "\" and \"" + b + "\"");
            }
        }

        public void FrameEqualityCheck(IFrameTransformation other)
        {
            if (!this.Equals(other))
            {
                throw new ArgumentException("base and to frames must match but are \"" +
                    BaseFrame + "\"/\"" + other.BaseFrame + "\", \"" + ToFrame + "\"/\"" + other.ToFrame + "\"", nameof(other));
            }
        }

        /// <summary>
        /// Verifies the given nextFrame transformation can be applied against this transformation
        /// in order to create a valid new transformation.
        /// </summary>
        /// <param name="nextFrame"></param>
        public void FrameTransitionCheck(IFrameTransformation nextFrame)
        {
            if (!ToFrame.Equals(nextFrame.BaseFrame))
            if (!ToFrame.Equals(nextFrame.BaseFrame))
            {
                throw new ArgumentException("the to frame of this frame must the same as the base of the frame transformation, but are " +
                    ToFrame + "\" and \"" + nextFrame.BaseFrame + "\"", nameof(nextFrame));
            }
        }
    }
}