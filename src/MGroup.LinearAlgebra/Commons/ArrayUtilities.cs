namespace MGroup.LinearAlgebra.Commons
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Utility methods for basic C# arrays, regardless of any linear algebra.
	/// </summary>
	internal static class ArrayUtilities
	{
		/// <summary>
		/// Sets all entries of <paramref name="array"/> to <paramref name="value"/>.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <param name="array">Any array.</param>
		/// <param name="value">Any value.</param>
		internal static void MemSet<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
		}
	}
}
