namespace MGroup.LinearAlgebra.Providers.Managed
{
	using System;

	using MGroup.LinearAlgebra.Exceptions;

	public class StationaryIterationManagedProvider
	{
		public void CsrGaussSeidelBack(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution)
		{
			// Do not read the vector and each matrix row backward. It destroys caching. And in any case, we do csr_row * vector,
			// thus the order of operations for the dot product do not matter. What does matter is starting from the last row. 
			for (int i = matrixOrder - 1; i >= 0; --i)
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

		public void CsrGaussSeidelForward(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution)
		{
			for (int i = 0; i < matrixOrder; ++i)
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
			Array.Copy(solution, work, matrixOrder);
			for (int i = 0; i < matrixOrder; ++i)
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

		public void CsrSorBack(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution, double omega)
		{
			// Do not read the vector and each matrix row backward. It destroys caching. And in any case, we do csr_row * vector,
			// thus the order of operations for the dot product do not matter. What does matter is starting from the last row. 
			double oneMinusOmega = 1.0 - omega;
			for (int i = matrixOrder - 1; i >= 0; --i)
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

				solution[i] = oneMinusOmega * solution[i] + omega * sum / diagEntry;
			}
		}

		public void CsrSorForward(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] solution, double omega)
		{
			double oneMinusOmega = 1.0 - omega;
			for (int i = 0; i < matrixOrder; ++i)
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

				solution[i] = oneMinusOmega * solution[i] + omega * sum / diagEntry;
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
