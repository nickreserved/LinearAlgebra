namespace MGroup.LinearAlgebra.Matrices
{
	using System;

	using MGroup.LinearAlgebra.Vectors;

	public interface IMinimalMatrix : IMinimalReadOnlyMatrix
	{
		/// <summary>
		/// Performs the following operation for all (i, j):
		/// this[i, j] = <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j] + this[i, j]. 
		/// Optimized version of <see cref="DoEntrywiseIntoThis(IMinimalReadOnlyMatrix, Func{double, double, double})"/> and 
		/// <see cref="LinearCombinationIntoThis(double, IMinimalReadOnlyMatrix, double)"/>. Named after BLAS axpy (y = a*x plus y). 
		/// The resulting matrix overwrites the entries of this.
		/// </summary>
		/// <param name="otherMatrix">A matrix with the same <see cref="ILinearTransformation.NumRows"/> and 
		///     <see cref="ILinearTransformation.NumColumns"/> as this.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if <paramref name="otherMatrix"/> has different 
		///     <see cref="ILinearTransformation.NumRows"/> or <see cref="ILinearTransformation.NumColumns"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">Thrown if an entry this[i, j] needs to be overwritten, but that 
		///     is not permitted by the matrix storage format.</exception>
		void AxpyIntoThis(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);


		void AddIntoThis(IMinimalReadOnlyMatrix otherMatrix);

		protected static void AddIntoThis(IMinimalMatrix thisMatrix, IMinimalReadOnlyMatrix otherMatrix) => thisMatrix.AxpyIntoThis(otherMatrix, 1);


		void SubtractIntoThis(IMinimalReadOnlyMatrix otherMatrix);
		
		protected static void SubtractIntoThis(IMinimalMatrix thisMatrix, IMinimalReadOnlyMatrix otherMatrix) => thisMatrix.AxpyIntoThis(otherMatrix, -1);


		/// <summary>
		/// Performs the following operation for all (i, j):
		/// this[i, j] = <paramref name="thisCoefficient"/> * this[i, j] + <paramref name="otherCoefficient"/> * 
		/// <paramref name="otherMatrix"/>[i, j]. 
		/// Optimized version of <see cref="DoEntrywiseIntoThis(IMinimalReadOnlyMatrix, Func{double, double, double})"/>.
		/// The resulting matrix overwrites the entries of this.
		/// </summary>
		/// <param name="thisCoefficient">A scalar that multiplies each entry of this.</param>
		/// <param name="otherMatrix">A matrix with the same <see cref="ILinearTransformation.NumRows"/> and 
		///     <see cref="ILinearTransformation.NumColumns"/> as this.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if <paramref name="otherMatrix"/> has different 
		///     <see cref="ILinearTransformation.NumRows"/> or <see cref="ILinearTransformation.NumColumns"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">Thrown if an entry this[i, j] needs to be overwritten, but that 
		///     is not permitted by the matrix storage format.</exception>
		void LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);
		

		void ScaleIntoThis(double coefficient);


		/// <summary>
		/// Sets all matrix elements to 0. For sparse or block matrices: the indexing arrays will not be mutated. Therefore the sparsity  
		/// pattern will be preserved. The non-zero entries will be set to 0, but they will still be stored explicitly. 
		/// </summary>
		void Clear();


		/// <summary>
		/// Performs a binary operation on each pair of entries:  
		/// this[i, j] = <paramref name="binaryOperation"/>(this[i,j], <paramref name="matrix"/>[i,j]).
		/// The resulting matrix overwrites the entries of this.
		/// </summary>
		/// <param name="matrix">A matrix with the same dimensions or some other special property than this matrix.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="matrix"/> has different dimensions or some other special property than this matrix.
		/// </exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i, j] needs to be overwritten, but that is not permitted by the matrix storage format.
		/// </exception>
		void DoEntrywiseIntoThis(IMinimalReadOnlyMatrix matrix, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: this[i] = <paramref name="unaryOperation"/>(this[i, j]).
		/// he resulting matrix overwrites the entries of this.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i, j] needs to be overwritten, but that is not permitted by the matrix storage format.
		/// </exception>
		void DoToAllEntriesIntoThis(Func<double, double> unaryOperation);
	}
}
