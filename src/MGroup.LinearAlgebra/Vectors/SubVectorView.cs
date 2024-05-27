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
			Values = elements;
			FromIndex = fromIndex;
			Length = toIndex - fromIndex;
			Debug.Assert(elements.Length >= toIndex);
		}

		public override double[] Values { get; }
		public override int FromIndex { get; }
		public override int Length { get; }


		/* Valid in C#9
		// ------------------- COVARIANT RETURN TYPE FROM AbstractContiguousFullyPopulatedVector

		override public SubVectorView View(int fromIndex, int toIndex) => (SubVectorView)base.View(fromIndex, toIndex);
		override public PermutatedVectorView View(int[] indices) => (PermutatedVectorView)base.View(indices);
		override public SubVectorView AddIntoThis(IReadOnlyVector otherVector) => (SubVectorView)base.AddIntoThis(otherVector);
		override public SubVectorView Clear() => (SubVectorView)base.Clear();
		override public SubVectorView CopyFrom(IReadOnlyVector otherVector) => (SubVectorView)base.CopyFrom(otherVector);
		override public SubVectorView DoToAllEntriesIntoThis(Func<double, double> unaryOperation) => (SubVectorView)base.DoToAllEntriesIntoThis(unaryOperation);
		override public SubVectorView LinearCombinationIntoThis(double thisCoefficient, AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (SubVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public SubVectorView LinearCombinationIntoThis(double thisCoefficient, IReadOnlyVector otherVector, double otherCoefficient) => (SubVectorView)base.LinearCombinationIntoThis(thisCoefficient, otherVector, otherCoefficient);
		override public SubVectorView NegateIntoThis() => (SubVectorView)base.NegateIntoThis();
		override public SubVectorView SetAll(double value) => (SubVectorView)base.SetAll(value);
		override public SubVectorView SubtractIntoThis(IReadOnlyVector otherVector) => (SubVectorView)base.SubtractIntoThis(otherVector);
		override public SubVectorView AxpyIntoThis(AbstractContiguousFullyPopulatedVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView AxpyIntoThis(SparseVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView AxpyIntoThis(IReadOnlyVector otherVector, double otherCoefficient) => (SubVectorView)base.AxpyIntoThis(otherVector, otherCoefficient);
		override public SubVectorView DoEntrywiseIntoThis(SparseVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView DoEntrywiseIntoThis(AbstractFullyPopulatedVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView DoEntrywiseIntoThis(IReadOnlyVector otherVector, Func<double, double, double> binaryOperation) => (SubVectorView)base.DoEntrywiseIntoThis(otherVector, binaryOperation);
		override public SubVectorView ScaleIntoThis(double coefficient) => (SubVectorView)base.ScaleIntoThis(coefficient);
		*/


		// -------- OPERATORS FROM IReadOnlyVector

		public static Vector operator -(SubVectorView x) => x.Negate();
		public static Vector operator +(SubVectorView x, SubVectorView y) => x.Add(y);
		public static Vector operator +(SubVectorView x, IReadOnlyVector y) => x.Add(y);
		public static Vector operator +(IReadOnlyVector y, SubVectorView x) => x.Add(y);
		public static Vector operator -(SubVectorView x, SubVectorView y) => x.Subtract(y);
		public static Vector operator -(SubVectorView x, IReadOnlyVector y) => x.Subtract(y);
		public static Vector operator -(IReadOnlyVector x, SubVectorView y) => y.LinearCombination(-1, x, 1);
		public static double operator *(SubVectorView x, SubVectorView y) => x.DotProduct(y);
		public static double operator *(SubVectorView x, IReadOnlyVector y) => x.DotProduct(y);
		public static double operator *(IReadOnlyVector x, SubVectorView y) => x.DotProduct(y);
		public static Vector operator *(SubVectorView x, double y) => x.Scale(y);
		public static Vector operator *(double y, SubVectorView x) => x.Scale(y);
	}
}
