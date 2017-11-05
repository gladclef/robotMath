using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Util;
using robotMath.Expression;

namespace robotMath.LinearAlgebra
{
    public class Matrix : IPrettyPrint
    {
        internal readonly Node[,] Values;

        public int Rows => Values.GetLength(0);
        public int Cols => Values.GetLength(1);
        public Node this[int a, int b] => Values[a, b];

        /// <summary>
        /// Create a new matrix with the given Values.
        /// </summary>
        /// <param name="values">a Rows x Cols 2D array</param>
        /// <exception cref="ArgumentNullException">If Values is null</exception>
        public Matrix(Node[,] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values), "array must not be null");
            }
            if (values.GetLength(0) == 0 || values.GetLength(1) == 0)
            {
                throw new ArgumentException("array dimensions must be greater than 0", nameof(values));
            }

            this.Values = (Node[,])values.Clone();
        }

        public Matrix(Matrix values) : this(values.Values)
        {
        }

        public Node Get(int row, int col)
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

            Node[,] newValues = new Node[this.Rows, other.Cols];
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
            Parser p = this[0, 0].Tree;
            Node[,] newValues = new Node[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    newValues[i, j] = p.m(scalar) * this[i, j];
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

            Node[,] newValues = new Node[Rows, Cols];
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
            Node[,] newValues = new Node[Cols, Rows];
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
        internal static Node multiplyAndSum(Matrix a, Matrix b, int rowIndex, int columnIndex)
        {
            Parser p = a[0, 0].Tree;
            if (p != b[0,0].Tree)
            {
                throw new Parser.IncompatibleParserException(p, b[0, 0].Tree);
            }

            Node retval = null;
            for (int i = 0; i < a.Cols; i++)
            {
                // val = l * r
                Node l = a[rowIndex, i];
                Node r = b[i, columnIndex];
                Node val;
                if (l.Tag == NodeTag.literal && Math.Abs(((Leaf)l).Value) < 0.000001)
                {
                    val = l;
                }
                else if (r.Tag == NodeTag.literal && Math.Abs(((Leaf)r).Value) < 0.000001)
                {
                    val = r;
                }
                else
                {
                    val = a[rowIndex, i] * b[i, columnIndex];
                }

                // retval += val
                if (retval == null)
                {
                    retval = val;
                }
                else if (val.Tag == NodeTag.literal && Math.Abs(((Leaf)val).Value) < 0.000001)
                {
                    continue;
                }
                else
                {
                    retval += val;
                }
            }

            if (retval == null)
            {
                retval = p.m(0d);
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

            Node[,] values = new Node[rowEnd - rowStart, colEnd - colStart];
            for (int row = rowStart; row < rowEnd; row++)
            {
                for (int col = colStart; col < colEnd; col++)
                {
                    values[row - rowStart, col - colStart] = this[row, col];
                }
            }
            return new Matrix(values);
        }

        /// <summary>
        /// Simplifies all contained expressions that don't contain an unset variable.
        /// </summary>
        /// 
        /// <returns>A new matrix.</returns>
        public Matrix Simplify()
        {
            Node[,] values = new Node[Rows, Cols];

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    values[row, col] = this[row, col].Simplify();
                }
            }

            return new Matrix(values);
        }

        public static Matrix Identity(Parser p, int size)
        {
            Node[,] values = new Node[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        values[i, i] = p.m(1d);
                    }
                    else
                    {
                        values[i, i] = p.m(0d);
                    }
                }
            }
            return new Matrix(values);
        }

        public static Node[,] genNodes(double[,] vals)
        {
            Parser p = SillyParser.GetInstance();
            Node[,] retval = new Node[vals.GetLength(0), vals.GetLength(1)];

            for (int row = 0; row < vals.GetLength(0); row++)
            {
                for (int col = 0; col < vals.GetLength(1); col++)
                {
                    retval[row, col] = p.m(vals[row, col]);
                }
            }

            return retval;
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
                    try
                    {
                        if (!this[row, column].Equals(other[row, column]))
                        {
                            return false;
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        throw new NullReferenceException($"Null value at index [{row}, {column}]'!", e);
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
                    valStrs[row, column] = this[row, column].Unparse();
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
                    retval.Append(' ', columnLengths[column] - valStrs[row, column].Length);

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
