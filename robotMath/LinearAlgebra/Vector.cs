using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMath.LinearAlgebra
{
    public class Vector : Matrix
    {
        public Vector(double[,] values) : base(values)
        {
            if (values.GetLength(0) != 1 && values.GetLength(1) != 1)
            {
                throw new ArgumentException("array must be either a 1xN or Nx1 array", nameof(values));
            }
        }
    }
}
