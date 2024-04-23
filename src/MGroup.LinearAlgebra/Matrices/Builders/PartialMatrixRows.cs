using System.Collections.Generic;
using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Matrices.Builders
{
    /// <summary>
    /// Facilitates the construction of a matrix when only some of its rows are needed explicitly.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public class PartialMatrixRows
    {
        private Dictionary<int, Dictionary<int, double>> rows;

        /// <summary>
        /// Initializes a new <see cref="PartialMatrixRows"/> instance to reperesent the relevant rows of a matrix 
        /// with the specified dimensions.
        /// </summary>
        /// <param name="numRows">The number of rows of the whole matrix.</param>
        /// <param name="numColumns">The number of columns of the whole matrix.</param>
        public PartialMatrixRows(int numRows, int numColumns)
        {
            this.NumRows = numRows;
            this.NumColumns = numColumns;
            this.rows = new Dictionary<int, Dictionary<int, double>>();
        }

        /// <summary>
        /// The number of columns of the whole matrix.
        /// </summary>
        public int NumColumns { get; }

        /// <summary>
        /// The number of rows of the whole matrix.
        /// </summary>
        public int NumRows { get; }

		/// <summary>
		/// The internal representation of the non-zero entries of the matrix: 
		/// Dictionary[int, Dictionary[int, double]] corresponds to Dictionary[row, Dictionary[column, value]]
		/// </summary>
		public Dictionary<int, Dictionary<int, double>> RawRow => rows;

		/// <summary>
		/// Adds the provided <paramref name="value"/> to the entry (<paramref name="rowIdx"/>, <paramref name="colIdx"/>). 
		/// </summary>
		/// <param name="rowIdx">The row index of the entry to modify. Constraints: 
		///     0 &lt;= <paramref name="rowIdx"/> &lt; this.<see cref="IBounded2D.NumRows"/>.</param>
		/// <param name="colIdx">The column index of the entry to modify. Constraints: 
		///     0 &lt;= <paramref name="rowIdx"/> &lt; this.<see cref="IBounded2D.NumColumns"/>.</param>
		/// <param name="value">The value that will be added to the entry (<paramref name="colIdx"/>, <paramref name="colIdx"/>).
		///     </param>
		public void AddToEntry(int rowIdx, int colIdx, double value)
        {
            if (rows.TryGetValue(rowIdx, out Dictionary<int, double> wholeRow)) // The row exists. Mutate it.
            {
                // If the entry did not exist, oldValue = default(double) = 0.0 and adding it should not reduce precision.
                wholeRow.TryGetValue(colIdx, out double oldValue);
                wholeRow[colIdx] = value + oldValue;
                //The Dictionary wholeRow is indexed twice in both cases. Is it possible to only index it once?
            }
            else // The row did not exist. Create a new one with the desired entry.
            {
                wholeRow = new Dictionary<int, double>();
                wholeRow.Add(colIdx, value);
                rows.Add(rowIdx, wholeRow);
            }
        }

        /// <summary>
        /// Performs the matrix-vector multiplication: this * <paramref name="otherVector"/>.
        /// </summary>
        /// <param name="otherVector">A vector with <see cref="IMinimalReadOnlyVector.Length"/> being equal to 
        ///     this.<see cref="NumColumns"/>.</param>
        /// <exception cref="NonMatchingDimensionsException">Thrown if the <see cref="IMinimalReadOnlyVector.Length"/> of
        ///     <paramref name="otherVector"/> is different than the this.<see cref="NumColumns"/>.</exception>
        public SparseVector MultiplyRight(Vector otherVector)
        {
            Preconditions.CheckMultiplicationDimensions(NumColumns, otherVector.Length);
            var result = new SortedDictionary<int, double>();
            foreach (var wholeRow in rows)
            {
                double dot = 0.0;
                foreach (var colValPair in wholeRow.Value)
                {
                    dot += colValPair.Value * otherVector[colValPair.Key];
                }
                result[wholeRow.Key] = dot;
            }
            return SparseVector.CreateFromDictionary(NumRows, result);
        }
    }
}
