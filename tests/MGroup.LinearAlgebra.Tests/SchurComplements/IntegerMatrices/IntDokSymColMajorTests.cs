namespace MGroup.LinearAlgebra.Tests.SchurComplements.IntegerMatrices
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
			var nnz = SparsePosDef10by10.SymmetricCscValues.Length;
			double scaleFactor = 1000;
			var array2D = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.Matrix, scaleFactor);

			var dok = CreateDokFromArray(array2D);

			var comparer = new MatrixComparer();
			comparer.AssertEqual(array2D, dok);
			Assert.Equal(nnz, dok.CountNonZeros());
		}

		[Fact]
		public static void TestBuildCsrArrays()
		{
			double scaleFactor = 1000;
			var array2D = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.Matrix, scaleFactor);
			var valuesExpected = ArrayUtilities.ScaleAndRound(SparsePosDef10by10.SymmetricCscValues, scaleFactor);
			var rowIndicesExpected = SparsePosDef10by10.SymmetricCscRowIndices;
			var colOffsetsExpected = SparsePosDef10by10.SymmetricCscColOffsets;

			var dok = CreateDokFromArray(array2D);
			(var values, var rowIndices, var colOffsets) = dok.BuildCscArrays();

			var comparer = new MatrixComparer();
			comparer.AssertEqual(valuesExpected, values);
			comparer.AssertEqual(rowIndicesExpected, rowIndices);
			comparer.AssertEqual(colOffsetsExpected, colOffsets);
		}

		private static IntDokSymColMajor CreateDokFromArray(int[,] array2D)
		{
			var n = array2D.GetLength(0);
			var dok = IntDokSymColMajor.CreateZero(n);

			for (var i = 0; i < n; i++)
			{
				for (var j = 0; j <= i; j++)
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
