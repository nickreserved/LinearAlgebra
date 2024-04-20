namespace MGroup.LinearAlgebra.Tests.TestData
{
	using MGroup.LinearAlgebra.Tests.Utilities;

	internal class SubmatricesExample
	{
		private readonly int[] indices0;
		private readonly int[] indices1;
		private readonly double[,] matrix;
		private readonly int order;

		private SubmatricesExample(double[,] matrix, int[] indices0, int[] indices1)
		{
			this.matrix = matrix;
			this.order = matrix.GetLength(0);
			this.indices0 = indices0;
			this.indices1 = indices1;
		}

		public static SubmatricesExample CreateExampleSymmetricA()
		{
			var matrix = new double[,]
			{
				{ 21.0,  1.0,  0.0,  4.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0 },
				{  1.0, 22.0,  2.0,  0.0,  0.0,  0.0,  1.0,  0.0,  0.0,  0.0 },
				{  0.0,  2.0, 23.0,  1.0,  3.0,  1.0,  0.0,  1.0,  0.0,  0.0 },
				{  4.0,  0.0,  1.0, 24.0,  2.0,  4.0,  0.0,  0.0,  0.0,  0.0 },
				{  0.0,  0.0,  3.0,  2.0, 25.0,  5.0,  2.0,  0.0,  0.0,  1.0 },
				{  0.0,  0.0,  1.0,  4.0,  5.0, 26.0,  0.0,  0.0,  2.0,  3.0 },
				{  0.0,  1.0,  0.0,  0.0,  2.0,  0.0, 27.0,  3.0,  0.0,  0.0 },
				{  0.0,  0.0,  1.0,  0.0,  0.0,  0.0,  3.0, 28.0,  4.0,  2.0 },
				{  0.0,  0.0,  0.0,  0.0,  0.0,  2.0,  0.0,  4.0, 29.0,  0.0 },
				{  0.0,  0.0,  0.0,  0.0,  1.0,  3.0,  0.0,  2.0,  0.0, 30.0 }
			};

			var indices0 = new int[] { 1, 7, 3, 6 };
			var indices1 = new int[] { 0, 4, 2, 8, 9, 5 };
			return new SubmatricesExample(matrix, indices0, indices1);
		}

		public static SubmatricesExample CreateExampleSymmetricB()
		{
			var matrix = new double[,]
			{
				{ 31.0,  1.0,  0.0,  0.0,  0.0,  0.0,  2.0,  0.0,  0.0,  0.0 },
				{  1.0, 32.0,  0.0,  4.0,  0.0,  0.0,  1.0,  0.0,  0.0,  0.0 },
				{  0.0,  0.0, 33.0,  1.0,  3.0,  1.0,  0.0,  0.0,  0.0, -1.0 },
				{  0.0,  4.0,  1.0, 34.0,  2.0,  8.0,  0.0,  0.0,  0.0,  0.0 },
				{  0.0,  0.0,  3.0,  2.0, 35.0,  5.0,  0.0, -2.0,  0.0,  1.0 },
				{  0.0,  0.0,  1.0,  8.0,  5.0, 36.0,  0.0,  0.0,  2.0,  3.0 },
				{  2.0,  1.0,  0.0,  0.0,  0.0,  0.0, 37.0,  3.0,  0.0,  0.0 },
				{  0.0,  0.0,  0.0,  0.0, -2.0,  0.0,  3.0, 38.0,  4.0,  2.0 },
				{  0.0,  0.0,  0.0,  0.0,  0.0,  2.0,  0.0,  4.0, 39.0,  0.0 },
				{  0.0,  0.0, -1.0,  0.0,  1.0,  3.0,  0.0,  2.0,  0.0, 40.0 }
			};

			var indices0 = new int[] { 0, 4, 6, 8 };
			var indices1 = new int[] { 3, 5, 1, 2, 7, 9 };
			return new SubmatricesExample(matrix, indices0, indices1);
		}

		public static SubmatricesExample CreateExampleNonSymmetricA()
		{
			var matrix = new double[,]
			{
				{11.0000,    2.6667,    7.0000,    0.0000,    0.0000,    9.0000,    6.0000,    2.2500,    0.0000,    0.0000 },
				{ 0.0000,   10.3333,    1.0000,   -2.5000,    5.5000,   -7.7500,    0.0000,    5.7500,    3.0000,    0.0000 },
				{ 2.0000,    2.0000,   14.0000,    9.0000,    0.0000,    5.0000,    3.5000,    0.0000,    6.0000,    8.0000 },
				{ 1.0000,    0.0000,    3.0000,   15.0000,    1.7500,    6.0000,    0.0000,    7.0000,    0.0000,    9.0000 },
				{ 0.0000,    0.6667,    4.5000,    0.0000,   10.2500,    2.3333,    1.5000,    1.2500,    9.0000,    0.7500 },
				{ 0.3333,    0.0000,    4.0000,    0.0000,    5.0000,   19.0000,    2.5000,    0.0000,    1.0000,    3.0000 },
				{ 0.2500,    6.0000,    0.0000,    0.7500,    0.0000,    3.0000,   18.0000,    1.2500,    0.0000,    0.0000 },
				{ 1.0000,    0.0000,    0.0000,    7.0000,    1.0000,    0.0000,    0.0000,   17.0000,    0.0000,    2.0000 },
				{ 3.5000,    0.0000,    0.0000,    2.0000,    8.0000,    4.0000,    2.5000,    6.0000,   15.0000,    9.0000 },
				{ 3.0000,    0.0000,    0.0000,    0.0000,    2.6667,    9.0000,    0.0000,    5.0000,    7.0000,   13.0000 }
			};

			var indices0 = new int[] { 3, 5, 1, 2, 7, 9 };
			var indices1 = new int[] { 0, 4, 6, 8 };
			return new SubmatricesExample(matrix, indices0, indices1);
		}

		public static SubmatricesExample CreateExampleNonSymmetricB()
		{
			var matrix = new double[,]
			{
				{11.0000,    2.6667,    7.0000,    0.0000,    0.0000,    9.0000,   -3.0000,    2.2500,    0.0000,   -1.0000 },
				{ 0.0000,   10.3333,    1.0000,   -2.5000,    5.5000,   -7.7500,    0.0000,    5.7500,    3.0000,    0.0000 },
				{ 2.0000,    2.0000,   14.0000,    9.0000,    0.0000,    5.0000,    3.5000,    0.0000,    6.0000,    0.0000 },
				{ 1.0000,    0.0000,    3.0000,   15.0000,    1.7500,    6.0000,   -7.0000,    0.0000,    0.0000,    9.0000 },
				{ 0.0000,    0.6667,    4.5000,    0.0000,   10.2500,    2.3333,    1.5000,    1.2500,    9.0000,    0.7500 },
				{ 0.3333,    0.0000,    4.0000,    0.0000,    5.0000,   19.0000,    2.5000,    0.0000,    1.0000,    3.0000 },
				{ 0.2500,    0.0000,    6.0000,    0.7500,    0.0000,    3.0000,   18.0000,    1.2500,    0.0000,    0.0000 },
				{ 0.0000,    2.0000,    0.0000,    7.0000,    1.0000,    0.0000,    0.0000,   17.0000,    0.0000,    2.0000 },
				{ 3.5000,   -1.0000,    0.0000,    0.0000,    8.0000,    4.0000,    2.5000,    6.0000,   15.0000,    9.0000 },
				{ 0.0000,   -3.0000,    3.0000,    0.0000,    2.6667,    9.0000,    0.0000,    5.0000,    7.0000,   13.0000 }
			};

			var indices0 = new int[] { 0, 4, 2, 8, 9, 5 };
			var indices1 = new int[] { 1, 7, 3, 6 };
			return new SubmatricesExample(matrix, indices0, indices1);
		}

		internal double ScalingFactor { get; set; } = 1.0;

		internal int MatrixOrder => order;

		internal int[] Indices0 => ArrayUtilities.Copy(indices0);

		internal int[] Indices1 => ArrayUtilities.Copy(indices1);

		internal double[,] Matrix => MatrixOperations.Scale(ScalingFactor, matrix);

		internal (double[] values, int[] colIndices, int[] rowOffsets) MatrixCsr
			=> FormatConversions.ArrayToCsr(Matrix);

		internal (double[] values, int[] rowIndices, int[] colOffsets) MatrixCsc
			=> FormatConversions.ArrayToCsc(Matrix);

		internal (double[] values, int[] rowIndices, int[] colOffsets) MatrixCscSymmetric
			=> FormatConversions.ArrayToSymmetricCsc(Matrix);

		internal double[] MatrixFullColMajor => FormatConversions.ArrayToFullColMajor(Matrix);

		internal double[] MatrixPackedUpper => FormatConversions.ArrayToPackedUpper(Matrix);

		internal double[,] Submatrix00 => ArrayUtilities.GetSubmatrix(Matrix, Indices0, Indices0);

		internal (double[] values, int[] colIndices, int[] rowOffsets) Submatrix00Csr
			=> FormatConversions.ArrayToCsr(Submatrix00);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix00Csc
			=> FormatConversions.ArrayToCsc(Submatrix00);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix00CscSymmetric
			=> FormatConversions.ArrayToSymmetricCsc(Submatrix00);

		internal double[] Submatrix00FullColMajor => FormatConversions.ArrayToFullColMajor(Submatrix00);

		internal double[] Submatrix00PackedUpper => FormatConversions.ArrayToPackedUpper(Submatrix00);

		internal double[,] Submatrix01 => ArrayUtilities.GetSubmatrix(Matrix, Indices0, Indices1);

		internal (double[] values, int[] colIndices, int[] rowOffsets) Submatrix01Csr
			=> FormatConversions.ArrayToCsr(Submatrix01);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix01Csc
			=> FormatConversions.ArrayToCsc(Submatrix01);

		internal double[,] Submatrix10 => ArrayUtilities.GetSubmatrix(Matrix, Indices1, Indices0);

		internal (double[] values, int[] colIndices, int[] rowOffsets) Submatrix10Csr
			=> FormatConversions.ArrayToCsr(Submatrix10);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix10Csc
			=> FormatConversions.ArrayToCsc(Submatrix10);

		internal double[,] Submatrix11 => ArrayUtilities.GetSubmatrix(Matrix, Indices1, Indices1);

		internal (double[] values, int[] colIndices, int[] rowOffsets) Submatrix11Csr
			=> FormatConversions.ArrayToCsr(Submatrix11);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix11Csc
			=> FormatConversions.ArrayToCsc(Submatrix11);

		internal (double[] values, int[] rowIndices, int[] colOffsets) Submatrix11CscSymmetric
			=> FormatConversions.ArrayToSymmetricCsc(Submatrix11);

		internal double[] Submatrix11PackedUpper => FormatConversions.ArrayToPackedUpper(Submatrix11);
	}
}
