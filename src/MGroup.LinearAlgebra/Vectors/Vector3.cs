#if (false)

using System;
using System.Diagnostics;

using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Reduction;

namespace MGroup.LinearAlgebra.Vectors
{
	/// <summary>
	/// A vector with 3 entries. Optimized version of <see cref="Vector"/>.
	/// Authors: Serafeim Bakalakos
	/// </summary>
	[Obsolete("Use Vector")]
	public class Vector3: Vector
    {
		/// <summary>
		/// Creates a zero vector
		/// </summary>
		public Vector3() : base(new double[3]) { }

		/// <summary>
		/// Create 3d vector from an array of 3 elements
		/// </summary>
		/// <param name="data">The 3 elements array.
		/// Array is not copied. Any future vector modification, also modifies the array.</param>
        public Vector3(double[] data) : base(data) { Debug.Assert(Length == 3); }

		/// <summary>
		/// Create 3d vector from its components.
		/// </summary>
		public Vector3(double x, double y, double z) : base(new double[3] { x, y, z }) {}

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> that contains the provided entries: 
		/// {<paramref name="entry0"/>, <paramref name="entry1"/>, <paramref name="entry2"/>}.
		/// </summary>
		/// <param name="entry0">The first entry of the new vector.</param>
		/// <param name="entry1">The second entry of the new vector.</param>
		/// <param name="entry2">The third entry of the new vector.</param>
		[Obsolete("Use Vector3(x,y,z)")]
        public static Vector3 Create(double entry0, double entry1, double entry2) => new Vector3(entry0, entry1, entry2);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> with <paramref name="data"/> or a clone as its internal array.
		/// </summary>
		/// <param name="data">The entries of the vector to create. Constraints: <paramref name="data"/>.Length == 3.</param>
		/// <param name="copyArray">If true, <paramref name="data"/> will be copied and the new <see cref="Vector3"/> instance 
		///     will have a reference to the copy, which is safer. If false, the new vector will have a reference to 
		///     <paramref name="data"/> itself, which is faster.</param>
		/// <returns></returns>
		[Obsolete("Use Vector3(data)")]
		public static Vector3 CreateFromArray(double[] data, bool copyArray = false)
        {
            if (data.Length != 3) throw new NonMatchingDimensionsException(
                $"The provided array had length = {data.Length} instead of 3");
            if (copyArray) return new Vector3(new double[] { data[0], data[1] });
            else return new Vector3(data);
        }

		/// <summary>
		/// Creates an new instance of <see cref="Vector3"/> with all entries being equal to 0.
		/// </summary>
		[Obsolete("Use new Vector3()")]
        public static Vector3 CreateZero() => new Vector3(new double[3]);

		#region operators
		public static Vector3 operator -(Vector3 v) => new Vector3(new double[] { -v[0], -v[1], -v[2] });

		/// <summary>
		/// Performs the operation: result[i] = <paramref name="v1"/>[i] + <paramref name="v2"/>[i], 
		/// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="v1">The first <see cref="Vector2"/> operand.</param>
		/// <param name="v2">The second <see cref="Vector2"/> operand.</param>
		public static Vector3 operator +(Vector3 v1, Vector3 v2) => new Vector3(new double[] { v1[0] + v2[0], v1[1] + v2[1], v1[2] + v2[2] });

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="v1"/>[i] - <paramref name="v2"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static Vector3 operator -(Vector3 v1, Vector3 v2) => new Vector3(new double[] { v1[0] - v2[0], v1[1] - v2[1], v1[2] - v2[2] });

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="scalar">The scalar value that will be multiplied with all vector entries.</param>
        /// <param name="vector">The vector to multiply.</param>
        public static Vector3 operator *(double scalar, Vector3 vector) => new Vector3(new double[] { scalar * vector[0], scalar * vector[1], scalar * vector[2] });

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * <paramref name="vector"/>[i],
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="vector">The vector to multiply.</param>
        /// <param name="scalar">The scalar value that will be multiplied with all vector entries.</param>
        public static Vector3 operator *(Vector3 vector, double scalar) => new Vector3(new double[] { scalar * vector[0], scalar * vector[1], scalar * vector[2] });

        /// <summary>
        /// Performs the operation: scalar = sum(<paramref name="v1"/>[i] * <paramref name="v2"/>[i]), 
        /// for 0 &lt;= i &lt; 3. The resulting entries are written to a new <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="v1">The first <see cref="Vector2"/> operand.</param>
        /// <param name="v2">The second <see cref="Vector2"/> operand.</param>
        public static double operator *(Vector3 v1, Vector3 v2) => v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
        #endregion

        /// <summary>
        /// Performs the operation: this[i] = this[i] + <paramref name="vector"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting vector overwrites the entries of this <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="vector">A vector with three entries.</param>
        public void AddIntoThis(Vector3 vector)
        {
            this[0] += vector[0];
            this[1] += vector[1];
            this[2] += vector[2];
        }

