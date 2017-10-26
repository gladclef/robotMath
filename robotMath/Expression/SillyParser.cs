using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using robotMath.LinearAlgebra;
using QUT.Gppg;

namespace robotMath.Expression
{
    public class SillyParser : Parser
    {
        public static Regex NameMatcher = new Regex("^[a-z_][a-z0-9_]*", RegexOptions.IgnoreCase);
        public static Regex ExprCharMatcher = new Regex("^[@!#$%^&*|/\\-+~]");
        public static Regex NumberMatcher = new Regex("^-?([0-9]*[,\\.][0-9]+|[0-9]+)");

        public SillyParser(AbstractScanner<Object, LexLocation> scanner) : base (scanner)
        {
        }
        
        public static Parser GetInstance()
        {
            if (singleton == null)
            {
                singleton = new SillyParser(null);
            }
            return singleton;
        }

        public Matrix InterpretMatrix(string matrixStr)
        {
            string[] rows = matrixStr.Split(';');
            Node[,] values = null;
            Parser p = SillyParser.GetInstance();
            int colLength = 0;

            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                try
                {
                    Node[] rowVals = InterpretListOfNodes(rows[rowIndex]);
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

        public Node InterpretNode(string inval)
        {
            string val = inval.Trim().Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
            while (val.Contains("  "))
            {
                val = val.Replace("  ", " ");
            }
            if (val.Length == 0 || val.Equals(" "))
            {
                throw new FormatException($"Expected expression, but instead just found \"{inval}\"!");
            }

            // interpret as a double
            try
            {
                double d = Convert.ToDouble(val);
                return m(d);
            }
            catch (FormatException) { }

            // check for negation
            if (val[0] == '-') // "-a"
            {
                return m("-", InterpretNode(val.Substring(1)));
            }

            // try to interpret as an operator
            if (!val.Contains('(')) // "a*b"
            {
                Node matchedNode = InterpretUnaryOrBinaryMatch(inval, val);
                if (matchedNode != null)
                {
                    return matchedNode;
                }

                // interpret as a variable
                return m(val);
            }

            // it must be a parenthetical
            string[] parts = val.Split(new char[] { '(' }, 2);
            if (parts.Length != 2)
            {
                throw new FormatException($"Expected binary or unary expression but instead found '{inval}'!");
            }
            string beforeParen = parts[0];
            string startingAtParen = val.Substring(beforeParen.Length);
            if (startingAtParen.Length == 0)
            {
                throw new FormatException($"Expected value inside parenthesis but instead found '{startingAtParen}'!");
            }
            string parenExpr = FindBallancedParens(startingAtParen);
            string insideParen = parenExpr.Substring(1, parenExpr.Length - 2);
            string afterParenExpr = startingAtParen.Substring(parenExpr.Length).Trim();

            // check for expressions pre-paren
            List<string> nvals = SplitOnRecognizedCharGroups(beforeParen, 2);
            if (nvals.Count > 1)
            {
                return InterpretBinaryMatch(inval, val);
            }

            // interpret as binary or unary expression
            Node[] innerNodes = InterpretListOfNodes(insideParen);
            Node result;
            if (beforeParen.Length == 0) // "(a)"
            {
                if (innerNodes.Length != 1)
                {
                    throw new InvalidOperationException($"Unexpected code reached, found '{innerNodes.Length}' expressions in '{insideParen}' when exactly 1 were expected!");
                }
                result = innerNodes[0];
            }
            else if (innerNodes.Length == 1) // "sin(a)"
            {
                result = m(beforeParen, innerNodes[0]);
            }
            else if (innerNodes.Length == 2) // "atan2(a, b)"
            {
                result = m(beforeParen, innerNodes[0], innerNodes[1]);
            }
            else
            {
                throw new FormatException($"Expected expression containing at most two arguments, but instead found '{insideParen}', which contains {innerNodes.Length} arguments!");
            }

            // check for any remaining text to be processes
            if (afterParenExpr.Length > 0) // "(a)*b"
            {
                try
                {
                    result = InterpretBinaryMatch(afterParenExpr, afterParenExpr, result);
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Unable to parse binary expression from '{afterParenExpr}' after parsing '{beforeParen+parenExpr}'!", e);
                }
            }
            return result;
        }

        private Node InterpretUnaryOrBinaryMatch(string inval, string val)
        {
            string partialInval = inval.Trim();
            List<string> nvals = SplitOnRecognizedCharGroups(val, 3);

            // try to interpret as single expression if only one match is found
            if (nvals.Count == 1)
            {
                return m(nvals[0]);
            }

            // prepare error string
            int partialLength = nvals[0].Length;
            partialLength = inval.IndexOf(nvals[1], partialLength) + nvals[1].Length;
            string errStr = inval.Substring(0, partialLength);
            if (nvals.Count > 2)
            {
                partialLength = partialInval.IndexOf(nvals[2], partialLength) + nvals[2].Length;
            }

            // try to interpret recognized character groups

            // interpret as binary expression?
            if (nvals.Count > 2)
            {
                return m(nvals[1], InterpretNode(nvals[0]), InterpretNode(nvals[2]));
            }

            // interpret as unary expression?
            try
            {
                string remaining = (nvals.Count > 2) ? nvals[1] + nvals[2] : nvals[1];
                return m(nvals[0], InterpretNode(remaining));
            }
            catch (Exception e)
            {
                if (e is BadOperatorException || e is FormatException)
                {
                    throw new FormatException(
                        $"Expected binary or unary expression but instead found '{partialInval.Substring(0, partialLength)}'!",
                        e);
                }
                throw e;
            }
        }

        private static List<string> SplitOnRecognizedCharGroups(string val, int maxMatches)
        {
            // split into recognized character groups
            List<string> nvals = new List<string>();
            nvals.Add(val.Replace(" ", ""));

            maxMatches = Math.Max(maxMatches, 2);
            for (int i = 0; i < nvals.Count && i < maxMatches - 1; i++)
            {
                string currVal = nvals[i];
                MatchCollection match;
                if ((match = NameMatcher.Matches(currVal)).Count > 0 ||
                    (match = ExprCharMatcher.Matches(currVal)).Count > 0 ||
                    (match = NumberMatcher.Matches(currVal)).Count > 0)
                {
                    string matchStr = match[0].Value;
                    nvals.RemoveAt(i);
                    nvals.Add(matchStr);
                    if (matchStr.Length < currVal.Length)
                    {
                        nvals.Add(currVal.Substring(matchStr.Length));
                    }
                }
            }

            return nvals;
        }

        private Node InterpretBinaryMatch(string inval, string val)
        {
            List<string> nvals = SplitOnRecognizedCharGroups(val, 1);
            string rest = val.Substring(nvals[0].Length);
            return InterpretBinaryMatch(rest, rest, InterpretNode(nvals[0]));
        }

        private Node InterpretBinaryMatch(string inval, string val, Node firstVal)
        {
            return InterpretBinaryMatch(inval, SplitOnRecognizedCharGroups(val, 2), firstVal);
        }

        private Node InterpretBinaryMatch(string inval, List<string> nvals, Node firstVal)
        {
            inval = inval.Trim();
            string errStr = firstVal.Unparse() + inval;

            // try to interpret recognized character groups
            if (nvals.Count > 1)
            {
                // interpret as binary expression?
                try
                {
                    return m(nvals[0], firstVal, InterpretNode(nvals[1]));
                }
                catch (Exception e)
                {
                    if (e is BadOperatorException || e is FormatException)
                    {
                        throw new FormatException(
                            $"Expected binary or unary expression but instead found '{errStr}'!",
                            e);
                    }
                    throw e;
                }
            }

            throw new FormatException($"Can't parse '{errStr}' as a binary expression!");
        }

        public Node[] InterpretListOfNodes(string val)
        {
            List<Node> retval = new List<Node>();

            bool justClosedParens = false;
            string next = "";
            for (int i = 0; i < val.Length; i++)
            {
                char curr = val[i];
                switch (curr)
                {
                    case '(':
                        if (justClosedParens) throw new FormatException($"Expected ',' or EOL after '{next}', but found '{curr}'!");
                        string sub;
                        try
                        {
                            sub = FindBallancedParens(val.Substring(i));
                        }
                        catch (FormatException e)
                        {
                            throw new FormatException($"Error in ballancing parenthesis in '{val}'!", e);
                        }
                        next += sub;
                        justClosedParens = true;
                        i += sub.Length - 1;
                        break;
                    case ')':
                        throw new FormatException($"Unballanced parenthesis ')' at index '{i}' in '{val}'!");
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        // do nothing
                        break;
                    case ',':
                        if (next.Length > 0)
                        {
                            retval.Add(InterpretNode(next));
                        }
                        justClosedParens = false;
                        next = "";
                        break;
                    default:
                        next += curr;
                        break;
                }
            }

            // catch any straggling values
            if (next.Length > 0)
            {
                retval.Add(InterpretNode(next));
            }

            return retval.ToArray();
        }

        public string FindBallancedParens(string val)
        {
            int parenCount = 0;
            for (int i = 0; i < val.Length; i++)
            {
                char curr = val[i];
                switch (curr)
                {
                    case '(':
                        parenCount++;
                        break;
                    case ')':
                        parenCount--;
                        if (parenCount == 0)
                        {
                            return val.Substring(0, i + 1);
                        }
                        break;
                    default:
                        break;
                }
            }

            // catch extra parens
            if (parenCount > 0)
            {
                throw new FormatException($"Unballanced parenthesis, found {parenCount} extra '(' in '{val}'!");
            }

            throw new FormatException($"Can't find any parentheses in '{val}'!");
        }
    }
}
