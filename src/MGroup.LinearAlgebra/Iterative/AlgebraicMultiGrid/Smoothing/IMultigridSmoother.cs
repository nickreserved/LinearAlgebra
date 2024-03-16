namespace MGroup.LinearAlgebra.Iterative.AlgebraicMultiGrid.Smoothing
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// Multigrid smoothing, also called relaxation.
	/// </summary>
	public interface IMultigridSmoother
	{
		void Initialize(IMatrixView matrix);

		void Smooth(IVectorView rhs, IVector lhs);
	}

	public interface IMultigridSmootherBuilder
	{
		IMultigridSmoother Create();
	}
}
