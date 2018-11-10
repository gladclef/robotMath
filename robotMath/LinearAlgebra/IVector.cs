using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Expression;

namespace robotMath.LinearAlgebra
{
    /// <summary>
    /// The basic vector class interface.
    /// It is an interface instead of a class because it should be implemented by
    /// a <see cref="Matrix"/> (or something similar).
    /// 
    /// <seealso cref="VectorHelper"/>
    /// </summary>
    public interface IVector
    {
        /// <summary>Get the value at the given index of the vector.</summary>
        Node this[int a] { get; }
    }

    /// <summary>
    /// Class to wrap a <see cref="Matrix"/> to look like a <see cref="IVector"/>.
    /// </summary>
    public class VectorHelper : IVector
    {
        Matrix Parent { get; }
        public Node this[int a] => (Parent.Rows == 1 ? Parent[0, a] : Parent[a, 0]);

        public VectorHelper(Matrix parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent matrix of VectorHelper can't be null!", nameof(parent));
            }
            if (parent.Rows != 1 && parent.Cols != 1)
            {
                throw new ArgumentException("array must be either a 1xN or Nx1 array");
            }

            this.Parent = parent;
        }
    }
}
