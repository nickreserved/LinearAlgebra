namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.SchurComplements.IntegerMatrices;

	public class SubmatrixExtractorCsrCsc : SubmatrixExtractorCsrBase
	{
		public CsrMatrix Submatrix00 { get; private set; }

		public override void Clear()
		{
			base.Clear();
			Submatrix00 = null;
		}

		public void ExtractSubmatrices(CsrMatrix originalMatrix, int[] indicesGroup0, int[] indicesGroup1)
		{
			if (this.originalMatrix == null)
			{
				ExtractMaps(originalMatrix, indicesGroup0, indicesGroup1);
				this.originalMatrix = originalMatrix;
				this.indicesGroup0 = indicesGroup0;
				this.indicesGroup1 = indicesGroup1;
			}
			else
			{
				if ((this.originalMatrix != originalMatrix)
					|| (this.indicesGroup0 != indicesGroup0)
					|| (this.indicesGroup1 != indicesGroup1))
				{
					throw new InvalidOperationException("The submatrix pattern stored corresponds to a different original matrix"
						+ " or to different subsets of rows/columns. Clear this object first");
				}
			}

			mapper00.CopyValuesArrayToSubmatrix(originalMatrix.RawValues, Submatrix00.RawValues);
			mapper01.CopyValuesArrayToSubmatrix(originalMatrix.RawValues, Submatrix01.RawValues);
			mapper10.CopyValuesArrayToSubmatrix(originalMatrix.RawValues, Submatrix10.RawValues);
			mapper11.CopyValuesArrayToSubmatrix(originalMatrix.RawValues, Submatrix11.RawValues);
		}

		private void ExtractMaps(CsrMatrix originalMatrix, int[] indicesGroup0, int[] indicesGroup1)
		{
			int n0 = indicesGroup0.Length;
			int n1 = indicesGroup1.Length;
			var submatrix00 = IntDokRowMajor.CreateZero(n0, n0);
			var submatrix01 = IntDokRowMajor.CreateZero(n0, n1);
			var submatrix10 = IntDokRowMajor.CreateZero(n1, n0);
			var submatrix11 = IntDokColMajor.CreateZero(n1, n1);

			// Original matrix indices to submatrix indices. Group 0 indices i0 are stored as: -i0-1.
			// Group 1 indices are stored as they are
			int[] originalToSubIndices = SubmatrixExtractorCscSymBase.MapOriginalToSubmatrixIndices(
				originalMatrix.NumRows, indicesGroup0, indicesGroup1);

			// Iterate the non zero values array of the original sparse matrix
			for (int i = 0; i < originalMatrix.NumColumns; ++i)
			{
				int start = originalMatrix.RawRowOffsets[i];
				int end = originalMatrix.RawRowOffsets[i + 1];
				int subI = originalToSubIndices[i];
				if (subI < 0) // i belongs to group 0 indices
				{
					subI = -subI - 1; // Mapping to [0, oo) for group 0 indices
					for (int t = start; t < end; ++t)
					{
						int j = originalMatrix.RawColIndices[t];
						int subJ = originalToSubIndices[j];
						if (subJ < 0) // (i, j) belongs to submatrix A00
						{
							subJ = -subJ - 1; // Mapping to [0, oo) for group 0 indices
							submatrix00[subI, subJ] = t;
						}
						else // (i, j) belongs to submatrix A01
						{
							submatrix01[subI, subJ] = t;
						}
					}
				}
				else // i belongs to group 1 indices
				{
					for (int t = start; t < end; ++t)
					{
						int j = originalMatrix.RawColIndices[t];
						int subJ = originalToSubIndices[j];
						if (subJ < 0) // (i, j) belongs to submatrix A10
						{
							subJ = -subJ - 1; // Mapping to [0, oo) for group 0 indices
							submatrix10[subI, subJ] = t;
						}
						else // (i, j) belongs to submatrix A11
						{
							submatrix11[subI, subJ] = t;
						}
					}
				}
			}

			// Finalize the data structures required to represent the submatrices
			// A00 CSR
			(int[] submatrixToOriginalValues00, int[] colIndices00, int[] rowOffsets00) = submatrix00.BuildCsrArrays();
			this.Submatrix00 = CsrMatrix.CreateFromArrays(
				n0, n0, new double[colIndices00.Length], colIndices00, rowOffsets00, false);
			this.mapper00 = new SameSparsityValuesArrayMapper(submatrixToOriginalValues00);

			// A01 CSR
			(int[] submatrixToOriginalValues01, int[] colIndices01, int[] rowOffsets01) = submatrix01.BuildCsrArrays();
			this.Submatrix01 = CsrMatrix.CreateFromArrays(
				n0, n1, new double[colIndices01.Length], colIndices01, rowOffsets01, false);
			this.mapper01 = new SameSparsityValuesArrayMapper(submatrixToOriginalValues01);

			// A10 CSR
			(int[] submatrixToOriginalValues10, int[] colIndices10, int[] rowOffsets10) = submatrix10.BuildCsrArrays();
			this.Submatrix10 = CsrMatrix.CreateFromArrays(
				n1, n0, new double[colIndices10.Length], colIndices10, rowOffsets10, false);
			this.mapper10 = new SameSparsityValuesArrayMapper(submatrixToOriginalValues10);

			// A11 CSC upper triangle
			(int[] submatrixToOriginalValues11, int[] rowIndices11, int[] colOffsets11) = submatrix11.BuildCscArrays();
			this.Submatrix11 = CscMatrix.CreateFromArrays(
				n1, n1, new double[rowIndices11.Length], rowIndices11, colOffsets11, false);
			this.mapper11 = new SameSparsityValuesArrayMapper(submatrixToOriginalValues11);
		}
	}
}
