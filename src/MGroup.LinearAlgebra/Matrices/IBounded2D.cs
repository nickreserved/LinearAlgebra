namespace MGroup.LinearAlgebra.Matrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Dimensions of matrix (or generalized, a linear transformation).
	/// </summary>
	public interface IBounded2D
	{
		/// <summary>
		/// The number of rows of matrix.
		/// </summary>
		int NumRows { get; }

		/// <summary>
		/// The number of columns of matrix.
		/// </summary>
		int NumColumns { get; }
	}
}
