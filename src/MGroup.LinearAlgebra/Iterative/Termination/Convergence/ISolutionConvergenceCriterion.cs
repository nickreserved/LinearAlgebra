namespace MGroup.LinearAlgebra.Iterative.Termination.Convergence
{
	using MGroup.LinearAlgebra.Vectors;

	public interface ISolutionConvergenceCriterion : ISettingsCopiable<ISolutionConvergenceCriterion>
	{
		double CalculateConvergenceMetric(IVectorView currentSolution, IVectorView previousSolution);

		string DescribeConvergenceCriterion(double tolerance);
	}
}
