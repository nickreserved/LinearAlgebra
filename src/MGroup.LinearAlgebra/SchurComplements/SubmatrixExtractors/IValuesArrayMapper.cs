namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Copies entries from the values array (internal storage) of a matrix to the values array of its submatrix.
	/// </summary>
	public interface IValuesArrayMapper
	{
		/// <summary>
		/// Copies the entries of the values array of the original matrix to their respective positions inside the values array
		/// of the submatrix.
		/// </summary>
		/// <param name="originalMatrixValues">
		/// The values array (internal storage) of the original matrix. It will be read only.
		/// </param>
		/// <param name="submatrixValues">
		/// The values array (internal storage) of the submatrix. It will be overwritten.
		/// </param>
		void CopyValuesArrayToSubmatrix(double[] originalMatrixValues, double[] submatrixValues);
	}
}
