namespace MGroup.LinearAlgebra.Vectors
{
	public class SparseVectorView : AbstractSparseVector
	{
		/// <summary>
		/// Create a sparse vector view.
		/// Usually a row or column vector of a CSR or CSC matrix.
		/// </summary>
		/// <param name="length">Number of all (zero and non-zero) elements of vector</param>
		/// <param name="values">An array with values of non-zero elements of vector.
		/// Only a part of this array corresponds to this vector view.
		/// Each element of <paramref name="values"/> array, makes a pair with the corresponding element of <paramref name="indices"/> array.
		/// Usually this is a CSR or CSC matrix array of values</param>
		/// <param name="indices">An array with indices of non-zero elements of vector.
		/// Only a part of this array corresponds to this vector view.
		/// Each element of <paramref name="values"/> array, makes a pair with the corresponding element of <paramref name="indices"/> array.
		/// Usually this is a CSR or CSC matrix array of indices</param>
		/// <param name="fromIndex">Vector view begins from this index (including) in <paramref name="indices"/> and <paramref name="values"/> arrays.
		/// Usually this is the index of row or column beginning in the CSR or CSC matrix array of values and indices.</param>
		/// <param name="toIndex">Vector view ends to this index (excluding) in <paramref name="indices"/> and <paramref name="values"/> arrays.
		/// Usually this is the index of row or column end in the CSR or CSC matrix array of values and indices.</param>
		public SparseVectorView(int length, double[] values, int[] indices, int fromIndex, int toIndex)
		{
			Length = length;
			Values = values;
			Indices = Indices;
			FromIndex = FromIndex;
			ToIndex = toIndex;
		}

		public override int FromIndex { get; }
		public override int ToIndex { get; }
	}
}
