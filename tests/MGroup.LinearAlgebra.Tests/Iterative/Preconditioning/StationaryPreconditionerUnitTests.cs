namespace MGroup.LinearAlgebra.Tests.Iterative.Preconditioning
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using MGroup.LinearAlgebra.Iterative.Preconditioning;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Reduction;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;
	using Xunit;

	public class StationaryPreconditionerUnitTests
	{
		[Fact]
		private static void TestGaussSeidelBackPreconditioner()
		{
			var preconditioner = new GaussSeidelPreconditioner(forwardDirection: false);
			RunTwoApplications(preconditioner, (StationaryAlgorithmDecomposition decomp, Vector rhs) =>
			{
				SparseMatrix UplusD = decomp.GetCombination("U+D", 0, 1, 1);
				return UplusD.SolveBackSubstitution(rhs);
			});
		}

		[Fact]
		private static void TestGaussSeidelForwardPreconditioner()
		{
			var preconditioner = new GaussSeidelPreconditioner(forwardDirection: true);
			RunTwoApplications(preconditioner, (StationaryAlgorithmDecomposition decomp, Vector rhs) =>
			{
				SparseMatrix LplusD = decomp.GetCombination("L+D", 1, 1, 0);
				return LplusD.SolveForwardSubstitution(rhs);
			});
		}

		[Fact]
		private static void TestJacobiPreconditioner()
		{
			var preconditioner = new JacobiPreconditioner(1E-10);
			RunTwoApplications(preconditioner, (StationaryAlgorithmDecomposition decomp, Vector rhs) =>
			{
				SparseMatrix D = decomp.GetD();
				return D.SolveDiagonal(rhs);
			});
		}

		[Fact]
		private static void TestJacobiPreconditioner1Application()
		{
			MatrixComparer comparer = new MatrixComparer(1E-10);
			var matrix = Matrix.CreateFromArray(SquareSingular10by10.Matrix);
			var preconditioner = new JacobiPreconditioner();
			preconditioner.UpdateMatrix(matrix, true);

			var b = Vector.CreateWithValue(10, 1.0);
			var xExpected = Vector.CreateFromArray(new double[]
			{
				1, 3.000300030003000, 0.25, 0.2, 4, 0.111111111111111, 0.125, 0.142857142857143, 1, 0.5
			});
			var xComputed = Vector.CreateZero(10);

			preconditioner.SolveLinearSystem(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}

		private static void RunTwoApplications(IPreconditioner preconditioner,
			Func<StationaryAlgorithmDecomposition, Vector, Vector> preconditionWithMatrixForm)
		{
			// Setup comparison code
			double entrywiseTolerance = 1E-15;
			var comparer = new MatrixComparer(entrywiseTolerance);

			// Initialize matrices and vectors
			var csrMatrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
					SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var y1 = Vector.CreateFromArray(SparsePosDef10by10.Rhs);
			double max = y1.MaxAbsolute();
			double min = y1.MinAbsolute();
			Vector y2 = y1.Scale(0.1);
			y2.DoToAllEntriesIntoThis(x => x + 0.01 * (max - min) / max);
			var xExpected = Vector.CreateZero(y1.Length);
			var xComputed = Vector.CreateZero(y1.Length);

			// Prepare test preconditioner
			var decomp = new StationaryAlgorithmDecomposition(SparseMatrix.CreateFromMatrix(csrMatrix));

			// Prepare actual preconditioner
			preconditioner.UpdateMatrix(csrMatrix, true);

			// First try
			xExpected = preconditionWithMatrixForm(decomp, y1);
			preconditioner.SolveLinearSystem(y1, xComputed);
			comparer.AssertEqual(xExpected, xComputed);

			// Second try
			xExpected = preconditionWithMatrixForm(decomp, y2);
			preconditioner.SolveLinearSystem(y2, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}

	}
}
