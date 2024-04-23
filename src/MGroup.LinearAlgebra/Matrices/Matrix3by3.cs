using System;
using MGroup.LinearAlgebra.Commons;
using MGroup.LinearAlgebra.Exceptions;
using MGroup.LinearAlgebra.Providers;
using MGroup.LinearAlgebra.Reduction;
using MGroup.LinearAlgebra.Vectors;

namespace MGroup.LinearAlgebra.Matrices
{
    /// <summary>
    /// A otherMatrix with 3 rows and 3 columns. Optimized version of <see cref="Matrix"/>.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    public class Matrix3by3 : IMatrix
    {
        private readonly double[,] data;

		public Matrix3by3() { data = new double[3, 3]; }

        private Matrix3by3(double[,] data)
        {
            this.data = data;
        }

		/// <summary>
		/// See <see cref="IIndexable2D.MatrixSymmetry"/>.
		/// </summary>
		public MatrixSymmetry MatrixSymmetry { get; set; }

		/// <summary>
		/// See <see cref="IIndexable2D.MatrixSymmetry"/>.
		/// </summary>
		MatrixSymmetry IIndexable2D.MatrixSymmetry => this.MatrixSymmetry;
		
		/// <summary>
		/// Returns true if <see cref="NumRows"/> == <see cref="NumColumns"/>.
		/// </summary>
		public bool IsSquare { get { return true; } }

        /// <summary>
        /// The number of columns of the otherMatrix. 
        /// </summary>
        public int NumColumns { get { return 3; } }

        /// <summary>
        /// The number of rows of the otherMatrix.
        /// </summary>
        public int NumRows { get { return 3; } }

        /// <summary>
        /// The internal array that stores the entries of the otherMatrix. It should only be used for passing the raw array to linear 
        /// algebra libraries.
        /// </summary>
        internal double[,] InternalData { get { return data; } }

        /// <summary>
        /// See <see cref="IIndexable2D.this[int, int]"/>.
        /// </summary>
        public double this[int rowIdx, int colIdx] //TODO: Should I add bound checking?
        {
            get { return data[rowIdx, colIdx]; }
            set { data[rowIdx, colIdx] = value; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> that contains the provided entries: 
        /// {{<paramref name="x00"/>, <paramref name="x01"/>, <paramref name="x02"/>}, 
        ///  {<paramref name="x10"/>, <paramref name="x11"/>, <paramref name="x12"/>}, 
        ///  {<paramref name="x20"/>, <paramref name="x21"/>, <paramref name="x22"/>}} 
        /// </summary>
        public static Matrix3by3 Create(double x00, double x01, double x02, double x10, double x11, double x12, 
            double x20, double x21, double x22) 
            => new Matrix3by3(new double[,] { { x00, x01, x02 }, { x10, x11, x12 }, { x20, x21, x22 } });

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> with <paramref name="array2D"/> or a clone as its internal 
        /// array.
        /// </summary>
        /// <param name="array2D">A 2-dimensional array containing the entries of the otherMatrix. Constraints 
        ///     <paramref name="array2D"/>.GetLength(0) ==  <paramref name="array2D"/>.GetLength(0) == 3.</param>
        /// <param name="copyArray">If true, <paramref name="array2D"/> will be copied and the new <see cref="Matrix3by3"/> 
        ///     instance will have a reference to the copy, which is safer. If false, the new otherMatrix will have a reference to 
        ///     <paramref name="array2D"/> itself, which is faster.</param>
        public static Matrix3by3 CreateFromArray(double[,] array2D, bool copyArray)
        {
            // TODO: more efficient checking
            if ((array2D.GetLength(0) != 3) || (array2D.GetLength(1) != 3)) throw new NonMatchingDimensionsException(
                $"The provided array was {array2D.GetLength(0)}-by{array2D.GetLength(1)} instead of 2-by-2");
            if (copyArray)
            {
                double[,] copy = new double[,] 
                { 
                    { array2D[0, 0], array2D[0, 1], array2D[0, 2] }, 
                    { array2D[1, 0], array2D[1, 1], array2D[1, 2] },
                    { array2D[2, 0], array2D[2, 1], array2D[2, 2] }
                };
                return new Matrix3by3(copy);
            }
            else return new Matrix3by3(array2D);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> that is equal to the identity otherMatrix, namely a square otherMatrix 
        /// with non-diagonal entries being equal to 0 and diagonal entries being equal to 1.
        /// </summary>
        public static Matrix3by3 CreateIdentity() 
            => new Matrix3by3(new double[,] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 } });

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> with all entries being equal to <paramref name="value"/>.
        /// </summary>
        public static Matrix3by3 CreateWithValue(double value)
            => new Matrix3by3(new double[,] { { value, value, value }, { value, value, value }, { value, value, value } });

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> with all entries being equal to 0.
        /// </summary> 
        public static Matrix3by3 CreateZero() => new Matrix3by3(new double[3, 3]);

        #region operators (use extension operators when they become available)
        /// <summary>
        /// Performs the operation: result[i, j] = <paramref name="m1"/>[i, j] + <paramref name="m2"/>[i, j], 
        /// for 0 &lt;= i, j &lt; 3. The resulting entries are written to a new <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="m1">The first <see cref="Matrix3by3"/> operand.</param>
        /// <param name="m2">The second <see cref="Matrix3by3"/> operand.</param>
        public static Matrix3by3 operator +(Matrix3by3 m1, Matrix3by3 m2)
        {
            return new Matrix3by3(new double[,]
            {
                { m1.data[0, 0] + m2.data[0, 0], m1.data[0, 1] + m2.data[0, 1], m1.data[0, 2] + m2.data[0, 2] },
                { m1.data[1, 0] + m2.data[1, 0], m1.data[1, 1] + m2.data[1, 1], m1.data[1, 2] + m2.data[1, 2] },
                { m1.data[2, 0] + m2.data[2, 0], m1.data[2, 1] + m2.data[2, 1], m1.data[2, 2] + m2.data[2, 2] }
            });
        }

        /// <summary>
        /// Performs the operation: result[i, j] = <paramref name="m1"/>[i, j] - <paramref name="m2"/>[i, j], 
        /// for 0 &lt;= i, j &lt; 3. The resulting entries are written to a new <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="m1">The first <see cref="Matrix3by3"/> operand.</param>
        /// <param name="m2">The second <see cref="Matrix3by3"/> operand.</param>
        public static Matrix3by3 operator -(Matrix3by3 m1, Matrix3by3 m2)
        {
            return new Matrix3by3(new double[,]
            {
                { m1.data[0, 0] - m2.data[0, 0], m1.data[0, 1] - m2.data[0, 1], m1.data[0, 2] - m2.data[0, 2] },
                { m1.data[1, 0] - m2.data[1, 0], m1.data[1, 1] - m2.data[1, 1], m1.data[1, 2] - m2.data[1, 2] },
                { m1.data[2, 0] - m2.data[2, 0], m1.data[2, 1] - m2.data[2, 1], m1.data[2, 2] - m2.data[2, 2] }
            });
        }

        /// <summary>
        /// Performs the operation: result[i, j] = <paramref name="scalar"/> * <paramref name="matrix"/>[i, j], 
        /// for 0 &lt;= i, j &lt; 3. The resulting entries are written to a new <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="scalar">The scalar value that will be multiplied with all vector entries.</param>
        /// <param name="matrix">The otherMatrix to multiply.</param>
        public static Matrix3by3 operator *(double scalar, Matrix3by3 matrix) => matrix.Scale(scalar);

        /// <summary>
        /// Performs the operation: result[i, j] = <paramref name="scalar"/> * <paramref name="matrix"/>[i, j], 
        /// for 0 &lt;= i, j &lt; 3. The resulting entries are written to a new <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="matrix">The otherMatrix to multiply.</param>
        /// <param name="scalar">The scalar value that will be multiplied with all vector entries.</param>
        public static Matrix3by3 operator *(Matrix3by3 matrix, double scalar) => matrix.Scale(scalar);

        /// <summary>
        /// Performs the otherMatrix-otherMatrix multiplication: result = <paramref name="matrixLeft"/> * <paramref name="matrixRight"/>.
        /// </summary>
        /// <param name="matrixLeft">The <see cref="Matrix3by3"/> operand on the left.</param>
        /// <param name="matrixRight">The <see cref="Matrix3by3"/> operand on the right.</param>
        public static Matrix3by3 operator *(Matrix3by3 matrixLeft, Matrix3by3 matrixRight)
            => matrixLeft.MultiplyRight(matrixRight, false, false);

        /// <summary>
        /// Performs the otherMatrix-vector multiplication: result = <paramref name="matrixLeft"/> * <paramref name="vectorRight"/>.
        /// </summary>
        /// <param name="matrixLeft">The <see cref="Matrix3by3"/> operand on the left.</param>
        /// <param name="vectorRight">The <see cref="Vector3"/> operand on the right. It can be considered as a column 
        ///     vector.</param>
        public static Vector3 operator *(Matrix3by3 matrixLeft, Vector3 vectorRight)
            => matrixLeft.Multiply(vectorRight, false);

        /// <summary>
        /// Performs the otherMatrix-vector multiplication: result = <paramref name="vectorLeft"/> * <paramref name="matrixRight"/>.
        /// </summary>
        /// <param name="vectorLeft">The <see cref="Vector3"/> operand on the left. It can be considered as a row vector.</param>
        /// <param name="matrixRight">The <see cref="Matrix3by3"/> operand on the right.</param>
        public static Vector3 operator *(Vector3 vectorLeft, Matrix3by3 matrixRight)
            => matrixRight.Multiply(vectorLeft, true);
        #endregion

        /// <inheritdoc/>
        public IMatrix Axpy(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
        {
            if (otherMatrix is Matrix3by3 casted) return Axpy(casted, otherCoefficient);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, otherMatrix);
                return new Matrix3by3(new double[,]
                {
                    {
                        data[0, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 0],
                        data[0, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 1],
                        data[0, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 2]
                    },
                    {
                        data[1, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 0],
                        data[1, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 1],
                        data[1, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 2]
                    },
                    {
                        data[2, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 0],
                        data[2, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 1],
                        data[2, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 2]
                    },
                });
            }
        }

        /// <summary>
        /// Performs the following operation for 0 &lt;= i, j &lt; 3:
        /// result[i, j] = <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j] + this[i, j]. 
        /// The resulting otherMatrix is written to a new <see cref="Matrix3by3"/> and then returned.
        /// </summary>
        /// <param name="otherMatrix">A otherMatrix with 3 rows and 3 columns.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
        public Matrix3by3 Axpy(Matrix3by3 otherMatrix, double otherCoefficient)
        {
            return new Matrix3by3(new double[,]
            {
                {
                    this.data[0, 0] + otherCoefficient * otherMatrix.data[0, 0],
                    this.data[0, 1] + otherCoefficient * otherMatrix.data[0, 1],
                    this.data[0, 2] + otherCoefficient * otherMatrix.data[0, 2]
                },
                {
                    this.data[1, 0] + otherCoefficient * otherMatrix.data[1, 0],
                    this.data[1, 1] + otherCoefficient * otherMatrix.data[1, 1],
                    this.data[1, 2] + otherCoefficient * otherMatrix.data[1, 2]
                },
                {
                    this.data[2, 0] + otherCoefficient * otherMatrix.data[2, 0],
                    this.data[2, 1] + otherCoefficient * otherMatrix.data[2, 1],
                    this.data[2, 2] + otherCoefficient * otherMatrix.data[2, 2]
                }
            });
        }

        /// <inheritdoc/>
        public void AxpyIntoThis(IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
        {
            if (otherMatrix is Matrix3by3 casted) AxpyIntoThis(casted, otherCoefficient);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, otherMatrix);
                data[0, 0] += otherCoefficient * ((IIndexable2D)otherMatrix)[0, 0];
                data[0, 1] += otherCoefficient * ((IIndexable2D)otherMatrix)[0, 1];
                data[0, 2] += otherCoefficient * ((IIndexable2D)otherMatrix)[0, 2];
                data[1, 0] += otherCoefficient * ((IIndexable2D)otherMatrix)[1, 0];
                data[1, 1] += otherCoefficient * ((IIndexable2D)otherMatrix)[1, 1];
                data[1, 2] += otherCoefficient * ((IIndexable2D)otherMatrix)[1, 2];
                data[2, 0] += otherCoefficient * ((IIndexable2D)otherMatrix)[2, 0];
                data[2, 1] += otherCoefficient * ((IIndexable2D)otherMatrix)[2, 1];
                data[2, 2] += otherCoefficient * ((IIndexable2D)otherMatrix)[2, 2];
            }
        }

