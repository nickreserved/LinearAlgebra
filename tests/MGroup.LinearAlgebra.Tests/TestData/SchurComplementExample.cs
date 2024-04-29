#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
namespace MGroup.LinearAlgebra.Tests.TestData
{
	using MGroup.LinearAlgebra.Tests.Utilities;

	internal class SchurComplementExample
	{
		private readonly int[] indices0;
		private readonly int[] indices1;
		private readonly double[,] matrix;
		private readonly int order;
		private readonly double[,] schurOfA00;
		private readonly double[,] schurOfA11;

		private SchurComplementExample(double[,] matrix, int[] indices0, int[] indices1,
			double[,] schurOfA00, double[,] schurOfA11)
		{
			this.matrix = matrix;
			this.order = matrix.GetLength(0);
			this.indices0 = indices0;
			this.indices1 = indices1;
			this.schurOfA00 = schurOfA00;
			this.schurOfA11 = schurOfA11;
		}

		public static SchurComplementExample CreateExampleSymmetricA()
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

			var S00 = new double[,]
			{
				{ 20.2878012109391,     -0.329919947986509,  -0.257913771384453,   -0.000731439717176642, -0.000365719858588321, -0.666666666666667 },
				{ -0.329919947986509,   24.6831443780731,     2.93153927424926,     0.0321833475557723,    1.01609167377789,      4.66666666666667 },
				{ -0.257913771384453,    2.93153927424926,   22.7393280913487,     -0.146044130196270,    -0.0730220650981348,    0.833333333333333 },
				{ -0.000731439717176643, 0.0321833475557723, -0.146044130196270,   28.4216749969523,      -0.289162501523833,     2.0 },
				{ -0.000365719858588321, 1.01609167377789,   -0.0730220650981348,  -0.289162501523833,    29.8554187492381,       3.0 },
				{ -0.666666666666667,    4.66666666666667,    0.833333333333333,    2.0,                   3.0,                  25.3333333333333 },
			};

			var S11 = new double[,]
			{
				{ 21.7756450959089,    -0.0897594033095575, -0.252278752665692,   1.02065680633318 },
				{ -0.0897594033095575, 27.2604724251365,     0.041815777399048,   3.00844741463716 },
				{ -0.252278752665692,   0.041815777399048,  22.5272584437833,    -0.0954293252131430 },
				{ 1.02065680633318,     3.00844741463716,   -0.0954293252131430, 26.8311089274107 },
			};

			return new SchurComplementExample(matrix, indices0, indices1, S00, S11);
		}

		public static SchurComplementExample CreateExampleSymmetricB()
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

			var S00 = new double[,]
			{
				{ 33.8857142857143,     7.71428571428571,    4.0,                 0.828571428571429,  0.114285714285714,  -0.0571428571428571 },
				{  7.71428571428571,   35.1831501831502,     0.0,                 0.571428571428571,  0.0805860805860806,  2.85714285714286 },
				{  4.0,                 0.0,                31.9440069991251,     0.0,               -0.0761154855643045,  0.0 },
				{  0.828571428571429,   0.571428571428572,   0.0,                32.7428571428571,    0.171428571428571,  -1.08571428571429 },
				{  0.114285714285714,   0.0805860805860805, -0.0761154855643045,  0.171428571428571, 37.2313633872689,     2.05714285714286 },
				{ -0.0571428571428571,  2.85714285714286,    0.0,                -1.08571428571429,   2.05714285714286,   39.9714285714286 }
			};

			var S11 = new double[,]
			{
				{ 30.9682569966764,      0.00328957703229347, 1.96824691701410,  -0.00177192780415189 },
				{  0.00328957703229347, 33.9146116356019,     0.164371268990715, -0.0431188083919515 },
				{  1.96824691701410,     0.164371268990715,  36.7307651069854,   -0.319108517758776 },
				{ -0.00177192780415189, -0.0431188083919515, -0.319108517758776, 38.4577472655064 }
			};

