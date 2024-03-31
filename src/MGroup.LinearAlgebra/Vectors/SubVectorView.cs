namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Diagnostics;
	using MGroup.LinearAlgebra.Commons;

	using static MGroup.LinearAlgebra.LibrarySettings;

	public class SubVectorView : AbstractContiguousFullyPopulatedVector
	{
		public SubVectorView(double[] elements, int fromIndex, int toIndex)
		{
			Elements = elements;
			FromIndex = fromIndex;
			Length = toIndex - fromIndex;
			Debug.Assert(elements.Length >= toIndex);
		}

		override public double[] Elements { get; }
		override public int FromIndex { get; }
		override public int Length { get; }


		/* Valid in C#9
		// ------------------- COVARIANT RETURN TYPE FROM AbstractContiguousFullyPopulatedVector

		override public SubVectorView View(int fromIndex, int toIndex) => (SubVectorView)base.View(fromIndex, toIndex);
		override public PermutatedVectorView View(int[] indices) => (PermutatedVectorView)base.View(indices);
		override public SubVectorView AddIntoThis(IMinimalImmutableVector otherVector) => (SubVectorView)base.AddIntoThis(otherVector);
		override public SubVectorView Clear() => (SubVectorView)base.Clear();
		override public SubVectorView CopyFrom(IMinimalImmutableVector otherVector) => (SubVectorView)base.CopyFrom(otherVector);
		override public SubVectorView DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => (SubVectorView)base.DoToAllEntriesIntoThis(unaryOperation);
		override public SubVectorView LinearCombinationIntoThis(double thisCoefficient, AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (SubVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public SubVectorView LinearCombinationIntoThis(double thisCoefficient, IMinimalImmutableVector otherVector, double otherCoefficient) => (SubVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public SubVectorView NegativeIntoThis() => (SubVectorView)base.NegativeIntoThis();
		override public SubVectorView SetAll(double value) => (SubVectorView)base.SetAll(value);
		override public SubVectorView SubtractIntoThis(IMinimalImmutableVector otherVector) => (SubVectorView)base.SubtractIntoThis(otherVector);
		override public SubVectorView AxpyIntoThis(AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView AxpyIntoThis(SparseVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView AxpyIntoThis(IMinimalImmutableVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView DoEntrywiseIntoThis(IMinimalImmutableVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView ScaleIntoThis(double coefficient) => (SubVectorView)base.ScaleIntoThis(coefficient);
		*/


		// -------- OPERATORS FROM IMinimalImmutableVector

		public static Vector operator -(SubVectorView x) => x.Negative();
		public static Vector operator +(SubVectorView x, SubVectorView y) => x.Add(y);
		public static Vector operator +(SubVectorView x, IMinimalImmutableVector y) => x.Add(y);
		public static Vector operator +(IMinimalImmutableVector y, SubVectorView x) => x.Add(y);
		public static Vector operator -(SubVectorView x, SubVectorView y) => x.Subtract(y);
		public static Vector operator -(SubVectorView x, IMinimalImmutableVector y) => x.Subtract(y);
		public static Vector operator -(IMinimalImmutableVector y, SubVectorView x) => (Vector) (x - y).NegativeIntoThis();
		public static double operator *(SubVectorView x, SubVectorView y) => x.DotProduct(y);
		public static double operator *(SubVectorView x, IMinimalImmutableVector y) => x.DotProduct(y);
		public static double operator *(IMinimalImmutableVector x, SubVectorView y) => x.DotProduct(y);
		public static Vector operator *(SubVectorView x, double y) => x.Scale(y);
		public static Vector operator *(double y, SubVectorView x) => x.Scale(y);
	}
}
