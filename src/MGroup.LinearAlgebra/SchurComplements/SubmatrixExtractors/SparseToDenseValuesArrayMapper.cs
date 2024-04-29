namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// <inheritdoc cref="IValuesArrayMapper"/>.
	/// The submatrix is assumed to be denser than the original matrix. Namely the submatrix explicitly stores some (zero)
	/// entries, whose corresponding entries are implied in the original matrix.
	/// </summary>
	public class SparseToDenseValuesArrayMapper : IValuesArrayMapper
	{
		private readonly int[] mapSubmatrixToOriginalValues;

		/// <summary>
		/// Initializes a new instance of <see cref="SparseToDenseValuesArrayMapper"/> with an array that maps the entries of the
		/// values array of the submatrix to the entries of the values array of the original matrix.
		/// </summary>
		/// <param name="submatrixToOriginalValues">
		/// Maps the entries of the values array of the submatrix to the entries of the values array of the original matrix.
		/// Zero entries stored explicitly in the submatrix, but not in the original matrix, should be mapped to -1, instead of a
		/// valid index of the values array of the original matrix.
		/// This array will be used as provided, without copying it.
		/// </param>
		public SparseToDenseValuesArrayMapper(int[] submatrixToOriginalValues)
		{
			this.mapSubmatrixToOriginalValues = submatrixToOriginalValues;
		}

		/// <inheritdoc/>
		public void CopyValuesArrayToSubmatrix(double[] originalValues, double[] submatrixValues)
		{
			for (int i = 0; i < submatrixValues.Length; ++i)
			{
				int indexOriginal = mapSubmatrixToOriginalValues[i];
				if (indexOriginal >= 0)
				{
					submatrixValues[i] = originalValues[indexOriginal];
				}
				else
				{
					submatrixValues[i] = 0.0;
				}
			}
		}
	}
}
