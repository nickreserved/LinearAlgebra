namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface INotFullyPopulatedMutableVector : INotFullyPopulatedImmutableVector, IExtendedMutableVector
	{
		/// <summary>
		/// Set <paramref name="value"/> to element at <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index of element to set</param>
		/// <param name="value">New value for element</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector,
		/// or element at that <paramref name="index"/> is not stored in internal structure of vector.</exception>
		void set(int index, double value);

		/// <summary>
		/// Add <paramref name="value"/> to element at <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index of element to add <paramref name="value"/></param>
		/// <param name="value">Value to add in specified element</param>
		/// <returns>true if element addition was successfull (if element is stored)</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector,
		/// or element at that <paramref name="index"/> is not stored in internal structure of vector.</exception>
		void add(int index, double value);



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector
		IExtendedImmutableVector IExtendedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/
		
		IExtendedImmutableVector IExtendedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/



		// ------------------- COVARIANT RETURN TYPE FROM INotFullyPopulatedImmutableVector

		new INotFullyPopulatedMutableVector View(int fromIndex, int toIndex);
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector View(int[] indices);
		INotFullyPopulatedImmutableVector INotFullyPopulatedImmutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/




		// ------------------- COVARIANT RETURN TYPE FROM IExtendedMutableVector

		IExtendedMutableVector IExtendedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/
		IExtendedMutableVector IExtendedMutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector AddIntoThis(IMinimalImmutableVector otherVector); // => AxpyIntoThis(otherVector, +1.0);
		IExtendedMutableVector IExtendedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector SubtractIntoThis(IMinimalImmutableVector otherVector); // => AxpyIntoThis(otherVector, -1.0);
		IExtendedMutableVector IExtendedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector ScaleIntoThis(double coefficient);
		IExtendedMutableVector IExtendedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient); /*TODO: remove line when C#9*/

		new INotFullyPopulatedMutableVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient);
		IExtendedMutableVector IExtendedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/
	}
}
