namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	/// <summary>
	/// The minimal vector functionality for algorithms, which not require any modification to vector.
	/// </summary>
	public interface IMinimalReadOnlyVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same number of Values with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.</exception>
		IMinimalVector Axpy(IMinimalReadOnlyVector otherVector, double otherCoefficient);
		protected static IMinimalVector Axpy(IMinimalReadOnlyVector thisVector, IMinimalReadOnlyVector otherVector, double otherCoefficient)
		{
			var result = thisVector.Copy();
			result.AxpyIntoThis(otherVector, otherCoefficient);
			return result;
		}

		/// <summary>
		/// Add this vector and vector <paramref name="otherVector"/> and return the result.
		/// </summary>
		/// <param name="otherVector">The other vector for addition</param>
		/// <returns>The sum of this vector and <paramref name="otherVector"/></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.</exception>
		IMinimalVector Add(IMinimalReadOnlyVector otherVector);
		protected static IMinimalVector Add(IMinimalVector thisVector, IMinimalReadOnlyVector otherVector) => thisVector.Axpy(otherVector, 1);

		/// <summary>
		/// Subtract from this vector the vector <paramref name="otherVector"/> and return the result.
		/// </summary>
		/// <param name="otherVector">The vector which subtracted from this vector</param>
		/// <returns>The difference from this vector, of vector <paramref name="otherVector"/></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.</exception>
		IMinimalVector Subtract(IMinimalReadOnlyVector otherVector);
		protected static IMinimalVector Subtract(IMinimalVector thisVector, IMinimalReadOnlyVector otherVector) => thisVector.Axpy(otherVector, -1);

		/// <summary>
		/// Dot product of this vector with <paramref name="otherVector"/>
		/// </summary>
		/// <param name="otherVector">The other vector for dot product</param>
		/// <returns>The scalar dot product of two vectors</returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.</exception>
		double DotProduct(IMinimalReadOnlyVector otherVector);

		/// <summary>
		/// Returns the square of this otherVector <c>this * this</c>
		/// </summary>
		double Square();
		protected static double Square(IMinimalReadOnlyVector thisVector) => thisVector.DotProduct(thisVector);

		/// <summary>
		/// Return the negative of this vector.
		/// </summary>
		/// <returns>Return the negative of this vector.</returns>
		IMinimalVector Negate();
		protected static IMinimalVector Negate(IMinimalVector thisVector)
		{
			var result = thisVector.Copy();
			result.NegateIntoThis();
			return result;
		}

		/// <summary>
		/// Return the multiplication of this vector and scalar <paramref name="coefficient"/>
		/// </summary>
		/// <param name="coefficient">The scalar for multiplacation with this vector</param>
		/// <returns>The multiplication of this vector with scalar <paramref name="coefficient"/></returns>
		IMinimalVector Scale(double coefficient);
		protected static IMinimalVector Scale(IMinimalVector thisVector, double coefficient)
		{
			var result = thisVector.Copy();
			result.ScaleIntoThis(coefficient);
			return result;
		}

		/// <summary>
		/// A linear combination between this and another one otherVector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherVector</param>
		/// <param name="otherVector">A otherVector with the same number of Values with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="Length"/> than this.</exception>
		IMinimalVector LinearCombination(double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient); // => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		protected static IMinimalVector LinearCombination(IMinimalVector thisVector, double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient)
		{
			var result = thisVector.Copy();
			result.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
			return result;
		}

		/// <summary>
		/// Number of elements in vector.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// Euclidian norm of vector.
		/// </summary>
		/// <returns>The Euclidian norm of vector which is the square root of Square()</returns>
		double Norm2();
		protected static double Norm2(IMinimalVector thisVector) => Math.Sqrt(thisVector.Square());

		/// <summary>
		/// Return a copy of this vector, of the same type.
		/// </summary>
		/// <returns>A copy of this vector</returns>
		IMinimalVector Copy();
		protected static IMinimalVector Copy(IMinimalVector thisVector)
		{
			var result = thisVector.CreateZero();
			result.CopyFrom(thisVector);
			return result;
		}

		/// <summary>
		/// Creates a new otherVector with all Values set to zero, the same number of Values with this otherVector and probably with the same format with this otherVector.
		/// </summary>
		/// For sparse vectors it returns an empty vector (with 0 stored non-zero elements).
		/// <returns>A new zero otherVector with the same number of Values with this otherVector</returns>
		IMinimalVector CreateZero();

		/// <summary>
		/// Check if this vector and <paramref name="otherVector"/> are almost equal.
		/// </summary>
		/// <param name="otherVector">A vector of any number of Values</param>
		/// <param name="tolerance">The maximum difference between corresponding elements to considered equal</param>
		/// <returns>True if both vectors are almost equal</returns>
		bool Equals(IMinimalReadOnlyVector otherVector, double tolerance = 1e-7);

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
		IMinimalVector DoEntrywise(IMinimalReadOnlyVector otherVector, Func<double, double, double> binaryOperation);
		protected static IMinimalVector DoEntrywise(IMinimalReadOnlyVector thisVector, IMinimalReadOnlyVector otherVector, Func<double, double, double> binaryOperation)
		{
			var result = thisVector.Copy();
			result.DoEntrywiseIntoThis(otherVector, binaryOperation);
			return result;
		}

		/// <summary>
		/// Performs a unary operation on each stored entry: result[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMinimalVector DoToAllEntries(Func<double, double> unaryOperation);
		protected static IMinimalVector DoToAllEntries(IMinimalReadOnlyVector thisVector, Func<double, double> unaryOperation)
		{
			var result = thisVector.Copy();
			result.DoToAllEntriesIntoThis(unaryOperation);
			return result;
		}



		// -------- OPERATORS
		public static IMinimalVector operator -(IMinimalReadOnlyVector x) => x.Negate();
		public static IMinimalVector operator +(IMinimalReadOnlyVector x, IMinimalReadOnlyVector y) => x.Add(y);
		public static IMinimalVector operator -(IMinimalReadOnlyVector x, IMinimalReadOnlyVector y) => x.Subtract(y);
		public static double operator *(IMinimalReadOnlyVector x, IMinimalReadOnlyVector y) => x.DotProduct(y);
		public static IMinimalVector operator *(IMinimalReadOnlyVector x, double y) => x.Scale(y);
		public static IMinimalVector operator *(double y, IMinimalReadOnlyVector x) => x.Scale(y);
	}
}
