
//
//  This is the helper file for the RealTree example
//  program.  It is an example program for use of the
//  GPPG parser generator.  Copyright (c) K John Gough 2012
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using QUT.Gppg;

namespace robotMath.Expression {
    //
    // There are several classes defined here.
    //
    // The parser class is named Parser (the gppg default),
    // and contains a handwritten scanner with class name
    // RealTree.Parser.Lexer.
    //
    // The Parser class is partial, so that most of the 
    // handwritten code may be placed in this file in a
    // second part of the class definition.
    //
    // This file also contains the class hierarchy for the
    // semantic action class RealTree.Node
    //
    // The token enumeration RealTree.Parser.Tokens (also
    // the gppg default) is generated by gppg.
    //
    public partial class Parser
    {
        private AbstractScanner<Object, LexLocation> scanner;
        public static Parser singleton = null;

        internal Dictionary<string, Node> Vars = new Dictionary<string, Node>();
        internal Dictionary<Node, double> VarVals = new Dictionary<Node, double>();
        internal HashSet<Node> VarsWithValues = new HashSet<Node>();
        public Node Root { get; private set; }
        public String LastSym { get; }

        // 
        // GPPG does not create a default parser constructor
        // Most applications will have a parser type with other
        // fields such as error handlers etc.  Here is a minimal
        // version that just adds the default scanner object.
        //

        public Parser( AbstractScanner<Object, LexLocation> scanner ) : base(scanner)
        {
            this.scanner = scanner;
            this.Root = null;
            this.LastSym = "";
        }

        private void RegisterNode(Node node)
        {
            Root = node;
        }

        public void SetVar(string name, double value)
        {
            if (!Vars.ContainsKey(name))
            {
                throw new ArgumentException($"Cannot set value of variable '{name}' to '{value}'. There is no such variable!");
            }
            Node var = Vars[name];
            if (!VarsWithValues.Contains(var))
            {
                VarsWithValues.Add(var);
                VarVals.Add(var, value);
            }
            else
            {
                VarVals.Remove(var);
                VarVals.Add(var, value);
            }
        }

        // ==================================================================================
        // Helper Methods
        // ==================================================================================

        private string GetText()
        {
            return ((Scanner) scanner).yytext;
        }

        // ==================================================================================
        // Factory Methods
        // ==================================================================================

        public NodeTag GetNodeTag(String op)
        {
            return Binary.getTag(op);
        }

        public Node MakeBinary(NodeTag tag, Node lhs, Node rhs)
        {
            Node retval = new Binary(this, tag, lhs, rhs);
            RegisterNode(retval);
            return retval;
        }

        public Node MakeBinary(String op, Node lhs, Node rhs)
        {
            return MakeBinary(Binary.getTag(op), lhs, rhs);
        }

        public Node m(string op, double l, double r)
        {
            return MakeBinary(op, m(l), m(r));
        }

        public Node m(string op, string l, double r)
        {
            return MakeBinary(op, m(l), m(r));
        }

        public Node m(string op, double l, string r)
        {
            return MakeBinary(op, m(l), m(r));
        }

        public Node m(string op, string l, string r)
        {
            return MakeBinary(op, m(l), m(r));
        }

        public Node m(string op, Node l, double r)
        {
            return MakeBinary(op, l, m(r));
        }

        public Node m(string op, Node l, string r)
        {
            return MakeBinary(op, l, m(r));
        }

        public Node m(string op, double l, Node r)
        {
            return MakeBinary(op, m(l), r);
        }

        public Node m(string op, string l, Node r)
        {
            return MakeBinary(op, m(l), r);
        }

        public Node m(string op, Node l, Node r)
        {
            return MakeBinary(op, l, r);
        }

        public Node MakeUnary(NodeTag tag, Node child) {
            Node retval = new Unary( this, tag, child );
            RegisterNode(retval);
            return retval;
        }

        public Node MakeUnary(string op, Node child)
        {
            return MakeUnary(Unary.getTag(op), child);
        }

        public Node m(string op, double child)
        {
            return MakeUnary(op, m(child));
        }

        public Node m(string op, string child)
        {
            return MakeUnary(op, m(child));
        }

        public Node m(string op, Node child)
        {
            return MakeUnary(op, child);
        }

