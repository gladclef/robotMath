using System;
using System.Collections.Generic;

namespace robotMath.Robot.FrameUtil
{
    public class Frame
    {
        public static Frame ReferenceFrame = new Frame("reference");

        protected String Name;
        protected HashSet<Frame> _PrevFrames = new HashSet<Frame>();
        protected HashSet<Frame> _NextFrames = new HashSet<Frame>();

        public HashSet<Frame> PrevFrames => new HashSet<Frame>(_PrevFrames);
        public HashSet<Frame> NextFrames => new HashSet<Frame>(_NextFrames);

        public Frame(string name) : this(name, null, null)
        {
        }

        public Frame(string name, HashSet<Frame> prev, HashSet<Frame> next)
        {
            this.Name = name;
            if (prev != null)
            {
                this._PrevFrames = prev;
            }
            if (next != null)
            {
                this._NextFrames = next;
            }
        }

        public void AddPrevFrame(Frame prevFrame)
        {
            if (this == ReferenceFrame)
            {
                throw new InvalidOperationException("Can't assign a previous frame to the \"" + this + "\" frame!");
            }
            _PrevFrames.Add(prevFrame);
        }

        public void AddNextFrame(Frame nextFrame)
        {
            _NextFrames.Add(nextFrame);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
