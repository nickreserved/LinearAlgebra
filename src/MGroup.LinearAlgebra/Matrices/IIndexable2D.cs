namespace MGroup.LinearAlgebra.Matrices
{
	using System;

	using MGroup.LinearAlgebra.Providers;

	/// <summary>
	/// A matrix that supports indexing These are the most basic operations all matrix classes must
	/// implement. As such, it can be used for matrix formats that do not support linear algebra operations, such as DOKs and 
	/// other builders.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public interface IIndexable2D : IBounded2D
    {
		/// <summary>
		/// Matrix symmetry properties (i.e.: unknown, symmetric or not symmetric).
		/// </summary>
		MatrixSymmetry MatrixSymmetry { get; }

		/// <summary>
		/// The entry with row index = rowIdx and column index = colIdx. 
		/// </summary>
		/// <param name="rowIdx">The row index: 0 &lt;= <paramref name="rowIdx"/> &lt; <see cref="IBounded2D.NumRows"/>.</param>
		/// <param name="colIdx">The column index: 0 &lt;= <paramref name="colIdx"/> &lt; <see cref="IBounded2D.NumColumns"/>.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="rowIdx"/> or <paramref name="colIdx"/> violate 
		///     the described constraints.</exception>
		double this[int rowIdx, int colIdx] { get; }
    }
}
