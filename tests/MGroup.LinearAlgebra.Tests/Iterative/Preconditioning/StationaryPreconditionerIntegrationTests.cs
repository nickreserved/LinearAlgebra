namespace MGroup.LinearAlgebra.Tests.Iterative.Preconditioning
{
	using MGroup.LinearAlgebra.Iterative.GeneralizedMinimalResidual;
	using MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient;
	using MGroup.LinearAlgebra.Iterative.Preconditioning;
	using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;

	using Xunit;

	public class StationaryPreconditionerIntegrationTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		private static void TestSorGmres(bool forwardDirection)
		{
			RunGmres(new SorPreconditionerCsr(1.2, forwardDirection));
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		private static void TestGaussSeidelGmres(bool forwardDirection)
		{
			RunGmres(new GaussSeidelPreconditionerCsr(forwardDirection));
		}

		[Fact]
		private static void TestJacobiGmres()
		{
			RunGmres(new JacobiPreconditioner(1E-10));
		}

		[Fact]
		private static void TestJacobiPcg()
		{
			RunPcg(new JacobiPreconditioner(1E-10));
		}

		private static void RunGmres(IPreconditioner preconditioner)
		{
			var A = CsrMatrix.CreateFromDense(Matrix.CreateFromArray(SquareInvertible10by10.Matrix));
			var b = Vector.CreateFromArray(SquareInvertible10by10.Rhs);
			var xExpected = Vector.CreateFromArray(SquareInvertible10by10.Lhs);
			var xComputed = Vector.CreateZero(A.NumRows);

			var builder = new GmresAlgorithm.Builder();
			builder.RelativeTolerance = 1E-7;
			builder.AbsoluteTolerance = 1E-5;
			builder.MaximumIterations = 10;
			GmresAlgorithm gmres = builder.Build();
			preconditioner.UpdateMatrix(A, true);
			var stats = gmres.Solve(A, preconditioner, b, xComputed, true, () => Vector.CreateZero(b.Length));

			var comparer = new MatrixComparer(1E-5);
			comparer.AssertEqual(xExpected, xComputed);
		}

		private static void RunPcg(IPreconditioner preconditioner)
		{
			var A = CsrMatrix.CreateFromArrays(SparsePosDef10by10.Order, SparsePosDef10by10.Order,
				SparsePosDef10by10.CsrValues, SparsePosDef10by10.CsrColIndices, SparsePosDef10by10.CsrRowOffsets, true);
			var b = Vector.CreateFromArray(SparsePosDef10by10.Rhs);
			var xExpected = Vector.CreateFromArray(SparsePosDef10by10.Lhs);
			var xComputed = Vector.CreateZero(A.NumRows);

			var builder = new PcgAlgorithm.Factory();
			builder.ResidualTolerance = 1E-7;
			builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
			var pcg = builder.Build();
			preconditioner.UpdateMatrix(A, true);
			var stats = pcg.Solve(A, preconditioner, b, xComputed, true, () => Vector.CreateZero(b.Length));

			var comparer = new MatrixComparer(1E-5);
			comparer.AssertEqual(xExpected, xComputed);
		}
	}
}
