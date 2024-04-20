namespace MGroup.LinearAlgebra.SchurComplements.SubmatrixExtractors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using MGroup.LinearAlgebra.Matrices;

	/// <summary>
	/// Divides a matrix A into 4 submatrices, such as A = [A00, A01; A10, A11] (in Matlab format). The rows and
	/// columns of the original matrix A that belong to the group "0" or "1", do not have to be contiguous.
	/// The original matrix A is in CSR format. Submatrices A01, A10 are in CSR format. Submatrix A11 is in CSC format.
	/// </summary>
	public abstract class SubmatrixExtractorCsrBase
	{
		protected int[] indicesGroup0;
		protected int[] indicesGroup1;
		protected CsrMatrix originalMatrix;

		/// <summary>
		/// Maps indices of the values arrays between the original matrix A and its submatrix A00
		/// A00.Values[i] = A.Values[map00[i]].
		/// </summary>
		protected int[] map00;

		/// <summary>
		/// Maps indices of the values arrays between the original matrix A and its submatrix A01
		/// A01.Values[i] = A.Values[map01[i]].
		/// </summary>
		protected int[] map01;

		/// <summary>
		/// Maps indices of the values arrays between the original matrix A and its submatrix A01
		/// A10.Values[i] = A.Values[map10[i]].
		/// </summary>
		protected int[] map10;

		/// <summary>
		/// Maps indices of the values arrays between the original matrix A and its submatrix A11
		/// A11.Values[i] = A.Values[map11[i]].
		/// </summary>
		protected int[] map11;

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
			map00 = null;
			map01 = null;
			map10 = null;
			map11 = null;
		}
	}
}