		[Obsolete("Use this.View(thisIndices).AddIntoThis(otherVector.View(otherIndices))")]
        public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

		[Obsolete("Use this.View(thisIndices).AddIntoThis(otherVector)")]
		public void AddIntoThisNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector)
            => DenseStrategies.AddNonContiguouslyFrom(this, thisIndices, otherVector);

		[Obsolete("Use this[index] += value")]
		public void AddToIndex(int index, double value) => this[index] += value;

		public Vector3 Axpy(Vector3 otherVector, double otherCoefficient) => new Vector3(new double[]
		{
            this[0] + otherCoefficient * otherVector[0],
            this[1] + otherCoefficient * otherVector[1],
			this[2] + otherCoefficient * otherVector[2]
        });

        public void AxpyIntoThis(Vector3 otherVector, double otherCoefficient)
        {
            this[0] += otherCoefficient * otherVector[0];
			this[1] += otherCoefficient * otherVector[1];
			this[2] += otherCoefficient * otherVector[2];
        }

		[Obsolete("Use this.View(destinationIndex, destinationIndex + length).AxpyIntoThis(otherVector.View(sourceIndex, sourceIndex + length), otherCoefficient)")]
		public void AxpySubvectorIntoThis(int destinationIndex, IExtendedImmutableVector otherVector, double otherCoefficient,
			int sourceIndex, int length) => View(destinationIndex, destinationIndex + length).AxpyIntoThis(otherVector.View(sourceIndex, sourceIndex + length), otherCoefficient);

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3"/> by deep copying the entries of this instance.
		/// </summary>
		public Vector3 Copy() => new Vector3((double[])Elements.Clone());


        /// <summary>
        /// See <see cref="IExtendedMutableVector.CopyNonContiguouslyFrom(int[], IExtendedImmutableVector, int[])"/>
        /// </summary>
        public void CopyNonContiguouslyFrom(int[] thisIndices, IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, thisIndices, otherVector, otherIndices);

        /// <summary>
        /// See <see cref="IExtendedMutableVector.CopyNonContiguouslyFrom(IExtendedImmutableVector, int[])"/>
        /// </summary>
        public void CopyNonContiguouslyFrom(IExtendedImmutableVector otherVector, int[] otherIndices)
            => DenseStrategies.CopyNonContiguouslyFrom(this, otherVector, otherIndices);

        /// <summary>
        /// Performs the operation: result = { a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0]}.
        /// The result is a vector. Also note that: other.Cross(this) = - this.Cross(other).
        /// </summary>
        /// <param name="vector">A vector with three entries.</param>
        public Vector3 CrossProduct(Vector3 vector) => new Vector3(base.CrossProduct(vector).Elements);

		/// <summary>
		/// See <see cref="IEntrywiseOperableView1D{TVectorIn, TVectorOut}.DoEntrywise(TVectorIn, Func{double, double, double})"/>.
		/// </summary>
		public Vector3 DoEntrywise(Vector3 vector, Func<double, double, double> binaryOperation) => new Vector3(base.DoEntrywise(vector, binaryOperation).Elements);

		/// <summary>
		/// See <see cref="IEntrywiseOperableView1D{TVectorIn, TVectorOut}.DoToAllEntries(Func{double, double})"/>.
		/// </summary>
		public Vector3 DoToAllEntries(Func<double, double> unaryOperation) => new Vector3(base.DoToAllEntries(unaryOperation).Elements);

        /// <summary>
        /// Calculates the dot (or inner/scalar) product of this vector with <paramref name="vector"/>:
        /// result = this[0] * <paramref name="vector"/>[0] + this[1] * <paramref name="vector"/>[1] 
        ///     + this[2] * <paramref name="vector"/>[2].
        /// </summary>
        /// <param name="vector">A vector with three entries</param>
        public double DotProduct(Vector3 vector) => this[0] * vector[0] + this[1] * vector[1] + this[2] * vector[2];

