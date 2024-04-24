namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using MGroup.LinearAlgebra.Exceptions;

	/// <summary>
	/// Represents a symmetric sparse matrix with integer entries. The non-zero entries of the diagonal and upper triangle are 
	/// stored in column-major order using an instance of SortedDictionary for each column.
	/// </summary>
	public class IntDokSymColMajor : IIndexableInt2D
	{
		private readonly int order;
		private readonly SortedDictionary<int, int>[] columns;

		private IntDokSymColMajor(int order, SortedDictionary<int, int>[] columns)
		{
			this.order = order;
			this.columns = columns;

			this.NumRows = order;
			this.NumColumns = order;
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
				if (rowIdx > colIdx)
				{
					int swap = rowIdx;
					rowIdx = colIdx;
					colIdx = swap;
				}

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
				if (rowIdx > colIdx)
				{
					int swap = rowIdx;
					rowIdx = colIdx;
					colIdx = swap;
				}

				columns[colIdx][rowIdx] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IntDokSymColMajor"/> with all entries being equal to 0. In essence, no entry
		/// is explicitly stored yet.
		/// </summary>
		/// <param name="numRows">The number of rows of the new matrix.</param>
		/// <param name="numColumns">The number of columns of the new matrix.</param>
		/// <returns>An integer matrix with sparse, col-major format, using dictionaries.</returns>
		public static IntDokSymColMajor CreateZero(int order)
		{
			var columns = new SortedDictionary<int, int>[order];
			for (int j = 0; j < order; ++j)
			{
				columns[j] = new SortedDictionary<int, int>(); //Initial capacity may be optimized.
			}

			return new IntDokSymColMajor(order, columns);
		}

		/// <summary>
		/// Creates the values and indexing arrays in symmetric CSC storage format of the current matrix. This method should be
		/// called after fully defining the matrix in <see cref="IntDokSymColMajor"/> format.
		/// </summary>
		/// <remarks>
		/// In the returned arrays "values" and "rowIndices", the entries corresponding to the same column are in ascending order.
		/// </remarks>
		/// <exception cref="EmptyMatrixBuilderException">Thrown if no non-zero entries have been defined yet.</exception>
		/// <returns>
		/// "values": the nonzero entries of the matrix that lie on or above the diagonal.
		/// "rowIndices": the row indices of the entries in "values".
		/// "colOffsets": the start of each column of the matrix inside "values" and "rowIndices". There is an additional entry
		/// at the end of this array that declares how many explicitly stored nonzero entries there are.
		/// </returns>
		public (int[] values, int[] rowIndices, int[] colOffsets) BuildCscArrays()
		{
			int[] colOffsets = new int[order + 1];
			int nnz = 0;
			for (int j = 0; j < order; ++j)
			{
				colOffsets[j] = nnz;
				nnz += columns[j].Count;
			}

			if (nnz == 0)
			{
				throw new EmptyMatrixBuilderException("Cannot build symmetric CSC arrays from a DOK with nnz = 0.");
			}

			colOffsets[order] = nnz; //The last CSC entry has colOffset = nnz.

			int[] rowIndices = new int[nnz];
			int[] values = new int[nnz];
			int counter = 0;
			for (int j = 0; j < order; ++j)
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
		/// could end up being stored explicitly too. All these entries lies on or above the diagonal of the matrix.
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
