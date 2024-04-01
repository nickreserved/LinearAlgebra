namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Collections.Generic;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;

	using static MGroup.LinearAlgebra.LibrarySettings;

	abstract public class AbstractSparseVector : IExtendedMutableVector
	{
		/// <summary>
		/// Indices of non-zero elements of vector in an ascending order. It is corresponding to <see cref="Values"/>
		/// </summary>
		public int[] Indices { get; set; }

		/// <summary>
		/// Values of Non-zero only elements, corresponding to <see cref="Indices"/>.
		/// </summary>
		public double[] Values { get; set; }

		/// <summary>
		/// From what index of <see cref="Indices"/> (and <see cref="Values"/>) starts the first non-zero element of sparse vector.
		/// </summary>
		/// This property is useful for row or column (sparse vector) views for CSR and CSC matrices.
		abstract public int FromIndex { get; }
		/// <summary>
		/// To what index (excluded) of <see cref="Indices"/> (and <see cref="Values"/>) ends the last non-zero element of sparse vector.
		/// </summary>
		/// This property is useful for row or column (sparse vector) views for CSR and CSC matrices.
		abstract public int ToIndex { get; }

		/// <summary>
		/// Total number of elements of sparse vector, either zero or non-zero.
		/// </summary>
		public int Length { get; protected set; }

		/// <summary>
		/// Finds the first non-zero element's index which is greater or equal to <paramref name="value"/>
		/// </summary>
		/// <param name="Indices">The array of indices of non-zero elements of matrix</param>
		/// <param name="fromIndex">The index in array <paramref name="Indices"/> from where search starts (including)</param>
		/// <param name="toIndex">The index in array <paramref name="Indices"/> where search ends (excluding)</param>
		/// <param name="value">The index which we search in array <paramref name="Indices"/>.
		/// While <paramref name="fromIndex"/> and <paramref name="toIndex"/> are indices to array <paramref name="Indices"/>,
		/// the <paramref name="value"/> is the index of a non-zero element in sparse vector</param>.
		/// This method searches complete <parameref name="Indices"/> and not the bounded of the current sparse vector with <see cref="FromIndex"/> and <see cref="Length"/>.
		/// <returns>The index to array <paramref name="Indices"/>, pointing to an index to a non-zero element of sparse vector,
		/// which index is greater or equal to <paramref name="value"/></returns>
		static private int GetLowerBound(int[] Indices, int fromIndex, int toIndex, int value)
		{
			int result = Array.BinarySearch(Indices, fromIndex, toIndex - fromIndex, value);
			if (result < 0) result = -(result + 1);
			return result;
		}

		/// <summary>
		/// Finds the first non-zero element's index which is greater to <paramref name="value"/>
		/// </summary>
		/// <param name="Indices">The array of indices of non-zero elements of matrix</param>
		/// <param name="fromIndex">The index in array <paramref name="Indices"/> from where search starts (including)</param>
		/// <param name="toIndex">The index in array <paramref name="Indices"/> where search ends (excluding)</param>
		/// <param name="value">The index which we search in array <paramref name="Indices"/>.
		/// While <paramref name="fromIndex"/> and <paramref name="toIndex"/> are indices to array <paramref name="Indices"/>,
		/// the <paramref name="value"/> is the index of a non-zero element in sparse vector</param>
		/// This method searches complete <parameref name="Indices"/> and not the bounded of the current sparse vector with <see cref="FromIndex"/> and <see cref="Length"/>.
		/// <returns>The index to array <paramref name="Indices"/>, pointing to an index to a non-zero element of sparse vector,
		/// which index is greater to <paramref name="value"/></returns>
		static private int GetUpperBound(int[] Indices, int fromIndex, int toIndex, int value)
		{
			int result = Array.BinarySearch(Indices, fromIndex, toIndex - fromIndex, value);
			if (result < 0) result = -(result + 1);
			else ++result;
			return result;
		}

		/// <summary>
		/// Finds the 2 indices of array <see cref="Indices"/> which form the range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>) in sparse vector.
		/// </summary>
		/// <param name="fromIndex">The index of vector element (not the index to array <see cref="Indices"/>) which is the beginning of bounds</param>
		/// <param name="toIndex">The index  of vector element (not the index to array <see cref="Indices"/>) which is the end of bounds</param>
		/// <returns>First value is greater or equal to <paramref name="fromIndex"/>. Second value is greater or equal to <paramref name="toIndex"/>.
		/// Both values form a range in array <see cref="Indices"/> [indicesFromIndex, indicesToIndex)
		/// which corresponds to a sparse vector range [<paramref name="fromIndex"/>, <paramref name="toIndex"/>).</returns>
		protected (int, int) Bounds(int fromIndex, int toIndex)
		{
			int indicesFromIndex = GetLowerBound(Indices, FromIndex, ToIndex, fromIndex);
			int indicesToIndex = GetLowerBound(Indices, indicesFromIndex, ToIndex, toIndex);
			return (indicesFromIndex, indicesToIndex);
		}

		/// <summary>
		/// Shift indices of non-zero elements by <paramref name="offsetIndex"/>
		/// </summary>
		/// Caller must avoid to shift indices to negative indices. This function doesn't check that.
		/// <param name="offsetIndex">The shift for all indices of non-zero elements. Usually this value is negative.</param>
		/// <returns>This vector</returns>
		virtual public AbstractSparseVector ShiftIntoThis(int offsetIndex)
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				Indices[i] += offsetIndex;
			return this;
		}

		public bool HasSameIndexer(AbstractSparseVector otherVector) => Indices == otherVector.Indices && FromIndex == otherVector.FromIndex && ToIndex == otherVector.ToIndex;

		/// <summary>
		/// Iterates over the non zero entries of the vector. This includes zeros that are explicitly stored.
		/// </summary>
		public IEnumerable<(int index, double value)> EnumerateStoredElements()
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				yield return (Indices[i], Values[i]);
		}

		/// <summary>
		/// Gets or sets sparse vector elements. This is a very inefficient way to do this. Try to avoid.
		/// </summary>
		/// <param name="index">Index of vector element</param>
		/// <returns>Value of vector element</returns>
		/// <exception cref="IndexOutOfRangeException">If you try to set an element not existed in sparse vector</exception>
		[Obsolete("Intention of this property, is for sparse vectors and it is highly inefficient. Please stop use it RIGHT NOW")]
		public double this[int index]
		{
			get
			{
				index = Array.BinarySearch(Indices, FromIndex, ToIndex - FromIndex, index);
				return index < 0 ? 0 : Values[index];
			}
			set
			{
				index = Array.BinarySearch(Indices, FromIndex, ToIndex - FromIndex, index);
				if (index >= 0) Values[index] = value;
				else throw new IndexOutOfRangeException("Element with that index is not stored in sparse vector");
			}
		}



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector

		virtual public SparseVector CopyUnshifted(int fromIndex, int toIndex)
		{
			var (indicesFromIndex, indicesToIndex) = Bounds(fromIndex, toIndex);
			var indices = new int[indicesToIndex - indicesFromIndex];
			var values = new double[indices.Length];
			Array.Copy(Indices, indicesFromIndex, indices, 0, indices.Length);
			Array.Copy(Values, indicesFromIndex, values, 0, indices.Length);
			return new SparseVector(toIndex - fromIndex, values, indices);
		}

		virtual public SparseVector Copy(int fromIndex, int toIndex) => (SparseVector)CopyUnshifted(fromIndex, toIndex).ShiftIntoThis(-fromIndex);
		IExtendedMutableVector IExtendedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex);

		virtual public SparseVector Axpy(AbstractSparseVector otherVector, double otherCoefficient)
		{
			if (HasSameIndexer(otherVector))
			{
				Preconditions.CheckVectorDimensions(this, otherVector);
				SparseVector result = Copy();
				Blas.Daxpy(result.ToIndex - result.FromIndex, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1, result.Values, result.FromIndex, 1);
				return result;
			}
			else return DoEntrywise(otherVector, (double x, double y) => x + y * otherCoefficient);
		}
		virtual public Vector Axpy(AbstractFullyPopulatedVector otherVector, double otherCoefficient) => (Vector)otherVector.Scale(otherCoefficient).AddIntoThis(this);
		virtual public IExtendedMutableVector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Axpy(sparseVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Axpy(fullVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}

		virtual public SparseVector Add(AbstractSparseVector otherVector) => DoEntrywise(otherVector, (double x, double y) => x + y);
		virtual public Vector Add(AbstractFullyPopulatedVector otherVector) => (Vector)otherVector.Add(this);
		virtual public IExtendedMutableVector Add(IMinimalImmutableVector otherVector)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Add(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Add(fullVector);
			throw new NotImplementedException("Add(NotSupportedVector)");
		}

		virtual public SparseVector Subtract(AbstractSparseVector otherVector) => DoEntrywise(otherVector, (double x, double y) => x - y);
		virtual public Vector Subtract(AbstractFullyPopulatedVector otherVector) => (Vector)otherVector.Subtract(this);
		virtual public IExtendedMutableVector Subtract(IMinimalImmutableVector otherVector)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Subtract(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Subtract(fullVector);
			throw new NotImplementedException("Subtract(NotSupportedVector)");
		}

		virtual public SparseVector Negative() => (SparseVector)IMinimalImmutableVector.Negative(this);
		IExtendedMutableVector IExtendedImmutableVector.Negative() => Negative();

		virtual public SparseVector Scale(double coefficient) => (SparseVector)IMinimalImmutableVector.Scale(this, coefficient);
		IExtendedMutableVector IExtendedImmutableVector.Scale(double coefficient) => Scale(coefficient);

		virtual public SparseVector LinearCombination(double thisCoefficient, AbstractSparseVector otherVector, double otherCoefficient)
		{
			if (HasSameIndexer(otherVector))
			{
				Preconditions.CheckVectorDimensions(this, otherVector);
				SparseVector result = Copy();
				BlasExtensions.Daxpby(result.ToIndex - result.FromIndex, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1,
																			thisCoefficient, result.Values, result.FromIndex, 1);
				return result;
			}
			return DoEntrywise(otherVector, (double x, double y) => x * thisCoefficient + y * otherCoefficient);
		}
		virtual public Vector LinearCombination(double thisCoefficient, AbstractFullyPopulatedVector otherVector, double otherCoefficient) => (Vector)otherVector.LinearCombination(thisCoefficient, this, otherCoefficient);
		virtual public IExtendedMutableVector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			if (otherVector is AbstractSparseVector sparseVector) return LinearCombination(thisCoefficient, sparseVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return LinearCombination(thisCoefficient, fullVector, otherCoefficient);
			throw new NotImplementedException("LinearCombination(thisCoefficient, NotSupportedVector, otherCoefficient)");
		}

		virtual public SparseVector DoEntrywise(AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
		{
			var indices = new int[ToIndex - FromIndex + otherVector.ToIndex - otherVector.FromIndex];	// shared indices can lead to fewer elements
			var values = new double[indices.Length];
			int outPos = 0, thisPos = FromIndex, otherPos = otherVector.FromIndex;
			for (; thisPos < ToIndex && otherPos < otherVector.ToIndex ;)
			{
				if (Indices[thisPos] == otherVector.Indices[otherPos])
				{
					indices[outPos] = Indices[thisPos];
					values[outPos] = binaryOperation(Values[thisPos], otherVector.Values[otherPos]);
					++thisPos; ++otherPos;
				}
				else if (Indices[thisPos] < otherVector.Indices[otherPos])
				{
					indices[outPos] = Indices[thisPos];
					values[outPos] = binaryOperation(Values[thisPos], 0);
					++thisPos;
				}
				else //  if (Indices[thisPos] > otherVector.Indices[otherPos])
				{
					indices[outPos] = otherVector.Indices[otherPos];
					values[outPos] = binaryOperation(0, otherVector.Values[otherPos]);
					++otherPos;
				}
				if (values[outPos] != 0)
					++outPos;
			}
			// remain elements from this vector OR...
			for (; thisPos < ToIndex; ++thisPos)
			{
				indices[outPos] = Indices[thisPos];
				values[outPos] = binaryOperation(Values[thisPos], 0);
				if (values[outPos] != 0)
					++outPos;
			}
			// ...OR remain elements from other vector
			for (; otherPos < otherVector.ToIndex; ++otherPos)
			{
				indices[outPos] = otherVector.Indices[otherPos];
				values[outPos] = binaryOperation(0, otherVector.Values[otherPos]);
				if (values[outPos] != 0)
					++outPos;
			}
			// reallocation
			if (outPos < indices.Length)
			{
				var indices2 = new int[outPos];
				var values2 = new double[outPos];
				Array.Copy(indices, indices2, outPos);
				Array.Copy(values, values2, outPos);
				indices = indices2;
				values = values2;
			}
			return new SparseVector(Length, values, indices);
		}
		virtual public Vector DoEntrywise(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (Vector)IMinimalImmutableVector.DoEntrywise(this, otherVector, binaryOperation);
		virtual public IExtendedMutableVector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			if (otherVector is AbstractSparseVector sparseVector) return DoEntrywise(sparseVector, binaryOperation);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return DoEntrywise(fullVector, binaryOperation);
			throw new NotImplementedException("DoEntrywise(thisCoefficient, NotSupportedVector, otherCoefficient)");
		}

		virtual public SparseVector DoToAllEntries(Func<double, double> unaryOperation) => (SparseVector)IMinimalImmutableVector.DoToAllEntries(this, unaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);

		virtual public double[] CopyToArray()
		{
			var result = new double[Length];
			for (int i = FromIndex; i < ToIndex; ++i)
				result[Indices[i]] = Values[i];
			return result;
		}

		virtual public double[] CopyToArray(int fromIndex, int toIndex) => IExtendedImmutableVector.CopyToArray(this, fromIndex, toIndex);

		virtual public void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length)
		{
			var (indicesFromIndex, indicesToIndex) = Bounds(fromIndex, fromIndex + length);
			for (int i = indicesFromIndex; i < indicesToIndex; ++i)
				array[arrayIndex + Indices[i] - fromIndex] = Values[i];
		}

		virtual public SparseVector Copy()
		{
			int indicesToIndex = GetLowerBound(Indices, FromIndex, Indices.Length, Length);
			var indices = new int[indicesToIndex - FromIndex];
			var values = new double[indices.Length];
			Array.Copy(Indices, FromIndex, indices, 0, indices.Length);
			Array.Copy(Values, FromIndex, values, 0, indices.Length);
			return new SparseVector(Length, values, indices);
		}
		IExtendedMutableVector IExtendedImmutableVector.Copy() => Copy();

		virtual public SparseVector CreateZero() => new SparseVector(Length, new double[0], new int[0]);
		IExtendedMutableVector IExtendedImmutableVector.CreateZero() => CreateZero();



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedMutableVector
		
		public AbstractSparseVector View(int fromIndex, int toIndex)
		{
			if (toIndex > Length) toIndex = Length;
			var (indicesFromIndex, indicesToIndex) = Bounds(fromIndex, toIndex);
			// Indices and Values go together but because of view we don't touch Values
			// So (unfortunatelly) we need a bigger array of Indices to match Values
			var indices = new int[indicesToIndex];
			// There is no need to complete populate it
			for (int i = indicesFromIndex; i < indicesToIndex; ++i)
				indices[i] = Indices[i] - fromIndex;
			return new SparseVectorView(toIndex - fromIndex, Values, indices, indicesFromIndex, indicesToIndex);
		}
		IExtendedMutableVector IExtendedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);

		virtual public AbstractSparseVector AxpyIntoThis(AbstractSparseVector otherVector, double otherCoefficient)
		{
			if (otherCoefficient != 0)
			{
				if (HasSameIndexer(otherVector))
				{
					Preconditions.CheckVectorDimensions(this, otherVector);
					Blas.Daxpy(ToIndex - FromIndex, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1, Values, FromIndex, 1);
				}
				else DoEntrywiseIntoThis(otherVector, (double x, double y) => x + y * otherCoefficient);
			}
			return this;
		}
		virtual public AbstractSparseVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return AxpyIntoThis(sparseVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}
		IExtendedMutableVector IExtendedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient);

		virtual public AbstractSparseVector AddIntoThis(SparseVector otherVector) => DoEntrywiseIntoThis(otherVector, (double x, double y) => x + y);
		virtual public AbstractSparseVector AddIntoThis(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return AddIntoThis(sparseVector);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}
		IExtendedMutableVector IExtendedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector);

		virtual public AbstractSparseVector SubtractIntoThis(SparseVector otherVector) => DoEntrywiseIntoThis(otherVector, (double x, double y) => x - y);
		virtual public AbstractSparseVector SubtractIntoThis(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return SubtractIntoThis(sparseVector);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}
		IExtendedMutableVector IExtendedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector);

		virtual public AbstractSparseVector NegativeIntoThis()
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				Values[i] = -Values[i];
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.NegativeIntoThis() => NegativeIntoThis();

		virtual public AbstractSparseVector ScaleIntoThis(double coefficient)
		{
			Blas.Dscal(ToIndex - FromIndex, coefficient, Values, FromIndex, 1);
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);

		virtual public AbstractSparseVector LinearCombinationIntoThis(double thisCoefficient, AbstractSparseVector otherVector, double otherCoefficient)
		{
			if (HasSameIndexer(otherVector))
			{
				Preconditions.CheckVectorDimensions(this, otherVector);
				BlasExtensions.Daxpby(ToIndex - FromIndex, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1,
																thisCoefficient, Values, FromIndex, 1);
			}
			else IMinimalMutableVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
			return this;
		}
		virtual public AbstractSparseVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return LinearCombinationIntoThis(thisCoefficient, sparseVector, otherCoefficient);
			else return (AbstractSparseVector)IMinimalMutableVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
		}
		IExtendedMutableVector IExtendedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		virtual public AbstractSparseVector CopyFrom(AbstractSparseVector otherVector) => DoEntrywiseIntoThis(otherVector, (double x, double y) => y);
		virtual public AbstractSparseVector CopyFrom(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return CopyFrom(sparseVector);
			throw new NotImplementedException("CopyFrom(NotSupportedVector)");
		}
		IExtendedMutableVector IExtendedMutableVector.CopyFrom(IMinimalImmutableVector otherVector) => CopyFrom(otherVector);

		virtual public AbstractSparseVector Clear()
		{
			Array.Clear(Values, FromIndex, ToIndex - FromIndex);
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.Clear() => Clear();

		virtual public AbstractSparseVector SetAll(double value)
		{
			Array.Fill(Values, value, FromIndex, ToIndex - FromIndex);
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.SetAll(double value) => SetAll(value);

		virtual public AbstractSparseVector DoEntrywiseIntoThis(AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			int i = FromIndex, j = otherVector.FromIndex;
			for (; i < ToIndex && j < otherVector.ToIndex;)
			{
				if (Indices[i] < otherVector.Indices[j])
				{
					Values[i] = binaryOperation(Values[i], 0);
					++i;
				}
				else if (Indices[i] == otherVector.Indices[j])
				{
					Values[i] = binaryOperation(Values[i], otherVector.Values[j]);
					++i; ++j;
				}
				else /*if (Indices[i] > otherVector.Indices[j])*/
				{
					if (binaryOperation(0, otherVector.Values[j]) == 0) ++j;
					else throw new SparsityPatternModifiedException("This operation will change the sparsity pattern");
				}
			}
			for (; i < ToIndex; ++i)
				Values[i] = binaryOperation(Values[i], 0);
			for (; j < otherVector.ToIndex; ++j)
				if (binaryOperation(0, otherVector.Values[j]) != 0)
					throw new SparsityPatternModifiedException("This operation will change the sparsity pattern");
			return this;
		}
		virtual public AbstractSparseVector DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return DoEntrywiseIntoThis(sparseVector, binaryOperation);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}
		IExtendedMutableVector IExtendedMutableVector.DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywiseIntoThis(otherVector, binaryOperation);

		virtual public AbstractSparseVector DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				Values[i] = unaryOperation(Values[i]);
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => DoToAllEntriesIntoThis(unaryOperation);



		// ------------------- COVARIANT RETURN TYPE FROM IMinimalImmutableVector

		virtual public double DotProduct(AbstractSparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			if (HasSameIndexer(otherVector))
				return Blas.Ddot(ToIndex - FromIndex, Values, FromIndex, 1, otherVector.Values, otherVector.FromIndex, 1);
			else
			{
				double result = 0;
				for (int i = FromIndex, j = otherVector.FromIndex; i < ToIndex && j < otherVector.ToIndex;)
					if (Indices[i] == otherVector.Indices[j])
					{
						result += Values[i] * otherVector.Values[j];
						++i; ++j;
					}
					else if (Indices[i] < otherVector.Indices[j]) ++i;
					else /*if (Indices[i] > otherVector.Indices[j])*/ ++j;
				return result;
			}
		}
		virtual public double DotProduct(AbstractFullyPopulatedVector otherVector) => otherVector.DotProduct(this);
		virtual public double DotProduct(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DotProduct(fullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}

		virtual public bool IsZero(double tolerance = 1e-7)
		{
			if (tolerance != 0)
			{
				for (int i = FromIndex; i < ToIndex; ++i)
					if (Values[i] != 0) return false;
			}
			else
			{
				for (int i = FromIndex; i < ToIndex; ++i)
					if (Math.Abs(Values[i]) > tolerance) return false;
			}
			return true;
		}

		virtual public double Square() => IMinimalImmutableVector.Square(this);

		virtual public double Norm2() => Blas.Dnrm2(ToIndex - FromIndex, Values, FromIndex, 1);

		virtual public bool Equals(AbstractSparseVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			int thisPos = FromIndex, otherPos = otherVector.FromIndex;
			for (; thisPos < ToIndex && otherPos < otherVector.ToIndex;)
			{
				if (Indices[thisPos] == otherVector.Indices[otherPos])
				{
					if (!cmp.AreEqual(Values[thisPos], otherVector.Values[otherPos])) return false;
					++thisPos; ++otherPos;
				}
				else if (Indices[thisPos] < otherVector.Indices[otherPos])
				{
					if(Math.Abs(Values[thisPos]) > tolerance) return false;
					++thisPos;
				}
				else //  if (Indices[thisPos] > otherVector.Indices[otherPos])
				{
					if (Math.Abs(otherVector.Values[otherPos]) > tolerance) return false;
					++otherPos;
				}
			}
			// remain elements from this vector OR...
			for (; thisPos < ToIndex; ++thisPos)
				if (Math.Abs(Values[thisPos]) > tolerance) return false;
			// ...OR remain elements from other vector
			for (; otherPos < otherVector.ToIndex; ++otherPos)
				if (Math.Abs(otherVector.Values[otherPos]) > tolerance) return false;
			return true;
		}
		virtual public bool Equals(AbstractFullyPopulatedVector otherVector, double tolerance = 1E-07) => otherVector.Equals(this, tolerance);
		virtual public bool Equals(IMinimalImmutableVector otherVector, double tolerance = 1E-07)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return Equals(sparseVector, tolerance);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return Equals(fullyPopulatedVector, tolerance);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}
	}
}
