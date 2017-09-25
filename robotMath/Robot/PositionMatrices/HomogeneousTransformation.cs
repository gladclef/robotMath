using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotMath.Robot.FrameUtil;
using RobotMath.LinearAlgebra;

namespace RobotMath.Robot.PositionMatrices
{
    public class HomogeneousTransformation : FrameTransformationMatrix
    {
        public static FrameTransformationVector BottomRowVector = new FrameTransformationVector(new double[,] { { 0, 0, 0, 1 } }, null, null);
        
        public RotationMatrix RotationMatrix { get; }
        public TranslationVector TranslationVector { get; }
        public HomogeneousTransformation HomogenousMatrix => this;

        public HomogeneousTransformation(RotationMatrix rotationMatrix) : this(
            GenerateHomogenousMatrix(rotationMatrix, new TranslationVector(new double[,] { { 0 }, { 0 }, { 0 } })),
            rotationMatrix.BaseFrame, rotationMatrix.ToFrame)
        {
        }

        public HomogeneousTransformation(TranslationVector translationVector) : this(
            GenerateHomogenousMatrix(new RotationMatrix(new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } }), translationVector),
            translationVector.BaseFrame, translationVector.ToFrame)
        {
        }

        public HomogeneousTransformation(Matrix values, Frame baseFrame, Frame toFrame) : base(values.Values, baseFrame, toFrame)
        {
            if (values.Rows != 4 || values.Cols != 4)
            {
                throw new ArgumentException("must be a 4x4 matrix", nameof(values));
            }
            if (!values.SubMatrix(3, 4, 0, 4).Equals(BottomRowVector))
            {
                throw new ArgumentException("bottom row must be equal to " + BottomRowVector, nameof(values));
            }
            
            RotationMatrix = new RotationMatrix(SubMatrix(0, 3, 0, 3).Values, BaseFrame, ToFrame);
            TranslationVector = new TranslationVector(SubMatrix(0, 3, 3, 4).Values, BaseFrame, ToFrame);
        }

        public HomogeneousTransformation(RotationMatrix rotationMatrix, TranslationVector translationVector) :
            this(GenerateHomogenousMatrix(rotationMatrix, translationVector), rotationMatrix.BaseFrame, rotationMatrix.ToFrame)
        {
            if (rotationMatrix.BaseFrame != translationVector.BaseFrame && !rotationMatrix.BaseFrame.Equals(translationVector.BaseFrame))
            {
                throw new ArgumentException("base frame and to frame must be the same for rotation matrix and translation vector, they are \"" +
                                            rotationMatrix.BaseFrame + "\" and \"" + translationVector.BaseFrame + "\"");
            }
            if (rotationMatrix.ToFrame != translationVector.ToFrame && !rotationMatrix.ToFrame.Equals(translationVector.ToFrame))
            {
                throw new ArgumentException("base frame and to frame must be the same for rotation matrix and translation vector, they are \"" +
                                            rotationMatrix.ToFrame + "\" and \"" + translationVector.ToFrame + "\"");
            }
        }

        private HomogeneousTransformation(RotationMatrix rotationMatrix, TranslationVector translationVector, Frame baseFrame, Frame toFrame) :
            this(GenerateHomogenousMatrix(rotationMatrix, translationVector), baseFrame, toFrame)
        {
        }

        public HomogeneousTransformation DotProduct(HomogeneousTransformation other)
        {
            return new HomogeneousTransformation(base.DotProduct(other), BaseFrame, other.ToFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="InvalidExpressionException">always</exception>
        public new void Sum(Matrix other)
        {
            throw new InvalidExpressionException("Can't perform a sum on a " + nameof(HomogenousMatrix));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="InvalidExpressionException">always</exception>
        public void Tranform()
        {
            throw new InvalidExpressionException("Can't perform a transform on a " + nameof(HomogenousMatrix));
        }

        /// <summary>
        /// Translates relative to the base frame.
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public HomogeneousTransformation Translate(TranslationVector translation)
        {
            translation = (translation.Rows == 3 ? translation : translation.Transform());
            return new HomogeneousTransformation(RotationMatrix, translation + TranslationVector, ToFrame, translation.ToFrame);
        }

        /// <summary>
        /// Translates along the current X, Y, and Z axis.
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public HomogeneousTransformation TranslateByCurrentOrientation(TranslationVector translation)
        {
            HomogeneousTransformation transHT = new HomogeneousTransformation(translation);
            TranslationVector relTranslation = this.DotProduct(transHT).TranslationVector;
            relTranslation = new TranslationVector(relTranslation, TranslationVector.ToFrame, relTranslation.ToFrame);
            return new HomogeneousTransformation(RotationMatrix, TranslationVector + relTranslation, BaseFrame, translation.ToFrame);
        }

        public HomogeneousTransformation Rotate(RotationMatrix rotation)
        {
            HomogeneousTransformation rotationHomogeneousTransformation = new HomogeneousTransformation(rotation);
            return new HomogeneousTransformation(DotProduct(rotationHomogeneousTransformation), BaseFrame, ToFrame);
        }

        private static Matrix GenerateHomogenousMatrix(RotationMatrix rotation, TranslationVector translation)
        {
            double[,] values = new double[4,4];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    values[i, j] = rotation.Values[i, j];
                }
            }

            values[0, 3] = translation.X;
            values[1, 3] = translation.Y;
            values[2, 3] = translation.Z;

            values[3, 3] = 1;

            return new Matrix(values);
        }

        public new String ToString()
        {
            StringBuilder retval = new StringBuilder("HT");
            if (BaseFrame != null || ToFrame != null)
            {
                retval.Append("<");
                retval.Append(BaseFrame);
                retval.Append(",");
                retval.Append(ToFrame);
                retval.Append(">:");
            }
            else
            {
                retval.Append("<>");
            }
            retval.Append(base.ToString());
            return retval.ToString();
        }

        public new string PrettyPrint()
        {
            StringBuilder retval = new StringBuilder();
            if (BaseFrame != Frame.ReferenceFrame || ToFrame != Frame.ReferenceFrame)
            {
                retval.Append("Homogenous Transformation Matrix from Base Frame \"");
                retval.Append(BaseFrame);
                retval.Append("\" To Frame \"");
                retval.Append(ToFrame);
                retval.Append("\"\n");
            }
            retval.Append(base.ToString());
            return retval.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return (o is HomogeneousTransformation) &&
                   base.Equals(o);
        }
    }
}
