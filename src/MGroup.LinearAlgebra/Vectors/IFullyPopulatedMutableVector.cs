namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface IFullyPopulatedMutableVector : IFullyPopulatedImmutableVector, INotFullyPopulatedMutableVector
	{
		/// <summary>
		/// Returns a reference to the element at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the element to return.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector</exception>
		new ref double this[int index] { get; }


		double INotFullyPopulatedImmutableVector.this[int index] => this[index];
		void INotFullyPopulatedMutableVector.set(int index, double value) => this[index] = value;
		void INotFullyPopulatedMutableVector.add(int index, double value) => this[index] += value;


		// ------------------- COVARIANT RETURN TYPE FROM IFullyPopulatedImmutableVector

		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/


		// ------------------- COVARIANT RETURN TYPE FROM IFullyPopulatedImmutableVector

		IFullyPopulatedImmutableVector IFullyPopulatedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/
		IFullyPopulatedImmutableVector IFullyPopulatedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/


		// ------------------- COVARIANT RETURN TYPE FROM INotFullyPopulatedMutableVector

		new IFullyPopulatedMutableVector View(int fromIndex, int toIndex);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector View(int[] indices);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector AddIntoThis(IMinimalImmutableVector otherVector); // => AxpyIntoThis(otherVector, +1.0);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector SubtractIntoThis(IMinimalImmutableVector otherVector); // => AxpyIntoThis(otherVector, -1.0);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector ScaleIntoThis(double coefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient); /*TODO: remove line when C#9*/

		new IFullyPopulatedMutableVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		INotFullyPopulatedMutableVector INotFullyPopulatedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/
	}
}
