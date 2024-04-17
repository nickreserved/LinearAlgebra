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

	public static class IntFullMatrixColMajorTests
	{
		[Fact]
		public static void TestProperties()
		{
			var m = RectangularFullRank10by5.NumRows;
			var n = RectangularFullRank10by5.NumCols;
			double scaleFactor = 1000;
			var array1D = ArrayUtilities.ScaleAndRound(RectangularFullRank10by5.MatrixColMajor, scaleFactor);
			var matrix = IntFullMatrixColMajor.CreateFromArray(m, n, array1D, true);

			Assert.Equal(m, matrix.NumRows);
			Assert.Equal(n, matrix.NumColumns);
			var comparer = new MatrixComparer();
			comparer.AssertEqual(array1D, matrix.RawValues);
		}

		[Fact]
		public static void TestIndexers()
		{
			var m = RectangularFullRank10by5.NumRows;
			var n = RectangularFullRank10by5.NumCols;
			double scaleFactor = 1000;
			var array2D = ArrayUtilities.ScaleAndRound(RectangularFullRank10by5.Matrix, scaleFactor);
			var matrix = IntFullMatrixColMajor.CreateZero(m, n);

			for (var i = 0; i < m; i++)
			{
				for (var j = 0; j < n; j++)
				{
					if (array2D[i, j] != 0)
					{
						matrix[i, j] = array2D[i, j];
					}
				}
			}

			var comparer = new MatrixComparer();
			comparer.AssertEqual(array2D, matrix);
		}
	}
}