			return new SchurComplementExample(matrix, indices0, indices1, S00, S11);
		}

		public static SchurComplementExample CreateExampleNonSymmetricA()
		{
			var matrix = new double[,]
			{
				{ 11.0000,    2.6667,    7.0000,    0.0000,    0.0000,    9.0000,    6.0000,    2.2500,    0.0000,    0.0000 },
				{  0.0000,   10.3333,    1.0000,   -2.5000,    5.5000,   -7.7500,    0.0000,    5.7500,    3.0000,    0.0000 },
				{  2.0000,    2.0000,   14.0000,    9.0000,    0.0000,    5.0000,    3.5000,    0.0000,    6.0000,    8.0000 },
				{  1.0000,    0.0000,    3.0000,   15.0000,    1.7500,    6.0000,    0.0000,    7.0000,    0.0000,    9.0000 },
				{  0.0000,    0.6667,    4.5000,    0.0000,   10.2500,    2.3333,    1.5000,    1.2500,    9.0000,    0.7500 },
				{  0.3333,    0.0000,    4.0000,    0.0000,    5.0000,   19.0000,    2.5000,    0.0000,    1.0000,    3.0000 },
				{  0.2500,    6.0000,    0.0000,    0.7500,    0.0000,    3.0000,   18.0000,    1.2500,    0.0000,    0.0000 },
				{  1.0000,    0.0000,    0.0000,    7.0000,    1.0000,    0.0000,    0.0000,   17.0000,    0.0000,    2.0000 },
				{  3.5000,    0.0000,    0.0000,    2.0000,    8.0000,    4.0000,    2.5000,    6.0000,   15.0000,    9.0000 },
				{  3.0000,    0.0000,    0.0000,    0.0000,    2.6667,    9.0000,    0.0000,    5.0000,    7.0000,   13.0000 }
			};

			var indices0 = new int[] { 3, 5, 1, 2, 7, 9 };
			var indices1 = new int[] { 0, 4, 6, 8 };

			var S00 = new double[,]
			{
				{ 10.7045152473610,   -3.43453211600961,  3.09613055596556,   -2.00956753308930 },
				{ -0.618108564446338,  9.97305794238903, -0.0485070455570842,  6.83247755996934 },
				{  1.05975842292095,  -5.65692551854517, 17.8207405068184,     0.657744815718584 },
				{  1.35495896307748,   6.66854586561876,  2.71883992153007,   10.4574831040166 }
			};

			var S11 = new double[,]
			{
				{ 15.4236641221374,    4.79444075215351, -0.316331150640801,  0.481441277400378,  7.47340500035017, 10.4931192660550 },
				{  0.793892366412214, 17.0883360179284,  -1.43342295785513,  -0.835000530693872,  1.06357116822684,  6.21100917431193 },
				{ -2.00190839694656,  -9.01230193991176,  9.97342899187618,  -2.73937834115368,   6.13039399351028,  1.52752293577982 },
				{  7.37595419847328,   3.43351335527698,  1.59890419672246,  17.0703772439713,   -3.84128644629643,  1.66972477064220 },
				{  7.25190839694657,  -1.00295861054696, -0.206934267105540, -1.71398557321941,  17.1985433153582,   2.85321100917431 },
				{ -1.09732061068702,   6.60251071446180,  0.460016384152018,  0.257520014629097,  1.65787856603715,  7.88993669724771 }
			};

			return new SchurComplementExample(matrix, indices0, indices1, S00, S11);
		}

		public static SchurComplementExample CreateExampleNonSymmetricB()
		{
			var matrix = new double[,]
			{
				{ 11.0000,    2.6667,    7.0000,    0.0000,    0.0000,    9.0000,   -3.0000,    2.2500,    0.0000,   -1.0000 },
				{  0.0000,   10.3333,    1.0000,   -2.5000,    5.5000,   -7.7500,    0.0000,    5.7500,    3.0000,    0.0000 },
				{  2.0000,    2.0000,   14.0000,    9.0000,    0.0000,    5.0000,    3.5000,    0.0000,    6.0000,    0.0000 },
				{  1.0000,    0.0000,    3.0000,   15.0000,    1.7500,    6.0000,   -7.0000,    0.0000,    0.0000,    9.0000 },
				{  0.0000,    0.6667,    4.5000,    0.0000,   10.2500,    2.3333,    1.5000,    1.2500,    9.0000,    0.7500 },
				{  0.3333,    0.0000,    4.0000,    0.0000,    5.0000,   19.0000,    2.5000,    0.0000,    1.0000,    3.0000 },
				{  0.2500,    0.0000,    6.0000,    0.7500,    0.0000,    3.0000,   18.0000,    1.2500,    0.0000,    0.0000 },
				{  0.0000,    2.0000,    0.0000,    7.0000,    1.0000,    0.0000,    0.0000,   17.0000,    0.0000,    2.0000 },
				{  3.5000,   -1.0000,    0.0000,    0.0000,    8.0000,    4.0000,    2.5000,    6.0000,   15.0000,    9.0000 },
				{  0.0000,   -3.0000,    3.0000,    0.0000,    2.6667,    9.0000,    0.0000,    5.0000,    7.0000,   13.0000 }
			};

			var indices0 = new int[] { 0, 4, 2, 8, 9, 5 };
			var indices1 = new int[] { 1, 7, 3, 6 };

			var S00 = new double[,]
			{
				{ 10.4847799287514,    4.52236953922628,  0.638339404541572,  3.40275255158354 },
				{  2.68152182151277,  16.1085373525288,   8.61548592994099,   1.59008226775780 },
				{  2.38638912521775,  -3.49245402389412, 18.4959226255360,   -2.46978074040489 },
				{ -0.539178824640097,  1.67669494827875, -1.65913592694926,  17.9265670632429 },
			};

			var S11 = new double[,]
			{
				{  11.0190016059046,    -1.45160995214376, 7.64351106252524,  -0.738958584602404, -1.30731729739247,  11.2609361998443 },
				{ -0.00144197195139802,  9.92925203771488, 4.03928162352459,   8.83507463877645,   0.810432667096448,  2.63601468282007 },
				{  1.22464202520723,    -2.23044914291320, 9.08680063981229,   5.35802707084537,  -5.74993119138942,   1.32824380124775 },
				{  3.70951161008916,     8.94815827752617, 0.529682111554553, 15.5275605774142,   10.1861885002549,    3.81571733711474 },
				{  0.281724686568795,    4.74040442182610, 4.74249142234254,   8.11928643250546,  14.4555481592492,    7.94863585024061 },
				{  0.300208337737282,    5.00189851627097, 3.17248616078977,   0.993681202959842,  3.03513939481952,  18.6103071320892 }
			};

			return new SchurComplementExample(matrix, indices0, indices1, S00, S11);
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

		internal double[] Submatrix11FullColMajor => FormatConversions.ArrayToFullColMajor(Submatrix11);

		internal double[] Submatrix11PackedUpper => FormatConversions.ArrayToPackedUpper(Submatrix11);

		internal double[,] SchurOfSubmatrix00 => MatrixOperations.Scale(ScalingFactor, schurOfA00);

		internal double[] SchurOfSubmatrix00FullColMajor => FormatConversions.ArrayToFullColMajor(SchurOfSubmatrix00);

		internal double[] SchurOfSubmatrix00PackedUpper => FormatConversions.ArrayToPackedUpper(SchurOfSubmatrix00);

		internal double[,] SchurOfSubmatrix11 => MatrixOperations.Scale(ScalingFactor, schurOfA11);

		internal double[] SchurOfSubmatrix11FullColMajor => FormatConversions.ArrayToFullColMajor(SchurOfSubmatrix11);

		internal double[] SchurOfSubmatrix11PackedUpper => FormatConversions.ArrayToPackedUpper(SchurOfSubmatrix11);
	}
}
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
