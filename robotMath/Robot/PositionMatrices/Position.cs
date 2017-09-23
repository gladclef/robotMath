using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotMath.LinearAlgebra;

namespace RobotMath.Robot.PositionMatrices
{
    public class Position : Vector
    {
        /// <summary>
        /// A 3x1 matrix with the values 0, 0, 0
        /// </summary>
        public static Position Origin = new Position(new double[,] { { 0 }, { 0 }, { 0 } });

        public Position(double[,] values) : base(values)
        {
            if (values.GetLength(0) != 3 && values.GetLength(1) != 3)
            {
                throw new ArgumentException("position must be specified by a 3 length vector", nameof(values));
            }
        }

        public double X => Values[0, 0];
        public double Y => (Rows == 3 ? Values[1, 0] : Values[0, 1]);
        public double Z => (Rows == 3 ? Values[2, 0] : Values[0, 2]);
    }
}
