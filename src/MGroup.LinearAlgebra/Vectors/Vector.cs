using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Reduction;
using static MGroup.LinearAlgebra.LibrarySettings;

//TODO: align Values using mkl_malloc
//TODO: tensor product, vector2D, vector3D
//TODO: remove legacy vector conversions
//TODO: add complete error checking for CopyNonContiguouslyFrom and AddNonContiguouslyFrom. Also update the documentation.
namespace MGroup.LinearAlgebra.Vectors
{
	/// <summary>
	/// General purpose fully populated vector class.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	[Serializable]
	public class Vector : AbstractContiguousFullyPopulatedVector
	{
		/// <summary>
		/// Construct a zero vector with <paramref name="length"/>.
		/// </summary>
		public Vector(int length) => Values = new double[length];

		/// <summary>
		/// Construct a vector from an array of elements.
		/// </summary>
		/// <param name="elements">Array of elements provided as is in vector.
		/// Any later change to a vector element, also modifies corresponding element of this array.
		/// If you don't want that, use (double[]) elements.Clone()</param>
		public Vector(double[] elements) => Values = elements;

		/// <summary>
		/// Construct a vector from another.
		/// </summary>
		/// <param name="vector">Source vector. Its contents copied to the newly created this vector.</param>
		public Vector(IExtendedReadOnlyVector vector) => Values = vector.CopyToArray();

		public override double[] Values { get; }

		public override int FromIndex { get => 0; }

		public override int Length { get => Values.Length; }

		public override ref double this[int index] => ref Values[index];



		/* Valid in C#9
		// ------------------- COVARIANT RETURN TYPE FROM AbstractContiguousFullyPopulatedVector

		override public Vector View(int fromIndex, int toIndex) => (Vector)base.View(fromIndex, toIndex);
		override public PermutatedVectorView View(int[] indices) => (PermutatedVectorView)base.View(indices);
		override public Vector AddIntoThis(IMinimalReadOnlyVector otherVector) => (Vector)base.AddIntoThis(otherVector);
		override public Vector Clear() => (Vector)base.Clear();
		override public Vector CopyFrom(IMinimalReadOnlyVector otherVector) => (Vector)base.CopyFrom(otherVector);
		override public Vector DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => (Vector)base.DoToAllEntriesIntoThis(unaryOperation);
		override public Vector LinearCombinationIntoThis(double thisCoefficient, AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (Vector)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public Vector LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient) => (Vector)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public Vector NegateIntoThis() => (Vector)base.NegateIntoThis();
		override public Vector SetAll(double value) => (Vector)base.SetAll(value);
		override public Vector SubtractIntoThis(IMinimalReadOnlyVector otherVector) => (Vector)base.SubtractIntoThis(otherVector);
		override public Vector AxpyIntoThis(AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (Vector)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public Vector AxpyIntoThis(SparseVector otherVector, double otherCoefficient) => (Vector)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public Vector AxpyIntoThis(IMinimalReadOnlyVector otherVector, double otherCoefficient) => (Vector)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public Vector DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation) => (Vector)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public Vector DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (Vector)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public Vector DoEntrywiseIntoThis(IMinimalReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => (Vector)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public Vector ScaleIntoThis(double coefficient) => (Vector)base.ScaleIntoThis(coefficient);
		*/



		// -------- OPERATORS FROM IMinimalReadOnlyVector

		public static Vector operator -(Vector x) => x.Negate();
		public static Vector operator +(Vector x, Vector y) => x.Add(y);
		public static Vector operator +(Vector x, IMinimalReadOnlyVector y) => x.Add(y);
		public static Vector operator +(IMinimalReadOnlyVector y, Vector x) => x.Add(y);
		public static Vector operator -(Vector x, Vector y) => x.Subtract(y);
		public static Vector operator -(Vector x, IMinimalReadOnlyVector y) => x.Subtract(y);
		public static Vector operator -(IMinimalReadOnlyVector x, Vector y) => y.LinearCombination(-1, x, 1);
		public static double operator *(Vector x, Vector y) => x.DotProduct(y);
		public static double operator *(Vector x, IMinimalReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IMinimalReadOnlyVector x, Vector y) => x.DotProduct(y);
		public static Vector operator *(Vector x, double y) => x.Scale(y);
		public static Vector operator *(double y, Vector x) => x.Scale(y);




































