namespace MGroup.LinearAlgebra.Vectors
{
	using System;


	/// <summary>
	/// Interface for sparse vectors.
	/// </summary>
	/// TODO: Probably in the future, IFullyPopulated*Vector will not inherit from this interface.
	public interface INotFullyPopulatedImmutableVector : IExtendedImmutableVector
	{
		/// <summary>
		/// Returns the element at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the element to return.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector</exception>
		/// TODO: Is this needed for sparse vectors? Or better an enumerator of (index,value) and a LowerBound/UpperBound?
		double this[int index] { get; }



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector

		new INotFullyPopulatedMutableVector Copy(int fromIndex, int toIndex);
		IExtendedMutableVector IExtendedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex);

		new INotFullyPopulatedImmutableVector View(int fromIndex, int toIndex);
		IExtendedImmutableVector IExtendedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IMinimalImmutableVector

		new INotFullyPopulatedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient);

		new INotFullyPopulatedMutableVector Add(IMinimalImmutableVector otherVector);
		IExtendedMutableVector IExtendedImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector);

		new INotFullyPopulatedMutableVector Subtract(IMinimalImmutableVector otherVector);
		IExtendedMutableVector IExtendedImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector);

		new INotFullyPopulatedMutableVector Negative();
		IExtendedMutableVector IExtendedImmutableVector.Negative() => Negative();

		new INotFullyPopulatedMutableVector Scale(double coefficient);
		IExtendedMutableVector IExtendedImmutableVector.Scale(double coefficient) => Scale(coefficient);

		new INotFullyPopulatedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient);

		new INotFullyPopulatedMutableVector Copy();
		IExtendedMutableVector IExtendedImmutableVector.Copy() => Copy();

		new INotFullyPopulatedMutableVector CreateZero();
		IExtendedMutableVector IExtendedImmutableVector.CreateZero() => CreateZero();

		new INotFullyPopulatedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation);

		new INotFullyPopulatedMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);




		// -------- OPERATORS FROM IMinimalImmutableVector

		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedImmutableVector x) => x.Negative();
		public static INotFullyPopulatedMutableVector operator +(INotFullyPopulatedImmutableVector x, INotFullyPopulatedImmutableVector y) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator +(INotFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator +(IMinimalImmutableVector y, INotFullyPopulatedImmutableVector x) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedImmutableVector x, INotFullyPopulatedImmutableVector y) => x.Subtract(y);
		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static INotFullyPopulatedMutableVector operator -(IMinimalImmutableVector y, INotFullyPopulatedImmutableVector x) => (x - y).NegativeIntoThis();
		public static double operator *(INotFullyPopulatedImmutableVector x, INotFullyPopulatedImmutableVector y) => x.DotProduct(y);
		public static double operator *(INotFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, INotFullyPopulatedImmutableVector y) => x.DotProduct(y);
		public static INotFullyPopulatedMutableVector operator *(INotFullyPopulatedImmutableVector x, double y) => x.Scale(y);
		public static INotFullyPopulatedMutableVector operator *(double y, INotFullyPopulatedImmutableVector x) => x.Scale(y);
	}
}
