namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	public interface ISolutionConvergenceCriterion
	{
		double CalculateConvergenceMetric(IReadOnlyVector currentSolution, IReadOnlyVector previousSolution);
		string DescribeConvergenceCriterion(double tolerance);
	}
}
