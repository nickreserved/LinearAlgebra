namespace MGroup.LinearAlgebra.SchurComplements
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Triangulation;
	using MGroup.LinearAlgebra.Vectors;

	public static class SchurComplementFullCsrCsrCsc
	{
		/// <summary>
		/// Calculates the Schur complement of A/A11 = S = A00 - A01 * inv(A11) * A10, where A = [A00, A01; A10, A11] (in Matlab
		/// notation).
		/// </summary>
		/// <param name="A01">It must be in typical CSR format, where the rows must contain columns in ascending order.</param>
		/// <param name="A10">It must be in typical CSR format, where the rows must contain columns in ascending order.</param>
		/// <remarks>
		/// This method constructs inv(A11) * A10 one column at a time and uses that column to calculate the superdiagonal
		/// entries of the corresponding column of A01 * inv(A11) * A10.
		/// </remarks>
		public static Matrix CalcSchurComplement(Matrix A00, CsrMatrix A01, CsrMatrix A10, ITriangulation inverseA11)
		{
			var S = Matrix.CreateZero(A00.NumRows, A00.NumColumns);
			CalcSchurComplement(A00, A01, A10, inverseA11, S);
			return S;
		}

		/// <summary>
		/// Calculates the Schur complement of A/A11 = S = A00 - A01 * inv(A11) * A10, where A = [A00, A01; A10, A11] (in Matlab
		/// notation).
		/// </summary>
		/// <param name="A01">It must be in typical CSR format, where the rows must contain columns in ascending order.</param>
		/// <param name="A10">It must be in typical CSR format, where the rows must contain columns in ascending order.</param>
		/// <remarks>
		/// This method constructs inv(A11) * A10 one column at a time and uses that column to calculate the superdiagonal
		/// entries of the corresponding column of A01 * inv(A11) * A10.
		/// </remarks>
		public static void CalcSchurComplement(Matrix A00, CsrMatrix A01, CsrMatrix A10, ITriangulation inverseA11,
			Matrix result)
		{
			//TODO: Assert that rows of CSR matrices are in ascending order

			// Use an auxilliary arrays to reduce misses when searching column j of each row
			var firstCols = new int[A10.NumRows]; // Start from col 0 for each row
			var rowStarts = new int[A10.NumRows];
			Array.Copy(A10.RawRowOffsets, rowStarts, rowStarts.Length);

			// Column j of A10
			for (int j = 0; j < A10.NumColumns; ++j)
			{
				// column j of (inv(A11) * A10) = inv(A11) * column j of A10
				Vector colA10 = GetCsrColumn(A10, j, rowStarts, firstCols);
				Vector colInvA11TimesA10 = inverseA11.SolveLinearSystem(colA10);

				// column j of (A01 * inv(A11) * A10) = A01 * column j of (inv(A11) * A10)
				Vector colS = A01.Multiply(colInvA11TimesA10);
				result.SetSubcolumn(j, colS);
			}

			// S = A00 - S
			result.LinearCombinationIntoThis(-1, A00, +1);
		}

		/// <summary>
		/// Efficient implementation that 1) assumes CSR.colIndices is sorted per row, 2) uses and modifies an auxilliary array 
		/// <paramref name="rowStarts"/> to hold the starting index for each row to search for <paramref name="colIdx"/>
		/// </summary>
		/// <param name="csr"></param>
		/// <param name="colIdx"></param>
		/// <param name="rowStarts">Will be modified.</param>
		/// <param name="firstCols">Will be modified.</param>
		private static Vector GetCsrColumn(CsrMatrix csr, int colIdx, int[] rowStarts, int[] firstCols)
		{
			double[] values = csr.RawValues;
			int[] colIndices = csr.RawColIndices;
			int[] rowOffsets = csr.RawRowOffsets;

			var result = new double[csr.NumRows];
			for (int i = 0; i < csr.NumRows; ++i)
			{
				if (colIdx <= firstCols[i])
				{
					// Start searching from rowStarts[i], which is further along rowOffsets[i]. The missing entries were 
					// already searched by previous columns, meaning that they are less than the new col index. 
					int start = rowStarts[i]; //inclusive
					int end = rowOffsets[i + 1]; //exclusive
					for (int k = start; k < end; ++k)
					{
						if (colIndices[k] == colIdx)
						{
							result[i] = values[k];
							rowStarts[i] = k + 1; // In the next search ignore all entries up to and including this one.
							firstCols[i] = colIdx + 1;
							break;
						}
						else if (colIndices[k] > colIdx) // no need to search pass this col index
						{
							rowStarts[i] = k; // In the next search ignore all entries up to but excluding this one.
											  //TODO: E.g say cols 3 and 7 are nonzero corresponding to k = 31, 32. After searching for col 4, 
											  //		rowStarts[i]=32. Searching for cols 5, 6 will repeat the loop and check and set rowStarts[i]=32.
											  //		It would be nice if that could be avoided.
							firstCols[i] = colIndices[k];
							break;
						}
					}
				}
				// otherwise there is no point in searching colIndices
			}

			return Vector.CreateFromArray(result);
		}
	}
}
