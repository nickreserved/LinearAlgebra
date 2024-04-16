//TODO: These should be delegated to C .dlls or to MKL if possible.
namespace MGroup.LinearAlgebra.Commons
{
	using MGroup.LinearAlgebra.Exceptions;

	/// <summary>
	/// Low level array operations for sparse matrix formats.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	internal class SparseArrays
	{
		internal static double[] LocateCsrDiagonal(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices)
		{
			var diagonal = new double[matrixOrder]; 
			for (int i = 0; i < matrixOrder; ++i)
			{
				int rowStart = csrRowOffsets[i]; // inclusive
				int rowEnd = csrRowOffsets[i + 1]; // exclusive

				//TODO: optimizations: bisection, start from the end of the row if row > n/2, etc.
				for (int k = rowStart; k < rowEnd; ++k)
				{
					int j = csrColIndices[k];
					if (j == i)
					{
						diagonal[i] = csrValues[k];
						break;
					}
					// If the diagonal entry is not explicitly stored, diagonal[i] will be 0, as it should.
				}
			}

			return diagonal;
		}
	}
}
