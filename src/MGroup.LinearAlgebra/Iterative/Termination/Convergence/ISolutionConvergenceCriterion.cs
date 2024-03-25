namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	public interface ISolutionConvergenceCriterion
	{
		double CalculateConvergenceMetric(IMinimalImmutableVector currentSolution, IMinimalImmutableVector previousSolution);
		string DescribeConvergenceCriterion(double tolerance);
	}
}
