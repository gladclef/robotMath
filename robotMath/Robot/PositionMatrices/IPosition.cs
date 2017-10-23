using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;
using robotMath.Expression;

namespace robotMath.Robot.PositionMatrices
{
    public interface IPosition : IVector
    {
        Node X { get; }
        Node Y { get; }
        Node Z { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the dimensions of this vector are not 1x3 or 3x1</exception>
        void CheckValidDimensions(int rows, int cols);
    }
}
