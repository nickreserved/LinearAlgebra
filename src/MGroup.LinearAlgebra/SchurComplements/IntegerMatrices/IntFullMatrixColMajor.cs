namespace MGroup.LinearAlgebra.SchurComplements.IntegerMatrices
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Represents a matrix with integer entries. All entries are stored explicitly (full format) in column-major order.
	/// </summary>
	public class IntFullMatrixColMajor : IIndexableInt2D
	{
		private readonly int[] values;

		private IntFullMatrixColMajor(int numRows, int numColumns, int[] values)
		{
			this.values = values;
			this.NumRows = numRows;
			this.NumColumns = numColumns;
		}

		/// <inheritdoc/>>
		public int NumColumns { get; }

		/// <inheritdoc/>>
		public int NumRows { get; }

		/// <summary>
		/// The internal array that stores the entries of the matrix in column major layout.
		/// It should only be used for passing the raw array to linear algebra libraries.
		/// </summary>
		public int[] RawValues => values;

		/// <inheritdoc/>>
		public int this[int rowIdx, int colIdx]
		{
			get => values[colIdx * NumRows + rowIdx];
			set => values[colIdx * NumRows + rowIdx] = value;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IntFullMatrixColMajor"/> with all entries being equal to 0.
		/// </summary>
		/// <param name="numRows">The number of rows of the new matrix.</param>
		/// <param name="numColumns">The number of columns of the new matrix.</param>
		/// <returns>An integer matrix with full, column-major format.</returns>
		public static IntFullMatrixColMajor CreateZero(int numRows, int numColumns)
		{
			var values = new int[numRows * numColumns];
			return new IntFullMatrixColMajor(numRows, numColumns, values);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IntFullMatrixColMajor"/> with with <paramref name="values"/> or a clone as 
		/// its internal array.
		/// </summary>
		/// <param name="numRows">The number of rows of the new matrix.</param>
		/// <param name="numColumns">The number of columns of the new matrix.</param>
		/// <param name="values">An array containing the entries of the matrix in full, column-major format</param>
		/// <param name="copyArray">
		/// If true, <paramref name="values"/> will be copied, which is safer.
		/// If false, the object <paramref name="values"/> will be used itself, which is faster.</param>
		/// <returns>An integer matrix with full, column-major format.</returns>
		public static IntFullMatrixColMajor CreateFromArray(int numRows, int numColumns, int[] values, bool copyArray = false)
		{
			if (copyArray)
			{
				var clone = new int[values.Length];
				Array.Copy(values, clone, clone.Length);
				return new IntFullMatrixColMajor(numRows, numColumns, clone);
			}
			else
			{
				return new IntFullMatrixColMajor(numRows, numColumns, values);
			}
		}
	}
}
