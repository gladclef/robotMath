using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;
using QUT.Gppg;

namespace robotMath.Expression
{
    public class SillyParser : Parser
    {
        public SillyParser(AbstractScanner<Object, LexLocation> scanner) : base (scanner)
        {
        }
        
        public static Parser GetInstance()
        {
            if (singleton == null)
            {
                singleton = new Parser(null);
            }
            return singleton;
        }

        public Matrix interpretMatrix(string matrixStr)
        {
            string[] rows = matrixStr.Split(';');
            Node[,] values = null;
            Parser p = SillyParser.GetInstance();
            int colLength = 0;

            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                try
                {
                    Node[] rowVals = interpretListOfNodes(rows[rowIndex]);
                    int newColLength = rowVals.Length;
                    if (values == null)
                    {
                        values = new Node[rows.Length, rowVals.Length];
                        colLength = values.GetLength(1);
                    }
                    else if (colLength != newColLength)
                    {
                        throw new FormatException($"Expected '{colLength}' columns on every row, but found '{newColLength}' columns in '{rows[rowIndex]}'");
                    }

                    for (int colIndex = 0; colIndex < colLength; colIndex++)
                    {
                        values[rowIndex, colIndex] = rowVals[colIndex];
                    }
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Error at row index '{rowIndex}'!", e);
                }
            }

            return new Matrix(values);
        }

        public Node interpretNode(string val)
        {
            val = val.Trim();

            // interpret as a double
            try
            {
                double d = Convert.ToDouble(val);
                return m(d);
            }
            catch (FormatException) { }

            // check for negation
            if (val[0] == '-')
            {
                return m("-", interpretNode(val.Substring(1)));
            }

            // interpret as variable
            if (!val.Contains('('))
            {
                return m(val);
            }

            // it must be a parenthetical
            string[] parts = val.Split(new char[] { '(' }, 2);
            if (parts.Length != 2)
            {
                throw new FormatException($"Expected binary or unary expression but instead found '{val}'!");
            }
            char lastChar = parts[1][parts[1].Length - 1];
            if (lastChar != ')')
            {
                throw new FormatException($"Expected ')' but instead found '{lastChar}' in '{val}'!");
            }
            string outside = parts[0];
            string inside = parts[1].Substring(0, parts[1].Length - 1);
            if (inside.Length == 0)
            {
                throw new FormatException($"Expected value inside parenthesis but instead found '{val}'!");
            }

            // interpret as parethesized expression
            if (outside.Length == 0)
            {
                return interpretNode(inside);
            }

            // interpret as binary or unary expression
            Node[] innerNodes = interpretListOfNodes(inside);
            if (innerNodes.Length == 1)
            {
                return m(outside, innerNodes[0]);
            }
            else if (innerNodes.Length == 2)
            {
                return m(outside, innerNodes[0], innerNodes[1]);
            }
            else
            {
                throw new FormatException($"Expected expression containing at most two arguments, but instead found '{inside}', which contains {innerNodes.Length} arguments!");
            }
        }

        public Node[] interpretListOfNodes(string val)
        {
            int parenCount = 0;
            List<Node> retval = new List<Node>();

            string next = "";
            bool closedExpr = false;
            for (int i = 0; i < val.Length; i++)
            {
                char curr = val[i];
                switch (curr)
                {
                    case '(':
                        next += curr;
                        parenCount++;
                        break;
                    case ')':
                        next += curr;
                        parenCount--;
                        closedExpr = true;
                        break;
                    case ' ':
                    case '\t':
                        // do nothing
                        break;
                    case ',':
                        if (parenCount == 0)
                        {
                            closedExpr = false;
                            retval.Add(interpretNode(next));
                            next = "";
                        }
                        else
                        {
                            next += curr;
                        }
                        break;
                    default:
                        if (closedExpr)
                        {
                            throw new FormatException($"Expected ',' after the last parentheses of an expression is closed, but instead found '{curr}' at index '{i}' of '{val}'!");
                        }
                        next += curr;
                        break;
                }
            }

            // catch any straggling values
            next = next.Trim();
            retval.Add(interpretNode(next));

            return retval.ToArray();
        }
    }
}
