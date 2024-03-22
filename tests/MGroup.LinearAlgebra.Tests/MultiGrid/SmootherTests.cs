namespace MGroup.LinearAlgebra.Tests.MultiGrid
{
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
		//[Fact] Jacobi smoother causes POD-2G to diverge
		public static void TestJacobiSmoother()
		{
			MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new JacobiIterationCsr(), 1)
				.AddPostSmoother(new JacobiIterationCsr(), 1);

			TestPod2gSolver(smoothing, 1000, 1E-12);
		}

		[Fact]
		public static void TestGaussSeidelSmoother()
		{
			MultigridLevelSmoothing smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: true), 1)
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: false), 1)
				.SetPostSmoothersSameAsPreSmoothers();

			TestPod2gSolver(smoothing, 119, 1E-12);
		}

		private static void TestPod2gSolver(MultigridLevelSmoothing smoothing, int numAmgCycles, double entrywiseTol)
		{
			var matrix = Matrix.CreateFromArray(DataSet2.Matrix);
			var csr = CsrMatrix.CreateFromDense(matrix);
			var rhs = Vector.CreateFromArray(DataSet2.Rhs);
			var solutionExpected = Vector.CreateFromArray(DataSet2.Solution);
			var samples = Matrix.CreateFromArray(DataSet2.Samples);
			var numPrincipalComponents = DataSet2.PrincipalComponents.GetLength(1);

			var solverBuilder = new PodAmgAlgorithm.Factory();
			solverBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(30000);
			solverBuilder.ConvergenceTolerance = 1E-5;
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
