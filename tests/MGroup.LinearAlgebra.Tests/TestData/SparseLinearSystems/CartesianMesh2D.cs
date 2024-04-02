namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;

	/// <summary>
	/// Best dof ordering when numElementsX >= numElementsY.
	/// </summary>
	public class CartesianMesh2D : CartesianMeshBase
	{
		public CartesianMesh2D(int numElementsX, int numElementsY, double lengthX, double lengthY)
			: base(2, new int[] { numElementsX, numElementsY }, new double[] { lengthX, lengthY })
		{
		}

		public override int[] GetNodeIdsOfElement(int[] elementIdx)
		{
			var nodeIds = new int[4];
			nodeIds[0] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] });
			nodeIds[1] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] });
			nodeIds[2] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] + 1 });
			nodeIds[3] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] + 1 });
			return nodeIds;
		}

		public override int GetElementId(int[] elementIdx)
		{
			CheckElementIndex(elementIdx);
			return numElementsPerAxis[1] * elementIdx[0] + elementIdx[1];
		}

		public override int[] GetElementIndex(int elementId)
		{
			CheckElementId(elementId);
			return new int[] { elementId / numElementsPerAxis[1], elementId % numElementsPerAxis[1] };
		}

		public override int GetNodeId(int[] nodeIdx)
		{
			CheckNodeIndex(nodeIdx);
			return numNodesPerAxis[1] * nodeIdx[0] + nodeIdx[1];
		}

		public override int[] GetNodeIndex(int nodeId)
		{
			CheckNodeId(nodeId);
			return new int[] { nodeId / numNodesPerAxis[1], nodeId % numNodesPerAxis[1] };
		}

	}
}
