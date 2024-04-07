namespace MGroup.LinearAlgebra.Tests.Matrices
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	public static class ConversionTests
	{
		[Fact]
		private static void TestCsrToTriangularUpper()
		{
			var csrMatrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
				SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var upperExpected = TriangularUpper.CreateFromArray(SparsePosDef10by10.Matrix);

			TriangularUpper upperComputed = csrMatrix.ExtractUpperAndDiagonalToPacked();

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(upperExpected, upperComputed);
		}
	}
}
