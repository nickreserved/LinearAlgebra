namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Diagnostics;
	using MGroup.LinearAlgebra.Commons;

	using static MGroup.LinearAlgebra.LibrarySettings;

	public class SubVectorView : IFullyPopulatedMutableVector
	{
		public SubVectorView(double[] elements, int fromIndex, int toIndex)
		{
			Elements = elements;
			FromIndex = fromIndex;
			ToIndex = toIndex;
			Debug.Assert(elements.Length >= toIndex);
		}

		public double[] Elements { get; }
		public int FromIndex { get; }
		public int ToIndex { get; }
		public int Length { get { return ToIndex - FromIndex; } }

		public ref double this[int index] => ref Elements[FromIndex + index];


		public Vector Add(IMinimalImmutableVector otherVector) => Axpy(otherVector, 1);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Add(IMinimalImmutableVector otherVector) => Add(otherVector); /*TODO: remove line when C#9*/

		public SubVectorView AddIntoThis(IMinimalImmutableVector otherVector) => AxpyIntoThis(otherVector, 1);
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.AddIntoThis(IMinimalImmutableVector otherVector) => AddIntoThis(otherVector); /*TODO: remove line when C#9*/

		public Vector Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Copy().AxpyIntoThis(otherVector, otherCoefficient);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Axpy(IMinimalImmutableVector otherVector, double otherCoefficient) => Copy().AxpyIntoThis(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		public SubVectorView AxpyIntoThis(SubVectorView otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			Blas.Daxpy(Length, otherCoefficient, otherVector.Elements, otherVector.FromIndex, 1, Elements, FromIndex, 1);
			return this;
		}
		public SubVectorView AxpyIntoThis(SparseVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			//Blas my ass
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
		public SubVectorView AxpyIntoThis(INotFullyPopulatedImmutableVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			//Blas my ass
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
		public SubVectorView AxpyIntoThis(Vector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			Blas.Daxpy(Length, otherCoefficient, otherVector.Elements, 0, 1, Elements, FromIndex, 1);
			return this;
		}
		public SubVectorView AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.Scale(otherCoefficient).Add(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is SubVectorView subVectorView) return AxpyIntoThis(subVectorView, otherCoefficient);
			if (otherVector is Vector vector) return AxpyIntoThis(vector, otherCoefficient);
			if (otherVector is SparseVector sparseVector) return AxpyIntoThis(sparseVector, otherCoefficient);
			if (otherVector is INotFullyPopulatedImmutableVector notFullyPopulatedVector) return AxpyIntoThis(notFullyPopulatedVector, otherCoefficient);
			throw new NotImplementedException("Axpy(NotSupportedVector, otherCoefficient)");
		}
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => AxpyIntoThis(otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		public void Clear() => SetAll(0);

		public Vector Copy(int fromIndex, int toIndex) => new Vector(CopyToArray(fromIndex, toIndex));
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Copy(int fromIndex, int toIndex) => Copy(fromIndex, toIndex); /*TODO: remove line when C#9*/

		public Vector Copy(int[] indices) => new Vector(CopyToArray(indices));
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Copy(int[] indices) => Copy(indices); /*TODO: remove line when C#9*/

		public Vector Copy() => new Vector(CopyToArray());
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Copy() => Copy(); /*TODO: remove line when C#9*/

		public void CopyFrom(IMinimalImmutableVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0; i < Length; ++i)
				this[i] = ((INotFullyPopulatedImmutableVector)otherVector)[i];
		}

		public double[] CopyToArray()
		{
			var result = new double[Length];
			Array.Copy(Elements, FromIndex, result, 0, result.Length);
			return result;
		}

		public double[] CopyToArray(int fromIndex, int toIndex)
		{
			var result = new double[toIndex - fromIndex];
			Array.Copy(Elements, FromIndex + fromIndex, result, 0, result.Length);
			return result;
		}

		public void CopyToArray(double[] array, int arrayIndex, int fromIndex, int toIndex)
			=> Array.Copy(Elements, FromIndex + fromIndex, array, arrayIndex, toIndex - fromIndex);

		public double[] CopyToArray(int[] indices)
		{
			var result = new double[indices.Length];
			for (int i = 0; i<indices.Length; ++i)
				result[i] = this[indices[i]];
			return result;
		}

		public void CopyToArray(double[] array, int arrayIndex, int[] indices)
		{
			for (int i = 0; i < indices.Length; ++i)
				array[i + arrayIndex] = this[indices[i]];
		}

		public Vector CreateZero() => new Vector(new double[Length]);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.CreateZero() => CreateZero(); /*TODO: remove line when C#9*/

		public Vector DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			Vector result = Copy();
			result.DoEntrywiseIntoThis(otherVector, binaryOperation);
			return result;
		}
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.DoEntrywise(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => DoEntrywise(otherVector, binaryOperation); /*TODO: remove line when C#9*/

		public void DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0, j = 0; i < Length; ++i)
				this[i] = binaryOperation(this[i],
					j >= otherVector.RawIndices.Length || i < otherVector.RawIndices[j]
					? 0
					: otherVector.RawValues[j++]);
		}
		
		public void DoEntrywiseIntoThis(INotFullyPopulatedImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			for (int i = 0; i < Length; ++i)
				this[i] = binaryOperation(this[i], otherVector[i]);
		}
		public void DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is SparseVector sparseVector) DoEntrywiseIntoThis(sparseVector, binaryOperation);
			else if (otherVector is INotFullyPopulatedImmutableVector notFullyPopulatedVector) DoEntrywiseIntoThis(notFullyPopulatedVector, binaryOperation);
			else throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}

		public Vector DoToAllEntries(Func<double, double> unaryOperation)
		{
			var result = Copy();
			result.DoToAllEntriesIntoThis(unaryOperation);
			return result;
		}
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation); /*TODO: remove line when C#9*/

		public void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
		{
			for (int i = FromIndex; i < ToIndex; ++i)
				Elements[i] = unaryOperation(Elements[i]);
		}

		public double DotProduct(SubVectorView otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			return Blas.Ddot(Length, Elements, FromIndex, 1, otherVector.Elements, otherVector.FromIndex, 1);
		}
		public double DotProduct(Vector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			return Blas.Ddot(Length, Elements, FromIndex, 1, otherVector.Elements, 0, 1);
		}
		public double DotProduct(SparseVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			double result = 0;
			for (int i = 0; i < otherVector.RawIndices.Length; ++i)
				result += this[otherVector.RawIndices[i]] * otherVector.RawValues[i];
			return result;
		}
		public double DotProduct(INotFullyPopulatedImmutableVector otherVector)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			double result = 0;
			for (int i = 0; i < Length; ++i)
				result += this[i] * otherVector[i];
			return result;
		}
		public double DotProduct(IMinimalImmutableVector otherVector)
		{
			// Runtime Identification is_a_bad_thing™
			// Another (better) solution is otherVector.DotProduct(this); because 'this' is already identified
			// but it needs an implementation for any type of vector
			if (otherVector is SubVectorView subVectorView) return DotProduct(subVectorView);
			if (otherVector is Vector vector) return DotProduct(vector);
			if (otherVector is SparseVector sparseVector) return DotProduct(sparseVector);
			if (otherVector is INotFullyPopulatedImmutableVector notFullyPopulatedVector) return DotProduct(notFullyPopulatedVector);
			throw new NotImplementedException("DotProduct(NotSupportedVector)");
		}

		public bool Equals(SparseVector otherVector, double tolerance = 1E-07)
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
		public bool Equals(INotFullyPopulatedImmutableVector otherVector, double tolerance = 1E-07)
		{
			if (Length != otherVector.Length) return false;
			var cmp = new ValueComparer(tolerance);
			for (int i = 0; i < Length; ++i)
				if (!cmp.AreEqual(this[i], otherVector[i])) return false;
			return true;
		}
		public bool Equals(IMinimalImmutableVector otherVector, double tolerance = 1E-07)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is SparseVector sparseVector) return Equals(sparseVector, tolerance);
			if (otherVector is INotFullyPopulatedImmutableVector notFullyPopulatedVector) return Equals(notFullyPopulatedVector, tolerance);
			throw new NotImplementedException("DoEntrywiseIntoThis(NotSupportedVector, binaryOperation)");
		}

		public Vector LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => Copy().LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.LinearCombination(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombination(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		public SubVectorView LinearCombinationIntoThis(double thisCoefficient, SubVectorView otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			BlasExtensions.Daxpby(Length, otherCoefficient, otherVector.Elements, otherVector.FromIndex, 1, thisCoefficient, Elements, FromIndex, 1);
			return this;
		}
		public SubVectorView LinearCombinationIntoThis(double thisCoefficient, Vector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			BlasExtensions.Daxpby(Length, otherCoefficient, otherVector.Elements, 0, 1, thisCoefficient, Elements, FromIndex, 1);
			return this;
		}
		public SubVectorView LinearCombinationIntoThis(double thisCoefficient, INotFullyPopulatedImmutableVector otherVector, double otherCoefficient)
		{
			Preconditions.CheckVectorDimensions(Length, otherVector.Length);
			if (thisCoefficient == 0)
			{
				if (otherCoefficient == 0)
					Clear();
				else
				{
					CopyFrom(otherVector);
					if (otherCoefficient != 1)
						ScaleIntoThis(otherCoefficient);
				}
			}
			else
			{
				if (thisCoefficient != 1)
					ScaleIntoThis(thisCoefficient);
				AxpyIntoThis(otherVector, otherCoefficient);
			}
			return this;
		}
		public SubVectorView LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient)
		{
			// Runtime Identification is_a_bad_thing™
			if (otherVector is SubVectorView subVectorView) return LinearCombinationIntoThis(thisCoefficient, subVectorView, otherCoefficient);
			if (otherVector is Vector vector) return LinearCombinationIntoThis(thisCoefficient, vector, otherCoefficient);
			if (otherVector is INotFullyPopulatedImmutableVector notFullyPopulatedVector) return LinearCombinationIntoThis(thisCoefficient, notFullyPopulatedVector, otherCoefficient);
			throw new NotImplementedException("LinearCombinationIntoThis(thisCoefficient, NotSupportedVector, otherCoefficient)");
		}
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient); /*TODO: remove line when C#9*/

		public double Norm2() => Math.Sqrt(Square());

		public Vector Scale(double coefficient) => Copy().ScaleIntoThis(coefficient);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Scale(double coefficient) => Scale(coefficient); /*TODO: remove line when C#9*/

		public SubVectorView ScaleIntoThis(double coefficient)
		{
			Blas.Dscal(Length, coefficient, Elements, FromIndex, 1);
			return this;
		}
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient); /*TODO: remove line when C#9*/

		public void SetAll(double value) => Array.Fill(Elements, value, FromIndex, Length);

		public double Square() => DotProduct(this);

		public Vector Subtract(IMinimalImmutableVector otherVector) => Axpy(otherVector, -1);
		IFullyPopulatedMutableVector IFullyPopulatedImmutableVector.Subtract(IMinimalImmutableVector otherVector) => Subtract(otherVector); /*TODO: remove line when C#9*/

		public SubVectorView SubtractIntoThis(IMinimalImmutableVector otherVector) => AxpyIntoThis(otherVector, -1);
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.SubtractIntoThis(IMinimalImmutableVector otherVector) => SubtractIntoThis(otherVector); /*TODO: remove line when C#9*/

		public SubVectorView View(int fromIndex, int toIndex) => new SubVectorView(Elements, FromIndex + fromIndex, FromIndex + toIndex);
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.View(int fromIndex, int toIndex) => View(fromIndex, toIndex); /*TODO: remove line when C#9*/
		
		public PermutatedVectorView View(int[] indices)
		{
			for (int i = 0; i < indices.Length; i++)
				indices[i] += FromIndex;
			return new PermutatedVectorView(Elements, indices);
		}
		IFullyPopulatedMutableVector IFullyPopulatedMutableVector.View(int[] indices) => View(indices); /*TODO: remove line when C#9*/
	}
}
