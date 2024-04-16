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
		public static void TestCsrToTriangularUpper()
		{
			var csrMatrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
				SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var upperExpected = TriangularUpper.CreateFromArray(SparsePosDef10by10.Matrix);

			TriangularUpper upperComputed = csrMatrix.ExtractUpperAndDiagonalToPacked();

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(upperExpected, upperComputed);
		}

		[Fact]
		public static void TestGeneralToCsc()
		{
			IIndexable2D matrix = Matrix.CreateFromArray(SparseRectangular10by5.Matrix);
			var cscExpected = CscMatrix.CreateFromArrays(SparseRectangular10by5.NumRows, SparseRectangular10by5.NumCols, 
				SparseRectangular10by5.CscValues, SparseRectangular10by5.CscRowIndices, SparseRectangular10by5.CscColOffsets, 
				true);

			CscMatrix cscComputed = matrix.ConvertToCsc();
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(cscExpected, cscComputed);
		}

		[Fact] 
		public static void TestGeneralToCsr()
		{
			IIndexable2D matrix = Matrix.CreateFromArray(SparseRectangular10by5.Matrix);
			var csrExpected = CsrMatrix.CreateFromArrays(SparseRectangular10by5.NumRows, SparseRectangular10by5.NumCols,
				SparseRectangular10by5.CsrValues, SparseRectangular10by5.CsrColIndices, SparseRectangular10by5.CsrRowOffsets,
				true);

			CsrMatrix cscComputed = matrix.ConvertToCsr();
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(csrExpected, cscComputed);
		}

		[Fact]
		public static void TestGetSubmatrixAsCsc()
		{
			IIndexable2D matrix = Matrix.CreateFromArray(SparseRectangular10by5.Matrix);
			int[] rows = { 1, 2, 4, 9 };
			int[] cols = { 0, 4, 2 };
			var submatrixExpected = CscMatrix.CreateFromArrays(4, 3,
				SparseRectangular10by5.SubmatrixRows1249Cols042_CscValues, 
				SparseRectangular10by5.SubmatrixRows1249Cols042_CscRowIndices, 
				SparseRectangular10by5.SubmatrixRows1249Cols042_CscColOffsets,
				true);

			CscMatrix submatrixComputed = matrix.GetSubmatrixAsCsc(rows, cols, 0);
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(submatrixExpected, submatrixComputed);
		}

		[Fact]
		public static void TestGetSubmatrixAsCsr()
		{
			IIndexable2D matrix = Matrix.CreateFromArray(SparseRectangular10by5.Matrix);
			int[] rows = { 1, 2, 4, 9 };
			int[] cols = { 0, 4, 2 };
			var submatrixExpected = CsrMatrix.CreateFromArrays(4, 3,
				SparseRectangular10by5.SubmatrixRows1249Cols042_CsrValues,
				SparseRectangular10by5.SubmatrixRows1249Cols042_CsrColIndices,
				SparseRectangular10by5.SubmatrixRows1249Cols042_CsrRowOffsets,
				true);

			CsrMatrix submatrixComputed = matrix.GetSubmatrixAsCsr(rows, cols, 0);
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(submatrixExpected, submatrixComputed);
		}
	}
}
