namespace MGroup.LinearAlgebra.Tests.Iterative.Stationary
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
	using MGroup.LinearAlgebra.Iterative.Stationary;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using Xunit;
	using MGroup.LinearAlgebra.Vectors;

	public class StationaryIterationTests
	{
		[Fact]
		private static void TestBackGaussSeidelIteration()
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
			// (U+D)*x(t+1) = b - L*x(t)
			var A = SparseMatrix.CreateFromMatrix(csrMatrix);
			SparseMatrix UplusD = A.ExtractUpperTriangleAndDiagonal();
			SparseMatrix L = A.ExtractLowerTriangle();

			// Prepare actual iteration
			IStationaryIteration stationaryIteration = new GaussSeidelIterationCsr(forwardDirection: false);
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
		private static void TestForwardGaussSeidelIteration()
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
			// (L+D)*x(t+1) = b - U*x(t)
			var A = SparseMatrix.CreateFromMatrix(csrMatrix);
			SparseMatrix LplusD = A.ExtractLowerTriangleAndDiagonal();
			SparseMatrix U = A.ExtractUpperTriangle();

			// Prepare actual iteration
			IStationaryIteration stationaryIteration = new GaussSeidelIterationCsr(forwardDirection: true);
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

		[Fact]
		private static void TestJacobiIteration()
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
			// D*x(t+1) = b - (U+L)*x(t)
			var A = SparseMatrix.CreateFromMatrix(csrMatrix);
			SparseMatrix L = A.ExtractLowerTriangle();
			SparseMatrix D = A.ExtractDiagonal();
			SparseMatrix U = A.ExtractUpperTriangle();
			SparseMatrix LplusU = L+U;

			// Prepare actual iteration
			IStationaryIteration stationaryIteration = new JacobiIterationCsr();
			stationaryIteration.UpdateMatrix(csrMatrix, true);

			// i = 0: x = 0
			xExpected = D.SolveDiagonal(b - LplusU.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);

			// i = 1: x = x1
			xExpected = D.SolveDiagonal(b - LplusU.Multiply(xExpected));
			stationaryIteration.Execute(b, xComputed);
			comparer.AssertEqual(xExpected, xComputed);
		}
	}
}
