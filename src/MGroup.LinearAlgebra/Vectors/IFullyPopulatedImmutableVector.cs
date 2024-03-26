namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface IFullyPopulatedImmutableVector : INotFullyPopulatedImmutableVector
	{
		// ------------------- COVARIANT RETURN TYPE FROM INotFullyPopulatedImmutableVector

		new IFullyPopulatedImmutableVector View(int fromIndex, int toIndex);
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new IFullyPopulatedImmutableVector View(int[] indices);
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Copy(int fromIndex, int toIndex);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Copy(int[] indices);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Copy(int[] indices) => Copy(indices); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().AxpyIntoThis(otherVector, otherCoefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Add(IMinimalImmutableVector otherVector); // => Axpy(otherVector, +1.0);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Subtract(IMinimalImmutableVector otherVector); // => Axpy(otherVector, -1.0);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Scale(double coefficient); // => Copy().ScaleIntoThis(coefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Scale(double coefficient) => Scale(coefficient); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient); // => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector Copy();
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.Copy() => Copy(); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector CreateZero();
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.CreateZero() => CreateZero(); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector DoToAllEntries(Func<double, double> unaryOperation);
		INotFullyPopulatedMutableVector INotFullyPopulatedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation); /*TODO: remove line when C#9*/



		// -------- OPERATORS / COVARIANT RETURN TYPE FROM IMinimalImmutableVector
		public static IFullyPopulatedMutableVector operator +(IFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Add(y);
		public static IFullyPopulatedMutableVector operator -(IFullyPopulatedImmutableVector x, IMinimalImmutableVector y) => x.Subtract(y);
		public static IFullyPopulatedMutableVector operator *(IFullyPopulatedImmutableVector x, double y) => x.Scale(y);
		public static IFullyPopulatedMutableVector operator *(double y, IFullyPopulatedImmutableVector x) => x.Scale(y);

	}
}
