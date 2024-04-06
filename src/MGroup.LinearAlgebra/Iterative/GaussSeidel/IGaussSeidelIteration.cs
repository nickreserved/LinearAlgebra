namespace MGroup.LinearAlgebra.Iterative.GaussSeidel
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	public interface IGaussSeidelIteration : IDisposable
	{
		void Initialize(IMatrixView matrix);

		/// <summary>
		/// Performs 1 backward Gauss-Seidel iteration.
		/// </summary>
		/// <param name="rhsVector">Will not check if the dimensions are compatible with the matrix.</param>
		/// <param name="lhsVector">Will not check if the dimensions are compatible with the matrix.</param>
		void GaussSeidelBackwardIteration(IExtendedReadOnlyVector rhsVector, IExtendedVector lhsVector);

		/// <summary>
		/// Performs 1 forward Gauss-Seidel iteration.
		/// </summary>
		/// <param name="rhsVector">Will not check if the dimensions are compatible with the matrix.</param>
		/// <param name="lhsVector">Will not check if the dimensions are compatible with the matrix.</param>
		void GaussSeidelForwardIteration(IExtendedReadOnlyVector rhsVector, IExtendedVector lhsVector);
	}

	public interface IGaussSeidelIterationBuilder
	{
		IGaussSeidelIteration Create();
	}
}
