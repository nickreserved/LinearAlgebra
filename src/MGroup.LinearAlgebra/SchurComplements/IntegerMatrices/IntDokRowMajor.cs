namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using MGroup.LinearAlgebra.Exceptions;

	/// <summary>
	/// Represents a sparse (square or rectangular) matrix with integer entries. Non-zero entries are stored in row-major order
	/// using an instance of SortedDictionary for each row.
	/// </summary>
	public class IntDokRowMajor : IIndexableInt2D
	{
		private readonly SortedDictionary<int, int>[] rows;

		private IntDokRowMajor(int numRows, int numColumns, SortedDictionary<int, int>[] columns)
		{
			this.rows = columns;
			this.NumRows = numRows;
			this.NumColumns = numColumns;
		}

		/// <inheritdoc/>>
		public int NumColumns { get; }

		/// <inheritdoc/>>
		public int NumRows { get; }

		/// <inheritdoc/>>
		public int this[int rowIdx, int colIdx]
		{
			get
			{
				if (rows[rowIdx].TryGetValue(colIdx, out int val))
				{
					return val;
				}
				else
				{
					return 0;
				}
			}

			set // not thread safe
			{
				rows[rowIdx][colIdx] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IntDokRowMajor"/> with all entries being equal to 0. In essence, no entry is
		/// explicitly stored yet.
		/// </summary>
		/// <param name="numRows">The number of rows of the new matrix.</param>
		/// <param name="numColumns">The number of columns of the new matrix.</param>
		/// <returns>An integer matrix with sparse, row-major format, using dictionaries.</returns>
		public static IntDokRowMajor CreateZero(int numRows, int numColumns)
		{
			var rows = new SortedDictionary<int, int>[numRows];
			for (int i = 0; i < numRows; ++i)
			{
				rows[i] = new SortedDictionary<int, int>(); //Initial capacity may be optimized.
			}

			return new IntDokRowMajor(numRows, numColumns, rows);
		}

		/// <summary>
		/// Creates the values and indexing arrays in CSR storage format of the current matrix. This method should be
		/// called after fully defining the matrix in <see cref="IntDokRowMajor"/> format.
		/// </summary>
		/// <remarks>
		/// In the returned arrays "values" and "colIndices", the entries corresponding to the same row are in ascending order.
		/// </remarks>
		/// <exception cref="EmptyMatrixBuilderException">Thrown if no non-zero entries have been defined yet.</exception>
		/// <returns>
		/// "values": the nonzero entries of the matrix.
		/// "colIndices": the column indices of the entries in "values".
		/// "rowOffsets": the start of each row of the matrix inside "values" and "colIndices". There is an additional entry at
		/// the end of this array that declares how many nonzero entries there are.
		/// </returns>
		public (int[] values, int[] colIndices, int[] rowOffsets) BuildCsrArrays()
		{
			int[] rowOffsets = new int[NumRows + 1];
			int nnz = 0;
			for (int i = 0; i < NumRows; ++i)
			{
				rowOffsets[i] = nnz;
				nnz += rows[i].Count;
			}

			if (nnz == 0)
			{
				throw new EmptyMatrixBuilderException("Cannot build symmetric CSC arrays from a DOK with nnz = 0.");
			}

			rowOffsets[NumRows] = nnz; //The last CSR entry has colOffset = nnz.

			int[] colIndices = new int[nnz];
			int[] values = new int[nnz];
			int counter = 0;
			for (int i = 0; i < NumRows; ++i)
			{
				// These are already ordered by the SortedDictionary
				foreach (var colValPair in rows[i])
				{
					colIndices[counter] = colValPair.Key;
					values[counter] = colValPair.Value;
					++counter;
				}
			}

			return (values, colIndices, rowOffsets);
		}

		/// <summary>
		/// Counts the number of entries explicitly stored by this matrix. These are mostly non zeros, but some zero entries
		/// could end up being stored explicitly too.
		/// </summary>
		/// <returns>The number of stored entries.</returns>
		public int CountNonZeros()
		{
			int count = 0;
			for (int i = 0; i < NumRows; ++i)
			{
				count += rows[i].Count;
			}

			return count;
		}
	}
}
