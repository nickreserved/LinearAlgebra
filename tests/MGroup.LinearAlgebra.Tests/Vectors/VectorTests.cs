using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MGroup.LinearAlgebra.Tests.TestData;
using MGroup.LinearAlgebra.Tests.Utilities;
using MGroup.LinearAlgebra.Vectors;
using Xunit;

namespace MGroup.LinearAlgebra.Tests.Vectors
{
	/// <summary>
	/// Tests for <see cref="Vector"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	public static class VectorTests
	{
		private static readonly MatrixComparer comparer = new MatrixComparer(1E-10);

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestAddition(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var v1 = new Vector(TestVectors.Vector1);
				var v2 = new Vector(TestVectors.Vector2);
				var expected = new Vector(TestVectors.Sum);

				// operator+
				comparer.AssertEqual(expected, v1 + v2);

				// AddIntoThis()
				var temp = new Vector(v1);
				temp.AddIntoThis(v2);
				comparer.AssertEqual(expected, temp);
			});
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestAxpy(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var v1 = new Vector(TestVectors.Vector1);
				var v2 = new Vector(TestVectors.Vector2);
				var expected = new Vector(TestVectors.Vector1PlusVector2Times3);

				// Axpy()
				comparer.AssertEqual(expected, v1.Axpy(v2, TestVectors.Scalar2));

				// AxpyIntoThis
				var temp = new Vector(v1);
				temp.AxpyIntoThis(v2, TestVectors.Scalar2);
				comparer.AssertEqual(expected, temp);
			});
		}

		[Fact]
		private static void TestClear()
		{
			var zero = new Vector(TestVectors.Vector1.Length);
			var vector = new Vector((double[]) TestVectors.Vector1.Clone());
			vector.Clear();
			comparer.AssertEqual(zero, vector);
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestDotProduct(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var v1 = new Vector(TestVectors.Vector1);
				var v2 = new Vector(TestVectors.Vector2);

				// DotProduct()
				comparer.AssertEqual(TestVectors.DotProduct, v1.DotProduct(v2));

				// operator*
				comparer.AssertEqual(TestVectors.DotProduct, v1 * v2);
			});
		}

		[Fact]
		private static void TestHadamardProduct()
		{
			var v1 = new Vector(TestVectors.Vector1);
			var v2 = new Vector(TestVectors.Vector2);
			var expected = new Vector(TestVectors.HadamardProduct);

			// MultiplyPointwise()
			comparer.AssertEqual(expected, v1.DoEntrywise(v2, (x, y) => x * y));

			// MultiplyPointwiseIntoThis()
			var temp = new Vector(v1);
			temp.DoEntrywiseIntoThis(v2, (x, y) => x * y);
			comparer.AssertEqual(expected, temp);
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestLinearCombination(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var v1 = new Vector(TestVectors.Vector1);
				var v2 = new Vector(TestVectors.Vector2);
				var expected = 2.5 * v1 + -3.5 * v2;
				var comparer = new MatrixComparer();

				// LinearCombination()
				comparer.AssertEqual(expected, v1.LinearCombination(2.5, v2, -3.5));

				// LinearCombinationIntoThis()
				var temp = new Vector(v1);
				temp.LinearCombinationIntoThis(2.5, v2, -3.5);
				comparer.AssertEqual(expected, temp);
			});
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestNorm2(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var vector = new Vector(TestVectors.Vector1);
				comparer.AssertEqual(TestVectors.Norm2OfVector1, vector.Norm2());
			});
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestScaling(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var vector = new Vector(TestVectors.Vector1);
				var expected = new Vector(TestVectors.Vector1Times2);

				// Scale()
				comparer.AssertEqual(expected, vector.Scale(2.0));

				// ScaleIntoThis()
				var temp = new Vector(vector);
				temp.ScaleIntoThis(2.0);
				comparer.AssertEqual(expected, temp);

				// operator*
				comparer.AssertEqual(expected, 2.0 * vector);
			});
		}

		[Fact]
		private static void TestSerialization()
		{
			var originalVector = new Vector(TestVectors.Vector1);
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, originalVector);
				stream.Seek(0, SeekOrigin.Begin);
				var deserializedVector = (Vector)formatter.Deserialize(stream);

				Assert.True(originalVector.Equals(deserializedVector));
			}
		}

		[Theory]
		[MemberData(nameof(TestSettings.ProvidersToTest), MemberType = typeof(TestSettings))]
		private static void TestSubtraction(LinearAlgebraProviderChoice providers)
		{
			TestSettings.RunMultiproviderTest(providers, delegate ()
			{
				var v1 = new Vector(TestVectors.Vector1);
				var v2 = new Vector(TestVectors.Vector2);
				var expected = new Vector(TestVectors.Difference);

				// operator-
				comparer.AssertEqual(expected, v1 - v2);

				// SubtractIntoThis()
				var temp = new Vector(v1);
				temp.SubtractIntoThis(v2);
				comparer.AssertEqual(expected, temp);
			});
		}
	}
}
