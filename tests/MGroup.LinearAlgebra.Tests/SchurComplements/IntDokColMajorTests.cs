namespace MGroup.LinearAlgebra.Tests.SchurComplements
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using MGroup.LinearAlgebra.SchurComplements.IntegerMatrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	public static class IntDokColMajorTests
	{
		[Fact]
		public static void TestProperties()
		{
			var matrix = IntDokColMajor.CreateZero(5, 10);
			Assert.Equal(5, matrix.NumRows);
			Assert.Equal(10, matrix.NumColumns);
			Assert.Equal(0, matrix.CountNonZeros());
		}

		[Fact]
		public static void TestIndexers()
		{
			int nnz = SparseRectangular10by5.CscValues.Length;
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.Matrix, scaleFactor);

			IntDokColMajor dok = CreateDokFromArray(array2D);

			var comparer = new MatrixComparer();
			comparer.AssertEqual(array2D, dok);
			Assert.Equal(nnz, dok.CountNonZeros());
		}

		[Fact]
		public static void TestBuildCsrArrays()
		{
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.Matrix, scaleFactor);
			int[] valuesExpected = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.CscValues, scaleFactor);
			int[] rowIndicesExpected = SparseRectangular10by5.CscRowIndices;
			int[] colOffsetsExpected = SparseRectangular10by5.CscColOffsets;

			IntDokColMajor dok = CreateDokFromArray(array2D);
			(int[] values, int[] rowIndices, int[] colOffsets) = dok.BuildCscArrays();

			var comparer = new MatrixComparer();
			comparer.AssertEqual(valuesExpected, values);
			comparer.AssertEqual(rowIndicesExpected, rowIndices);
			comparer.AssertEqual(colOffsetsExpected, colOffsets);
		}

		private static IntDokColMajor CreateDokFromArray(int[,] array2D)
		{
			int m = array2D.GetLength(0);
			int n = array2D.GetLength(1);
			var dok = IntDokColMajor.CreateZero(m, n);

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (array2D[i, j] != 0)
					{
						dok[i, j] = array2D[i, j];
					}
				}
			}

			return dok;
		}
	}
}