        /// <summary>
        /// Performs the following operation for 0 &lt;= i, j &lt; 3:
        /// this[i, j] = <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j] + this[i, j]. 
        /// The resulting otherMatrix overwrites the entries of this <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="otherMatrix">A otherMatrix with 3 rows and 3 columns.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
        public void AxpyIntoThis(Matrix3by3 otherMatrix, double otherCoefficient)
        {
            this.data[0, 0] += otherCoefficient * otherMatrix.data[0, 0];
            this.data[0, 1] += otherCoefficient * otherMatrix.data[0, 1];
            this.data[0, 2] += otherCoefficient * otherMatrix.data[0, 2];
            this.data[1, 0] += otherCoefficient * otherMatrix.data[1, 0];
            this.data[1, 1] += otherCoefficient * otherMatrix.data[1, 1];
            this.data[1, 2] += otherCoefficient * otherMatrix.data[1, 2];
            this.data[2, 0] += otherCoefficient * otherMatrix.data[2, 0];
            this.data[2, 1] += otherCoefficient * otherMatrix.data[2, 1];
            this.data[2, 2] += otherCoefficient * otherMatrix.data[2, 2];
        }

        /// <summary>
        /// Calculates the determinant of this otherMatrix, which must be square. If the inverse otherMatrix is also needed, use
        /// <see cref="InvertAndDetermninant"/> instead.
        /// </summary>
        public double CalcDeterminant()
        {
            // Laplace formula: 
            // det = a00 * (a11 * a22 - a12 * a21) - a01 * (a10 * a22 - a12 * a20)  + a02 * (a10 * a21 - a11 * a20)
            return data[0, 0] * (data[1, 1] * data[2, 2] - data[1, 2] * data[2, 1])
                - data[0, 1] * (data[1, 0] * data[2, 2] - data[1, 2] * data[2, 0])
                + data[0, 2] * (data[1, 0] * data[2, 1] - data[1, 1] * data[2, 0]);
        }