		// ------------ STATICALLY CONSTRUCT VECTOR



		/// <summary>
		/// Initializes a new instance of <see cref="Vector"/> with <paramref name="data"/> or a clone as its internal array.
		/// </summary>
		/// <param name="data">The entries of the vector to create.</param>
		/// <param name="copyArray">If true, <paramref name="data"/> will be copied and the new <see cref="Vector"/> instance 
		///     will have a reference to the copy, which is safer. If false, the new vector will have a reference to 
		///     <paramref name="data"/> itself, which is faster.</param>
		/// <returns></returns>
//		[Obsolete("Use new Vector(data) or new Vector((double[])data.Clone())")]
		public static Vector CreateFromArray(double[] data, bool copyArray = false) => new Vector(copyArray ? (double[])data.Clone() : data);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector"/> by copying the entries of an existing vector: 
		/// <paramref name="original"/>.
		/// </summary>
		/// <param name="original">The original vector to copy.</param>
//		[Obsolete("Use new Vector(original.CopyToArray()) or new Vector(original)")]
		public static Vector CreateFromVector(IExtendedReadOnlyVector original) => new Vector(original);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector"/> with all entries being equal to <paramref name="value"/>.
		/// </summary>
		/// <param name="length">The number of entries of the new <see cref="Vector"/> instance.</param>
		/// <param name="value">The value that all entries of the new vector will be initialized to.</param>
//		[Obsolete("Use v = new Vector(length); v.SetAll(value);")]
		public static Vector CreateWithValue(int length, double value)
		{
			var result = new Vector(length);
			result.SetAll(value);
			return result;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Vector"/> with all entries being equal to 0.
		/// </summary>
		/// <param name="length">The number of entries of the new <see cref="Vector"/> instance.</param>
//		[Obsolete("Use new Vector(length)")]
		public static Vector CreateZero(int length) => new Vector(length);






		// ----------- OBSOLETE SUBVECTOR OPERATIONS



		[Obsolete("Use this.View(thisIndices).AddIntoThis(otherVector.View(otherIndices))")]
		public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, AbstractFullyPopulatedVector otherVector, int[] otherIndices)
			=> View(thisIndices).AddIntoThis(otherVector.View(otherIndices));

		[Obsolete("Use this.View(thisIndices).AddIntoThis(otherVector)")]
		public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IMinimalReadOnlyVector otherVector)
			=> View(thisIndices).AddIntoThis(otherVector);

		/// <summary>
		/// Performs the operation: this[<paramref name="destinationIdx"/> + i] = this[<paramref name="destinationIdx"/> + i]
		/// + <paramref name="sourceVector"/>[<paramref name="sourceIdx"/> + j], for 0 &lt;= j &lt; <paramref name="length"/>.
		/// </summary>
		/// <param name="length">The number of entries to add.</param>
		/// <param name="destinationIdx">
		/// The index into this <see cref="Vector"/> instance, at which to start adding entries to. Constraints: 
		/// 0 &lt;= <paramref name="destinationIdx"/>, 
		/// <paramref name="destinationIdx"/> + <paramref name="length"/> &lt;= this.<see cref="Length"/>.
		/// </param>
		/// <param name="sourceVector">The vector that will be added to a part of this <see cref="Vector"/> instance.</param>
		/// <param name="sourceIdx">
		/// The index into <paramref name="sourceVector"/>, at which to start adding entries from. Constraints:
		/// 0 &lt;= <paramref name="sourceIdx"/>, 
		/// <paramref name="sourceIdx"/> + <paramref name="length"/> &lt;= <paramref name="sourceVector"/>.<see cref="Length"/>.
		/// </param>
		/// <exception cref="NonMatchingDimensionsException">
		/// Thrown if <paramref name="destinationIdx"/>, <paramref name="sourceVector"/> or <paramref name="sourceIdx"/> 
		/// violate the described constraints.
		/// </exception>
		[Obsolete("Use this.View(destinationIdx, destinationIdx + length).AddIntoThis(sourceVector.View(sourceIdx, sourceIdx + length))")]
		public void AddSubvectorIntoThis(int destinationIdx, IExtendedReadOnlyVector sourceVector, int sourceIdx, int length)
			=> View(destinationIdx, destinationIdx + length).AddIntoThis(sourceVector.View(sourceIdx, sourceIdx + length));

