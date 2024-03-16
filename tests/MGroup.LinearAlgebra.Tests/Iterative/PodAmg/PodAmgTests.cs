namespace MGroup.LinearAlgebra.Tests.Iterative.PodAmg
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Iterative;
	using MGroup.LinearAlgebra.Iterative.AlgebraicMultiGrid;
	using MGroup.LinearAlgebra.Iterative.AlgebraicMultiGrid.PodAmg;
	using MGroup.LinearAlgebra.Iterative.AlgebraicMultiGrid.Smoothing;
	using MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient;
	using MGroup.LinearAlgebra.Iterative.Preconditioning;
	using MGroup.LinearAlgebra.Iterative.StationaryPoint.GaussSeidel;
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
			solverBuilder.SmootherBuilder = new GaussSeidelSmoother.Builder(
				new GaussSeidelIterationCsrSerial.Builder(), GaussSeidelSweepDirection.Symmetric, numIterations: 1);

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
			preconditionerFactory.SmootherBuilder = new GaussSeidelSmoother.Builder(
				new GaussSeidelIterationCsrSerial.Builder(), GaussSeidelSweepDirection.Symmetric, numIterations: 1);
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
