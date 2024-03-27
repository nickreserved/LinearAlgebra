namespace MGroup.LinearAlgebra.Iterative.Preconditioning
{
	/// <summary>
	/// Applies one the following preconditionings: 1/w*(D+w*L) * x = y (forward SOR) or 
	/// 1/w*(D+w*L) * x = y (back SOR), where x is the unknown vector and w the relaxation factor (scalar).
	/// </summary>
	public class SorPreconditionerCsr : CsrStationaryPreconditionerBase
	{
		private readonly double relaxationFactor;
		private readonly bool forwardDirection;

		public SorPreconditionerCsr(double relaxationFactor, bool forwardDirection = true, int numApplications = 1)
			: base(numApplications)
		{
			this.relaxationFactor = relaxationFactor;
			this.forwardDirection = forwardDirection;
		}

		public override IPreconditioner CopyWithInitialSettings() => new SorPreconditionerCsr(relaxationFactor, forwardDirection);

		protected override void SolveLinearSystemInternal(double[] rhsVector, double[] lhsVector)
		{
			if (forwardDirection)
			{
				provider.CsrSorForwardPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, rhsVector, lhsVector, relaxationFactor);
			}
			else
			{
				provider.CsrSorBackPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, rhsVector, lhsVector, relaxationFactor);
			}
		}
	}
}
