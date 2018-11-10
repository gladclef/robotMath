using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;
using robotMath.Expression;

namespace robotMath.Robot.PositionMatrices
{
    /// <summary>
    /// A triplet to hold XYZ coordinates.
    /// </summary>
    public interface IPosition : IVector
    {
        /// <summary>The X coordinate (index 0 of the source/backing vector).</summary>
        Node X { get; }
        /// <summary>The Y coordinate (index 1 of the source/backing vector).</summary>
        Node Y { get; }
        /// <summary>The Z coordinate (index 2 of the source/backing vector).</summary>
        Node Z { get; }

        /// <summary>
        /// Verify that the vector used to back this IPosition has valid dimensions.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the dimensions of this vector are not 1x3 or 3x1</exception>
        void CheckValidDimensions(int rows, int cols);
    }
}
