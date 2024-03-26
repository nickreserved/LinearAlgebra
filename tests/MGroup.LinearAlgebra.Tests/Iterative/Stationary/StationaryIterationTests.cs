namespace MGroup.LinearAlgebra.Tests.Iterative.Stationary
{
	using System;

	using MGroup.LinearAlgebra.Iterative.Stationary;
	using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;
	using Xunit;

	public class StationaryIterationTests
	{
		[Fact]
		private static void TestGaussSeidelBackIteration()
		{
			var stationaryIteration = new GaussSeidelIterationCsr(forwardDirection: false);
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// (U+D)*x(t+1) = b - L*x(t)
				SparseMatrix L = decomp.GetL();
				SparseMatrix UplusD = decomp.GetCombination("U+D", 0, 1, 1);
				return UplusD.SolveBackSubstitution(rhs - L * solution);
			});
		}

		[Fact]
		private static void TestGaussSeidelForwardIteration()
		{
			var stationaryIteration = new GaussSeidelIterationCsr(forwardDirection: true);
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// (L+D)*x(t+1) = b - U*x(t)
				SparseMatrix U = decomp.GetU();
				SparseMatrix LplusD = decomp.GetCombination("L+D", 1, 1, 0);
				return LplusD.SolveForwardSubstitution(rhs - U * solution);
			});
		}

		[Fact]
		private static void TestJacobiIteration()
		{
			var stationaryIteration = new JacobiIterationCsr();
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// D*x(t+1) = b - (U+L)*x(t)
				SparseMatrix D = decomp.GetD();
				SparseMatrix LplusU = decomp.GetCombination("L+U", 1, 0, 1);
				return D.SolveDiagonal(rhs - LplusU * solution);
			});
		}

		[Fact]
		private static void TestSorBackIteration()
		{
			double omega = 1.2;
			var stationaryIteration = new SorIterationCsr(omega, forwardDirection: false);
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// (D + ωU) * x(t + 1) = ω * (b - L * x(t)) + (1 - ω)*D*x(t)
				SparseMatrix L = decomp.GetL();
				SparseMatrix D = decomp.GetD();
				SparseMatrix lhsMatrix = decomp.GetCombination("D+ωU", 0, 1, omega);
				return lhsMatrix.SolveBackSubstitution(omega * (rhs - L * solution) + (1 - omega) * (D * solution));
			});
		}

		[Fact]
		private static void TestSorForwardIteration()
		{
			double omega = 1.2;
			var stationaryIteration = new SorIterationCsr(omega, forwardDirection: true);
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// (D + ωL) * x(t + 1) = ω * (b - U * x(t)) + (1 - ω)*D*x(t)
				SparseMatrix U = decomp.GetU();
				SparseMatrix D = decomp.GetD();
				SparseMatrix lhsMatrix = decomp.GetCombination("D+ωL", omega, 1, 0);
				return lhsMatrix.SolveForwardSubstitution(omega * (rhs - U * solution) + (1 - omega) * (D * solution));
			});
		}

		[Fact]
		private static void TestSsorIteration()
		{
			double omega = 1.2;
			var stationaryIteration = new SsorIterationCsr(omega);
			RunTwoApplications(stationaryIteration, (StationaryAlgorithmDecomposition decomp, Vector rhs, Vector solution) =>
			{
				// (D+ωL) * x(t+1/2) = ω*(b -U*x(t)) +(1-ω)D*x(t), (D+ωU) * x(t+1) = ω*(b -L*x(t+1/2)) +(1-ω)D*x(t+1/2)
				SparseMatrix L = decomp.GetL();
				SparseMatrix U = decomp.GetU();
				SparseMatrix D = decomp.GetD();
				SparseMatrix DplusWL = decomp.GetCombination("D+ωL", omega, 1, 0);
				SparseMatrix DplusWU = decomp.GetCombination("D+ωU", 0, 1, omega);
				solution = DplusWL.SolveForwardSubstitution(omega * (rhs - U * solution) + (1 - omega) * (D * solution));
				return DplusWU.SolveBackSubstitution(omega * (rhs - L * solution) + (1 - omega) * (D * solution));
			});
		}

		private static void RunTwoApplications(IStationaryIteration stationaryIteration,
			Func<StationaryAlgorithmDecomposition, Vector, Vector, Vector> applyIterationUsingMatrixForm)
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

			// Prepare stationary iteration
			var decomp = new StationaryAlgorithmDecomposition(SparseMatrix.CreateFromMatrix(csrMatrix));
			stationaryIteration.UpdateMatrix(csrMatrix, true);

			// First application
			xExpected = applyIterationUsingMatrixForm(decomp, b, xExpected);
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);

			// Second application
			xExpected = applyIterationUsingMatrixForm(decomp, b, xExpected);
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}
	}
}
