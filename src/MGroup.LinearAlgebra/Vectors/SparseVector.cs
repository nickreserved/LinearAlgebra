using System;
using System.Collections.Generic;
using System.Linq;
using CSparse.Double.Factorization;

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
        public static SparseVector CreateFromDense(Vector denseVector) => CreateFromDense(denseVector.RawData);

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


















		/// <summary>
		/// See <see cref="IVector.AddIntoThisNonContiguouslyFrom(int[], IVectorView, int[])"/>
		/// </summary>
		[Obsolete("Avoid use of this")]
        public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IMinimalImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

		/// <summary>
		/// See <see cref="IVector.AddIntoThisNonContiguouslyFrom(int[], IVectorView)"/>
		/// </summary>
		[Obsolete("Avoid use of this")]
		public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IMinimalImmutableVector otherVector)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector);

		/// <summary>
		/// See <see cref="IVector.AddToIndex(int, double)"/>.
		/// </summary>
		[Obsolete("use this[index] += value instead")]
		public void AddToIndex(int index, double value) => this[index] += value;

		/// <summary>
		/// See <see cref="IVectorView.Axpy(IVectorView, double)"/>.
		/// </summary>
		public IVector Axpy(IVectorView otherVector, double otherCoefficient)
        {
            Preconditions.CheckVectorDimensions(this, otherVector);
            if (otherVector is SparseVector otherSparse) // In case both matrices have the exact same index arrays
            {
                if (HasSameIndexer(otherSparse))
                {
                    // Do not copy the index arrays, since they are already spread around. TODO: is this a good idea?
                    double[] result = new double[this.values.Length];
                    Array.Copy(this.values, result, this.values.Length);
                    Blas.Daxpy(values.Length, otherCoefficient, otherSparse.values, 0, 1, result, 0, 1);
                    return new SparseVector(Length, result, indices);
                }
            }
            else if (otherVector is Vector otherDense)
            {
                double[] result = otherDense.Scale(otherCoefficient).RawData;
                SparseBlas.Daxpyi(this.indices.Length, 1.0, this.values, this.indices, 0, result, 0);
                return Vector.CreateFromArray(result, false);
            }
            // All entries must be processed. TODO: optimizations may be possible (e.g. only access the nnz in this vector)
            return DenseStrategies.LinearCombination(this, 1.0, otherVector, otherCoefficient);
        }

        /// <summary>
        /// See <see cref="IVector.AxpyIntoThis(IVectorView, double)"/>.
        /// </summary>
        public void AxpyIntoThis(IVectorView otherVector, double otherCoefficient)
        {
            if (otherVector is SparseVector otherSparse) AxpyIntoThis(otherSparse, otherCoefficient);
            else throw new SparsityPatternModifiedException(
                 "This operation is legal only if the otherVector vector has the same sparsity pattern");
        }

        /// <summary>
        /// Performs the following operation for all i:
        /// this[i] = <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i] + this[i]. 
        /// Optimized version of <see cref="IVector.DoEntrywise(IVectorView, Func{double, double, double})"/> and 
        /// <see cref="IVector.LinearCombination(double, IVectorView, double)"/>. Named after BLAS axpy (y = a*x plus y).
        /// The resulting vector overwrites the entries of this.
        /// </summary>
        /// <param name="otherVector">
        /// A vector with the same <see cref="IIndexable1D.Length"/> as this.
        /// </param>
        /// <param name="otherCoefficient">
        /// A scalar that multiplies each entry of <paramref name="otherVector"/>.
        /// </param>
        /// <exception cref="NonMatchingDimensionsException">
        /// Thrown if <paramref name="otherVector"/> has different <see cref="IIndexable1D.Length"/> than this.
        /// </exception>
        /// <exception cref="SparsityPatternModifiedException">
        /// Thrown if an entry this[i] needs to be overwritten, but that is not permitted by the vector storage format.
        /// </exception> 
        public void AxpyIntoThis(SparseVector otherVector, double otherCoefficient)
        {
            Preconditions.CheckVectorDimensions(this, otherVector);
            if (!HasSameIndexer(otherVector)) throw new SparsityPatternModifiedException(
                "This operation is legal only if the otherVector vector has the same sparsity pattern");

            Blas.Daxpy(values.Length, otherCoefficient, otherVector.values, 0, 1, this.values, 0, 1);
        }

        /// <summary>
        /// See <see cref="IVector.AxpySubvectorIntoThis(int, IVectorView, double, int, int)"/>.
        /// </summary>
        public void AxpySubvectorIntoThis(int destinationIndex, IVectorView sourceVector, double sourceCoefficient,
            int sourceIndex, int length)
        {  
            //TODO: needs testing for off-by-1 bugs and extension to cases where source and destination indices are different.

            Preconditions.CheckSubvectorDimensions(this, destinationIndex, length);
            Preconditions.CheckSubvectorDimensions(sourceVector, sourceIndex, length);

            if ( (sourceVector is SparseVector otherSparse) && HasSameIndexer(otherSparse))
            {
                if (destinationIndex != sourceIndex) throw new NotImplementedException();
                int start = Array.FindIndex(this.indices, x => x >= destinationIndex);
                int end = Array.FindIndex(this.indices, x => x >= destinationIndex + length);
                int sparseLength = end - start;
                Blas.Daxpy(sparseLength, sourceCoefficient, otherSparse.values, start, 1, this.values, start, 1);
            }
            throw new SparsityPatternModifiedException(
                "This operation is legal only if the otherVector vector has the same sparsity pattern");
        }


        /// <summary>
        /// See <see cref="IVector.CopyFrom(IVectorView)"/>
        /// </summary>
        public void CopyFrom(IVectorView sourceVector)
        {
            Preconditions.CheckVectorDimensions(this, sourceVector);
            if ((sourceVector is SparseVector otherSparse) && HasSameIndexer(otherSparse))
            {
                Array.Copy(otherSparse.values, this.values, this.Length);
            }
            throw new SparsityPatternModifiedException(
                 "This operation is legal only if the otherVector vector has the same sparsity pattern");
        }

