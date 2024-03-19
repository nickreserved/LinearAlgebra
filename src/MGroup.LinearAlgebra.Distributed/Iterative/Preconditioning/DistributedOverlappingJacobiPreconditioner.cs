using System;
using System.Collections.Generic;
using MGroup.LinearAlgebra.Vectors;
using MGroup.Environments;
using MGroup.LinearAlgebra.Distributed.Overlapping;
using MGroup.LinearAlgebra.Iterative.Preconditioning;

//TODOMPI: Needs testing
namespace MGroup.LinearAlgebra.Distributed.IterativeMethods.Preconditioning
{
	public class DistributedOverlappingJacobiPreconditioner : IPreconditioner
	{
		private readonly IComputeEnvironment environment;

		/// <summary>
		/// Creates a Jacobi preconditioner in distributed environment.
		/// </summary>
		/// <param name="environment">
		/// The computing environment that will be used for the operations during this constructor and during 
		/// <see cref="Apply(IImmutableVector, IMutableVector)"/>.
		/// </param>
		/// <param name="diagonal">
		/// A distributed vector that contains the diagonal entries of each local matrix that corresponds to a 
		/// <see cref="ComputeNode"/> of the <paramref name="environment"/>. If an entry is overlapping, namely if it exists
		/// in many neighboring local diagonal vectors, then its value must be the same in all these local vectors.
		/// </param>
		public DistributedOverlappingJacobiPreconditioner(IComputeEnvironment environment, DistributedOverlappingVector diagonal)
		{
			this.environment = environment;
			this.Indexer = diagonal.Indexer;

			Func<int, Vector> invertDiagonal = nodeID => diagonal.LocalVectors[nodeID].DoToAllEntries(x => 1 / x);
			this.LocalInverseDiagonals = environment.CalcNodeData(invertDiagonal);
		}

		public IDistributedIndexer Indexer { get; }

		public Dictionary<int, Vector> LocalInverseDiagonals { get; }


		public void Apply(DistributedOverlappingVector rhsVector, DistributedOverlappingVector lhsVector)
		{
			if (!Indexer.IsCompatibleWith(rhsVector.Indexer) ||
				!Indexer.IsCompatibleWith(lhsVector.Indexer))
				throw IDistributedIndexer.IncompatibleIndexersException;

			Action<int> multiplyLocal = nodeID =>
			{
				Vector localX = rhsVector.LocalVectors[nodeID];
				Vector localY = lhsVector.LocalVectors[nodeID];
				Vector localDiagonal = this.LocalInverseDiagonals[nodeID];
				localY.CopyFrom(localX);
				localY.MultiplyEntrywiseIntoThis(localDiagonal);
			};
			environment.DoPerNode(multiplyLocal);

			//TODOMPI: do we need to call lhsVector.SumOverlappingEntries() here? Is this need covered by the fact that 
			//      LocalInverseDiagonals already have the total stiffnesses?
		}

		public void Apply(IImmutableVector rhsVector, IMutableVector lhsVector)
			=> Apply((DistributedOverlappingVector) rhsVector, (DistributedOverlappingVector) lhsVector);
	}
}
