using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative.StationaryPoint.GaussSeidel
{
	public class GaussSeidelIterationCsrSerial : IGaussSeidelIteration
	{
		private CsrMatrix matrix;
		private int[] diagonalOffsets;
		private bool inactive = false;

		public GaussSeidelIterationCsrSerial()
		{
		}

		public void GaussSeidelBackwardIteration(IVectorView rhsVector, IVector lhsVector)
		{
			CheckActive();

			if (lhsVector is Vector lhsDense && rhsVector is Vector rhsDense)
			{
				BackwardIteration(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices, diagonalOffsets,
					rhsDense.RawData, lhsDense.RawData);
			}
			else
			{
				var lhs = lhsVector.CopyToArray();
				var rhs = rhsVector.CopyToArray();
				BackwardIteration(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices, diagonalOffsets,
					rhs, lhs);
				lhsVector.CopyFrom(Vector.CreateFromArray(lhs));
			}
		}

		public void GaussSeidelForwardIteration(IVectorView rhsVector, IVector lhsVector)
		{
			CheckActive();

			if (lhsVector is Vector lhsDense && rhsVector is Vector rhsDense)
			{
				ForwardIteration(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices, diagonalOffsets,
					rhsDense.RawData, lhsDense.RawData);
			}
			else
			{
				var lhs = lhsVector.CopyToArray();
				var rhs = rhsVector.CopyToArray();
				ForwardIteration(matrix.NumRows, matrix.RawValues, matrix.RawRowOffsets, matrix.RawColIndices, diagonalOffsets,
					rhs, lhs);
				lhsVector.CopyFrom(Vector.CreateFromArray(lhs));
			}
		}

		public void Initialize(IMatrixView matrix)
		{
			if (matrix is CsrMatrix csrMatrix)
			{
				Preconditions.CheckSquare(csrMatrix);
				this.matrix = csrMatrix;
				diagonalOffsets = SparseArrays.LocateDiagonalOffsets(
					matrix.NumRows, csrMatrix.RawRowOffsets, csrMatrix.RawColIndices);
				inactive = false;
			}
			else
			{
				throw new InvalidSparsityPatternException(GetType().Name + " can be used only for matrices in CSR format." +
					"Consider using the general " + typeof(GaussSeidelIterationGeneral).Name + " or another implementation.");
			}
		}

		private static void BackwardIteration(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] lhs)
		{
			// Do not read the vector and each matrix row backward. It destroys caching. And in any case, we do csr_row * vector,
			// thus the order of operations for the dot product do not matter. What does matter is starting from the last row. 
			var n = matrixOrder;
			for (var i = n - 1; i >= 0; --i)
			{
				var sum = rhs[i];
				var rowStart = csrRowOffsets[i]; // inclusive
				var rowEnd = csrRowOffsets[i + 1]; // exclusive
				var diagOffset = diagOffsets[i];

				for (var k = rowStart; k < diagOffset; ++k)
				{
					sum -= csrValues[k] * lhs[csrColIndices[k]];
				}

				var diagEntry = csrValues[diagOffset];

				for (var k = diagOffset + 1; k < rowEnd; ++k)
				{
					sum -= csrValues[k] * lhs[csrColIndices[k]];
				}
				lhs[i] = sum / diagEntry;
			}
		}

		private static void BackwardIterationBasic(int matrixOrder, double[] csrValues, int[] csrRowOffsets,
			int[] csrColIndices, double[] rhs, double[] lhs)
		{
			var n = matrixOrder;
			for (var i = n - 1; i >= 0; --i)
			{
				var sum = rhs[i];
				double diagEntry = 0;

				var rowStart = csrRowOffsets[i]; // inclusive
				var rowEnd = csrRowOffsets[i + 1]; // exclusive
				for (var k = rowEnd - 1; k >= rowStart; --k)
				{
					var j = csrColIndices[k];
					if (j == i)
					{
						diagEntry = csrValues[k];
					}
					else
					{
						sum -= csrValues[k] * lhs[j];
					}
				}
				lhs[i] = sum / diagEntry;
			}
		}

		private static void ForwardIteration(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			int[] diagOffsets, double[] rhs, double[] lhs)
		{
			var n = matrixOrder;
			for (var i = 0; i < n; ++i)
			{
				var sum = rhs[i];
				var rowStart = csrRowOffsets[i]; // inclusive
				var rowEnd = csrRowOffsets[i + 1]; // exclusive
				var diagOffset = diagOffsets[i];

				for (var k = rowStart; k < diagOffset; ++k)
				{
					sum -= csrValues[k] * lhs[csrColIndices[k]];
				}

				var diagEntry = csrValues[diagOffset];

				for (var k = diagOffset + 1; k < rowEnd; ++k)
				{
					sum -= csrValues[k] * lhs[csrColIndices[k]];
				}
				lhs[i] = sum / diagEntry;
			}
		}

		private static void ForwardIterationBasic(int matrixOrder, double[] csrValues, int[] csrRowOffsets, int[] csrColIndices,
			double[] rhs, double[] lhs)
		{
			var n = matrixOrder;
			for (var i = 0; i < n; ++i)
			{
				var sum = rhs[i];
				double diagEntry = 0;

				var rowStart = csrRowOffsets[i]; // inclusive
				var rowEnd = csrRowOffsets[i + 1]; // exclusive
				for (var k = rowStart; k < rowEnd; ++k)
				{
					var j = csrColIndices[k];
					if (j == i)
					{
						diagEntry = csrValues[k];
					}
					else
					{
						sum -= csrValues[k] * lhs[j];
					}
				}
				lhs[i] = sum / diagEntry;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckActive()
		{
			if (inactive)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		public class Builder : IGaussSeidelIterationBuilder
		{
			public IGaussSeidelIteration Create() => new GaussSeidelIterationCsrSerial();
		}
	}
}
