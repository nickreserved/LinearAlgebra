namespace MGroup.LinearAlgebra.Vectors
{
	using MGroup.LinearAlgebra.Matrices;
	using System;

	public interface IExtendedImmutableVector : IMinimalImmutableVector
	{
		/// <summary>
		/// Provides a vector from a contiguous part of this vector.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned vector</param>
		/// <returns>A subvector of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedMutableVector Copy(int fromIndex, int toIndex);

		/// <summary>
		/// Provides a vector from scattered elements of this vector.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned vector.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector from this vector with elements for given indices</returns>
		IExtendedMutableVector Copy(int[] indices);


		/// <summary>
		/// Return elements of this vector to a new array.
		/// </summary>
		/// <returns>A new array with the elements of vector</returns>
		double[] CopyToArray();

		/// <summary>
		/// Return elements from a contiguous part of this vector to a new array.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned array</param>
		/// <returns>A new array with elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		double[] CopyToArray(int fromIndex, int toIndex);

		/// <summary>
		/// Copy to an existed array, elements from a contiguous part of this vector.
		/// </summary>
		/// <param name="array">An existing target array. Elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)
		/// will be written to target array in the range [<paramref name="arrayIndex"/>, <paramref name="arrayIndex"/> + <paramref name="toIndex"/> - <paramref name="fromIndex"/>)</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where elements of this vector will be written</param>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the <paramref name="arrayIndex"/> element of given <paramref name="array"/></param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the first element not written in target array</param>
		void CopyToArray(double[] array, int arrayIndex, int fromIndex, int toIndex);

		/// <summary>
		/// Return scattered elements from this vector to a new array.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A new array with elements from this vector for given indices</returns>
		double[] CopyToArray(int[] indices);

		/// <summary>
		/// Copy to an existed array, scattered elements from this vector to a new array.
		/// </summary>
		/// <param name="array">An existing target array.</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where elements of this vector will be written</param>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		void CopyToArray(double[] array, int arrayIndex, int[] indices);


		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedImmutableVector View(int fromIndex, int toIndex);

		/// <summary>
		/// Provides a scattered view to this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="indices">Element with that indices of this vector, form the returned vector view.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector view of this vector with elements for given indices</returns>
		IExtendedImmutableVector View(int[] indices);






		/// <summary>
		/// A row operation from Gauss elimination.
		/// </summary>
		/// <param name="otherVector">A otherVector with the same number of elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IExtendedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient)
			=> (IExtendedMutableVector) IMinimalImmutableVector.Axpy(otherVector, otherCoefficient);

		IMinimalMutableVector Add(IMinimalImmutableVector otherVector)
			=> Axpy(otherVector, +1.0);

		IMinimalMutableVector Subtract(IMinimalImmutableVector otherVector)
			=> Axpy(otherVector, -1.0);

		double DotProduct(IMinimalImmutableVector otherVector);

		/// <summary>
		/// Returns the square of this otherVector <c>this * this</c>
		/// </summary>
		double Square()
			=> DotProduct(this);

		IMinimalMutableVector Scale(double coefficient)
			=> Copy().ScaleIntoThis(coefficient);

		/// <summary>
		/// A linear combination between this and another one otherVector.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherVector</param>
		/// <param name="otherVector">A otherVector with the same number of elements with this otherVector</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherVector"/></param>
		/// <returns>thisVector * <paramref name="thisCoefficient"/> + <paramref name="otherVector"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
			=> Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);


		/// <summary>
		/// Number of elements in vector.
		/// </summary>
		int Length { get; }

		double Norm2() => Math.Sqrt(Square());

		IMinimalMutableVector Copy()
		{
			IMinimalMutableVector copy = CreateZero();
			copy.CopyFrom(this);
			return copy;
		}

		/// <summary>
		/// Creates a new otherVector with all elements set to zero, the same number of elements with this otherVector and probably with the same format with this otherVector.
		/// </summary>
		/// <returns>A new zero otherVector with the same number of elements with this otherVector</returns>
		IMinimalMutableVector CreateZero();

		/// <summary>
		/// Check if this otherVector and <paramref name="otherVector"/> are almost equal.
		/// </summary>
		/// <param name="otherVector">A otherVector of any number of elements</param>
		/// <param name="tolerance">The maximum difference between corresponding elements to considered equal</param>
		/// <returns>True if both vectors are almost equal</returns>
		bool Equals(IMinimalImmutableVector otherVector, double tolerance = 1e-7);



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
		IMinimalMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i]).
		/// The resulting otherVector is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		IMinimalMutableVector DoToAllEntries(Func<double, double> unaryOperation);



		// -------- OPERATORS
		public static IMinimalMutableVector operator +(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IMinimalMutableVector operator -(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static double operator *(IMinimalImmutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static IMinimalMutableVector operator *(IMinimalImmutableVector x, double y) => x.Scale(y);
		public static IMinimalMutableVector operator *(double y, IMinimalImmutableVector x) => x.Scale(y);
	}
}
}
