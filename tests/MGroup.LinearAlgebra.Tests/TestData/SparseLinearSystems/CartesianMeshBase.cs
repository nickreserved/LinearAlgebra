namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;
	using System.Text;

	using MGroup.LinearAlgebra.Tests.Utilities;

	public abstract class CartesianMeshBase
	{
		private readonly int dim;
		protected readonly int[] numElementsPerAxis;
		protected readonly int[] numNodesPerAxis;
		protected readonly double[] lengthPerAxis;
		protected readonly double[] nodeDistancePerAxis;

		protected CartesianMeshBase(int dim, int[] numElementsPerAxis, double[] lengthPerAxis)
		{
			this.dim = dim;
			this.numElementsPerAxis = numElementsPerAxis;
			this.lengthPerAxis = lengthPerAxis;
			numNodesPerAxis = new int[dim];
			nodeDistancePerAxis = new double[dim];
			NumElementsTotal = 1;
			NumNodesTotal = 1;
			for (int d = 0; d < dim; ++d)
			{
				numNodesPerAxis[d] = numElementsPerAxis[d] + 1;
				nodeDistancePerAxis[d] = lengthPerAxis[d] / numElementsPerAxis[d];
				NumElementsTotal *= numElementsPerAxis[d];
				NumNodesTotal *= numNodesPerAxis[d];
			}
		}

		public double[] LengthPerAxis => ArrayUtilities.Copy(lengthPerAxis);

		public int[] NumElementsPerAxis => ArrayUtilities.Copy(numElementsPerAxis);

		public int NumElementsTotal { get; }

		public int[] NumNodesPerAxis => ArrayUtilities.Copy(numNodesPerAxis);

		public int NumNodesTotal { get; }

		public double[] GetCoordsOfNode(int[] nodeIdx)
		{
			var coords = new double[dim];
			for (int d = 0; d < dim; ++d)
			{
				coords[d] = nodeIdx[d] * nodeDistancePerAxis[d];
			}

			return coords;
		}

		public abstract int[] GetNodeIdsOfElement(int[] elementIdx);

		public abstract int GetElementId(int[] elementIdx);

		public abstract int[] GetElementIndex(int elementId);

		public abstract int GetNodeId(int[] nodeIdx);

		public abstract int[] GetNodeIndex(int nodeId);

		protected void CheckElementId(int elementId)
		{
			if ((elementId < 0) || (elementId >= NumElementsTotal))
			{
				throw new ArgumentException(
					$"Invalid element id={elementId}. It must belong to the interval [0, {NumElementsTotal})");
			}
		}

		protected void CheckElementIndex(int[] elementIdx)
		{
			bool exists = true;
			for (int d = 0; d < dim; ++d)
			{
				if ((elementIdx[d] < 0) || (elementIdx[d] >= numElementsPerAxis[d]))
				{
					exists = false;
					break;
				}
			}

			if (!exists)
			{
				var msg = new StringBuilder("There is no element with index: (");
				msg.Append(elementIdx[0]);
				for (int d = 1; d < dim; ++d)
				{
					msg.Append(", ");
					msg.Append(elementIdx[d]);
				}
				msg.Append(")");
				throw new ArgumentException(msg.ToString());
			}

		}

		protected void CheckNodeId(int nodeId)
		{
			if ((nodeId < 0) || (nodeId >= NumNodesTotal))
			{
				throw new ArgumentException(
					$"Invalid node id={nodeId}. It must belong to the interval [0, {NumNodesTotal})");
			}
		}

		protected void CheckNodeIndex(int[] nodeIdx)
		{
			bool exists = true;
			for (int d = 0; d < dim; ++d)
			{
				if ((nodeIdx[d] < 0) || (nodeIdx[d] >= numNodesPerAxis[d]))
				{
					exists = false;
					break;
				}
			}

			if (!exists)
			{
				var msg = new StringBuilder("There is no node with index: (");
				msg.Append(nodeIdx[0]);
				for (int d = 1; d < dim; ++d)
				{
					msg.Append(", ");
					msg.Append(nodeIdx[d]);
				}
				msg.Append(")");
				throw new ArgumentException(msg.ToString());
			}
		}
	}
}
