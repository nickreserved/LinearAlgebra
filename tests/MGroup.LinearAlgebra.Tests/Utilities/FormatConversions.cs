namespace MGroup.LinearAlgebra.Tests.Utilities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	internal static class FormatConversions
	{
		public static (double[] values, int[] colIndices, int[] rowOffsets) ArrayToCsr(double[,] matrix,
			double zeroEntryTol = 0.0)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			int nnz = CountNonZeros(matrix, zeroEntryTol);

			var values = new double[nnz];
			var colIndices = new int[nnz];
			var rowOffsets = new int[m + 1];
			rowOffsets[m] = nnz;

			int k = 0;
			for (int i = 0; i < m; i++)
			{
				rowOffsets[i] = k;
				for (int j = 0; j < n; ++j)
				{
					double val = matrix[i, j];
					if (Math.Abs(val) > zeroEntryTol)
					{
						values[k] = val;
						colIndices[k] = j;
						++k;
					}
				}
			}

			return (values, colIndices, rowOffsets);
		}

		public static (double[] values, int[] rowIndices, int[] colOffsets) ArrayToCsc(double[,] matrix,
			double zeroEntryTol = 0.0)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			int nnz = CountNonZeros(matrix, zeroEntryTol);

			var values = new double[nnz];
			var rowIndices = new int[nnz];
			var colOffsets = new int[n + 1];
			colOffsets[n] = nnz;

			int k = 0;
			for (int j = 0; j < n; j++)
			{
				colOffsets[j] = k;
				for (int i = 0; i < m; ++i)
				{
					double val = matrix[i, j];
					if (Math.Abs(val) > zeroEntryTol)
					{
						values[k] = val;
						rowIndices[k] = i;
						++k;
					}
				}
			}

			return (values, rowIndices, colOffsets);
		}

		public static (double[] values, int[] rowIndices, int[] colOffsets) ArrayToSymmetricCsc(double[,] matrix,
			double zeroEntryTol = 0.0)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			if (m != n)
			{
				throw new ArgumentException("The provided matrix is not square");
			}

			int nnz = CountNonZerosUpperDiagonal(matrix, zeroEntryTol);

			var values = new double[nnz];
			var rowIndices = new int[nnz];
			var colOffsets = new int[n + 1];
			colOffsets[n] = nnz;

			int k = 0;
			for (int j = 0; j < n; j++)
			{
				colOffsets[j] = k;
				for (int i = 0; i <= j; ++i)
				{
					double val = matrix[i, j];
					if (Math.Abs(val) > zeroEntryTol)
					{
						values[k] = val;
						rowIndices[k] = i;
						++k;
					}
				}
			}

			return (values, rowIndices, colOffsets);
		}

		public static double[] ArrayToPackedUpper(double[,] matrix)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			if (m != n)
			{
				throw new ArgumentException("The provided matrix is not square");
			}

			var values = new double[(n * (n + 1)) / 2];
			int counter = 0;
			for (int j = 0; j < n; ++j)
			{
				for (int i = 0; i <= j; ++i)
				{
					values[counter] = matrix[i, j];
					++counter;
				}
			}

			return values;
		}

		private static int CountNonZeros(double[,] matrix, double zeroEntryTolerance)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			int nnz = 0;
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; ++j)
				{
					if (Math.Abs(matrix[i, j]) > zeroEntryTolerance)
					{
						++nnz;
					}
				}
			}

			return nnz;
		}

		private static int CountNonZerosUpperDiagonal(double[,] matrix, double zeroEntryTolerance)
		{
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			int nnz = 0;
			for (int i = 0; i < m; i++)
			{
				for (int j = i; j < n; ++j)
				{
					if (Math.Abs(matrix[i, j]) > zeroEntryTolerance)
					{
						++nnz;
					}
				}
			}

			return nnz;
		}
	}
}
