using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative
{
	public static class ExactResidual
	{
		public static IMutableVector Calculate(ILinearTransformation matrix, IImmutableVector rhs, IImmutableVector solution)
		{
			IMutableVector residual = rhs.CreateZero();
			Calculate(matrix, rhs, solution, residual);
			return residual;
		}

		public static void Calculate(ILinearTransformation matrix, IImmutableVector rhs, IImmutableVector solution, IMutableVector residual)
		{
			//TODO: There is a BLAS operation y = y + a * A*x, that would be perfect for here. rhs.Copy() and then that.
			matrix.Multiply(solution, residual);
			residual.LinearCombinationIntoThis(-1.0, rhs, 1.0);
		}

	}
}
