using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotMath.Robot.FrameUtil
{
    class FrameRegistry
    {
        protected static Dictionary<string, Frame> frames = new Dictionary<string, Frame>();

        public static Frame GetFrame(string name)
        {
            if (!frames.ContainsKey(name))
            {
                frames.Add(name, new Frame(name));
            }
            return frames[name];
        }
    }
}
