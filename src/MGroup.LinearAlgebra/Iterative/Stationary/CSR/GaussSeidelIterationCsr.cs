namespace MGroup.LinearAlgebra.Iterative.Stationary.CSR
{
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// Represents the forward Gauss-Seidel (D+L) * x(t+1) = -U*x(t) + b or back Gauss-Seidel (D+U) * x(t+1) = -L*x(t) + b. 
	/// The matrix A = D+L+U must be given in CSR format, where the entries of each row are sorted in ascending column order.
	/// </summary>
	/// <remarks>
	/// When using this iteration by itself repeatedly, convergence is guaranteed only for strictly diagonally dominant or
	/// positive definite (symmetric) matrices. Might converge in general matrix systems, as well, but with no guarantee.
	/// </remarks>
	public class GaussSeidelIterationCsr : CsrStationaryIterationBase
	{
		private readonly bool forwardDirection;

		public GaussSeidelIterationCsr(bool forwardDirection = true)
		{
				this.forwardDirection = forwardDirection;
		}

		public override string Name => "Forward Gauss-Seidel";

		public override void Execute(Vector input, Vector output)
		{
			if (forwardDirection)
			{
				provider.CsrGaussSeidelForward(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, input.RawData, output.RawData);
			}
			else
			{
				provider.CsrGaussSeidelBack(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices,
					diagonalOffsets, input.RawData, output.RawData);
			}
		}
	}
}
