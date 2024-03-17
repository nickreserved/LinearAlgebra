namespace MGroup.LinearAlgebra.Iterative.Termination.Stegnation
{
	//TODO: Combine this with convergence criterion
	public interface IStagnationCriterion : ISettingsCopiable<IStagnationCriterion>
	{
		bool HasStagnated();

		void StoreInitialError(double initialError);

		void StoreNewError(double currentError);
	}
}
