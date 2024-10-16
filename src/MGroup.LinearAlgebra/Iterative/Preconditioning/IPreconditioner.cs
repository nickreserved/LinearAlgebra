using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative.Preconditioning
{
	/// <summary>
	/// Represents a matrix M such that inverse(M) is close to inverse(A) and inverse(M) * A has a smaller condition number 
	/// than A, where A is the original matrix of the linear system A*x=b.
	/// </summary>
	public interface IPreconditioner : ISettingsCopiable<IPreconditioner>
	{
		/// <summary>
		/// Applies the preconditioner by solving the system: M * v = w during the preconditioning step of an iterative 
		/// algorithm, where M is the preconditioner and the definition of the vectors v, w depends on the iterative algorithm. 
		/// This is equivalent to evaluating: inverse(M) * w.
		/// </summary>
		/// <param name="rhsVector">The right hand side vector of the system M * v = w. It will not be modified.</param>
		/// <param name="lhsVector">
		/// The left hand side vector of the system M * v = w. It will be overwritten by the solution of the linear system.
		/// </param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if the <see cref="IIndexable1D.Length"/> of <paramref name="rhsVector"/> or <paramref name="lhsVector"/> 
		/// is different than the number of rows of this <see cref="IPreconditioner"/>.
		/// </exception>
		void SolveLinearSystem(IVectorView rhsVector, IVector lhsVector);

		public void UpdateMatrix(IMatrixView matrix, bool isPatternModified);
	}
}
