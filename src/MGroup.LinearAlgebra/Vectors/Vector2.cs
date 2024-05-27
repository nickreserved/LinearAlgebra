using System;
using System.Diagnostics;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Reduction;

namespace MGroup.LinearAlgebra.Vectors
{
	/// <summary>
	/// A otherVector with 2 entries. Optimized version of <see cref="Vector"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	[Serializable]
	public class Vector2 : Vector
    {
		public Vector2() : base(new double[2]) {}
		
		public Vector2(double[] data) : base(data) { Debug.Assert(Length == 2); }
		
		public Vector2(double x, double y) : base(new double[2] { x, y }) {}



		// ------------ CROSS PRODUCT


		/// <summary>
		/// Performs the operation: result = this[0] * otherVector[1] - this[1] * otherVector[0]. The result is a scalar value.  
		/// Also note that: otherVector.Cross(this) = - this.Cross(otherVector).
		/// </summary>
		/// <param name="otherVector">A otherVector with two entries.</param>
		public double CrossProduct(Vector2 otherVector) => VectorExtensions.CrossProductZ(this, otherVector);



		// ------------------- STATIC CREATORS


		/// <summary>
		/// Initializes a new instance of <see cref="Vector2"/> that contains the provided entries: 
		/// {<paramref name="entry0"/>, <paramref name="entry1"/>}.
		/// </summary>
		/// <param name="entry0">The first entry of the new otherVector.</param>
		/// <param name="entry1">The second entry of the new otherVector.</param>
//		[Obsolete("Use new Vector2(entry0, entry1)")]
		public static Vector2 Create(double entry0, double entry1) => new Vector2(new double[] { entry0, entry1 });

		/// <summary>
		/// Initializes a new instance of <see cref="Vector2"/> with <paramref name="data"/> or a clone as its internal array.
		/// </summary>
		/// <param name="data">The entries of the otherVector to create. Constraints: <paramref name="data"/>.Length == 2.</param>
		/// <param name="copyArray">If true, <paramref name="data"/> will be copied and the new <see cref="Vector2"/> instance 
		///     will have a reference to the copy, which is safer. If false, the new otherVector will have a reference to 
		///     <paramref name="data"/> itself, which is faster.</param>
		/// <returns></returns>
//		[Obsolete("Use new Vector2(data) or new Vector2((double[])data.Clone())")]
		new public static Vector2 CreateFromArray(double[] data, bool copyArray = false)
        {
            if (data.Length != 2) throw new NonMatchingDimensionsException($"The provided array had length = {data.Length} instead of 2");
            if (copyArray) return new Vector2(new double[] { data[0], data[1] });
            else return new Vector2(data);
        }

		/// <summary>
		/// Creates an new instance of <see cref="Vector2"/> with all entries being equal to 0.
		/// </summary>
//		[Obsolete("Use new Vector2()")]
        public static Vector2 CreateZero() => new Vector2(new double[2]);








		// ------------- OPERATORS


		public static Vector2 operator -(Vector2 v) => new Vector2(-v.Values[0], -v.Values[1]);
		
		/// <summary>
		/// Performs the operation: result[i] = <paramref name="v1"/>[i] + <paramref name="v2"/>[i], 
		/// for 0 &lt;= i &lt; 2. The resulting entries are written to a new <see cref="Vector2"/> instance.
		/// </summary>
		/// <param name="v1">The first <see cref="Vector2"/> operand.</param>
		/// <param name="v2">The second <see cref="Vector2"/> operand.</param>
		public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.Values[0] + v2.Values[0], v1.Values[1] + v2.Values[1]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="v1"/>[i] - <paramref name="v2"/>[i], 
        /// for 0 &lt;= i &lt; 2. The resulting entries are written to a new <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.Values[0] - v2.Values[0], v1.Values[1] - v2.Values[1]);

