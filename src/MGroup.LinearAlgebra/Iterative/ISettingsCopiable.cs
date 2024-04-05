namespace MGroup.LinearAlgebra.Iterative
{
	/// <summary>
	/// Classes that implement this interface must be able to copy their initial settings, but not their current state, which
	/// may be modified by the execution of a linear algebra algorithm. Esentially they copy themselves as they were at the
	/// start of their lifetime.
	/// </summary>
	/// <typeparam name="T">Anything really.</typeparam>
	public interface ISettingsCopiable<T>
	{
		/// <summary>
		/// Creates a new instance that has the same initial settings, but does not carry over any mutable state that depends on
		/// the execution of linear algebra algorithms.
		/// </summary>
		/// <returns>The brand new object that is identical to the current one at the beginning of its lifetime.</returns>
		public T CopyWithInitialSettings();
	}
}
