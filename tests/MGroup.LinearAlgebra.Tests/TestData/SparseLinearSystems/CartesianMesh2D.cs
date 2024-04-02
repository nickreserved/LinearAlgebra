namespace MGroup.LinearAlgebra.Tests.TestData.SparseLinearSystems
{
	using System;

	internal class CartesianMesh2D
	{
		private readonly double dx;
		private readonly double dy;

		internal CartesianMesh2D(int numElementsX, int numElementsY, double lengthX, double lengthY)
		{
			NumElementsX = numElementsX;
			NumElementsY = numElementsY;
			LengthX = lengthX;
			LengthY = lengthY;
			NumNodesX = NumElementsX + 1;
			NumNodesY = NumElementsY + 1;
			dx = LengthX / numElementsX;
			dy = LengthY / numElementsY;
			ElementIdRange = (0, NumElementsX * NumElementsY - 1);
			NodeIdRange = (0, NumNodesX * NumNodesY - 1);
		}

		internal int NumElementsX { get; }

		internal int NumElementsY { get; }

		public double LengthX { get; }

		public double LengthY { get; }

		internal (int min, int max) ElementIdRange { get; }

		internal (int min, int max) NodeIdRange { get; }

		internal int NumNodesX { get; }

		internal int NumNodesY { get; }

		internal int[] GetNodeIdsOfElement(int[] elementIdx)
		{
			var nodeIds = new int[4];
			nodeIds[0] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] });
			nodeIds[1] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] });
			nodeIds[2] = GetNodeId(new int[] { elementIdx[0] + 1, elementIdx[1] + 1 });
			nodeIds[3] = GetNodeId(new int[] { elementIdx[0], elementIdx[1] + 1 });
			return nodeIds;
		}

		internal double[] GetCoordsOfNode(int[] nodeIdx)
		{
			double[] coords = { nodeIdx[0] * dx, nodeIdx[1] * dy };
			return coords;
		}

		internal int GetElementId(int[] elementIdx)
		{
			if ((elementIdx[0] < 0) || (elementIdx[0] >= NumElementsX) 
				|| (elementIdx[1] < 0) || (elementIdx[1] >= NumElementsY))
			{
				throw new ArgumentException("There is no such element");
			}

			return NumElementsY * elementIdx[0] + elementIdx[1];
		}

		internal int[] GetElementIndex(int elementId)
		{
			int maxElementId = NumElementsX * NumElementsY - 1;
			if ((elementId < 0) || (elementId > maxElementId))
			{
				throw new ArgumentException("There is no such element");
			}

			return new int[] { elementId / NumElementsY, elementId % NumElementsY };
		}

		internal int GetNodeId(int[] nodeIdx)
		{
			if ((nodeIdx[0] < 0) || (nodeIdx[0] >= NumNodesX) || (nodeIdx[1] < 0) || (nodeIdx[1] >= NumNodesY))
			{
				throw new ArgumentException("There is no such node");
			}

			return NumNodesY * nodeIdx[0] + nodeIdx[1];
		}

		internal int[] GetNodeIndex(int nodeId)
		{
			int maxNodeId = NumNodesX * NumNodesY - 1;
			if ((nodeId < 0) || (nodeId > maxNodeId))
			{
				throw new ArgumentException("There is no such node");
			}

			return new int[] { nodeId / NumNodesY, nodeId % NumNodesY };
		}
	}
}
