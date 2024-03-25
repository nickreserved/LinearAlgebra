using System;
using System.Collections.Concurrent;

using MGroup.Environments;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Distributed.Overlapping
{
	public class DistributedOverlappingMatrix<TMatrix> : IMutableMatrix
		where TMatrix : class, IMutableMatrix
	{
		public DistributedOverlappingMatrix(DistributedOverlappingIndexer indexer)
		{
			Indexer = indexer;
			Environment = indexer.Environment;
		}

		public IComputeEnvironment Environment { get; }

		public DistributedOverlappingIndexer Indexer { get; }

		public ConcurrentDictionary<int, TMatrix> LocalMatrices { get; } = new ConcurrentDictionary<int, TMatrix>();
		public bool CheckForCompatibility { get; set; } = true;



		/// <summary>
		/// A partial linear combination between this and another otherMatrix.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public DistributedOverlappingMatrix<TMatrix> Axpy(DistributedOverlappingMatrix<TMatrix> otherMatrix, double otherCoefficient)
			=> Copy().AxpyIntoThis(otherMatrix, otherCoefficient);

		/// <summary>
		/// A partial linear combination between this and another otherMatrix.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public DistributedOverlappingMatrix<TMatrix> Axpy(IImmutableMatrix otherMatrix, double otherCoefficient)
			=> Copy().AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		/// <summary>
		/// A partial linear combination between this and another otherMatrix.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMutableMatrix IImmutableMatrix.Axpy(IImmutableMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis(otherMatrix, otherCoefficient);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> Add(DistributedOverlappingMatrix<TMatrix> otherMatrix)
			=> Axpy(otherMatrix, +1.0);

		public DistributedOverlappingMatrix<TMatrix> Add(IImmutableMatrix otherMatrix)
			=> Add((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMutableMatrix IImmutableMatrix.Add(IImmutableMatrix otherMatrix)
			=> Add(otherMatrix);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> Subtract(DistributedOverlappingMatrix<TMatrix> otherMatrix)
			=> Axpy(otherMatrix, -1.0);

		public DistributedOverlappingMatrix<TMatrix> Subtract(IImmutableMatrix otherMatrix)
			=> Subtract((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMutableMatrix IImmutableMatrix.Subtract(IImmutableMatrix otherMatrix)
			=> Subtract(otherMatrix);/*TODO: remove line when C#9*/


		/// <summary>
		/// A linear combination between this and another one otherMatrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherMatrix</param>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public DistributedOverlappingMatrix<TMatrix> LinearCombination(double thisCoefficient, DistributedOverlappingMatrix<TMatrix> otherMatrix, double otherCoefficient)
			=> Copy().LinearCombinationIntoThis(thisCoefficient, otherMatrix, otherCoefficient);

		/// <summary>
		/// A linear combination between this and another one otherMatrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherMatrix</param>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		public DistributedOverlappingMatrix<TMatrix> LinearCombination(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient)
			=> LinearCombination(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		/// <summary>
		/// A linear combination between this and another one otherMatrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherMatrix</param>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMutableMatrix IImmutableMatrix.LinearCombination(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient)
			=> LinearCombination(thisCoefficient, otherMatrix, otherCoefficient);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> Scale(double coefficient)
			=> Copy().ScaleIntoThis(coefficient);

		IMutableMatrix IImmutableMatrix.Scale(double coefficient)
			=> Scale(coefficient);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> AxpyIntoThis(DistributedOverlappingMatrix<TMatrix> otherMatrix, double otherCoefficient)
		{
			if (!Indexer.IsCompatibleWith(otherMatrix.Indexer)) throw IDistributedIndexer.IncompatibleIndexersException;
			Action<int> localOperation = nodeID =>
			{
				var thisSubdomainMatrix = LocalMatrices[nodeID];
				var otherSubdomainMatrix = otherMatrix.LocalMatrices[nodeID];
				thisSubdomainMatrix.AxpyIntoThis(otherSubdomainMatrix, otherCoefficient);
			};
			Environment.DoPerNode(localOperation);
			return this;
		}

		public DistributedOverlappingMatrix<TMatrix> AxpyIntoThis(IImmutableMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		IMutableMatrix IMutableMatrix.AxpyIntoThis(IImmutableMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> LinearCombinationIntoThis(double thisCoefficient, DistributedOverlappingMatrix<TMatrix> otherMatrix, double otherCoefficient)
		{
			if (!Indexer.IsCompatibleWith(otherMatrix.Indexer)) throw IDistributedIndexer.IncompatibleIndexersException;
			Action<int> localOperation = nodeID =>
			{
				var thisSubdomainMatrix = LocalMatrices[nodeID];
				var otherSubdomainMatrix = otherMatrix.LocalMatrices[nodeID];
				thisSubdomainMatrix.LinearCombinationIntoThis(thisCoefficient, otherSubdomainMatrix, otherCoefficient);
			};
			Environment.DoPerNode(localOperation);
			return this;
		}

		public DistributedOverlappingMatrix<TMatrix> LinearCombinationIntoThis(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient)
			=> LinearCombinationIntoThis(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		IMutableMatrix IMutableMatrix.LinearCombinationIntoThis(double thisCoefficient, IImmutableMatrix otherMatrix, double otherCoefficient)
			=> LinearCombinationIntoThis(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);/*TODO: remove line when C#9*/


		public void Multiply(DistributedOverlappingVector inputVector, DistributedOverlappingVector outputVector)
		{
			if (!Indexer.IsCompatibleWith(inputVector.Indexer) ||
				!Indexer.IsCompatibleWith(outputVector.Indexer))
				throw IDistributedIndexer.IncompatibleIndexersException;

			Action<int> multiplyLocal = nodeID =>
			{
				var localA = LocalMatrices[nodeID];
				var localX = inputVector.LocalVectors[nodeID];
				var localY = outputVector.LocalVectors[nodeID];
				localA.Multiply(localX, localY);
			};
			Environment.DoPerNode(multiplyLocal);

			outputVector.SumOverlappingEntries();
		}

		public void Multiply(IMinimalImmutableVector inputVector, IMinimalMutableVector outputVector)
			=> Multiply((DistributedOverlappingVector)inputVector, (DistributedOverlappingVector)outputVector);

		void ILinearTransformation.Multiply(IMinimalImmutableVector inputVector, IMinimalMutableVector outputVector)
			=> Multiply((DistributedOverlappingVector)inputVector, (DistributedOverlappingVector)outputVector);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> ScaleIntoThis(double coefficient)
		{
			Environment.DoPerNode(nodeID => LocalMatrices[nodeID].ScaleIntoThis(coefficient));
			return this;
		}

		IMutableMatrix IMutableMatrix.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);/*TODO: remove line when C#9*/


		public void Clear()
			=> Environment.DoPerNode(nodeID => LocalMatrices[nodeID].Clear());


		public DistributedOverlappingMatrix<TMatrix> Copy()
		{
			var copy = new DistributedOverlappingMatrix<TMatrix>(Indexer);
			Environment.DoPerNode(nodeID => copy.LocalMatrices[nodeID] = (TMatrix)LocalMatrices[nodeID].Copy());
			return copy;
		}

		IMutableMatrix IImmutableMatrix.Copy() => Copy();/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> CreateZero()
		{
			var copy = new DistributedOverlappingMatrix<TMatrix>(Indexer);
			Environment.DoPerNode(nodeID => copy.LocalMatrices[nodeID] = (TMatrix)LocalMatrices[nodeID].CreateZero());
			return copy;
		}

		IMutableMatrix IImmutableMatrix.CreateZero() => CreateZero();/*TODO: remove line when C#9*/


		bool Equals(DistributedOverlappingMatrix<TMatrix> otherMatrix, double tolerance = 1e-7)
		{
			if (!Indexer.IsCompatibleWith(otherMatrix.Indexer))
				return false;
			var flags = Environment.CalcNodeData(
					node => LocalMatrices[node].Equals(otherMatrix.LocalMatrices[node], tolerance));
			return Environment.AllReduceAnd(flags);
		}

		bool IImmutableMatrix.Equals(IImmutableMatrix otherMatrix, double tolerance)
		{
			if (otherMatrix is DistributedOverlappingMatrix<TMatrix> distributedMatrix)
				return Equals(distributedMatrix, tolerance);
			// This is wrong: They are not different, they are not comparable.
			// It is better to throw a Class Cast Exception
			else return false;
		}






		public void DoEntrywiseIntoThis(DistributedOverlappingMatrix<TMatrix> otherMatrix, Func<double, double, double> binaryOperation)
		{
			if (!Indexer.IsCompatibleWith(otherMatrix.Indexer)) throw IDistributedIndexer.IncompatibleIndexersException;
			Environment.DoPerNode(node => LocalMatrices[node].DoEntrywiseIntoThis(otherMatrix.LocalMatrices[node], binaryOperation));
		}

		public void DoEntrywiseIntoThis(IImmutableMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywiseIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, binaryOperation);


		public void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
			=> Environment.DoPerNode(node => LocalMatrices[node].DoToAllEntriesIntoThis(unaryOperation));


		public DistributedOverlappingMatrix<TMatrix> DoEntrywise(DistributedOverlappingMatrix<TMatrix> otherMatrix, Func<double, double, double> binaryOperation)
		{
			var matrix = Copy();
			matrix.DoEntrywiseIntoThis(otherMatrix, binaryOperation);
			return matrix;
		}

		public DistributedOverlappingMatrix<TMatrix> DoEntrywise(IImmutableMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywise((DistributedOverlappingMatrix<TMatrix>)otherMatrix, binaryOperation);

		IMutableMatrix IImmutableMatrix.DoEntrywise(IImmutableMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywise(otherMatrix, binaryOperation); /*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> DoToAllEntries(Func<double, double> unaryOperation)
		{
			var matrix = Copy();
			matrix.DoToAllEntriesIntoThis(unaryOperation);
			return matrix;
		}

		IMutableMatrix IImmutableMatrix.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation); /*TODO: remove line when C#9*/





		// -------- OPERATORS
		public static DistributedOverlappingMatrix<TMatrix> operator +(DistributedOverlappingMatrix<TMatrix> x, DistributedOverlappingMatrix<TMatrix> y) => x.Add(y);
		public static DistributedOverlappingMatrix<TMatrix> operator -(DistributedOverlappingMatrix<TMatrix> x, DistributedOverlappingMatrix<TMatrix> y) => x.Subtract(y);
		public static DistributedOverlappingMatrix<TMatrix> operator *(DistributedOverlappingMatrix<TMatrix> x, double y) => x.Scale(y);
		public static DistributedOverlappingMatrix<TMatrix> operator *(double y, DistributedOverlappingMatrix<TMatrix> x) => x.Scale(y);
	}
}
