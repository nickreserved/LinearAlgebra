namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Diagnostics;
	using MGroup.LinearAlgebra.Commons;

	using static MGroup.LinearAlgebra.LibrarySettings;

	abstract public class AbstractContiguousFullyPopulatedVector : AbstractFullyPopulatedVector
	{
		abstract public double[] Elements { get; }
		abstract public int FromIndex { get; }
		override public ref double this[int index] => ref Elements[FromIndex + index];

		override public AbstractFullyPopulatedVector View(int fromIndex, int toIndex) => new SubVectorView(Elements, FromIndex + fromIndex, FromIndex + toIndex);
		
		/// <summary>
		/// Provides a scattered view to this contiguousVector.
		/// </summary>
		/// Any change in subvector view elements, changes also corresponding elements of this contiguousVector.
		/// <param name="indices">Element with that indices of this contiguousVector, form the returned contiguousVector view.
		/// Not all indices from this contiguousVector needed and the same indices can exist more than once.
		/// This method will modify indices. If you want this array intact, keep a copy.</param>
		/// <returns>A contiguousVector view of this contiguousVector with elements for given indices</returns>
		override public AbstractFullyPopulatedVector View(int[] indices)
		{
			if (FromIndex != 0)
				for (int i = 0; i < indices.Length; i++)
					indices[i] += FromIndex;
			return new PermutatedVectorView(Elements, indices);
		}

		override public AbstractFullyPopulatedVector Clear()
		{
			Array.Clear(Elements, FromIndex, Length);
			return this;
		}
		
		override public void CopyToArray(int fromIndex, double[] array, int arrayIndex, int length) => Array.Copy(Elements, FromIndex + fromIndex, array, arrayIndex, length);
		
		virtual public AbstractContiguousFullyPopulatedVector LinearCombinationIntoThis(double thisCoefficient, AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			BlasExtensions.Daxpby(Length, otherCoefficient, otherVector.Elements, otherVector.FromIndex, 1, thisCoefficient, Elements, FromIndex, 1);
			return this;
		}
		override public AbstractFullyPopulatedVector LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) return LinearCombinationIntoThis(thisCoefficient, contiguousVector, otherCoefficient);
			return (SubVectorView) IMinimalMutableVector.LinearCombinationIntoThis(this, thisCoefficient, otherVector, otherCoefficient);
		}

		override public AbstractFullyPopulatedVector SetAll(double value)
		{
			Array.Fill(Elements, value, FromIndex, Length);
			return this;
		}
		
		virtual public AbstractFullyPopulatedVector AxpyIntoThis(AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			Blas.Daxpy(Length, otherCoefficient, otherVector.Elements, otherVector.FromIndex, 1, Elements, FromIndex, 1);
			return this;
		}
		override public AbstractFullyPopulatedVector AxpyIntoThis(AbstractSparseVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			SparseBlas.Daxpyi(otherVector.Indices.Length, otherCoefficient, otherVector.Values, otherVector.Indices, otherVector.FromIndex, Elements, FromIndex);
			return this;
		}
		override public AbstractFullyPopulatedVector AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return AxpyIntoThis(sparseVector, otherCoefficient);
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) return AxpyIntoThis(contiguousVector, otherCoefficient);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return AxpyIntoThis(fullyPopulatedVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}

		virtual public double DotProduct(AbstractContiguousFullyPopulatedVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			return Blas.Ddot(Length, Elements, FromIndex, 1, otherVector.Elements, otherVector.FromIndex, 1);
		}
		override public double DotProduct(AbstractSparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(this, otherVector);
			return SparseBlas.Ddoti(Length, otherVector.Values, otherVector.Indices, otherVector.FromIndex, Elements, FromIndex);
		}
		override public double DotProduct(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is AbstractSparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is AbstractContiguousFullyPopulatedVector contiguousVector) return DotProduct(contiguousVector);
			if (otherVector is AbstractFullyPopulatedVector fullyPopulatedVector) return DotProduct(fullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}

		override public double Norm2() => Blas.Dnrm2(Length, Elements, FromIndex, 1);

		override public AbstractFullyPopulatedVector ScaleIntoThis(double coefficient)
		{
			Blas.Dscal(Length, coefficient, Elements, FromIndex, 1);
			return this;
		}
	}
}
