namespace MGroup.LinearAlgebra.Vectors
{
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
		/// Provides a vector from scattered Elements of this vector.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned vector.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector from this vector with Elements for given indices</returns>
		IExtendedMutableVector Copy(int[] indices);


		/// <summary>
		/// Return Elements of this vector to a new array.
		/// </summary>
		/// <returns>A new array with the Elements of vector</returns>
		double[] CopyToArray();

		/// <summary>
		/// Return Elements from a contiguous part of this vector to a new array.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned array</param>
		/// <returns>A new array with Elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		double[] CopyToArray(int fromIndex, int toIndex);

		/// <summary>
		/// Copy to an existed array, Elements from a contiguous part of this vector.
		/// </summary>
		/// <param name="array">An existing target array. Elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)
		/// will be written to target array in the range [<paramref name="arrayIndex"/>, <paramref name="arrayIndex"/> + <paramref name="toIndex"/> - <paramref name="fromIndex"/>)</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where Elements of this vector will be written</param>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the <paramref name="arrayIndex"/> element of given <paramref name="array"/></param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the first element not written in target array</param>
		void CopyToArray(double[] array, int arrayIndex, int fromIndex, int toIndex);

		/// <summary>
		/// Return scattered Elements from this vector to a new array.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A new array with Elements from this vector for given indices</returns>
		double[] CopyToArray(int[] indices);

		/// <summary>
		/// Copy to an existed array, scattered Elements from this vector to a new array.
		/// </summary>
		/// <param name="array">An existing target array.</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where Elements of this vector will be written</param>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		void CopyToArray(double[] array, int arrayIndex, int[] indices);


		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view Elements, changes also corresponding Elements of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedImmutableVector View(int fromIndex, int toIndex);

		/// <summary>
		/// Provides a scattered view to this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view Elements, changes also corresponding Elements of this vector.
		/// <param name="indices">Element with that indices of this vector, form the returned vector view.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector view of this vector with Elements for given indices</returns>
		IExtendedImmutableVector View(int[] indices);




		// ------------------- COVARIANT RETURN TYPE FROM IMinimalImmutableVector

		new IExtendedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().AxpyIntoThis(otherVector, otherCoefficient);
		IMinimalMutableVector IMinimalImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new IExtendedMutableVector Add(IMinimalImmutableVector otherVector); // => Axpy(otherVector, +1.0);
		IMinimalMutableVector IMinimalImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector); /*TODO: remove line when C#9*/

		new IExtendedMutableVector Subtract(IMinimalImmutableVector otherVector); // => Axpy(otherVector, -1.0);
		IMinimalMutableVector IMinimalImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector); /*TODO: remove line when C#9*/

		new IExtendedMutableVector Scale(double coefficient); // => Copy().ScaleIntoThis(coefficient);
		IMinimalMutableVector IMinimalImmutableVector.Scale(double coefficient) => Scale(coefficient); /*TODO: remove line when C#9*/

		new IExtendedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		IMinimalMutableVector IMinimalImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new IExtendedMutableVector Copy();
		IMinimalMutableVector IMinimalImmutableVector.Copy() => Copy(); /*TODO: remove line when C#9*/

		new IExtendedMutableVector CreateZero();
		IMinimalMutableVector IMinimalImmutableVector.CreateZero() => CreateZero(); /*TODO: remove line when C#9*/

		new IExtendedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IMinimalMutableVector IMinimalImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation); /*TODO: remove line when C#9*/

		new IExtendedMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		IMinimalMutableVector IMinimalImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation); /*TODO: remove line when C#9*/



		// -------- OPERATORS / COVARIANT RETURN TYPE FROM IMinimalImmutableVector
		public static IExtendedMutableVector operator +(IExtendedImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IExtendedMutableVector operator -(IExtendedImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static IExtendedMutableVector operator *(IExtendedImmutableVector x, double y) => x.Scale(y);
		public static IExtendedMutableVector operator *(double y, IExtendedImmutableVector x) => x.Scale(y);
	}
}
