namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Matrices;

	/// <summary>
	/// Divides a symmetric matrix A into 4 submatrices, such as A = [A00, A01; A10, A11] (in Matlab format). The rows and
	/// columns of the original matrix A that belong to the group "0" or "1", do not have to be contiguous.
	/// Submatrices A00 and A11 are symmetric, while A10 is not expicitly stored because it is the transpose of A01.
	/// The original matrix A is in symmetric CSC format, namely only the diagonal and upper triangle are stored. Submatrix A01 
	/// is in CSR format. Submatrix A11 is in symmetric CSC format (same as A).
	/// </summary>
	public abstract class SubmatrixExtractorCscSymBase
	{
		protected int[] indicesGroup0;
		protected int[] indicesGroup1;
		protected SymmetricCscMatrix originalMatrix;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A00.
		/// </summary>
		protected IValuesArrayMapper mapper00;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A01.
		/// </summary>
		protected IValuesArrayMapper mapper01;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A11.
		/// </summary>
		protected IValuesArrayMapper mapper11;

		/// <summary>
		/// A01 of A = [A00, A01; A10, A11] (using Matlab notation). It is equal to transpose(A10).
		/// </summary>
		public CsrMatrix Submatrix01 { get; protected set; }

		/// <summary>
		/// A11 of A = [A00, A01; A10, A11] (using Matlab notation). It is symmetric.
		/// </summary>
		public SymmetricCscMatrix Submatrix11 { get; protected set; }

		/// <summary>
		/// Deletes the submatrices and any other data that was stored previously, returning the object to its just-initialized 
		/// state.
		/// </summary>
		public virtual void Clear()
		{
			originalMatrix = null;
			indicesGroup0 = null;
			indicesGroup1 = null;

			Submatrix01 = null;
			Submatrix11 = null;
			mapper00 = null;
			mapper01 = null;
			mapper11 = null;
		}

		/// <summary>
		/// Creates the indexing arrays that describe the sparsity pattern (in symmetric CSC format) of a submatrix A00 of
		/// A=<paramref name="originalMatrix"/>=[A00, A01; A10, A11] (using Matlab notation), with
		/// <paramref name="rowsColsToKeep"/> specifying which rows and columns to keep.
		/// </summary>
		/// <param name="originalMatrix">
		/// The original matrix A in symmetric CSC format (only diagonal and upper triangle are stored.
		/// </param>
		/// <param name="rowsColsToKeep">
		/// Specifies the rows and columns of the entries of <paramref name="originalMatrix"/> that will be kept in the
		/// submatrix result.
		/// </param>
		/// <returns>
		/// rowIndices: the indices of the stored entries (diagonal and upper triangle), according to CSC format.
		/// colOffsets: the offset into rowIndices of the 1st entry of each column, according to CSC format.
		/// </returns>
		public (int[] rowIndices, int[] colOffsets) ExtractSparsityPattern(SymmetricCscMatrix originalMatrix, int[] rowsColsToKeep)
		{
			// Store which entries of each column are nonzero
			var columnsA00 = new SortedSet<int>[rowsColsToKeep.Length];
			for (int j = 0; j < rowsColsToKeep.Length; ++j)
			{
				columnsA00[j] = new SortedSet<int>();
			}

			// Original matrix indices to submatrix indices. Indices not belonging to group 0 will be marked as -1.
			var originalToSubIndices = new int[originalMatrix.NumRows];
			Fill(originalToSubIndices, -1);
			for (int i0 = 0; i0 < rowsColsToKeep.Length; ++i0)
			{
				originalToSubIndices[rowsColsToKeep[i0]] = i0;
			}

			// Iterate the non zero values array of the original sparse matrix
			for (int subJ = 0; subJ < rowsColsToKeep.Length; ++subJ)
			{
				int j = rowsColsToKeep[subJ];
				int start = originalMatrix.RawColOffsets[j];
				int end = originalMatrix.RawColOffsets[j + 1];
				for (int t = start; t < end; ++t)
				{
					int i = originalMatrix.RawRowIndices[t];
					int subI = originalToSubIndices[i];
					if (subI >= 0) // (i, j) belongs to submatrix A00
					{
						if (subI <= subJ)
						{
							columnsA00[subJ].Add(subI);
						}
						else // ensure that only upper triangular entries are storedr
						{
							columnsA00[subI].Add(subJ);
						}
					}
				}
			}

			return BuildCscArrays(rowsColsToKeep.Length, columnsA00);
		}

		//public (int[] rowIndices, int[] colOffsets) ExtractSparsityPattern(SymmetricCscMatrix originalMatrix, int[] rowsColsToKeep)
		//{
		//	// Store which entries of each column are nonzero
		//	var columnsA00 = new SortedSet<int>[rowsColsToKeep.Length];
		//	for (int j = 0; j < rowsColsToKeep.Length; ++j)
		//	{
		//		columnsA00[j] = new SortedSet<int>();
		//	}

		//	// Original matrix indices to submatrix indices. Indices not belonging to group 0 will be marked as -1.
		//	var originalToSubIndices = new int[originalMatrix.NumRows];
		//	Fill(originalToSubIndices, -1);
		//	for (int i0 = 0; i0 < rowsColsToKeep.Length; ++i0)
		//	{
		//		originalToSubIndices[rowsColsToKeep[i0]] = i0;
		//	}

		//	// Iterate the non zero values array of the original sparse matrix
		//	for (int j = 0; j < originalMatrix.NumColumns; ++j)
		//	{
		//		int start = originalMatrix.RawColOffsets[j];
		//		int end = originalMatrix.RawColOffsets[j + 1];
		//		int subJ = originalToSubIndices[j];
		//		if (subJ >= 0) // j belongs to group 0 indices
		//		{
		//			for (int t = start; t < end; ++t)
		//			{
		//				int i = originalMatrix.RawRowIndices[t];
		//				int subI = originalToSubIndices[i];
		//				if (subI >= 0) // (i, j) belongs to submatrix A00
		//				{
		//					if (subI <= subJ)
		//					{
		//						columnsA00[subJ].Add(subI);
		//					}
		//					else // ensure that only upper triangular entries are storedr
		//					{
		//						columnsA00[subI].Add(subJ);
		//					}
		//				}
		//			}
		//		}
		//	}

		//	return BuildCscArrays(rowsColsToKeep.Length, columnsA00);
		//}

		protected static (int[] rowIndices, int[] colOffsets) BuildCscArrays(int order, SortedSet<int>[] nonzeroRowsOfEachCol)
		{
			// Create CSC arrays from the dictionary
			int[] colOffsets = new int[order + 1];
			int nnz = 0;
			for (int j = 0; j < order; ++j)
			{
				colOffsets[j] = nnz;
				nnz += nonzeroRowsOfEachCol[j].Count;
			}

			colOffsets[order] = nnz; //The last CSC entry is nnz.

			int[] rowIndices = new int[nnz];
			int counter = 0;
			for (int j = 0; j < order; ++j)
			{
				foreach (var rowIdx in nonzeroRowsOfEachCol[j])
				{
					rowIndices[counter] = rowIdx;
					++counter;
				}
			}

			return (rowIndices, colOffsets);
		}

		//TODO: There must be a faster way to do this
		internal static void Fill<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				array[i] = value;
			}
		}

		/// <summary>
		/// Creates an array that maps the indices (rows/columns) of the original matrix to the indices of its 2x2 submatrices.
		/// Group 0 indices i are stored as: -i-1. Group 1 indices are stored as they are.
		/// </summary>
		/// <param name="originalOrder"></param>
		/// <param name="indicesGroup0"></param>
		/// <param name="indicesGroup1"></param>
		internal static int[] MapOriginalToSubmatrixIndices(int originalOrder, int[] indicesGroup0, int[] indicesGroup1)
		{
			var originalToSubIndices = new int[originalOrder];
			for (int i0 = 0; i0 < indicesGroup0.Length; ++i0)
			{
				originalToSubIndices[indicesGroup0[i0]] = -i0 - 1;
			}

			for (int i1 = 0; i1 < indicesGroup1.Length; ++i1)
			{
				originalToSubIndices[indicesGroup1[i1]] = i1;
			}

			return originalToSubIndices;
		}
	}
}
