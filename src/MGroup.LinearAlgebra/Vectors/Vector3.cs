using System;
using System.Diagnostics;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Reduction;

namespace MGroup.LinearAlgebra.Vectors
{
	/// <summary>
	/// A otherVector with 3 entries. Optimized version of <see cref="Vector"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	[Serializable]
	public class Vector3 : Vector
    {
		/// <summary>
		/// Creates a zero otherVector
		/// </summary>
		public Vector3() : base(new double[3]) { }

		/// <summary>
		/// Create 3d otherVector from an array of 3 elements
		/// </summary>
		/// <param name="data">The 3 elements array.
		/// Array is not copied. Any future otherVector modification, also modifies the array.</param>
        public Vector3(double[] data) : base(data) { Debug.Assert(Length == 3); }

		/// <summary>
		/// Create 3d otherVector from its components.
		/// </summary>
		public Vector3(double x, double y, double z) : base(new double[3] { x, y, z }) {}



		// ------------ CROSS PRODUCT


		/// <summary>
		/// Returns the X component of the cross product of this otherVector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 2nd and 3rd components of this otherVector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other otherVector</param>
		/// <returns>The X component of the cross product</returns>
		public double CrossProductX(Vector3 otherVector) => Values[1] * otherVector.Values[2] - Values[2] * otherVector.Values[1];
		/// <summary>
		/// Returns the Y component of the cross product of this otherVector with <paramref name="otherVector"/>.
		/// </summary>
		/// For calculation uses the 1st and 3rd components of this otherVector with corresponding components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other otherVector</param>
		/// <returns>The Y component of the cross product</returns>
		public double CrossProductY(Vector3 otherVector) => -Values[0] * otherVector.Values[2] + Values[2] * otherVector.Values[0];
		/// <summary>
		/// Returns the Z component of the cross product of this otherVector with <paramref name="otherVector"/>.
		/// This is the cross product of 2 dimensional vectors.
		/// </summary>
		/// For calculation uses the 2 first components of this otherVector with 2 first components of <paramref name="otherVector"/>.
		/// <param name="otherVector">The other otherVector</param>
		/// <returns>The Z component of the cross product</returns>
		public double CrossProductZ(Vector3 otherVector) => Values[0] * otherVector.Values[1] - Values[1] * otherVector.Values[0];
		/// <summary>
		/// Returns the cross product of this otherVector with <paramref name="otherVector"/>.
		/// This is the cross product of 3 dimensional vectors.
		/// </summary>
		/// <param name="otherVector">The other otherVector</param>
		/// <returns>The cross product</returns>
		public Vector3 CrossProduct(Vector3 otherVector) => new Vector3(CrossProductX(otherVector), CrossProductY(otherVector), CrossProductZ(otherVector));



		// ------------------- STATIC CREATORS


		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> that contains the provided entries: 
		/// {<paramref name="entry0"/>, <paramref name="entry1"/>, <paramref name="entry2"/>}.
		/// </summary>
		/// <param name="entry0">The first entry of the new otherVector.</param>
		/// <param name="entry1">The second entry of the new otherVector.</param>
		/// <param name="entry2">The third entry of the new otherVector.</param>
//		[Obsolete("Use new Vector3(x,y,z)")]
        public static Vector3 Create(double entry0, double entry1, double entry2) => new Vector3(entry0, entry1, entry2);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> with <paramref name="data"/> or a clone as its internal array.
		/// </summary>
		/// <param name="data">The entries of the otherVector to create. Constraints: <paramref name="data"/>.Length == 3.</param>
		/// <param name="copyArray">If true, <paramref name="data"/> will be copied and the new <see cref="Vector3"/> instance 
		///     will have a reference to the copy, which is safer. If false, the new otherVector will have a reference to 
		///     <paramref name="data"/> itself, which is faster.</param>
		/// <returns></returns>
//		[Obsolete("Use new Vector3(data) or new Vector((double[]) data.Clone())")]
		new public static Vector3 CreateFromArray(double[] data, bool copyArray = false) => new Vector3(copyArray ? (double[])data.Clone() : data);

