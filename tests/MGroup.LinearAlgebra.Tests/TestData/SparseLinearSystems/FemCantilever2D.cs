namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;

	using MGroup.LinearAlgebra.Matrices;

	public class FemCantilever2D : FemCantileverBase
	{
		public FemCantilever2D(CartesianMesh2D mesh, double thickness)
			: base(2, mesh, mesh.LengthPerAxis[1], thickness)
		{
		}

		protected override Matrix ElementStiffness()
		{
			// See "A 99 Line Topology Optimization Code Written in MATLAB", 10.1007/s001580050176
			double v = PoissonRatio;
			double E = ElasticityModulus;
			double[] k = { 0.5 - v / 6.0, 0.125 + v / 8.0, -0.25 - v / 12.0, -0.125 + 3 * v / 8.0,
				-0.25 + v / 12.0, -0.125 - v / 8.0, v / 6.0, 0.125 - 3 * v / 8.0 }; // unique stiffness matrix entries
			var Ke = Matrix.CreateFromArray(new double[,]
			{
				 { k[0], k[1], k[2], k[3], k[4], k[5], k[6], k[7] },
				 { k[1], k[0], k[7], k[6], k[5], k[4], k[3], k[2] },
				 { k[2], k[7], k[0], k[5], k[6], k[3], k[4], k[1] },
				 { k[3], k[6], k[5], k[0], k[7], k[2], k[1], k[4] },
				 { k[4], k[5], k[6], k[7], k[0], k[1], k[2], k[3] },
				 { k[5], k[4], k[3], k[2], k[1], k[0], k[7], k[6] },
				 { k[6], k[3], k[4], k[1], k[2], k[7], k[0], k[5] },
				 { k[7], k[2], k[1], k[4], k[3], k[6], k[5], k[0] }
			});
			Ke.ScaleIntoThis(E / (1 - v * v));
			return Ke;
		}

		protected override double[] CalcKnownDisplacementsForNode(double[] coords)
		{
			double x = coords[0];
			double z = coords[1] - 0.5 * mesh.LengthPerAxis[1];
			(double u, double w) = base.CalcDisplacementsEulerBernoulli(x, z);
			return new double[] { u, w };
		}
	}
}
