namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface IImmutableVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same number of elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IMutableVector Axpy(IImmutableVector otherVector, double otherCoefficient)
			=> Copy().AxpyIntoThis(otherVector, otherCoefficient);

		IMutableVector Add(IImmutableVector otherVector)
			=> Axpy(otherVector, +1.0);

		IMutableVector Subtract(IImmutableVector otherVector)
			=> Axpy(otherVector, -1.0);

		double DotProduct(IImmutableVector otherVector);

		/// <summary>
		/// Returns the square of this otherVector <c>this * this</c>
		/// </summary>
		double Square()
			=> this.DotProduct(this);

		IMutableVector Scale(double coefficient)
			=> Copy().ScaleIntoThis(coefficient);

		/// <summary>
		/// A linear combination between this and another one otherVector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherVector</param>
		/// <param name="otherVector">A otherVector with the same number of elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IMutableVector LinearCombination(double thisCoefficient, IImmutableVector otherVector, double otherCoefficient)
			=> Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		double Norm2() => Math.Sqrt(Square());

		IMutableVector Copy()
		{
			IMutableVector copy = CreateZero();
			copy.CopyFrom(this);
			return copy;
		}

		/// <summary>
		/// Creates a new otherVector with all elements set to zero, the same number of elements with this otherVector and probably with the same format with this otherVector.
		/// </summary>
		/// <returns>A new zero otherVector with the same number of elements with this otherVector</returns>
		IMutableVector CreateZero();

		/// <summary>
		/// Check if this otherVector and <paramref name="otherVector"/> are almost equal.
		/// </summary>
		/// <param name="otherVector">A otherVector of any number of elements</param>
		/// <param name="tolerance">The maximum difference between corresponding elements to considered equal</param>
		/// <returns>True if both vectors are almost equal</returns>
		bool Equals(IImmutableVector otherVector, double tolerance = 1e-7);



		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// result[i] = <paramref name="binaryOperation"/>(this[i], <paramref name="otherVector"/>[i]). 
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same <see cref="IIndexable1D.Length"/> as this.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IIndexable1D.Length"/> than this.
		/// </exception>
		IMutableVector DoEntrywise(IImmutableVector otherVector, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMutableVector DoToAllEntries(Func<double, double> unaryOperation);



		// -------- OPERATORS
		public static IMutableVector operator +(IImmutableVector x, IImmutableVector y) => x.Add(y);
		public static IMutableVector operator -(IImmutableVector x, IImmutableVector y) => x.Subtract(y);
		public static double operator *(IImmutableVector x, IImmutableVector y) => x.DotProduct(y);
		public static IMutableVector operator *(IImmutableVector x, double y) => x.Scale(y);
		public static IMutableVector operator *(double y, IImmutableVector x) => x.Scale(y);
	}
}
