namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Runtime.CompilerServices;


	/// The minimal vector functionality for algorithms, which requires modifications to vector elements.
	public interface IExtendedVector : IExtendedReadOnlyVector, IVector
	{
		// ------------------- COVARIANT RETURN TYPE FROM IExtendedReadOnlyVector


		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// Any change in subvector view Values, changes also corresponding Values of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		new IExtendedVector View(int fromIndex, int toIndex);
		IExtendedReadOnlyVector IExtendedReadOnlyVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// -------- OPERATORS FROM IReadOnlyVector

		public static IExtendedVector operator -(IExtendedVector x) => x.Negate();
		public static IExtendedVector operator +(IExtendedVector x, IExtendedVector y) => x.Add(y);
		public static IExtendedVector operator +(IExtendedVector x, IReadOnlyVector y) => x.Add(y);
		public static IExtendedVector operator +(IReadOnlyVector y, IExtendedVector x) => x.Add(y);
		public static IExtendedVector operator -(IExtendedVector x, IExtendedVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IExtendedVector x, IReadOnlyVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IReadOnlyVector x, IExtendedVector y) => (IExtendedVector) x.Subtract(y);
		public static double operator *(IExtendedVector x, IExtendedVector y) => x.DotProduct(y);
		public static double operator *(IExtendedVector x, IReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IReadOnlyVector x, IExtendedVector y) => x.DotProduct(y);
		public static IExtendedVector operator *(IExtendedVector x, double y) => x.Scale(y);
		public static IExtendedVector operator *(double y, IExtendedVector x) => x.Scale(y);
	}
}
