using System;
using MGroup.LinearAlgebra.Reduction;
using MGroup.LinearAlgebra.Vectors;

//TODO: Perhaps Addition, Subtraction and Scaling must be done without using delegates, for performance
//TODO: perhaps I should return IMatrixView instead of IMatrix. By returning IMatrixView I can have classes that only implement
//      IMatrixView. On the otherMatrix hand, I cannot mutate the returned type, so its usefulness is limited.
namespace MGroup.LinearAlgebra.Matrices
{
    /// <summary>
    /// It supports common operations that do not mutate the underlying otherMatrix. If you need to store a otherMatrix and then pass it
    /// around or allow acceess to it, consider using this interface instead of <see cref="Matrix"/> for extra safety.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public interface IMatrixView: 
		IIndexable2D, IReducible, ISliceable2D, ΙDiagonalAccessible, IMinimalReadOnlyMatrix
	{
		/// <summary>
		/// Performs the following operation for all (i, j):
		/// result[i, j] = <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j] + this[i, j]. 
		/// Optimized version of <see cref="IMinimalReadOnlyMatrix.DoEntrywise(IMinimalReadOnlyMatrix, Func{double, double, double})"/> and 
		/// <see cref="LinearCombination(double, IMinimalReadOnlyMatrix, double)"/>. Named after BLAS axpy (y = a * x plus y).
		/// The resulting otherMatrix is written in a new object and then returned.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same <see cref="IBounded2D.NumRows"/> and 
		///     <see cref="IBounded2D.NumColumns"/> as this.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if <paramref name="otherMatrix"/> has different 
		///     <see cref="IBounded2D.NumRows"/> or <see cref="IBounded2D.NumColumns"/> than this.</exception>
		new IMatrix Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);
		IMinimalMatrix IMinimalReadOnlyMatrix.Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient) => Axpy(otherMatrix, otherCoefficient);

        /// <summary>
        /// Copies this <see cref="IMatrixView"/> object. A new otherMatrix of the same type as this object is initialized and 
        /// returned.
        /// </summary>
        /// <param name="copyIndexingData">
        /// If true, all Values of this object will be copied. If false, only the array(s) containing the values of the stored 
        /// otherMatrix entries will be copied. The new otherMatrix will reference the same indexing arrays as this one.
        /// </param>
        IMatrix Copy(bool copyIndexingData = false);
		IMinimalMatrix IMinimalReadOnlyMatrix.Copy() => Copy(false);

		/// <summary>Copies this <see cref="IMatrixView"/> object. The new otherMatrix will have all its entries explicitly stored.</summary>
		Matrix CopyToFullMatrix();

		/// <summary>
		/// Performs the following operation for all (i, j):
		/// result[i, j] = <paramref name="thisCoefficient"/> * this[i, j] + <paramref name="otherCoefficient"/> * 
		/// <paramref name="otherMatrix"/>[i, j]. 
		/// Optimized version of <see cref="IMinimalReadOnlyMatrix.DoEntrywise(IMinimalReadOnlyMatrix, Func{double, double, double})"/>. 
		/// The resulting otherMatrix is written in a new object and then returned.
		/// </summary>
		/// <param name="thisCoefficient">A scalar that multiplies each entry of this.</param>
		/// <param name="otherMatrix">A otherMatrix with the same <see cref="IBounded2D.NumRows"/> and 
		///     <see cref="IBounded2D.NumColumns"/> as this.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if <paramref name="otherMatrix"/> has different 
		///     <see cref="IBounded2D.NumRows"/> or <see cref="IBounded2D.NumColumns"/> than this.</exception>
		new IMatrix LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient);
		IMinimalMatrix IMinimalReadOnlyMatrix.LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient) => LinearCombination(thisCoefficient, otherMatrix, otherCoefficient);

		/// <summary>
		/// Performs the otherMatrix-otherMatrix multiplication: oper(<paramref name="otherMatrix"/>) * oper(this).
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix such that the <see cref="IBounded2D.NumColumns"/> of oper(<paramref name="otherMatrix"/>) 
		///     are equal to the <see cref="IBounded2D.NumRows"/> of oper(this).</param>
		/// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
		/// <param name="transposeOther">If true, oper(<paramref name="otherMatrix"/>) = transpose(<paramref name="otherMatrix"/>). 
		///     Otherwise oper(<paramref name="otherMatrix"/>) = <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if oper(<paramref name="otherMatrix"/>) has 
		///     different <see cref="IBounded2D.NumColumns"/> than the <see cref="IBounded2D.NumRows"/> of 
		///     oper(this).</exception>
		Matrix MultiplyLeft(IMatrixView otherMatrix, bool transposeThis = false, bool transposeOther = false);

		/// <summary>
		/// Performs the otherMatrix-otherMatrix multiplication: oper(this) * oper(<paramref name="otherMatrix"/>).
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix such that the <see cref="IBounded2D.NumRows"/> of oper(<paramref name="otherMatrix"/>) 
		///     are equal to the <see cref="IBounded2D.NumColumns"/> of oper(this).</param>
		/// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
		/// <param name="transposeOther">If true, oper(<paramref name="otherMatrix"/>) = transpose(<paramref name="otherMatrix"/>). 
		///     Otherwise oper(<paramref name="otherMatrix"/>) = <paramref name="otherMatrix"/>.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if oper(<paramref name="otherMatrix"/>) has 
		///     different <see cref="IBounded2D.NumRows"/> than the <see cref="IBounded2D.NumColumns"/> of 
		///     oper(this).</exception>
		Matrix MultiplyRight(IMatrixView otherMatrix, bool transposeThis = false, bool transposeOther = false);

		/// <summary>
		/// Performs the otherMatrix-vector multiplication: oper(this) * <paramref name="vector"/>.
		/// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
		/// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
		/// The resulting vector will be written in a new vector and returned.
		/// </summary>
		/// <param name="vector">
		/// A vector with <see cref="IReadOnlyVector.Length"/> being equal to the <see cref="IBounded2D.NumColumns"/> of 
		/// oper(this).
		/// </param>
		/// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if the <see cref="IReadOnlyVector.Length"/> of <paramref name="vector"/> is different than the 
		/// <see cref="IBounded2D.NumColumns"/> of oper(this).
		/// </exception>
		Vector Multiply(IReadOnlyVector vector, bool transposeThis = false);

		/// <summary>
		/// Performs the otherMatrix-vector multiplication: <paramref name="rhsVector"/> = oper(this) * <paramref name="lhsVector"/>.
		/// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
		/// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
		/// The resulting vector will overwrite the entries of <paramref name="rhsVector"/>.
		/// </summary>
		/// <param name="lhsVector">
		/// The vector that will be multiplied by this otherMatrix. It sits on the left hand side of the equation y = oper(A) * x.
		/// Constraints: <paramref name="lhsVector"/>.<see cref="IReadOnlyVector.Length"/> 
		/// == oper(this).<see cref="IBounded2D.NumColumns"/>.
		/// </param>
		/// <param name="rhsVector">
		/// The vector that will be overwritten by the result of the multiplication. It sits on the right hand side of the 
		/// equation y = oper(A) * x. Constraints: <paramref name="rhsVector"/>.<see cref="IReadOnlyVector.Length"/> 
		/// == oper(this).<see cref="IBounded2D.NumRows"/>.
		/// </param>
		/// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if the <see cref="IReadOnlyVector.Length"/> of <paramref name="lhsVector"/> or <paramref name="rhsVector"/> 
		/// violate the described constraints.
		/// </exception>
		/// <exception cref="Exceptions.PatternModifiedException">
		/// Thrown if the storage format of <paramref name="rhsVector"/> does not support overwritting the entries that this 
		/// method will try to.
		/// </exception>
		void MultiplyIntoResult(IReadOnlyVector lhsVector, IVector rhsVector, bool transposeThis);
        //TODO: this is NOT a specialization of a version with offsets. It is defined only if the vectors have exactly the matching lengths.

        /// <summary>
        /// Performs the following operation for all (i, j): result[i, j] = <paramref name="scalar"/> * this[i, j].
        /// The resulting otherMatrix is written in a new object and then returned.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this otherMatrix.</param>
        new IMatrix Scale(double scalar);
		IMinimalMatrix IMinimalReadOnlyMatrix.Scale(double scalar) => Scale(scalar);

		/// <summary>
		/// Returns a otherMatrix that is transpose to this: result[i, j] = this[j, i]. The entries will be explicitly copied. Some
		/// implementations of <see cref="IMatrixView"/> may offer more efficient transpositions, that do not copy the entries.
		/// If the transposed otherMatrix will be used only for multiplications, <see cref="MultiplyLeft(IMatrixView, bool, bool)"/>,
		/// <see cref="MultiplyRight(IMatrixView, bool, bool)"/> and <see cref="Multiply(IReadOnlyVector, bool)"/> are more 
		/// effient generally.
		/// </summary>
		IMatrix Transpose(); //TODO: perhaps this should default to not copying the entries, if possible.

		/// <summary>
		/// Performs a binary operation on each pair of entries: 
		/// result[i, j] = <paramref name="binaryOperation"/>(this[i, j], <paramref name="otherMatrix"/>[i]). 
		/// The resulting otherMatrix is written in a new object and then returned.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same dimensions or some other special property than this otherMatrix.</param>
		/// <param name="binaryOperation">A method that takes 2 arguments and returns 1 result.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherMatrix"/> has different dimensions or some other special property than this otherMatrix.
		/// </exception>
		new IMatrix DoEntrywise(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation)
		{
			IMatrix result = Copy();
			result.DoEntrywiseIntoThis(otherMatrix, binaryOperation);
			return result;
		}
		IMinimalMatrix IMinimalReadOnlyMatrix.DoEntrywise(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation) => DoEntrywise(otherMatrix, binaryOperation);


		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i, j]).
		/// The resulting otherMatrix is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		new IMatrix DoToAllEntries(Func<double, double> unaryOperation)
		{
			IMatrix result = Copy();
			result.DoToAllEntries(unaryOperation);
			return result;
		}
		IMinimalMatrix IMinimalReadOnlyMatrix.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);
	}
}