        public Node MakeLeaf(string n) {
            if (Vars.ContainsKey(n))
            {
                return Vars[n];
            }
            Leaf retval = new Leaf( this, n );
            Vars.Add(n, retval);
            VarVals.Add(retval, 0);
            RegisterNode(retval);
            return retval;
        }

        public Node MakeLeaf(double v) {
            Node retval = new Leaf( this, v);
            RegisterNode(retval);
            return retval;
        }

        public Node m(string n)
        {
            return MakeLeaf(n);
        }

        public Node m(double v)
        {
            return MakeLeaf(v);
        }

        public Node MakeIdentifierOp(String name, Node lhs, Node rhs)
        {
            NodeTag tag = (NodeTag)Enum.Parse(typeof(NodeTag), name);
            if (!Enum.IsDefined(typeof(NodeTag), tag))
            {
                throw new BadOperatorException(name);
            }
            try
            {
                Node retval = MakeBinary(tag, lhs, rhs);
                retval.Unparse();
                RegisterNode(retval);
                return retval;
            }
            catch (BadOperatorException e)
            {
                throw new BadOperatorException($"Operator {name} is not a binary operator!", e);
            }
        }

        public Node MakeIdentifierOp(String name, Node child)
        {
            NodeTag tag = (NodeTag)Enum.Parse(typeof(NodeTag), name);
            if (!Enum.IsDefined(typeof(NodeTag), tag))
            {
                throw new BadOperatorException(name);
            }
            try
            {
                Node retval = MakeUnary(tag, child);
                retval.Unparse();
                RegisterNode(retval);
                return retval;
            }
            catch (BadOperatorException e)
            {
                throw new BadOperatorException($"Operator {name} is not a unary operator!", e);
            }
        }

        // ==================================================================================
        // Error classes
        // ==================================================================================

        public class NoSetValueException : Exception
        {
            public NoSetValueException(string name) : base($"Variable \"{name}\" has no value set, cannot be evaluated!")
            {
            }
        }

        public class BadOperatorException : Exception
        {
            public BadOperatorException(string name) : base($"Operator tag \"{name}\" is not a recognized tag name!")
            {
            }

            public BadOperatorException(string message, BadOperatorException innerException) : base(message, innerException)
            {
            }
        }

        public class IncompatibleParserException : Exception
        {
            public IncompatibleParserException(Parser a, Parser b) : base ($"Parser {a} and parser {b} must be the same parser in order to compare their internal expressions.")
            {
            }
        }
    }

    // ==================================================================================
    //  Start of Node Definitions
    // ==================================================================================

    public enum NodeTag { name, literal, plus, minus, mul, div, remainder, negate, exp, sin, cos, tan, asin, acos, atan, atan2 }

    public abstract class Node {
        public NodeTag Tag { get;  }
        public Parser Tree { get; }

        protected Node(Parser tree, NodeTag tag) { this.Tag = tag; this.Tree = tree; }
        public abstract double Eval();
        public abstract string Unparse();

        public static Node operator +(Node lhs, Node rhs)
        {
            return lhs.Tree.m("+", lhs, rhs);
        }

        public static Node operator -(Node lhs, Node rhs)
        {
            return lhs.Tree.m("-", lhs, rhs);
        }

        public static Node operator *(Node lhs, Node rhs)
        {
            return lhs.Tree.m("*", lhs, rhs);
        }

        public static Node operator /(Node lhs, Node rhs)
        {
            return lhs.Tree.m("/", lhs, rhs);
        }

        public static Node operator ^(Node lhs, Node rhs)
        {
            return lhs.Tree.m("^", lhs, rhs);
        }

        public Node Simplify()
        {
            string current = Unparse();
            Node newNode;
            bool change;
            do
            {
                newNode = SimplifyNode();
                string next = newNode.Unparse();
                change = !current.Equals(next);
                current = next;
            } while (change);
            return newNode;
        }

        protected abstract Node SimplifyNode();
    }

    public class Leaf : Node {
        public String Name { get; }
        public double Value { get; set; }

        internal Leaf(Parser tree, String name) : base(tree, NodeTag.name)
        {
            this.Name = name;
            this.Value = 0;
        }

        internal Leaf(Parser tree, double value) : base(tree, NodeTag.literal)
        {
            this.Name = "";
            this.Value = value;
        }

