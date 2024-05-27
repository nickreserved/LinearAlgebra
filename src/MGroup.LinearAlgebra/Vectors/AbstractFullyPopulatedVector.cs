namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Reduction;


	/// <summary>
	/// A fully populated with elements vector.
	/// </summary>
	/// <remarks>Fully populated does not mean contiguous stored elements.</remarks>
	[Serializable]
	public abstract class AbstractFullyPopulatedVector : IExtendedVector
	{
		/// <summary>
		/// Returns a reference to the element at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the element to return.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector</exception>
		public abstract ref double this[int index] { get; }
		double IVector.this[int index]
		{
			get => this[index];
			set { this[index] = value; }
		}

		public abstract int Length { get; }
		int IReadOnlyVector.Length => Length;

		/// <summary>
		/// Provides a vector from scattered elements of this vector.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned vector.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector from this vector with elements for given indices</returns>
		public Vector Copy(int[] indices) => new Vector(CopyToArray(indices));

		/// <summary>
		/// Return scattered elements from this vector to a new array.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A new array with elements from this vector for given indices</returns>
		public double[] CopyToArray(int[] indices)
		{
			var result = new double[indices.Length];
			for (int i = 0; i < indices.Length; ++i)
				result[i] = this[indices[i]];
			return result;
		}

		/// <summary>
		/// Copy to an existed array, scattered elements from this vector to a new array.
		/// </summary>
		/// <param name="array">An existing target array.</param>
		/// <param name="arrayIndex">Index of first element in target <paramref name="array"/> where elements of this vector will be written</param>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		public void CopyToArray(double[] array, int arrayIndex, int[] indices)
		{
			for (int i = 0; i < indices.Length; ++i)
				array[i + arrayIndex] = this[indices[i]];
		}
		
		public double[] CopyToArray() => CopyToArray(0, Length);
		public double[] CopyToArray(int fromIndex, int toIndex) => IExtendedReadOnlyVector.CopyToArray(this, fromIndex, toIndex);
		public virtual void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length)
		{
			for (int i = 0; i < length; ++i)
				array[arrayIndex + i] = this[fromIndex + i];
		}

		/// <summary>
		/// Provides a scattered view to this vector.
		/// </summary>
		/// View can expose mutable functionality in derived classes.
		/// In that case, any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="indices">Element with that indices of this vector, form the returned vector view.
		/// Not all indices from this vector needed and the same indices can exist more than once.
		/// It is not guaranteed that this method will not modify indices. If you want this array intact, keep a copy.</param>
		/// <returns>A vector view of this vector with elements for given indices</returns>
		public abstract AbstractFullyPopulatedVector View(int[] indices);

		/// <summary>
		/// Calculates the tensor product of this vector with <paramref name="otherVector"/>:
		/// result[i, j] = this[i] * otherVector[j], for all valid i, j.
		/// </summary>
		/// <param name="otherVector">The other vector.</param>
		public Matrix TensorProduct(AbstractFullyPopulatedVector otherVector)
		{
			var result = Matrix.CreateZero(Length, otherVector.Length);
			for (int i = 0; i < this.Length; ++i)
				for (int j = 0; j < otherVector.Length; ++j)
					result[i, j] = this[i] * otherVector[j];
			return result;
		}



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedReadOnlyVector

		public Vector Copy(int fromIndex, int toIndex) => new Vector(CopyToArray(fromIndex, toIndex));
		IExtendedVector IExtendedReadOnlyVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex);

		public Vector Axpy(IReadOnlyVector otherVector, double otherCoefficient) => (Vector)IReadOnlyVector.Axpy(this, otherVector, otherCoefficient);
		IExtendedVector IExtendedReadOnlyVector.Axpy(IReadOnlyVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient);

		public Vector Add(IReadOnlyVector otherVector) => (Vector)IReadOnlyVector.Add(this, otherVector);
		IExtendedVector IExtendedReadOnlyVector.Add(IReadOnlyVector otherVector) => Add(otherVector);

		public Vector Subtract(IReadOnlyVector otherVector) => (Vector)IReadOnlyVector.Subtract(this, otherVector);
		IExtendedVector IExtendedReadOnlyVector.Subtract(IReadOnlyVector otherVector) => (Vector)IReadOnlyVector.Subtract(this, otherVector);

		public Vector Negate() => (Vector)IReadOnlyVector.Negate(this);
		IExtendedVector IExtendedReadOnlyVector.Negate() => Negate();

		public Vector Scale(double coefficient) => (Vector)IReadOnlyVector.Scale(this, coefficient);
		IExtendedVector IExtendedReadOnlyVector.Scale(double coefficient) => Scale(coefficient);

		public Vector LinearCombination(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient) => (Vector)IReadOnlyVector.LinearCombination(this, thisCoefficient, otherVector, otherCoefficient);
		IExtendedVector IExtendedReadOnlyVector.LinearCombination(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient);

		public Vector DoEntrywise(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => (Vector)IReadOnlyVector.DoEntrywise(this, otherVector, binaryOperation);
		IExtendedVector IExtendedReadOnlyVector.DoEntrywise(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation);

		public Vector DoToAllEntries(Func<double, double> unaryOperation) => (Vector)IReadOnlyVector.DoToAllEntries(this, unaryOperation);
		IExtendedVector IExtendedReadOnlyVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);

		public Vector Copy() => new Vector(CopyToArray());
		IExtendedVector IExtendedReadOnlyVector.Copy() => Copy();

		public Vector CreateZeroWithTheSameFormat() => new Vector(Length);
		IExtendedVector IExtendedReadOnlyVector.CreateZeroWithSameFormat() => CreateZeroWithTheSameFormat();



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedVector

		public abstract AbstractFullyPopulatedVector View(int fromIndex, int toIndex);
		IExtendedVector IExtendedVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);



		// ------------------- COVARIANT RETURN TYPE FROM IVector

		public virtual AbstractFullyPopulatedVector AxpyIntoThis(AbstractSparseVector otherVector, double otherCoefficient)
			=> AbstractSparseVector.AxpyIntoDenseVector(this, otherVector, otherCoefficient);
		public AbstractFullyPopulatedVector AxpyIntoThis(AbstractFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			if (otherCoefficient == 1)
				for (int i = 0; i < Length; ++i)
					this[i] += otherVector[i];
			else if (otherCoefficient == -1)
				for (int i = 0; i < Length; ++i)
					this[i] -= otherVector[i];
			else if (otherCoefficient != 0)
				for (int i = 0; i < Length; ++i)
					this[i] += otherCoefficient * otherVector[i];
			return this;
		}
		public virtual void AxpyIntoThis(IReadOnlyVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.Scale(otherCoefficient).Add(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is AbstractSparseVector sparseVector) AxpyIntoThis(sparseVector, otherCoefficient);
			else if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) AxpyIntoThis(fullyPopulatedVector, otherCoefficient);
			else throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}

		public virtual void AddIntoThis(IReadOnlyVector otherVector) => IVector.AddIntoThis(this, otherVector);

		public virtual void SubtractIntoThis(IReadOnlyVector otherVector) => IVector.SubtractIntoThis(this, otherVector);

		public virtual void NegateIntoThis()
		{
			for (int i = 0; i < Length; ++i)
				this[i] = -this[i];
		}

		public virtual void ScaleIntoThis(double coefficient)
		{
			for (int i = 0; i < Length; ++i)
				this[i] *= coefficient;
		}

		public virtual void LinearCombinationIntoThis(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient) => IVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);

		public virtual AbstractFullyPopulatedVector CopyFrom(AbstractSparseVector otherVector) => AbstractSparseVector.CopyToDenseVector(this, otherVector);
		public AbstractFullyPopulatedVector CopyFrom(AbstractFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			for (int i = 0; i < Length; ++i)
				this[i] = otherVector[i];
			return this;
		}
		public virtual void CopyFrom(IReadOnlyVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) CopyFrom(sparseVector);
			else if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) CopyFrom(fullyPopulatedVector);
			else throw new NotImplementedException("CopyFrom(NotSupportedVector)");
		}

		public virtual void Clear() => IVector.Clear(this);

		public virtual void SetAll(double value)
		{
			for (int i = 0; i < Length; ++i)
				this[i] = value;
		}

		public virtual AbstractFullyPopulatedVector DoEntrywiseIntoThis(AbstractSparseVector otherVector, Func<double, double, double> binaryOperation)
			=> AbstractSparseVector.DoEntrywiseIntoDenseVector(this, otherVector, binaryOperation);
		public AbstractFullyPopulatedVector DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			for (int i = 0; i < Length; ++i)
				this[i] = binaryOperation(this[i], otherVector[i]);
			return this;
		}
		public virtual void DoEntrywiseIntoThis(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) DoEntrywiseIntoThis(sparseVector, binaryOperation);
			else if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) DoEntrywiseIntoThis(fullyPopulatedVector, binaryOperation);
			else throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}

		public virtual void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
		{
			for (int i = 0; i < Length; ++i)
				this[i] = unaryOperation(this[i]);
		}



		// ------------------- COVARIANT RETURN TYPE FROM IReadOnlyVector

		public virtual double DotProduct(AbstractSparseVector otherVector) => otherVector.DotProduct(this);
		public double DotProduct(AbstractFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			double result = 0;
			for (int i = 0; i < Length; ++i)
				result += this[i] * otherVector[i];
			return result;
		}
		public virtual double DotProduct(IReadOnlyVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.DotProduct(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is AbstractSparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DotProduct(fullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}
		
		public virtual bool IsZero(double tolerance = 1e-7)
		{
			if (tolerance != 0)
			{
				for (int i = 0; i < Length; ++i)
					if (this[i] != 0) return false;
			}
			else
			{
				for (int i = 0; i < Length; ++i)
					if (Math.Abs(this[i]) > tolerance) return false;
			}
			return true;
		}

		public virtual double Square() => IReadOnlyVector.Square(this);

		public virtual double Norm2() => IReadOnlyVector.Norm2(this);

		public virtual bool Equals(AbstractSparseVector otherVector, double tolerance = 1E-07) => otherVector.Equals(this, tolerance);
		public bool Equals(AbstractFullyPopulatedVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			for (int i = 0; i < Length; ++i)
				if (!cmp.AreEqual(this[i], otherVector[i])) return false;
			return true;
		}
		public virtual bool Equals(IReadOnlyVector otherVector, double tolerance = 1E-07)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return Equals(sparseVector, tolerance);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return Equals(fullyPopulatedVector, tolerance);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}

		public double Reduce(double identityValue, ProcessEntry processEntry, ProcessZeros processZeros, Finalize finalize)
		{
			double accumulator = identityValue;
			for (int i = 0; i < Length; ++i) accumulator = processEntry(this[i], accumulator);
			// no zeros implied
			return finalize(accumulator);
		}

	}
}
