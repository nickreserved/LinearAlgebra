using MGroup.LinearAlgebra.Triangulation;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Matrices.Builders;
using MGroup.LinearAlgebra.Tests.TestData;
using MGroup.LinearAlgebra.Tests.Utilities;
using MGroup.LinearAlgebra.Vectors;
using Xunit;

namespace MGroup.LinearAlgebra.Tests.Triangulation
{
    /// <summary>
    /// Tests for <see cref="CholeskyCSparseNet"/>.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public static class CholeskyCSparseNetTests
    {
        private static readonly MatrixComparer comparer = new MatrixComparer(1E-13);

        [Fact]
        private static void TestSystemSolution()
        {
            int order = SparsePosDef10by10.Order;
            var skyline = SkylineMatrix.CreateFromArrays(order, SparsePosDef10by10.SkylineValues,
                SparsePosDef10by10.SkylineDiagOffsets, true, true);
            var dok = DokSymmetric.CreateFromSparseMatrix(skyline);
            var b = new Vector(SparsePosDef10by10.Rhs);
            var xExpected = new Vector(SparsePosDef10by10.Lhs);

            (double[] cscValues, int[] cscRowIndices, int[] cscColOffsets) = dok.BuildSymmetricCscArrays(true);
            var factor = CholeskyCSparseNet.Factorize(order, cscValues.Length, cscValues, cscRowIndices, cscColOffsets);
            Vector xComputed = factor.SolveLinearSystem(b);
            comparer.AssertEqual(xExpected, xComputed);
        }
    }
}
