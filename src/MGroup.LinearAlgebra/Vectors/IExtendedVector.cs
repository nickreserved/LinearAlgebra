namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Runtime.CompilerServices;


	/// The minimal vector functionality for algorithms, which requires modifications to vector elements.
	public interface IExtendedVector : IExtendedReadOnlyVector, IMinimalVector
	{
		[Obsolete("This property is EXTREMELY inefficient on sparce vectors")]
		new double this[int index] { get; set; }
		double IExtendedReadOnlyVector.this[int index] => this[index];


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



		// -------- OPERATORS FROM IMinimalReadOnlyVector

		public static IExtendedVector operator -(IExtendedVector x) => x.Negate();
		public static IExtendedVector operator +(IExtendedVector x, IExtendedVector y) => x.Add(y);
		public static IExtendedVector operator +(IExtendedVector x, IMinimalReadOnlyVector y) => x.Add(y);
		public static IExtendedVector operator +(IMinimalReadOnlyVector y, IExtendedVector x) => x.Add(y);
		public static IExtendedVector operator -(IExtendedVector x, IExtendedVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IExtendedVector x, IMinimalReadOnlyVector y) => x.Subtract(y);
		public static IExtendedVector operator -(IMinimalReadOnlyVector x, IExtendedVector y) => (IExtendedVector) x.Subtract(y);
		public static double operator *(IExtendedVector x, IExtendedVector y) => x.DotProduct(y);
		public static double operator *(IExtendedVector x, IMinimalReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IMinimalReadOnlyVector x, IExtendedVector y) => x.DotProduct(y);
		public static IExtendedVector operator *(IExtendedVector x, double y) => x.Scale(y);
		public static IExtendedVector operator *(double y, IExtendedVector x) => x.Scale(y);
	}
}
