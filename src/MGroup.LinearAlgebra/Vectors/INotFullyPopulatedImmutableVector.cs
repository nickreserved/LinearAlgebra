namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface INotFullyPopulatedImmutableVector : IExtendedImmutableVector
	{
		/// <summary>
		/// Returns the element at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the element to return.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector</exception>
		double this[int index] { get; }





		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector


		new INotFullyPopulatedMutableVector Copy(int fromIndex, int toIndex);
		IExtendedMutableVector IExtendedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Copy(int[] indices);
		IExtendedMutableVector IExtendedImmutableVector.Copy(int[] indices) => Copy(indices); /*TODO: remove line when C#9*/

		new INotFullyPopulatedImmutableVector View(int fromIndex, int toIndex);
		IExtendedImmutableVector IExtendedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new INotFullyPopulatedImmutableVector View(int[] indices);
		IExtendedImmutableVector IExtendedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().AxpyIntoThis(otherVector, otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Add(IMinimalImmutableVector otherVector); // => Axpy(otherVector, +1.0);
		IExtendedMutableVector IExtendedImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Subtract(IMinimalImmutableVector otherVector); // => Axpy(otherVector, -1.0);
		IExtendedMutableVector IExtendedImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Scale(double coefficient); // => Copy().ScaleIntoThis(coefficient);
		IExtendedMutableVector IExtendedImmutableVector.Scale(double coefficient) => Scale(coefficient); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector Copy();
		IExtendedMutableVector IExtendedImmutableVector.Copy() => Copy(); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector CreateZero();
		IExtendedMutableVector IExtendedImmutableVector.CreateZero() => CreateZero(); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation); /*TODO: remove line when C#9*/



		// -------- OPERATORS / COVARIANT RETURN TYPE FROM IExtendedImmutableVector
		public static INotFullyPopulatedMutableVector operator +(INotFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static INotFullyPopulatedMutableVector operator -(INotFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static INotFullyPopulatedMutableVector operator *(INotFullyPopulatedImmutableVector x, double y) => x.Scale(y);
		public static INotFullyPopulatedMutableVector operator *(double y, INotFullyPopulatedImmutableVector x) => x.Scale(y);

	}
}
