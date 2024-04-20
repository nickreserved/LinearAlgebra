namespace MGroup.LinearAlgebra.Tests.SchurComplements.SubmatrixExtractors
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	public static class SubmatrixExtractorCscSymBaseTests
	{
		[Theory]
		[InlineData("A", 0)]
		[InlineData("A", 1)]
		[InlineData("B", 0)]
		[InlineData("B", 1)]
		public static void TestExtractSparsityPattern(string exampleLetter, int indexGroupNumber)
		{
			SubmatricesExample example;
			if (exampleLetter == "A")
			{
				example = SubmatricesExample.CreateExampleSymmetricA();
			}
			else if (exampleLetter == "B")
			{
				example = SubmatricesExample.CreateExampleSymmetricB();
			}
			else
			{
				throw new NotImplementedException();
			}

			(double[] values, int[] rowIndices, int[] colOffsets) submatrixExpected;
			int[] rowsColsToKeep;
			if (indexGroupNumber == 0)
			{
				rowsColsToKeep = example.Indices0;
				submatrixExpected = example.Submatrix00CscSymmetric;
			}
			else
			{
				rowsColsToKeep = example.Indices1;
				submatrixExpected = example.Submatrix11CscSymmetric;
			}

			int n = example.MatrixOrder;
			(double[] values, int[] rowIndices, int[] colOffsets) matrix = example.MatrixCscSymmetric;
			var symmCsc = SymmetricCscMatrix.CreateFromArrays(n, matrix.values, matrix.rowIndices, matrix.colOffsets, true);

			SubmatrixExtractorCscSymBase extractor = new SubmatrixExtractorCsrCscSym();
			(int[] rowIndices, int[] colOffsets) subatrix00CscSym = extractor.ExtractSparsityPattern(symmCsc, rowsColsToKeep);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(submatrixExpected.rowIndices, subatrix00CscSym.rowIndices);
			comparer.AssertEqual(submatrixExpected.colOffsets, subatrix00CscSym.colOffsets);
		}
	}
}
