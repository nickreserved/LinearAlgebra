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

		override public int Length { get { return Indices.Length; } }

		override public ref double this[int index] => ref Elements[Indices[index]];

		override public AbstractFullyPopulatedVector View(int fromIndex, int toIndex)   //TODO: on C#9 change covariant return type to PermutatedVectorView
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
		override public AbstractFullyPopulatedVector View(int[] indices)   //TODO: on C#9 change covariant return type to PermutatedVectorView
		{
			for (int i = 0; i < indices.Length; i++)
				indices[i] = Indices[indices[i]];
			return new PermutatedVectorView(Elements, indices);
		}



		/* Valid in C#9
		// ------------------- COVARIANT RETURN TYPE FROM AbstractFullyPopulatedVector

		new public PermutatedVectorView AddIntoThis(IMinimalImmutableVector otherVector) => (PermutatedVectorView)base.AddIntoThis(otherVector);
		new public PermutatedVectorView Clear() => (PermutatedVectorView)base.Clear();
		new public PermutatedVectorView CopyFrom(IMinimalImmutableVector otherVector) => (PermutatedVectorView)base.CopyFrom(otherVector);
		new public PermutatedVectorView DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => (PermutatedVectorView)base.DoToAllEntriesIntoThis(unaryOperation);
		new public PermutatedVectorView LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		new public PermutatedVectorView NegativeIntoThis() => (PermutatedVectorView)base.NegativeIntoThis();
		new public PermutatedVectorView ScaleIntoThis(double coefficient) => (PermutatedVectorView)base.ScaleIntoThis(coefficient);
		new public PermutatedVectorView SetAll(double value) => (PermutatedVectorView)base.SetAll(value);
		new public PermutatedVectorView SubtractIntoThis(IMinimalImmutableVector otherVector) => (PermutatedVectorView)base.SubtractIntoThis(otherVector);

		new public PermutatedVectorView AxpyIntoThis(SparseVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		new public PermutatedVectorView AxpyIntoThis(AbstractFullyPopulatedVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		new public PermutatedVectorView AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => (PermutatedVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);

		new public PermutatedVectorView DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		new public PermutatedVectorView DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		new public PermutatedVectorView DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => (PermutatedVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		*/

		// -------- OPERATORS FROM IMinimalImmutableVector

		public static Vector operator -(PermutatedVectorView x) => x.Negative();
		public static Vector operator +(PermutatedVectorView x, PermutatedVectorView y) => x.Add(y);
		public static Vector operator +(PermutatedVectorView x, IMinimalImmutableVector y) => x.Add(y);
		public static Vector operator +(IMinimalImmutableVector y, PermutatedVectorView x) => x.Add(y);
		public static Vector operator -(PermutatedVectorView x, PermutatedVectorView y) => x.Subtract(y);
		public static Vector operator -(PermutatedVectorView x, IMinimalImmutableVector y) => x.Subtract(y);
		public static Vector operator -(IMinimalImmutableVector y, PermutatedVectorView x) => (Vector)(x - y).NegativeIntoThis();
		public static double operator *(PermutatedVectorView x, PermutatedVectorView y) => x.DotProduct(y);
		public static double operator *(PermutatedVectorView x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, PermutatedVectorView y) => x.DotProduct(y);
		public static Vector operator *(PermutatedVectorView x, double y) => x.Scale(y);
		public static Vector operator *(double y, PermutatedVectorView x) => x.Scale(y);
	}
}
