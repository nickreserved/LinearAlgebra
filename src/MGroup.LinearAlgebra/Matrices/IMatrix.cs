using System;

namespace MGroup.LinearAlgebra.Matrices
{
    /// <summary>
    /// Operations specified by this interface modify the matrix. Therefore it is possible that they may throw exceptions if they 
    /// are used on sparse or triangular storage matrix formats.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public interface IMatrix: IMatrixView, IMinimalMatrix
    {
		/// <summary>
		/// Setter that will work as expected for general dense matrices. For sparse matrices it will throw a 
		/// <see cref="Exceptions.SparsityPatternModifiedException"/> if a structural zero entry is written to.
		/// For symmetric matrices, this will set both (<paramref name="rowIdx"/>, <paramref name="colIdx"/>) and 
		/// (<paramref name="colIdx"/>, <paramref name="rowIdx"/>).
		/// </summary>
		/// <param name="rowIdx">The row index: 0 &lt;= rowIdx &lt; <see cref="IBounded2D.NumRows"/></param>
		/// <param name="colIdx">The column index: 0 &lt;= colIdx &lt; <see cref="IBounded2D.NumColumns"/></param>
		/// <param name="value">The new value of this[<paramref name="rowIdx"/>, <paramref name="colIdx"/>].</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="rowIdx"/> or <paramref name="colIdx"/> violate 
		///     the described constraints.</exception>
		/// <exception cref="Exceptions.SparsityPatternModifiedException"> Thrown if a structural zero entry of a sparse matrix 
		///     format is written to.</exception>
		void SetEntryRespectingPattern(int rowIdx, int colIdx, double value);
    }
}
