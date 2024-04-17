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

	public static class IntSymMatrixColMajorTests
	{
		[Fact]
		public static void TestProperties()
		{
			var n = SymmPosDef10by10.Order;
			double scaleFactor = 1000;
			var array1D = ArrayUtilities.ScaleAndRound(SymmPosDef10by10.LowerTriangleRowMajor, scaleFactor); // same as upper, col-major
			var matrix = IntSymMatrixColMajor.CreateFromArray(n, array1D, true);

			Assert.Equal(n, matrix.NumRows);
			Assert.Equal(n, matrix.NumColumns);
			var comparer = new MatrixComparer();
			comparer.AssertEqual(array1D, matrix.RawValues);
		}

		[Fact]
		public static void TestIndexers()
		{
			var n = SymmPosDef10by10.Order;
			double scaleFactor = 1000;
			var array2D = ArrayUtilities.ScaleAndRound(SymmPosDef10by10.Matrix, scaleFactor);
			var matrix = IntSymMatrixColMajor.CreateZero(n);

			for (var i = 0; i < n; i++)
			{
				for (var j = 0; j <= i; j++)
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
