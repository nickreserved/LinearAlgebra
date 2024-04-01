using System;
using System.Transactions;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Vectors;

//TODO: Should I reimplement the methods to avoid allocating and deallocating vectors?
namespace MGroup.LinearAlgebra.Matrices
{
    public static class RawArrayVectorExtensions
    {
        /// <summary>
        /// Performs the following operation for all i:
        /// result[i] = this[i] + <paramref name="otherVector"/>[i].
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static double[] Add(this double[] thisVector, double[] otherVector)
            => thisVector.Axpy(otherVector, 1.0);

        /// <summary>
        /// Performs the following operation for all i:
        /// this[i] = this[i] + <paramref name="otherVector"/>[i]. 
        /// The resulting vector overwrites the entries of this.
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static double[] AddIntoThis(this double[] thisVector, double[] otherVector)
            => thisVector.AxpyIntoThis(otherVector, 1.0);

        /// <summary>
        /// Performs the following operation for all i:
        /// result[i] = <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i] + this[i].
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static double[] Axpy(this double[] thisVector, double[] otherVector, double otherCoefficient)
            => new Vector(thisVector).Axpy(new Vector(otherVector), otherCoefficient).Elements;

        /// <summary>
        /// Performs the following operation for all i:
        /// this[i] = <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i] + this[i]. 
        /// The resulting vector overwrites the entries of this.
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
        /// <exception cref="Exceptions.NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static double[] AxpyIntoThis(this double[] thisVector, double[] otherVector, double otherCoefficient)
            => ((Vector) new Vector(thisVector).AxpyIntoThis(new Vector(otherVector), otherCoefficient)).Elements;

        /// <summary>
        /// Sets all entries of this vector to 0. 
        /// </summary>
        public static void Clear(this double[] thisVector) => Array.Clear(thisVector, 0, thisVector.Length);

		/// <summary>
		/// Copies the entries of this instance to a new double[] array and returns it.
		/// </summary>
		public static double[] Copy(this double[] thisVector) => (double[]) thisVector.Clone();

        /// <summary>
        /// Copies all entries from <paramref name="sourceVector"/> to this vector.
        /// </summary>
        /// <param name="sourceVector">The vector containing the entries to be copied.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="sourceVector"/>.Length != this.Length.
        /// </exception>
        public static double[] CopyFrom(this double[] thisVector, double[] sourceVector)
        {
            Preconditions.CheckVectorDimensions(thisVector, sourceVector);
            Array.Copy(sourceVector, thisVector, thisVector.Length);
			return thisVector;
        }

		/// <summary>
		/// Returns the Z component of the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 2 dimensional vectors.
		/// </summary>
		/// For calculation uses the 2 first components of this vector with 2 first components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Z component of the cross product</returns>
		static public double CrossProductZ(this double[] thisVector, double[] otherVector) => thisVector[0] * otherVector[1] - thisVector[1] * otherVector[0];
		/// <summary>
		/// Returns the X component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 2nd and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The X component of the cross product</returns>
		static public double CrossProductX(this double[] thisVector, double[] otherVector) => -thisVector[1] * otherVector[2] + thisVector[2] * otherVector[1];
		/// <summary>
		/// Returns the Y component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 1st and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Y component of the cross product</returns>
		static public double CrossProductY(this double[] thisVector, double[] otherVector) => thisVector[0] * otherVector[2] - thisVector[2] * otherVector[0];
		/// <summary>
		/// Returns the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 3 dimensional vectors.
		/// </summary>
		/// <param name="otherVector">The other vector</param>
		/// <returns>The cross product</returns>
		static public Vector CrossProduct(this double[] thisVector, double[] otherVector)
			=> new Vector(new double[] { CrossProductX(thisVector, otherVector), CrossProductY(thisVector, otherVector), CrossProductZ(thisVector, otherVector) });

		/// <summary>
		/// Calculates the dot (or inner/scalar) product of this vector with <paramref name="otherVector"/>
		/// </summary>
		/// <param name="otherVector">A vector with the same Length as this.</param>
		public static double DotProduct(this double[] thisVector, double[] otherVector)
            => new Vector(thisVector).DotProduct(new Vector(otherVector));

