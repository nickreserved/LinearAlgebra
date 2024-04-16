namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public interface IIndexableInt2D
	{
		int NumRows { get; }

		int NumColumns { get; }

		int this[int rowIdx, int colIdx] { get; }
	}
}
