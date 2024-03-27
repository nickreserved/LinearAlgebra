namespace MGroup.LinearAlgebra.Iterative.Preconditioning
{
	using System;
	using MGroup.LinearAlgebra.Matrices;

	/// <summary>
	/// Applies the following preconditioning: [1/(w*(2-w) * (D+w*L) * inv(D) * (D+w*U)] * x = y ,
	/// where x is the unknown vector and w the relaxation factor (scalar).
	/// </summary>
	public class SsorPreconditionerCsr : CsrStationaryPreconditionerBase
	{
		private readonly double relaxationFactor;
		private double[] workArray;

		public SsorPreconditionerCsr(double relaxationFactor, int numApplications = 1)
			: base(numApplications)
		{
			this.relaxationFactor = relaxationFactor;
		}

		public override IPreconditioner CopyWithInitialSettings() => new SsorPreconditionerCsr(relaxationFactor);

		public override void UpdateMatrix(IMatrixView matrix, bool isPatternModified)
		{
			base.UpdateMatrix(matrix, isPatternModified);
			if (isPatternModified || workArray == null)
			{
				workArray = new double[matrix.NumColumns];
			}
		}

		protected override void SolveLinearSystemInternal(double[] rhsVector, double[] lhsVector)
		{
			provider.CsrSsorPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
				diagonalOffsets, rhsVector, lhsVector, relaxationFactor, workArray);
			//provider.CsrSorForwardPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
			//	diagonalOffsets, rhsVector, lhsVector, relaxationFactor);
			//provider.CsrSorBackPrecond(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
			//	diagonalOffsets, rhsVector, lhsVector, relaxationFactor);
		}
	}
}
