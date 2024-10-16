namespace MGroup.LinearAlgebra.Tests.MultiGrid
{
	using System.Diagnostics;

	using MGroup.LinearAlgebra.AlgebraicMultiGrid;
	using MGroup.LinearAlgebra.AlgebraicMultiGrid.PodAmg;
	using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
	using MGroup.LinearAlgebra.Iterative.Termination.Convergence;
	using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.MultiGrid.PodAmg;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;

	using Xunit;

	public static class SmootherTests
	{
		[Fact]
		public static void TestGaussSeidelSmoother()
		{
			MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: true), 1)
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: false), 1)
				.SetPostSmoothersSameAsPreSmoothers();

			//MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
			//	.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: true), 1)
			//	.AddPostSmoother(new GaussSeidelIterationCsr(forwardDirection: false), 1);

			TestPod2gSolver(smoothing, 119, 1E-7, 1E-5);
		}

		[Fact]
		public static void TestSorSmoother()
		{
			double omega = 1.1;
			MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new SorIterationCsr(omega, forwardDirection: true), 1)
				.AddPreSmoother(new SorIterationCsr(omega, forwardDirection: false), 1)
				.SetPostSmoothersSameAsPreSmoothers();

			//MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
			//	.AddPreSmoother(new SorIterationCsr(omega, forwardDirection: true), 1)
			//	.AddPostSmoother(new SorIterationCsr(omega, forwardDirection: false), 1);

			TestPod2gSolver(smoothing, 123, 1E-7, 1E-5);
		}

		[Fact]
		public static void TestSsorSmoother()
		{
			double omega = 1.1;
			MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new SsorIterationCsr(omega), 1)
				.SetPostSmoothersSameAsPreSmoothers();

			TestPod2gSolver(smoothing, 123, 1E-7, 1E-5);
		}

		private static void TestPod2gSolver(MultigridLevelSmoothing smoothing, int numAmgCycles, double entrywiseTol, 
			double convergenceTol)
		{
			var matrix = Matrix.CreateFromArray(DataSet2.Matrix);
			var csr = CsrMatrix.CreateFromDense(matrix);
			var rhs = Vector.CreateFromArray(DataSet2.Rhs);
			var solutionExpected = Vector.CreateFromArray(DataSet2.Solution);
			var samples = Matrix.CreateFromArray(DataSet2.Samples);
			var numPrincipalComponents = DataSet2.PrincipalComponents.GetLength(1);

			var solverBuilder = new PodAmgAlgorithm.Factory();
			solverBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(30000);
			solverBuilder.ConvergenceTolerance = convergenceTol;
			solverBuilder.ConvergenceCriterion = new SolutionNeverConvergesCriterion();
			solverBuilder.Smoothing = smoothing;

			var solver = solverBuilder.Create(samples, numPrincipalComponents);
			var solutionComputed = Vector.CreateZero(rhs.Length);
			solver.Initialize(csr);
			var stats = solver.Solve(rhs, solutionComputed);

			var comparer = new MatrixComparer(entrywiseTol);
			comparer.AssertEqual(solutionExpected, solutionComputed);
			Assert.InRange(stats.NumIterationsRequired, 0, numAmgCycles);
		}
	}
}
