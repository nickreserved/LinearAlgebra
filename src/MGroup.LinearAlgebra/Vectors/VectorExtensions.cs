//TODO: Move the operators here when C# supports extension operators
using System;
using System.Collections.Generic;
using System.Numerics;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;

//TODO: Use the generic interfaces IEntrywiseOperable1D, etc (and create some for axpy, linear combo), instead of implementing
//      the extensions for each vector/matrix type and the IExtendedReadOnlyVector, etc. interfaces. However the extension method should be
//      concise as possible. Having to declare the generic types (see the generic MultiplyEntrywise) is prohibitive, especially
//      if IntelliSense does not suggest the generic extension method. How dids LINQ solve this issue?
namespace MGroup.LinearAlgebra.Vectors
{
	/// <summary>
	/// Defines common vector operation shortcuts that can be used as extensions for <see cref="Vector"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public static class VectorExtensions
	{
		/// <summary>
		/// Returns the X component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 2nd and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The X component of the cross product</returns>
		public static double CrossProductX(this AbstractFullyPopulatedVector thisVector, AbstractFullyPopulatedVector otherVector)
		{
			if (thisVector.Length != 3 || otherVector.Length != 3)
				throw new NonMatchingDimensionsException($"thisVector and otherVector must have Length = 3 and they have Lengths {thisVector.Length} and {otherVector.Length}");
			return thisVector[1] * otherVector[2] - thisVector[2] * otherVector[1];
		}
		/// <summary>
		/// Returns the Y component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 1st and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Y component of the cross product</returns>
		public static double CrossProductY(this AbstractFullyPopulatedVector thisVector, AbstractFullyPopulatedVector otherVector)
		{
			if (thisVector.Length != 3 || otherVector.Length != 3)
				throw new NonMatchingDimensionsException($"thisVector and otherVector must have Length = 3 and they have Lengths {thisVector.Length} and {otherVector.Length}");
			return -thisVector[0] * otherVector[2] + thisVector[2] * otherVector[0];
		}
		/// <summary>
		/// Returns the Z component of the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 2 dimensional vectors.
		/// </summary>
		/// For calculation uses the 2 first components of this vector with 2 first components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Z component of the cross product</returns>
		public static double CrossProductZ(this AbstractFullyPopulatedVector thisVector, AbstractFullyPopulatedVector otherVector)
		{
			if (	(thisVector.Length != 3 || otherVector.Length != 3) &&
					(thisVector.Length != 2 || otherVector.Length != 2))
				throw new NonMatchingDimensionsException($"thisVector and otherVector must have Length = 2 or 3 and they have Lengths {thisVector.Length} and {otherVector.Length}");
			return thisVector[0] * otherVector[1] - thisVector[1] * otherVector[0];
		}
		/// <summary>
		/// Returns the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 3 dimensional vectors.
		/// </summary>
		/// <param name="otherVector">The other vector</param>
		/// <returns>The cross product</returns>
		public static Vector CrossProduct(this AbstractFullyPopulatedVector thisVector, AbstractFullyPopulatedVector otherVector)
		{
			if (thisVector.Length != 3 || otherVector.Length != 3)
				throw new NonMatchingDimensionsException($"thisVector and otherVector must have Length = 3 and they have Lengths {thisVector.Length} and {otherVector.Length}");
			return new Vector(new double[]
			{
				thisVector[1] * otherVector[2] - thisVector[2] * otherVector[1],
				-thisVector[0] * otherVector[2] + thisVector[2] * otherVector[0],
				thisVector[0] * otherVector[1] - thisVector[1] * otherVector[0]
			});
		}







		// -------------------- ENTRYWISE OPERATIONS

		/// <summary>
		/// Performs the operation: result[i] = <paramref name="thisVector"/>[i] * <paramref name="otherVector"/>[i], for all
		/// valid i. The resulting vector is written in a new object and then returned.
		/// </summary>
		/// <param name="thisVector">A vector.</param>
		/// <param name="otherVector">A vector with the same <see cref="IMinimalReadOnlyVector.Length"/> as this vector.</param>
		/// <exception cref="NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this vector.
		/// </exception>
		[Obsolete("Use thisVector.DoEntrywise(otherVector, (x, y) => x * y)")]
		public static IMinimalVector MultiplyEntrywise(
			this IMinimalReadOnlyVector thisVector, IMinimalReadOnlyVector otherVector)
			=> thisVector.DoEntrywise(otherVector, (x, y) => x * y); //TODO: nice in theory, but passing a lambda to DoEntrywise is less verbose.

		/// <summary>
		/// Performs the operation: 
		/// <paramref name="thisVector"/>[i] = <paramref name="thisVector"/>[i] * <paramref name="otherVector"/>[i], for all
		/// valid i. The resulting vector overwrites the entries of this vector.
		/// </summary>
		/// <param name="thisVector">A vector.</param>
		/// <param name="otherVector">A vector with the same <see cref="IMinimalReadOnlyVector.Length"/> as this vector.</param>
		/// <exception cref="NonMatchingDimensionsException">
		/// Thrown if <paramref name="otherVector"/> has different <see cref="IMinimalReadOnlyVector.Length"/> than this vector.
		/// </exception>
		/// <exception cref="PatternModifiedException">
		/// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
		/// </exception>
		[Obsolete("Use thisVector.DoEntrywiseIntoThis(otherVector, (x, y) => x * y)")]
		public static void MultiplyEntrywiseIntoThis(
			this IMinimalVector thisVector, IMinimalReadOnlyVector otherVector)
			=> thisVector.DoEntrywiseIntoThis(otherVector, (x, y) => x * y); //TODO: nice in theory, but passing a lambda to DoEntrywise() is less verbose.

		/// <summary>
		/// Performs the operation: result[i] = this[i] ^ 0.5 for all valid i. 
		/// The resulting vector is written in a new object and then returned.
		/// </summary>
		[Obsolete("Use thisVector.DoToAllEntries(x => Math.Sqrt(x))")]
		public static IMinimalVector Sqrt(this IMinimalReadOnlyVector vector) => vector.DoToAllEntries(x => Math.Sqrt(x));

		/// <summary>
		/// Performs the operation: this[i] = this[i] ^ 0.5 for all valid i. 
		/// The resulting vector overwrites the entries of this vector.
		/// </summary>
		[Obsolete("Use thisVector.DoToAllEntriesIntoThis(x => Math.Sqrt(x))")]
		public static void SqrtIntoThis(this IMinimalVector vector) => vector.DoToAllEntriesIntoThis(x => Math.Sqrt(x));

		/// <summary>
		/// Performs the operation: result[i] = this[i] ^ 2 for all valid i. 
		/// The resulting vector is written in a new object and then returned.
		/// </summary>
		[Obsolete("Use thisVector.DoToAllEntries(x => x * x)")]
		public static IMinimalVector Square(this IMinimalReadOnlyVector vector) => vector.DoToAllEntries(x => x * x);

		/// <summary>
		/// Performs the operation: this[i] = this[i] ^ 2 for all valid i. 
		/// The resulting vector overwrites the entries of this vector.
		/// </summary>
		[Obsolete("Use thisVector.DoToAllEntriesIntoThis(x => x * x)")]
		public static void SquareIntoThis(this IMinimalVector vector) => vector.DoToAllEntriesIntoThis(x => x * x);


		
		// ------------ OBSOLETE SUBVECTOR OPERATIONS

		/// <summary>
		/// Performs the following operation for <paramref name="length"/> consecutive entries starting from the provided 
		/// indices: this[i] = this[i] + <paramref name="sourceVector"/>[i].
		/// </summary>
		/// <param name="destinationIndex">The index into this <see cref="IExtendedVector"/> where to start overwritting. Constraints:
		///     <paramref name="destinationIndex"/> + <paramref name="length"/> &lt;= this.<see cref="IMinimalReadOnlyVector.Length"/>.
		///     </param>
		/// <param name="sourceVector">The other vector operand.</param>
		/// <param name="sourceIndex">The index into <paramref name="sourceVector"/> where to start operating. Constraints: 
		///     <paramref name="sourceIndex"/> + <paramref name="length"/> &lt;= 
		///     <paramref name="sourceVector"/>.<see cref="IMinimalReadOnlyVector.Length"/>.</param>
		/// <param name="length">The number of entries to copy.</param>
		/// <exception cref="Exceptions.NonMatchingDimensionsException">Thrown if <paramref name="length"/> and 
		///     <paramref name="destinationIndex"/> or <paramref name="sourceIndex"/> violate the described constraints.
		///     </exception>
		/// <exception cref="Exceptions.PatternModifiedException">Thrown if an entry this[i] needs to be overwritten, but that 
		///     is not permitted by the vector storage format.</exception>
		[Obsolete("Use destinationVector.View(destinationIndex, destinationIndex + length).AddIntoThis(sourceVector.View(sourceIndex, sourceIndex + length))")]
		public static void AddSubvectorIntoThis(this IExtendedVector destinationVector, int destinationIndex, IExtendedReadOnlyVector sourceVector,
			int sourceIndex, int length)
			=> destinationVector.View(destinationIndex, destinationIndex + length).AddIntoThis(sourceVector.View(sourceIndex, sourceIndex + length));

		/// <summary>
		/// Performs the following operation for <paramref name="length"/> consecutive entries starting from the provided 
		/// indices: this[i] = this[i] - <paramref name="sourceVector"/>[i].
		/// </summary>
		/// <param name="destinationIndex">The index into this <see cref="IExtendedVector"/> where to start overwritting. Constraints:
		///     <paramref name="destinationIndex"/> + <paramref name="length"/> &lt;= this.<see cref="IMinimalReadOnlyVector.Length"/>.
		///     </param>
		/// <param name="sourceVector">The other vector operand.</param>
		/// <param name="sourceIndex">The index into <paramref name="sourceVector"/> where to start operating. Constraints: 
		///     <paramref name="sourceIndex"/> + <paramref name="length"/> &lt;= 
		///     <paramref name="sourceVector"/>.<see cref="IMinimalReadOnlyVector.Length"/>.</param>
		/// <param name="length">The number of entries to copy.</param>
		/// <exception cref="NonMatchingDimensionsException">Thrown if <paramref name="length"/> and 
		///     <paramref name="destinationIndex"/> or <paramref name="sourceIndex"/> violate the described constraints.
		///     </exception>
		/// <exception cref="PatternModifiedException">Thrown if an entry this[i] needs to be overwritten, but that 
		///     is not permitted by the vector storage format.</exception>
		[Obsolete("Use destinationVector.View(destinationIndex, destinationIndex + length).SubtractIntoThis(sourceVector.View(sourceIndex, sourceIndex + length))")]
		public static void SubtractSubvectorIntoThis(this IExtendedVector destinationVector, int destinationIndex,
			IExtendedReadOnlyVector sourceVector, int sourceIndex, int length)
			=> destinationVector.View(destinationIndex, destinationIndex + length).SubtractIntoThis(sourceVector.View(sourceIndex, sourceIndex + length));



		// ------------ COLLECT ELEMENTS OF VECTOR BASED ON A PREDICATE

		public static int[] Find(this AbstractFullyPopulatedVector vector)
		{
			var result = new List<int>(vector.Length);
			for (int i = 0; i < vector.Length; ++i)
			{
				if (vector[i] != 0)
				{
					result.Add(i);
				}
			}
			return result.ToArray();
		}

		public static int[] Find(this AbstractFullyPopulatedVector vector, Predicate<double> predicate)
		{
			var result = new List<int>(vector.Length);
			for (int i = 0; i < vector.Length; ++i)
			{
				if (predicate(vector[i]))
				{
					result.Add(i);
				}
			}
			return result.ToArray();
		}
	}
}
