namespace MGroup.LinearAlgebra.Iterative.Preconditioning
{
	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;
	using MGroup.LinearAlgebra.LinearAlgebraExtensions;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Providers.Managed;
	using MGroup.LinearAlgebra.Vectors;

	public class GaussSeidelPreconditioner : IPreconditioner
	{
		private readonly bool forwardDirection;
		private readonly StationaryIterationManagedProvider provider;
		private int[] diagonalOffsets;
		private CsrMatrix matrix;

		public GaussSeidelPreconditioner(bool forwardDirection = true)
		{
			this.forwardDirection = forwardDirection;
			provider = new StationaryIterationManagedProvider();
		}

		public IPreconditioner CopyWithInitialSettings() => new GaussSeidelPreconditioner(forwardDirection);

		public void SolveLinearSystem(IVectorView rhsVector, IVector lhsVector)
		{
			if ((rhsVector is Vector rhs) && (lhsVector is Vector lhs))
			{
				if (forwardDirection)
				{
					provider.CsrGaussSeidelForwardPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
						diagonalOffsets, rhs.RawData, lhs.RawData);
				}
				else
				{
					provider.CsrGaussSeidelBackPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
						diagonalOffsets, rhs.RawData, lhs.RawData);
				}
			}
			else
			{
				throw new NonMatchingFormatException("Input and output vectors must be dense.");
			}
		}

		public void UpdateMatrix(IMatrixView matrix, bool isPatternModified)
		{
			Preconditions.CheckSquare(matrix);
			if (matrix is CsrMatrix csrMatrix)
			{
				this.matrix = csrMatrix;

				if (isPatternModified || diagonalOffsets == null)
				{
					diagonalOffsets = provider.LocateDiagonalOffsetsCsr(
						matrix.NumRows, csrMatrix.RawRowOffsets, csrMatrix.RawColIndices);
				}
			}
			else
			{
				throw new InvalidSparsityPatternException(GetType().Name + " can be used only for matrices in CSR format.");
			}
		}
	}
}
