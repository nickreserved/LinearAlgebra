using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative.StationaryPoint.GaussSeidel
{
	public class GaussSeidelIterationGeneral : IGaussSeidelIteration
	{
		private IMatrixView matrix;
		private bool inactive = true;

		public GaussSeidelIterationGeneral()
		{
		}


		public void GaussSeidelBackwardIteration(IVectorView rhsVector, IVector lhsVector)
		{
			CheckActive();

			var x = lhsVector;
			var b = rhsVector;
			Preconditions.CheckSquareLinearSystemDimensions(matrix, x, b);
			var n = matrix.NumRows;
			for (var i = n - 1; i >= 0; --i)
			{
				var sum = b[i];
				int j;
				for (j = 0; j < i; ++j)
				{
					sum -= matrix[i, j] * x[j];
				}

				var diagEntry = matrix[i, i];

				for (j = i + 1; j < n; ++j)
				{
					sum -= matrix[i, j] * x[j];
				}

				x.Set(i, sum / diagEntry);
			}
		}

		public void GaussSeidelForwardIteration(IVectorView rhsVector, IVector lhsVector)
		{
			CheckActive();

			var x = lhsVector;
			var b = rhsVector;
			Preconditions.CheckSquareLinearSystemDimensions(matrix, x, b);
			var n = matrix.NumRows;
			for (var i = 0; i < n; ++i)
			{
				var sum = b[i];
				int j;
				for (j = 0; j < i; ++j)
				{
					sum -= matrix[i, j] * x[j];
				}

				var diagEntry = matrix[i, i];

				for (j = i + 1; j < n; ++j)
				{
					sum -= matrix[i, j] * x[j];
				}

				x.Set(i, sum / diagEntry);
			}
		}

		public void Initialize(IMatrixView matrix)
		{
			Preconditions.CheckSquare(matrix);
			this.matrix = matrix;
			inactive = false;
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
			public IGaussSeidelIteration Create() => new GaussSeidelIterationGeneral();
		}
	}
}
