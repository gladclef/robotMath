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
        public enum Token { name, exprChar, num, paren, other, parenOp };
        public static Regex NameMatcher = new Regex("^[a-z_][a-z0-9_]*", RegexOptions.IgnoreCase);
        public static Regex ExprCharMatcher = new Regex("^[@!#$%^&*|/\\-+~]");
        public static Regex NumberMatcher = new Regex("^([0-9]*[,\\.][0-9]+|[0-9]+)");
        public static Regex OpenParen = new Regex("^\\(");

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

            // try to interpret as an operator
            if (!val.Contains('(') && !val.Contains(',')) // "a*b"
            {
                List<string> testSplits = SplitOnRecognizedCharGroups(val, 2);
                if (testSplits.Count > 1)
                {
                    Node matchedNode = InterpretUnaryOrBinaryMatch(inval, val);
                    if (matchedNode != null)
                    {
                        return matchedNode;
                    }
                }

                // interpret as a variable
                return m(val);
            }

            // it must contain a parenthetical somewhere
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
                return InterpretUnaryOrBinaryMatch(inval, val);
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
                    result = InterpretUnaryOrBinaryMatch(afterParenExpr, afterParenExpr, result);
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Unable to parse binary expression from '{afterParenExpr}' after parsing '{beforeParen+parenExpr}'!", e);
                }
            }
            return result;
        }

        /// <summary>
        /// Splits on names, operators, and numbers.
        /// If a name is followed immediately by a parenthesis group, the two are returned as one string.
        /// If a minus sign "-" is followed immediately by a number and preceded by either
        ///   nothing or another operator char, then the minus sign and number are returned as one string.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="maxMatches"></param>
        /// <returns></returns>
        private List<string> SplitOnRecognizedCharGroups(string val, int maxMatches)
        {
            // split into recognized character groups
            List<string> nvals = new List<string>();
            nvals.Add(val.Replace(" ", ""));

            maxMatches = Math.Max(maxMatches, 2);
            MatchCollection nameMatch, exprCharMatch, numberMatch, openParenMatch;
            List<Token> history = new List<Token>();
            for (int i = 0; i < nvals.Count && i < maxMatches; i++)
            {
                string currVal = nvals[i];
                MatchCollection match;
                bool isName = (nameMatch = NameMatcher.Matches(currVal)).Count > 0;
                bool isExprChar = (exprCharMatch = ExprCharMatcher.Matches(currVal)).Count > 0;
                bool isNum = (numberMatch = NumberMatcher.Matches(currVal)).Count > 0;
                bool isParen = (openParenMatch = OpenParen.Matches(currVal)).Count > 0;
                if (isName || isExprChar || isNum || isParen)
                {
                    Token token = (isName) ? Token.name :
                                  (isExprChar) ? Token.exprChar :
                                  (isNum) ? Token.num :
                                  Token.paren;
                    string matchStr = (isName) ? nameMatch[0].Value :
                                      (isExprChar) ? exprCharMatch[0].Value :
                                      (isNum) ? numberMatch[0].Value :
                                      openParenMatch[0].Value;

                    if (isParen && i > 0 && history[i - 1].Equals(Token.name))
                    {
                        // special case ^..."name("
                        matchStr = FindBallancedParens(currVal);
                        nvals[i - 1] += matchStr;
                        token = Token.parenOp;
                        i--;
                    }
                    else if (i > 0 && nvals[i - 1].Equals("-") &&
                             ( i == 1 || (i > 1 && history[i-2].Equals(Token.exprChar)) ) )
                    {
                        // special case ^..."*-1" or ^"-1"
                        nvals[i - 1] += matchStr;
                        i--;
                    }
                    else
                    {
                        // name, special symbol, number, or paren group
                        if (isParen)
                        {
                            matchStr = FindBallancedParens(currVal);
                        }
                        nvals.Insert(i, matchStr);
                    }

                    nvals.RemoveAt(i+1);
                    if (!currVal.Equals(matchStr))
                    {
                        nvals.Add(currVal.Substring(matchStr.Length));
                    }
                    history = history.GetRange(0, i);
                    history.Add(token);
                }
            }

            return nvals;
        }

        /// <summary>
        /// Finds one or more unary/binary matches in the given val.
        /// </summary>
        /// <param name="inval"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private Node InterpretUnaryOrBinaryMatch(string inval, string val)
        {
            List<string> nvals = SplitOnRecognizedCharGroups(val, 10000);
            Node firstVal = InterpretNode(nvals[0]);
            nvals = nvals.GetRange(1, nvals.Count - 1);
            while (nvals.Count > 1)
            {
                firstVal = InterpretBinaryMatch(inval, nvals, 0, firstVal);
            }
            return firstVal;
        }

        /// <summary>
        /// Finds one or more unary/binary matches in the given val and with the given firstVal node.
        /// </summary>
        /// <param name="inval"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private Node InterpretUnaryOrBinaryMatch(string inval, string val, Node firstVal)
        {
            List<String> nvals = SplitOnRecognizedCharGroups(val, 10000);
            while (nvals.Count > 1)
            {
                firstVal = InterpretBinaryMatch(inval, nvals, 0, firstVal);
            }
            return firstVal;
        }

        /// <summary>
        /// Finds one or more binary matches in the given nvals and with the given firstVal node.
        /// </summary>
        /// <param name="inval"></param>
        /// <param name="nvals">Length of 2+. Any values used by this method are removed.</param>
        /// <param name="firstVal"></param>
        /// <returns></returns>
        private Node InterpretBinaryMatch(string inval, List<string> nvals, int startIndex, Node firstVal)
        {
            inval = inval.Trim();
            string errStr = firstVal.Unparse() + inval;

            // check: is either unary, binary, or invalid
            if (nvals.Count <= startIndex + 1)
            {
                throw new FormatException($"Can't parse '{errStr}' as a binary expression!");
            }
            string op = nvals[startIndex];

            // Change the order based on operator precident.
            // More values will be interpretted as long as the following operators have a higher precidence than the operator at nvals[0].
            // No more than nvals.Count - 1 values will be interpretted (aka the first value will never be touched).
            Node secondVal = InterpretNode(nvals[startIndex + 1]);
            while (nvals.Count > startIndex + 2 && ComparePrecident(op, nvals[startIndex + 2]) > 0)
            {
                secondVal = InterpretBinaryMatch(inval, nvals, startIndex + 2, secondVal);
            }

            // interpret the first value and secondVal as a binary expression
            try
            {
                nvals.RemoveAt(startIndex);
                nvals.RemoveAt(startIndex);
                return m(op, firstVal, secondVal);
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

        /// <summary>
        /// Interprets a comma-delimited list of nodes.
        /// Parenthesis ballancing is observed via FindBallancedParens(string).
        /// </summary>
        /// <param name="val">A string containing 0 or more commas.</param>
        /// <returns></returns>
        public Node[] InterpretListOfNodes(string val)
        {
            List<Node> retval = new List<Node>();
            
            string next = "";
            for (int i = 0; i < val.Length; i++)
            {
                char curr = val[i];
                switch (curr)
                {
                    case '(':
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
                List<string> nvals = SplitOnRecognizedCharGroups(next, 2);
                if (nvals.Count > 1)
                {
                    retval.Add(InterpretUnaryOrBinaryMatch(val, next));
                }
                else
                {
                    retval.Add(InterpretNode(next));
                }
            }

            return retval.ToArray();
        }

        /// <summary>
        /// Find the substring containing the next parenthesis group.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
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
