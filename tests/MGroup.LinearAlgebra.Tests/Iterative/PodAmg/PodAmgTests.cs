namespace MGroup.LinearAlgebra.Tests.Iterative.PodAmg
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.AlgebraicMultiGrid;
	using MGroup.LinearAlgebra.AlgebraicMultiGrid.PodAmg;
	using MGroup.LinearAlgebra.Iterative;
	using MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient;
	using MGroup.LinearAlgebra.Iterative.Preconditioning;
	using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
	using MGroup.LinearAlgebra.Iterative.Termination.Convergence;
	using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;

	using Xunit;

	public static class PodAmgTests
	{
		[Fact]
		public static void TestPodAmgSolver()
		{
			var matrix = Matrix.CreateFromArray(DataSet2.Matrix);
			var csr = CsrMatrix.CreateFromDense(matrix);
			var rhs = Vector.CreateFromArray(DataSet2.Rhs);
			var solutionExpected = Vector.CreateFromArray(DataSet2.Solution);
			var samples = Matrix.CreateFromArray(DataSet2.Samples);
			int numPrincipalComponents = DataSet2.PrincipalComponents.GetLength(1);
			int numPodAmgCyclesExpected = DataSet2.NumPodAmgCycles;

			var solverBuilder = new PodAmgAlgorithm.Builder();
			solverBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(30000);
			solverBuilder.ConvergenceTolerance = 1E-5;
			solverBuilder.ConvergenceCriterion = new SolutionNeverConvergesCriterion();
			solverBuilder.Smoothing = new MultigridLevelSmoothing()
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection:true), 1)
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection:false), 1)
				.SetPostSmoothersSameAsPreSmoothers();

			PodAmgAlgorithm solver = solverBuilder.Create(samples, numPrincipalComponents);
			var solutionComputed = Vector.CreateZero(rhs.Length);
			solver.Initialize(csr);
			IterativeStatistics stats = solver.Solve(rhs, solutionComputed);

			var comparer = new MatrixComparer(1E-12);
			comparer.AssertEqual(solutionExpected, solutionComputed);
			Assert.InRange(stats.NumIterationsRequired, 0, numPodAmgCyclesExpected);
		}

		[Fact]
		public static void TestPodAmgPreconditioner()
		{
			// Input and expected output
			var matrix = Matrix.CreateFromArray(DataSet2.Matrix);
			var csr = CsrMatrix.CreateFromDense(matrix);
			var rhs = Vector.CreateFromArray(DataSet2.Rhs);
			var solutionExpected = Vector.CreateFromArray(DataSet2.Solution);
			int n = matrix.NumRows;
			var samples = Matrix.CreateFromArray(DataSet2.Samples);
			int numPrincipalComponents = DataSet2.PrincipalComponents.GetLength(1);
			int numPodAmgCyclesExpected = DataSet2.NumPodAmgCycles;
			
			// POD-AMG as preconditioner
			var preconditionerFactory = new PodAmgPreconditioner.Factory();
			preconditionerFactory.NumIterations = 1;
			preconditionerFactory.KeepOnlyNonZeroPrincipalComponents = true;
			preconditionerFactory.CreateSmoothers = () => new MultigridLevelSmoothing()
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: true), 1)
				.AddPreSmoother(new GaussSeidelIterationCsr(forwardDirection: false), 1)
				.SetPostSmoothersSameAsPreSmoothers();
			preconditionerFactory.Initialize(samples, numPrincipalComponents);
			IPreconditioner preconditioner = preconditionerFactory.CreatePreconditionerFor(csr);


			// PCG algorithm as solver
			var solverBuilder = new PcgAlgorithm.Builder();
			solverBuilder.ResidualTolerance = 1E-6;
			solverBuilder.MaxIterationsProvider = new FixedMaxIterationsProvider(n);
			var pcg = solverBuilder.Build();

			// Solve
			var solutionComputed = Vector.CreateZero(n);
			IterativeStatistics stats = pcg.Solve(matrix, preconditioner, rhs, solutionComputed, true, () => Vector.CreateZero(n));

			var comparer = new MatrixComparer(1E-6);
			comparer.AssertEqual(solutionExpected, solutionComputed);
		}
	}
}
