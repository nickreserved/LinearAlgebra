namespace MGroup.LinearAlgebra.Vectors
{
	using System;


	/// The minimal vector functionality for algorithms, which requires modifications to vector elements.
	public interface IExtendedMutableVector : IExtendedImmutableVector, IMinimalMutableVector
	{
		[Obsolete("Intention of this property, is for sparse vectors and it is highly inefficient. Please stop use it RIGHT NOW")]
		new double this[int index] { get; set; }
		double IExtendedImmutableVector.this[int index] => this[index];


		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector


		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// Any change in subvector view Elements, changes also corresponding Elements of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		new IExtendedMutableVector View(int fromIndex, int toIndex);
		IExtendedImmutableVector IExtendedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IMinimalMutableVector

		new IExtendedMutableVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient);
		IMinimalMutableVector IMinimalMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient);

		new IExtendedMutableVector AddIntoThis(IMinimalImmutableVector otherVector);
		IMinimalMutableVector IMinimalMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector);

		new IExtendedMutableVector SubtractIntoThis(IMinimalImmutableVector otherVector);
		IMinimalMutableVector IMinimalMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector);

		new IExtendedMutableVector NegativeIntoThis();
		IMinimalMutableVector IMinimalMutableVector.NegativeIntoThis() => NegativeIntoThis();

		new IExtendedMutableVector ScaleIntoThis(double coefficient);
		IMinimalMutableVector IMinimalMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);

		new IExtendedMutableVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		IMinimalMutableVector IMinimalMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		new IExtendedMutableVector CopyFrom(IMinimalImmutableVector otherVector);
		IMinimalMutableVector IMinimalMutableVector.CopyFrom(IMinimalImmutableVector otherVector) => CopyFrom(otherVector);

		new IExtendedMutableVector Clear();
		IMinimalMutableVector IMinimalMutableVector.Clear() => Clear();

		new IExtendedMutableVector SetAll(double value);
		IMinimalMutableVector IMinimalMutableVector.SetAll(double value) => SetAll(value);

		new IExtendedMutableVector DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IMinimalMutableVector IMinimalMutableVector.DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywiseIntoThis(otherVector, binaryOperation);

		new IExtendedMutableVector DoToAllEntriesIntoThis(Func<double, double> unaryOperation);
		IMinimalMutableVector IMinimalMutableVector.DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => DoToAllEntriesIntoThis(unaryOperation);



		// -------- OPERATORS FROM IMinimalImmutableVector

		public static IExtendedMutableVector operator -(IExtendedMutableVector x) => x.Negative();
		public static IExtendedMutableVector operator +(IExtendedMutableVector x, IExtendedMutableVector y) => x.Add(y);
		public static IExtendedMutableVector operator +(IExtendedMutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IExtendedMutableVector operator +(IMinimalImmutableVector y, IExtendedMutableVector x) => x.Add(y);
		public static IExtendedMutableVector operator -(IExtendedMutableVector x, IExtendedMutableVector y) => x.Subtract(y);
		public static IExtendedMutableVector operator -(IExtendedMutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static IExtendedMutableVector operator -(IMinimalImmutableVector y, IExtendedMutableVector x) => (x - y).NegativeIntoThis();
		public static double operator *(IExtendedMutableVector x, IExtendedMutableVector y) => x.DotProduct(y);
		public static double operator *(IExtendedMutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, IExtendedMutableVector y) => x.DotProduct(y);
		public static IExtendedMutableVector operator *(IExtendedMutableVector x, double y) => x.Scale(y);
		public static IExtendedMutableVector operator *(double y, IExtendedMutableVector x) => x.Scale(y);
	}
}
