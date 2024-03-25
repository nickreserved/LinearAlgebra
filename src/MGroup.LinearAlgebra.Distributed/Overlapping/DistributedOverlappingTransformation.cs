using System;
using MGroup.LinearAlgebra.Vectors;
using MGroup.Environments;
using MGroup.LinearAlgebra.Matrices;

namespace MGroup.LinearAlgebra.Distributed.Overlapping
{
	public class DistributedOverlappingTransformation : ILinearTransformation
	{
		/// <summary>
		/// Multiplies a matrix (A) with the inputVector vector (x) and writes the result in the outputVector vector (y), such that 
		/// y = A * x. An equivalent operator y = opA(x) can also be used, instead of an explict matrix A. In any case, this 
		/// operation is done for a specific <see cref="MGroup.Environments.ComputeNode"/>.
		/// </summary>
		/// <param name="computeNodeID">
		/// The ID of the <see cref="MGroup.Environments.ComputeNode"/> for which the matrix-vector multiplication will be 
		/// performed. The matrix (A) or operator (opA), and the 2 vectors must all belong to this 
		/// <see cref="MGroup.Environments.ComputeNode"/>, wlthough this will not be checked explicitly.
		/// </param>
		/// <param name="input">
		/// The left-hand-side vector multiplied by the matrix A or used as inputVector for operator opA. Its dimensions must match 
		/// the number of columns of A or the dimension of the inputVector space of opA respectively.</param>
		/// <param name="output">
		/// The right-hand-side vector where the result of the multiplication will be written to. Its dimensions must match 
		/// the number of rows of A or the dimension of the outputVector space of opA respectively. On inputVector it is not guaranteed to
		/// be a zero vector, thus this method must make sure to zero it out, if needed.
		/// </param>
		public delegate void MultiplyLocalVector(int computeNodeID, Vector input, Vector output);

		private readonly MultiplyLocalVector multiplyMatrixVectorPerComputeNode;

		public DistributedOverlappingTransformation(DistributedOverlappingIndexer indexer,
			MultiplyLocalVector multiplyMatrixVectorPerComputeNode)
		{
			this.Indexer = indexer;
			this.Environment = indexer.Environment;
			this.multiplyMatrixVectorPerComputeNode = multiplyMatrixVectorPerComputeNode;
		}

		public IComputeEnvironment Environment { get; }

		public DistributedOverlappingIndexer Indexer { get; }

		public void Multiply(IMinimalImmutableVector inputVector, IMinimalMutableVector outputVector)
			=> Multiply((DistributedOverlappingVector) inputVector, (DistributedOverlappingVector) outputVector);

		public void Multiply(DistributedOverlappingVector inputVector, DistributedOverlappingVector outputVector)
		{
			if (!Indexer.IsCompatibleWith(inputVector.Indexer) ||
				!Indexer.IsCompatibleWith(outputVector.Indexer))
				throw IDistributedIndexer.IncompatibleIndexersException;

			Action<int> multiplyLocal = nodeID =>
			{
				Vector localX = inputVector.LocalVectors[nodeID];
				Vector localY = outputVector.LocalVectors[nodeID];
				multiplyMatrixVectorPerComputeNode(nodeID, localX, localY);
			};
			Environment.DoPerNode(multiplyLocal);
			#region debug
			//Environment.DoPerNodeSerially(multiplyLocal);
			#endregion

			outputVector.SumOverlappingEntries();
		}
	}
}
