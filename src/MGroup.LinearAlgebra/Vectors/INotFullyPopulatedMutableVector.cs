namespace MGroup.LinearAlgebra.Vectors
{
	using System;


	/// <summary>
	/// Interface for sparse vectors.
	/// </summary>
	/// TODO: Probably in the future, IFullyPopulatedMutableVector will not inherit from this interface.
	public interface INotFullyPopulatedMutableVector : INotFullyPopulatedImmutableVector, IExtendedMutableVector
	{
		/// <summary>
		/// Set <paramref name="value"/> to element at <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index of element to set</param>
		/// <param name="value">New value for element</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector,
		/// or element at that <paramref name="index"/> is not stored in internal structure of vector.</exception>
		[Obsolete("This method is inefficient - don't use it!")]
		void set(int index, double value);

		/// <summary>
		/// Add <paramref name="value"/> to element at <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index of element to add <paramref name="value"/></param>
		/// <param name="value">Value to add in specified element</param>
		/// <returns>true if element addition was successfull (if element is stored)</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector,
		/// or element at that <paramref name="index"/> is not stored in internal structure of vector.</exception>
		[Obsolete("This method is inefficient - don't use it!")]
		void add(int index, double value);



		// ------------------- COVARIANT RETURN TYPE FROM BOTH INotFullyPopulatedImmutableVector AND IExtendedMutableVector

		new INotFullyPopulatedMutableVector View(int fromIndex, int toIndex);
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);
		IExtendedMutableVector IExtendedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedMutableVector

		new INotFullyPopulatedMutableVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient);

		new INotFullyPopulatedMutableVector AddIntoThis(IMinimalImmutableVector otherVector);
		IExtendedMutableVector IExtendedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector);

		new INotFullyPopulatedMutableVector SubtractIntoThis(IMinimalImmutableVector otherVector);
		IExtendedMutableVector IExtendedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector);

		new INotFullyPopulatedMutableVector NegativeIntoThis();
		IExtendedMutableVector IExtendedMutableVector.NegativeIntoThis() => NegativeIntoThis();

		new INotFullyPopulatedMutableVector ScaleIntoThis(double coefficient);
		IExtendedMutableVector IExtendedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);

		new INotFullyPopulatedMutableVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		new INotFullyPopulatedMutableVector CopyFrom(IMinimalImmutableVector otherVector);
		IExtendedMutableVector IExtendedMutableVector.CopyFrom(IMinimalImmutableVector otherVector) => CopyFrom(otherVector);

		new INotFullyPopulatedMutableVector Clear();
		IExtendedMutableVector IExtendedMutableVector.Clear() => Clear();

		new INotFullyPopulatedMutableVector SetAll(double value);
		IExtendedMutableVector IExtendedMutableVector.SetAll(double value) => SetAll(value);

		new INotFullyPopulatedMutableVector DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IExtendedMutableVector IExtendedMutableVector.DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywiseIntoThis(otherVector, binaryOperation);

		new INotFullyPopulatedMutableVector DoToAllEntriesIntoThis(Func<double, double> unaryOperation);
		IExtendedMutableVector IExtendedMutableVector.DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => DoToAllEntriesIntoThis(unaryOperation);




		// -------- OPERATORS FROM IMinimalImmutableVector

		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedMutableVector x) => x.Negative();
		public static INotFullyPopulatedMutableVector operator +(INotFullyPopulatedMutableVector x, INotFullyPopulatedMutableVector y) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator +(INotFullyPopulatedMutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator +(IMinimalImmutableVector y, INotFullyPopulatedMutableVector x) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedMutableVector x, INotFullyPopulatedMutableVector y) => x.Subtract(y);
		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedMutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static INotFullyPopulatedMutableVector operator -(IMinimalImmutableVector y, INotFullyPopulatedMutableVector x) => (x - y).NegativeIntoThis();
		public static double operator *(INotFullyPopulatedMutableVector x, INotFullyPopulatedMutableVector y) => x.DotProduct(y);
		public static double operator *(INotFullyPopulatedMutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, INotFullyPopulatedMutableVector y) => x.DotProduct(y);
		public static INotFullyPopulatedMutableVector operator *(INotFullyPopulatedMutableVector x, double y) => x.Scale(y);
		public static INotFullyPopulatedMutableVector operator *(double y, INotFullyPopulatedMutableVector x) => x.Scale(y);
	}
}
