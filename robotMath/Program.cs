using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using robotMath.Expression;
using robotMath.LinearAlgebra;
using robotMath.Robot;
using robotMath.Robot.PositionMatrices;

namespace robotMath
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string pi = Math.PI.ToString("F9");
            SillyParser p = (SillyParser) SillyParser.GetInstance();

            // build dh table
            Matrix m = p.InterpretMatrix(
                $"3, -{pi}/2, 0, t1;" +
                $"0, {pi}/2, d2, 0;" +
                $"2, 0, 0, t3");
            DenavitHartenbergTable dhTable = new DenavitHartenbergTable(m);
            Debug.WriteLine("DH Table = \n" + m.PrettyPrint().Replace("\\n", "\\n\\t"));

            // get homogenous transforms
            HomogeneousTransformation[] As = dhTable.IntermediateHomogeneousTransformations();
            As[0] = As[0].Simplify();
            As[1] = As[1].Simplify();
            As[2] = As[2].Simplify();
            Debug.WriteLine("A4.5 = \n" + ((Matrix) As[0]).PrettyPrint().Replace("\\n", "\\n\\t"));
            Debug.WriteLine("A5.6 = \n" + ((Matrix) As[1]).PrettyPrint().Replace("\\n", "\\n\\t"));
            Debug.WriteLine("A6.7 = \n" + ((Matrix) As[2]).PrettyPrint().Replace("\\n", "\\n\\t"));

            // find composite homogenous transform
            Matrix A4_6 = (As[0] * As[1]).Simplify();
            Matrix A4_7 = (A4_6 * As[2]).Simplify();
            Debug.WriteLine("A4.6 = \n" + A4_6.PrettyPrint().Replace("\\n", "\\n\\t"));
            Debug.WriteLine("A4.7 = \n" + A4_7.PrettyPrint().Replace("\\n", "\\n\\t"));

            Debug.WriteLine("done");
        }
    }
}
