namespace MGroup.LinearAlgebra.Tests.Matrices
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;
	using Xunit;

	/// <summary>
	/// Tests for <see cref="FullMatrixRowMajor"/>.
	/// </summary>
	public static class FullMatrixRowMajorTests
	{
		[Fact]
		public static void TestConstructorFrom2dArray()
		{
			double[,] array2D = RectangularFullRank10by5.Matrix;
			var matrix = FullMatrixRowMajor.CreateFromArray(array2D);

			// Compare
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(array2D, matrix);
		}

		[Fact]
		public static void TestProperties()
		{
			var matrix = FullMatrixRowMajor.CreateFromArray(10, 5, RectangularFullRank10by5.MatrixRowMajor, true);

			Assert.Equal(10, matrix.NumRows);
			Assert.Equal(5, matrix.NumColumns);

			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(RectangularFullRank10by5.MatrixRowMajor, matrix.RawData);
		}

		[Fact]
		public static void TestIndexerGet()
		{
			double[,] array2D = RectangularFullRank10by5.Matrix;
			var matrix = FullMatrixRowMajor.CreateFromArray(10, 5, RectangularFullRank10by5.MatrixRowMajor, true);

			// Compare
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(array2D, matrix);
		}

		[Fact]
		public static void TestIndexerSet()
		{
			// Expected
			double[,] array2D = RectangularFullRank10by5.Matrix;
			int m = array2D.GetLength(0);
			int n = array2D.GetLength(1);

			// Computed
			var matrix = FullMatrixRowMajor.CreateZero(m, n);
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					matrix[i, j] = array2D[i, j];
				}
			}

			// Compare
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(array2D, matrix);
		}

		[Fact]
		public static void TestMatrixVectorMultiplications()
		{
			var mvChecker = new MatrixDenseVectorMultiplicationChecker(
				(A, x, tranpose) => ((FullMatrixRowMajor)A).Multiply(x, tranpose),
				(A, x, y, tranpose) => ((FullMatrixRowMajor)A).MultiplyIntoResult(x, y, tranpose));
			mvChecker.Tolerance = 1E-13;

			// rectangular 10-by-5
			var A1 = FullMatrixRowMajor.CreateFromArray(RectangularFullRank10by5.Matrix);
			mvChecker.CheckAllMultiplications(A1, RectangularFullRank10by5.Lhs5, RectangularFullRank10by5.Rhs10, false);

			// rectangular 5-by-10
			mvChecker.CheckAllMultiplications(A1, RectangularFullRank10by5.Lhs10, RectangularFullRank10by5.Rhs5, true);

			// square invertible 10-by-10
			var A3 = FullMatrixRowMajor.CreateFromArray(SquareInvertible10by10.Matrix);
			mvChecker.CheckAllMultiplications(A3, SquareInvertible10by10.Lhs, SquareInvertible10by10.Rhs, false);

			// square singular 10-by-10 (rank = 8)
			var A4 = FullMatrixRowMajor.CreateFromArray(SquareSingular10by10.Matrix);
			mvChecker.CheckAllMultiplications(A4, SquareSingular10by10.Lhs, SquareSingular10by10.Rhs, false);

			// square singular 10-by-10 (rank = 9)
			var A5 = FullMatrixRowMajor.CreateFromArray(SquareSingularSingleDeficiency10by10.Matrix);
			mvChecker.CheckAllMultiplications(A5, SquareSingularSingleDeficiency10by10.Lhs, 
				SquareSingularSingleDeficiency10by10.Rhs, false);
		}

		[Fact]
		public static void TestMatrixVectorMultiplicationIntoResult()
		{
			var comparer = new MatrixComparer(1E-13);

			// rectangular 10-by-5
			var A1 = FullMatrixRowMajor.CreateFromArray(RectangularFullRank10by5.Matrix);
			var x1 = Vector.CreateFromArray(RectangularFullRank10by5.Lhs5);
			double[] b1Expected = RectangularFullRank10by5.Rhs10;
			var b1Computed = Vector.CreateZero(b1Expected.Length);
			A1.MultiplyIntoResult(x1, b1Computed, false);
			comparer.AssertEqual(b1Expected, b1Computed);

			// rectangular 5-by-10
			double[,] fullRank5by10 = MatrixOperations.Transpose(RectangularFullRank10by5.Matrix);
			var x2 = Vector.CreateFromArray(RectangularFullRank10by5.Lhs10);
			double[] b2Expected = RectangularFullRank10by5.Rhs5;
			Vector b2Computed = A1.Multiply(x2, true);
			comparer.AssertEqual(b2Expected, b2Computed);

			// square invertible 10-by-10
			var A3 = FullMatrixRowMajor.CreateFromArray(SquareInvertible10by10.Matrix);
			var x3 = Vector.CreateFromArray(SquareInvertible10by10.Lhs);
			var b3Expected = Vector.CreateFromArray(SquareInvertible10by10.Rhs);
			Vector b3Computed = A3.Multiply(x3, false);
			comparer.AssertEqual(b3Expected, b3Computed);

			// square singular 10-by-10 (rank = 8)
			var A4 = FullMatrixRowMajor.CreateFromArray(SquareSingular10by10.Matrix);
			var x4 = Vector.CreateFromArray(SquareSingular10by10.Lhs);
			double[] b4Expected = SquareSingular10by10.Rhs;
			Vector b4Computed = A4.Multiply(x4, false);
			comparer.AssertEqual(b4Expected, b4Computed);

			// square singular 10-by-10 (rank = 9)
			var A5 = FullMatrixRowMajor.CreateFromArray(SquareSingularSingleDeficiency10by10.Matrix);
			var x5 = Vector.CreateFromArray(SquareSingularSingleDeficiency10by10.Lhs);
			double[] b5Expected = SquareSingularSingleDeficiency10by10.Rhs;
			Vector b5Computed = A5.Multiply(x5, false);
			comparer.AssertEqual(b5Expected, b5Computed);
		}

		[Fact]
		public static void TestSetRow()
		{
			double[,] array2D = RectangularFullRank10by5.Matrix;
			var matrix = FullMatrixRowMajor.CreateFromArray(10, 5, RectangularFullRank10by5.MatrixRowMajor, true);

			// Expected
			int rowIdx = 3;
			double[] newRowValues = { 1.1, 2.2, 3.3, 4.4, 5.5 };
			for (int j = 0; j < newRowValues.Length; j++)
			{
				array2D[rowIdx, j] = newRowValues[j];
			}

			// Computed
			matrix.SetRow(rowIdx, Vector.CreateFromArray(newRowValues));

			// Compare
			var comparer = new MatrixComparer(1E-15);
			comparer.AssertEqual(array2D, matrix);
		}
	}
}