        /// <inheritdoc/>
        public void Clear() => Array.Clear(data, 0, 9); //TODO: would it be faster to do it myself?

        /// <summary>
        /// See <see cref="IMatrixView.Copy(bool)"/>.
        /// </summary>
        IMatrix IMatrixView.Copy(bool copyIndexingData) => Copy();

        /// <summary>
        /// Initializes a new instance of <see cref="Matrix3by3"/> by copying the entries of this instance.
        /// </summary>
        public Matrix3by3 Copy()
        {
            return new Matrix3by3(new double[,] 
            { 
                { data[0, 0], data[0, 1], data[0, 2] }, 
                { data[1, 0], data[1, 1], data[1, 2] }, 
                { data[2, 0], data[2, 1], data[2, 2] }
            });
        }

        /// <summary>
        /// Copies the entries of the otherMatrix into a 2-dimensional array. The returned array has length(0) = length(1) = 3. 
        /// </summary>
        public double[,] CopyToArray2D()
        {
            return new double[,]
            {
                { data[0, 0], data[0, 1], data[0, 2] },
                { data[1, 0], data[1, 1], data[1, 2] },
                { data[2, 0], data[2, 1], data[2, 2] }
            };
        }

        /// <summary>
        /// See <see cref="IMatrixView.CopyToFullMatrix()"/>
        /// </summary>
        public Matrix CopyToFullMatrix() => Matrix.CreateFromArray(data);

