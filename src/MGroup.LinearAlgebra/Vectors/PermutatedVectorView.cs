namespace MGroup.LinearAlgebra.Vectors
{
	using System;

	public class PermutatedVectorView : AbstractFullyPopulatedVector
	{
		public PermutatedVectorView(double[] elements, int[] indices)
		{
			Elements = elements;
			Indices = indices;
		}

		public double[] Elements { get; }
		public int[] Indices { get; }

		public override int Length { get { return Indices.Length; } }

		public override ref double this[int index] => ref Elements[Indices[index]];

		public override AbstractFullyPopulatedVector View(int fromIndex, int toIndex)   //TODO: on C#9 change covariant return type to PermutatedVectorView
		{
			var indices = new int[toIndex - fromIndex];
			Array.Copy(Indices, fromIndex, indices, 0, indices.Length);
			return new PermutatedVectorView(Elements, indices);
		}

		/// <summary>
		/// Provides a scattered view to this vector.
		/// </summary>
		/// Any change in subvector view elements, changes also corresponding elements of this vector.
		/// <param name="indices">Element with that indices of this vector, form the returned vector view.
		/// Not all indices from this vector needed and the same indices can exist more than once.
		/// This method will modify indices. If you want this array intact, keep a copy.</param>
		/// <returns>A vector view of this vector with elements for given indices</returns>
		public override AbstractFullyPopulatedVector View(int[] indices)   //TODO: on C#9 change covariant return type to PermutatedVectorView
		{
			for (int i = 0; i < indices.Length; i++)
				indices[i] = Indices[indices[i]];
			return new PermutatedVectorView(Elements, indices);
		}



		/* Valid in C#9
		// ------------------- COVARIANT RETURN TYPE FROM AbstractFullyPopulatedVector

		new public PermutatedVectorView AddIntoThis(IMinimalReadOnlyVector otherVector) => (PermutatedVectorView)base.AddIntoThis(otherVector);
		new public PermutatedVectorView Clear() => (PermutatedVectorView)base.Clear();
		new public PermutatedVectorView CopyFrom(IMinimalReadOnlyVector otherVector) => (PermutatedVectorView)base.CopyFrom(otherVector);
		new public PermutatedVectorView DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => (PermutatedVectorView)base.DoToAllEntriesIntoThis(unaryOperation);
		new public PermutatedVectorView LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		new public PermutatedVectorView NegateIntoThis() => (PermutatedVectorView)base.NegateIntoThis();
		new public PermutatedVectorView ScaleIntoThis(double coefficient) => (PermutatedVectorView)base.ScaleIntoThis(coefficient);
		new public PermutatedVectorView SetAll(double value) => (PermutatedVectorView)base.SetAll(value);
		new public PermutatedVectorView SubtractIntoThis(IMinimalReadOnlyVector otherVector) => (PermutatedVectorView)base.SubtractIntoThis(otherVector);

		new public PermutatedVectorView AxpyIntoThis(SparseVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		new public PermutatedVectorView AxpyIntoThis(AbstractFullyPopulatedVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		new public PermutatedVectorView AxpyIntoThis(IMinimalReadOnlyVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);

		new public PermutatedVectorView DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		new public PermutatedVectorView DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		new public PermutatedVectorView DoEntrywiseIntoThis(IMinimalReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		*/

		// -------- OPERATORS FROM IMinimalReadOnlyVector

		public static Vector operator -(PermutatedVectorView x) => x.Negative();
		public static Vector operator +(PermutatedVectorView x, PermutatedVectorView y) => x.Add(y);
		public static Vector operator +(PermutatedVectorView x, IMinimalReadOnlyVector y) => x.Add(y);
		public static Vector operator +(IMinimalReadOnlyVector y, PermutatedVectorView x) => x.Add(y);
		public static Vector operator -(PermutatedVectorView x, PermutatedVectorView y) => x.Subtract(y);
		public static Vector operator -(PermutatedVectorView x, IMinimalReadOnlyVector y) => x.Subtract(y);
		public static Vector operator -(IMinimalReadOnlyVector y, PermutatedVectorView x) => (Vector)(x - y).NegativeIntoThis();
		public static double operator *(PermutatedVectorView x, PermutatedVectorView y) => x.DotProduct(y);
		public static double operator *(PermutatedVectorView x, IMinimalReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IMinimalReadOnlyVector x, PermutatedVectorView y) => x.DotProduct(y);
		public static Vector operator *(PermutatedVectorView x, double y) => x.Scale(y);
		public static Vector operator *(double y, PermutatedVectorView x) => x.Scale(y);
	}
}
