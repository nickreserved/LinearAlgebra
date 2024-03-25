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
	}
}
