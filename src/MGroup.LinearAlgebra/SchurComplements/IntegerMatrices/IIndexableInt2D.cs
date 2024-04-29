namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Represents a matrix with integer entries that can be read using 2 indices, a row and a column one.
	/// </summary>
	public interface IIndexableInt2D
	{
		/// <summary>
		/// The number of columns of the matrix.
		/// </summary>
		int NumColumns { get; }

		/// <summary>
		/// The number of rows of the matrix.
		/// </summary>
		int NumRows { get; }

		/// <summary>
		/// Reads the entry with indices [<paramref name="rowIdx"/>, <paramref name="colIdx"/>].
		/// </summary>
		/// <param name="rowIdx">The index of the row, where the entry lies.</param>
		/// <param name="colIdx">The index of the column, where the entry lies.</param>
		/// <returns>An integer entry of the matrix.</returns>
		int this[int rowIdx, int colIdx] { get; }
	}
}
