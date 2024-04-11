namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using MGroup.LinearAlgebra.Commons;

	/// <summary>
	/// The minimal vector functionality for algorithms, which require modifications to vector elements.
	/// </summary>
	public interface IMinimalVector : IMinimalReadOnlyVector
	{
		/// <summary>
		/// A row operation from Gauss elimination.
		/// thisVector += <paramref name="otherVector"/> * <paramref name="otherCoefficient"/>.
		/// </summary>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		void AxpyIntoThis(IMinimalReadOnlyVector otherVector, double otherCoefficient);


		/// <summary>
		/// Add <paramref name="otherVector"/> to this vector.
		/// </summary>
		/// thisVector += <paramref name="otherVector"/>
		/// <param name="otherVector">A vector to add to this vector</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		void AddIntoThis(IMinimalReadOnlyVector otherVector);

		protected static void AddIntoThis(IMinimalVector thisVector, IMinimalReadOnlyVector otherVector) => thisVector.AxpyIntoThis(otherVector, 1);


		/// <summary>
		/// Subtract <paramref name="otherVector"/> from this vector.
		/// </summary>
		/// thisVector -= <paramref name="otherVector"/>
		/// <param name="otherVector">A vector to subtract from this vector</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		void SubtractIntoThis(IMinimalReadOnlyVector otherVector);

		protected static void SubtractIntoThis(IMinimalVector thisVector, IMinimalReadOnlyVector otherVector) => thisVector.AxpyIntoThis(otherVector, -1);


		/// <summary>Negate this vector.</summary>
		/// thisVector = -thisVector.
		void NegateIntoThis();

		protected static void NegateIntoThis(IMinimalVector thisVector) => thisVector.ScaleIntoThis(-1);


		/// <summary>
		/// MultiplyIntoThis this vector with scalar <paramref name="coefficient"/>.
		/// </summary>
		/// thisVector *= <paramref name="coefficient"/>
		/// <param name="coefficient">A scalar to multiply this vector</param>
		void ScaleIntoThis(double coefficient);


		/// <summary>
		/// A linear combination between this and another one vector.
		/// </summary>
		/// thisVector = <paramref name="thisCoefficient"/> * thisVector + <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>.
		/// <param name="thisCoefficient">A scalar as coefficient to this vector</param>
		/// <param name="otherVector">A vector with the same number of elements with this vector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		void LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient);

		protected static void LinearCombinationIntoThis(IMinimalVector thisVector, double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient)
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
		}


		/// <summary>
		/// Copy elements from <paramref name="otherVector"/>.
		/// </summary>
		/// <param name="otherVector">The source vector from where the elements will by copied to this vector.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		void CopyFrom(IMinimalReadOnlyVector otherVector);


		/// <summary>
		/// Set all elements of vector to zero.
		/// </summary>
		/// In sparse vectors, it does not eliminate current stored (non-zero) elements. It just set them to 0.
		/// <returns>This vector</returns>
		void Clear();
		protected static void Clear(IMinimalVector thisVector) => thisVector.SetAll(0);

		/// <summary>
		/// Set all stored elements of vector to <paramref name="value"/>.
		/// </summary>
		/// In sparse vectors only the non-zero (stored) elements get that value. The non-stored (implied as zero) elements remain zero.
		/// <returns>This vector</returns>
		void SetAll(double value);

		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// this[i] = <paramref name="binaryOperation"/>(this[i], <paramref name="otherVector"/>[i]).
		/// The resulting vector overwrites the entries of this.
		/// </summary>
		/// <param name="otherVector">A vector with the same length as this.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// For instance try to set a non-stored element in a sparse vector.
		/// </exception>
		/// <returns>This vector.</returns>
		void DoEntrywiseIntoThis(IMinimalReadOnlyVector otherVector, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: this[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting vector overwrites the entries of this.
		/// In sparse vectors, this method applies only to the non-zero (stored) elements. The non-stored (implied as zero) elements remain zero.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		/// <returns>This vector.</returns>
		void DoToAllEntriesIntoThis(Func<double, double> unaryOperation);



		// -------- OPERATORS FROM IMinimalReadOnlyVector

		public static IMinimalVector operator -(IMinimalVector x) => x.Negate();
		public static IMinimalVector operator +(IMinimalVector x, IMinimalVector y) => x.Add(y);
		public static IMinimalVector operator +(IMinimalVector x, IMinimalReadOnlyVector y) => x.Add(y);
		public static IMinimalVector operator +(IMinimalReadOnlyVector y, IMinimalVector x) => x.Add(y);
		public static IMinimalVector operator -(IMinimalVector x, IMinimalVector y) => x.Subtract(y);
		public static IMinimalVector operator -(IMinimalVector x, IMinimalReadOnlyVector y) => x.Subtract(y);
		public static IMinimalVector operator -(IMinimalReadOnlyVector x, IMinimalVector y) => x.Subtract(y);
		public static double operator *(IMinimalVector x, IMinimalVector y) => x.DotProduct(y);
		public static double operator *(IMinimalVector x, IMinimalReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IMinimalReadOnlyVector x, IMinimalVector y) => x.DotProduct(y);
		public static IMinimalVector operator *(IMinimalVector x, double y) => x.Scale(y);
		public static IMinimalVector operator *(double y, IMinimalVector x) => x.Scale(y);
	}
}
