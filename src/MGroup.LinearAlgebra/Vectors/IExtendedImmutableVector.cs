namespace MGroup.LinearAlgebra.Vectors
{
	using System;


	/// <summary>
	/// The minimal vector functionality, which not require any modification to vector.
	/// </summary>
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
		/// Return Elements of this vector to a new array.
		/// </summary>
		/// <returns>A new array with the Elements of vector</returns>
		double[] CopyToArray();
		public static double[] CopyToArray(IExtendedImmutableVector thisVector) => thisVector.CopyToArray(0, thisVector.Length);

		/// <summary>
		/// Return Elements from a contiguous part of this vector to a new array.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned array</param>
		/// <returns>A new array with Elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		double[] CopyToArray(int fromIndex, int toIndex);
		public static double[] CopyToArray(IExtendedImmutableVector thisVector, int fromIndex, int toIndex)
		{
			var result = new double[toIndex - fromIndex];
			thisVector.CopyToArray(fromIndex, result, 0, result.Length);
			return result;
		}

		/// <summary>
		/// Copy to an existed array, Elements from a contiguous part of this vector.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the <paramref name="arrayIndex"/> element of given <paramref name="array"/></param>
		/// <param name="array">An existing target array. Elements of this vector from range [<paramref name="fromIndex"/>, <paramref name="fromIndex"/> + <paramref name="length"/>)
		/// will be written to target array in the range [<paramref name="arrayIndex"/>, <paramref name="arrayIndex"/> + <paramref name="length"/>)</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where Elements of this vector will be written</param>
		/// <param name="length">Number of elements copied in target <paramref name="array"/></param>
		void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length);

		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view Elements, changes also corresponding Elements of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedImmutableVector View(int fromIndex, int toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IMinimalImmutableVector

		new IExtendedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient);
		IMinimalMutableVector IMinimalImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient);

		new IExtendedMutableVector Add(IMinimalImmutableVector otherVector);
		IMinimalMutableVector IMinimalImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector);

		new IExtendedMutableVector Subtract(IMinimalImmutableVector otherVector);
		IMinimalMutableVector IMinimalImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector);

		new IExtendedMutableVector Negative();
		IMinimalMutableVector IMinimalImmutableVector.Negative() => Negative();

		new IExtendedMutableVector Scale(double coefficient);
		IMinimalMutableVector IMinimalImmutableVector.Scale(double coefficient) => Scale(coefficient);

		new IExtendedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		IMinimalMutableVector IMinimalImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient);

		new IExtendedMutableVector Copy();
		IMinimalMutableVector IMinimalImmutableVector.Copy() => Copy();

		new IExtendedMutableVector CreateZero();
		IMinimalMutableVector IMinimalImmutableVector.CreateZero() => CreateZero();

		new IExtendedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IMinimalMutableVector IMinimalImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation);

		new IExtendedMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		IMinimalMutableVector IMinimalImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);



		// -------- OPERATORS FROM IMinimalImmutableVector

		public static IExtendedMutableVector operator -(IExtendedImmutableVector x) => x.Negative();
		public static IExtendedMutableVector operator +(IExtendedImmutableVector x, IExtendedImmutableVector y) => x.Add(y);
		public static IExtendedMutableVector operator +(IExtendedImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IExtendedMutableVector operator +(IMinimalImmutableVector y, IExtendedImmutableVector x) => x.Add(y);
		public static IExtendedMutableVector operator -(IExtendedImmutableVector x, IExtendedImmutableVector y) => x.Subtract(y);
		public static IExtendedMutableVector operator -(IExtendedImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static IExtendedMutableVector operator -(IMinimalImmutableVector y, IExtendedImmutableVector x) => (x - y).NegativeIntoThis();
		public static double operator *(IExtendedImmutableVector x, IExtendedImmutableVector y) => x.DotProduct(y);
		public static double operator *(IExtendedImmutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, IExtendedImmutableVector y) => x.DotProduct(y);
		public static IExtendedMutableVector operator *(IExtendedImmutableVector x, double y) => x.Scale(y);
		public static IExtendedMutableVector operator *(double y, IExtendedImmutableVector x) => x.Scale(y);
	}
}
