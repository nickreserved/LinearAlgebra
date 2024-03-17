namespace MGroup.LinearAlgebra.Iterative.Termination.Stegnation
{
	public class NullStagnationCriterion : IStagnationCriterion
	{
		public IStagnationCriterion CopyWithInitialSettings() => new NullStagnationCriterion();

		public bool HasStagnated() => false;

		public void StoreInitialError(double initialError) { }

		public void StoreNewError(double currentError) { }

	}
}
