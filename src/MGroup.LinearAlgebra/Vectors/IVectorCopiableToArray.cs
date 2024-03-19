namespace MGroup.LinearAlgebra.Vectors
{
	public interface IVectorCopiableToArray
	{
		/// <summary>
		/// Create an array and copy all elements of this vector to that array.
		/// </summary>
		/// <returns>Array with the <c>Length()</c> elements from this vector</returns>
		double[] CopyToArray();

		/// <summary>
		/// Create an array and copy elements from this vector, in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>) to that array.
		/// </summary>
		/// <returns>Array with <c><paramref name="toIndex"/> - <paramref name="fromIndex"/></c> elements from this vector</returns>
		double[] CopyToArray(int fromIndex, int toIndex);

		/// <summary>
		/// Copy elements from this vector, in range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>) to <paramref name="array"/> starting from <paramref name="arrayIndex"/>.
		/// </summary>
		/// <param name="array">Size of array must be at least <c><paramref name="arrayIndex"/> + <paramref name="toIndex"/> - <paramref name="fromIndex"/></c></param>
		/// <param name="arrayIndex">Element with index <paramref name="fromIndex"/> in this vector, will be copied in element with index <paramref name="arrayIndex"/> in <paramref name="array"/></param>
		/// <param name="fromIndex"></param>
		/// <param name="toIndex"></param>
		void CopyToArray(double[] array, int arrayIndex, int fromIndex, int toIndex);
	}
}
