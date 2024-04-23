using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

//TODOMPI: Needs testing
namespace MGroup.LinearAlgebra.Iterative.Preconditioning
{
	public class JacobiPreconditioner : IPreconditioner
	{
		/// <summary>
		/// Creates a Jacobi preconditioner.
		/// </summary>
		/// <param name="diagonal">
		/// A vector that contains the entries of matrix's main diagonal (or the inverse).
		/// If vector is overlapping on distributed environment, it contains the diagonal entries of each local matrix that corresponds to a 
		/// <c>ComputeNode</c> of the environment. If an entry is overlapping, namely if it exists
		/// in many neighboring local diagonal vectors, then its value must be the same in all these local vectors.
		/// </param>
		/// <param name="alreadyInverted">Diagonal vector is already inverted</param>
		public JacobiPreconditioner(IMinimalReadOnlyVector diagonal, bool alreadyInverted = false)
		{
			Diagonal = alreadyInverted ? diagonal : diagonal.DoToAllEntries(x => 1 / x);
		}

		public IMinimalReadOnlyVector Diagonal { get; }

		public void Apply(IMinimalReadOnlyVector rhsVector, IMinimalVector lhsVector)
		{
			lhsVector.CopyFrom(rhsVector.DoEntrywise(Diagonal, (x, y) => x * y));
		}



		/// <summary>
		/// Creates instances of <see cref="JacobiPreconditioner"/>.
		/// </summary>
		public class Factory : IPreconditionerFactory
		{
			public bool PreInvert = false;
			/// <summary>
			/// See <see cref="IPreconditionerFactory.CreatePreconditionerFor(ILinearTransformation)"/>.
			/// </summary>
			public IPreconditioner CreatePreconditionerFor(ILinearTransformation matrix)
				=> new JacobiPreconditioner(((IMatrixView) matrix).GetDiagonal(), PreInvert);
		}

	}
}
