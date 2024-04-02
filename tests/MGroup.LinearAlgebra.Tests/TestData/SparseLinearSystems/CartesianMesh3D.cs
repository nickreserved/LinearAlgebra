namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;

	/// <summary>
	/// Best dof ordering when Best dof ordering when numElementsX >= numElementsZ >= numElementsY.
	/// </summary>
	public class CartesianMesh3D : CartesianMeshBase
	{
		public CartesianMesh3D(int numElementsX, int numElementsY, int numElementsZ, 
			double lengthX, double lengthY, double lengthZ)
			: base(3, new int[] { numElementsX, numElementsY, numElementsZ }, new double[] { lengthX, lengthY, lengthZ })
		{
		}

		public override int[] GetNodeIdsOfElement(int[] elementIdx)
		{
			var nodeIds = new int[8];
			nodeIds[0] = GetNodeId(new int[] { elementIdx[0], elementIdx[1], elementIdx[2] });
			nodeIds[1] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1], elementIdx[2] });
			nodeIds[2] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] + 1, elementIdx[2] });
			nodeIds[3] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] + 1, elementIdx[2] });
			nodeIds[4] = GetNodeId(new int[] { elementIdx[0], elementIdx[1], elementIdx[2] + 1 });
			nodeIds[5] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1], elementIdx[2] + 1 });
			nodeIds[6] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] + 1, elementIdx[2] + 1 });
			nodeIds[7] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] + 1, elementIdx[2] + 1 });
			return nodeIds;
		}

		public override int GetElementId(int[] elementIdx)
		{
			CheckElementIndex(elementIdx);
			// y-major, z-medium, x-minor: id = iY + iZ * numElementsY + iX * NumElementsY * NumElementsZ
			return elementIdx[1] + elementIdx[2] * numElementsPerAxis[1] 
				+ elementIdx[0] * numElementsPerAxis[1] * numElementsPerAxis[2];
		}

		public override int[] GetElementIndex(int elementId)
		{
			CheckElementId(elementId);
			int numElementsPlane = numElementsPerAxis[1] * numElementsPerAxis[2];
			int mod = elementId % numElementsPlane;
			var idx = new int[3];
			idx[0] = elementId / numElementsPlane;
			idx[2] = mod / numElementsPerAxis[1];
			idx[1] = mod % numElementsPerAxis[1];
			return idx;
		}

		public override int GetNodeId(int[] nodeIdx)
		{
			CheckNodeIndex(nodeIdx);
			// y-major, z-medium, x-minor: id = iY + iZ * numNodesY + iX * NumNodesY * NumNodesZ
			return nodeIdx[1] + nodeIdx[2] * numNodesPerAxis[1] + nodeIdx[0] * numNodesPerAxis[1] * numNodesPerAxis[2];
		}

		public override int[] GetNodeIndex(int nodeId)
		{
			CheckNodeId(nodeId);
			int numNodesPlane = numNodesPerAxis[1] * numNodesPerAxis[2];
			int mod = nodeId % numNodesPlane;
			var idx = new int[3];
			idx[0] = nodeId / numNodesPlane;
			idx[2] = mod / numNodesPerAxis[1];
			idx[1] = mod % numNodesPerAxis[1];
			return idx;
		}

	}
}
