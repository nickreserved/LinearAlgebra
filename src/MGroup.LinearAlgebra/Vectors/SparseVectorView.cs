namespace MGroup.LinearAlgebra.Vectors
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class SparseVectorView : AbstractSparseVector
	{
		public SparseVectorView(int length, double[] values, int[] indices, int fromIndex) { Length = length; Values = values; Indices = Indices; FromIndex = FromIndex; }

		override public int FromIndex { get; }

		public override AbstractFullyPopulatedVector View(int fromIndex, int toIndex) => throw new NotImplementedException();
	}
}
