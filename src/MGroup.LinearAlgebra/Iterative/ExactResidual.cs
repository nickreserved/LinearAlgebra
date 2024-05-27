using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative
{
	public static class ExactResidual
	{
		public static IVector Calculate(ILinearTransformation matrix, IReadOnlyVector rhs, IReadOnlyVector solution)
		{
			IVector residual = rhs.CreateZeroWithSameFormat();
			Calculate(matrix, rhs, solution, residual);
			return residual;
		}

		public static void Calculate(ILinearTransformation matrix, IReadOnlyVector rhs, IReadOnlyVector solution, IVector residual)
		{
			//TODO: There is a BLAS operation y = y + a * A*x, that would be perfect for here. rhs.Copy() and then that.
			matrix.MultiplyIntoResult(solution, residual);
			residual.LinearCombinationIntoThis(-1.0, rhs, 1.0);
		}

	}
}
