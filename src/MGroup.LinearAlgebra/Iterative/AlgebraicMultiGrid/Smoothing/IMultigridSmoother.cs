namespace MGroup.LinearAlgebra.Iterative.AlgebraicMultiGrid.Smoothing
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Iterative.GaussSeidel;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// Multigrid smoothing, also called relaxation.
	/// </summary>
	public interface IMultigridSmoother : IDisposable
	{
		void Initialize(IMatrixView matrix);

		void Smooth(IExtendedImmutableVector rhs, IExtendedMutableVector lhs);
	}

	public interface IMultigridSmootherBuilder
	{
		IMultigridSmoother Create();
	}
}
