using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative
{
	public static class ExactResidual
	{
		public static IMinimalVector Calculate(ILinearTransformation matrix, IMinimalReadOnlyVector rhs, IMinimalReadOnlyVector solution)
		{
			IMinimalVector residual = rhs.CreateZero();
			Calculate(matrix, rhs, solution, residual);
			return residual;
		}

		public static void Calculate(ILinearTransformation matrix, IMinimalReadOnlyVector rhs, IMinimalReadOnlyVector solution, IMinimalVector residual)
		{
			//TODO: There is a BLAS operation y = y + a * A*x, that would be perfect for here. rhs.Copy() and then that.
			matrix.MultiplyIntoThis(solution, residual);
			residual.LinearCombinationIntoThis(-1.0, rhs, 1.0);
		}

	}
}