        public override double Eval() {
            if (this.Tag == NodeTag.name)
            {
                if (!Tree.VarsWithValues.Contains(this))
                {
                    throw new Parser.NoSetValueException(this.Name);
                }
            }
            return this.Value;
        }

        protected override Node SimplifyNode()
        {
            if (this.Tag == NodeTag.name)
            {
                if (Tree.VarsWithValues.Contains(this))
                {
                    return Tree.m(Tree.VarVals[this]);
                }
            }
            return this;
        }

        public override string Unparse() {
            if (Tag == NodeTag.name)
                return this.Name;
            else
                return this.Value.ToString(CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return Unparse();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Leaf o = (Leaf)obj;
            if (this.Tag != o.Tag)
            {
                return false;
            }

            if (this.Tag == NodeTag.name)
            {
                return this.Name.Equals(o.Name);
            }
            return Math.Abs(this.Value - o.Value) < 0.000001;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Tag.GetHashCode()
                ^ Name.GetHashCode()
                ^ Value.GetHashCode();
        }
    }

    public class Unary : Node {
        public Node Child { get; }
        internal Unary(Parser tree, NodeTag tag, Node child )
            : base( tree, tag ) { this.Child = child; }

        public override double Eval()
        {
            double childVal = this.Child.Eval();
            switch (this.Tag)
            {
                case NodeTag.negate: return -childVal;
                case NodeTag.sin: return Math.Sin(childVal);
                case NodeTag.cos: return Math.Cos(childVal);
                case NodeTag.tan: return Math.Tan(childVal);
                case NodeTag.asin: return Math.Asin(childVal);
                case NodeTag.acos: return Math.Acos(childVal);
                case NodeTag.atan: return Math.Atan(childVal);
                default: throw new Parser.BadOperatorException(Enum.GetName(typeof(NodeTag), Tag));
            }
        }

        protected override Node SimplifyNode()
        {
            Node newChild = this.Child.Simplify();
            Node newNode = Tree.MakeUnary(Tag, newChild);
            if (newChild.Tag.Equals(NodeTag.literal))
            {
                return Tree.m(newNode.Eval());
            }
            return newNode;
        }

        public override string Unparse()
        {
            string childVal = this.Child.Unparse();
            string op = "";
            switch (this.Tag)
            {
                case NodeTag.negate: return $"-{childVal}";
                case NodeTag.sin:  op = "sin"; break;
                case NodeTag.cos:  op = "cos"; break;
                case NodeTag.tan:  op = "tan"; break;
                case NodeTag.asin: op = "asin"; break;
                case NodeTag.acos: op = "acos"; break;
                case NodeTag.atan: op = "atan"; break;
                default: throw new Parser.BadOperatorException(Enum.GetName(typeof(NodeTag), Tag));
            }
            return $"{op}({childVal})";
        }

        public override string ToString()
        {
            return Unparse();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Unary o = (Unary)obj;
            if (this.Tag != o.Tag)
            {
                return false;
            }
            return this.Child.Equals(o.Child);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Tag.GetHashCode()
                ^ Child.GetHashCode();
        }

        public static NodeTag getTag(string op)
        {
            switch (op)
            {
                case "-": return NodeTag.negate;
                case "sin": return NodeTag.sin;
                case "cos": return NodeTag.cos;
                case "tan": return NodeTag.tan;
                case "asin": return NodeTag.asin;
                case "acos": return NodeTag.acos;
                case "atan": return NodeTag.atan;
                default: throw new Parser.BadOperatorException(op);
            }
        }
    }

    public class Binary : Node {
        public Node lhs { get; }
        public Node rhs { get; }

        internal Binary(Parser tree, NodeTag tag, Node lhs, Node rhs ) : base( tree, tag ) { 
            this.lhs = lhs; this.rhs = rhs; 
        }

        public override double Eval() {
            double lVal = this.lhs.Eval();
            double rVal = this.rhs.Eval();
            switch (this.Tag) {
                case NodeTag.div:       return lVal / rVal;
                case NodeTag.minus:     return lVal - rVal;
                case NodeTag.plus:      return lVal + rVal;
                case NodeTag.remainder: return lVal % rVal;
                case NodeTag.mul:       return lVal * rVal;
                case NodeTag.exp:       return Math.Pow(lVal, rVal);
                case NodeTag.atan2:     return Math.Atan2(lVal, rVal);
                default: throw new Parser.BadOperatorException(Enum.GetName(typeof(NodeTag), Tag));
            }
        }

