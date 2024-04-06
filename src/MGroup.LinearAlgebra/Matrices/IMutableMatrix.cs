namespace MGroup.LinearAlgebra.Matrices
{
	using System;

	using MGroup.LinearAlgebra.Vectors;

	public interface IMutableMatrix : IImmutableMatrix
	{
		IMutableMatrix AxpyIntoThis(IImmutableMatrix otherMatrix, double otherCoefficient);


		IMutableMatrix AddIntoThis(IImmutableMatrix otherMatrix);

		protected static IMutableMatrix AddIntoThis(IMutableMatrix thisMatrix, IImmutableMatrix otherMatrix) => thisMatrix.AxpyIntoThis(otherMatrix, 1);


		IMutableMatrix SubtractIntoThis(IImmutableMatrix otherMatrix);
		
		protected static IMutableMatrix SubtractIntoThis(IMutableMatrix thisMatrix, IImmutableMatrix otherMatrix) => thisMatrix.AxpyIntoThis(otherMatrix, -1);


		IMutableMatrix LinearCombinationIntoThis(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient);
		

		IMutableMatrix ScaleIntoThis(double coefficient);


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
		void DoEntrywiseIntoThis(IImmutableMatrix matrix, Func<double, double, double> binaryOperation);

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
