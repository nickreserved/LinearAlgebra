using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative
{
    /// <summary>
    /// Defines matrix-vector multiplication to allow iterative algorithms to operate without any modifications on various 
    /// matrix and vector types, such as distributed matrices and vectors.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public interface ILinearTransformation
	{
		/// <summary>
		/// Performs the matrix-vector multiplication (with the matrix represented by this 
		/// <see cref="ILinearTransformation"/>): <paramref name="outputVector"/> = this * <paramref name="inputVector"/>.
		/// </summary>
		/// <param name="inputVector">
		/// The vector that will be multiplied by the represented matrix. It sits on the left hand side of the equation 
		/// y = A * x. Constraints: Its <see cref="IFinite1D.Length()"/> must be equal to the number of columns of the matrix  
		/// represented by this <see cref="ILinearTransformation"/>.
		/// </param>
		/// <param name="outputVector">
		/// The vector that will be overwritten by the result of the multiplication. It sits on the right hand side of the 
		/// equation y = A * x. Constraints: Its <see cref="IFinite1D.Length()"/> must be equal to the number of rows of the
		/// matrix represented by this <see cref="ILinearTransformation"/>.
		/// </param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="inputVector"/> or <paramref name="outputVector"/> violate the described constraints.
		/// </exception>
		void Multiply(IImmutableVector inputVector, IMutableVector outputVector);
    }
}
