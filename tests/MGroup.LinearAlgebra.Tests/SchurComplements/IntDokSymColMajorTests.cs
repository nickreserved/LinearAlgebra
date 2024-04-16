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

	public static class IntDokSymColMajorTests
	{
		[Fact]
		public static void TestProperties()
		{
			var matrix = IntDokSymColMajor.CreateZero(8);
			Assert.Equal(8, matrix.NumRows);
			Assert.Equal(8, matrix.NumColumns);
			Assert.Equal(0, matrix.CountNonZeros());
		}

		[Fact]
		public static void TestIndexers()
		{
			int nnz = SparsePosDef10by10.SymmetricCscValues.Length;
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.Matrix, scaleFactor);

			IntDokSymColMajor dok = CreateDokFromArray(array2D);

			var comparer = new MatrixComparer();
			comparer.AssertEqual(array2D, dok);
			Assert.Equal(nnz, dok.CountNonZeros());
		}

		[Fact]
		public static void TestBuildCsrArrays()
		{
			double scaleFactor = 1000;
			int[,] array2D = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.Matrix, scaleFactor);
			int[] valuesExpected = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.SymmetricCscValues, scaleFactor);
			int[] rowIndicesExpected = SparsePosDef10by10.SymmetricCscRowIndices;
			int[] colOffsetsExpected = SparsePosDef10by10.SymmetricCscColOffsets;

			IntDokSymColMajor dok = CreateDokFromArray(array2D);
			(int[] values, int[] rowIndices, int[] colOffsets) = dok.BuildCscArrays();

			var comparer = new MatrixComparer();
			comparer.AssertEqual(valuesExpected, values);
			comparer.AssertEqual(rowIndicesExpected, rowIndices);
			comparer.AssertEqual(colOffsetsExpected, colOffsets);
		}

		private static IntDokSymColMajor CreateDokFromArray(int[,] array2D)
		{
			int n = array2D.GetLength(0);
			var dok = IntDokSymColMajor.CreateZero(n);

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j <= i; j++)
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
