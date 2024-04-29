namespace MGroup.LinearAlgebra.Tests.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	public static class SparseToDenseValuesArrayCopierTests
	{
		[Fact]
		public static void TestCopyValuesArrayToSubmatrixNonSymmetric()
		{
			var example = SchurComplementExample.CreateExampleNonSymmetricA();
			int n = example.MatrixOrder;
			var originalA = CsrMatrix.CreateFromArrays(n, n, example.MatrixCsr.values, example.MatrixCsr.colIndices,
				example.MatrixCsr.rowOffsets, true);
			double[] expectedA00 = example.Submatrix00FullColMajor;

			int[] mapA00CsrtoAFull =
			{
				23, -1, 8, 16, 50, -1, 25, 39, 10, 17, -1, 64, -1, -1, 6, 14, -1, -1,
				22, 37, 7, 15, -1, -1, 26, -1, 11, -1, 52, 65, 27, 42, -1, 20, 53, 67
			};
			var valuesMapper = new SparseToDenseValuesArrayMapper(mapA00CsrtoAFull);

			var computedValues = new double[expectedA00.Length];
			valuesMapper.CopyValuesArrayToSubmatrix(originalA.RawValues, computedValues);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(expectedA00, computedValues);
		}

		[Fact]
		public static void TestCopyValuesArrayToSubmatrixSymmetric()
		{
			var example = SchurComplementExample.CreateExampleSymmetricA();
			int n = example.MatrixOrder;
			var originalA = SymmetricCscMatrix.CreateFromArrays(n, example.MatrixCscSymmetric.values,
				example.MatrixCscSymmetric.rowIndices, example.MatrixCscSymmetric.colOffsets, true);
			double[] expectedA00 = example.Submatrix00PackedUpper;

			int[] mapA00SymCsctoAPck = { 2, -1, 20, -1, -1, 7, 15, 19, -1, 17 };
			var valuesMapper = new SparseToDenseValuesArrayMapper(mapA00SymCsctoAPck);

			var computedValues = new double[expectedA00.Length];
			valuesMapper.CopyValuesArrayToSubmatrix(originalA.RawValues, computedValues);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(expectedA00, computedValues);
		}
	}
}