/*		MUCH OF EFFORT FOR NOTHING(?)
 * 
		 /// <summary>
        /// See <see cref="IVector.CopyNonContiguouslyFrom(int[], IVectorView, int[])"/>
        /// </summary>
        public void CopyNonContiguouslyFrom(int[] thisIndices, IVectorView otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

        /// <summary>
        /// See <see cref="IVector.CopyNonContiguouslyFrom(IVectorView, int[])"/>
        /// </summary>
        public void CopyNonContiguouslyFrom(IVectorView otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, otherVector, otherIndices);

        /// <summary>
        /// See <see cref="IVector.CopySubvectorFrom(int, IVectorView, int, int)"/>
        /// </summary>
        public void CopySubvectorFrom(int destinationIndex, IVectorView sourceVector, int sourceIndex, int length)
        {
            //TODO: needs testing for off-by-1 bugs and extension to cases where source and destination indices are different.
            Preconditions.CheckSubvectorDimensions(this, destinationIndex, length);
            Preconditions.CheckSubvectorDimensions(sourceVector, sourceIndex, length);

            if ((sourceVector is SparseVector otherSparse) && HasSameIndexer(otherSparse))
            {
                if (destinationIndex != sourceIndex) throw new NotImplementedException();
                int start = Array.FindIndex(this.indices, x => x >= destinationIndex);
                int end = Array.FindIndex(this.indices, x => x >= destinationIndex + length);
                int sparseLength = end - start;
                Array.Copy(otherSparse.values, start, this.values, start, sparseLength);
            }
            throw new SparsityPatternModifiedException(
                "This operation is legal only if the otherVector vector has the same sparsity pattern");
        }
*/


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
        /// See <see cref="IEntrywiseOperableView1D{TVectorIn, TVectorOut}.DoEntrywise(TVectorIn, Func{double, double, double})"/>.
        /// </summary>
        public IVector DoEntrywise(IVectorView otherVector, Func<double, double, double> binaryOperation)
        {
            Preconditions.CheckVectorDimensions(this, otherVector);
            if (otherVector is SparseVector otherSparse) // In case both matrices have the exact same index arrays
            {
                if (HasSameIndexer(otherSparse))
                {
                    // Do not copy the index arrays, since they are already spread around. TODO: is this a good idea?
                    double[] resultValues = new double[values.Length];
                    for (int i = 0; i < values.Length; ++i)
                    {
                        resultValues[i] = binaryOperation(this.values[i], otherSparse.values[i]);
                    }
                    return new SparseVector(Length, resultValues, indices);
                }
            }

            // All entries must be processed. TODO: optimizations may be possible (e.g. only access the nnz in this vector)
            return DenseStrategies.DoEntrywise(this, otherVector, binaryOperation);
        }

        /// <summary>
        /// See <see cref="IEntrywiseOperable1D{TVectorIn}.DoEntrywiseIntoThis(TVectorIn, Func{double, double, double})"/>
        /// </summary>
        public void DoEntrywiseIntoThis(IVectorView otherVector, Func<double, double, double> binaryOperation)
        {
            Preconditions.CheckVectorDimensions(this, otherVector);
            if ((otherVector is SparseVector otherSparse) && HasSameIndexer(otherSparse))
            {
                for (int i = 0; i < values.Length; ++i)
                {
                    this.values[i] = binaryOperation(this.values[i], otherSparse.values[i]);
                }
            }
            throw new SparsityPatternModifiedException(
                 "This operation is legal only if the otherVector vector has the same sparsity pattern");
        }

        /// <summary>
        /// See <see cref="IEntrywiseOperableView1D{TVectorIn, TVectorOut}.DoToAllEntries(Func{double, double})"/>.
        /// </summary>
        public IVector DoToAllEntries(Func<double, double> unaryOperation)
        {
            // Only apply the operation on non zero entries
            double[] newValues = new double[values.Length];
            for (int i = 0; i < values.Length; ++i) newValues[i] = unaryOperation(values[i]);

            if (new ValueComparer(1e-10).AreEqual(unaryOperation(0.0), 0.0)) // The same sparsity pattern can be used.
            {
                // Copy the index arrays. TODO: See if we can use the same index arrays (e.g. if this class does not change them (it shouldn't))
                int[] indicesCopy = new int[indices.Length];
                Array.Copy(indices, indicesCopy, indices.Length);
                return new SparseVector(Length, newValues, indicesCopy);
            }
            else // The sparsity is destroyed. Revert to a full vector.
            {
                return new SparseVector(Length, newValues, indices).CopyToFullVector();
            }
        }

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
            double aggregator = identityValue;
            int nnz = values.Length;
            for (int i = 0; i < nnz; ++i) aggregator = processEntry(values[i], aggregator);
            aggregator = processZeros(Length - nnz, aggregator);
            return finalize(aggregator);
        }

		[Obsolete("use this[index] = value")]
        public void Set(int index, double value) => this[index] = value;


		private void CheckMutatedIndex(int index, int sparseIdx)
		{
			if (sparseIdx < 0) throw new SparsityPatternModifiedException(
				$"The entry at index = {index} is zero and not stored explicilty, therefore it cannot be modified.");
		}
	}
}