        /// <summary>
        /// Returns true if <paramref name="matrix"/>[i, j] and <paramref name="matrix"/>[j, i] are equal or at least within the 
        /// specified <paramref name="tolerance"/> for all 0 &lt;= i &lt; NumRows, 0 &lt;= j &lt; NumColumns. 
        /// </summary>
        /// <param name="matrix">The matrix that will be checked for symmetry.</param>
        /// <param name="tolerance">The entries at (i, j), (j, i) the matrix will be considered equal, if
        ///     (<paramref name="matrix"/>[i, j] - <paramref name="matrix"/>[i, j]) / <paramref name="matrix"/>[i, j] 
        ///         &lt;= <paramref name="tolerance"/>. 
        ///     Setting <paramref name="tolerance"/> = 0, will check if these entries are exactly the same.</param>
        public static bool IsSymmetric(this double[,] matrix, double tolerance = double.Epsilon) //TODO: Move this to the array extensions file.
        {
            var comparer = new ValueComparer(tolerance);
            if (matrix.GetLength(0) != matrix.GetLength(1)) return false;
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (!comparer.AreEqual(matrix[i, j], matrix[j, i])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs the matrix-vector multiplication: oper(this) * <paramref name="vector"/>.
        /// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
        /// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
        /// The resulting vector will be written in a new vector and returned.
        /// </summary>
        /// <param name="vector">
        /// A vector with length being equal to the number of columns of oper(this).
        /// </param>
        /// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="vector"/>.Length is different than the number of columns of oper(this).
        /// </exception>
        public static double[] Multiply(this IMatrixView matrix, double[] vector, bool transposeThis = false)
        {
            var result = new double[transposeThis ? matrix.NumColumns : matrix.NumRows];
            matrix.MultiplyIntoResult(new Vector(vector), new Vector(result), transposeThis);
            return result;
        }

        /// <summary>
        /// Performs the matrix-vector multiplication: <paramref name="rhsVector"/> = oper(this) * <paramref name="lhsVector"/>.
        /// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
        /// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
        /// The resulting vector will overwrite the entries of <paramref name="rhsVector"/>.
        /// </summary>
        /// <param name="lhsVector">
        /// The vector that will be multiplied by this matrix. It sits on the left hand side of the equation y = oper(A) * x.
        /// Constraints: <paramref name="lhsVector"/>.Length == oper(this).NumColumns.
        /// </param>
        /// <param name="rhsVector">
        /// The vector that will be overwritten by the result of the multiplication. It sits on the right hand side of the 
        /// equation y = oper(A) * x. Constraints: <paramref name="rhsVector"/>.Length 
        /// == oper(this).NumRows.
        /// </param>
        /// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if the <paramref name="lhsVector"/>.Length or <paramref name="rhsVector"/>.Length violate the described 
        /// constraints.
        /// </exception>
        /// <exception cref="PatternModifiedException">
        /// Thrown if the storage format of <paramref name="rhsVector"/> does not support overwritting the entries that this 
        /// method will try to.
        /// </exception>
        public static void MultiplyIntoResult(this IMatrixView matrix, double[] lhsVector, double[] rhsVector,
            bool transposeThis = false)
            => matrix.MultiplyIntoResult(new Vector(lhsVector), new Vector(rhsVector), transposeThis);

        /// <summary>
        /// Calculates the Euclidian norm or 2-norm of this vector. For more see 
        /// https://en.wikipedia.org/wiki/Norm_(mathematics)#Euclidean_norm.
        /// </summary>
        public static double Norm2(this double[] thisVector) => new Vector(thisVector).Norm2();

        /// <summary>
        /// Performs the following operation for all i: result[i] = <paramref name="scalar"/> * this[i].
        /// The resulting vector is written in a new object and then returned.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this vector.</param>
        public static double[] Scale(this double[] thisVector, double scalar) 
            => new Vector(thisVector).Scale(scalar).Elements;

        /// <summary>
        /// Performs the following operation for all i: this[i] = <paramref name="scalar"/> * this[i].
        /// The resulting vector overwrites the entries of this.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this vector.</param>
        public static double[] ScaleIntoThis(this double[] thisVector, double scalar)
            => ((Vector) new Vector(thisVector).ScaleIntoThis(scalar)).Elements;

        /// <summary>
        /// Performs the following operation for all i:
        /// result[i] = this[i] - <paramref name="otherVector"/>[i].
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <exception cref="Exceptions.NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static double[] Subtract(this double[] thisVector, double[] otherVector)
            => thisVector.Axpy(otherVector, -1.0);

        /// <summary>
        /// Performs the following operation for all i:
        /// this[i] = this[i] - <paramref name="otherVector"/>[i]. 
        /// The resulting vector overwrites the entries of this.
        /// </summary>
        /// <param name="otherVector">A vector with the same Length as this.</param>
        /// <exception cref="Exceptions.NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different Length than this.
        /// </exception>
        public static void SubtractIntoThis(this double[] thisVector, double[] otherVector)
            => thisVector.AxpyIntoThis(otherVector, -1.0);

        /// <summary>
        /// Calculates the tensor product of this vector with <paramref name="otherVector"/>:
        /// result[i, j] = this[i] * vector[j], for all valid i, j.
        /// </summary>
        /// <param name="otherVector">The other vector.</param>
        public static Matrix TensorProduct(this double[] thisVector, double[] otherVector)
        {
            //TODO: perhaps I should store them directly in a 1D col major array. That is more efficient but then I should move 
            //      this method elsewhere, so that it doesn't break the encapsulation of Matrix.
            var result = Matrix.CreateZero(thisVector.Length, otherVector.Length);
            for (int i = 0; i < thisVector.Length; ++i)
                for (int j = 0; j < otherVector.Length; ++j)
					result[i, j] = thisVector[i] * otherVector[j];
            return result;
        }
    }
}
