#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace MGroup.LinearAlgebra.Tests.Matrices
{
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;

	using MGroup.LinearAlgebra.Commons;
	using MGroup.LinearAlgebra.Exceptions;
	using MGroup.LinearAlgebra.Matrices;
	using MGroup.LinearAlgebra.Tests.TestData;
	using MGroup.LinearAlgebra.Tests.Utilities;
	using MGroup.LinearAlgebra.Vectors;
	using Xunit;

	public static class MatrixConcatenationTests
	{
		private static readonly MatrixComparer comparer = new MatrixComparer(1E-13);

		private static Matrix A1 => Matrix.CreateFromArray(new double[,]
		{
			{ 101, 102 },
			{ 103, 104 },
			{ 105, 106 },
		});

		private static Matrix A2 => Matrix.CreateFromArray(new double[,]
		{
			{ 201, 202, 203 },
			{ 204, 205, 206 },
			{ 207, 208, 209 },
		});

		private static Matrix A3 = Matrix.CreateFromArray(new double[,]
		{
			{ 301, 302, 303, 304 },
			{ 305, 306, 307, 308 },
			{ 309, 310, 311, 312 },
		});

		private static Matrix A4 = Matrix.CreateFromArray(new double[,]
		{
			{ 401, 402 },
			{ 403, 404 },
			{ 405, 406 },
			{ 407, 408 },
		});

		private static Matrix A5 = Matrix.CreateFromArray(new double[,]
		{
			{ 501, 502, 503 },
			{ 504, 505, 506 },
			{ 507, 508, 509 },
			{ 510, 511, 512 },
		});

		private static Matrix A6 = Matrix.CreateFromArray(new double[,]
		{
			{ 601, 602, 603, 604 },
			{ 605, 606, 607, 608 },
			{ 609, 610, 611, 612 },
			{ 613, 614, 615, 616 },
		});

		private static Matrix A7 = Matrix.CreateFromArray(new double[,]
		{
			{ 701, 702 },
			{ 703, 704 },
			{ 705, 706 },
			{ 707, 708 },
			{ 709, 710 },
		});

		private static Matrix A8 = Matrix.CreateFromArray(new double[,]
		{
			{ 801, 802, 803 },
			{ 804, 805, 806 },
			{ 807, 808, 809 },
			{ 810, 811, 812 },
			{ 813, 814, 815 },
		});

		private static Matrix A9 = Matrix.CreateFromArray(new double[,]
		{
			{ 901, 902, 903, 904 },
			{ 905, 906, 907, 908 },
			{ 909, 910, 911, 912 },
			{ 913, 914, 915, 916 },
			{ 917, 918, 919, 920 },
		});

		[Fact]
		private static void TestAppendHorizontally()
		{
			Matrix result =  A7.AppendRight(A8).AppendRight(A9);

			var expected = Matrix.CreateFromArray(new double[,]
			{
				{ 701, 702, 801, 802, 803, 901, 902, 903, 904 },
				{ 703, 704, 804, 805, 806, 905, 906, 907, 908 },
				{ 705, 706, 807, 808, 809, 909, 910, 911, 912 },
				{ 707, 708, 810, 811, 812, 913, 914, 915, 916 },
				{ 709, 710, 813, 814, 815, 917, 918, 919, 920 },
			});

			comparer.AssertEqual(expected, result);
		}

		[Fact]
		private static void TestAppendVertically()
		{
			Matrix result = A3.AppendBottom(A6).AppendBottom(A9);

			var expected = Matrix.CreateFromArray(new double[,]
			{
				{ 301, 302, 303, 304 },
				{ 305, 306, 307, 308 },
				{ 309, 310, 311, 312 },
				{ 601, 602, 603, 604 },
				{ 605, 606, 607, 608 },
				{ 609, 610, 611, 612 },
				{ 613, 614, 615, 616 },
				{ 901, 902, 903, 904 },
				{ 905, 906, 907, 908 },
				{ 909, 910, 911, 912 },
				{ 913, 914, 915, 916 },
				{ 917, 918, 919, 920 },
			});

			comparer.AssertEqual(expected, result);
		}


		[Fact]
		private static void TestConcatenate1x3()
		{
			Matrix result = Matrix.Concatenate(new Matrix[,]
			{
				{ A7, A8, A9 },
			});

			var expected = Matrix.CreateFromArray(new double[,]
			{
				{ 701, 702, 801, 802, 803, 901, 902, 903, 904 },
				{ 703, 704, 804, 805, 806, 905, 906, 907, 908 },
				{ 705, 706, 807, 808, 809, 909, 910, 911, 912 },
				{ 707, 708, 810, 811, 812, 913, 914, 915, 916 },
				{ 709, 710, 813, 814, 815, 917, 918, 919, 920 },
			});

			comparer.AssertEqual(expected, result);
		}

		[Fact]
		private static void TestConcatenate3x1()
		{
			Matrix result = Matrix.Concatenate(new Matrix[,]
			{
				{ A3 },
				{ A6 },
				{ A9 },
			});

			var expected = Matrix.CreateFromArray(new double[,]
			{
				{ 301, 302, 303, 304 },
				{ 305, 306, 307, 308 },
				{ 309, 310, 311, 312 },
				{ 601, 602, 603, 604 },
				{ 605, 606, 607, 608 },
				{ 609, 610, 611, 612 },
				{ 613, 614, 615, 616 },
				{ 901, 902, 903, 904 },
				{ 905, 906, 907, 908 },
				{ 909, 910, 911, 912 },
				{ 913, 914, 915, 916 },
				{ 917, 918, 919, 920 },
			});

			comparer.AssertEqual(expected, result);
		}

		[Fact]
		private static void TestConcatenate3x3()
		{
			Matrix result = Matrix.Concatenate(new Matrix[,]
			{
				{ A1, A2, A3 },
				{ A4, A5, A6 },
				{ A7, A8, A9 },
			});

			var expected = Matrix.CreateFromArray(new double[,]
			{
				{ 101, 102, 201, 202, 203, 301, 302, 303, 304 },
				{ 103, 104, 204, 205, 206, 305, 306, 307, 308 },
				{ 105, 106, 207, 208, 209, 309, 310, 311, 312 },
				{ 401, 402, 501, 502, 503, 601, 602, 603, 604 },
				{ 403, 404, 504, 505, 506, 605, 606, 607, 608 },
				{ 405, 406, 507, 508, 509, 609, 610, 611, 612 },
				{ 407, 408, 510, 511, 512, 613, 614, 615, 616 },
				{ 701, 702, 801, 802, 803, 901, 902, 903, 904 },
				{ 703, 704, 804, 805, 806, 905, 906, 907, 908 },
				{ 705, 706, 807, 808, 809, 909, 910, 911, 912 },
				{ 707, 708, 810, 811, 812, 913, 914, 915, 916 },
				{ 709, 710, 813, 814, 815, 917, 918, 919, 920 },
			});

			comparer.AssertEqual(expected, result);
		}
	}
}
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
