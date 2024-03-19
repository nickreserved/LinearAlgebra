using System;

namespace MGroup.LinearAlgebra.Iterative.PreconditionedConjugateGradient
{
    public class RhsNormalizedConvergence : IPcgResidualConvergence
    {
        private double denominator;

        public double EstimateResidualNormRatio(PcgAlgorithmBase pcg) => Math.Sqrt(pcg.ResDotPrecondRes) / denominator;

        public void Initialize(PcgAlgorithmBase pcg) => denominator = Math.Sqrt(pcg.Rhs.Norm2());
    }
}
