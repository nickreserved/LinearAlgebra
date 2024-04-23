using System;
using System.Collections.Generic;
using System.Text;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Matrices.Operators
{
    public interface IMappingMatrix
    {
		int NumRows { get; }
		int NumColumns { get; }
		Vector Multiply(Vector vector, bool transposeThis = false);
        Matrix MultiplyRight(Matrix other, bool transposeThis = false);
    }
}
