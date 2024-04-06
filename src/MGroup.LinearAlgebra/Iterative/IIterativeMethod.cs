using MGroup.LinearAlgebra.Vectors;
using MGroup.LinearAlgebra.Iterative.Preconditioning;
using MGroup.LinearAlgebra.Matrices;

namespace MGroup.LinearAlgebra.Iterative
{
	public interface IIterativeMethod
	{
		void Clear();

		//TODO: initialGuessIsZero can be ommited if solution is null, but in that case we need a zero vector generator
		IterativeStatistics Solve(ILinearTransformation matrix, IPreconditioner preconditioner,
			IMinimalReadOnlyVector rhs, IMinimalVector solution, bool initialGuessIsZero);
	}
}
