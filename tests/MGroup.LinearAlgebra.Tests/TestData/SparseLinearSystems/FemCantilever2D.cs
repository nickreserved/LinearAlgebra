namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;

	internal class FemCantilever2D
	{
		private readonly CartesianMesh2D mesh;

		internal FemCantilever2D(CartesianMesh2D mesh, double thickness)
		{
			this.mesh = mesh;
			NumDofsAll = 2 * (mesh.NumElementsX + 1) * (mesh.NumElementsY + 1);
			Thickness = thickness;
			MomentOfInertia = thickness * Math.Pow(mesh.LengthY, 3) / 12.0;
		}

		internal double[,] ElementDensities { get; set; } = null;

		internal double ElasticityModulus { get; set; } = 1.0;

		internal double PoissonRatio { get; set; } = 0.3;

		internal double DistributedLoad { get; set; } = 1.0;

		internal double MomentOfInertia { get; }

		internal int NumDofsAll { get; }

		internal double Thickness { get; }

		internal (SparseMatrix A, Vector x, Vector b) CreateLinearSystem()
		{
			UpdateElementDensities();
			SparseMatrix K = AssembleGlobalMatrix();
			int[] freeDofs = FindFreeDofs();
			SparseMatrix Kff = K.ExtractSubmatrix(freeDofs, freeDofs);
			Vector Uf = CalcFreeDisplacements();
			Vector Ff = Kff * Uf;
			return (Kff, Uf, Ff);
		}

		private SparseMatrix AssembleGlobalMatrix()
		{
			Matrix Ke = ElementStiffness();
			int numElementDofs = Ke.NumRows;
			int[] elementDofsLocal = Enumerable.Range(0, numElementDofs).ToArray();

			int numAllDofs = NumDofsAll;
			var Kglob = new SparseMatrix(numAllDofs, numAllDofs);
			(int minElementId, int maxElementId) = mesh.ElementIdRange;
			for (int e = minElementId; e <= maxElementId; e++)
			{
				int[] elementIdx = mesh.GetElementIndex(e);
				int[] elementDofsGlobal = GetElementDofsGlobal(elementIdx);
				double density = ElementDensities[elementIdx[0], elementIdx[1]];
				Kglob.AddSubmatrix(density * Ke, elementDofsLocal, elementDofsGlobal, elementDofsLocal, elementDofsGlobal);
			}

			return Kglob;
		}

		private Matrix ElementStiffness()
		{
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

		/// <summary>
		/// The global indices of the element's dofs, in 0-based numbering.
		/// </summary>
		private int[] GetElementDofsGlobal(int[] elementIdx)
		{
			int[] nodeIds = mesh.GetNodeIdsOfElement(elementIdx);
			var globalDofs = new int[2 * nodeIds.Length];
			for (int i = 0; i < nodeIds.Length; ++i)
			{
				globalDofs[2 * i] = 2 * nodeIds[i];
				globalDofs[2 * i + 1] = 2 * nodeIds[i] + 1;
			}
			return globalDofs;
		}

		private void UpdateElementDensities()
		{
			if (ElementDensities == null)
			{
				ElementDensities = new double[mesh.NumElementsX, mesh.NumElementsY];
				for (int j = 0; j < mesh.NumElementsY; ++j)
				{
					for (int i = 0; i < mesh.NumElementsX; ++i)
					{
						ElementDensities[i, j] = 1.0;
					}
				}
			}
		}

		private int CountFreeDofs() => 2 * ((mesh.NumNodesX - 1) * mesh.NumNodesY); // nodes with x = 0 are constrained

		private int[] FindFreeDofs()
		{
			var freeDofs = new int[CountFreeDofs()];
			int pos = -1;
			for (int n = 0; n < mesh.NumNodesX * mesh.NumNodesY; ++n)
			{
				int[] nodeIdx = mesh.GetNodeIndex(n);
				if (nodeIdx[0] == 0)
				{
					continue; // constrained nodes at x=0 (i=0);
				}

				freeDofs[++pos] = 2 * n;
				freeDofs[++pos] = 2 * n + 1;
			}

			return freeDofs;
		}

		private Vector CalcFreeDisplacements()
		{
			var Uf = new double[CountFreeDofs()];
			int pos = -1;
			for (int n = 0; n < mesh.NumNodesX * mesh.NumNodesY; ++n)
			{
				int[] nodeIdx = mesh.GetNodeIndex(n);
				if (nodeIdx[0] == 0)
				{
					continue; // constrained nodes at x=0 (i=0);
				}

				double[] coords = mesh.GetCoordsOfNode(nodeIdx);
				double[] displ = CalcKnownDisplacementsForNode(coords);
				Uf[++pos] = displ[0];
				Uf[++pos] = displ[1];
			}

			return Vector.CreateFromArray(Uf);
		}

		private double[] CalcKnownDisplacementsForNode(double[] coords)
		{
			double x = coords[0];
			double z = coords[1];
			double L = mesh.LengthX;
			double q = DistributedLoad;
			double E = ElasticityModulus;
			double I = MomentOfInertia;

			double w = -q / (24 * E * I) * (Math.Pow(x,4) - 4 * L * Math.Pow(x,3) + 6 * L * L * x * x);
			double rot = -q / (6 * E * I) * (Math.Pow(x, 3) - 3 * L * x * x + 3 * L * L * x);
			double u = -rot * z;

			return new double[] { u, w };
		}
	}
}
