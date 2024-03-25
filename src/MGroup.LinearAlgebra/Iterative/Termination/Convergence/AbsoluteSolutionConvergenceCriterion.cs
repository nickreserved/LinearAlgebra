namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// Convergence criterion: norm2(x - x_previous) &lt; tolerance, where x is the solution vector.
	/// </summary>
	public class AbsoluteSolutionConvergenceCriterion : ISolutionConvergenceCriterion
	{
		public double CalculateConvergenceMetric(IMinimalImmutableVector currentSolution, IMinimalImmutableVector previousSolution)
		{
			//TODO: The next can be optimized to not create a new vector (using SubtractIntoThis) in some cases.
			//		E.g. in Gauss-Seidel the previousSolution vector is no longer necessary and can be overwritten.
			return previousSolution.Subtract(currentSolution).Norm2();
		}

		public string DescribeConvergenceCriterion(double convergenceTolerance) => "norm2(x - x_previous) < " + convergenceTolerance;
	}
}
