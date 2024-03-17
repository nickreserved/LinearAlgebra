namespace MGroup.LinearAlgebra.Iterative.Stationary.CSR
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Providers.Managed;
	using MGroup.LinearAlgebra.Vectors;

	public abstract class CsrStationaryIterationBase : IStationaryIteration
	{
		protected readonly StationaryIterationManagedProvider provider;
		protected CsrMatrix matrix;
		protected int[] diagonalOffsets;

		public CsrStationaryIterationBase()
		{
			provider = new StationaryIterationManagedProvider();
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

		public abstract string Name { get; }

		public abstract IStationaryIteration CopyWithInitialSettings();

		public abstract void Execute(Vector input, Vector output);
	}
}
