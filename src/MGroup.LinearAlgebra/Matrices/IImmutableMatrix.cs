namespace MGroup.LinearAlgebra.Matrices
{
	using System;

	using MGroup.LinearAlgebra.Vectors;

	public interface IImmutableMatrix : ILinearTransformation
	{
		/// <summary>
		/// A partial linear combination between this and another matrix.
		/// </summary>
		/// <param name="otherMatrix">A vector with the same number of Elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMutableMatrix Axpy(IImmutableMatrix otherMatrix, double otherCoefficient)
			=> Copy().AxpyIntoThis(otherMatrix, otherCoefficient);

		IMutableMatrix Add(IImmutableMatrix otherMatrix)
			=> Axpy(otherMatrix, +1.0);

		IMutableMatrix Subtract(IImmutableMatrix otherMatrix)
			=> Axpy(otherMatrix, -1.0);

		/// <summary>
		/// A linear combination between this and another one matrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this matrix</param>
		/// <param name="otherMatrix">A matrix with the same number of Elements with this matrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public IMutableMatrix LinearCombination(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient)
			=> Copy().LinearCombinationIntoThis(thisCoefficient, otherMatrix, otherCoefficient);

		IMutableMatrix Scale(double coefficient)
			=> Copy().ScaleIntoThis(coefficient);

		IMutableMatrix Copy();

		/// <summary>
		/// Creates a new matrix with all Elements set to zero, the same dimensions with this matrix and probably with the same format with this matrix.
		/// </summary>
		/// <returns>A new zero matrix with the same dimensions with this matrix</returns>
		IMutableMatrix CreateZero();

		/// <summary>
		/// Check if this matrix and <paramref name="otherMatrix"/> are almost equal.
		/// </summary>
		/// <param name="otherMatrix">A matrix of any dimensions</param>
		/// <param name="tolerance">The maximum difference between corresponding Elements to considered equal</param>
		/// <returns>True if both vectors are almost equal</returns>
		bool Equals(IImmutableMatrix otherMatrix, double tolerance = 1e-7);



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
		IMutableMatrix DoEntrywise(IImmutableMatrix matrix, Func<double, double, double> binaryOperation)
		{
			IMutableMatrix result = Copy();
			result.DoEntrywiseIntoThis(matrix, binaryOperation);
			return result;
		}

		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i, j]).
		/// The resulting matrix is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMutableMatrix DoToAllEntries(Func<double, double> unaryOperation)
		{
			IMutableMatrix result = Copy();
			result.DoToAllEntries(unaryOperation);
			return result;
		}



		// -------- OPERATORS
		public static IMutableMatrix operator +(IImmutableMatrix x, IImmutableMatrix y) => x.Add(y);
		public static IMutableMatrix operator -(IImmutableMatrix x, IImmutableMatrix y) => x.Subtract(y);
		public static IMutableMatrix operator *(IImmutableMatrix x, double y) => x.Scale(y);
		public static IMutableMatrix operator *(double y, IImmutableMatrix x) => x.Scale(y);
	}
}
