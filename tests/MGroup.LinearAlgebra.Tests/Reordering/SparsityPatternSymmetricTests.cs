namespace MGroup.LinearAlgebra.Tests.Reordering
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Output;
	using MGroup.LinearAlgebra.Reordering;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;

	using Xunit;

	/// <summary>
	/// Tests for <see cref="SparsityPatternSymmetric"/>.
	/// </summary>
	public static class SparsityPatternSymmetricTests
	{
		private static readonly MatrixComparer comparer = new MatrixComparer(1E-13);

		[Fact]
		private static void TestConnectIndices()
		{
			int n = GlobalMatrixAssembly.GlobalOrder;
			var dense = Matrix.CreateFromArray(GlobalMatrixAssembly.GlobalMatrix);
			var pattern = SparsityPatternSymmetric.CreateEmpty(n);
			pattern.ConnectIndices(GlobalMatrixAssembly.GlobalIndices1, true);
			pattern.ConnectIndices(GlobalMatrixAssembly.GlobalIndices2, true);
			pattern.ConnectIndices(GlobalMatrixAssembly.GlobalIndices3, true);

			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < n; ++j)
				{
					bool denseHasZero = dense[i, j] == 0.0;
					bool patternHasZero = !pattern.IsNonZero(i, j);
					Assert.True(patternHasZero == denseHasZero);
				}
			}
		}

		[Fact]
		private static void TestReorderingAmdCSparseNet()
		{
			var pattern = SparsityPatternSymmetric.CreateFromDense(Matrix.CreateFromArray(SparsePosDef10by10.Matrix));
			var orderingAlg = new OrderingAmdCSparseNet();
			(int[] permutation, bool oldToNew) = orderingAlg.FindPermutation(pattern);
			Assert.True(!oldToNew);
			comparer.AssertEqual(SparsePosDef10by10.MatlabPermutationAMD, permutation);
		}

		[Fact]
		private static void TestReorderingAmdCSparseNetFromRawArrays()
		{
			int order = SparsePosDef10by10.Order;
			int[] cscColOffsets = SparsePosDef10by10.SymmetricCscColOffsets;
			int[] cscRowIndices = SparsePosDef10by10.SymmetricCscRowIndices;
			var orderingAlg = new OrderingAmdCSparseNet();
			(int[] permutation, bool oldToNew) = orderingAlg.FindPermutation(order, cscRowIndices, cscColOffsets);
			Assert.True(!oldToNew);
			comparer.AssertEqual(SparsePosDef10by10.MatlabPermutationAMD, permutation);
		}

		[SkippableFact]
		private static void TestReorderingAmdSuiteSparse()
		{
			Skip.IfNot(TestSettings.TestSuiteSparse, TestSettings.MessageWhenSkippingSuiteSparse);

			var pattern = SparsityPatternSymmetric.CreateFromDense(Matrix.CreateFromArray(SparsePosDef10by10.Matrix));
			var orderingAlg = new OrderingAmdSuiteSparse();
			(int[] permutation, bool oldToNew) = orderingAlg.FindPermutation(pattern);
			comparer.AssertEqual(SparsePosDef10by10.MatlabPermutationAMD, permutation);
		}

		[SkippableFact]
		private static void TestReorderingCamdSuiteSparse()
		{
			Skip.IfNot(TestSettings.TestSuiteSparse, TestSettings.MessageWhenSkippingSuiteSparse);

			int n = SparsePosDef10by10.Order;
			var pattern = SparsityPatternSymmetric.CreateFromDense(Matrix.CreateFromArray(SparsePosDef10by10.Matrix));
			var orderingAlg = new OrderingCamdSuiteSparse();
			(int[] permutation, ReorderingStatistics stats) =
				orderingAlg.FindPermutation(pattern, SparsePosDef10by10.ConstraintsCAMD);

			var originalDiagonal = new double[n];
			var permutedDiagonal = new double[n];
			for (int i = 0; i < n; ++i) originalDiagonal[i] = SparsePosDef10by10.Matrix[i, i];
			for (int i = 0; i < n; ++i) permutedDiagonal[i] = originalDiagonal[permutation[i]];

			var writer = new Array1DWriter();
			Console.Write("Permutation (new-to-old): ");
			writer.WriteToConsole(permutation);
			Console.Write("Original diagonal: ");
			writer.WriteToConsole(originalDiagonal);
			Console.Write("Permuted diagonal: ");
			writer.WriteToConsole(permutedDiagonal);

			comparer.AssertEqual(SparsePosDef10by10.PermutationCAMD, permutation);
		}
	}
}
