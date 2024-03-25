namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public interface IFullyPopulatedVector : INotFullyPopulatedMutableVector
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
	}
}
