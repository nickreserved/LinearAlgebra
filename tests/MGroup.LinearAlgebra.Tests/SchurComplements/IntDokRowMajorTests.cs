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

	public static class IntDokRowMajorTests
	{
		[Fact]
		public static void TestProperties()
		{
			var matrix = IntDokRowMajor.CreateZero(5, 10);
			Assert.Equal(5, matrix.NumRows);
			Assert.Equal(10, matrix.NumColumns);
			Assert.Equal(0, matrix.CountNonZeros());
		}

		[Fact]
		public static void TestIndexers()
		{
			int nnz = SparseRectangular10by5.CsrValues.Length;
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.Matrix, scaleFactor);

			IntDokRowMajor dok = CreateDokFromArray(array2D);

			var comparer = new MatrixComparer();
			comparer.AssertEqual(array2D, dok);
			Assert.Equal(nnz, dok.CountNonZeros());
		}

		[Fact]
		public static void TestBuildCsrArrays()
		{
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.Matrix, scaleFactor);
			int[] valuesExpected = ArrayUtilities.ScaleAndRound(SparseRectangular10by5.CsrValues, scaleFactor);
			int[] colIndicesExpected = SparseRectangular10by5.CsrColIndices;
			int[] rowOffsetsExpected = SparseRectangular10by5.CsrRowOffsets;

			IntDokRowMajor dok = CreateDokFromArray(array2D);
			(int[] values, int[] colIndices, int[] rowOffsets) = dok.BuildCsrArrays();

			var comparer = new MatrixComparer();
			comparer.AssertEqual(valuesExpected, values);
			comparer.AssertEqual(colIndicesExpected, colIndices);
			comparer.AssertEqual(rowOffsetsExpected, rowOffsets);
		}

		private static IntDokRowMajor CreateDokFromArray(int[,] array2D)
		{
			int m = array2D.GetLength(0);
			int n = array2D.GetLength(1);
			var dok = IntDokRowMajor.CreateZero(m, n);

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