        /// <inheritdoc/>
        public IMatrix DoEntrywise(IMinimalReadOnlyMatrix matrix, Func<double, double, double> binaryOperation)
        {
            if (matrix is Matrix3by3 casted) return DoEntrywise(casted, binaryOperation);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, matrix);
                return new Matrix3by3(new double[,]
                {
                    {
                        binaryOperation(data[0, 0], ((IIndexable2D) matrix)[0, 0]),
                        binaryOperation(data[0, 1], ((IIndexable2D) matrix)[0, 1]),
                        binaryOperation(data[0, 2], ((IIndexable2D) matrix)[0, 2])
                    },
                    {
                        binaryOperation(data[1, 0], ((IIndexable2D) matrix)[1, 0]),
                        binaryOperation(data[1, 1], ((IIndexable2D) matrix)[1, 1]),
                        binaryOperation(data[1, 2], ((IIndexable2D) matrix)[1, 2])
                    },
                    {
                        binaryOperation(data[2, 0], ((IIndexable2D) matrix)[2, 0]),
                        binaryOperation(data[2, 1], ((IIndexable2D) matrix)[2, 1]),
                        binaryOperation(data[2, 2], ((IIndexable2D) matrix)[2, 2])
                    }
                });
            }
        }

        /// <summary>
        /// See <see cref="IMatrixView.DoEntrywise(IMinimalReadOnlyMatrix, Func{double, double, double})"/>.
        /// </summary>
        public Matrix3by3 DoEntrywise(Matrix3by3 matrix, Func<double, double, double> binaryOperation)
        {
            return new Matrix3by3(new double[,]
            {
                {
                    binaryOperation(this.data[0, 0], matrix.data[0, 0]),
                    binaryOperation(this.data[0, 1], matrix.data[0, 1]),
                    binaryOperation(this.data[0, 2], matrix.data[0, 2])
                },
                {
                    binaryOperation(this.data[1, 0], matrix.data[1, 0]),
                    binaryOperation(this.data[1, 1], matrix.data[1, 1]),
                    binaryOperation(this.data[1, 2], matrix.data[1, 2])
                },
                {
                    binaryOperation(this.data[2, 0], matrix.data[2, 0]),
                    binaryOperation(this.data[2, 1], matrix.data[2, 1]),
                    binaryOperation(this.data[2, 2], matrix.data[2, 2])
                }
            });
        }

        /// <inheritdoc/>
        public void DoEntrywiseIntoThis(IMinimalReadOnlyMatrix otherMatrix, Func<double, double, double> binaryOperation)
        {
            if (otherMatrix is Matrix3by3 casted) DoEntrywiseIntoThis(casted, binaryOperation);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, otherMatrix);
                data[0, 0] = binaryOperation(data[0, 0], ((IIndexable2D)otherMatrix)[0, 0]);
                data[0, 1] = binaryOperation(data[0, 1], ((IIndexable2D)otherMatrix)[0, 1]);
                data[0, 2] = binaryOperation(data[0, 2], ((IIndexable2D)otherMatrix)[0, 2]);
                data[1, 0] = binaryOperation(data[1, 0], ((IIndexable2D)otherMatrix)[1, 0]);
                data[1, 1] = binaryOperation(data[1, 1], ((IIndexable2D)otherMatrix)[1, 1]);
                data[1, 2] = binaryOperation(data[1, 2], ((IIndexable2D)otherMatrix)[1, 2]);
                data[2, 0] = binaryOperation(data[2, 0], ((IIndexable2D)otherMatrix)[2, 0]);
                data[2, 1] = binaryOperation(data[2, 1], ((IIndexable2D)otherMatrix)[2, 1]);
                data[2, 2] = binaryOperation(data[2, 2], ((IIndexable2D)otherMatrix)[2, 2]);
            }
        }

		/// <summary>
		/// See <see cref="IMinimalMatrix.DoEntrywiseIntoThis(IMinimalReadOnlyMatrix, Func{double, double, double})"/>.
		/// </summary>
		public void DoEntrywiseIntoThis(Matrix3by3 matrix, Func<double, double, double> binaryOperation)
        {
            this.data[0, 0] = binaryOperation(this.data[0, 0], matrix.data[0, 0]);
            this.data[0, 1] = binaryOperation(this.data[0, 1], matrix.data[0, 1]);
            this.data[0, 2] = binaryOperation(this.data[0, 2], matrix.data[0, 2]);
            this.data[1, 0] = binaryOperation(this.data[1, 0], matrix.data[1, 0]);
            this.data[1, 1] = binaryOperation(this.data[1, 1], matrix.data[1, 1]);
            this.data[1, 2] = binaryOperation(this.data[1, 2], matrix.data[1, 2]);
            this.data[2, 0] = binaryOperation(this.data[2, 0], matrix.data[2, 0]);
            this.data[2, 1] = binaryOperation(this.data[2, 1], matrix.data[2, 1]);
            this.data[2, 2] = binaryOperation(this.data[2, 2], matrix.data[2, 2]);
        }

        /// <inheritdoc/>
        IMatrix IMatrixView.DoToAllEntries(Func<double, double> unaryOperation) => DoToAllEntries(unaryOperation);

		/// <summary>
		/// Performs a unary operation on each entry: result[i] = <paramref name="unaryOperation"/>(this[i, j]).
		/// The resulting otherMatrix is written in a new object and then returned.
		/// </summary>
		/// <param name="unaryOperation">A method that takes 1 argument and returns 1 result.</param>
		public Matrix3by3 DoToAllEntries(Func<double, double> unaryOperation)
        {
            return new Matrix3by3(new double[,]
            {
                { unaryOperation(data[0, 0]), unaryOperation(data[0, 1]), unaryOperation(data[0, 2]) },
                { unaryOperation(data[1, 0]), unaryOperation(data[1, 1]), unaryOperation(data[1, 2]) },
                { unaryOperation(data[2, 0]), unaryOperation(data[2, 1]), unaryOperation(data[2, 2]) }
            });
        }

        /// <inheritdoc/>
        public void DoToAllEntriesIntoThis(Func<double, double> unaryOperation)
        {
            data[0, 0] = unaryOperation(data[0, 0]);
            data[0, 1] = unaryOperation(data[0, 1]);
            data[0, 2] = unaryOperation(data[0, 2]);
            data[1, 0] = unaryOperation(data[1, 0]);
            data[1, 1] = unaryOperation(data[1, 1]);
            data[1, 2] = unaryOperation(data[1, 2]);
            data[2, 0] = unaryOperation(data[2, 0]);
            data[2, 1] = unaryOperation(data[2, 1]);
            data[2, 2] = unaryOperation(data[2, 2]);
        }

        /// <inheritdoc/>
        public bool Equals(IMinimalReadOnlyMatrix otherMatrix, double tolerance = 1e-13)
        {
            var comparer = new ValueComparer(1e-13);
            if (otherMatrix is Matrix3by3 casted)
            {
                return comparer.AreEqual(this.data[0, 0], casted.data[0, 0])
                    && comparer.AreEqual(this.data[0, 1], casted.data[0, 1])
                    && comparer.AreEqual(this.data[0, 2], casted.data[0, 2])
                    && comparer.AreEqual(this.data[1, 0], casted.data[1, 0])
                    && comparer.AreEqual(this.data[1, 1], casted.data[1, 1])
                    && comparer.AreEqual(this.data[1, 2], casted.data[1, 2])
                    && comparer.AreEqual(this.data[2, 0], casted.data[2, 0])
                    && comparer.AreEqual(this.data[2, 1], casted.data[2, 1])
                    && comparer.AreEqual(this.data[2, 2], casted.data[2, 2]);
            }
            else
            {
                return comparer.AreEqual(this.data[0, 0], ((IIndexable2D) otherMatrix)[0, 0])
                    && comparer.AreEqual(this.data[0, 1], ((IIndexable2D)otherMatrix)[0, 1])
                    && comparer.AreEqual(this.data[0, 2], ((IIndexable2D)otherMatrix)[0, 2])
                    && comparer.AreEqual(this.data[1, 0], ((IIndexable2D)otherMatrix)[1, 0])
                    && comparer.AreEqual(this.data[1, 1], ((IIndexable2D)otherMatrix)[1, 1])
                    && comparer.AreEqual(this.data[1, 2], ((IIndexable2D)otherMatrix)[1, 2])
                    && comparer.AreEqual(this.data[2, 0], ((IIndexable2D)otherMatrix)[2, 0])
                    && comparer.AreEqual(this.data[2, 1], ((IIndexable2D)otherMatrix)[2, 1])
                    && comparer.AreEqual(this.data[2, 2], ((IIndexable2D)otherMatrix)[2, 2]);
            }
        }

        /// <summary>
        /// See <see cref="ISliceable2D.GetColumn(int)"/>.
        /// </summary>
        public Vector GetColumn(int colIndex)
        {
            Preconditions.CheckIndexCol(this, colIndex);
            return new Vector(new double[] { data[0, colIndex], data[1, colIndex], data[2, colIndex] });
        }

        /// <summary>
        /// See <see cref="ISliceable2D.GetRow(int)"/>.
        /// </summary>
        public Vector GetRow(int rowIndex)
        {
            Preconditions.CheckIndexRow(this, rowIndex);
            return new Vector(new double[] { data[rowIndex, 0], data[rowIndex, 1], data[rowIndex, 2] });
        }

        /// <summary>
        /// See <see cref="ISliceable2D.GetSubmatrix(int[], int[])"/>.
        /// </summary>
        public IMatrix GetSubmatrix(int[] rowIndices, int[] colIndices)
            => DenseStrategies.GetSubmatrix(this, rowIndices, colIndices);

        /// <summary>
        /// See <see cref="ISliceable2D.GetSubmatrix(int, int, int, int)"/>.
        /// </summary>
        public IMatrix GetSubmatrix(int rowStartInclusive, int rowEndExclusive, int colStartInclusive, int colEndExclusive)
            => DenseStrategies.GetSubmatrix(this, rowStartInclusive, rowEndExclusive, colStartInclusive, colEndExclusive);

        /// <summary>
        /// Calculates the inverse otherMatrix and returns it in a new <see cref="Matrix3by3"/> instance. This only works if this 
        /// <see cref="Matrix3by3"/> is invertible. If the determinant otherMatrix is also needed, use 
        /// <see cref="InvertAndDetermninant"/> instead.
        /// </summary>
        /// <exception cref="SingularMatrixException">Thrown if the otherMatrix is not invertible.</exception>
        public Matrix3by3 Invert()
        {
            (Matrix3by3 inverse, double det) = InvertAndDetermninant();
            return inverse;
        }

        /// <summary>
        /// Calculates the determinant and the inverse otherMatrix and returns the latter in a new <see cref="Matrix"/> instance. 
        /// This only works if this <see cref="Matrix3by3"/> is invertible.
        /// </summary>
        /// <exception cref="SingularMatrixException">Thrown if the otherMatrix is not invertible.</exception>
        public (Matrix3by3 inverse, double determinant) InvertAndDetermninant()
        {
            // Minors of the first row (with signs)
            double c00 = data[1, 1] * data[2, 2] - data[1, 2] * data[2, 1];     // c00 = + a11*a22 - a12*a21
            double c01 = -data[1, 0] * data[2, 2] + data[1, 2] * data[2, 0];    // c01 = - a10*a22 + a12*a20
            double c02 = data[1, 0] * data[2, 1] - data[1, 1] * data[2, 0];     // c02 = + a10*a21 - a11*a20
            double det = data[0, 0] * c00 + data[0, 1] * c01 + data[0, 2] * c02; // Laplace: det = a00*c00 + a01*c01 + a02*c02 
            if (Math.Abs(det) < AnalyticFormulas.determinantTolerance)
            {
                throw new SingularMatrixException($"Determinant == {det}");
            }

            // Cramer's rule: inverse = 1/det * transpose(C), C = otherMatrix of minors
            double[,] inverse = new double[3, 3];
            inverse[0, 0] = c00 / det; // inv[0,0]: c10
            inverse[1, 0] = c01 / det; // inv[1,0]: c01
            inverse[2, 0] = c02 / det; // inv[2,0]: c02
            inverse[0, 1] = (-data[0, 1] * data[2, 2] + data[0, 2] * data[2, 1]) / det;    // inv[0,1]: c10 = - a01*a22 + a02*a21
            inverse[1, 1] = (data[0, 0] * data[2, 2] - data[0, 2] * data[2, 0]) / det;     // inv[1,1]: c11 = + a00*a22 - a02*a20
            inverse[2, 1] = (-data[0, 0] * data[2, 1] + data[0, 1] * data[2, 0]) / det;    // inv[2,1]: c12 = - a00*a21 + a01*a20
            inverse[0, 2] = (data[0, 1] * data[1, 2] - data[0, 2] * data[1, 1]) / det;     // inv[0,2]: c20 = + a01*a12 - a02*a11
            inverse[1, 2] = (-data[0, 0] * data[1, 2] + data[0, 2] * data[1, 0]) / det;    // inv[1,2]: c21 = - a00*a12 + a02*a10
            inverse[2, 2] = (data[0, 0] * data[1, 1] - data[0, 1] * data[1, 0]) / det;     // inv[2,2]: c22 = + a00*a11 - a01*a10

            return (new Matrix3by3(inverse), det);
        }

        /// <inheritdoc/>
        public IMatrix LinearCombination(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) return Axpy(otherMatrix, otherCoefficient);
            else if (otherMatrix is Matrix3by3 casted) return LinearCombination(thisCoefficient, casted, otherCoefficient);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, otherMatrix);
                return new Matrix3by3(new double[,]
                {
                    {
                        thisCoefficient * data[0, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 0],
                        thisCoefficient * data[0, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 1],
                        thisCoefficient * data[0, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 2]

                    },
                    {
                        thisCoefficient * data[1, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 0],
                        thisCoefficient * data[1, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 1],
                        thisCoefficient * data[1, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 2]
                    },
                    {
                        thisCoefficient * data[2, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 0],
                        thisCoefficient * data[2, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 1],
                        thisCoefficient * data[2, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 2]
                    }
                });
            }
        }

        /// <summary>
        /// Performs the following operation for 0 &lt;= i, j &lt; 3:
        /// result[i, j] = <paramref name="thisCoefficient"/> * this[i, j] 
        ///     + <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j]. 
        /// The resulting otherMatrix is written to a new <see cref="Matrix3by3"/> and then returned.
        /// </summary>
        /// <param name="thisCoefficient">A scalar that multiplies each entry of this <see cref="Matrix3by3"/>.</param>
        /// <param name="otherMatrix">A otherMatrix with 3 rows and 3 columns.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
        public Matrix3by3 LinearCombination(double thisCoefficient, Matrix3by3 otherMatrix, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) return Axpy(otherMatrix, otherCoefficient);
            else return new Matrix3by3(new double[,]
            {
                {
                    thisCoefficient * this.data[0, 0] + otherCoefficient * otherMatrix.data[0, 0],
                    thisCoefficient * this.data[0, 1] + otherCoefficient * otherMatrix.data[0, 1],
                    thisCoefficient * this.data[0, 2] + otherCoefficient * otherMatrix.data[0, 2]
                },
                {
                    thisCoefficient * this.data[1, 0] + otherCoefficient * otherMatrix.data[1, 0],
                    thisCoefficient * this.data[1, 1] + otherCoefficient * otherMatrix.data[1, 1],
                    thisCoefficient * this.data[1, 2] + otherCoefficient * otherMatrix.data[1, 2]
                },
                {
                    thisCoefficient * this.data[2, 0] + otherCoefficient * otherMatrix.data[2, 0],
                    thisCoefficient * this.data[2, 1] + otherCoefficient * otherMatrix.data[2, 1],
                    thisCoefficient * this.data[2, 2] + otherCoefficient * otherMatrix.data[2, 2]
                }
            });
        }

        /// <inheritdoc/>
        public void LinearCombinationIntoThis(double thisCoefficient, IMinimalReadOnlyMatrix otherMatrix, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) AxpyIntoThis(otherMatrix, otherCoefficient);
            else if (otherMatrix is Matrix3by3 casted) LinearCombinationIntoThis(thisCoefficient, casted, otherCoefficient);
            else
            {
                Preconditions.CheckSameMatrixDimensions(this, otherMatrix);
                data[0, 0] = thisCoefficient * data[0, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 0];
                data[0, 1] = thisCoefficient * data[0, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 1];
                data[0, 2] = thisCoefficient * data[0, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[0, 2];
                data[1, 0] = thisCoefficient * data[1, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 0];
                data[1, 1] = thisCoefficient * data[1, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 1];
                data[1, 2] = thisCoefficient * data[1, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[1, 2];
                data[2, 0] = thisCoefficient * data[2, 0] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 0];
                data[2, 1] = thisCoefficient * data[2, 1] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 1];
                data[2, 2] = thisCoefficient * data[2, 2] + otherCoefficient * ((IIndexable2D)otherMatrix)[2, 2];
            }
        }

        /// <summary>
        /// Performs the following operation for 0 &lt;= i, j &lt; 3:
        /// result[i, j] = <paramref name="thisCoefficient"/> * this[i, j] 
        ///     + <paramref name="otherCoefficient"/> * <paramref name="otherMatrix"/>[i, j]. 
        /// The resulting otherMatrix overwrites the entries of this <see cref="Matrix3by3"/> instance.
        /// </summary>
        /// <param name="thisCoefficient">A scalar that multiplies each entry of this <see cref="Matrix3by3"/>.</param>
        /// <param name="otherMatrix">A otherMatrix with 3 rows and 3 columns.</param>
        /// <param name="otherCoefficient">A scalar that multiplies each entry of <paramref name="otherMatrix"/>.</param>
        public void LinearCombinationIntoThis(double thisCoefficient, Matrix3by3 otherMatrix, double otherCoefficient)
        {
            if (thisCoefficient == 1.0) AxpyIntoThis(otherMatrix, otherCoefficient);
            else
            {
                this.data[0, 0] = thisCoefficient * this.data[0, 0] + otherCoefficient * otherMatrix.data[0, 0];
                this.data[0, 1] = thisCoefficient * this.data[0, 1] + otherCoefficient * otherMatrix.data[0, 1];
                this.data[0, 2] = thisCoefficient * this.data[0, 2] + otherCoefficient * otherMatrix.data[0, 2];
                this.data[1, 0] = thisCoefficient * this.data[1, 0] + otherCoefficient * otherMatrix.data[1, 0];
                this.data[1, 1] = thisCoefficient * this.data[1, 1] + otherCoefficient * otherMatrix.data[1, 1];
                this.data[1, 2] = thisCoefficient * this.data[1, 2] + otherCoefficient * otherMatrix.data[1, 2];
                this.data[2, 0] = thisCoefficient * this.data[2, 0] + otherCoefficient * otherMatrix.data[2, 0];
                this.data[2, 1] = thisCoefficient * this.data[2, 1] + otherCoefficient * otherMatrix.data[2, 1];
                this.data[2, 2] = thisCoefficient * this.data[2, 2] + otherCoefficient * otherMatrix.data[2, 2];
            }
        }

        /// <summary>
        /// See <see cref="IMatrixView.MultiplyLeft(IMatrixView, bool, bool)"/>.
        /// </summary>
        public Matrix MultiplyLeft(IMatrixView matrix, bool transposeThis = false, bool transposeOther = false)
        {
            // For now piggy back on Matrix. TODO: Optimize this
            double[] colMajor = new double[] {
                data[0, 0], data[1, 0], data[2, 0], data[0, 1], data[1, 1], data[2, 1], data[0, 2], data[1, 2], data[2, 2] };
            return Matrix.CreateFromArray(colMajor, 3, 3, false).MultiplyLeft(matrix, transposeThis, transposeOther);
        }

        /// <summary>
        /// See <see cref="IMatrixView.MultiplyRight(IMatrixView, bool, bool)"/>.
        /// </summary>
        public Matrix MultiplyRight(IMatrixView matrix, bool transposeThis = false, bool transposeOther = false)
        {
            // For now piggy back on Matrix. TODO: Optimize this
            double[] colMajor = new double[] {
                data[0, 0], data[1, 0], data[2, 0], data[0, 1], data[1, 1], data[2, 1], data[0, 2], data[1, 2], data[2, 2] };
            return Matrix.CreateFromArray(colMajor, 3, 3, false).MultiplyRight(matrix, transposeThis, transposeOther);
        }

        /// <summary>
        /// Performs the otherMatrix-otherMatrix multiplication: oper(this) * oper(<paramref name="matrix"/>).
        /// </summary>
        /// <param name="matrix">A otherMatrix with 3 rows and 3 columns.</param>
        /// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
        /// <param name="transposeOther">If true, oper(<paramref name="matrix"/>) = transpose(<paramref name="matrix"/>). 
        ///     Otherwise oper(<paramref name="matrix"/>) = <paramref name="matrix"/>.</param>
        public Matrix3by3 MultiplyRight(Matrix3by3 matrix, bool transposeThis = false, bool transposeOther = false)
        {
            double[,] result = new double[3, 3];
            if (transposeThis)
            {
                if (transposeOther)
                {
                    result[0, 0] = this.data[0, 0] * matrix.data[0, 0] + this.data[1, 0] * matrix.data[0, 1] + this.data[2, 0] * matrix.data[0, 2];
                    result[0, 1] = this.data[0, 0] * matrix.data[1, 0] + this.data[1, 0] * matrix.data[1, 1] + this.data[2, 0] * matrix.data[1, 2];
                    result[0, 2] = this.data[0, 0] * matrix.data[2, 0] + this.data[1, 0] * matrix.data[2, 1] + this.data[2, 0] * matrix.data[2, 2];
                    result[1, 0] = this.data[0, 1] * matrix.data[0, 0] + this.data[1, 1] * matrix.data[0, 1] + this.data[2, 1] * matrix.data[0, 2];
                    result[1, 1] = this.data[0, 1] * matrix.data[1, 0] + this.data[1, 1] * matrix.data[1, 1] + this.data[2, 1] * matrix.data[1, 2];
                    result[1, 2] = this.data[0, 1] * matrix.data[2, 0] + this.data[1, 1] * matrix.data[2, 1] + this.data[2, 1] * matrix.data[2, 2];
                    result[2, 0] = this.data[0, 2] * matrix.data[0, 0] + this.data[1, 2] * matrix.data[0, 1] + this.data[2, 2] * matrix.data[0, 2];
                    result[2, 1] = this.data[0, 2] * matrix.data[1, 0] + this.data[1, 2] * matrix.data[1, 1] + this.data[2, 2] * matrix.data[1, 2];
                    result[2, 2] = this.data[0, 2] * matrix.data[2, 0] + this.data[1, 2] * matrix.data[2, 1] + this.data[2, 2] * matrix.data[2, 2];
                }
                else
                {
                    result[0, 0] = this.data[0, 0] * matrix.data[0, 0] + this.data[1, 0] * matrix.data[1, 0] + this.data[2, 0] * matrix.data[2, 0];
                    result[0, 1] = this.data[0, 0] * matrix.data[0, 1] + this.data[1, 0] * matrix.data[1, 1] + this.data[2, 0] * matrix.data[2, 1];
                    result[0, 2] = this.data[0, 0] * matrix.data[0, 2] + this.data[1, 0] * matrix.data[1, 2] + this.data[2, 0] * matrix.data[2, 2];
                    result[1, 0] = this.data[0, 1] * matrix.data[0, 0] + this.data[1, 1] * matrix.data[1, 0] + this.data[2, 1] * matrix.data[2, 0];
                    result[1, 1] = this.data[0, 1] * matrix.data[0, 1] + this.data[1, 1] * matrix.data[1, 1] + this.data[2, 1] * matrix.data[2, 1];
                    result[1, 2] = this.data[0, 1] * matrix.data[0, 2] + this.data[1, 1] * matrix.data[1, 2] + this.data[2, 1] * matrix.data[2, 2];
                    result[2, 0] = this.data[0, 2] * matrix.data[0, 0] + this.data[2, 2] * matrix.data[1, 0] + this.data[2, 2] * matrix.data[2, 0];
                    result[2, 1] = this.data[0, 2] * matrix.data[0, 1] + this.data[2, 2] * matrix.data[1, 1] + this.data[2, 2] * matrix.data[2, 1];
                    result[2, 2] = this.data[0, 2] * matrix.data[0, 2] + this.data[2, 2] * matrix.data[1, 2] + this.data[2, 2] * matrix.data[2, 2];
                }
            }
            else
            {
                if (transposeOther)
                {
                    result[0, 0] = this.data[0, 0] * matrix.data[0, 0] + this.data[0, 1] * matrix.data[0, 1] + this.data[0, 2] * matrix.data[0, 2];
                    result[0, 1] = this.data[0, 0] * matrix.data[1, 0] + this.data[0, 1] * matrix.data[1, 1] + this.data[0, 2] * matrix.data[1, 2];
                    result[0, 2] = this.data[0, 0] * matrix.data[2, 0] + this.data[0, 1] * matrix.data[2, 1] + this.data[0, 2] * matrix.data[2, 2];
                    result[1, 0] = this.data[1, 0] * matrix.data[0, 0] + this.data[1, 1] * matrix.data[0, 1] + this.data[1, 2] * matrix.data[0, 2];
                    result[1, 1] = this.data[1, 0] * matrix.data[1, 0] + this.data[1, 1] * matrix.data[1, 1] + this.data[1, 2] * matrix.data[1, 2];
                    result[1, 2] = this.data[1, 0] * matrix.data[2, 0] + this.data[1, 1] * matrix.data[2, 1] + this.data[1, 2] * matrix.data[2, 2];
                    result[2, 0] = this.data[2, 0] * matrix.data[0, 0] + this.data[2, 1] * matrix.data[0, 1] + this.data[2, 2] * matrix.data[0, 2];
                    result[2, 1] = this.data[2, 0] * matrix.data[1, 0] + this.data[2, 1] * matrix.data[1, 1] + this.data[2, 2] * matrix.data[1, 2];
                    result[2, 2] = this.data[2, 0] * matrix.data[2, 0] + this.data[2, 1] * matrix.data[2, 1] + this.data[2, 2] * matrix.data[2, 2];
                }
                else
                {
                    result[0, 0] = this.data[0, 0] * matrix.data[0, 0] + this.data[0, 1] * matrix.data[1, 0] + this.data[0, 2] * matrix.data[2, 0];
                    result[0, 1] = this.data[0, 0] * matrix.data[0, 1] + this.data[0, 1] * matrix.data[1, 1] + this.data[0, 2] * matrix.data[2, 1];
                    result[0, 2] = this.data[0, 0] * matrix.data[0, 2] + this.data[0, 1] * matrix.data[1, 2] + this.data[0, 2] * matrix.data[2, 2];
                    result[1, 0] = this.data[1, 0] * matrix.data[0, 0] + this.data[1, 1] * matrix.data[1, 0] + this.data[1, 2] * matrix.data[2, 0];
                    result[1, 1] = this.data[1, 0] * matrix.data[0, 1] + this.data[1, 1] * matrix.data[1, 1] + this.data[1, 2] * matrix.data[2, 1];
                    result[1, 2] = this.data[1, 0] * matrix.data[0, 2] + this.data[1, 1] * matrix.data[1, 2] + this.data[1, 2] * matrix.data[2, 2];
                    result[2, 0] = this.data[2, 0] * matrix.data[0, 0] + this.data[2, 1] * matrix.data[1, 0] + this.data[2, 2] * matrix.data[2, 0];
                    result[2, 1] = this.data[2, 0] * matrix.data[0, 1] + this.data[2, 1] * matrix.data[1, 1] + this.data[2, 2] * matrix.data[2, 1];
                    result[2, 2] = this.data[2, 0] * matrix.data[0, 2] + this.data[2, 1] * matrix.data[1, 2] + this.data[2, 2] * matrix.data[2, 2];
                }
            }
            return new Matrix3by3(result);
        }

        /// <inheritdoc/>
        public Vector Multiply(IMinimalReadOnlyVector vector, bool transposeThis = false)
        {
            if (vector is Vector3 casted) return Multiply(casted, transposeThis);

            Preconditions.CheckMultiplicationDimensions(2, vector.Length);
			AbstractFullyPopulatedVector vectorFull = (AbstractFullyPopulatedVector)vector;
			if (transposeThis)
            {
                return new Vector(new double[]
                {
                    data[0, 0] * vectorFull[0] + data[1, 0] * vectorFull[1] + data[2, 0] * vectorFull[2],
                    data[0, 1] * vectorFull[0] + data[1, 1] * vectorFull[1] + data[2, 1] * vectorFull[2],
                    data[0, 2] * vectorFull[0] + data[1, 2] * vectorFull[1] + data[2, 2] * vectorFull[2]
                });
            }
            else
            {
                return new Vector(new double[]
                {
                    data[0, 0] * vectorFull[0] + data[0, 1] * vectorFull[1] + data[0, 2] * vectorFull[2],
                    data[1, 0] * vectorFull[0] + data[1, 1] * vectorFull[1] + data[1, 2] * vectorFull[2],
                    data[2, 0] * vectorFull[0] + data[2, 1] * vectorFull[1] + data[2, 2] * vectorFull[2]
                });
            }
        }

        /// <summary>
        /// Performs the otherMatrix-vector multiplication: oper(this) * <paramref name="vector"/>.
        /// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
        /// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
        /// </summary>
        /// <param name="vector">A vector with 3 entries.</param>
        /// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
        public Vector3 Multiply(Vector3 vector, bool transposeThis = false)
        {
            double[] x = vector.Values;
            if (transposeThis)
            {
                return new Vector3(
                    data[0, 0] * x[0] + data[1, 0] * x[1] + data[2, 0] * x[2],
                    data[0, 1] * x[0] + data[1, 1] * x[1] + data[2, 1] * x[2],
                    data[0, 2] * x[0] + data[1, 2] * x[1] + data[2, 2] * x[2]);
            }
            else
            {
                return new Vector3(
                    data[0, 0] * x[0] + data[0, 1] * x[1] + data[0, 2] * x[2],
                    data[1, 0] * x[0] + data[1, 1] * x[1] + data[1, 2] * x[2],
                    data[2, 0] * x[0] + data[2, 1] * x[1] + data[2, 2] * x[2]);
            }
        }

        /// <inheritdoc/>
        public void MultiplyIntoResult(IMinimalReadOnlyVector lhsVector, IMinimalVector rhsVector, bool transposeThis)
        {
            if ((lhsVector is Vector2 lhsDense) && (rhsVector is Vector2 rhsDense))
            {
                MultiplyIntoResult(lhsDense, rhsDense, transposeThis);
            }

            Preconditions.CheckMultiplicationDimensions(2, lhsVector.Length);
            Preconditions.CheckSystemSolutionDimensions(2, rhsVector.Length);
			AbstractFullyPopulatedVector rhsFull = (AbstractFullyPopulatedVector)rhsVector;
			AbstractFullyPopulatedVector lhsFull = (AbstractFullyPopulatedVector)lhsVector;
			if (transposeThis)
            {
				rhsFull[0] = data[0, 0] * lhsFull[0] + data[1, 0] * lhsFull[1] + data[2, 0] * lhsFull[2];
				rhsFull[1] = data[0, 1] * lhsFull[0] + data[1, 1] * lhsFull[1] + data[2, 1] * lhsFull[2];
				rhsFull[2] = data[0, 2] * lhsFull[0] + data[1, 2] * lhsFull[1] + data[2, 2] * lhsFull[2];
            }
            else
            {
				rhsFull[0] = data[0, 0] * lhsFull[0] + data[0, 1] * lhsFull[1] + data[0, 2] * lhsFull[2];
				rhsFull[1] = data[1, 0] * lhsFull[0] + data[1, 1] * lhsFull[1] + data[1, 2] * lhsFull[2];
				rhsFull[2] = data[2, 0] * lhsFull[0] + data[2, 1] * lhsFull[1] + data[2, 2] * lhsFull[2];
            }
        }
		public void MultiplyIntoResult(IMinimalReadOnlyVector lhsVector, IMinimalVector rhsVector) => MultiplyIntoResult(lhsVector, rhsVector, false);

		/// <summary>
		/// Performs the otherMatrix-vector multiplication: <paramref name="rhsVector"/> = oper(this) * <paramref name="lhsVector"/>.
		/// To multiply this * columnVector, set <paramref name="transposeThis"/> to false.
		/// To multiply rowVector * this, set <paramref name="transposeThis"/> to true.
		/// The resulting vector will overwrite the entries of <paramref name="rhsVector"/>.
		/// </summary>
		/// <param name="lhsVector">
		/// The vector that will be multiplied by this otherMatrix. It sits on the left hand side of the equation y = oper(A) * x.
		/// Constraints: <paramref name="lhsVector"/>.<see cref="IMinimalReadOnlyVector.Length"/> 
		/// == oper(this).<see cref="IBounded2D.NumColumns"/>.
		/// </param>
		/// <param name="rhsVector">
		/// The vector that will be overwritten by the result of the multiplication. It sits on the right hand side of the 
		/// equation y = oper(A) * x. Constraints: <paramref name="lhsVector"/>.<see cref="IMinimalReadOnlyVector.Length"/> 
		/// == oper(this).<see cref="IBounded2D.NumRows"/>.
		/// </param>
		/// <param name="transposeThis">If true, oper(this) = transpose(this). Otherwise oper(this) = this.</param>
		public void MultiplyIntoResult(Vector3 lhsVector, Vector3 rhsVector, bool transposeThis = false)
        {
            double[] x = lhsVector.Values;
            double[] y = rhsVector.Values;
            if (transposeThis)
            {
                y[0] = data[0, 0] * x[0] + data[1, 0] * x[1] + data[2, 0] * x[2];
                y[1] = data[0, 1] * x[0] + data[1, 1] * x[1] + data[2, 1] * x[2];
                y[2] = data[0, 2] * x[0] + data[1, 2] * x[1] + data[2, 2] * x[2];
            }
            else
            {
                y[0] = data[0, 0] * x[0] + data[0, 1] * x[1] + data[0, 2] * x[2];
                y[1] = data[1, 0] * x[0] + data[1, 1] * x[1] + data[1, 2] * x[2];
                y[2] = data[2, 0] * x[0] + data[2, 1] * x[1] + data[2, 2] * x[2];
            }
        }

        /// <summary>
        /// See <see cref="IReducible.Reduce(double, ProcessEntry, ProcessZeros, Reduction.Finalize)"/>.
        /// </summary>
        public double Reduce(double identityValue, ProcessEntry processEntry, ProcessZeros processZeros, Finalize finalize)
        {
            double aggregator = identityValue;
            double accumulator = identityValue; // no zeros implied
            accumulator = processEntry(data[0, 0], processEntry(data[0, 1], processEntry(data[0, 2],
                processEntry(data[1, 0], processEntry(data[1, 1], processEntry(data[1, 2],
                processEntry(data[2, 0], processEntry(data[2, 1], processEntry(data[2, 2], accumulator)))))))));
            return finalize(accumulator);
        }

        /// <inheritdoc/>
        IMatrix IMatrixView.Scale(double scalar) => Scale(scalar);

        /// <summary>
        /// Performs the following operation for 0 &lt;= i, j &lt; 2:
        /// result[i, j] = <paramref name="scalar"/> * this[i, j].
        /// The resulting otherMatrix is written to a new <see cref="Matrix3by3"/> and then returned.
        /// </summary>
        /// <param name="scalar">A scalar that multiplies each entry of this otherMatrix.</param>
        public Matrix3by3 Scale(double scalar)
        {
            return new Matrix3by3(new double[,]
            {
                { scalar * data[0, 0], scalar * data[0, 1], scalar * data[0, 2] },
                { scalar * data[1, 0], scalar * data[1, 1], scalar * data[1, 2] },
                { scalar * data[2, 0], scalar * data[2, 1], scalar * data[2, 2] }
            });
        }

        /// <inheritdoc/>
        public void ScaleIntoThis(double scalar)
        {
            data[0, 0] *= scalar; data[0, 1] *= scalar; data[0, 2] *= scalar;
            data[1, 0] *= scalar; data[1, 1] *= scalar; data[1, 2] *= scalar;
            data[2, 0] *= scalar; data[2, 1] *= scalar; data[2, 2] *= scalar;
        }

        /// <summary>
        /// Sets all entries of this otherMatrix to be equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value that all entries of the this otherMatrix will be equal to.</param>
        public void SetAll(double value)
        {
            data[0, 0] = value; data[0, 1] = value; data[0, 2] = value;
            data[1, 0] = value; data[1, 1] = value; data[1, 2] = value;
            data[2, 0] = value; data[2, 1] = value; data[2, 2] = value;
        }

        /// <summary>
        /// See <see cref="IMatrix.SetEntryRespectingPattern(int, int, double)"/>.
        /// </summary>
        public void SetEntryRespectingPattern(int rowIdx, int colIdx, double value) => data[rowIdx, colIdx] = value;

        /// <summary>
        /// See <see cref="IMatrixView.Transpose"/>.
        /// </summary>
        IMatrix IMatrixView.Transpose() => Transpose();

        /// <summary>
        /// Initializes a new <see cref="Matrix3by3"/> instance, that is transpose to this: result[i, j] = this[j, i].  
        /// The entries will be explicitly copied.
        /// </summary>
        public Matrix3by3 Transpose()
        {
            return new Matrix3by3(new double[,] 
            {
                { data[0, 0], data[1, 0], data[2, 0] }, 
                { data[0, 1], data[1, 1], data[2, 1] },
                { data[0, 2], data[1, 2], data[2, 2] }
            });
        }

		public void AddIntoThis(IMinimalReadOnlyMatrix otherMatrix) => AxpyIntoThis(otherMatrix, 1);
		public void SubtractIntoThis(IMinimalReadOnlyMatrix otherMatrix) => AxpyIntoThis(otherMatrix, -1);
		public IMinimalMatrix Add(IMinimalReadOnlyMatrix otherMatrix) => Axpy(otherMatrix, 1);
		public IMinimalMatrix Subtract(IMinimalReadOnlyMatrix otherMatrix) => Axpy(otherMatrix, -1);

		public Matrix3by3 CreateZeroWithSameFormat() => new Matrix3by3();
		IMinimalMatrix IMinimalReadOnlyMatrix.CreateZeroWithSameFormat() => new Matrix3by3();

	}
}
