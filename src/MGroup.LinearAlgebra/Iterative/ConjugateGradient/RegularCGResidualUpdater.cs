namespace MGroup.LinearAlgebra.Iterative.ConjugateGradient
{
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// Updates the residual vector according to the usual CG formula r = r - α * A*d. No corrections are applied.
	/// </summary>
	public class RegularCGResidualUpdater : ICGResidualUpdater
	{
		public ICGResidualUpdater CopyWithInitialSettings() => new RegularCGResidualUpdater();

		/// <summary>
		/// See <see cref="ICGResidualUpdater.UpdateResidual(CGAlgorithm, IVector, out double)"/>
		/// </summary>
		public void UpdateResidual(CGAlgorithm cg, IVector residual, out double resDotRes)
		{
			// Normally the residual vector is updated as: r = r - α * A*d
			residual.AxpyIntoThis(cg.MatrixTimesDirection, -cg.StepSize);
			resDotRes = residual.DotProduct(residual);
		}
	}
}
