namespace MGroup.LinearAlgebra.Vectors
{
	public interface IExtendedMutableVector : IExtendedImmutableVector, IMinimalMutableVector
	{
		/// <summary>
		/// Provides a view to a contiguous part of this vector.
		/// </summary>
		/// Any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="fromIndex">Element with index <paramref name="fromIndex"/>, of this vector, will be the first element of view vector</param>
		/// <param name="toIndex">Index <paramref name="toIndex"/>, of this vector, will be the last+1 element of view vector</param>
		/// <returns>A subvector view of this vector in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>)</returns>
		new IExtendedMutableVector View(int fromIndex, int toIndex);

		IExtendedImmutableVector IExtendedImmutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);

		/// <summary>
		/// Provides a scattered view to this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="indices">Element with that indices of this vector, form the returned vector view.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector view of this vector with elements for given indices</returns>
		new IExtendedMutableVector View(int[] indices);

		IExtendedImmutableVector IExtendedImmutableVector.View(int[] indices) => View(indices);
	}
}