		/// <summary>
		/// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
		/// for 0 &lt;= i &lt; 2. The resulting entries are written to a new <see cref="Vector2"/> instance.
		/// </summary>
		/// <param name="scalar">The scalar value that will be multiplied with all otherVector entries.</param>
		/// <param name="vector">The otherVector to multiply.</param>
		public static Vector2 operator *(double scalar, Vector2 vector) => new Vector2(scalar * vector.Values[0], scalar * vector.Values[1]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
        /// for 0 &lt;= i &lt; 2. The resulting entries are written to a new <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="vector">The otherVector to multiply.</param>
        /// <param name="scalar">The scalar value that will be multiplied with all otherVector entries.</param>
        public static Vector2 operator *(Vector2 vector, double scalar) => new Vector2(scalar * vector.Values[0], scalar * vector.Values[1]);

        /// <summary>
        /// Performs the operation: scalar = sum(<paramref name="v1"/>[i] * <paramref name="v2"/>[i]), 
        /// for 0 &lt;= i &lt; 2. The resulting entries are written to a new <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static double operator *(Vector2 v1, Vector2 v2) => v1.Values[0] * v2.Values[0] + v1.Values[1] * v2.Values[1];



		// --------- IVector


        /// <summary>
        /// Performs the operation: this[i] = this[i] + <paramref name="otherVector"/>[i], 
        /// for 0 &lt;= i &lt; 2. The resulting otherVector overwrites the entries of this <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="otherVector">A otherVector with two entries.</param>
        public Vector2 AddIntoThis(Vector2 otherVector)
        {
			Values[0] += otherVector.Values[0];
			Values[1] += otherVector.Values[1];
			return this;
        }

		/// <summary>
		/// Performs the operation: this[i] = this[i] - <paramref name="vector"/>[i], 
		/// for 0 &lt;= i &lt; 2. The resulting otherVector overwrites the entries of this <see cref="Vector2"/> instance.
		/// </summary>
		/// <param name="vector">A otherVector with two entries.</param>
		public Vector2 SubtractIntoThis(Vector2 vector)
		{
			Values[0] -= vector.Values[0];
			Values[1] -= vector.Values[1];
			return this;
		}

		/// <summary>
		/// Performs the operation: this[i] = <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i] + this[i], 
		/// for 0 &lt;= i &lt; 2. The resulting otherVector overwrites the entries of this <see cref="Vector2"/> instance.
		/// </summary>
		/// <param name="otherVector">A otherVector with two entries.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
		public Vector2 AxpyIntoThis(Vector2 otherVector, double otherCoefficient)
        {
			Values[0] += otherCoefficient * otherVector.Values[0];
			Values[1] += otherCoefficient * otherVector.Values[1];
			return this;
        }

		public Vector2 LinearCombinationIntoThis(double thisCoefficient, Vector2 otherVector, double otherCoefficient)
		{
			Values[0] = thisCoefficient * Values[0] + otherCoefficient * otherVector.Values[0];
			Values[1] = thisCoefficient * Values[1] + otherCoefficient * otherVector.Values[1];
			return this;
		}

		/// <summary>
		/// Performs the operation: this[i] = <paramref name="scalar"/> * this[i],
		/// for 0 &lt;= i &lt; 2. The resulting otherVector overwrites the entries of this <see cref="Vector2"/> instance.
		/// </summary>
		/// <param name="scalar">A scalar that multiplies each entry of this otherVector.</param>
		new public Vector2 ScaleIntoThis(double scalar)
		{
			Values[0] *= scalar;
			Values[1] *= scalar;
			return this;
		}



		// -----------IReadOnlyVector

		public Vector2 DoEntrywise(Vector2 vector, Func<double, double, double> binaryOperation)
			=> new Vector2(binaryOperation(Values[0], vector.Values[0]), binaryOperation(Values[1], vector.Values[1]));

        new public Vector2 DoToAllEntries(Func<double, double> unaryOperation)
			=> new Vector2(unaryOperation(Values[0]), unaryOperation(Values[1]));

        /// <summary>
        /// Calculates the dot (or inner/scalar) product of this otherVector with <paramref name="otherVector"/>:
        /// result = this[0] * <paramref name="otherVector"/>[0] + this[1] * <paramref name="otherVector"/>[1].
        /// </summary>
        /// <param name="otherVector">A otherVector with two entries</param>
        public double DotProduct(Vector2 otherVector) => Values[0] * otherVector.Values[0] + this.Values[1] * otherVector.Values[1];

        bool Equals(Vector2 otherVector, double tolerance = 1e-13)
		{
            if (otherVector.Length != 2) return false;
            var comparer = new ValueComparer(tolerance);
            return comparer.AreEqual(Values[0], otherVector.Values[0]) && comparer.AreEqual(Values[1], otherVector.Values[1]);
        }

		public Vector2 LinearCombination(double thisCoefficient, Vector2 otherVector, double otherCoefficient)
			=> new Vector2(thisCoefficient * Values[0] + otherCoefficient * otherVector.Values[0],
							thisCoefficient * Values[1] + otherCoefficient * otherVector.Values[1]);

        public override double Norm2() => Math.Sqrt(Values[0] * Values[0] + Values[1] * Values[1]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * this[i],
        /// for 0 &lt;= i &lt; 2. The resulting otherVector is written to a new <see cref="Vector2"/> and then returned.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this otherVector.</param>
        new public Vector2 Scale(double scalar) => new Vector2(scalar * Values[0], scalar * Values[1]);
    }
}
