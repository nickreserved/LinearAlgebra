using System.Text;

using MGroup.LinearAlgebra.Iterative;
using MGroup.LinearAlgebra.Iterative.Stationary;
using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
using MGroup.LinearAlgebra.Iterative.Termination.Convergence;
using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Output;
using MGroup.LinearAlgebra.Tests.TestData;
using MGroup.LinearAlgebra.Tests.Utilities;
using MGroup.LinearAlgebra.Vectors;

using Xunit;

namespace MGroup.LinearAlgebra.Tests.Iterative.Stationary
{
	/// <summary>
	/// Tests for <see cref="GaussSeidel"/>.
	/// </summary>
	public static class GaussSeidelTests
	{
		[Fact]
		private static void TestSingleIterationBack()
		{
			// Setup comparison code
			double entrywiseTolerance = 1E-15;
			var comparer = new MatrixComparer(entrywiseTolerance);

			// Initialize matrices and vectors
			var csrMatrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
					SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var b = Vector.CreateFromArray(SparsePosDef10by10.Rhs);
			var xExpected = Vector.CreateZero(b.Length);
			var xComputed = Vector.CreateZero(b.Length);

			// Prepare test iteration
			// (U+D)*x(t+1) = L*x(t)+b
			var A = SparseMatrix.CreateFromMatrix(csrMatrix);
			SparseMatrix UplusD = A.ExtractUpperTriangleAndDiagonal();
			SparseMatrix L = A.ExtractLowerTriangle();

			// Prepare actual iteration
			IStationaryIteration stationaryIteration = new GaussSeidelIterationCsr(forwardDirection:false);
			stationaryIteration.UpdateMatrix(csrMatrix, true);

			// i = 0: x = 0
			xExpected = UplusD.SolveBackSubstitution(b - L.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);

			// i = 1: x = x1
			xExpected = UplusD.SolveBackSubstitution(b - L.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}

		[Fact]
		private static void TestSingleIterationForward()
		{
			// Setup comparison code
			double entrywiseTolerance = 1E-15;
			var comparer = new MatrixComparer(entrywiseTolerance);

			// Initialize matrices and vectors
			var csrMatrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
					SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var b = Vector.CreateFromArray(SparsePosDef10by10.Rhs);
			var xExpected = Vector.CreateZero(b.Length);
			var xComputed = Vector.CreateZero(b.Length);

			// Prepare test iteration
			// (L+D)*x(t+1) = U*x(t)+b
			var A = SparseMatrix.CreateFromMatrix(csrMatrix);
			SparseMatrix LplusD = A.ExtractLowerTriangleAndDiagonal();
			SparseMatrix U = A.ExtractUpperTriangle();

			// Prepare actual iteration
			IStationaryIteration stationaryIteration = new GaussSeidelIterationCsr(forwardDirection:true);
			stationaryIteration.UpdateMatrix(csrMatrix, true);

			// i = 0: x = 0
			xExpected = LplusD.SolveForwardSubstitution(b - U.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);

			// i = 1: x = x1
			xExpected = LplusD.SolveForwardSubstitution(b - U.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}

		[Theory]
		[InlineData(true, 10, 1E-9, 1E-5)]
		[InlineData(false, 10, 1E-8, 1E-5)]
		private static void TestSystemSolution(bool forwardGaussSeidel, int numIterations, double gsConvergenceTolerance, double entrywiseTolerance)
		{
			var matrix = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
					SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var b = Vector.CreateFromArray(SparsePosDef10by10.Rhs);
			var xExpected = Vector.CreateFromArray(SparsePosDef10by10.Lhs);

			IStationaryIteration stationaryIteration = new GaussSeidelIterationCsr(forwardGaussSeidel);
			var builder = new StationaryIterativeMethod.Factory(stationaryIteration);
			//builder.ConvergenceCriterion = new SolutionNeverConvergesCriterion(); // We would use this, but to test we need to track the convergence rate.
			builder.ConvergenceCriterion = new AbsoluteSolutionConvergenceCriterion();
			builder.ConvergenceTolerance = 0.0;
			builder.MaxIterationsProvider = new FixedMaxIterationsProvider(numIterations);
			var gs = builder.Build();
			var xComputed = Vector.CreateZero(b.Length);

			var stats = gs.Solve(matrix, b, xComputed);

			Assert.Equal(numIterations, stats.NumIterationsRequired);
			Assert.InRange(stats.ConvergenceCriterion.value, 0.0, gsConvergenceTolerance);

			var comparer = new MatrixComparer(entrywiseTolerance);
			comparer.AssertEqual(xExpected, xComputed);
		}
	}
}
