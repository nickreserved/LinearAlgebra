namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;

	/// <summary>
	/// See "The Numerical Solution of Poisson Equation with Dirichlet Boundary Conditions" (10.4236/jamp.2021.912194) 
	/// https://www.scirp.org/journal/paperinformation?paperid=113731
	/// </summary>
	public class FdmPoisson2D
	{
		private const double sideLength = 1.0;
		protected readonly CartesianMesh2D mesh;

		public FdmPoisson2D(int numPointsPerAxis)
		{
			this.mesh = new CartesianMesh2D(numPointsPerAxis - 1, numPointsPerAxis - 1, sideLength, sideLength);
		}

		public (SparseMatrix A, Vector x, Vector b) CreateLinearSystem()
		{
			SparseMatrix A = CalcMatrixAll();
			int[] knowns = FindKnowns();
			int[] unknowns = FindUnknowns(knowns);
			int[] allRows = Enumerable.Range(0, A.NumRows).ToArray();
			SparseMatrix Au = A.ExtractSubmatrix(allRows, unknowns);
			Vector x = CalcKnownSolution();
			Vector xu = x.GetSubvector(unknowns);
			Vector b = Au * xu;
			return (Au, xu, b);

			// In FD we would solve Au * xu = b = f - Ak * xk
			SparseMatrix Ak = A.ExtractSubmatrix(allRows, knowns);
			Vector xk = x.GetSubvector(knowns);
			Vector f = CalcRhs().GetSubvector(unknowns);
		}

		private SparseMatrix CalcMatrixAll()
		{
			int n = mesh.NumNodesPerAxis[0]; // equal to other axis
			int numEquations = (n - 2) * (n - 2);
			int numNodes = n * n;
			var A = new SparseMatrix(numEquations, numNodes);
			int eq = 0;
			for (int i = 1; i < n - 1; ++i)
			{
				for (int j = 1; j < n - 1; ++j)
				{
					int cen = mesh.GetNodeId(new int[] { i, j });
					int nor = mesh.GetNodeId(new int[] { i, j + 1 });
					int sou = mesh.GetNodeId(new int[] { i, j - 1 });
					int wes = mesh.GetNodeId(new int[] { i - 1, j });
					int eas = mesh.GetNodeId(new int[] { i + 1, j });
					A[eq, cen] = 4;
					A[eq, nor] = -1;
					A[eq, sou] = -1;
					A[eq, wes] = -1;
					A[eq, eas] = -1;
					++eq;
				}
			}

			return A;
		}

		private Vector CalcKnownSolution()
		{
			int n = mesh.NumNodesPerAxis[0]; // equal to other axis
			var x = Vector.CreateZero(n * n);
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < n; ++j)
				{
					var nodeIdx = new int[] { i, j };
					int nodeId = mesh.GetNodeId(nodeIdx);
					double[] coords = mesh.GetCoordsOfNode(nodeIdx);
					x[nodeId] = Math.Exp(-coords[0] * coords[1]);
				}
			}

			return x;
		}

		private Vector CalcRhs()
		{
			int n = mesh.NumNodesPerAxis[0]; // equal to other axis
			var fuk = Vector.CreateZero(n * n); // Also stores 0 for known quantities
			double h = sideLength / (n - 1);
			for (int i = 1; i < n - 1; ++i)
			{
				for (int j = 1; j < n - 1; ++j)
				{
					var nodeIdx = new int[] { i, j };
					int nodeId = mesh.GetNodeId(nodeIdx);
					double[] coords = mesh.GetCoordsOfNode(nodeIdx);
					double x = coords[0];
					double y = coords[1];

					fuk[nodeId] = -Math.Exp(-x * y) * (x * x + y * y) * h;
				}
			}

			return fuk;
		}

		private int[] FindKnowns()
		{
			int n = mesh.NumNodesPerAxis[0]; // equal to other axis
			var knowns = new SortedSet<int>();

			// North
			for (int i = 0; i < n; ++i)
			{
				knowns.Add(mesh.GetNodeId(new int[] {i, 0}));
			}

			// South
			for (int i = 0; i < n; ++i)
			{
				knowns.Add(mesh.GetNodeId(new int[] { i, n - 1 }));
			}

			// West
			for (int j = 0; j < n; ++j)
			{
				knowns.Add(mesh.GetNodeId(new int[] { 0, j }));
			}

			// East
			for (int j = 0; j < n; ++j)
			{
				knowns.Add(mesh.GetNodeId(new int[] { n - 1, j }));
			}

			return knowns.ToArray();
		}

		private int[] FindUnknowns(int[] knowns)
		{
			int numNodes = mesh.NumNodesPerAxis[0] * mesh.NumNodesPerAxis[1];
			var unknowns = new SortedSet<int>(Enumerable.Range(0, numNodes));
			unknowns.ExceptWith(knowns);
			return unknowns.ToArray();
		}
	}
}
