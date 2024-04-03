#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	public class FemCantilever3D : FemCantileverBase
	{
		public FemCantilever3D(CartesianMesh3D mesh)
			: base(3, mesh, mesh.LengthPerAxis[2], mesh.LengthPerAxis[1])
		{
		}

		protected override Matrix ElementStiffness()
		{
			// See "An efficient 3D topology optimization code written in Matlab", 10.1007/s00158-014-1107-x
			double v = PoissonRatio;
			double E = ElasticityModulus;

			// Copied from Matlab code
			var A = Matrix.CreateFromArray(new double[,]
			{
				{ 32, 6, -8,  6,  -6, 4, 3, -6, -10,   3, -3, -3, -4, -8 },
				{-48, 0,  0, -24, 24, 0, 0,  0,  12, -12,  0, 12, 12, 12 },
			});
			Vector kTemp = A.Transpose() * Vector.CreateFromArray(new double[] { 1, v });
			kTemp.ScaleIntoThis(1.0 / 72.0);

			// The indices are 1-based so I extend the k array to avoid reindexing
			var k = Vector.CreateZero(kTemp.Length + 1);
			k.CopySubvectorFrom(1, kTemp, 0, kTemp.Length);

			var K1 = Matrix.CreateFromArray(new double[,]
			{
				{ k[1], k[2], k[2], k[3], k[5], k[5] },
				{ k[2], k[1], k[2], k[4], k[6], k[7] },
				{ k[2], k[2], k[1], k[4], k[7], k[6] },
				{ k[3], k[4], k[4], k[1], k[8], k[8] },
				{ k[5], k[6], k[7], k[8], k[1], k[2] },
				{ k[5], k[7], k[6], k[8], k[2], k[1] },
			});

			var K2 = Matrix.CreateFromArray(new double[,]
			{
				{ k[9] , k[8] , k[12], k[6] , k[4] , k[7]  },
				{ k[8] , k[9] , k[12], k[5] , k[3] , k[5]  },
				{ k[10], k[10], k[13], k[7] , k[4] , k[6]  },
				{ k[6] , k[5] , k[11], k[9] , k[2] , k[10] },
				{ k[4] , k[3] , k[5] , k[2] , k[9] , k[12] },
				{ k[11], k[4] , k[6] , k[12], k[10], k[13] },
			});

			var K3 = Matrix.CreateFromArray(new double[,]
			{
				{ k[6] , k[7] , k[4] , k[9] , k[12], k[8]  },
				{ k[7] , k[6] , k[4] , k[10], k[13], k[10] },
				{ k[5] , k[5] , k[3] , k[8] , k[12], k[9]  },
				{ k[9] , k[10], k[2] , k[6] , k[11], k[5]  },
				{ k[12], k[13], k[10], k[11], k[6] , k[4]  },
				{ k[2] , k[12], k[9] , k[4] , k[5] , k[3]  },
			});

			var K4 = Matrix.CreateFromArray(new double[,]
			{
				{ k[14], k[11], k[11], k[13], k[10], k[10] },
				{ k[11], k[14], k[11], k[12], k[9] , k[8]  },
				{ k[11], k[11], k[14], k[12], k[8] , k[9]  },
				{ k[13], k[12], k[12], k[14], k[7] , k[7]  },
				{ k[10], k[9] , k[8] , k[7] , k[14], k[11] },
				{ k[10], k[8] , k[9] , k[7] , k[11], k[14] },
			});

			var K5 = Matrix.CreateFromArray(new double[,]
			{
				{ k[1], k[2] , k[8] , k[3], k[5] , k[4]  },
				{ k[2], k[1] , k[8] , k[4], k[6] , k[11] },
				{ k[8], k[8] , k[1] , k[5], k[11], k[6]  },
				{ k[3], k[4] , k[5] , k[1], k[8] , k[2]  },
				{ k[5], k[6] , k[11], k[8], k[1] , k[8]  },
				{ k[4], k[11], k[6] , k[2], k[8] , k[1]  },
			});

			var K6 = Matrix.CreateFromArray(new double[,]
			{
				{ k[14], k[11], k[7] , k[13], k[10], k[12] },
				{ k[11], k[14], k[7] , k[12], k[9] , k[2]  },
				{ k[7] , k[7] , k[14], k[10], k[2] , k[9]  },
				{ k[13], k[12], k[10], k[14], k[7] , k[11] },
				{ k[10], k[9] , k[2] , k[7] , k[14], k[7]  },
				{ k[12], k[2] , k[9] , k[11], k[7] , k[14] },
			});

			Matrix Ke = Matrix.Concatenate(new Matrix[,]
			{
				{ K1, K2, K3, K4 },
				{ K2.Transpose(), K5, K6, K3.Transpose() },
				{ K3.Transpose(), K6, K5.Transpose(), K2.Transpose() },
				{ K4, K3, K2, K1.Transpose() },
			});
			Ke.ScaleIntoThis(E / ((v + 1) * (1 - 2 * v)));
			return Ke;
		}

		protected override double[] CalcKnownDisplacementsForNode(double[] coords)
		{
			double x = coords[0];
			double z = coords[2] - 0.5 * mesh.LengthPerAxis[2];
			(double u, double w) = base.CalcDisplacementsEulerBernoulli(x, z);
			return new double[] { u, 0, w };
		}
	}
}
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
