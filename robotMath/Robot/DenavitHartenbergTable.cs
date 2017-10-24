using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotMath.Expression;
using robotMath.LinearAlgebra;
using robotMath.Robot.FrameUtil;
using robotMath.Robot.PositionMatrices;

namespace robotMath.Robot
{
    public class DenavitHartenbergTable : Matrix
    {
        public DenavitHartenbergTable(Matrix table) : base(table.Values)
        {
        }

        public HomogeneousTransformation[] IntermediateHomogeneousTransformations()
        {
            HomogeneousTransformation[] retval = new HomogeneousTransformation[Rows - 1];
            SillyParser p = (SillyParser) SillyParser.GetInstance();

            for (int i = 1; i < Rows; i++)
            {
                string matrixStr =
                    $"cos(_th{i}), *(-sin(_th{i}),cos(_al{i})), *(sin(_th{i}),sin(_al{i})),  *(_a{i},cos(_th{i}));" +
                    $"sin(_th{i}), *(cos(_th{i}),cos(_al{i})),  *(-cos(_th{i}),sin(_al{i})), *(_a{i},sin(_th{i}));" +
                    $"0,           sin(_al{i}),                 cos(_al{i}),                 _d{i};" +
                    $"0,           0,                           0,                           1";
                matrixStr = matrixStr.Replace($"_a{i}",  this[i - 1, 0].Unparse());
                matrixStr = matrixStr.Replace($"_al{i}", this[i - 1, 1].Unparse());
                matrixStr = matrixStr.Replace($"_d{i}",  this[i - 1, 2].Unparse());
                matrixStr = matrixStr.Replace($"_th{i}", this[i - 1, 3].Unparse());
                Matrix vals = p.interpretMatrix(matrixStr).Simplify();

                Frame baseFrame = FrameRegistry.GetFrame($"{i - 1}");
                Frame toFrame = FrameRegistry.GetFrame($"{i}");
                retval[i - 1] = new HomogeneousTransformation(vals, baseFrame, toFrame);
            }

            return retval;
        }
    }
}