		[Obsolete("Use this.View(destinationIndex, destinationIndex + length).AxpyIntoThis(sourceVector.View(sourceIndex, sourceIndex + length), sourceCoefficient)")]
		public void AxpySubvectorIntoThis(int destinationIndex, IExtendedReadOnlyVector sourceVector, double sourceCoefficient, int sourceIndex, int length)
			=> View(destinationIndex, destinationIndex + length).AxpyIntoThis(sourceVector.View(sourceIndex, sourceIndex + length), sourceCoefficient);


		[Obsolete("Use this.View(thisIndices).CopyFrom(otherVector.View(otherIndices))")]
		public void CopyNonContiguouslyFrom(int[] thisIndices, AbstractFullyPopulatedVector otherVector, int[] otherIndices)
			=> View(thisIndices).CopyFrom(otherVector.View(otherIndices));


		/// <summary>
		/// Copies selected entries from <paramref name="otherVector"/> to this vector:
		/// this[<paramref name="thisIndices"/>[i]] = <paramref name="otherVector"/>[i],
		/// for 0 &lt;= i &lt; this.
		/// </summary>
		/// <param name="thisIndices">
		/// The indices of this vector, where entries will be copied to. Constraints: 
		/// 2) 0 &lt;= <paramref name="thisIndices"/>[i] &lt; this, for all valid i</param>
		/// <param name="otherVector">The vector from which entries will be copied.</param>
		/// /// <exception cref="IndexOutOfRangeException">
		/// Thrown if <paramref name="thisIndices"/> violates the described constraints.
		/// </exception>
		[Obsolete("Use this.View(thisIndices).CopyFrom(otherVector)")]
		public void CopyNonContiguouslyFrom(int[] thisIndices, IMinimalReadOnlyVector otherVector)
			=> View(thisIndices).CopyFrom(otherVector);

		[Obsolete("Use this.CopyFrom(otherVector.View(otherIndices))")]
		public void CopyNonContiguouslyFrom(AbstractFullyPopulatedVector otherVector, int[] otherIndices)
			=> CopyFrom(otherVector.View(otherIndices));

		[Obsolete("Use this.View(destinationIndex, destinationIndex + length).CopyFrom(sourceVector.View(sourceIndex, sourceIndex + length))")]
		public void CopySubvectorFrom(int destinationIndex, IExtendedReadOnlyVector sourceVector, int sourceIndex, int length)
			=> View(destinationIndex, destinationIndex + length).CopyFrom(sourceVector.View(sourceIndex, sourceIndex + length));

		[Obsolete("Use this.Copy(indices)")]
		public Vector GetSubvector(int[] indices) => Copy(indices);

		[Obsolete("Use this.Copy(startInclusive, endExclusive)")]
		public Vector GetSubvector(int startInclusive, int endExclusive) => Copy(startInclusive, endExclusive);

