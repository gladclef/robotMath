﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotMath.Util;

namespace RobotMath.LinearAlgebra
{
    public class Matrix : PrettyPrintInterface
    {
        internal readonly double[,] Values;

        public int Rows => Values.GetLength(0);
        public int Cols => Values.GetLength(1);
        public double this[int a, int b] => Values[a, b];

        /// <summary>
        /// Create a new matrix with the given Values.
        /// </summary>
        /// <param name="values">a Rows x Cols 2D array</param>
        /// <exception cref="ArgumentNullException">If Values is null</exception>
        public Matrix(double[,] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values), "array must not be null");
            }
            if (values.GetLength(0) == 0 || values.GetLength(1) == 0)
            {
                throw new ArgumentException("array dimensions must be greater than 0", nameof(values));
            }

            this.Values = (double[,])values.Clone();
        }

        public double Get(int row, int col)
        {
            return this[row, col];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException">If the other matrix is null</exception>
        /// <exception cref="ArgumentException">If the size of the two matrices is incorrect</exception>
        protected void DotProductViabilityChecks(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other), "matrix must not be null");
            }
            if (this.Cols != other.Rows)
            {
                throw new ArgumentException("first matrix column count must be equal to second matrix row count");
            }
        }

        /// <exception cref="ArgumentNullException">If the other matrix is null</exception>
        /// <exception cref="ArgumentException">If the size of the two matrices is incorrect</exception>
        public Matrix DotProduct(Matrix other)
        {
            DotProductViabilityChecks(other);

            double[,] newValues = new double[this.Rows, other.Cols];
            for (int row = 0; row < this.Rows; row++)
            {
                for (int column = 0; column < other.Cols; column++)
                {
                    newValues[row, column] = multiplyAndSum(this, other, row, column);
                }
            }
            return new Matrix(newValues);
        }

        public Matrix Multiply(double scalar)
        {
            double[,] newValues = new double[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    newValues[i, j] = scalar * this[i, j];
                }
            }
            return new Matrix(newValues);
        }

        public Matrix Sum(Matrix other)
        {
            if (other.Rows != Rows || other.Cols != Cols)
            {
                throw new ArgumentOutOfRangeException("must have the same number of rows and columns as this matrix", nameof(other));
            }

            double[,] newValues = new double[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    newValues[i, j] = this[i, j] + other[i, j];
                }
            }
            return new Matrix(newValues);
        }

        public Matrix Transform()
        {
            double[,] newValues = new double[Cols, Rows];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    newValues[j, i] = this[i, j];
                }
            }
            return new Matrix(newValues);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="rowIndex">The row index in the resulting matrix</param>
        /// <param name="columnIndex">The column index in the resulting matrix</param>
        /// <returns></returns>
        internal static double multiplyAndSum(Matrix a, Matrix b, int rowIndex, int columnIndex)
        {
            double retval = 0;
            for (int i = 0; i < a.Cols; i++)
            {
                retval += a[rowIndex, i] * b[i, columnIndex];
            }
            return retval;
        }

        /// <summary>
        /// Create a new matrix from a part of this matrix.
        /// </summary>
        /// <param name="rowStart">inclusive</param>
        /// <param name="rowEnd">exclusive</param>
        /// <param name="colStart">inclusive</param>
        /// <param name="colEnd">exclusive</param>
        /// <returns></returns>
        public Matrix SubMatrix(int rowStart, int rowEnd, int colStart, int colEnd)
        {
            if (rowStart < 0 || rowStart >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(rowStart));
            }
            if (colStart < 0 || colStart >= Cols)
            {
                throw new ArgumentOutOfRangeException(nameof(colStart));
            }
            if (rowEnd < rowStart || rowEnd > Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(rowEnd));
            }
            if (colEnd < colStart || colEnd > Cols)
            {
                throw new ArgumentOutOfRangeException(nameof(colEnd));
            }

            double[,] values = new double[rowEnd - rowStart, colEnd - colStart];
            for (int row = rowStart; row < rowEnd; row++)
            {
                for (int col = colStart; col < colEnd; col++)
                {
                    values[row - rowStart, col - colStart] = this[row, col];
                }
            }
            return new Matrix(values);
        }

        public static Matrix Identity(int size)
        {
            double[,] values = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                values[i, i] = 1;
            }
            return new Matrix(values);
        }

        public override bool Equals(object o)
        {
            if (!(o is Matrix))
            {
                return false;
            }

            Matrix other = (Matrix) o;
            if (other.Rows != Rows || other.Cols != Cols)
            {
                return false;
            }
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Cols; column++)
                {
                    if (Math.Abs(this[row, column] - other[row, column]) > 0.000001)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder retval = new StringBuilder("[  ");
            for (int row = 0; row < Rows; row++)
            {
                if (row != 0)
                {
                    retval.Append(" ; ");
                }
                for (int column = 0; column < Cols; column++)
                {
                    if (column != 0)
                    {
                        retval.Append(", ");
                    }
                    retval.Append(this[row, column]);
                }
            }
            retval.Append("  ]");
            return retval.ToString();
        }

        public string PrettyPrint()
        {
            String[,] valStrs = new string[Rows, Cols];
            int[] columnLengths = new int[Cols];
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Cols; column++)
                {
                    valStrs[row, column] = this[row, column].ToString("F2");
                    columnLengths[column] = Math.Max(columnLengths[column], 3);
                    columnLengths[column] = Math.Max(columnLengths[column], valStrs[row, column].Length);
                }
            }

            StringBuilder retval = new StringBuilder();
            String columnIndexRow = "";
            if (Cols > 7)
            {
                StringBuilder columnIndexBuilder = new StringBuilder();
                int spaces = 4;
                for (int column = 0; column < Cols; column++)
                {
                    if (column % 5 == 0)
                    {
                        String columnStr = column.ToString();
                        columnIndexBuilder.Append(' ', spaces);
                        columnIndexBuilder.Append(columnStr);
                        columnIndexBuilder.Append(' ', columnLengths[column] - columnStr.Length);
                        spaces = 1;
                    }
                    else
                    {
                        spaces += columnLengths[column] + 1;
                    }
                }
                columnIndexRow = columnIndexBuilder.ToString();
                retval.Append(columnIndexRow);
                retval.Append("\n");
            }
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Cols; column++)
                {
                    if (column == 0)
                    {
                        if (row == 0)
                        {
                            if (Rows == 1)
                            {
                                retval.Append("[   ");
                            }
                            else
                            {
                                retval.Append("/   ");
                            }
                        }
                        else if (row == Rows - 1)
                        {
                            retval.Append("\\   ");
                        }
                        else if (row % 5 == 0)
                        {
                            String rowStr = row.ToString();
                            retval.Append(rowStr);
                            retval.Append(' ', 4 - rowStr.Length);
                        }
                        else
                        {
                            retval.Append("|   ");
                        }
                    }

                    retval.Append(' ');
                    retval.Append(valStrs[row, column]);
                    retval.Append(' ');

                    if (column == Cols - 1)
                    {
                        if (row == 0)
                        {
                            if (Rows == 1)
                            {
                                retval.Append("   ]");
                            }
                            else
                            {
                                retval.Append("   \\");
                            }
                        }
                        else if (row == Rows - 1)
                        {
                            retval.Append("   /");
                        }
                        else if (row % 5 == 0)
                        {
                            String rowStr = row.ToString();
                            retval.Append(' ', 4 - rowStr.Length);
                            retval.Append(rowStr);
                        }
                        else
                        {
                            retval.Append("   |");
                        }
                    }
                }

                if (row < Rows - 1)
                {
                    retval.Append("\n");
                }
            }
            if (Cols > 7)
            {
                retval.Append("\n");
                retval.Append(columnIndexRow);
            }

            return retval.ToString();
        }

        public override int GetHashCode()
        {
            return Rows ^ Cols;
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            return left.Sum(right);
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            return left.Sum(right * -1);
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            return left.DotProduct(right);
        }

        public static Matrix operator *(Matrix left, double right)
        {
            return left.Multiply(right);
        }

        public static Matrix operator *(double left, Matrix right)
        {
            return right.Multiply(left);
        }
    }
}
