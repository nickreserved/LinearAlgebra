using MGroup.LinearAlgebra.Iterative;
using MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient;
using MGroup.LinearAlgebra.Iterative.Preconditioning;
using MGroup.LinearAlgebra.Iterative.Termination;
using MGroup.LinearAlgebra.Iterative.Termination.Iterations;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Tests.TestData;
using MGroup.LinearAlgebra.Tests.Utilities;
using MGroup.LinearAlgebra.Vectors;

using Xunit;

namespace MGroup.LinearAlgebra.Tests.Iterative
{
	/// <summary>
	/// Tests for <see cref="BlockPcgAlgorithm"/>.
	/// </summary>
	public static class BlockPcgTests
	{
		private static readonly MatrixComparer comparer = new MatrixComparer(1E-4);

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestPosDefDenseSystem(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var A = Matrix.CreateFromArray(SymmPosDef10by10.Matrix);
				var b = new Vector(SymmPosDef10by10.Rhs);
				var xExpected = new Vector(SymmPosDef10by10.Lhs);

				var builder = new BlockPcgAlgorithm.Builder();
				builder.ResidualTolerance = 1E-10;
				builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
				builder.BlockSize = 2;
				var pcg = builder.Build();
				var M = new JacobiPreconditioner(A.GetDiagonal());
				Vector xComputed = new Vector(A.NumRows);
				IterativeStatistics stats = pcg.Solve(A, M, b, xComputed, true);
				comparer.AssertEqual(xExpected, xComputed);
			});
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestPosDefSparseSystem(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var A = Matrix.CreateFromArray(SparsePosDef10by10.Matrix);
				var b = new Vector(SparsePosDef10by10.Rhs);
				var xExpected = new Vector(SparsePosDef10by10.Lhs);

				var builder = new BlockPcgAlgorithm.Builder();
				builder.ResidualTolerance = 1E-10;
				builder.MaxIterationsProvider = new PercentageMaxIterationsProvider(1.0);
				var pcg = builder.Build();
				var M = new JacobiPreconditioner(A.GetDiagonal());
				Vector xComputed = new Vector(A.NumRows);
				IterativeStatistics stats = pcg.Solve(A, M, b, xComputed, true);
				comparer.AssertEqual(xExpected, xComputed);
			});
		}

	}
}
