namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Exceptions;

	/// <summary>
	/// <inheritdoc cref="IValuesArrayMapper"/>.
	/// The original matrix and the submatrix must either both store only nonzeros or both store every entry of the same area
	/// (e.g lower triangle, upper triangle plus diagonal, etc.) of a matrix.
	/// </summary>
	public class SameSparsityValuesArrayMapper : IValuesArrayMapper
	{
		private readonly int[] mapSubmatrixToOriginalValues;

		/// <summary>
		/// Initializes a new instance of <see cref="SameSparsityValuesArrayMapper"/> with an array that maps the entries of the
		/// values array of the submatrix to the entries of the values array of the original matrix.
		/// </summary>
		/// <param name="submatrixToOriginalValues">
		/// Maps the entries of the values array of the submatrix to the entries of the values array of the original matrix.
		/// This array will be used as provided, without copying it.
		/// </param>
		public SameSparsityValuesArrayMapper(int[] submatrixToOriginalValues)
		{
			this.mapSubmatrixToOriginalValues = submatrixToOriginalValues;
		}

		/// <inheritdoc/>
		public void CopyValuesArrayToSubmatrix(double[] originalValues, double[] submatrixValues)
		{
			CheckSizeOfRawArray(submatrixValues, mapSubmatrixToOriginalValues);
			for (int i = 0; i < submatrixValues.Length; i++)
			{
				submatrixValues[i] = originalValues[mapSubmatrixToOriginalValues[i]];
			}
		}

		private static void CheckSizeOfRawArray(double[] submatrixValues, int[] map)
		{
			if (submatrixValues.Length != map.Length)
			{
				throw new InvalidSparsityPatternException($"Can copy to values array with length={map.Length}," +
					$" but a submatrix with {submatrixValues} entries was provided.");
			}
		}
	}
}
