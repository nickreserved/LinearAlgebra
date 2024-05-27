namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	using MGroup.LinearAlgebra.Reduction;


	/// <summary>
	/// The minimal vector functionality, which not require any modification to vector.
	/// </summary>
	public interface IExtendedReadOnlyVector : IReadOnlyVector
	{
		/// <summary>
		/// Provides a vector from a contiguous part of this vector.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned vector</param>
		/// <returns>A subvector of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedVector Copy(int fromIndex, int toIndex);

		/// <summary>
		/// Return Values of this vector to a new array.
		/// </summary>
		/// <returns>A new array with the Values of vector</returns>
		double[] CopyToArray();
		protected static double[] CopyToArray(IExtendedReadOnlyVector thisVector) => thisVector.CopyToArray(0, thisVector.Length);

		/// <summary>
		/// Return Values from a contiguous part of this vector to a new array.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of returned vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of returned array</param>
		/// <returns>A new array with Values of this vector from range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		double[] CopyToArray(int fromIndex, int toIndex);
		protected static double[] CopyToArray(IExtendedReadOnlyVector thisVector, int fromIndex, int toIndex)
		{
			var result = new double[toIndex - fromIndex];
			thisVector.CopyToArray(fromIndex, result, 0, result.Length);
			return result;
		}

		/// <summary>
		/// Copy to an existed array, Values from a contiguous part of this vector.
		/// </summary>
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the <paramref name="arrayIndex"/> element of given <paramref name="array"/></param>
		/// <param name="array">An existing target array. Values of this vector from range [<paramref name="fromIndex"/>, <paramref name="fromIndex"/> + <paramref name="length"/>)
		/// will be written to target array in the range [<paramref name="arrayIndex"/>, <paramref name="arrayIndex"/> + <paramref name="length"/>)</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where Values of this vector will be written</param>
		/// <param name="length">Number of elements copied in target <paramref name="array"/></param>
		void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length);

		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view Values, changes also corresponding Values of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		IExtendedReadOnlyVector View(int fromIndex, int toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IReadOnlyVector

		new IExtendedVector Axpy(IReadOnlyVector otherVector, double otherCoefficient);
		IVector IReadOnlyVector.Axpy(IReadOnlyVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient);

		new IExtendedVector Add(IReadOnlyVector otherVector);
		IVector IReadOnlyVector.Add(IReadOnlyVector otherVector) => Add(otherVector);

		new IExtendedVector Subtract(IReadOnlyVector otherVector);
		IVector IReadOnlyVector.Subtract(IReadOnlyVector otherVector) => Subtract(otherVector);

		new IExtendedVector Negate();
		IVector IReadOnlyVector.Negate() => Negate();

		new IExtendedVector Scale(double coefficient);
		IVector IReadOnlyVector.Scale(double coefficient) => Scale(coefficient);

		new IExtendedVector LinearCombination(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient);
		IVector IReadOnlyVector.LinearCombination(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient);

		new IExtendedVector Copy();
		IVector IReadOnlyVector.Copy() => Copy();

		new IExtendedVector CreateZeroWithSameFormat();
		IVector IReadOnlyVector.CreateZeroWithSameFormat() => CreateZeroWithSameFormat();

		new IExtendedVector DoEntrywise(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation);
		IVector IReadOnlyVector.DoEntrywise(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation);

		new IExtendedVector DoToAllEntries(Func<double, double> unaryOperation);
		IVector IReadOnlyVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);



		// -------- OPERATORS FROM IReadOnlyVector

		public static IExtendedVector operator -(IExtendedReadOnlyVector x) => x.Negate();
		public static IExtendedVector operator +(IExtendedReadOnlyVector x, IExtendedReadOnlyVector y) => x.Add(y);
		public static IExtendedVector operator +(IExtendedReadOnlyVector x, IReadOnlyVector y) => x.Add(y);
		public static IExtendedVector operator +(IReadOnlyVector y, IExtendedReadOnlyVector x) => x.Add(y);
		public static IExtendedVector operator -(IExtendedReadOnlyVector x, IExtendedReadOnlyVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IExtendedReadOnlyVector x, IReadOnlyVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IReadOnlyVector x, IExtendedReadOnlyVector y) => (IExtendedVector) x.Subtract(y);
		public static double operator *(IExtendedReadOnlyVector x, IExtendedReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IExtendedReadOnlyVector x, IReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IReadOnlyVector x, IExtendedReadOnlyVector y) => x.DotProduct(y);
		public static IExtendedVector operator *(IExtendedReadOnlyVector x, double y) => x.Scale(y);
		public static IExtendedVector operator *(double y, IExtendedReadOnlyVector x) => x.Scale(y);
	}
}
