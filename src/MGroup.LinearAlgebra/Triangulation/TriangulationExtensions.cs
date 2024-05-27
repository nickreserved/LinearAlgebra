using System;
using System.Collections.Generic;
using System.Text;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Triangulation
{
    public static class TriangulationExtensions
    {
		/// <summary>
		/// Solves the linear system A * x = b, where A is the original matrix (before the factorization), 
		/// b = <paramref name="rhsVector"/> and x is the solution vector, which will be returned.
		/// </summary>
		/// <param name="rhsVector">
		/// The right hand side vector. Its <see cref="IReadOnlyVector.Length"/> must be equal to 
		/// <see cref="Matrices.IBounded2D.NumRows"/> of the original matrix A.
		/// </param>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if the length of <paramref name="rhsVector"/> is different than <see cref="Matrices.IBounded2D.NumRows"/> 
		/// of the original matrix A.
		/// </exception>
		public static Vector SolveLinearSystem(this ITriangulation triangulation, Vector rhsVector)
        {
            var solution = new Vector(triangulation.Order);
            triangulation.SolveLinearSystem(rhsVector, solution);
            return solution;
        }

        public static Matrix SolveLinearSystems(this LdlSkyline triangulation, Matrix rhsMatrix)
        {
            var solution = Matrix.CreateZero(triangulation.Order, rhsMatrix.NumColumns);
            triangulation.SolveLinearSystems(rhsMatrix, solution);
            return solution;
        }
    }
}
