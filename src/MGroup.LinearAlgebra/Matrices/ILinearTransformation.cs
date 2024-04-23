using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Matrices
{
	/// <summary>
	/// Defines matrix-vector multiplication to allow iterative algorithms to operate without any modifications on various 
	/// matrix and vector types, such as distributed matrices and vectors.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public interface ILinearTransformation : IBounded2D
	{
		/// <summary>
		/// Performs the matrix-vector multiplication (with the matrix represented by this 
		/// <see cref="ILinearTransformation"/>): <paramref name="lhsVector"/> = thisLinearTransformation * <paramref name="rhsVector"/>.
		/// </summary>
		/// <param name="rhsVector">The vector that will be multiplied by the represented matrix.</param>
		/// <param name="lhsVector">The vector that will be overwritten by the result of the multiplication.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="rhsVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/>
		/// than this linear transformation <see cref="IBounded2D.NumColumns"/>, or
		/// if <paramref name="lhsVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/>
		/// than this linear transformation <see cref="IBounded2D.NumRows"/>.</exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if the storage format of <paramref name="lhsVector"/> does not support overwritting the entries that this method will try to.</exception>
		void MultiplyIntoResult(IMinimalReadOnlyVector rhsVector, IMinimalVector lhsVector);
	}
}
