namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Matrices;


	/// <summary>
	/// A fully populated with elements vector.
	/// </summary>
	/// Fully populated does not mean contiguous stored elements.
	public abstract class AbstractFullyPopulatedVector : IExtendedMutableVector
	{
		/// <summary>
		/// Returns a reference to the element at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the element to return.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is lower than 0 or greater or equal to size of the vector</exception>
		abstract public ref double this[int index] { get; }

		abstract public int Length { get; }
		int IMinimalImmutableVector.Length => Length;

		/// <summary>
		/// Provides a vector from scattered elements of this vector.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned vector.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A vector from this vector with elements for given indices</returns>
		virtual public Vector Copy(int[] indices) => new Vector(CopyToArray(indices));

		/// <summary>
		/// Return scattered elements from this vector to a new array.
		/// </summary>
		/// <param name="indices">Element with that indices of this vector, form the returned array.
		/// Not all indices from this vector needed and the same indices can exist more than once.</param>
		/// <returns>A new array with elements from this vector for given indices</returns>
		virtual public double[] CopyToArray(int[] indices)
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
		virtual public void CopyToArray(double[] array, int arrayIndex, int[] indices)
		{
			for (int i = 0; i < indices.Length; ++i)
				array[i + arrayIndex] = this[indices[i]];
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
		abstract public AbstractFullyPopulatedVector View(int[] indices);

		/// <summary>
		/// Calculates the tensor product of this vector with <paramref name="otherVector"/>:
		/// result[i, j] = this[i] * otherVector[j], for all valid i, j.
		/// </summary>
		/// <param name="otherVector">The other vector.</param>
		virtual public Matrix TensorProduct(AbstractFullyPopulatedVector otherVector)
		{
			var result = Matrix.CreateZero(Length, otherVector.Length);
			for (int i = 0; i < this.Length; ++i)
				for (int j = 0; j < otherVector.Length; ++j)
					result[i, j] = this[i] * otherVector[j];
			return result;
		}



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedImmutableVector

		virtual public Vector Copy(int fromIndex, int toIndex) => new Vector(CopyToArray(fromIndex, toIndex));
		IExtendedMutableVector IExtendedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex);

		virtual public Vector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => (Vector)IMinimalImmutableVector.Axpy(this, otherVector, otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Axpy(otherVector, otherCoefficient);

		virtual public Vector Add(IMinimalImmutableVector otherVector) => (Vector)IMinimalImmutableVector.Add(this, otherVector);
		IExtendedMutableVector IExtendedImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector);

		virtual public Vector Subtract(IMinimalImmutableVector otherVector) => (Vector)IMinimalImmutableVector.Subtract(this, otherVector);
		IExtendedMutableVector IExtendedImmutableVector.Subtract(IMinimalImmutableVector otherVector) => (Vector)IMinimalImmutableVector.Subtract(this, otherVector);

		virtual public Vector Negative() => (Vector)IMinimalImmutableVector.Negative(this);
		IExtendedMutableVector IExtendedImmutableVector.Negative() => Negative();

		virtual public Vector Scale(double coefficient) => (Vector)IMinimalImmutableVector.Scale(this, coefficient);
		IExtendedMutableVector IExtendedImmutableVector.Scale(double coefficient) => Scale(coefficient);

		virtual public Vector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => (Vector)IMinimalImmutableVector.LinearCombination(this, thisCoefficient, otherVector, otherCoefficient);
		IExtendedMutableVector IExtendedImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient);

		virtual public Vector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => (Vector)IMinimalImmutableVector.DoEntrywise(this, otherVector, binaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation);

		virtual public Vector DoToAllEntries(Func<double, double> unaryOperation) => (Vector)IMinimalImmutableVector.DoToAllEntries(this, unaryOperation);
		IExtendedMutableVector IExtendedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);

		virtual public double[] CopyToArray() => IExtendedImmutableVector.CopyToArray(this);
		virtual public double[] CopyToArray(int fromIndex, int toIndex) => IExtendedImmutableVector.CopyToArray(this, fromIndex, toIndex);
		virtual public void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length)
		{
			for (int i = 0; i < length; ++i)
				array[arrayIndex + i] = this[fromIndex + i];
		}

		virtual public Vector Copy() => new Vector(CopyToArray());
		IExtendedMutableVector IExtendedImmutableVector.Copy() => Copy();

		virtual public Vector CreateZero() => new Vector(new double[Length]);
		IExtendedMutableVector IExtendedImmutableVector.CreateZero() => CreateZero();



		// ------------------- COVARIANT RETURN TYPE FROM IExtendedMutableVector

		abstract public AbstractFullyPopulatedVector View(int fromIndex, int toIndex);
		IExtendedMutableVector IExtendedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex);

		virtual public AbstractFullyPopulatedVector AxpyIntoThis(SparseVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			if (otherCoefficient == 1)
				for (int i = 0; i < otherVector.RawIndices.Length; ++i)
					this[otherVector.RawIndices[i]] += otherVector.RawValues[i];
			else if (otherCoefficient == -1)
				for (int i = 0; i < otherVector.RawIndices.Length; ++i)
					this[otherVector.RawIndices[i]] -= otherVector.RawValues[i];
			else if (otherCoefficient != 0)
				for (int i = 0; i < otherVector.RawIndices.Length; ++i)
					this[otherVector.RawIndices[i]] += otherCoefficient * otherVector.RawValues[i];
			return this;
		}
		virtual public AbstractFullyPopulatedVector AxpyIntoThis(AbstractFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
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
		virtual public AbstractFullyPopulatedVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.Scale(otherCoefficient).Add(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is SparseVector sparseVector) return AxpyIntoThis(sparseVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return AxpyIntoThis(fullyPopulatedVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}
		IExtendedMutableVector IExtendedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient);

		virtual public AbstractFullyPopulatedVector AddIntoThis(IMinimalImmutableVector otherVector) => (AbstractFullyPopulatedVector) IMinimalMutableVector.AddIntoThis(this, otherVector);
		IExtendedMutableVector IExtendedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector);

		virtual public AbstractFullyPopulatedVector SubtractIntoThis(IMinimalImmutableVector otherVector) => (AbstractFullyPopulatedVector)IMinimalMutableVector.SubtractIntoThis(this, otherVector);
		IExtendedMutableVector IExtendedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector);

		virtual public AbstractFullyPopulatedVector NegativeIntoThis()
		{
			for (int i = 0; i < Length; ++i)
				this[i] = -this[i];
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.NegativeIntoThis() => NegativeIntoThis();

		virtual public AbstractFullyPopulatedVector ScaleIntoThis(double coefficient)
		{
			for (int i = 0; i < Length; ++i)
				this[i] *= coefficient;
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);

		virtual public AbstractFullyPopulatedVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => (AbstractFullyPopulatedVector)IMinimalMutableVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
		IExtendedMutableVector IExtendedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);

		virtual public AbstractFullyPopulatedVector CopyFrom(IMinimalImmutableVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0; i < Length; ++i)
				this[i] = ((AbstractFullyPopulatedVector)otherVector)[i];
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.CopyFrom(IMinimalImmutableVector otherVector) => CopyFrom(otherVector);

		virtual public AbstractFullyPopulatedVector Clear() => (AbstractFullyPopulatedVector)IMinimalMutableVector.Clear(this);
		IExtendedMutableVector IExtendedMutableVector.Clear() => Clear();

		virtual public AbstractFullyPopulatedVector SetAll(double value)
		{
			for (int i = 0; i < Length; ++i)
				this[i] = value;
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.SetAll(double value) => SetAll(value);

		virtual public AbstractFullyPopulatedVector DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0, j = 0; i < Length; ++i)
				this[i] = binaryOperation(this[i],
					j >= otherVector.RawIndices.Length || i < otherVector.RawIndices[j]
					? 0
					: otherVector.RawValues[j++]);
			return this;
		}
		virtual public AbstractFullyPopulatedVector DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0; i < Length; ++i)
				this[i] = binaryOperation(this[i], otherVector[i]);
			return this;
		}
		virtual public AbstractFullyPopulatedVector DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is SparseVector sparseVector) return DoEntrywiseIntoThis(sparseVector, binaryOperation);
			else if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DoEntrywiseIntoThis(fullyPopulatedVector, binaryOperation);
			else throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}
		IExtendedMutableVector IExtendedMutableVector.DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywiseIntoThis(otherVector, binaryOperation);

		virtual public AbstractFullyPopulatedVector DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
		{
			for (int i = 0; i < Length; ++i)
				this[i] = unaryOperation(this[i]);
			return this;
		}
		IExtendedMutableVector IExtendedMutableVector.DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => DoToAllEntriesIntoThis(unaryOperation);



		// ------------------- COVARIANT RETURN TYPE FROM IMinimalImmutableVector

		virtual public double DotProduct(SparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			double result = 0;
			for (int i = 0; i < otherVector.RawIndices.Length; ++i)
				result += this[otherVector.RawIndices[i]] * otherVector.RawValues[i];
			return result;
		}
		virtual public double DotProduct(AbstractFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			double result = 0;
			for (int i = 0; i < Length; ++i)
				result += this[i] * otherVector[i];
			return result;
		}
		virtual public double DotProduct(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.DotProduct(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is SparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DotProduct(fullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}
		
		virtual public bool IsZero(double tolerance = 1e-7)
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

		virtual public double Square() => IMinimalImmutableVector.Square(this);

		virtual public double Norm2() => IMinimalImmutableVector.Norm2(this);

		virtual public bool Equals(SparseVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			for (int i = 0, j = 0; i < Length; ++i)
				if (!cmp.AreEqual(this[i],
						j >= otherVector.RawIndices.Length || i < otherVector.RawIndices[j]
						? 0
						: otherVector.RawValues[j++])) return false;
			return true;
		}
		virtual public bool Equals(AbstractFullyPopulatedVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			for (int i = 0; i < Length; ++i)
				if (!cmp.AreEqual(this[i], otherVector[i])) return false;
			return true;
		}
		virtual public bool Equals(IMinimalImmutableVector otherVector, double tolerance = 1E-07)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is SparseVector sparseVector) return Equals(sparseVector, tolerance);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return Equals(fullyPopulatedVector, tolerance);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}

		/// <summary>
		/// Returns the Z component of the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 2 dimensional vectors.
		/// </summary>
		/// For calculation uses the 2 first components of this vector with 2 first components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Z component of the cross product</returns>
		public double CrossProductZ(AbstractFullyPopulatedVector otherVector) => this[0] * otherVector[1] - this[1] * otherVector[0];
		/// <summary>
		/// Returns the X component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 2nd and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The X component of the cross product</returns>
		public double CrossProductX(AbstractFullyPopulatedVector otherVector) => -this[1] * otherVector[2] + this[2] * otherVector[1];
		/// <summary>
		/// Returns the Y component of the cross product of this vector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 1st and 3rd components of this vector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other vector</param>
		/// <returns>The Y component of the cross product</returns>
		public double CrossProductY(AbstractFullyPopulatedVector otherVector) => this[0] * otherVector[2] - this[2] * otherVector[0];
		/// <summary>
		/// Returns the cross product of this vector with <paramref name="otherVector"/>.
		/// This is the cross product of 3 dimensional vectors.
		/// </summary>
		/// <param name="otherVector">The other vector</param>
		/// <returns>The cross product</returns>
		public Vector CrossProduct(AbstractFullyPopulatedVector otherVector) => new Vector(new double[] { CrossProductX(otherVector), CrossProductY(otherVector), CrossProductZ(otherVector) });
	}
}