		/// <summary>
		/// Creates a new <see cref="Vector"/> that contains all entries of this followed by all entries of 
		/// <paramref name="last"/>.
		/// </summary>
		/// <param name="last">The vector whose entries will be appended after all entries of this vector.</param>
		[Obsolete("Use View with some lines more")]
		public Vector Append(Vector last)
		{
			var result = new Vector(Length + last.Length);
			result.View(0, Length).CopyFrom(this);
			result.View(Length, result.Length).CopyFrom(last);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Vector"/> that contains the entries of this <see cref="Vector"/> with a different order,
		/// which is specified by the provided <paramref name="permutation"/> and <paramref name="oldToNew"/>.
		/// </summary>
		/// <param name="permutation">An array that contains the indices of this <see cref="Vector"/> in a different 
		///     order.</param>
		/// <param name="oldToNew">If true, reordered[<paramref name="permutation"/>[i]] = original[i]. If false, 
		///     reordered[i] = original[<paramref name="permutation"/>[i]].</param>
		[Obsolete("Use View(indices) both ways")]
		public Vector Reorder(IReadOnlyList<int> permutation, bool oldToNew)
		{
			// this copy is a super bad approach but it is obsolete
			int[] indices = new int[permutation.Count];
			for (int i = 0; i < permutation.Count; ++i)
				indices[i] = permutation[i];
			if (oldToNew)
			{
				var result = new Vector(Length);
				result.View(indices).CopyFrom(this);
				return result;
			}
			else return Copy(indices);
		}



		// -------------- REMAININGS FROM VECTOR

		/// <summary>
		/// The internal array that stores the entries of the vector. 
		/// It should only be used for passing the raw array to linear algebra libraries.
		/// </summary>
		[Obsolete("Use Values")]
		public double[] RawData => Values;



		// ----------- ENTRYWISE OPERATIONS


		/// <summary>
		/// Performs the following operation for 0 &lt;= i &lt; this.<see cref="Length"/>: 
		/// result[i] = this[i] * <paramref name="vector"/>[i]. 
		/// The resulting vector is written to a new <see cref="Vector"/> and then returned.
		/// </summary>
		/// <param name="vector">A vector with the same <see cref="Length"/> as this <see cref="Vector"/> instance.</param>
		[Obsolete("Use this.DoEntrywise(vector, (x, y) => x * y)")]
		public Vector MultiplyEntrywise(Vector vector) => DoEntrywise(vector, (x, y) => x * y);

		/// <summary>
		/// Performs the following operation for 0 &lt;= i &lt; this.<see cref="Length"/>: 
		/// this[i] = this[i] * <paramref name="vector"/>[i]. 
		/// The resulting vector overwrites the entries of this <see cref="Vector"/> instance.
		/// </summary>
		/// <param name="vector">A vector with the same <see cref="Length"/> as this <see cref="Vector"/> instance.</param>
		[Obsolete("Use this.DoEntrywiseIntoThis(vector, (x, y) => x * y)")]
		public void MultiplyEntrywiseIntoThis(Vector vector) => DoEntrywiseIntoThis(vector, (x, y) => x * y);






		// ------------ VERY BAD PLACE FOR THAT ALIEN OPERATION


		/// <summary>
		/// This method is used to remove duplicate values of a Knot Value Vector and return the multiplicity up to
		/// the requested Knot. The multiplicity of a single Knot can be derived using the exported multiplicity vector. 
		/// The entries of this <see cref="Vector"/> will be sorted.
		/// </summary>
		[Obsolete("Obsolete because I want to know where this alien operation needed")]
		public Vector[] RemoveDuplicatesFindMultiplicity()
		{
			Array.Sort(Values);
			HashSet<double> set = new HashSet<double>();
			int indexSingles = 0;
			double[] singles = new double[Values.Length];

			int[] multiplicity = new int[Values.Length];
			int counterMultiplicity = 0;

			for (int i = 0; i < Values.Length; i++)
			{
				// If same integer is already present then add method will return
				// FALSE
				if (set.Add(Values[i]) == true)
				{
					singles[indexSingles] = Values[i];

					multiplicity[indexSingles] = counterMultiplicity;
					indexSingles++;

				}
				else
				{
					counterMultiplicity++;
				}
			}
			int numberOfZeros = 0;
			for (int i = Values.Length - 1; i >= 0; i--)
			{
				if (singles[i] == 0)
				{
					numberOfZeros++;
				}
				else
				{
					break;
				}
			}
			Vector[] singlesMultiplicityVectors = new Vector[2];

			singlesMultiplicityVectors[0] = new Vector(Values.Length - numberOfZeros);
			for (int i = 0; i < Values.Length - numberOfZeros; i++)
			{
				singlesMultiplicityVectors[0][i] = singles[i];
			}

			singlesMultiplicityVectors[1] = new Vector(Values.Length - numberOfZeros);
			for (int i = 0; i < Values.Length - numberOfZeros; i++)
			{
				singlesMultiplicityVectors[1][i] = multiplicity[i];
			}

			return singlesMultiplicityVectors;
		}
	}
}
