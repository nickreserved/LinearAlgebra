namespace MGroup.LinearAlgebra.Providers.Managed
{
	using System;

	using MGroup.LinearAlgebra.Exceptions;

	public class StationaryIterationManagedProvider
	{
		public void CsrGaussSeidelBack(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] vIn, double[] vOut)
		{
			// Do not read the vector and each matrix row backward. It destroys caching. And in any case, we do csr_row * vector,
			// thus the order of operations for the dot product do not matter. What does matter is starting from the last row. 
			int n = matrixOrder;
			for (int i = n - 1; i >= 0; --i)
			{
				double sum = vIn[i];
				int rowStart = csrRowOffsets[i]; // inclusive
				int rowEnd = csrRowOffsets[i + 1]; // exclusive
				int diagOffset = diagOffsets[i];

				for (int k = rowStart; k < diagOffset; ++k)
				{
					sum -= csrValues[k] * vOut[csrColIndices[k]];
				}

				double diagEntry = csrValues[diagOffset];

				for (int k = diagOffset + 1; k < rowEnd; ++k)
				{
					sum -= csrValues[k] * vOut[csrColIndices[k]];
				}

				vOut[i] = sum / diagEntry;
			}
		}

		public void CsrGaussSeidelForward(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution)
		{
			int n = matrixOrder;
			for (int i = 0; i < n; ++i)
			{
				double sum = rhs[i];
				int rowStart = csrRowOffsets[i]; // inclusive
				int rowEnd = csrRowOffsets[i + 1]; // exclusive
				int diagOffset = diagOffsets[i];

				for (int k = rowStart; k < diagOffset; ++k)
				{
					sum -= csrValues[k] * solution[csrColIndices[k]];
				}

				double diagEntry = csrValues[diagOffset];

				for (int k = diagOffset + 1; k < rowEnd; ++k)
				{
					sum -= csrValues[k] * solution[csrColIndices[k]];
				}

				solution[i] = sum / diagEntry;
			}
		}

		public void CsrJacobi(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution, double[] work)
		{
			int n = matrixOrder;
			Array.Copy(solution, work, n);
			for (int i = 0; i < n; ++i)
			{
				double sum = rhs[i];
				int rowStart = csrRowOffsets[i]; // inclusive
				int rowEnd = csrRowOffsets[i + 1]; // exclusive
				int diagOffset = diagOffsets[i];

				for (int k = rowStart; k < diagOffset; ++k)
				{
					sum -= csrValues[k] * work[csrColIndices[k]];
				}

				double diagEntry = csrValues[diagOffset];

				for (int k = diagOffset + 1; k < rowEnd; ++k)
				{
					sum -= csrValues[k] * work[csrColIndices[k]];
				}

				solution[i] = sum / diagEntry;
			}
		}

		//TODO: Move to another provider class. This can be parallelized trivially E.g. 
		public int[] LocateDiagonalOffsetsCsr(int matrixOrder, int[] csrRowOffsets, int[] csrColIndices)
		{
			var diagonalOffsets = new int[matrixOrder];
			for (int i = 0; i < matrixOrder; ++i)
			{
				int rowStart = csrRowOffsets[i]; // inclusive
				int rowEnd = csrRowOffsets[i + 1]; // exclusive

				//TODO: optimizations: bisection, start from the end of the row if row > n/2, etc.
				bool hasZeroInDiagonal = true;
				for (int k = rowStart; k < rowEnd; ++k)
				{
					int j = csrColIndices[k];
					if (j == i)
					{
						diagonalOffsets[i] = k;
						hasZeroInDiagonal = false;
						break;
					}
				}

				if (hasZeroInDiagonal)
				{
					//TODO: Should this be necessary for every caller? Or provide another similar method, that works with structural 0s in diagonal?
					//		Are the offsets defined when there are structural 0s in the diagonal?
					throw new InvalidSparsityPatternException($"Found structural zero diagonal entry at ({i},{i}).");
				}
			}

			return diagonalOffsets;
		}
	}
}
