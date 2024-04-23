using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

//TODO: each matrix-vector multiplication will internally cast the vectors, in order to perform efficiently. Could I avoid all
//      that casting by using generics
namespace MGroup.LinearAlgebra.Iterative
{
	/// <summary>
	/// Wrapper for a matrix class so that it can be used by iterative algorithms, which operate on 
	/// <see cref="ILinearTransformation"/>
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public class ExplicitMatrixTransformation : ILinearTransformation
    {
        private readonly IMatrixView matrix;

        /// <summary>
        /// Initializes a new instance of <see cref="ExplicitMatrixTransformation"/> that wraps the provided <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix that will be multiplied with vectors during the iterative algorithms.</param>
        public ExplicitMatrixTransformation(IMatrixView matrix) => this.matrix = matrix;

        /// <summary>
        /// See <see cref="IBounded2D.NumColumns"/>
        /// </summary>
        public int NumColumns => matrix.NumColumns;

        /// <summary>
        /// See <see cref="IBounded2D.NumRows"/>
        /// </summary>
        public int NumRows => matrix.NumRows;

		/// <summary>
		/// See <see cref="ILinearTransformation.MultiplyIntoResult(IMinimalReadOnlyVector, IMinimalVector)"/>
		/// </summary>
		public void MultiplyIntoResult(IMinimalReadOnlyVector lhsVector, IMinimalVector rhsVector) => matrix.MultiplyIntoResult(lhsVector, rhsVector, false);
    }
}
