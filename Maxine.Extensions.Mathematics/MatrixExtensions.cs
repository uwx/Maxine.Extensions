using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Mathematics;

public static class MatrixExtensions
{
    extension(ref Matrix matrix)
    {
        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        public void Transpose()
        {
            (matrix.M21, matrix.M12) = (matrix.M12, matrix.M21);
            (matrix.M31, matrix.M13) = (matrix.M13, matrix.M31);
            (matrix.M41, matrix.M14) = (matrix.M14, matrix.M41);

            (matrix.M32, matrix.M23) = (matrix.M23, matrix.M32);
            (matrix.M42, matrix.M24) = (matrix.M24, matrix.M42);

            (matrix.M43, matrix.M34) = (matrix.M34, matrix.M43);
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
        public void SetComponent(int index, float value)
        {
            switch (index)
            {
                case 0: matrix.M11 = value; break;
                case 1: matrix.M12 = value; break;
                case 2: matrix.M13 = value; break;
                case 3: matrix.M14 = value; break;
                case 4: matrix.M21 = value; break;
                case 5: matrix.M22 = value; break;
                case 6: matrix.M23 = value; break;
                case 7: matrix.M24 = value; break;
                case 8: matrix.M31 = value; break;
                case 9: matrix.M32 = value; break;
                case 10: matrix.M33 = value; break;
                case 11: matrix.M34 = value; break;
                case 12: matrix.M41 = value; break;
                case 13: matrix.M42 = value; break;
                case 14: matrix.M43 = value; break;
                case 15: matrix.M44 = value; break;
                default: ThrowArgumentOutOfRangeException(); break;
            }

            [DoesNotReturn]
            void ThrowArgumentOutOfRangeException()
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive.");
            }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="row">The row of the matrix to access.</param>
        /// <param name="column">The column of the matrix to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent(int row, int column, float value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(row, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(row, 3);
            ArgumentOutOfRangeException.ThrowIfLessThan(column, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(column, 3);

            matrix.SetComponent((row * 4) + column, value);
        }

        /// <summary>
        /// Gets or sets the first row in the matrix; that is M11, M12, M13, and M14.
        /// </summary>
        public Vector4 Row1
        {
            get { return new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14); }
            set { matrix.M11 = value.X; matrix.M12 = value.Y; matrix.M13 = value.Z; matrix.M14 = value.W; }
        }

        /// <summary>
        /// Gets or sets the second row in the matrix; that is matrix.M21, matrix.M22, matrix.M23, and matrix.M24.
        /// </summary>
        public Vector4 Row2
        {
            get { return new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24); }
            set { matrix.M21 = value.X; matrix.M22 = value.Y; matrix.M23 = value.Z; matrix.M24 = value.W; }
        }

