namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	public interface ISolutionConvergenceCriterion
	{
		double CalculateConvergenceMetric(IImmutableVector currentSolution, IImmutableVector previousSolution);
		string DescribeConvergenceCriterion(double tolerance);
	}
}
