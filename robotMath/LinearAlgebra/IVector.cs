﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMath.LinearAlgebra
{
    public interface IVector
    {
        double this[int a] { get; }
    }

    public class VectorHelper : IVector
    {
        Matrix Parent { get; }
        public double this[int a] => (Parent.Rows == 1 ? Parent[0, a] : Parent[a, 0]);

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
