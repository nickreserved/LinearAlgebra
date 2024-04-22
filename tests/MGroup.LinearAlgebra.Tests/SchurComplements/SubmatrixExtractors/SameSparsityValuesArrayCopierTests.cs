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

	public static class SameSparsityValuesArrayCopierTests
	{
		[Fact]
		public static void TestCopyValuesArrayToSubmatrixNonSymmetric()
		{
			var example = SchurComplementExample.CreateExampleNonSymmetricA();
			int n = example.MatrixOrder;
			var originalA = CsrMatrix.CreateFromArrays(n, n, example.MatrixCsr.values, example.MatrixCsr.colIndices,
				example.MatrixCsr.rowOffsets, true);
			(double[] values, int[] colIndices, int[] rowOffsets) expectedA00 = example.Submatrix00Csr;

			int[] mapA00toACsr = { 23, 25, 22, 26, 27, 39, 37, 42, 8, 10, 6, 7, 11, 16, 17, 14, 15, 20, 50, 52, 53, 64, 65, 67 };
			var valuesMapper = new SameSparsityValuesArrayMapper(mapA00toACsr);

			var computedValues = new double[expectedA00.values.Length];
			valuesMapper.CopyValuesArrayToSubmatrix(originalA.RawValues, computedValues);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(expectedA00.values, computedValues);
		}

		[Fact]
		public static void TestCopyValuesArrayToSubmatrixSymmetric()
		{
			var example = SchurComplementExample.CreateExampleSymmetricA();
			int n = example.MatrixOrder;
			var originalA = SymmetricCscMatrix.CreateFromArrays(n, example.MatrixCscSymmetric.values,
				example.MatrixCscSymmetric.rowIndices, example.MatrixCscSymmetric.colOffsets, true);
			(double[] values, int[] colIndices, int[] rowOffsets) expectedA00 = example.Submatrix00Csr;

			int[] mapA00SymCsctoACsr = { 2, 15, 20, 19, 7, 15, 19, 17 };
			var valuesMapper = new SameSparsityValuesArrayMapper(mapA00SymCsctoACsr);

			var computedValues = new double[expectedA00.values.Length];
			valuesMapper.CopyValuesArrayToSubmatrix(originalA.RawValues, computedValues);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(expectedA00.values, computedValues);
		}
	}
}
