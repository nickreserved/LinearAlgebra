using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Reduction;
using static MGroup.LinearAlgebra.LibrarySettings;

namespace MGroup.LinearAlgebra.Vectors
{
    /// <summary>
    /// A vector that only stores non-zero entries. Some zero entries can also be stored but they are  non-structural zeros 
    /// and thus handled as non-zero entries.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public class SparseVector: AbstractSparseVector
    {
		/// <summary>
		/// Creates a sparse vector
		/// </summary>
		/// <param name="length">Number of vector elements, zero or non-zero.</param>
		/// <param name="values">An array of non-zero element values, althrough it is not a problem if some elements are zero.
		/// Number of array elements must be equal to number of elements in <paramref name="indices"/> array and at most <paramref name="length"/>.</param>
		/// <param name="indices">An array of indices corresponding to <paramref name="values"/>, in ascending order.
		/// Number of array elements must be equal to number of elements in <paramref name="values"/> array and at most <paramref name="length"/>.</param>
		public SparseVector(int length, double[] values, int[] indices)
        {
            Length = length;
            Values = values;
            Indices = indices;
			Debug.Assert(Indices.Length == 0 || Indices.Last() < Length);
			Debug.Assert(Indices.Length <= Values.Length);
        }

		/// <summary>
		/// Create a sparse vector from a dense vector
		/// </summary>
		/// <param name="otherVector">The other dense vector</param>
		/// <param name="torelance">Dense vector's elements with absolute value smaller or equal than that, considered zero and they are not stored in sparse vector.</param>
		public SparseVector(AbstractFullyPopulatedVector otherVector, double torelance = 1e-7)
		{
			Length = otherVector.Length;
			Indices = new int[Length];
			int pos = 0;
			for (int i = 0; i < Length; ++i)
				if (Math.Abs(otherVector[i]) > torelance)
					Indices[pos++] = i;
			Values = new double[pos];
			for (int i = 0; i < Values.Length; ++i)
				Values[i] = otherVector[Indices[i]];
			if (Indices.Length != Values.Length)
			{
				var indices = new int[Values.Length];
				Array.Copy(Indices, indices, indices.Length);
				Indices = indices;
			}
		}

		override public int FromIndex { get => 0; }
		override public int ToIndex { get => Indices.Length; }

		override public AbstractSparseVector CopyFrom(AbstractSparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			Indices = new int[otherVector.ToIndex - otherVector.FromIndex];
			Values = new double[Indices.Length];
			Array.Copy(otherVector.Indices, otherVector.FromIndex, Indices, 0, Indices.Length);
			Array.Copy(otherVector.Values, otherVector.FromIndex, Values, 0, Values.Length);
			return this;
		}
























		/// <summary>
		/// The internal array that stores the indices of the non-zero entries of the vector. 
		/// Its length is equal to the number of non-zero entries.
		/// It should only be used for passing the raw array to linear algebra libraries.
		/// </summary>
		[Obsolete("Use Indices instead")]
		public int[] RawIndices => Indices;

		/// <summary>
		/// The internal array that stores the values of the non-zero entries of the vector.
		/// Its length is equal to the number of non-zero entries.
		/// It should only be used for passing the raw array to linear algebra libraries.
		/// </summary>
		[Obsolete("Use Values instead")]
		public double[] RawValues => Values;

		/// <summary>
		/// Creates a new instance of <see cref="SparseVector"/> with the provided arrays as its internal Elements.
		/// </summary>
		/// <param name="length">The number of zero and non-zero entries of the new <see cref="SparseVector"/>.</param>
		/// <param name="values">The internal array that stores the values of the non-zero entries of the vector. Constraints: 
		///     <paramref name="values"/>.Length == <paramref name="indices"/>.Length &lt;= <paramref name="length"/>.</param>
		/// <param name="indices">The internal array that stores the indices of the non-zero entries of the vector. Constraints: 
		///     <paramref name="values"/>.Length == <paramref name="indices"/>.Length &lt;= <paramref name="length"/>.</param>
		/// <param name="checkInput">If true, the validity of <paramref name="values"/> and <paramref name="indices"/> will be 
		///     checked, which is safer. If false, no check will be made, which is daster.</param>
		/// <param name="sortInput">If true, <paramref name="values"/> and <paramref name="indices"/> will be sorted in
		///     ascending order of the entries of <paramref name="indices"/>. If false, they are assumed to be sorted. If they 
		///     are not, some methods may produce errors or have lower performance.</param>
		public static SparseVector CreateFromArrays(int length, double[] values, int[] indices, 
            bool checkInput, bool sortInput)
        {
            bool verifiedSorted = false;
            if (sortInput)
            {
                Array.Sort<int, double>(indices, values);
                verifiedSorted = true;
            }

            if (checkInput)
            {
                int nnz = indices.Length;
                if (values.Length != nnz)
                {
                    throw new ArgumentException("The length of the values and indices arrays must be equal (and equal"
                        + $" to the number of non zero entries), but were {values.Length} and {indices.Length} respectively");
                }
                if (!verifiedSorted) // if they have already been sorted we do not need to check them again.
                {
                    for (int i = 1; i < nnz; ++i)
                    {
                        if (indices[i] <= indices[i - 1]) throw new ArgumentException("The indices array must be sorted");
                    }
                }
                int maxIndex = indices[nnz - 1]; // first sort them
                if (maxIndex >= length) 
                {
                    throw new ArgumentException($"This sparse vector contains {maxIndex + 1} entries, while its length is {length}");
                }
            }
            return new SparseVector(length, values, indices);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> with the entries of <paramref name="denseArray"/>. Only the
        /// entries that satisfy: <paramref name="denseArray"/>[i] != 0 are explicitly stored in the new 
        /// <see cref="SparseVector"/>.
        /// </summary>
        /// <param name="denseArray">The original vector that will be converted to <see cref="SparseVector"/>.</param>
        public static SparseVector CreateFromDense(double[] denseArray)
        {
            var indicesValues = new SortedDictionary<int, double>();
            for (int i = 0; i < denseArray.Length; ++i)
            {
                if (denseArray[i] != 0) indicesValues.Add(i, denseArray[i]);
            }
            return CreateFromDictionary(denseArray.Length, indicesValues);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> with the entries of <paramref name="denseArray"/>. Only the
        /// entries that satisfy: <paramref name="denseArray"/>[i] > <paramref name="tolerance"/> are explicitly stored in the 
        /// new <see cref="SparseVector"/>.
        /// </summary>
        /// <param name="denseArray">The original vector that will be converted to <see cref="SparseVector"/>.</param>
        /// <param name="tolerance">The tolerance under which an entry of <paramref name="denseArray"/> is considered to be 0. 
        ///     Constraints: <paramref name="tolerance"/> &gt; 0. To keep only exact zeros, instead of setting 
        ///     <paramref name="tolerance"/> = 0 use <see cref="CreateFromDense(double[])"/>.</param>
        public static SparseVector CreateFromDense(double[] denseArray, double tolerance)
        {
            var indicesValues = new SortedDictionary<int, double>();
            for (int i = 0; i < denseArray.Length; ++i)
            {
                if (Math.Abs(denseArray[i]) > tolerance) indicesValues.Add(i, denseArray[i]);
            }
            return CreateFromDictionary(denseArray.Length, indicesValues);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> with the entries of <paramref name="denseVector"/>. Only the
        /// entries that satisfy: <paramref name="denseVector"/>[i] != 0 are explicitly stored in the new 
        /// <see cref="SparseVector"/>.
        /// </summary>
        /// <param name="denseVector">The original vector that will be converted to <see cref="SparseVector"/>.</param>
        public static SparseVector CreateFromDense(Vector denseVector) => CreateFromDense(denseVector.Elements);

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> with the entries of <paramref name="denseVector"/>. Only the
        /// entries that satisfy: <paramref name="denseVector"/>[i] > <paramref name="tolerance"/> are explicitly stored in the 
        /// new <see cref="SparseVector"/>.
        /// </summary>
        /// <param name="denseVector">The original vector that will be converted to <see cref="SparseVector"/>.</param>
        /// <param name="tolerance">The tolerance under which an entry of <paramref name="denseVector"/> is considered to be 0. 
        ///     Constraints: <paramref name="tolerance"/> &gt; 0. To keep only exact zeros, instead of setting 
        ///     <paramref name="tolerance"/> = 0 use <see cref="CreateFromDense(double[])"/>.</param>
        public static SparseVector CreateFromDense(Vector denseVector, double tolerance)
        {
            return CreateFromDense(denseVector.Elements, tolerance);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> that has the provided <paramref name="length"/> and explicitly
        /// stores only the entries in <paramref name="nonZeroEntries"/>. All otherVector entries are considered as 0. First 
        /// <paramref name="nonZeroEntries"/> will be sorted.
        /// </summary>
        /// <param name="length">The number of zero and non-zero entries in the new <see cref="SparseVector"/>.</param>
        /// <param name="nonZeroEntries">The indices and values of the non-zero entries of the new vector. Constraints:
        ///     (foreach int idx in <paramref name="nonZeroEntries"/>.Keys) 0 &lt;= idx &lt; <paramref name="length"/>.</param>
        public static SparseVector CreateFromDictionary(int length, Dictionary<int, double> nonZeroEntries)
        {
            if (nonZeroEntries.Count == 0) new SparseVector(length, new double[0], new int[0]); // All zero vector

            double[] values = new double[nonZeroEntries.Count];
            int[] indices = new int[nonZeroEntries.Count];
            int nnz = 0;
            foreach (var idxValPair in nonZeroEntries.OrderBy(pair => pair.Key))
            {
                indices[nnz] = idxValPair.Key;
                values[nnz] = idxValPair.Value;
                ++nnz;
            }

            int maxIndex = indices[nnz - 1]; // first sort them
            if (maxIndex >= length)
            {
                throw new ArgumentException($"This sparse vector contains {maxIndex + 1} entries, while its length is {length}");
            }
            return new SparseVector(length, values, indices);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SparseVector"/> that has the provided <paramref name="length"/> and explicitly
        /// stores only the entries in <paramref name="nonZeroEntries"/>. All otherVector entries are considered as 0.
        /// </summary>
        /// <param name="length">The number of zero and non-zero entries in the new <see cref="SparseVector"/>.</param>
        /// <param name="nonZeroEntries">The indices and values of the non-zero entries of the new vector. Constraints:
        ///     (foreach int idx in <paramref name="nonZeroEntries"/>.Keys) 0 &lt;= idx &lt; <paramref name="length"/>.</param>
        public static SparseVector CreateFromDictionary(int length, SortedDictionary<int, double> nonZeroEntries)
        {
            if (nonZeroEntries.Count == 0) return new SparseVector(length, new double[0], new int[0]); // All zero vector

            double[] values = new double[nonZeroEntries.Count];
            int[] indices = new int[nonZeroEntries.Count];
            int nnz = 0;
            foreach (var idxValPair in nonZeroEntries)
            {
                indices[nnz] = idxValPair.Key;
                values[nnz] = idxValPair.Value;
                ++nnz;
            }
            
            int maxIndex = indices[nnz - 1]; // first sort them
            if (maxIndex >= length)
            {
                throw new ArgumentException($"This sparse vector contains {maxIndex + 1} entries, while its length is {length}");
            }
            return new SparseVector(length, values, indices);
        }


















		[Obsolete("Avoid use of this")]
        public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

		[Obsolete("Avoid use of this")]
		public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector);

		[Obsolete("use this[index] += value instead - better not using it at all because it is highly inefficient")]
		public void AddToIndex(int index, double value) => this[index] += value;

		[Obsolete("Avoid use of this")]
		public void AxpySubvectorIntoThis(int destinationIndex, IExtendedImmutableVector sourceVector, double sourceCoefficient, int sourceIndex, int length)
			=> View(destinationIndex, destinationIndex + length).AxpyIntoThis(sourceVector.View(sourceIndex, sourceIndex + length), sourceCoefficient);

		[Obsolete("Avoid use of this because it is highly inefficient")]
		public void CopyNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

		[Obsolete("Avoid use of this because it is highly inefficient")]
		public void CopyNonContiguouslyFrom(IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, otherVector, otherIndices);

		[Obsolete("Avoid use of this")]
		public void CopySubvectorFrom(int destinationIndex, IExtendedImmutableVector sourceVector, int sourceIndex, int length)
			=> View(destinationIndex, destinationIndex + length).CopyFrom(sourceVector.View(sourceIndex, sourceIndex + length));

		/// <summary>
		/// Initializes a new instance of <see cref="Vector"/> that contains the same non-zero entries as this 
		/// <see cref="SparseVector"/>, while the rest entries are explicitly 0.
		/// </summary>
		[Obsolete("use new Vector(this)")]
		public Vector CopyToFullVector() => new Vector(this);

		/// <summary>
		/// Counts how many non zero entries are stored in the vector. This includes zeros that are explicitly stored.
		/// </summary>
		[Obsolete("Use this.ToIndex - this.FromIndex instead")]
        public int CountNonZeros() => ToIndex - FromIndex;

		/// <summary>
		/// Iterates over the non zero entries of the vector. This includes zeros that are explicitly stored.
		/// </summary>
		[Obsolete("use EnumerateStoredElements() instead")]
		public IEnumerable<(int index, double value)> EnumerateNonZeros() => EnumerateStoredElements();

        /// <summary>
        /// See <see cref="IReducible.Reduce(double, ProcessEntry, ProcessZeros, Reduction.Finalize)"/>.
        /// </summary>
        public double Reduce(double identityValue, ProcessEntry processEntry, ProcessZeros processZeros, Finalize finalize)
        {
            for (int i = FromIndex; i < ToIndex; ++i)
				identityValue = processEntry(Values[i], identityValue);
			identityValue = processZeros(Length - (ToIndex - FromIndex), identityValue);
            return finalize(identityValue);
        }

		[Obsolete("use this[index] = value")]
        public void Set(int index, double value) => this[index] = value;

/*		private void CheckMutatedIndex(int index, int sparseIdx)
		{
			if (sparseIdx < 0) throw new SparsityPatternModifiedException(
				$"The entry at index = {index} is zero and not stored explicilty, therefore it cannot be modified.");
		}*/
	}
}