        protected override Node SimplifyNode()
        {
            Node newLhs = lhs.Simplify();
            Node newRhs = rhs.Simplify();
            Node newNode = Tree.MakeBinary(Tag, newLhs, newRhs);
            if (newLhs.Tag.Equals(NodeTag.literal))
            {
                if (newRhs.Tag.Equals(NodeTag.literal))
                {
                    return Tree.m(newNode.Eval());
                }
                bool isZero = Math.Abs(((Leaf)newLhs).Value) < 0.000001;
                bool isOne = Math.Abs(1d - ((Leaf)newLhs).Value) < 0.000001;
                if (isZero && (Tag.Equals(NodeTag.plus) || (Tag.Equals(NodeTag.minus))))
                {
                    return newRhs;
                }
                if (isZero && (Tag.Equals(NodeTag.mul) || (Tag.Equals(NodeTag.div))))
                {
                    return newLhs;
                }
                if (isOne && Tag.Equals(NodeTag.mul))
                {
                    return newRhs;
                }
            }
            else if (newRhs.Tag.Equals(NodeTag.literal))
            {
                bool isZero = Math.Abs(((Leaf)newRhs).Value) < 0.000001;
                bool isOne = Math.Abs(1d - ((Leaf)newRhs).Value) < 0.000001;
                if (isZero && (Tag.Equals(NodeTag.plus) || (Tag.Equals(NodeTag.minus))))
                {
                    return newLhs;
                }
                if (isZero && Tag.Equals(NodeTag.mul))
                {
                    return newRhs;
                }
                if (isZero && Tag.Equals(NodeTag.div))
                {
                    return Tree.m(Double.NaN);
                }
                if (isOne && (Tag.Equals(NodeTag.mul)
                              || (Tag.Equals(NodeTag.remainder))
                              || (Tag.Equals(NodeTag.exp))))
                {
                    return newLhs;
                }
            }
            return newNode;
        }

        public override string Unparse()
        {
            string op = "";
            string lVal = this.lhs.Unparse();
            string rVal = this.rhs.Unparse();
            switch (this.Tag)
            {
                case NodeTag.div:       op = "/"; break;
                case NodeTag.minus:     op = "-"; break;
                case NodeTag.plus:      op = "+"; break;
                case NodeTag.remainder: op = "%"; break;
                case NodeTag.mul:       op = "*"; break;
                case NodeTag.exp:       op = "^"; break;
                case NodeTag.atan2: return $"atan2({lVal}, {rVal})";
                default: throw new Parser.BadOperatorException(Enum.GetName(typeof(NodeTag), Tag));
            }
            return $"({lVal} {op} {rVal})";
        }

        public bool IsReflexive()
        {
            switch (this.Tag)
            {
                case NodeTag.div: return false;
                case NodeTag.minus: return false;
                case NodeTag.plus: return true;
                case NodeTag.remainder: return false;
                case NodeTag.mul: return true;
                case NodeTag.exp: return false;
                case NodeTag.atan2: return false;
                default: throw new Parser.BadOperatorException(Enum.GetName(typeof(NodeTag), Tag));
            }
        }

        public override string ToString()
        {
            return Unparse().Replace(" ", "");
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Binary o = (Binary)obj;
            if (this.Tag != o.Tag)
            {
                return false;
            }
            if (this.lhs.Equals(o.lhs) && this.rhs.Equals(o.rhs))
            {
                return true;
            }
            if (IsReflexive() && this.rhs.Equals(o.lhs) && this.rhs.Equals(o.lhs))
            {
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Tag.GetHashCode()
                ^ lhs.GetHashCode()
                ^ rhs.GetHashCode();
        }

        public static NodeTag getTag(string op)
        {
            switch (op)
            {
                case "/": return NodeTag.div;
                case "-": return NodeTag.minus;
                case "+": return NodeTag.plus;
                case "%": return NodeTag.remainder;
                case "*": return NodeTag.mul;
                case "^": return NodeTag.exp;
                case "atan2": return NodeTag.atan2;
                default: throw new Parser.BadOperatorException(op);
            }
        }
    }
    // ==================================================================================
}

