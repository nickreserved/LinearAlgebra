namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Diagnostics;
	using MGroup.LinearAlgebra.Commons;

	using static MGroup.LinearAlgebra.LibrarySettings;

	/// <summary>A fully populated with elements vector, with its elements stored contiguously in memory.</summary>
	/// <remarks>Contiguously stored elements in memory, give much better performance in almost any operations.</remarks>

	[Serializable]
	public abstract class AbstractContiguousFullyPopulatedVector : AbstractFullyPopulatedVector
	{
		public abstract double[] Values { get; }
		public abstract int FromIndex { get; }
		public override ref double this[int index] => ref Values[FromIndex + index];

		public override AbstractFullyPopulatedVector View(int fromIndex, int toIndex) => new SubVectorView(Values, FromIndex + fromIndex, FromIndex + toIndex);
		
		/// <summary>
		/// Provides a scattered view to this contiguousVector.
		/// </summary>
		/// Any change in subvector view elements, changes also corresponding elements of this contiguousVector.
		/// <param name="indices">Element with that indices of this contiguousVector, form the returned contiguousVector view.
		/// Not all indices from this contiguousVector needed and the same indices can exist more than once.
		/// This method will modify indices. If you want this array intact, keep a copy.</param>
		/// <returns>A contiguousVector view of this contiguousVector with elements for given indices</returns>
		public override AbstractFullyPopulatedVector View(int[] indices)
		{
			if (FromIndex != 0)
				for (int i = 0; i < indices.Length; i++)
					indices[i] += FromIndex;
			return new PermutatedVectorView(Values, indices);
		}

		public override void Clear() => Array.Clear(Values, FromIndex, Length);
		
		public override void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length) => Array.Copy(Values, FromIndex + fromIndex, array, arrayIndex, length);

		public AbstractFullyPopulatedVector CopyFrom(AbstractContiguousFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			otherVector.CopyToArray(otherVector.FromIndex, Values, FromIndex, Length);
			return this;
		}
		public override void CopyFrom(IMinimalReadOnlyVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractContiguousFullyPopulatedVector fullyPopulatedVector) CopyFrom(fullyPopulatedVector);
			else base.CopyFrom(otherVector);
		}

		public AbstractContiguousFullyPopulatedVector LinearCombinationIntoThis(double thisCoefficient, AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			BlasExtensions.Daxpby(Length, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1, thisCoefficient, Values, FromIndex, 1);
			return this;
		}
		public override void LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) LinearCombinationIntoThis(thisCoefficient, contiguousVector, otherCoefficient);
			else IMinimalVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
		}

		public override void SetAll(double value) => Array.Fill(Values, value, FromIndex, Length);
		
		public AbstractContiguousFullyPopulatedVector AxpyIntoThis(AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			Blas.Daxpy(Length, otherCoefficient, otherVector.Values, otherVector.FromIndex, 1, Values, FromIndex, 1);
			return this;
		}
		public override AbstractFullyPopulatedVector AxpyIntoThis(AbstractSparseVector otherVector, double otherCoefficient)
			=> AbstractSparseVector.AxpyIntoDenseVector(this, otherVector, otherCoefficient);
		public override void AxpyIntoThis(IMinimalReadOnlyVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) AxpyIntoThis(contiguousVector, otherCoefficient);
			else base.AxpyIntoThis(otherVector, otherCoefficient);
		}

		public double DotProduct(AbstractContiguousFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			return Blas.Ddot(Length, Values, FromIndex, 1, otherVector.Values, otherVector.FromIndex, 1);
		}
		public override double DotProduct(AbstractSparseVector otherVector) => otherVector.DotProduct(this);
		public override double DotProduct(IMinimalReadOnlyVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) return DotProduct(contiguousVector);
			else return base.DotProduct(otherVector);
		}

		public override double Norm2() => Blas.Dnrm2(Length, Values, FromIndex, 1);

		public override void ScaleIntoThis(double coefficient) => Blas.Dscal(Length, coefficient, Values, FromIndex, 1);
	}
}
