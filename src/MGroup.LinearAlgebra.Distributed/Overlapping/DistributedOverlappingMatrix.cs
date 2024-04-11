using System;
using System.Collections.Concurrent;

using MGroup.Environments;
using MGroup.LinearAlgebra.Matrices;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Distributed.Overlapping
{
	public class DistributedOverlappingMatrix<TMatrix> : IMinimalMatrix
		where TMatrix : class, IMinimalMatrix
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

		public int NumRows => throw new NotImplementedException();

		public int NumColumns => throw new NotImplementedException();



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
		public DistributedOverlappingMatrix<TMatrix> Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> Copy().AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		/// <summary>
		/// A partial linear combination between this and another otherMatrix.
		/// </summary>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMatrix IMinimalReadOnlyMatrix.Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis(otherMatrix, otherCoefficient);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> Add(DistributedOverlappingMatrix<TMatrix> otherMatrix)
			=> Axpy(otherMatrix, +1.0);

		public DistributedOverlappingMatrix<TMatrix> Add(IMinimalReadOnlyMatrix otherMatrix)
			=> Add((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMinimalMatrix IMinimalReadOnlyMatrix.Add(IMinimalReadOnlyMatrix otherMatrix)
			=> Add(otherMatrix);/*TODO: remove line when C#9*/


		public DistributedOverlappingMatrix<TMatrix> Subtract(DistributedOverlappingMatrix<TMatrix> otherMatrix)
			=> Axpy(otherMatrix, -1.0);

		public DistributedOverlappingMatrix<TMatrix> Subtract(IMinimalReadOnlyMatrix otherMatrix)
			=> Subtract((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMinimalMatrix IMinimalReadOnlyMatrix.Subtract(IMinimalReadOnlyMatrix otherMatrix)
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
		public DistributedOverlappingMatrix<TMatrix> LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> LinearCombination(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		/// <summary>
		/// A linear combination between this and another one otherMatrix.
		/// </summary>
		/// <param name="thisCoefficient">A scalar as coefficient to this otherMatrix</param>
		/// <param name="otherMatrix">A otherMatrix with the same number of elements with this otherMatrix</param>
		/// <param name="otherCoefficient">A scalar as coefficient to <paramref name="otherMatrix"/></param>
		/// <returns>thisMatrix * <paramref name="thisCoefficient"/> + <paramref name="otherMatrix"/> * <paramref name="otherCoefficient"/></returns>
		IMinimalMatrix IMinimalReadOnlyMatrix.LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> LinearCombination(thisCoefficient, otherMatrix, otherCoefficient);


		public DistributedOverlappingMatrix<TMatrix> Scale(double coefficient)
			=> Copy().ScaleIntoThis(coefficient);

		IMinimalMatrix IMinimalReadOnlyMatrix.Scale(double coefficient)
			=> Scale(coefficient);


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

		public DistributedOverlappingMatrix<TMatrix> AxpyIntoThis(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		IMinimalMatrix IMinimalMatrix.AxpyIntoThis(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> AxpyIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);


		public DistributedOverlappingMatrix<TMatrix> AddIntoThis(DistributedOverlappingMatrix<TMatrix> otherMatrix) => AxpyIntoThis(otherMatrix, 1);

		public DistributedOverlappingMatrix<TMatrix> AddIntoThis(IMinimalReadOnlyMatrix otherMatrix) => AddIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMinimalMatrix IMinimalMatrix.AddIntoThis(IMinimalReadOnlyMatrix otherMatrix) => AddIntoThis(otherMatrix);


		public DistributedOverlappingMatrix<TMatrix> SubtractIntoThis(DistributedOverlappingMatrix<TMatrix> otherMatrix) => AxpyIntoThis(otherMatrix, -1);

		public DistributedOverlappingMatrix<TMatrix> SubtractIntoThis(IMinimalReadOnlyMatrix otherMatrix) => SubtractIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix);

		IMinimalMatrix IMinimalMatrix.SubtractIntoThis(IMinimalReadOnlyMatrix otherMatrix) => SubtractIntoThis(otherMatrix);


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

		public DistributedOverlappingMatrix<TMatrix> LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> LinearCombinationIntoThis(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);

		IMinimalMatrix IMinimalMatrix.LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
			=> LinearCombinationIntoThis(thisCoefficient, (DistributedOverlappingMatrix<TMatrix>)otherMatrix, otherCoefficient);


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
				localA.MultiplyIntoThis(localX, localY);
			};
			Environment.DoPerNode(multiplyLocal);

			outputVector.SumOverlappingEntries();
		}

		public void Multiply(IMinimalReadOnlyVector inputVector, IMinimalVector outputVector)
			=> Multiply((DistributedOverlappingVector)inputVector, (DistributedOverlappingVector)outputVector);

		void ILinearTransformation.MultiplyIntoThis(IMinimalReadOnlyVector inputVector, IMinimalVector outputVector)
			=> Multiply((DistributedOverlappingVector)inputVector, (DistributedOverlappingVector)outputVector);


		public DistributedOverlappingMatrix<TMatrix> ScaleIntoThis(double coefficient)
		{
			Environment.DoPerNode(nodeID => LocalMatrices[nodeID].ScaleIntoThis(coefficient));
			return this;
		}

		IMinimalMatrix IMinimalMatrix.ScaleIntoThis(double coefficient) => ScaleIntoThis(coefficient);


		public void Clear()
			=> Environment.DoPerNode(nodeID => LocalMatrices[nodeID].Clear());


		public DistributedOverlappingMatrix<TMatrix> Copy()
		{
			var copy = new DistributedOverlappingMatrix<TMatrix>(Indexer);
			Environment.DoPerNode(nodeID => copy.LocalMatrices[nodeID] = (TMatrix)LocalMatrices[nodeID].Copy());
			return copy;
		}

		IMinimalMatrix IMinimalReadOnlyMatrix.Copy() => Copy();


		public DistributedOverlappingMatrix<TMatrix> CreateZero()
		{
			var copy = new DistributedOverlappingMatrix<TMatrix>(Indexer);
			Environment.DoPerNode(nodeID => copy.LocalMatrices[nodeID] = (TMatrix)LocalMatrices[nodeID].CreateZeroWithSameFormat());
			return copy;
		}

		IMinimalMatrix IMinimalReadOnlyMatrix.CreateZeroWithSameFormat() => CreateZero();


		bool Equals(DistributedOverlappingMatrix<TMatrix> otherMatrix, double tolerance = 1e-7)
		{
			if (!Indexer.IsCompatibleWith(otherMatrix.Indexer))
				return false;
			var flags = Environment.CalcNodeData(
					node => LocalMatrices[node].Equals(otherMatrix.LocalMatrices[node], tolerance));
			return Environment.AllReduceAnd(flags);
		}

		bool IMinimalReadOnlyMatrix.Equals(IMinimalReadOnlyMatrix otherMatrix, double tolerance)
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

		public void DoEntrywiseIntoThis(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywiseIntoThis((DistributedOverlappingMatrix<TMatrix>)otherMatrix, binaryOperation);


		public void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
			=> Environment.DoPerNode(node => LocalMatrices[node].DoToAllEntriesIntoThis(unaryOperation));


		public DistributedOverlappingMatrix<TMatrix> DoEntrywise(DistributedOverlappingMatrix<TMatrix> otherMatrix, Func<double, double, double> binaryOperation)
		{
			var matrix = Copy();
			matrix.DoEntrywiseIntoThis(otherMatrix, binaryOperation);
			return matrix;
		}

		public DistributedOverlappingMatrix<TMatrix> DoEntrywise(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywise((DistributedOverlappingMatrix<TMatrix>)otherMatrix, binaryOperation);

		IMinimalMatrix IMinimalReadOnlyMatrix.DoEntrywise(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation)
			=> DoEntrywise(otherMatrix, binaryOperation);


		public DistributedOverlappingMatrix<TMatrix> DoToAllEntries(Func<double, double> unaryOperation)
		{
			var matrix = Copy();
			matrix.DoToAllEntriesIntoThis(unaryOperation);
			return matrix;
		}

		IMinimalMatrix IMinimalReadOnlyMatrix.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);





		// -------- OPERATORS
		public static DistributedOverlappingMatrix<TMatrix> operator +(DistributedOverlappingMatrix<TMatrix> x, DistributedOverlappingMatrix<TMatrix> y) => x.Add(y);
		public static DistributedOverlappingMatrix<TMatrix> operator -(DistributedOverlappingMatrix<TMatrix> x, DistributedOverlappingMatrix<TMatrix> y) => x.Subtract(y);
		public static DistributedOverlappingMatrix<TMatrix> operator *(DistributedOverlappingMatrix<TMatrix> x, double y) => x.Scale(y);
		public static DistributedOverlappingMatrix<TMatrix> operator *(double y, DistributedOverlappingMatrix<TMatrix> x) => x.Scale(y);
	}
}