		/// <summary>
		/// Creates an new instance of <see cref="Vector3"/> with all entries being equal to 0.
		/// </summary>
//		[Obsolete("Use new Vector3()")]
        public static Vector3 CreateZero() => new Vector3(new double[3]);

		


		// ------------- OPERATORS


		public static Vector3 operator -(Vector3 v) => new Vector3(-v.Values[0], -v.Values[1], -v.Values[2]);

		/// <summary>
		/// Performs the operation: result[i] = <paramref name="v1"/>[i] + <paramref name="v2"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="v1">The first <see cref="Vector2"/> operand.</param>
		/// <param name="v2">The second <see cref="Vector2"/> operand.</param>
		public static Vector3 operator +(Vector3 v1, Vector3 v2) => new Vector3(v1.Values[0] + v2.Values[0], v1.Values[1] + v2.Values[1], v1.Values[2] + v2.Values[2]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="v1"/>[i] - <paramref name="v2"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static Vector3 operator -(Vector3 v1, Vector3 v2) => new Vector3(v1.Values[0] - v2.Values[0], v1.Values[1] - v2.Values[1], v1.Values[2] - v2.Values[2]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="scalar">The scalar value that will be multiplied with all otherVector entries.</param>
        /// <param name="vector">The otherVector to multiply.</param>
        public static Vector3 operator *(double scalar, Vector3 vector) => new Vector3(scalar * vector.Values[0], scalar * vector.Values[1], scalar * vector.Values[2]);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="vector">The otherVector to multiply.</param>
        /// <param name="scalar">The scalar value that will be multiplied with all otherVector entries.</param>
        public static Vector3 operator *(Vector3 vector, double scalar) => new Vector3(scalar * vector.Values[0], scalar * vector.Values[1], scalar * vector.Values[2]);

        /// <summary>
        /// Performs the operation: scalar = sum(<paramref name="v1"/>[i] * <paramref name="v2"/>[i]), 
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static double operator *(Vector3 v1, Vector3 v2) => v1.Values[0] * v2.Values[0] + v1.Values[1] * v2.Values[1] + v1.Values[2] * v2.Values[2];




		// --------- IMinimalVector

		/// <summary>
		/// Performs the operation: this[i] = this[i] + <paramref name="otherVector"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting otherVector overwrites the entries of this <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="otherVector">A otherVector with three entries.</param>
		public Vector3 AddIntoThis(Vector3 otherVector)
		{
            Values[0] += otherVector.Values[0];
            Values[1] += otherVector.Values[1];
            Values[2] += otherVector.Values[2];
			return this;
        }

		/// <summary>
		/// Performs the operation: this[i] = this[i] - <paramref name="otherVector"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting otherVector overwrites the entries of this <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="otherVector">A otherVector with three entries.</param>
		public Vector3 SubtractIntoThis(Vector3 otherVector)
		{
			Values[0] -= otherVector.Values[0];
			Values[1] -= otherVector.Values[1];
			Values[2] -= otherVector.Values[2];
			return this;
		}

        public Vector3 AxpyIntoThis(Vector3 otherVector, double otherCoefficient)
		{
            Values[0] += otherCoefficient * otherVector.Values[0];
			Values[1] += otherCoefficient * otherVector.Values[1];
			Values[2] += otherCoefficient * otherVector.Values[2];
			return this;
        }

		/// <summary>
		/// Performs the operation: this[i] = <paramref name="thisCoefficient"/> * this[i] + 
		///     <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting otherVector overwrites the entries of this <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="thisCoefficient">A scalar that multiplies each entry of this otherVector.</param>
		/// <param name="otherVector">A otherVector with three entries.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
		public Vector3 LinearCombinationIntoThis(double thisCoefficient, Vector3 otherVector, double otherCoefficient)
		{
			Values[0] = thisCoefficient * Values[0] * otherCoefficient * otherVector.Values[0];
			Values[1] = thisCoefficient * Values[1] * otherCoefficient * otherVector.Values[1];
			Values[2] = thisCoefficient * Values[2] * otherCoefficient * otherVector.Values[2];
			return this;
		}

		/// <summary>
		/// Performs the operation: this[i] = <paramref name="scalar"/> * this[i],
		/// for 0 &lt;= i &lt; 3. The resulting otherVector overwrites the entries of this <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="scalar">A scalar that multiplies each entry of this otherVector.</param>
		public override void ScaleIntoThis(double scalar)
		{
			Values[0] *= scalar;
			Values[1] *= scalar;
			Values[2] *= scalar;
		}




		// --------- IMinimalReadOnlyVector

		public Vector3 Axpy(Vector3 otherVector, double otherCoefficient) => new Vector3(Values[0] + otherCoefficient * otherVector.Values[0],
																						Values[1] + otherCoefficient * otherVector.Values[1],
																						Values[2] + otherCoefficient * otherVector.Values[2]);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> by deep copying the entries of this instance.
		/// </summary>
		new public Vector3 Copy() => new Vector3((double[])Values.Clone());

		public Vector3 DoEntrywise(Vector3 vector, Func<double, double, double> binaryOperation) => new Vector3(base.DoEntrywise(vector, binaryOperation).Values);

		new public Vector3 DoToAllEntries(Func<double, double> unaryOperation) => new Vector3(base.DoToAllEntries(unaryOperation).Values);

		/// <summary>
		/// Calculates the dot (or inner/scalar) product of this otherVector with <paramref name="otherVector"/>:
		/// result = this[0] * <paramref name="otherVector"/>[0] + this[1] * <paramref name="otherVector"/>[1] 
		///     + this[2] * <paramref name="otherVector"/>[2].
		/// </summary>
		/// <param name="otherVector">A otherVector with three entries</param>
		public double DotProduct(Vector3 otherVector) => Values[0] * otherVector.Values[0] + Values[1] * otherVector.Values[1] + Values[2] * otherVector.Values[2];

		/// <summary>
		/// Performs the operation: result[i] = <paramref name="thisCoefficient"/> * this[i] + 
		///     <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting otherVector is written to a new <see cref="Vector3"/> and then returned.
		/// </summary>
		/// <param name="thisCoefficient">A scalar that multiplies each entry of this otherVector.</param>
		/// <param name="otherVector">A otherVector with three entries.</param>
		/// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
		public Vector3 LinearCombination(double thisCoefficient, Vector3 otherVector, double otherCoefficient)
			=> new Vector3(thisCoefficient * Values[0] + otherCoefficient * otherVector.Values[0],
							thisCoefficient * Values[1] + otherCoefficient * otherVector.Values[1],
							thisCoefficient * Values[2] + otherCoefficient * otherVector.Values[2]);

		public override double Norm2() => Math.Sqrt(Values[0] * Values[0] + Values[1] * Values[1] + Values[2] * Values[2]);
		/// <summary>
		/// Performs the operation: result[i] = <paramref name="scalar"/> * this[i],
		/// for 0 &lt;= i &lt; 23. The resulting otherVector is written to a new <see cref="Vector3"/> and then returned.
		/// </summary>
		/// <param name="scalar">A scalar that multiplies each entry of this otherVector.</param>

		new public Vector3 Scale(double scalar) => new Vector3(scalar * Values[0], scalar * Values[1], scalar * Values[2]);



















		[Obsolete("Use this.View(thisIndices).CopyFrom(otherVector.View(otherIndices)")]
        public void CopyNonContiguouslyFrom(int[] thisIndices, IExtendedReadOnlyVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

		[Obsolete("Use CopyFrom(otherVector.View(otherIndices)")]
		public void CopyNonContiguouslyFrom(IExtendedReadOnlyVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, otherVector, otherIndices);
    }
}
