namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using MGroup.LinearAlgebra.Exceptions;

	/// <summary>
	/// Represents a sparse (square or rectangular) matrix with integer entries. Non-zero entries are stored in column-major
	/// order using an instance of SortedDictionary for each column.
	/// </summary>
	public class IntDokColMajor : IIndexableInt2D
	{
		private readonly SortedDictionary<int, int>[] columns;

		public IntDokColMajor(int numRows, int numColumns, SortedDictionary<int, int>[] columns)
		{
			this.columns = columns;
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
				if (columns[colIdx].TryGetValue(rowIdx, out int val))
				{
					return val;
				}
				else
				{
					return 0;
				}
			}

			set //not thread safe
			{
				columns[colIdx][rowIdx] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IntDokColMajor"/> with all entries being equal to 0. In essence, no entry is
		/// explicitly stored yet.
		/// </summary>
		/// <param name="numRows">The number of rows of the new matrix.</param>
		/// <param name="numColumns">The number of columns of the new matrix.</param>
		/// <returns>An integer matrix with sparse, col-major format, using dictionaries.</returns>
		public static IntDokColMajor CreateZero(int numRows, int numColumns)
		{
			var columns = new SortedDictionary<int, int>[numColumns];
			for (int j = 0; j < numColumns; ++j)
			{
				columns[j] = new SortedDictionary<int, int>(); //Initial capacity may be optimized.
			}

			return new IntDokColMajor(numRows, numColumns, columns);
		}

		/// <summary>
		/// Creates the values and indexing arrays in CSC storage format of the current matrix. This method should be
		/// called after fully defining the matrix in <see cref="IntDokColMajor"/> format.
		/// </summary>
		/// <remarks>
		/// In the returned arrays "values" and "rowIndices", the entries corresponding to the same column are in ascending order.
		/// </remarks>
		/// <exception cref="EmptyMatrixBuilderException">Thrown if no non-zero entries have been defined yet.</exception>
		/// <returns>
		/// "values": the nonzero entries of the matrix.
		/// "rowIndices": the row indices of the entries in "values".
		/// "colOffsets": the start of each column of the matrix inside "values" and "rowIndices". There is an additional entry
		/// at the end of this array that declares how many nonzero entries there are.
		/// </returns>
		public (int[] values, int[] rowIndices, int[] colOffsets) BuildCscArrays()
		{
			int[] colOffsets = new int[NumColumns + 1];
			int nnz = 0;
			for (int j = 0; j < NumColumns; ++j)
			{
				colOffsets[j] = nnz;
				nnz += columns[j].Count;
			}

			if (nnz == 0)
			{
				throw new EmptyMatrixBuilderException("Cannot build symmetric CSC arrays from a DOK with nnz = 0.");
			}

			colOffsets[NumColumns] = nnz; //The last CSC entry has colOffset = nnz.

			int[] rowIndices = new int[nnz];
			int[] values = new int[nnz];
			int counter = 0;
			for (int j = 0; j < NumColumns; ++j)
			{
				// These are already ordered by the SortedDictionary
				foreach (var rowValPair in columns[j])
				{
					rowIndices[counter] = rowValPair.Key;
					values[counter] = rowValPair.Value;
					++counter;
				}
			}

			return (values, rowIndices, colOffsets);
		}

		/// <summary>
		/// Counts the number of entries explicitly stored by this matrix. These are mostly non zeros, but some zero entries
		/// could end up being stored explicitly too.
		/// </summary>
		/// <returns>The number of stored entries.</returns>
		public int CountNonZeros()
		{
			int count = 0;
			for (int j = 0; j < NumColumns; ++j)
			{
				count += columns[j].Count;
			}

			return count;
		}
	}
}
