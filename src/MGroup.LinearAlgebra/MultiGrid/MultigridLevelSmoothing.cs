namespace MGroup.LinearAlgebra.AlgebraicMultiGrid
{
	using System;
	using System.Collections.Generic;

	using MGroup.LinearAlgebra.Iterative.Stationary;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Vectors;

	public class MultigridLevelSmoothing
	{
		private readonly List<(IStationaryIteration stationaryIteration, int numApplications)> preSmoothers;
		private readonly List<(IStationaryIteration stationaryIteration, int numApplications)> postSmoothers;

		public MultigridLevelSmoothing()
		{
			preSmoothers = new List<(IStationaryIteration, int)>();
			postSmoothers = new List<(IStationaryIteration, int)>();
		}

		public MultigridLevelSmoothing AddPreSmoother(IStationaryIteration stationaryIteration, int numApplications)
		{
			preSmoothers.Add((stationaryIteration, numApplications));
			return this;
		}

		public MultigridLevelSmoothing AddPostSmoother(IStationaryIteration stationaryIteration, int numApplications)
		{
			postSmoothers.Add((stationaryIteration, numApplications));
			return this;
		}

		public void ApplyPreSmoothers(IVectorView input, IVector output) => ApplySmoothers(preSmoothers, input, output);

		public void ApplyPostSmoothers(IVectorView input, IVector output) => ApplySmoothers(postSmoothers, input, output);

		public MultigridLevelSmoothing SetPostSmoothersSameAsPreSmoothers()
		{
			postSmoothers.Clear();
			postSmoothers.AddRange(preSmoothers);
			return this;
		}

		public void UpdateMatrix(IMatrixView matrix, bool isPatternModified)
		{
			CheckSmoothers();
			foreach ((var stationaryIteration, _) in preSmoothers)
			{
				stationaryIteration.UpdateMatrix(matrix, isPatternModified);
			}

			foreach ((var stationaryIteration, _) in postSmoothers)
			{
				stationaryIteration.UpdateMatrix(matrix, isPatternModified);
			}
		}

		private static void ApplySmoothers(List<(IStationaryIteration stationaryIteration, int numApplications)> smoothers,
			IVectorView input, IVector output)
		{
			var inputDense = (Vector)input;
			var outputDense = (Vector)output;
			foreach ((var stationaryIteration, var numApplications) in smoothers)
			{
				for (var t = 0; t < numApplications; t++)
				{
					stationaryIteration.Execute(inputDense, outputDense);
				}
			}
		}

		private void CheckSmoothers()
		{
			if (preSmoothers.Count == 0)
			{
				throw new InvalidOperationException("There are no pre-smoothers defined");
			}

			if (preSmoothers.Count == 0)
			{
				throw new InvalidOperationException("There are no post-smoothers defined");
			}
		}
	}
}
