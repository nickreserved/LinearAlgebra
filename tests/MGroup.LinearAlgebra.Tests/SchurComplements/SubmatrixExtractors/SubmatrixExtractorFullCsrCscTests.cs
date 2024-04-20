namespace MGroup.LinearAlgebra.Tests.SchurComplements.SubmatrixExtractors
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	public static class SubmatrixExtractorFullCsrCscTests
	{
		[Fact]
		public static void TestClear()
		{
			var extractor = new SubmatrixExtractorFullCsrCsc();
			var example = SubmatricesExample.CreateExampleNonSymmetricA();

			int n = example.MatrixOrder;
			(double[] values, int[] colIndices, int[] rowOffsets) A = example.MatrixCsr;
			var csrA = CsrMatrix.CreateFromArrays(n, n, A.values, A.colIndices, A.rowOffsets, true);

			int[] indices0 = example.Indices0;
			int[] indices1 = example.Indices1;
			extractor.ExtractSubmatrices(csrA, indices0, indices1);
			TestExtractorForExample(extractor, example);

			extractor.Clear();

			Assert.Null(extractor.Submatrix00);
			Assert.Null(extractor.Submatrix01);
			Assert.Null(extractor.Submatrix10);
			Assert.Null(extractor.Submatrix11);
		}

		[Fact]
		public static void TestExtractSubmatricesMultiple()
		{
			var extractor = new SubmatrixExtractorFullCsrCsc();
			var example = SubmatricesExample.CreateExampleNonSymmetricA();

			int n = example.MatrixOrder;
			(double[] values, int[] colIndices, int[] rowOffsets) A = example.MatrixCsr;
			var csr = CsrMatrix.CreateFromArrays(n, n, A.values, A.colIndices, A.rowOffsets, true);

			int[] indices0 = example.Indices0;
			int[] indices1 = example.Indices1;
			extractor.ExtractSubmatrices(csr, indices0, indices1);

			TestExtractorForExample(extractor, example);

			// 2nd matrix 3*A. Pattern is the same
			double factor = 3.0;
			csr.ScaleIntoThis(factor);
			example.ScalingFactor = factor;
			extractor.ExtractSubmatrices(csr, indices0, indices1);
			TestExtractorForExample(extractor, example);

			// 3rd matrix B is completely different
			example = SubmatricesExample.CreateExampleNonSymmetricB();
			(double[] values, int[] colIndices, int[] rowOffsets) B = example.MatrixCsr;
			csr = CsrMatrix.CreateFromArrays(n, n, B.values, B.colIndices, B.rowOffsets, true);

			// Without first clearing, it will fail (should it though?)
			indices0 = example.Indices0;
			indices1 = example.Indices1;
			Assert.Throws<InvalidOperationException>(() => extractor.ExtractSubmatrices(csr, indices0, indices1));

			extractor.Clear();
			extractor.ExtractSubmatrices(csr, indices0, indices1);

			TestExtractorForExample(extractor, example);
		}

		[Fact]
		public static void TestExtractSubmatricesOnce()
		{
			var extractor = new SubmatrixExtractorFullCsrCsc();
			var example = SubmatricesExample.CreateExampleNonSymmetricA();

			int n = example.MatrixOrder;
			(double[] values, int[] colIndices, int[] rowOffsets) A = example.MatrixCsr;
			var csrA = CsrMatrix.CreateFromArrays(n, n, A.values, A.colIndices, A.rowOffsets, true);

			int[] indices0 = example.Indices0;
			int[] indices1 = example.Indices1;
			extractor.ExtractSubmatrices(csrA, indices0, indices1);

			TestExtractorForExample(extractor, example);
		}

		private static void TestExtractorForExample(SubmatrixExtractorFullCsrCsc extractor, SubmatricesExample example)
		{
			double[] expected00 = example.Submatrix00FullColMajor;
			(double[] values, int[] colIndices, int[] rowOffsets) expected01 = example.Submatrix01Csr;
			(double[] values, int[] colIndices, int[] rowOffsets) expected10 = example.Submatrix10Csr;
			(double[] values, int[] rowIndices, int[] colOffsets) expected11 = example.Submatrix11Csc;

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(expected00, extractor.Submatrix00.RawData);

			comparer.AssertEqual(expected01.values, extractor.Submatrix01.RawValues);
			comparer.AssertEqual(expected01.colIndices, extractor.Submatrix01.RawColIndices);
			comparer.AssertEqual(expected01.rowOffsets, extractor.Submatrix01.RawRowOffsets);

			comparer.AssertEqual(expected10.values, extractor.Submatrix10.RawValues);
			comparer.AssertEqual(expected10.colIndices, extractor.Submatrix10.RawColIndices);
			comparer.AssertEqual(expected10.rowOffsets, extractor.Submatrix10.RawRowOffsets);

			comparer.AssertEqual(expected11.values, extractor.Submatrix11.RawValues);
			comparer.AssertEqual(expected11.rowIndices, extractor.Submatrix11.RawRowIndices);
			comparer.AssertEqual(expected11.colOffsets, extractor.Submatrix11.RawColOffsets);
		}
	}
}
