using System.Collections.Generic;
using System.Diagnostics;
using MGroup.LinearAlgebra.Iterative;
using MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient;
using MGroup.LinearAlgebra.Iterative.Preconditioning;
using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Tests.TestData;
using MGroup.LinearAlgebra.Tests.TestData.FiniteElementMatrices;
using MGroup.LinearAlgebra.Tests.Utilities;
using MGroup.LinearAlgebra.Vectors;
using Xunit;

namespace MGroup.LinearAlgebra.Tests.Iterative
{
	/// <summary>
	/// Tests for <see cref="ReorthogonalizedPcg"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public static class PcgReorthoTests
	{
		private static readonly MatrixComparer comparer = new MatrixComparer(1E-5);

		//[Fact]
		private static void InvestigateNoiseStagnation()
		{
			double noiseWidth = 100;

			int order = 100;
			//var A = Matrix.CreateFromArray(MultiDiagonalMatrices.CreateSymmetric(order, new int[] { 0, 1, 5, 7, 12 }));
			var valueOfEachDiagonal = new Dictionary<int, double>();
			valueOfEachDiagonal[0] = 10.0;
			valueOfEachDiagonal[1] = 4.0;
			valueOfEachDiagonal[5] = 3.0;
			var A = Matrix.CreateFromArray(MultiDiagonalMatrices.CreateSymmetric(order, valueOfEachDiagonal));
			var M = new IdentityPreconditioner();
			//var M = new JacobiPreconditionerDeprecated(A.GetDiagonalAsArray());

			// Method A: Regular PCG
			var pcgBuilder = new PcgAlgorithm.Builder();
			pcgBuilder.ResidualTolerance = 1E-15;
			pcgBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcg = pcgBuilder.Build();

			// Method B: Reorthogonalized PCG, but without keeping direction vectors from previous solutions.
			var pcgReorthoRestartBuilder = new ReorthogonalizedPcg.Builder();
			pcgReorthoRestartBuilder.ResidualTolerance = 1E-15;
			pcgReorthoRestartBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcgReorthoRestart = pcgReorthoRestartBuilder.Build();

			// Method C: Reorthogonalized PCG, where the second solution will reuse direction vectors from the first
			var pcgReorthoBuilder = new ReorthogonalizedPcg.Builder();
			pcgReorthoBuilder.ResidualTolerance = 1E-15;
			pcgReorthoBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcgReortho = pcgReorthoBuilder.Build();

			// Initial rhs
			Vector x0 = new Vector(order);
			x0.SetAll(1);
			Vector x0Expected = x0.Copy();
			Vector b0 = A * x0Expected;

			Vector xA = new Vector(A.NumRows);
			IterativeStatistics statsA = pcg.Solve(A, M, b0, xA, true);
			Debug.WriteLine($"Initial run - method A: iterations = {statsA.NumIterationsRequired}");

			Vector xB = new Vector(A.NumRows);
			IterativeStatistics statsB = pcgReorthoRestart.Solve(A, M, b0, xB, true);
			Debug.WriteLine($"Initial run - method B iterations = {statsB.NumIterationsRequired}");

			Vector xC = new Vector(A.NumRows);
			IterativeStatistics statsC = pcgReortho.Solve(A, M, b0, xC, true);
			Debug.WriteLine($"Initial run - method C: iterations = {statsC.NumIterationsRequired}");

			// Perturbed rhs
			int seed = 345;
			Vector dx = new Vector(RandomMatrices.CreateRandomVector(order, seed));

			Vector x1Expected = x0 + noiseWidth * dx;
			Vector b1 = A * x1Expected;

			xA = new Vector(A.NumRows);
			statsA = pcg.Solve(A, M, b1, xA, true);
			Debug.WriteLine($"2nd run, noise = {noiseWidth} - method A: iterations = {statsA.NumIterationsRequired}");

			xB = new Vector(A.NumRows);
			pcgReorthoRestart.ReorthoCache.Clear();
			statsB = pcgReorthoRestart.Solve(A, M, b1, xB, true);
			Debug.WriteLine($"2nd run, noise = {noiseWidth} - method B iterations = {statsB.NumIterationsRequired}");

			xC = new Vector(A.NumRows);
			statsC = pcgReortho.Solve(A, M, b1, xC, true);
			Debug.WriteLine($"2nd run, noise = {noiseWidth} - method C: iterations = {statsC.NumIterationsRequired}");
		}

		//[Fact]
		private static void InvestigatePFetiDPCoarseProblem2D()
		{
			var A = Matrix.CreateFromArray(PFetiDPCoarseProblem2D.MatrixScc);
			var M = new IdentityPreconditioner();

			// Method A: Regular PCG
			var pcgBuilder = new PcgAlgorithm.Builder();
			pcgBuilder.ResidualTolerance = 1E-20;
			pcgBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcg = pcgBuilder.Build();

			// Method B: Reorthogonalized PCG, but without keeping direction vectors from previous solutions.
			var pcgReorthoRestartBuilder = new ReorthogonalizedPcg.Builder();
			pcgReorthoRestartBuilder.ResidualTolerance = 1E-20;
			pcgReorthoRestartBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcgReorthoRestart = pcgReorthoRestartBuilder.Build();

			// Method C: Reorthogonalized PCG, where the second solution will reuse direction vectors from the first
			var pcgReorthoBuilder = new ReorthogonalizedPcg.Builder();
			pcgReorthoBuilder.ResidualTolerance = 1E-20;
			pcgReorthoBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(50);
			var pcgReortho = pcgReorthoBuilder.Build();

			// Initial rhs
			var b = new Vector(PFetiDPCoarseProblem2D.RhsVectors[0]);
			var xExpected = new Vector(PFetiDPCoarseProblem2D.SolutionVectors[0]);

			Vector xA = new Vector(A.NumRows);
			IterativeStatistics statsA = pcg.Solve(A, M, b, xA, true);
			Assert.True(xExpected.Equals(xA, 1E-10));
			Debug.WriteLine($"Initial run - method A: iterations = {statsA.NumIterationsRequired}");

			Vector xB = new Vector(A.NumRows);
			IterativeStatistics statsB = pcgReorthoRestart.Solve(A, M, b, xB, true);
			Assert.True(xExpected.Equals(xB, 1E-10));
			Debug.WriteLine($"Initial run - method B iterations = {statsB.NumIterationsRequired}");

			Vector xC = new Vector(A.NumRows);
			IterativeStatistics statsC = pcgReortho.Solve(A, M, b, xC, true);
			Assert.True(xExpected.Equals(xC, 1E-10));
			Debug.WriteLine($"Initial run - method C: iterations = {statsC.NumIterationsRequired}");

			// Next rhs
			b = new Vector(PFetiDPCoarseProblem2D.RhsVectors[1]);
			xExpected = new Vector(PFetiDPCoarseProblem2D.SolutionVectors[1]);

			xA = new Vector(A.NumRows);
			statsA = pcg.Solve(A, M, b, xA, true);
			Assert.True(xExpected.Equals(xA, 1E-10));
			Debug.WriteLine($"Initial run - method A: iterations = {statsA.NumIterationsRequired}");

			xB = new Vector(A.NumRows);
			pcgReorthoRestart.ReorthoCache.Clear();
			statsB = pcgReorthoRestart.Solve(A, M, b, xB, true);
			Assert.True(xExpected.Equals(xB, 1E-10));
			Debug.WriteLine($"Initial run - method B iterations = {statsB.NumIterationsRequired}");

			xC = new Vector(A.NumRows);
			statsC = pcgReortho.Solve(A, M, b, xC, true);
			Assert.True(xExpected.Equals(xC, 1E-10));
			Debug.WriteLine($"Initial run - method C: iterations = {statsC.NumIterationsRequired}");
		}

		[Theory]
		[InlineData(0.1, 5, 10)]
		[InlineData(0.01, 5, 20)]
		private static void TestNearbyProblems(double noiseWidth, int maxIterations, int numRhsVectors)
		{
			int order = SymmPosDef10by10.Order;
			var A = Matrix.CreateFromArray(SymmPosDef10by10.Matrix);
			var builder = new ReorthogonalizedPcg.Builder();
			builder.ResidualTolerance = 1E-6;
			builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
			builder.Convergence = new RhsNormalizedConvergence();
			var pcg = builder.Build();
			var M = new JacobiPreconditioner(A.GetDiagonal());

			// Initial run
			Vector x0 = new Vector(order);
			x0.SetAll(1);
			Vector x0Expected = x0.Copy();
			Vector b0 = A * x0Expected;
			Vector x0Computed = new Vector(A.NumRows);
			IterativeStatistics stats0 = pcg.Solve(A, M, b0, x0Computed, true);
			 Debug.WriteLine($"Initial run: iterations = {stats0.NumIterationsRequired}");
			comparer.AssertEqual(x0Expected, x0Computed);

			// Subsequent runs
			int seed = 345;
			for (int i = 0; i < numRhsVectors; ++i)
			{
				Vector dx = new Vector(RandomMatrices.CreateRandomVector(order, seed));
				Vector xExpected = x0 + noiseWidth * dx;
				Vector b = A * xExpected;

				pcg.Clear(); //TODO: preferably do not call this.
				//pcg.ReorthoCache.Clear();

				Vector xComputed = new Vector(A.NumRows);
				IterativeStatistics stats = pcg.Solve(A, M, b, xComputed, true);
				Debug.WriteLine($"Subsequent run: iterations = {stats.NumIterationsRequired}");
				comparer.AssertEqual(xExpected, xComputed);
				Assert.InRange(stats.NumIterationsRequired, 1, maxIterations);
			}
		}

		[Fact]
		private static void TestPosDefDenseSystem()
		{
			var A = Matrix.CreateFromArray(SymmPosDef10by10.Matrix);
			var b = new Vector(SymmPosDef10by10.Rhs);
			var xExpected = new Vector(SymmPosDef10by10.Lhs);

			var builder = new ReorthogonalizedPcg.Builder();
			builder.ResidualTolerance = 1E-7;
			builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
			var pcg = builder.Build();
			var M = new JacobiPreconditioner(A.GetDiagonal());
			Vector xComputed = new Vector(A.NumRows);
			IterativeStatistics stats = pcg.Solve(A, M, b, xComputed, true);
			comparer.AssertEqual(xExpected, xComputed);
		}

		[Fact]
		private static void TestPosDefSparseSystem()
		{
			var A = Matrix.CreateFromArray(SparsePosDef10by10.Matrix);
			var b = new Vector(SparsePosDef10by10.Rhs);
			var xExpected = new Vector(SparsePosDef10by10.Lhs);

			var builder = new ReorthogonalizedPcg.Builder();
			builder.ResidualTolerance = 1E-7;
			builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
			var pcg = builder.Build();
			var M = new JacobiPreconditioner(A.GetDiagonal());
			Vector xComputed = new Vector(A.NumRows);
			IterativeStatistics stats = pcg.Solve(A, M, b, xComputed, true);
			comparer.AssertEqual(xExpected, xComputed);
		}
	}
}
