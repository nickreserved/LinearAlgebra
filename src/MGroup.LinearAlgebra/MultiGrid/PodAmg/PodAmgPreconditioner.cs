namespace MGroup.LinearAlgebra.AlgebraicMultiGrid.PodAmg
{
	using System;

	using MGroup.LinearAlgebra.AlgebraicMultiGrid;
	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;
	using MGroup.LinearAlgebra.Iterative.Preconditioning;
	using MGroup.LinearAlgebra.Iterative.Stationary.CSR;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Triangulation;
	using MGroup.LinearAlgebra.Vectors;

	public class PodAmgPreconditioner : IPreconditioner
	{
		private readonly int numIterations;
		private readonly MultigridLevelSmoothing smoothing;

		private CsrMatrix fineMatrix;
		private CholeskyFull coarseMatrixFactorized;

		// Restriction is the transpose of this
		private Matrix prolongation;

		public PodAmgPreconditioner(CsrMatrix fineMatrix, CholeskyFull coarseMatrixFactorized, Matrix prolongation,
			MultigridLevelSmoothing smoothing, int numIterations)
		{
			this.fineMatrix = fineMatrix;
			this.coarseMatrixFactorized = coarseMatrixFactorized;
			this.prolongation = prolongation;
			this.smoothing = smoothing;
			this.numIterations = numIterations;
		}

		public void SolveLinearSystem(IVectorView rhsVector, IVector lhsVector)
		{
			var rhs = (Vector)rhsVector;
			var solution = (Vector)lhsVector;
			solution.Clear();

			Preconditions.CheckSquareLinearSystemDimensions(fineMatrix, rhs, solution);
			var n0 = fineMatrix.NumRows;
			var r0 = Vector.CreateZero(n0);
			var e0 = Vector.CreateZero(n0);
			var n1 = coarseMatrixFactorized.Order;
			var r1 = Vector.CreateZero(n1);
			var e1 = Vector.CreateZero(n1);

			for (var i = 0; i < numIterations; i++)
			{
				// Pre-smoothing on lvl 0 to get an estimate of the solution x0. Use the x0 from previous cycles as initial guess.
				smoothing.ApplyPreSmoothers(rhs, solution);

				// Find the residual on lvl 0: r0=b-A0*x0
				//TODO: Use ExactResidual class for this
				fineMatrix.MultiplyIntoResult(solution, r0);
				r0.LinearCombinationIntoThis(-1.0, rhs, 1.0);

				// Restrict lvl 0 residual to lvl 1: r1 = P^T * r0
				prolongation.MultiplyIntoResult(r0, r1, transposeThis: true);

				// Find an estimate of the error on lvl 1 by solving exactly the system: A1*e1=r1.
				coarseMatrixFactorized.SolveLinearSystem(r1, e1);

				// Interpolate the lvl 1 error estimate to lvl 0: e0 = P * e1
				prolongation.MultiplyIntoResult(e1, e0, transposeThis: false);

				// Correct the solution estimate on lvl 0 using the interpolated error: x0 = x0 + e0
				solution.AddIntoThis(e0);

				// Post-smoothing on lvl 0 to further improve the solution estimate x0. Use the corrected x0 as initial guess.
				smoothing.ApplyPostSmoothers(rhs, solution);
			}
		}

		public class Factory : IPreconditionerFactory
		{
			private Matrix _prolongation;

			public Factory()
			{
				NumIterations = 1;
				KeepOnlyNonZeroPrincipalComponents = true;
				CreateSmoothers = () => new MultigridLevelSmoothing()
				.AddPreSmoother(new GaussSeidelIterationCsr(), 1)
				.AddPostSmoother(new GaussSeidelIterationCsr(), 1);
			}

			public bool KeepOnlyNonZeroPrincipalComponents { get; set; }

			public int NumIterations { get; set; }

			public Func<MultigridLevelSmoothing> CreateSmoothers { get; set; }

			public IPreconditioner CreatePreconditionerFor(IMatrixView matrix)
			{
				if (_prolongation == null)
				{
					throw new InvalidOperationException("The preconditioner factory must be initialized first");
				}

				var fineMatrix = CheckMatrixFormat(matrix);
				var smoothing = CreateSmoothers();
				smoothing.UpdateMatrix(fineMatrix, true);

				var temp = fineMatrix.MultiplyRight(_prolongation);
				var coarseMatrix = _prolongation.MultiplyRight(temp, transposeThis: true, transposeOther: false);

				var coarseMatrixFactorized = CholeskyFull.Factorize(coarseMatrix.NumRows, coarseMatrix.RawData);

				return new PodAmgPreconditioner(fineMatrix, coarseMatrixFactorized, _prolongation, smoothing, NumIterations);
			}

			public void Initialize(Matrix sampleVectors, int numPrincipalComponents)
			{
				var pod = new ProperOrthogonalDecomposition(KeepOnlyNonZeroPrincipalComponents);
				_prolongation = pod.CalculatePrincipalComponents(sampleVectors.NumColumns, sampleVectors, numPrincipalComponents);
			}

			private CsrMatrix CheckMatrixFormat(IMatrixView matrix)
			{
				if (matrix is CsrMatrix csrMatrix)
				{
					return csrMatrix;
				}
				else
				{
					throw new InvalidSparsityPatternException("PodAmgPreconditioner can be used only for matrices in CSR format");
				}
			}
		}
	}
}
