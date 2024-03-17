namespace MGroup.LinearAlgebra.Iterative.Stationary
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	public interface IStationaryIteration
	{
		public string Name { get; }

		public void Execute(Vector input, Vector output);

		public void UpdateMatrix(IMatrixView matrix, bool isPatternModified);
	}
}
