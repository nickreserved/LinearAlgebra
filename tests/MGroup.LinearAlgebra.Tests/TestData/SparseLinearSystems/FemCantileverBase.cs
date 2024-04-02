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

	public abstract class FemCantileverBase
	{
		private readonly int dim;
		protected readonly CartesianMeshBase mesh;

		protected FemCantileverBase(int dim, CartesianMeshBase mesh, double thickness)
		{
			this.dim = dim;
			this.mesh = mesh;
			MomentOfInertia = thickness * Math.Pow(mesh.NumNodesPerAxis[1], 3) / 12.0;
			NumDofsAll = dim * mesh.NumNodesTotal;
			NumDofsFree = dim * (mesh.NumNodesPerAxis[0] - 1); // nodes with x = 0 are constrained
			NumDofsFree *= mesh.NumNodesTotal / mesh.NumNodesPerAxis[0];
		}

		public double[] ElementDensities { get; set; } = null;

		public double ElasticityModulus { get; set; } = 1.0;

		public double PoissonRatio { get; set; } = 0.3;

		public double DistributedLoad { get; set; } = 1.0;

		public double MomentOfInertia { get; }

		public int NumDofsAll { get; }

		public int NumDofsFree { get; }

		public (SparseMatrix A, Vector x, Vector b) CreateLinearSystem()
		{
			UpdateElementDensities();
			SparseMatrix K = AssembleGlobalMatrix();
			int[] freeDofs = FindFreeDofs();
			SparseMatrix Kff = K.ExtractSubmatrix(freeDofs, freeDofs);
			Vector Uf = CalcFreeDisplacements();
			Vector Ff = Kff * Uf;
			return (Kff, Uf, Ff);
		}

		protected (double u, double w) CalcDisplacementsEulerBernoulli(double x, double z)
		{
			double L = mesh.LengthPerAxis[0];
			double q = DistributedLoad;
			double E = ElasticityModulus;
			double I = MomentOfInertia;

			double w = -q / (24 * E * I) * (Math.Pow(x, 4) - 4 * L * Math.Pow(x, 3) + 6 * L * L * x * x);
			double rot = -q / (6 * E * I) * (Math.Pow(x, 3) - 3 * L * x * x + 3 * L * L * x);
			double u = -rot * z;

			return (u, w);
		}

		protected abstract Matrix ElementStiffness();

		protected abstract double[] CalcKnownDisplacementsForNode(double[] coords);

		private SparseMatrix AssembleGlobalMatrix()
		{
			Matrix Ke = ElementStiffness();
			int numElementDofs = Ke.NumRows;
			int[] elementDofsLocal = Enumerable.Range(0, numElementDofs).ToArray();

			int numAllDofs = NumDofsAll;
			var Kglob = new SparseMatrix(numAllDofs, numAllDofs);
			for (int e = 0; e < mesh.NumElementsTotal; e++)
			{
				int[] elementIdx = mesh.GetElementIndex(e);
				int[] elementDofsGlobal = GetElementDofsGlobal(elementIdx);
				double density = ElementDensities[e];
				Kglob.AddSubmatrix(density * Ke, elementDofsLocal, elementDofsGlobal, elementDofsLocal, elementDofsGlobal);
			}

			return Kglob;
		}

		/// <summary>
		/// The global indices of the element's dofs, in 0-based numbering.
		/// </summary>
		private int[] GetElementDofsGlobal(int[] elementIdx)
		{
			int[] nodeIds = mesh.GetNodeIdsOfElement(elementIdx);
			var globalDofs = new int[dim * nodeIds.Length];
			for (int i = 0; i < nodeIds.Length; ++i)
			{
				// 2D: 2*n, 2*n+1. 3D: 3*n, 3*n+1, 3*n+2
				for (int d = 0; d < dim; ++d)
				{
					globalDofs[dim * i + d] = dim * nodeIds[i] + d;
				}
			}

			return globalDofs;
		}

		private void UpdateElementDensities()
		{
			if (ElementDensities == null)
			{
				ElementDensities = new double[mesh.NumElementsTotal];
				for (int e = 0; e < mesh.NumElementsTotal; e++)
				{
					ElementDensities[e] = 1.0;
				}
			}
		}

		private int[] FindFreeDofs()
		{
			var freeDofs = new int[NumDofsFree];
			int pos = -1;
			for (int n = 0; n < mesh.NumNodesTotal; ++n)
			{
				int[] nodeIdx = mesh.GetNodeIndex(n);
				if (nodeIdx[0] == 0)
				{
					continue; // constrained nodes at x=0 (i=0);
				}

				// 2D: 2*n, 2*n+1. 3D: 3*n, 3*n+1, 3*n+2
				for (int d = 0; d < dim; ++d)
				{
					freeDofs[++pos] = dim * n + d;
				}
			}

			return freeDofs;
		}

		private Vector CalcFreeDisplacements()
		{
			var Uf = new double[NumDofsFree];
			int pos = -1;
			for (int n = 0; n < mesh.NumNodesTotal; ++n)
			{
				int[] nodeIdx = mesh.GetNodeIndex(n);
				if (nodeIdx[0] == 0)
				{
					continue; // constrained nodes at x=0 (i=0);
				}

				double[] coords = mesh.GetCoordsOfNode(nodeIdx);
				double[] displ = CalcKnownDisplacementsForNode(coords);

				for (int i = 0; i < displ.Length; ++i)
				{
					Uf[++pos] = displ[i];
				}
			}

			return Vector.CreateFromArray(Uf);
		}
	}
}
