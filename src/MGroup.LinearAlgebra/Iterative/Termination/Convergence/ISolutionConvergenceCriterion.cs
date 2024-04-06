namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	public interface ISolutionConvergenceCriterion
	{
		double CalculateConvergenceMetric(IMinimalReadOnlyVector currentSolution, IMinimalReadOnlyVector previousSolution);
		string DescribeConvergenceCriterion(double tolerance);
	}
}
