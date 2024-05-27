namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Collections.Generic;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;
	using MGroup.LinearAlgebra.Reduction;

	using static MGroup.LinearAlgebra.LibrarySettings;

	public abstract class AbstractSparseVector : IExtendedVector
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
		public abstract int FromIndex { get; }
		/// <summary>
		/// To what index (excluded) of <see cref="Indices"/> (and <see cref="Values"/>) ends the last non-zero element of sparse vector.
		/// </summary>
		/// This property is useful for row or column (sparse vector) views for CSR and CSC matrices.
		public abstract int ToIndex { get; }

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
		private static int GetLowerBound(int[] Indices, int fromIndex, int toIndex, int value)
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
		private static int GetUpperBound(int[] Indices, int fromIndex, int toIndex, int value)
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

		public bool HasSameIndexer(AbstractSparseVector otherVector) => Indices == otherVector.Indices && FromIndex == otherVector.FromIndex && ToIndex == otherVector.ToIndex;

		/// <summary>
		/// Iterates over the non zero entries of the vector. This includes zeros that are explicitly stored.
		/// </summary>
		public IEnumerable<(int index, double value)> EnumerateStoredEntries()
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
		[Obsolete("This property is EXTREMELY inefficient")]
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



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedReadOnlyVector

		public virtual SparseVector Copy(int fromIndex, int toIndex)
		{
			var (indicesFromIndex, indicesToIndex) = Bounds(fromIndex, toIndex);
			var indices = new int[indicesToIndex - indicesFromIndex];
			var values = new double[indices.Length];
			for (int i = 0; i < indices.Length; ++i)
				indices[i] = Indices[i + indicesFromIndex] - fromIndex;
			Array.Copy(Values, indicesFromIndex, values, 0, indices.Length);
			return new SparseVector(toIndex - fromIndex, values, indices);
		}
		IExtendedVector IExtendedReadOnlyVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex);

		public virtual SparseVector Axpy(AbstractSparseVector otherVector, double otherCoefficient)
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
		public virtual Vector Axpy(AbstractFullyPopulatedVector otherVector, double otherCoefficient)
		{
			var result = otherVector.Scale(otherCoefficient);
			result.AddIntoThis(this);
			return result;
		}
		public virtual IExtendedVector Axpy(IReadOnlyVector otherVector, double otherCoefficient)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Axpy(sparseVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Axpy(fullVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}

		public virtual SparseVector Add(AbstractSparseVector otherVector) => Axpy(otherVector, 1);
		public virtual Vector Add(AbstractFullyPopulatedVector otherVector) => (Vector)otherVector.Add(this);
		public virtual IExtendedVector Add(IReadOnlyVector otherVector)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Add(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Add(fullVector);
			throw new NotImplementedException("Add(NotSupportedVector)");
		}

		public virtual SparseVector Subtract(AbstractSparseVector otherVector) => Axpy(otherVector, -1);
		public virtual Vector Subtract(AbstractFullyPopulatedVector otherVector) => (Vector)otherVector.Subtract(this);
		public virtual IExtendedVector Subtract(IReadOnlyVector otherVector)
		{
			if (otherVector is AbstractSparseVector sparseVector) return Subtract(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return Subtract(fullVector);
			throw new NotImplementedException("Subtract(NotSupportedVector)");
		}

		public virtual SparseVector Negate() => (SparseVector)IReadOnlyVector.Negate(this);
		IExtendedVector IExtendedReadOnlyVector.Negate() => Negate();

		public virtual SparseVector Scale(double coefficient) => (SparseVector)IReadOnlyVector.Scale(this, coefficient);
		IExtendedVector IExtendedReadOnlyVector.Scale(double coefficient) => Scale(coefficient);

		public virtual SparseVector LinearCombination(double thisCoefficient, AbstractSparseVector otherVector, double otherCoefficient)
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
		public virtual Vector LinearCombination(double thisCoefficient, AbstractFullyPopulatedVector otherVector, double otherCoefficient) => (Vector)otherVector.LinearCombination(thisCoefficient, this, otherCoefficient);
		public virtual IExtendedVector LinearCombination(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient)
		{
			if (otherVector is AbstractSparseVector sparseVector) return LinearCombination(thisCoefficient, sparseVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return LinearCombination(thisCoefficient, fullVector, otherCoefficient);
			throw new NotImplementedException("LinearCombination(thisCoefficient, NotSupportedVector, otherCoefficient)");
		}

		public virtual SparseVector DoEntrywise(AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
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
		public virtual Vector DoEntrywise(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (Vector)IReadOnlyVector.DoEntrywise(this, otherVector, binaryOperation);
		public virtual IExtendedVector DoEntrywise(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation)
		{
			if (otherVector is AbstractSparseVector sparseVector) return DoEntrywise(sparseVector, binaryOperation);
			if (otherVector is AbstractFullyPopulatedVector fullVector) return DoEntrywise(fullVector, binaryOperation);
			throw new NotImplementedException("DoEntrywise(thisCoefficient, NotSupportedVector, otherCoefficient)");
		}

		public virtual SparseVector DoToAllEntries(Func<double, double> unaryOperation) => (SparseVector)IReadOnlyVector.DoToAllEntries(this, unaryOperation);
		IExtendedVector IExtendedReadOnlyVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);

		public virtual double[] CopyToArray()
		{
			var result = new double[Length];
			for (int i = FromIndex; i < ToIndex; ++i)
				result[Indices[i]] = Values[i];
			return result;
		}

		public virtual double[] CopyToArray(int fromIndex, int toIndex) => IExtendedReadOnlyVector.CopyToArray(this, fromIndex, toIndex);

		public virtual void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length)
		{
			var (indicesFromIndex, indicesToIndex) = Bounds(fromIndex, fromIndex + length);
			for (int i = indicesFromIndex; i < indicesToIndex; ++i)
				array[arrayIndex + Indices[i] - fromIndex] = Values[i];
		}

		public virtual SparseVector Copy()
		{
			int indicesToIndex = GetLowerBound(Indices, FromIndex, Indices.Length, Length);
			var indices = new int[indicesToIndex - FromIndex];
			var values = new double[indices.Length];
			Array.Copy(Indices, FromIndex, indices, 0, indices.Length);
			Array.Copy(Values, FromIndex, values, 0, indices.Length);
			return new SparseVector(Length, values, indices);
		}
		IExtendedVector IExtendedReadOnlyVector.Copy() => Copy();

		public virtual SparseVector CreateZeroWithSameFormat()
		{
			var indices = new int[ToIndex - FromIndex];
			Array.Copy(Indices, FromIndex, indices, 0, indices.Length);
			return new SparseVector(Length, new double[indices.Length], indices);
		}
		IExtendedVector IExtendedReadOnlyVector.CreateZeroWithSameFormat() => CreateZeroWithSameFormat();



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedVector
		
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
		IExtendedVector IExtendedVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IVector
		
		public virtual AbstractSparseVector AxpyIntoThis(AbstractSparseVector otherVector, double otherCoefficient)
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
		public virtual void AxpyIntoThis(IReadOnlyVector otherVector, double otherCoefficient) => AxpyIntoThis((AbstractSparseVector)otherVector, otherCoefficient);

		public virtual AbstractSparseVector AddIntoThis(SparseVector otherVector) => AxpyIntoThis(otherVector, 1);
		public virtual void AddIntoThis(IReadOnlyVector otherVector) => AddIntoThis((AbstractSparseVector)otherVector);

		public virtual AbstractSparseVector SubtractIntoThis(SparseVector otherVector) => AxpyIntoThis(otherVector, -1);
		public virtual void SubtractIntoThis(IReadOnlyVector otherVector) => SubtractIntoThis((AbstractSparseVector)otherVector);

		public virtual void NegateIntoThis() => ScaleIntoThis(-1);

		public virtual void ScaleIntoThis(double coefficient) => Blas.Dscal(ToIndex - FromIndex, coefficient, Values, FromIndex, 1);

		public virtual AbstractSparseVector LinearCombinationIntoThis(double thisCoefficient, AbstractSparseVector otherVector, double otherCoefficient)
		{
			if (HasSameIndexer(otherVector))
			{
				Preconditions.CheckVectorDimensions(this, otherVector);
				BlasExtensions.Daxpby(ToIndex - FromIndex, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1,
																thisCoefficient, Values, FromIndex, 1);
			}
			else IVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
			return this;
		}
		public virtual void LinearCombinationIntoThis(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient)
			=> LinearCombinationIntoThis(thisCoefficient, (AbstractSparseVector)otherVector, otherCoefficient);

		public virtual AbstractSparseVector CopyFrom(AbstractSparseVector otherVector) => DoEntrywiseIntoThis(otherVector, (double x, double y) => y);
		public virtual void CopyFrom(IReadOnlyVector otherVector) => CopyFrom((AbstractSparseVector)otherVector);

		public virtual void Clear() => Array.Clear(Values, FromIndex, ToIndex - FromIndex);

		public virtual void SetAll(double value) => Array.Fill(Values, value, FromIndex, ToIndex - FromIndex);

		public virtual AbstractSparseVector DoEntrywiseIntoThis(AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
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
		public virtual void DoEntrywiseIntoThis(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation)
			=> DoEntrywiseIntoThis((AbstractSparseVector) otherVector, binaryOperation);

		public virtual void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				Values[i] = unaryOperation(Values[i]);
		}



		// ------------------- COVARIANT RETURN TYPE FROM IReadOnlyVector

		public virtual double DotProduct(AbstractSparseVector otherVector)
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
		public virtual double DotProduct(AbstractContiguousFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			return SparseBlas.Ddoti(ToIndex - FromIndex, Values, Indices, FromIndex, otherVector.Values, otherVector.FromIndex);
		}
		public virtual double DotProduct(AbstractFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			double result = 0;
			for (int i = FromIndex; i < ToIndex; ++i)
				result += otherVector[Indices[i]] * Values[i];
			return result;
		}
		public virtual double DotProduct(IReadOnlyVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) return DotProduct(contiguousVector);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DotProduct(fullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}

		public virtual bool IsZero(double tolerance = 1e-7)
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

		public virtual double Square() => IReadOnlyVector.Square(this);

		public virtual double Norm2() => Blas.Dnrm2(ToIndex - FromIndex, Values, FromIndex, 1);

		public virtual bool Equals(AbstractSparseVector otherVector, double tolerance = 1E-07)
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
		public virtual bool Equals(AbstractFullyPopulatedVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			for (int i = 0, j = FromIndex; i < otherVector.Length; ++i)
				if (j >= ToIndex || i < Indices[j])
				{
					if (Math.Abs(otherVector[i]) > tolerance) return false;
				}
				else
				{
					if (!cmp.AreEqual(otherVector[i], Values[j])) return false;
					++j;
				}
			return true;
		}
		public virtual bool Equals(IReadOnlyVector otherVector, double tolerance = 1E-07)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return Equals(sparseVector, tolerance);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return Equals(fullyPopulatedVector, tolerance);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}



		// ------------ STATIC MEMBERS WHICH DO NOT EXPOSE SPARSE FUNCTIONALITY TO DENSE VECTORS

		internal static AbstractFullyPopulatedVector AxpyIntoDenseVector(AbstractFullyPopulatedVector thisVector, AbstractSparseVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(thisVector, otherVector);
			if (otherCoefficient == 1)
				for (int i = otherVector.FromIndex; i < otherVector.ToIndex; ++i)
					thisVector[otherVector.Indices[i]] += otherVector.Values[i];
			else if (otherCoefficient == -1)
				for (int i = otherVector.FromIndex; i < otherVector.ToIndex; ++i)
					thisVector[otherVector.Indices[i]] -= otherVector.Values[i];
			else if (otherCoefficient != 0)
				for (int i = otherVector.FromIndex; i < otherVector.ToIndex; ++i)
					thisVector[otherVector.Indices[i]] += otherCoefficient * otherVector.Values[i];
			return thisVector;
		}

		internal static AbstractFullyPopulatedVector CopyToDenseVector(AbstractFullyPopulatedVector thisVector, AbstractSparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(thisVector, otherVector);
			thisVector.Clear();    // it is faster, initially to set all elements to zero, then change only the sparse stored elements
			for (int i = otherVector.FromIndex; i < otherVector.ToIndex; ++i)
				thisVector[otherVector.Indices[i]] = otherVector.Values[i];
			return thisVector;
		}

		internal static AbstractFullyPopulatedVector DoEntrywiseIntoDenseVector(AbstractFullyPopulatedVector thisVector, AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(thisVector, otherVector);
			for (int i = 0, j = otherVector.FromIndex; i < thisVector.Length; ++i)
				if (j >= otherVector.ToIndex || i < otherVector.Indices[j])
					thisVector[i] = binaryOperation(thisVector[i], 0);
				else
				{
					thisVector[i] = binaryOperation(thisVector[i], otherVector.Values[j]);
					++j;
				}
			return thisVector;
		}
		internal static AbstractFullyPopulatedVector AxpyIntoDenseVector(AbstractContiguousFullyPopulatedVector thisVector, AbstractSparseVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(thisVector, otherVector);
			SparseBlas.Daxpyi(otherVector.ToIndex - otherVector.FromIndex, otherCoefficient, otherVector.Values, otherVector.Indices, otherVector.FromIndex, thisVector.Values, thisVector.FromIndex);
			return thisVector;
		}

		public double Reduce(double identityValue, ProcessEntry processEntry, ProcessZeros processZeros, Finalize finalize)
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				identityValue = processEntry(Values[i], identityValue);
			identityValue = processZeros(Length - (ToIndex - FromIndex), identityValue);
			return finalize(identityValue);
		}
	}
}
