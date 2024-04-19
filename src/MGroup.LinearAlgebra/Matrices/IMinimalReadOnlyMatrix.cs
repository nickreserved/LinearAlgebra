namespace MGroup.LinearAlgebra.Matrices
{
	using System;

	using MGroup.LinearAlgebra.Vectors;

	public interface IMinimalReadOnlyMatrix : ILinearTransformation
	{
		/// <summary>
		/// A partial linear combination between this and another matrix.
		/// </summary>
		/// <param name="otherMatrix">A vector with the same number of Values with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMatrix Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);

		protected static IMinimalMatrix Axpy(IMinimalReadOnlyMatrix thisMatrix, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
		{
			var result = thisMatrix.Copy();
			result.AxpyIntoThis(otherMatrix, otherCoefficient);
			return result;
		}


		IMinimalMatrix Add(IMinimalReadOnlyMatrix otherMatrix);

		protected static IMinimalMatrix Add(IMinimalReadOnlyMatrix thisMatrix, IMinimalReadOnlyMatrix otherMatrix) => thisMatrix.Axpy(otherMatrix, 1);


		IMinimalMatrix Subtract(IMinimalReadOnlyMatrix otherMatrix);

		protected static IMinimalMatrix Subtract(IMinimalReadOnlyMatrix thisMatrix, IMinimalReadOnlyMatrix otherMatrix) => thisMatrix.Axpy(otherMatrix, -1);


		/// <summary>
		/// A linear combination between this and another one matrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this matrix</param>
		/// <param name="otherMatrix">A matrix with the same number of Values with this matrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public IMinimalMatrix LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);

		protected static IMinimalMatrix LinearCombination(IMinimalReadOnlyMatrix thisMatrix, double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
		{
			var result = thisMatrix.Copy();
			result.LinearCombinationIntoThis(thisCoefficient, otherMatrix, otherCoefficient);
			return result;
		}
			


		IMinimalMatrix Scale(double coefficient);

		protected static IMinimalMatrix Scale(IMinimalReadOnlyMatrix thisMatrix, double coefficient)
		{
			var result = thisMatrix.Copy();
			result.ScaleIntoThis(coefficient);
			return result;
		}


		IMinimalMatrix Copy();


		/// <summary>
		/// Creates a new matrix with all Values set to zero, the same dimensions with this matrix and probably with the same format with this matrix.
		/// </summary>
		/// <returns>A new zero matrix with the same dimensions with this matrix</returns>
		IMinimalMatrix CreateZeroWithSameFormat();


		/// <summary>
		/// Check if this matrix and <paramref name="otherMatrix"/> are almost equal.
		/// </summary>
		/// <param name="otherMatrix">A matrix of any dimensions</param>
		/// <param name="tolerance">The maximum difference between corresponding elements to considered equal</param>
		/// <returns>True if both matrices are almost equal</returns>
		/// <exception cref="InvalidCastException">If 2 matrices cannot be compared.</exception>
		bool Equals(IMinimalReadOnlyMatrix otherMatrix, double tolerance = 1e-10);


		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// result[i, j] = <paramref name="binaryOperation"/>(this[i, j], <paramref name="matrix"/>[i]). 
		/// The resulting matrix is written in a new object and then returned.
		/// </summary>
		/// <param name="matrix">A matrix with the same dimensions or some other special property than this matrix.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="matrix"/> has different dimensions or some other special property than this matrix.
		/// </exception>
		IMinimalMatrix DoEntrywise(IMinimalReadOnlyMatrix matrix, Func<double, double, double> binaryOperation)
		{
			IMinimalMatrix result = Copy();
			result.DoEntrywiseIntoThis(matrix, binaryOperation);
			return result;
		}


		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i, j]).
		/// The resulting matrix is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMinimalMatrix DoToAllEntries(Func<double, double> unaryOperation)
		{
			IMinimalMatrix result = Copy();
			result.DoToAllEntries(unaryOperation);
			return result;
		}



		// -------- OPERATORS
		public static IMinimalMatrix operator +(IMinimalReadOnlyMatrix x, IMinimalReadOnlyMatrix y) => x.Add(y);
		public static IMinimalMatrix operator -(IMinimalReadOnlyMatrix x, IMinimalReadOnlyMatrix y) => x.Subtract(y);
		public static IMinimalMatrix operator *(IMinimalReadOnlyMatrix x, double y) => x.Scale(y);
		public static IMinimalMatrix operator *(double y, IMinimalReadOnlyMatrix x) => x.Scale(y);
	}
}
