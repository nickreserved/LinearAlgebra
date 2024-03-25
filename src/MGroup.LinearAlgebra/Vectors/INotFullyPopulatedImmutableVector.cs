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
	}
}
