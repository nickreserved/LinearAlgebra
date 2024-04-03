namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using MGroup.LinearAlgebra.Commons;

	/// <summary>
	/// The minimal vector functionality for algorithms, which require modifications to vector elements.
	/// </summary>
	public interface IMinimalMutableVector : IMinimalImmutableVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// Adds to this vector the <paramref name="otherVector"/> * <paramref name="otherCoefficient"/> and returns the this vector after that.
		/// </summary>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns><c>thisVector += <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></c></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		IMinimalMutableVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient);


		/// <summary>
		/// Add <paramref name="otherVector"/> to this vector and return this vector.
		/// </summary>
		/// <param name="otherVector">A vector to add to this vector</param>
		/// <returns>This vector</returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		IMinimalMutableVector AddIntoThis(IMinimalImmutableVector otherVector);

		protected static IMinimalMutableVector AddIntoThis(IMinimalMutableVector thisVector, IMinimalImmutableVector otherVector) => thisVector.AxpyIntoThis(otherVector, 1);


		/// <summary>
		/// Subtract <paramref name="otherVector"/> from this vector and return this vector.
		/// </summary>
		/// <param name="otherVector">A vector to subtract from this vector</param>
		/// <returns>This vector</returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		IMinimalMutableVector SubtractIntoThis(IMinimalImmutableVector otherVector);

		protected static IMinimalMutableVector SubtractIntoThis(IMinimalMutableVector thisVector, IMinimalImmutableVector otherVector) => thisVector.AxpyIntoThis(otherVector, -1);


		/// <summary>
		/// Negative this vector and return this vector
		/// </summary>
		/// <returns>This vector negative</returns>
		IMinimalMutableVector NegativeIntoThis();

		protected static IMinimalMutableVector NegativeIntoThis(IMinimalMutableVector thisVector) => thisVector.ScaleIntoThis(-1);


		/// <summary>
		/// MultiplyIntoThis this vector with scalar <paramref name="coefficient"/> and return this vector.
		/// </summary>
		/// <param name="coefficient">A scalar to multiply this vector</param>
		/// <returns>This vector</returns>
		IMinimalMutableVector ScaleIntoThis(double coefficient);


		/// <summary>
		/// A linear combination between this and another one vector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this vector</param>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns><c>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></c></returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		IMinimalMutableVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);

		protected static IMinimalMutableVector LinearCombinationIntoThis(IMinimalMutableVector thisVector, double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(thisVector, otherVector);
			if (thisCoefficient == 0)
			{
				if (otherCoefficient == 0)
					thisVector.Clear();
				else
				{
					thisVector.CopyFrom(otherVector);
					if (otherCoefficient != 1)
						thisVector.ScaleIntoThis(otherCoefficient);
				}
			}
			else
			{
				if (thisCoefficient != 1)
					thisVector.ScaleIntoThis(thisCoefficient);
				thisVector.AxpyIntoThis(otherVector, otherCoefficient);
			}
			return thisVector;
		}


		/// <summary>
		/// Copy elements from <paramref name="otherVector"/>
		/// </summary>
		/// <param name="otherVector">The source vector from where the elements will by copied to this vector</param>
		/// <returns>This vector</returns>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		IMinimalMutableVector CopyFrom(IMinimalImmutableVector otherVector);


		/// <summary>
		/// Set all elements of vector to zero.
		/// </summary>
		/// In sparse vectors, it does not eliminate current stored (non-zero) elements. It just set them to 0.
		/// <returns>This vector</returns>
		IMinimalMutableVector Clear();
		protected static IMinimalMutableVector Clear(IMinimalMutableVector thisVector) => thisVector.SetAll(0);

		/// <summary>
		/// Set all stored elements of vector to <paramref name="value"/>.
		/// </summary>
		/// In sparse vectors only the non-zero (stored) elements get that value. The non-stored (implied as zero) elements remain zero.
		/// <returns>This vector</returns>
		IMinimalMutableVector SetAll(double value);

		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// this[i] = <paramref name="binaryOperation"/>(this[i], <paramref name="otherVector"/>[i]). 
		/// The resulting vector overwrites the entries of this.
		/// </summary>
		/// <param name="otherVector">A vector with the same length as this.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalImmutableVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception> 
		/// <returns>This vector</returns>
		IMinimalMutableVector DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: this[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting vector overwrites the entries of this.
		/// </summary>
		/// In sparse vectors, this method applies only to the non-zero (stored) elements. The non-stored (implied as zero) elements remain zero.
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		/// <returns>This vector</returns>
		IMinimalMutableVector DoToAllEntriesIntoThis(Func<double, double> unaryOperation);



		// -------- OPERATORS FROM IMinimalImmutableVector

		public static IMinimalMutableVector operator -(IMinimalMutableVector x) => x.Negative();
		public static IMinimalMutableVector operator +(IMinimalMutableVector x, IMinimalMutableVector y) => x.Add(y);
		public static IMinimalMutableVector operator +(IMinimalMutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IMinimalMutableVector operator +(IMinimalImmutableVector y, IMinimalMutableVector x) => x.Add(y);
		public static IMinimalMutableVector operator -(IMinimalMutableVector x, IMinimalMutableVector y) => x.Subtract(y);
		public static IMinimalMutableVector operator -(IMinimalMutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static IMinimalMutableVector operator -(IMinimalImmutableVector y, IMinimalMutableVector x) => (x - y).NegativeIntoThis();
		public static double operator *(IMinimalMutableVector x, IMinimalMutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalMutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, IMinimalMutableVector y) => x.DotProduct(y);
		public static IMinimalMutableVector operator *(IMinimalMutableVector x, double y) => x.Scale(y);
		public static IMinimalMutableVector operator *(double y, IMinimalMutableVector x) => x.Scale(y);
	}
}
