namespace Maxine.Extensions.Mathematics;

using Microsoft.Xna.Framework;

public static class QuaternionExtensions
{
    extension(ref Quaternion quat)
    {
        /// <summary>
        /// A <see cref="Quaternion"/> with all of its components set to one.
        /// </summary>
        public static Quaternion One => new(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        public void Normalize()
        {
            float length = quat.Length();
            if (length > MathUtil.ZeroTolerance)
            {
                float inverse = 1.0f / length;
                quat.X *= inverse;
                quat.Y *= inverse;
                quat.Z *= inverse;
                quat.W *= inverse;
            }
        }
        
        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public void Invert()
        {
            float lengthSq = quat.LengthSquared();
            if (lengthSq > MathUtil.ZeroTolerance)
            {
                lengthSq = 1.0f / lengthSq;

                quat.X = -quat.X * lengthSq;
                quat.Y = -quat.Y * lengthSq;
                quat.Z = -quat.Z * lengthSq;
                quat.W = quat.W * lengthSq;
            }
        }
    }
    
    extension(Quaternion quat)
    {
        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref readonly Matrix matrix, out Quaternion result)
        {
            float sqrt;
            float half;
            float scale = matrix.M11 + matrix.M22 + matrix.M33;

            if (scale > 0.0f)
            {
                sqrt = MathF.Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion RotationMatrix(Matrix matrix)
        {
            RotationMatrix(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion that rotates around the x-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationX(float angle, out Quaternion result)
        {
            float halfAngle = angle * 0.5f;
            result = new Quaternion(MathF.Sin(halfAngle), 0.0f, 0.0f, MathF.Cos(halfAngle));
        }

        /// <summary>
        /// Creates a quaternion that rotates around the x-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <returns>The created rotation quaternion.</returns>
        public static Quaternion RotationX(float angle)
        {
            RotationX(angle, out var result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion that rotates around the y-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationY(float angle, out Quaternion result)
        {
            float halfAngle = angle * 0.5f;
            result = new Quaternion(0.0f, MathF.Sin(halfAngle), 0.0f, MathF.Cos(halfAngle));
        }

        /// <summary>
        /// Creates a quaternion that rotates around the y-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <returns>The created rotation quaternion.</returns>
        public static Quaternion RotationY(float angle)
        {
            RotationY(angle, out var result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion that rotates around the z-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationZ(float angle, out Quaternion result)
        {
            float halfAngle = angle * 0.5f;
            result = new Quaternion(0.0f, 0.0f, MathF.Sin(halfAngle), MathF.Cos(halfAngle));
        }

        /// <summary>
        /// Creates a quaternion that rotates around the z-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <returns>The created rotation quaternion.</returns>
        public static Quaternion RotationZ(float angle)
        {
            RotationZ(angle, out var result);
            return result;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the components.</param>
        public static Quaternion CreateFromVector4(Vector4 value)
        {
            var q = new Quaternion
            {
                X = value.X,
                Y = value.Y,
                Z = value.Z,
                W = value.W
            };
            return q;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public static Quaternion CreateFromValue(float value)
        {
            var q = new Quaternion
            {
                X = value,
                Y = value,
                Z = value,
                W = value
            };
            return q;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static Quaternion operator *(float scale, in Quaternion value)
        {
            Quaternion.Multiply(in value, scale, out var result);
            return result;
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Thrid source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
        public static void Squad(ref readonly Quaternion value1, ref readonly Quaternion value2, ref readonly Quaternion value3, ref readonly Quaternion value4, float amount, out Quaternion result)
        {
            Quaternion.Slerp(in value1, in value4, amount, out var start);
            Quaternion.Slerp(in value2, in value3, amount, out var end);
            Quaternion.Slerp(in start, in end, 2.0f * amount * (1.0f - amount), out result);
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Thrid source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <returns>The spherical quadrangle interpolation of the quaternions.</returns>
        public static Quaternion Squad(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4, float amount)
        {
            Squad(ref value1, ref value2, ref value3, ref value4, amount, out var result);
            return result;
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
        public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
        {
            Quaternion q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
            Quaternion q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
            Quaternion q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
            Quaternion q1 = value2;

            Exponential(ref q1, out var q1Exp);
            Exponential(ref q2, out var q2Exp);

            Quaternion[] results = new Quaternion[3];
            results[0] = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
            results[1] = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
            results[2] = q2;

            return results;
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
        public static void Exponential(ref readonly Quaternion value, out Quaternion result)
        {
            float angle = MathF.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            float sin = MathF.Sin(angle);

            if (MathF.Abs(sin) >= MathUtil.ZeroTolerance)
            {
                float coeff = sin / angle;
                result.X = coeff * value.X;
                result.Y = coeff * value.Y;
                result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = MathF.Cos(angle);
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <returns>The exponentiated quaternion.</returns>
        public static Quaternion Exponential(Quaternion value)
        {
            Exponential(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
        public static void Logarithm(ref readonly Quaternion value, out Quaternion result)
        {
            if (MathF.Abs(value.W) < 1.0f)
            {
                float angle = MathF.Acos(value.W);
                float sin = MathF.Sin(angle);

                if (MathF.Abs(sin) >= MathUtil.ZeroTolerance)
                {
                    float coeff = angle / sin;
                    result.X = value.X * coeff;
                    result.Y = value.Y * coeff;
                    result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }

            result.W = 0.0f;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <returns>The natural logarithm of the quaternion.</returns>
        public static Quaternion Logarithm(Quaternion value)
        {
            Logarithm(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains a new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref readonly Quaternion value1, ref readonly Quaternion value2, ref readonly Quaternion value3, float amount1, float amount2, out Quaternion result)
        {
            Quaternion.Slerp(in value1, in value2, amount1 + amount2, out var start);
            Quaternion.Slerp(in value1, in value3, amount1 + amount2, out var end);
            Quaternion.Slerp(in start, in end, amount2 / (amount1 + amount2), out result);
        }

        /// <summary>
        /// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</returns>
        public static Quaternion Barycentric(Quaternion value1, Quaternion value2, Quaternion value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out var result);
            return result;
        }

        /// <summary>
        /// Computes a quaternion corresponding to the rotation transforming the vector <paramref name="source"/> to the vector <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source vector of the transformation.</param>
        /// <param name="target">The target vector of the transformation.</param>
        /// <returns>The resulting quaternion corresponding to the transformation of the source vector to the target vector.</returns>
        public static Quaternion BetweenDirections(Vector3 source, Vector3 target)
        {
            BetweenDirections(ref source, ref target, out var result);
            return result;
        }

        /// <summary>
        /// Computes a quaternion corresponding to the rotation transforming the vector <paramref name="source"/> to the vector <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source vector of the transformation.</param>
        /// <param name="target">The target vector of the transformation.</param>
        /// <param name="result">The resulting quaternion corresponding to the transformation of the source vector to the target vector.</param>
        public static void BetweenDirections(ref readonly Vector3 source, ref readonly Vector3 target, out Quaternion result)
        {
            var norms = MathF.Sqrt(source.LengthSquared() * target.LengthSquared());
            var real = norms + Vector3.Dot(source, target);
            if (real < MathUtil.ZeroTolerance * norms)
            {
                // If source and target are exactly opposite, rotate 180 degrees around an arbitrary orthogonal axis.
                // Axis normalisation can happen later, when we normalise the quaternion.
                result = MathF.Abs(source.X) > MathF.Abs(source.Z)
                    ? new Quaternion(-source.Y, source.X, 0.0f, 0.0f)
                    : new Quaternion(0.0f, -source.Z, source.Y, 0.0f);
            }
            else
            {
                // Otherwise, build quaternion the standard way.
                var axis = Vector3.Cross(source, target);
                result = new Quaternion(axis, real);
            }
            result.Normalize();
        }

        /// <summary>
        /// Returns the absolute angle in radians between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        public static float AngleBetween(in Quaternion a, in Quaternion b)
        {
            return MathF.Acos(MathF.Min(MathF.Abs(Quaternion.Dot(a, b)), 1f)) * 2f;
        }
        
        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        /// <value>The quaternion's angle.</value>
        public float Angle
        {
            get
            {
                float length = (quat.X * quat.X) + (quat.Y * quat.Y) + (quat.Z * quat.Z);
                if (length < MathUtil.ZeroTolerance)
                    return 0.0f;

                return 2.0f * MathF.Acos(quat.W);
            }
        }

        /// <summary>
        /// Gets the axis components of the quaternion.
        /// </summary>
        /// <value>The axis components of the quaternion.</value>
        public Vector3 Axis
        {
            get
            {
                float length = (quat.X * quat.X) + (quat.Y * quat.Y) + (quat.Z * quat.Z);
                if (length < MathUtil.ZeroTolerance)
                    return Vector3.UnitX;

                float inv = 1.0f / length;
                return new Vector3(quat.X * inv, quat.Y * inv, quat.Z * inv);
            }
        }

        // /// <summary>
        // /// Calculate the yaw/pitch/roll rotation equivalent to the provided quaternion.
        // /// </summary>
        // /// <param name="rotation">The input quaternion</param>
        // /// <param name="yaw">The yaw component in radians.</param>
        // /// <param name="pitch">The pitch component in radians.</param>
        // /// <param name="roll">The roll component in radians.</param>
        // public static void StrideYawPitchRoll(ref readonly Quaternion rotation, out float yaw, out float pitch, out float roll)
        // {
        //     // Equivalent to:
        //     //  Matrix rotationMatrix;
        //     //  Matrix.Rotation(ref cachedRotation, out rotationMatrix);
        //     //  rotationMatrix.Decompose(out float yaw, out float pitch, out float roll);
        //
        //     var xx = rotation.X * rotation.X;
        //     var yy = rotation.Y * rotation.Y;
        //     var zz = rotation.Z * rotation.Z;
        //     var xy = rotation.X * rotation.Y;
        //     var zw = rotation.Z * rotation.W;
        //     var zx = rotation.Z * rotation.X;
        //     var yw = rotation.Y * rotation.W;
        //     var yz = rotation.Y * rotation.Z;
        //     var xw = rotation.X * rotation.W;
        //
        //     var M11 = 1.0f - (2.0f * (yy + zz));
        //     var M12 = 2.0f * (xy + zw);
        //     //var M13 = 2.0f * (zx - yw);
        //     var M21 = 2.0f * (xy - zw);
        //     var M22 = 1.0f - (2.0f * (zz + xx));
        //     //var M23 = 2.0f * (yz + xw);
        //     var M31 = 2.0f * (zx + yw);
        //     var M32 = 2.0f * (yz - xw);
        //     var M33 = 1.0f - (2.0f * (yy + xx));
        //
        //     /*** Refer to Matrix.Decompose(out float yaw, out float pitch, out float roll) for code and license ***/
        //     if (MathUtil.IsOne(Math.Abs(M32)))
        //     {
        //         if (M32 >= 0)
        //         {
        //             // Edge case where M32 == +1
        //             pitch = -MathUtil.PiOverTwo;
        //             yaw = MathF.Atan2(-M21, M11);
        //             roll = 0;
        //         }
        //         else
        //         {
        //             // Edge case where M32 == -1
        //             pitch = MathUtil.PiOverTwo;
        //             yaw = -MathF.Atan2(-M21, M11);
        //             roll = 0;
        //         }
        //     }
        //     else
        //     {
        //         // Common case
        //         pitch = MathF.Asin(-M32);
        //         yaw = MathF.Atan2(M31, M33);
        //         roll = MathF.Atan2(M12, M22);
        //     }
        // }
        //
        // /// <summary>
        // /// Creates a quaternion given a yaw, pitch, and roll value (angles in radians).
        // /// </summary>
        // /// <param name="yaw">The yaw of rotation in radians.</param>
        // /// <param name="pitch">The pitch of rotation in radians.</param>
        // /// <param name="roll">The roll of rotation in radians.</param>
        // /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        // public static void CreateStrideFromYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
        // {
        //     var halfRoll = roll * 0.5f;
        //     var halfPitch = pitch * 0.5f;
        //     var halfYaw = yaw * 0.5f;
        //
        //     var sinRoll = MathF.Sin(halfRoll);
        //     var cosRoll = MathF.Cos(halfRoll);
        //     var sinPitch = MathF.Sin(halfPitch);
        //     var cosPitch = MathF.Cos(halfPitch);
        //     var sinYaw = MathF.Sin(halfYaw);
        //     var cosYaw = MathF.Cos(halfYaw);
        //
        //     var cosYawPitch = cosYaw * cosPitch;
        //     var sinYawPitch = sinYaw * sinPitch;
        //
        //     result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
        //     result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
        //     result.Z = (cosYawPitch * sinRoll) - (sinYawPitch * cosRoll);
        //     result.W = (cosYawPitch * cosRoll) + (sinYawPitch * sinRoll);
        // }
        //
        // public static Quaternion CreateStrideFromYawPitchRoll(float yaw, float pitch, float roll)
        // {
        //     CreateStrideFromYawPitchRoll(yaw, pitch, roll, out var result);
        //     return result;
        // }

        /// <summary>
        /// Rotate <paramref name="current"/> towards <paramref name="target"/> by <paramref name="angle"/>.
        /// </summary>
        /// <remarks>
        /// When the angle difference between <paramref name="current"/> and <paramref name="target"/> is less than
        /// the given <paramref name="angle"/>, returns <paramref name="target"/> instead of overshooting past it.
        /// </remarks>
        public static Quaternion RotateTowards(in Quaternion current, in Quaternion target, float angle)
        {
            var maxAngle = AngleBetween(current, target);
            return maxAngle == 0f ? target : Quaternion.Slerp(current, target, MathF.Min(1f, angle / maxAngle));
        }

        /// <summary>
        /// Rotates a Vector3 by the specified quaternion rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        public void Rotate(ref Vector3 vector)
        {
            var pureQuaternion = new Quaternion(vector, 0);
            pureQuaternion = quat * pureQuaternion * Quaternion.Conjugate(quat);

            vector.X = pureQuaternion.X;
            vector.Y = pureQuaternion.Y;
            vector.Z = pureQuaternion.Z;
        }

    }
}

public static class QuaternionExtensions2
{
    extension(Quaternion quat)
    {
        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate and renormalize.</param>
        /// <param name="result">When the method completes, contains the conjugated and renormalized quaternion.</param>
        public static void Invert(ref readonly Quaternion value, out Quaternion result)
        {
            result = value;
            result.Invert();
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate and renormalize.</param>
        /// <returns>The conjugated and renormalized quaternion.</returns>
        public static Quaternion Invert(Quaternion value)
        {
            Invert(ref value, out var result);
            return result;
        }
    }
}