        /// <summary>
        /// Gets or sets the third row in the matrix; that is matrix.M31, matrix.M32, matrix.M33, and matrix.M34.
        /// </summary>
        public Vector4 Row3
        {
            get { return new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34); }
            set { matrix.M31 = value.X; matrix.M32 = value.Y; matrix.M33 = value.Z; matrix.M34 = value.W; }
        }

        /// <summary>
        /// Gets or sets the fourth row in the matrix; that is matrix.M41, matrix.M42, matrix.M43, and matrix.M44.
        /// </summary>
        public Vector4 Row4
        {
            get { return new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44); }
            set { matrix.M41 = value.X; matrix.M42 = value.Y; matrix.M43 = value.Z; matrix.M44 = value.W; }
        }

        /// <summary>
        /// Gets or sets the first column in the matrix; that is matrix.M11, matrix.M21, matrix.M31, and matrix.M41.
        /// </summary>
        public Vector4 Column1
        {
            get { return new Vector4(matrix.M11, matrix.M21, matrix.M31, matrix.M41); }
            set { matrix.M11 = value.X; matrix.M21 = value.Y; matrix.M31 = value.Z; matrix.M41 = value.W; }
        }

        /// <summary>
        /// Gets or sets the second column in the matrix; that is matrix.M12, matrix.M22, matrix.M32, and matrix.M42.
        /// </summary>
        public Vector4 Column2
        {
            get { return new Vector4(matrix.M12, matrix.M22, matrix.M32, matrix.M42); }
            set { matrix.M12 = value.X; matrix.M22 = value.Y; matrix.M32 = value.Z; matrix.M42 = value.W; }
        }

        /// <summary>
        /// Gets or sets the third column in the matrix; that is matrix.M13, matrix.M23, matrix.M33, and matrix.M43.
        /// </summary>
        public Vector4 Column3
        {
            get { return new Vector4(matrix.M13, matrix.M23, matrix.M33, matrix.M43); }
            set { matrix.M13 = value.X; matrix.M23 = value.Y; matrix.M33 = value.Z; matrix.M43 = value.W; }
        }

        /// <summary>
        /// Gets or sets the fourth column in the matrix; that is matrix.M14, matrix.M24, matrix.M34, and matrix.M44.
        /// </summary>
        public Vector4 Column4
        {
            get { return new Vector4(matrix.M14, matrix.M24, matrix.M34, matrix.M44); }
            set { matrix.M14 = value.X; matrix.M24 = value.Y; matrix.M34 = value.Z; matrix.M44 = value.W; }
        }

        /// <summary>
        /// Gets or sets the translation of the matrix; that is matrix.M41, matrix.M42, and matrix.M43.
        /// </summary>
        public Vector3 TranslationVector
        {
            get { return new Vector3(matrix.M41, matrix.M42, matrix.M43); }
            set { matrix.M41 = value.X; matrix.M42 = value.Y; matrix.M43 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the scale of the matrix; that is matrix.M11, matrix.M22, and matrix.M33.
        /// </summary>
        /// <remarks>This property does not do any computation and will return a correct scale vector only if the matrix is a scale matrix.</remarks>
        public Vector3 ScaleVector
        {
            get { return new Vector3(matrix.M11, matrix.M22, matrix.M33); }
            set { matrix.M11 = value.X; matrix.M22 = value.Y; matrix.M33 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the up <see cref="Vector3"/> of the matrix; that is matrix.M21, matrix.M22, and matrix.M23.
        /// </summary>
        public Vector3 Up
        {
            get { return new Vector3(matrix.M21, matrix.M22, matrix.M23); }
            set { matrix.M21 = value.X; matrix.M22 = value.Y; matrix.M23 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the down <see cref="Vector3"/> of the matrix; that is -M21, -M22, and -M23.
        /// </summary>
        public Vector3 Down
        {
            get { return new Vector3(-matrix.M21, -matrix.M22, -matrix.M23); }
            set { matrix.M21 = -value.X; matrix.M22 = -value.Y; matrix.M23 = -value.Z; }
        }

        /// <summary>
        /// Gets or sets the right <see cref="Vector3"/> of the matrix; that is matrix.M11, matrix.M12, and matrix.M13.
        /// </summary>
        public Vector3 Right
        {
            get { return new Vector3(matrix.M11, matrix.M12, matrix.M13); }
            set { matrix.M11 = value.X; matrix.M12 = value.Y; matrix.M13 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the left <see cref="Vector3"/> of the matrix; that is -M11, -M12, and -M13.
        /// </summary>
        public Vector3 Left
        {
            get { return new Vector3(-matrix.M11, -matrix.M12, -matrix.M13); }
            set { matrix.M11 = -value.X; matrix.M12 = -value.Y; matrix.M13 = -value.Z; }
        }

        /// <summary>
        /// Gets or sets the forward <see cref="Vector3"/> of the matrix; that is -M31, -M32, and -M33.
        /// </summary>
        public Vector3 Forward
        {
            get { return new Vector3(-matrix.M31, -matrix.M32, -matrix.M33); }
            set { matrix.M31 = -value.X; matrix.M32 = -value.Y; matrix.M33 = -value.Z; }
        }

        /// <summary>
        /// Gets or sets the backward <see cref="Vector3"/> of the matrix; that is matrix.M31, matrix.M32, and matrix.M33.
        /// </summary>
        public Vector3 Backward
        {
            get { return new Vector3(matrix.M31, matrix.M32, matrix.M33); }
            set { matrix.M31 = value.X; matrix.M32 = value.Y; matrix.M33 = value.Z; }
        }

        /// <summary>
        /// Exchanges two rows in the matrix.
        /// </summary>
        /// <param name="firstRow">The first row to exchange. This is an index of the row starting at zero.</param>
        /// <param name="secondRow">The second row to exchange. This is an index of the row starting at zero.</param>
        public void ExchangeRows(int firstRow, int secondRow)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(firstRow, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(firstRow, 3);
            ArgumentOutOfRangeException.ThrowIfLessThan(secondRow, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(secondRow, 3);

            if (firstRow == secondRow)
                return;

            float temp0 = matrix.Component(secondRow, 0);
            float temp1 = matrix.Component(secondRow, 1);
            float temp2 = matrix.Component(secondRow, 2);
            float temp3 = matrix.Component(secondRow, 3);

            matrix.SetComponent(secondRow, 0, matrix.Component(firstRow, 0));
            matrix.SetComponent(secondRow, 1, matrix.Component(firstRow, 1));
            matrix.SetComponent(secondRow, 2, matrix.Component(firstRow, 2));
            matrix.SetComponent(secondRow, 3, matrix.Component(firstRow, 3));

            matrix.SetComponent(firstRow, 0, temp0);
            matrix.SetComponent(firstRow, 1, temp1);
            matrix.SetComponent(firstRow, 2, temp2);
            matrix.SetComponent(firstRow, 3, temp3);
        }

        /// <summary>
        /// Exchange columns.
        /// </summary>
        /// <param name="firstColumn">The first column to exchange.</param>
        /// <param name="secondColumn">The second column to exchange.</param>
        public void ExchangeColumns(int firstColumn, int secondColumn)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(firstColumn, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(firstColumn, 3);
            ArgumentOutOfRangeException.ThrowIfLessThan(secondColumn, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(secondColumn, 3);

            if (firstColumn == secondColumn)
                return;

            float temp0 = matrix.Component(0, secondColumn);
            float temp1 = matrix.Component(1, secondColumn);
            float temp2 = matrix.Component(2, secondColumn);
            float temp3 = matrix.Component(3, secondColumn);

            matrix.SetComponent(0, secondColumn, matrix.Component(0, firstColumn));
            matrix.SetComponent(1, secondColumn, matrix.Component(1, firstColumn));
            matrix.SetComponent(2, secondColumn, matrix.Component(2, firstColumn));
            matrix.SetComponent(3, secondColumn, matrix.Component(3, firstColumn));

            matrix.SetComponent(0, firstColumn, temp0);
            matrix.SetComponent(1, firstColumn, temp1);
            matrix.SetComponent(2, firstColumn, temp2);
            matrix.SetComponent(3, firstColumn, temp3);
        }


    }
    
    extension(Matrix matrix)
    {
        /// <summary>
        /// A <see cref="Matrix"/> with all of its components set to zero.
        /// </summary>
        public static Matrix Zero => new();

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Component(int index)
        {
            return index switch
            {
                0 => matrix.M11,
                1 => matrix.M12,
                2 => matrix.M13,
                3 => matrix.M14,
                4 => matrix.M21,
                5 => matrix.M22,
                6 => matrix.M23,
                7 => matrix.M24,
                8 => matrix.M31,
                9 => matrix.M32,
                10 => matrix.M33,
                11 => matrix.M34,
                12 => matrix.M41,
                13 => matrix.M42,
                14 => matrix.M43,
                15 => matrix.M44,
                _ => ThrowArgumentOutOfRangeException(),
            };

            [DoesNotReturn]
            float ThrowArgumentOutOfRangeException()
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive.");
            }
        }

        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and then transpose the output.</para>
        /// </remarks>
        public void Orthogonalize()
        {
            Orthogonalize(ref matrix, out matrix);
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and then transpose the output.</para>
        /// </remarks>
        public void Orthonormalize()
        {
            Orthonormalize(ref matrix, out matrix);
        }

        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthogonalize.</param>
        /// <param name="result">When the method completes, contains the orthogonalized matrix.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthogonalize(ref readonly Matrix value, out Matrix result)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            result = value;

            var row1 = result.Row1;
            var row2 = result.Row2;
            var row3 = result.Row3;
            var row4 = result.Row4;

            row2 -= Vector4.Dot(row1, row2) / Vector4.Dot(row1, row1) * row1;

            row3 -= Vector4.Dot(row1, row3) / Vector4.Dot(row1, row1) * row1;
            row3 -= Vector4.Dot(row2, row3) / Vector4.Dot(row2, row2) * row2;

            row4 -= Vector4.Dot(row1, row4) / Vector4.Dot(row1, row1) * row1;
            row4 -= Vector4.Dot(row2, row4) / Vector4.Dot(row2, row2) * row2;
            row4 -= Vector4.Dot(row3, row4) / Vector4.Dot(row3, row3) * row3;

            result.Row2 = row2;
            result.Row3 = row3;
            result.Row4 = row4;
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthonormalize.</param>
        /// <param name="result">When the method completes, contains the orthonormalized matrix.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthonormalize(ref readonly Matrix value, out Matrix result)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            var v = value;
            var row1 = v.Row1;
            var row2 = v.Row2;
            var row3 = v.Row3;
            var row4 = v.Row4;

            row1.Normalize();

            row2 -= Vector4.Dot(row1, row2) * row1;
            row2.Normalize();

            row3 -= Vector4.Dot(row1, row3) * row1;
            row3 -= Vector4.Dot(row2, row3) * row2;
            row3.Normalize();

            row4 -= Vector4.Dot(row1, row4) * row1;
            row4 -= Vector4.Dot(row2, row4) * row2;
            row4 -= Vector4.Dot(row3, row4) * row3;
            row4.Normalize();

            result = default;
            result.Row1 = row1;
            result.Row2 = row2;
            result.Row3 = row3;
            result.Row4 = row4;
        }

        /// <summary>
        /// Decomposes a matrix into an orthonormalized matrix Q and a right triangular matrix R.
        /// </summary>
        /// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
        /// <param name="R">When the method completes, contains the right triangular matrix of the decomposition.</param>
        public void DecomposeQR(out Matrix Q, out Matrix R)
        {
            Matrix temp = matrix;
            temp.Transpose();
            Orthonormalize(ref temp, out Q);
            Q.Transpose();

            R = new Matrix
            {
                M11 = Vector4.Dot(Q.Column1, matrix.Column1),
                M12 = Vector4.Dot(Q.Column1, matrix.Column2),
                M13 = Vector4.Dot(Q.Column1, matrix.Column3),
                M14 = Vector4.Dot(Q.Column1, matrix.Column4),

                M22 = Vector4.Dot(Q.Column2, matrix.Column2),
                M23 = Vector4.Dot(Q.Column2, matrix.Column3),
                M24 = Vector4.Dot(Q.Column2, matrix.Column4),

                M33 = Vector4.Dot(Q.Column3, matrix.Column3),
                M34 = Vector4.Dot(Q.Column3, matrix.Column4),

                M44 = Vector4.Dot(Q.Column4, matrix.Column4)
            };
        }

        /// <summary>
        /// Decomposes a matrix into a lower triangular matrix L and an orthonormalized matrix Q.
        /// </summary>
        /// <param name="L">When the method completes, contains the lower triangular matrix of the decomposition.</param>
        /// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
        public void DecomposeLQ(out Matrix L, out Matrix Q)
        {
            Orthonormalize(ref matrix, out Q);

            L = new Matrix
            {
                M11 = Vector4.Dot(Q.Row1, matrix.Row1),

                M21 = Vector4.Dot(Q.Row1, matrix.Row2),
                M22 = Vector4.Dot(Q.Row2, matrix.Row2),

                M31 = Vector4.Dot(Q.Row1, matrix.Row3),
                M32 = Vector4.Dot(Q.Row2, matrix.Row3),
                M33 = Vector4.Dot(Q.Row3, matrix.Row3),

                M41 = Vector4.Dot(Q.Row1, matrix.Row4),
                M42 = Vector4.Dot(Q.Row2, matrix.Row4),
                M43 = Vector4.Dot(Q.Row3, matrix.Row4),
                M44 = Vector4.Dot(Q.Row4, matrix.Row4)
            };
        }

        /// <summary>
        /// Decomposes a rotation matrix with the specified yaw, pitch, roll value (angles in radians).
        /// </summary>
        /// <param name="yaw">The yaw component in radians.</param>
        /// <param name="pitch">The pitch component in radians.</param>
        /// <param name="roll">The roll component in radians.</param>
        /// <remarks>
        /// This rotation matrix can be represented by <b>intrinsic</b> rotations in the order <paramref name="yaw"/>, <paramref name="pitch"/>, then <paramref name="roll"/>.
        /// <br/>
        /// Therefore, the <b>extrinsic</b> rotations to achieve this matrix is the reversed order of operations,
        /// i.e. Matrix.RotationZ(roll) * Matrix.RotationX(pitch) * Matrix.RotationY(yaw)
        /// </remarks>
        public void Decompose(out float yaw, out float pitch, out float roll)
        {
            // Adapted from 'Euler Angle Formulas' by David Eberly - https://www.geometrictools.com/Documentation/EulerAngles.pdf
            // 2.3 Factor as Ry Rx Rz
            // License under CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/)
            //
            // Note the Stride's matrix row/column ordering is swapped, indices starts at one,
            // and the if-statement ordering is written to minimize the number of operations to get to
            // the common case, and made to handle the +/- 1 cases better due to low precision in floats
            if (MathUtil.IsOne(Math.Abs(matrix.M32)))
            {
                if (matrix.M32 >= 0)
                {
                    // Edge case where M32 == +1
                    pitch = -MathUtil.PiOverTwo;
                    yaw = MathF.Atan2(-matrix.M21, matrix.M11);
                    roll = 0;
                }
                else
                {
                    // Edge case where M32 == -1
                    pitch = MathUtil.PiOverTwo;
                    yaw = -MathF.Atan2(-matrix.M21, matrix.M11);
                    roll = 0;
                }
            }
            else
            {
                // Common case
                pitch = MathF.Asin(-matrix.M32);
                yaw = MathF.Atan2(matrix.M31, matrix.M33);
                roll = MathF.Atan2(matrix.M12, matrix.M22);
            }
        }

        // /// <summary>
        // /// Decomposes a rotation matrix with the specified X, Y and Z euler angles in radians.
        // /// Matrix.RotationX(rotation.X) * Matrix.RotationY(rotation.Y) * Matrix.RotationZ(rotation.Z) should represent the same rotation.
        // /// </summary>
        // /// <param name="rotation">The vector containing the 3 rotations angles to be applied in order.</param>
        // public void DecomposeXYZ(out Vector3 rotation)
        // {
        //     // Adapted from 'Euler Angle Formulas' by David Eberly - https://www.geometrictools.com/Documentation/EulerAngles.pdf
        //     // 2.6 Factor as Rz Ry Rx
        //     // License under CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/)
        //     //
        //     // Note the Stride's matrix row/column ordering is swapped, indices starts at one,
        //     // and the if-statement ordering is written to minimize the number of operations to get to
        //     // the common case, and made to handle the +/- 1 cases better due to low precision in floats.
        //     // The above documentation implies the *extrinsic* rotation order is X-Y-Z,
        //     // so the *intrinsic* rotation is Z-Y-X which is the formula to use here
        //     if (MathUtil.IsOne(Math.Abs(matrix.M13)))
        //     {
        //         if (matrix.M13 >= 0)
        //         {
        //             // Edge case where M13 == +1
        //             rotation.Y = -MathUtil.PiOverTwo;
        //             rotation.Z = MathF.Atan2(-matrix.M32, matrix.M22);
        //             rotation.X = 0;
        //         }
        //         else
        //         {
        //             // Edge case where M13 == -1
        //             rotation.Y = MathUtil.PiOverTwo;
        //             rotation.Z = -MathF.Atan2(-matrix.M32, matrix.M22);
        //             rotation.X = 0;
        //         }
        //     }
        //     else
        //     {
        //         // Common case
        //         rotation.Y = MathF.Asin(-matrix.M13);
        //         rotation.Z = MathF.Atan2(matrix.M12, matrix.M11);
        //         rotation.X = MathF.Atan2(matrix.M23, matrix.M33);
        //     }
        // }

        /// <summary>
        /// Decomposes a matrix into a scale, rotation, and translation.
        /// </summary>
        /// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
        /// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
        /// <returns><c>true</c> if a rotation exist for this matrix, <c>false</c> otherwise.</returns>
        /// <remarks>This method is designed to decompose an SRT transformation matrix only.</remarks>
        public bool Decompose(out Vector3 scale, out Vector3 translation)
        {
            //Source: Unknown
            //References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

            //Get the translation.
            translation.X = matrix.M41;
            translation.Y = matrix.M42;
            translation.Z = matrix.M43;

            //Scaling is the length of the rows.
            scale.X = MathF.Sqrt((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12) + (matrix.M13 * matrix.M13));
            scale.Y = MathF.Sqrt((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22) + (matrix.M23 * matrix.M23));
            scale.Z = MathF.Sqrt((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32) + (matrix.M33 * matrix.M33));

            //If any of the scaling factors are zero, than the rotation matrix can not exist.
            return MathF.Abs(scale.X) >= MathUtil.ZeroTolerance &&
                MathF.Abs(scale.Y) >= MathUtil.ZeroTolerance &&
                MathF.Abs(scale.Z) >= MathUtil.ZeroTolerance;
        }

        /// <summary>
        /// Decomposes a matrix into a scale, rotation, and translation.
        /// </summary>
        /// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
        /// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
        /// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
        /// <remarks>
        /// This method is designed to decompose an SRT transformation matrix only.
        /// </remarks>
        public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            matrix.Decompose(out scale, out Matrix rotationMatrix, out translation);
            Quaternion.RotationMatrix(ref rotationMatrix, out rotation);
            return true;
        }

        /// <summary>
        /// Decomposes a matrix into a scale, rotation, and translation.
        /// </summary>
        /// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
        /// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
        /// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
        /// <remarks>
        /// This method is designed to decompose an SRT transformation matrix only.
        /// </remarks>
        public bool Decompose(out Vector3 scale, out Matrix rotation, out Vector3 translation)
        {
            //Source: Unknown
            //References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

            //Get the translation.
            translation.X = matrix.M41;
            translation.Y = matrix.M42;
            translation.Z = matrix.M43;

            //Scaling is the length of the rows.
            scale.X = MathF.Sqrt((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12) + (matrix.M13 * matrix.M13));
            scale.Y = MathF.Sqrt((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22) + (matrix.M23 * matrix.M23));
            scale.Z = MathF.Sqrt((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32) + (matrix.M33 * matrix.M33));

            //If any of the scaling factors are zero, then the rotation matrix can not exist.
            if (MathF.Abs(scale.X) < MathUtil.ZeroTolerance ||
                MathF.Abs(scale.Y) < MathUtil.ZeroTolerance ||
                MathF.Abs(scale.Z) < MathUtil.ZeroTolerance)
            {
                rotation = Matrix.Identity;
                return false;
            }

            // Calculate a perfect orthonormal matrix (no reflections)
            var at = new Vector3(matrix.M31 / scale.Z, matrix.M32 / scale.Z, matrix.M33 / scale.Z);
            var up = Vector3.Cross(at, new Vector3(matrix.M11 / scale.X, matrix.M12 / scale.X, matrix.M13 / scale.X));
            var right = Vector3.Cross(up, at);

            rotation = Matrix.Identity;
            rotation.Right = right;
            rotation.Up = up;
            rotation.Backward = at;

            // In case of reflexions
            scale.X = Vector3.Dot(right, matrix.Right) > 0.0f ? scale.X : -scale.X;
            scale.Y = Vector3.Dot(up, matrix.Up) > 0.0f ? scale.Y : -scale.Y;
            scale.Z = Vector3.Dot(at, matrix.Backward) > 0.0f ? scale.Z : -scale.Z;

            return true;
        }
        
        /// <summary>
        /// Creates a matrix that contains both the X, Y and Z rotation, as well as scaling and translation. Note: This function is NOT thead safe.
        /// </summary>
        /// <param name="scaling">The scaling.</param>
        /// <param name="rotation">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="result">When the method completes, contains the created rotation matrix.</param>
        public static void CreateTransformation(ref readonly Vector3 scaling, ref readonly Quaternion rotation, ref readonly Vector3 translation, out Matrix result)
        {
            // Equivalent to:
            //result =
            //    Matrix.Scaling(scaling)
            //    *Matrix.RotationX(rotation.X)
            //    *Matrix.RotationY(rotation.Y)
            //    *Matrix.RotationZ(rotation.Z)
            //    *Matrix.Position(translation);

            // Rotation
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (zz + xx));
            result.M23 = 2.0f * (yz + xw);
            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (yy + xx));

            // Position
            result.M41 = translation.X;
            result.M42 = translation.Y;
            result.M43 = translation.Z;

            // Scale
            if (scaling.X != 1.0f)
            {
                result.M11 *= scaling.X;
                result.M12 *= scaling.X;
                result.M13 *= scaling.X;
            }
            if (scaling.Y != 1.0f)
            {
                result.M21 *= scaling.Y;
                result.M22 *= scaling.Y;
                result.M23 *= scaling.Y;
            }
            if (scaling.Z != 1.0f)
            {
                result.M31 *= scaling.Z;
                result.M32 *= scaling.Z;
                result.M33 *= scaling.Z;
            }

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void CreateAffineTransformation(float scaling, ref readonly Quaternion rotation, ref readonly Vector3 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix CreateAffineTransformation(float scaling, Quaternion rotation, Vector3 translation)
        {
            CreateAffineTransformation(scaling, ref rotation, ref translation, out var result);
            return result;
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void CreateAffineTransformation(float scaling, ref readonly Vector3 rotationCenter, ref readonly Quaternion rotation, ref readonly Vector3 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling) * Matrix.CreateTranslation(-rotationCenter) * Matrix.CreateFromQuaternion(rotation) *
                     Matrix.CreateTranslation(rotationCenter) * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix CreateAffineTransformation(float scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            CreateAffineTransformation(scaling, ref rotationCenter, ref rotation, ref translation, out var result);
            return result;
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void CreateAffineTransformation2D(float scaling, float rotation, ref readonly Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling, scaling, 1.0f) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(translation.X, translation.Y, 0));
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix CreateAffineTransformation2D(float scaling, float rotation, Vector2 translation)
        {
            CreateAffineTransformation2D(scaling, rotation, ref translation, out var result);
            return result;
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void CreateAffineTransformation2D(float scaling, ref readonly Vector2 rotationCenter, float rotation, ref readonly Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling, scaling, 1.0f) * Matrix.CreateTranslation(new Vector3(-rotationCenter.X, -rotationCenter.Y, 0)) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(rotationCenter.X, rotationCenter.Y, 0)) * Matrix.CreateTranslation(new Vector3(translation.X, translation.Y, 0));
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix CreateAffineTransformation2D(float scaling, Vector2 rotationCenter, float rotation, Vector2 translation)
        {
            CreateAffineTransformation2D(scaling, ref rotationCenter, rotation, ref translation, out var result);
            return result;
        }

        /// <summary>
        /// Creates a transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created transformation matrix.</param>
        public static void CreateTransformation(ref readonly Vector3 scalingCenter, ref readonly Quaternion scalingRotation, ref readonly Vector3 scaling, ref readonly Vector3 rotationCenter, ref readonly Quaternion rotation, ref readonly Vector3 translation, out Matrix result)
        {
            Matrix sr = Matrix.CreateFromQuaternion(scalingRotation);

            result = Matrix.CreateTranslation(-scalingCenter) * Matrix.Transpose(sr) * Matrix.CreateScale(scaling) * sr * Matrix.CreateTranslation(scalingCenter) * Matrix.CreateTranslation(-rotationCenter) *
                Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(rotationCenter) * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created transformation matrix.</returns>
        public static Matrix CreateTransformation(Vector3 scalingCenter, Quaternion scalingRotation, Vector3 scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            CreateTransformation(ref scalingCenter, ref scalingRotation, ref scaling, ref rotationCenter, ref rotation, ref translation, out var result);
            return result;
        }

        /// <summary>
        /// Creates a 2D transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created transformation matrix.</param>
        public static void CreateTransformation2D(ref readonly Vector2 scalingCenter, float scalingRotation, ref readonly Vector2 scaling, ref readonly Vector2 rotationCenter, float rotation, ref readonly Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateTranslation(new Vector3(-scalingCenter.X, -scalingCenter.Y, 0)) * Matrix.CreateRotationZ(-scalingRotation) * Matrix.CreateScale(new Vector3(scaling.X, scaling.Y, 1)) * Matrix.CreateRotationZ(scalingRotation) * Matrix.CreateTranslation(new Vector3(scalingCenter.X, scalingCenter.Y, 0)) *
                Matrix.CreateTranslation(new Vector3(-rotationCenter.X, -rotationCenter.Y, 0)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(rotationCenter.X, rotationCenter.Y, 0)) * Matrix.CreateTranslation(new Vector3(translation.X, translation.Y, 0));

            result.M33 = 1f;
            result.M44 = 1f;
        }

        /// <summary>
        /// Creates a 2D transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created transformation matrix.</returns>
        public static Matrix CreateTransformation2D(Vector2 scalingCenter, float scalingRotation, Vector2 scaling, Vector2 rotationCenter, float rotation, Vector2 translation)
        {
            CreateTransformation2D(ref scalingCenter, scalingRotation, ref scaling, ref rotationCenter, rotation, ref translation, out var result);
            return result;
        }

    }
}

public static class MatrixExtensions2
{
    extension(Matrix matrix)
    {
        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthogonalize.</param>
        /// <returns>The orthogonalized matrix.</returns>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static Matrix Orthogonalize(Matrix value)
        {
            Matrix.Orthogonalize(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthonormalize.</param>
        /// <returns>The orthonormalized matrix.</returns>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static Matrix Orthonormalize(Matrix value)
        {
            Matrix.Orthonormalize(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public static Matrix FromAllComponents(float value)
        {
            var m = new Matrix();
            m.M11 = m.M21 = m.M31 = m.M41 =
                m.M12 = m.M22 = m.M32 = m.M42 =
                    m.M13 = m.M23 = m.M33 = m.M43 =
                        m.M14 = m.M24 = m.M34 = m.M44 = value;
            return m;
        }
        

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <param name="result">When the method completes, contains the shadow matrix.</param>
        public static void CreateShadow(ref readonly Vector4 light, ref readonly Plane plane, out Matrix result)
        {
            float dot = (plane.Normal.X * light.X) + (plane.Normal.Y * light.Y) + (plane.Normal.Z * light.Z) + (plane.D * light.W);
            float x = -plane.Normal.X;
            float y = -plane.Normal.Y;
            float z = -plane.Normal.Z;
            float d = -plane.D;

            result.M11 = (x * light.X) + dot;
            result.M21 = y * light.X;
            result.M31 = z * light.X;
            result.M41 = d * light.X;
            result.M12 = x * light.Y;
            result.M22 = (y * light.Y) + dot;
            result.M32 = z * light.Y;
            result.M42 = d * light.Y;
            result.M13 = x * light.Z;
            result.M23 = y * light.Z;
            result.M33 = (z * light.Z) + dot;
            result.M43 = d * light.Z;
            result.M14 = x * light.W;
            result.M24 = y * light.W;
            result.M34 = z * light.W;
            result.M44 = (d * light.W) + dot;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <returns>The shadow matrix.</returns>
        public static Matrix CreateShadow(Vector4 light, Plane plane)
        {
            CreateShadow(ref light, ref plane, out var result);
            return result;
        }
        
        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two matrices.</param>
        public static void SmoothStep(ref readonly Matrix start, ref readonly Matrix end, float amount, out Matrix result)
        {
            amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
            amount = amount * amount * (3.0f - (2.0f * amount));

            result.M11 = start.M11 + ((end.M11 - start.M11) * amount);
            result.M21 = start.M21 + ((end.M21 - start.M21) * amount);
            result.M31 = start.M31 + ((end.M31 - start.M31) * amount);
            result.M41 = start.M41 + ((end.M41 - start.M41) * amount);
            result.M12 = start.M12 + ((end.M12 - start.M12) * amount);
            result.M22 = start.M22 + ((end.M22 - start.M22) * amount);
            result.M32 = start.M32 + ((end.M32 - start.M32) * amount);
            result.M42 = start.M42 + ((end.M42 - start.M42) * amount);
            result.M13 = start.M13 + ((end.M13 - start.M13) * amount);
            result.M23 = start.M23 + ((end.M23 - start.M23) * amount);
            result.M33 = start.M33 + ((end.M33 - start.M33) * amount);
            result.M43 = start.M43 + ((end.M43 - start.M43) * amount);
            result.M14 = start.M14 + ((end.M14 - start.M14) * amount);
            result.M24 = start.M24 + ((end.M24 - start.M24) * amount);
            result.M34 = start.M34 + ((end.M34 - start.M34) * amount);
            result.M44 = start.M44 + ((end.M44 - start.M44) * amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two matrices.</returns>
        public static Matrix SmoothStep(Matrix start, Matrix end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out var result);
            return result;
        }

        
        /// <summary>
        /// Scales a matrix by the given value.
        /// </summary>
        /// <param name="left">The matrix to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled matrix.</param>
        public static void Multiply(ref readonly Matrix left, float right, out Matrix result)
        {
            result.M11 = left.M11 * right;
            result.M21 = left.M21 * right;
            result.M31 = left.M31 * right;
            result.M41 = left.M41 * right;
            result.M12 = left.M12 * right;
            result.M22 = left.M22 * right;
            result.M32 = left.M32 * right;
            result.M42 = left.M42 * right;
            result.M13 = left.M13 * right;
            result.M23 = left.M23 * right;
            result.M33 = left.M33 * right;
            result.M43 = left.M43 * right;
            result.M14 = left.M14 * right;
            result.M24 = left.M24 * right;
            result.M34 = left.M34 * right;
            result.M44 = left.M44 * right;
        }

        /// <summary>
        /// Scales a matrix by the given value.
        /// </summary>
        /// <param name="left">The matrix to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled matrix.</returns>
        public static Matrix Multiply(Matrix left, float right)
        {
            Multiply(ref left, right, out var result);
            return result;
        }

        /// <summary>
        /// Scales a matrix by a given value.
        /// </summary>
        /// <param name="left">The amount by which to scale.</param>
        /// <param name="right">The matrix to scale.</param>
        /// <returns>The scaled matrix.</returns>
        public static Matrix operator *(float left, in Matrix right)
        {
            Multiply(in right, left, out var result);
            return result;
        }

        /// <summary>
        /// Scales a matrix by a given value.
        /// </summary>
        /// <param name="left">The matrix to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled matrix.</returns>
        public static Matrix operator *(in Matrix left, float right)
        {
            Multiply(in left, right, out var result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that rotates around an arbitary axis.
        /// </summary>
        /// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="result">When the method completes, contains the created rotation matrix.</param>
        public static void CreateRotationAxis(ref readonly Vector3 axis, float angle, out Matrix result)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            result = Matrix.Identity;
            result.M11 = xx + (cos * (1.0f - xx));
            result.M12 = xy - (cos * xy) + (sin * z);
            result.M13 = xz - (cos * xz) - (sin * y);
            result.M21 = xy - (cos * xy) - (sin * z);
            result.M22 = yy + (cos * (1.0f - yy));
            result.M23 = yz - (cos * yz) + (sin * x);
            result.M31 = xz - (cos * xz) + (sin * y);
            result.M32 = yz - (cos * yz) - (sin * x);
            result.M33 = zz + (cos * (1.0f - zz));
        }

        /// <summary>
        /// Creates a matrix that rotates around an arbitary axis.
        /// </summary>
        /// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <returns>The created rotation matrix.</returns>
        public static Matrix CreateRotationAxis(Vector3 axis, float angle)
        {
            CreateRotationAxis(ref axis, angle, out var result);
            return result;
        }

        /// <summary>
        /// Performs the exponential operation on a matrix.
        /// </summary>
        /// <param name="value">The matrix to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the matrix to.</param>
        /// <param name="result">When the method completes, contains the exponential matrix.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static void Exponent(ref readonly Matrix value, int exponent, out Matrix result)
        {
            //Source: http://rosettacode.org
            //Refrence: http://rosettacode.org/wiki/Matrix-exponentiation_operator

            ArgumentOutOfRangeException.ThrowIfNegative(exponent);

            if (exponent == 0)
            {
                result = Matrix.Identity;
                return;
            }

            if (exponent == 1)
            {
                result = value;
                return;
            }

            Matrix identity = Matrix.Identity;
            Matrix temp = value;

            while (true)
            {
                if ((exponent & 1) != 0)
                    identity *= temp;

                exponent /= 2;

                if (exponent > 0)
                    temp *= temp;
                else
                    break;
            }

            result = identity;
        }

        /// <summary>
        /// Performs the exponential operation on a matrix.
        /// </summary>
        /// <param name="value">The matrix to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the matrix to.</param>
        /// <returns>The exponential matrix.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static Matrix Exponent(Matrix value, int exponent)
        {
            Exponent(ref value, exponent, out var result);
            return result;
        }

        /// <summary>
        /// Creates an array containing the elements of the matrix.
        /// </summary>
        /// <returns>A sixteen-element array containing the components of the matrix.</returns>
        public float[] ToArray()
        {
            return [matrix.M11, matrix.M12, matrix.M13, matrix.M14, matrix.M21, matrix.M22, matrix.M23, matrix.M24, matrix.M31, matrix.M32, matrix.M33, matrix.M34, matrix.M41, matrix.M42, matrix.M43, matrix.M44];
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="row">The row of the matrix to access.</param>
        /// <param name="column">The column of the matrix to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Component(int row, int column)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(row, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(row, 3);
            ArgumentOutOfRangeException.ThrowIfLessThan(column, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(column, 3);

            return matrix.Component((row * 4) + column);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the components of the matrix. This must be an array with sixteen elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than sixteen elements.</exception>
        public static Matrix CreateFromValues(float[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            ArgumentOutOfRangeException.ThrowIfNotEqual(values.Length, 16);

            var m = new Matrix
            {
                M11 = values[0],
                M12 = values[1],
                M13 = values[2],
                M14 = values[3],
                M21 = values[4],
                M22 = values[5],
                M23 = values[6],
                M24 = values[7],
                M31 = values[8],
                M32 = values[9],
                M33 = values[10],
                M34 = values[11],
                M41 = values[12],
                M42 = values[13],
                M43 = values[14],
                M44 = values[15]
            };

            return m;
        }
        
        
        /// <summary>
        /// Inverts the matrix.
        /// If the matrix cannot be inverted (eg. Determinant was zero), then the matrix will be set equivalent to <see cref="Zero"/>.
        /// </summary>
        /// <remarks>
        /// This method delegates to <see cref="Matrix.Invert(Matrix4x4, out Matrix4x4)"/> from System.Numerics.
        /// Named InvertSelf to avoid ambiguity with FNA's Invert(Matrix) extension which returns a new matrix.
        /// </remarks>
        public void InvertSelf()
        {
            if (!Matrix.Invert(matrix, out matrix))
            {
                matrix = default;
            }
        }
        
        /// <summary>
        /// Calculates the inverse of the specified matrix.
        /// If the matrix cannot be inverted (eg. Determinant was zero), then <paramref name="result"/> will be <see cref="Zero"/>.
        /// </summary>
        /// <param name="value">The matrix whose inverse is to be calculated.</param>
        /// <param name="result">When the method completes, contains the inverse of the specified matrix.</param>
        public static void InvertSelf(ref readonly Matrix value, out Matrix result)
        {
            // Invert works the same in row and column major, no need to transpose
            Unsafe.SkipInit(out result);
            if (!Matrix.Invert(value, out result))
            {
                result = default;
            }
        }
    }
}