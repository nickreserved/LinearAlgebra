namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface IMutableVector : IImmutableVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// Adds to this vector the <paramref name="otherVector"/> * <paramref name="otherCoefficient"/> and returns the this vector after that.
		/// </summary>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns><c>thisVector += <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></c></returns>
		IMutableVector AxpyIntoThis(IImmutableVector otherVector, double otherCoefficient);

		IMutableVector AddIntoThis(IImmutableVector otherVector)
			=> AxpyIntoThis(otherVector, +1.0);

		IMutableVector SubtractIntoThis(IImmutableVector otherVector)
			=> AxpyIntoThis(otherVector, -1.0);

		IMutableVector ScaleIntoThis(double coefficient);

		/// <summary>
		/// A linear combination between this and another one vector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this vector</param>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns><c>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></c></returns>
		IMutableVector LinearCombinationIntoThis(double thisCoefficient, IImmutableVector otherVector, double otherCoefficient);

		void CopyFrom(IImmutableVector otherVector);

		/// <summary>
		/// Set all elements of vector to zero.
		/// </summary>
		void Clear();

		/// <summary>
		/// Set all elements of vector to <paramref name="value"/>.
		/// </summary>
		void SetAll(double value);



		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// this[i] = <paramref name="binaryOperation"/>(this[i], <paramref name="otherVector"/>[i]). 
		/// The resulting vector overwrites the entries of this.
		/// </summary>
		/// <param name="otherVector">A vector with the same <see cref="IIndexable1D.Length"/> as this.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IIndexable1D.Length"/> than this.
		/// </exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// </exception> 
		void DoEntrywiseIntoThis(IImmutableVector otherVector, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: this[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting vector overwrites the entries of this.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// </exception> 
		void DoToAllEntriesIntoThis(Func<double, double> unaryOperation);


		// -------- OPERATORS: implied by C# because of +, - and *

		// static IMutableVector operator +=(IImmutableVector otherVector) => AddIntoThis(otherVector);
		// static IMutableVector operator -=(IImmutableVector otherVector) => SubtractIntoThis(otherVector);
		// static IMutableVector operator *=(double coefficient) => ScaleIntoThis(coefficient);
	}
}
