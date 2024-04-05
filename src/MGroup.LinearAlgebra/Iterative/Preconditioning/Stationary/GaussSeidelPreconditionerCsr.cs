namespace MGroup.LinearAlgebra.Iterative.Preconditioning.Stationary
{
	/// <summary>
	/// Applies one the following preconditionings: (L+D) * x = y (forward Gauss-Seidel) or (U+D) * x = y (back Gauss-Seidel),
	/// where x is the unknown vector.
	/// </summary>
	public class GaussSeidelPreconditionerCsr : CsrStationaryPreconditionerBase
	{
		private readonly bool forwardDirection;

		public GaussSeidelPreconditionerCsr(bool forwardDirection = true, int numApplications = 1)
			: base(numApplications)
		{
			this.forwardDirection = forwardDirection;
		}

		public override IPreconditioner CopyWithInitialSettings() => new GaussSeidelPreconditionerCsr(forwardDirection);

		protected override void SolveLinearSystemInternal(double[] rhsVector, double[] lhsVector)
		{
			if (forwardDirection)
			{
				provider.CsrGaussSeidelForwardPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, rhsVector, lhsVector);
			}
			else
			{
				provider.CsrGaussSeidelBackPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, rhsVector, lhsVector);
			}
		}
	}
}
