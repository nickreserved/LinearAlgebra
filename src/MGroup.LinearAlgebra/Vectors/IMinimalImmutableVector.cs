namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	
	/// <summary>
	/// The minimal vector functionality for algorithms, which not require any modification to vector.
	/// </summary>
	public interface IMinimalImmutableVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same number of Elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient);
		static public IMinimalMutableVector Axpy(IMinimalImmutableVector thisVector, IMinimalImmutableVector otherVector, double otherCoefficient) => thisVector.Copy().AxpyIntoThis(otherVector, otherCoefficient);

		/// <summary>
		/// Add this vector and vector <paramref name="otherVector"/> and return the result.
		/// </summary>
		/// <param name="otherVector">The other vector for addition</param>
		/// <returns>The sum of this vector and <paramref name="otherVector"/></returns>
		IMinimalMutableVector Add(IMinimalImmutableVector otherVector);
		static public IMinimalMutableVector Add(IMinimalMutableVector thisVector, IMinimalImmutableVector otherVector) => thisVector.Axpy(otherVector, 1);

		/// <summary>
		/// Subtract from this vector the vector <paramref name="otherVector"/> and return the result.
		/// </summary>
		/// <param name="otherVector">The vector which subtracted from this vector</param>
		/// <returns>The difference from this vector, of vector <paramref name="otherVector"/></returns>
		IMinimalMutableVector Subtract(IMinimalImmutableVector otherVector);
		static public IMinimalMutableVector Subtract(IMinimalMutableVector thisVector, IMinimalImmutableVector otherVector) => thisVector.Axpy(otherVector, -1);

		/// <summary>
		/// Dot product of this vector with <paramref name="otherVector"/>
		/// </summary>
		/// <param name="otherVector">The other vector for dot product</param>
		/// <returns>The scalar dot product of two vectors</returns>
		double DotProduct(IMinimalImmutableVector otherVector);

		/// <summary>
		/// Returns the square of this otherVector <c>this * this</c>
		/// </summary>
		double Square();
		static public double Square(IMinimalImmutableVector thisVector) => thisVector.DotProduct(thisVector);

		/// <summary>
		/// Return the negative of this vector.
		/// </summary>
		/// <returns>Return the negative of this vector.</returns>
		IMinimalMutableVector Negative();
		static public IMinimalMutableVector Negative(IMinimalMutableVector thisVector) => thisVector.Scale(-1);

		/// <summary>
		/// Return the multiplication of this vector and scalar <paramref name="coefficient"/>
		/// </summary>
		/// <param name="coefficient">The scalar for multiplacation with this vector</param>
		/// <returns>The multiplication of this vector with scalar <paramref name="coefficient"/></returns>
		IMinimalMutableVector Scale(double coefficient);
		static public IMinimalMutableVector Scale(IMinimalMutableVector thisVector, double coefficient) => thisVector.Copy().ScaleIntoThis(coefficient);

		/// <summary>
		/// A linear combination between this and another one otherVector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherVector</param>
		/// <param name="otherVector">A otherVector with the same number of Elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		static public IMinimalMutableVector LinearCombination(IMinimalMutableVector thisVector, double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => thisVector.Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		/// <summary>
		/// Number of Elements in vector.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// Length of vector.
		/// </summary>
		/// <returns>The length of vector which is the square root of Square()</returns>
		double Norm2();
		static public double Norm2(IMinimalMutableVector thisVector) => Math.Sqrt(thisVector.Square());

		/// <summary>
		/// Return a copy of this vector.
		/// </summary>
		/// <returns>A copy of this vector</returns>
		IMinimalMutableVector Copy();
		static public IMinimalMutableVector Copy(IMinimalMutableVector thisVector) => thisVector.CreateZero().CopyFrom(thisVector);

		/// <summary>
		/// Creates a new otherVector with all Elements set to zero, the same number of Elements with this otherVector and probably with the same format with this otherVector.
		/// </summary>
		/// <returns>A new zero otherVector with the same number of Elements with this otherVector</returns>
		IMinimalMutableVector CreateZero();

		/// <summary>
		/// Check if this vector and <paramref name="otherVector"/> are almost equal.
		/// </summary>
		/// <param name="otherVector">A vector of any number of Elements</param>
		/// <param name="tolerance">The maximum difference between corresponding elements to considered equal</param>
		/// <returns>True if both vectors are almost equal</returns>
		bool Equals(IMinimalImmutableVector otherVector, double tolerance = 1e-7);

		/// <summary>
		/// Check if elements of this vector are almost zero.
		/// </summary>
		/// <param name="tolerance">The maximum absolute value of an element to considered zero</param>
		/// <returns>True if all elements are almost zero</returns>
		bool IsZero(double tolerance = 1e-7);

		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// result[i] = <paramref name="binaryOperation"/>(this[i], <paramref name="otherVector"/>[i]). 
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same <see cref="Length"/> as this.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.
		/// </exception>
		IMinimalMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		public static IMinimalMutableVector DoEntrywise(IMinimalImmutableVector thisVector, IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => thisVector.Copy().DoEntrywiseIntoThis(otherVector, binaryOperation);

		/// <summary>
		/// Performs a unary operation on each stored entry: result[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMinimalMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		public static IMinimalMutableVector DoToAllEntries(IMinimalImmutableVector thisVector, Func<double, double> unaryOperation) => thisVector.Copy().DoToAllEntries(unaryOperation);



		// -------- OPERATORS
		public static IMinimalMutableVector operator -(IMinimalImmutableVector x) => x.Negative();
		public static IMinimalMutableVector operator +(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IMinimalMutableVector operator -(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static double operator *(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static IMinimalMutableVector operator *(IMinimalImmutableVector x, double y) => x.Scale(y);
		public static IMinimalMutableVector operator *(double y, IMinimalImmutableVector x) => x.Scale(y);
	}
}
