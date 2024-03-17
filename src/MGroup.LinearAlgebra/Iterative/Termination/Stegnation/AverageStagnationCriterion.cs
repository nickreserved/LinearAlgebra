//ERROR: If there is one sharp increase in the error (outlier), followed by decreases then the current approach will incorrectly 
//       detect it as stagnation.
namespace MGroup.LinearAlgebra.Iterative.Termination.Stegnation
{
	using System;
	using System.Collections.Generic;

	using MGroup.LinearAlgebra.Reduction;
	using MGroup.LinearAlgebra.Vectors;

	public class AverageStagnationCriterion : IStagnationCriterion
	{
		private readonly int iterationSpan;
		private double relativeImprovementTolerance;
		private List<double> residualDotProductsHistory;

		public AverageStagnationCriterion(int iterationSpan, double relativeImprovementTolerance = -1)
		{
			this.iterationSpan = iterationSpan;
			this.relativeImprovementTolerance = relativeImprovementTolerance;
			residualDotProductsHistory = new List<double>();
		}

		public IStagnationCriterion CopyWithInitialSettings() 
			=> new AverageStagnationCriterion(iterationSpan, relativeImprovementTolerance);

		public bool HasStagnated()
		{
			double[] errorReductions = CalcRelativeErrorReductions();
			if (errorReductions == null) return false; // Not enough data yet
			double relativeImprovement = Vector.CreateFromArray(errorReductions).Average();
			if (relativeImprovementTolerance == -1)
			{
				relativeImprovementTolerance = 1E-3 * CalcInitialErrorReduction();
			}
			if (relativeImprovement <= relativeImprovementTolerance) return true;
			else return false;
		}

		public void StoreInitialError(double initialError)
		{
			residualDotProductsHistory.Clear();
			residualDotProductsHistory.Add(initialError);
		}

		public void StoreNewError(double currentError)
		{
			residualDotProductsHistory.Add(currentError);
		}

		private double CalcInitialErrorReduction()
		{
			var t = 0;
			while (t < iterationSpan)
			{
				double current = residualDotProductsHistory[t];
				double next = residualDotProductsHistory[t + 1];
				double reduction = (current - next) / current;
				if (reduction > 0) return reduction;
				else ++t;
			}
			// At this point PCG has made no improvement. Thus it diverges.
			throw new Exception("PCG diverges");
		}

		private double[] CalcRelativeErrorReductions()
		{
			int numIterations = residualDotProductsHistory.Count;
			if (numIterations <= iterationSpan) return null;

			double[] relativeReductions = new double[iterationSpan];
			for (var t = 0; t < iterationSpan; ++t)
			//for (int t = numIterations - iterationSpan - 1; t < numIterations - 1; ++t)
			{
				double current = residualDotProductsHistory[t + numIterations - iterationSpan - 1];
				double next = residualDotProductsHistory[t + numIterations - iterationSpan];
				relativeReductions[t] = (current - next) / current;
			}

			return relativeReductions;
		}
	}
}
