namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using MGroup.LinearAlgebra.Matrices;

	/// <summary>
	/// Divides a matrix A into 4 submatrices, such as A = [A00, A01; A10, A11] (in Matlab format). The rows and
	/// columns of the original matrix A that belong to the group "0" or "1", do not have to be contiguous.
	/// The original matrix A is in CSR format. Submatrices A01, A10 are in CSR format. Submatrix A11 is in CSC format.
	/// </summary>
	/// <remarks>
	/// Base class for square, nonsymmetric submatrix extractors.
	/// </remarks>
	public abstract class SubmatrixExtractorCsrBase
	{
		protected int[] indicesGroup0;
		protected int[] indicesGroup1;
		protected CsrMatrix originalMatrix;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A00.
		/// </summary>
		protected IValuesArrayMapper mapper00;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A01.
		/// </summary>
		protected IValuesArrayMapper mapper01;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A10.
		/// </summary>
		protected IValuesArrayMapper mapper10;

		/// <summary>
		/// Maps the values array of the original matrix A to the values array of its submatrix A11.
		/// </summary>
		protected IValuesArrayMapper mapper11;

		/// <summary>
		/// A01 of A = [A00, A01; A10, A11] (using Matlab notation).
		/// </summary>
		public CsrMatrix Submatrix01 { get; protected set; }

		/// <summary>
		/// A10 of A = [A00, A01; A10, A11] (using Matlab notation).
		/// </summary>
		public CsrMatrix Submatrix10 { get; protected set; }

		/// <summary>
		/// A11 of A = [A00, A01; A10, A11] (using Matlab notation).
		/// </summary>
		public CscMatrix Submatrix11 { get; protected set; }

		/// <summary>
		/// Deletes the submatrices and any other data that was stored previously, returning the object to its just-initialized
		/// state.
		/// </summary>
		public virtual void Clear()
		{
			originalMatrix = null;
			indicesGroup0 = null;
			indicesGroup1 = null;

			Submatrix01 = null;
			Submatrix10 = null;
			Submatrix11 = null;
			mapper00 = null;
			mapper01 = null;
			mapper10 = null;
			mapper11 = null;
		}
	}
}