        /// <summary>
        /// See <see cref="IExtendedImmutableVector.LinearCombination(double, IExtendedImmutableVector, double)"/>.
        /// </summary>
        public IExtendedMutableVector LinearCombination(double thisCoefficient, IExtendedImmutableVector otherVector, double otherCoefficient)
        {
            if (otherVector is Vector3 casted) return LinearCombination(thisCoefficient, casted, otherCoefficient);
            else if (thisCoefficient == 1.0) return Axpy(otherVector, otherCoefficient);
            else
            {
                Preconditions.CheckVectorDimensions(this, otherVector);
                return new Vector3(new double[]
                {
                    thisCoefficient * data[0] + otherCoefficient * otherVector[0],
                    thisCoefficient * data[1] + otherCoefficient * otherVector[1],
                    thisCoefficient * data[2] + otherCoefficient * otherVector[2]
                });
            }
        }

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="thisCoefficient"/> * this[i] + 
        ///     <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting vector is written to a new <see cref="Vector3"/> and then returned.
        /// </summary>
        /// <param name="thisCoefficient">A scalar that multiplies each entry of this vector.</param>
        /// <param name="otherVector">A vector with three entries.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
        public Vector3 LinearCombination(double thisCoefficient, Vector3 otherVector, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) return Axpy(otherVector, otherCoefficient);
            return new Vector3(new double[] { thisCoefficient * this.data[0] + otherCoefficient * otherVector.data[0],
                thisCoefficient * this.data[1] + otherCoefficient * otherVector.data[1],
                thisCoefficient * this.data[2] + otherCoefficient * otherVector.data[2] });
        }

        /// <summary>
        /// See <see cref="IExtendedMutableVector.LinearCombinationIntoThis(double, IExtendedImmutableVector, double)"/>
        /// </summary>
        public void LinearCombinationIntoThis(double thisCoefficient, IExtendedImmutableVector otherVector, double otherCoefficient)
        {
            if (otherVector is Vector3 casted) LinearCombinationIntoThis(thisCoefficient, casted, otherCoefficient);
            else if (thisCoefficient == 1.0) AxpyIntoThis(otherVector, otherCoefficient);
            else
            {
                Preconditions.CheckVectorDimensions(this, otherVector);
                data[0] = thisCoefficient * data[0] * otherCoefficient * otherVector[0];
                data[1] = thisCoefficient * data[1] * otherCoefficient * otherVector[1];
                data[2] = thisCoefficient * data[2] * otherCoefficient * otherVector[2];
            }
        }

        /// <summary>
        /// Performs the operation: this[i] = <paramref name="thisCoefficient"/> * this[i] + 
        ///     <paramref name="otherCoefficient"/> * <paramref name="otherVector"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting vector overwrites the entries of this <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="thisCoefficient">A scalar that multiplies each entry of this vector.</param>
        /// <param name="otherVector">A vector with three entries.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherVector"/>.</param>
        public void LinearCombinationIntoThis(double thisCoefficient, Vector3 otherVector, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) AxpyIntoThis(otherVector, otherCoefficient);
            else
            {
                this.data[0] = thisCoefficient * this.data[0] * otherCoefficient * otherVector.data[0];
                this.data[1] = thisCoefficient * this.data[1] * otherCoefficient * otherVector.data[1];
                this.data[2] = thisCoefficient * this.data[2] * otherCoefficient * otherVector.data[2];
            }
        }

        /// <summary>
        /// See <see cref="IExtendedImmutableVector.Norm2"/>
        /// </summary>
        public double Norm2() => Math.Sqrt(data[0] * data[0] + data[1] * data[1] + data[2] * data[2]);

        /// <summary>
        /// See <see cref="IReducible.Reduce(double, ProcessEntry, ProcessZeros, Reduction.Finalize)"/>.
        /// </summary>
        public double Reduce(double identityValue, ProcessEntry processEntry, ProcessZeros processZeros, Finalize finalize)
        {
            // no zeros implied
            double accumulator = identityValue;
            accumulator = processEntry(data[0], accumulator);
            accumulator = processEntry(data[1], accumulator);
            accumulator = processEntry(data[2], accumulator);
            return finalize(accumulator);
        }

        /// <summary>
        /// See <see cref="IExtendedImmutableVector.Scale(double)"/>.
        /// </summary>
        IExtendedMutableVector IExtendedImmutableVector.Scale(double scalar) => Scale(scalar);

        /// <summary>
        /// Performs the operation: result[i] = <paramref name="scalar"/> * this[i],
        /// for 0 &lt;= i &lt; 23. The resulting vector is written to a new <see cref="Vector3"/> and then returned.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this vector.</param>
        public Vector3 Scale(double scalar) => new Vector3(new double[] { scalar * data[0], scalar * data[1], scalar * data[2] });

        /// <summary>
        /// Performs the operation: this[i] = <paramref name="scalar"/> * this[i],
        /// for 0 &lt;= i &lt; 3. The resulting vector overwrites the entries of this <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this vector.</param>
        public void ScaleIntoThis(double scalar)
        {
            data[0] *= scalar;
            data[1] *= scalar;
            data[2] *= scalar;
        }

        /// <summary>
        /// See <see cref="IExtendedMutableVector.Set(int, double)"/>
        /// </summary>
        public void Set(int index, double value)
        {
            data[index] = value;
        }

        ///<summary>
        /// Sets all entries of this vector to be equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value that all entries of the this vector will be equal to.</param>
        public void SetAll(double value)
        {
            data[0] = value;
            data[1] = value;
            data[2] = value;
        }

        /// <summary>
        /// Performs the operation: this[i] = this[i] - <paramref name="vector"/>[i], 
        /// for 0 &lt;= i &lt; 3. The resulting vector overwrites the entries of this <see cref="Vector3"/> instance.
        /// </summary>
        /// <param name="vector">A vector with three entries.</param>
        public void SubtractIntoThis(Vector3 vector)
        {
            this.data[0] -= vector.data[0];
            this.data[1] -= vector.data[1];
            this.data[2] -= vector.data[3];
        }
    }
}
#endif
