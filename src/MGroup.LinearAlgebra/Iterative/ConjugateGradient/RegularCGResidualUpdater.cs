using System;
using System.Collections.Generic;
using System.Text;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Iterative.ConjugateGradient
{
    /// <summary>
    /// Updates the residual vector according to the usual CG formula r = r - α * A*d. No corrections are applied.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public class RegularCGResidualUpdater : ICGResidualUpdater
    {
        /// <summary>
        /// See <see cref="ICGResidualUpdater.UpdateResidual(CGAlgorithm, IMinimalVector, out double)"/>
        /// </summary>
        public void UpdateResidual(CGAlgorithm cg, IMinimalVector residual, out double resDotRes)
        {
            // Normally the residual vector is updated as: r = r - α * A*d
            residual.AxpyIntoThis(cg.MatrixTimesDirection, -cg.StepSize);
            resDotRes = residual.DotProduct(residual);
        }
    }
}
