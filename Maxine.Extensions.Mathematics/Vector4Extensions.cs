using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Mathematics;

public static class Vector4Extensions
{
    extension(ref Vector4 vec4)
    {
        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float length = vec4.Length();
            if (length > MathUtil.ZeroTolerance)
            {
                float inverse = 1.0f / length;
                vec4.X *= inverse;
                vec4.Y *= inverse;
                vec4.Z *= inverse;
                vec4.W *= inverse;
            }
        }
    }
    
    extension(Vector4 vec4)
    {
        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator /(Vector4 value, float scale)
        {
            return new Vector4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
        }

        /// <summary>
        /// Divides a numerator by a vector.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator /(float numerator, Vector4 value)
        {
            return new Vector4(numerator / value.X, numerator / value.Y, numerator / value.Z, numerator / value.W);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(Vector4 value, float scale)
        {
            return new Vector4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <param name="value">The vector to scale.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(float scale, Vector4 value)
        {
            return new Vector4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <param name="result">When the method completes, contains the modulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Modulate(ref readonly Vector4 left, ref readonly Vector4 right, out Vector4 result)
        {
            result = new Vector4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <returns>The modulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Modulate(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <param name="result">When the method completes, contains the demodulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Demodulate(ref readonly Vector4 left, ref readonly Vector4 right, out Vector4 result)
        {
            result = new Vector4(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <returns>The demodulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Demodulate(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
        }

        /// <summary>
        /// Converts to a <see cref="Vector2"/>, dropping Z and W.
        /// </summary>
        /// <remarks>
        /// C# does not allow conversion operators to be declared in an extension block for a type
        /// this project does not own (System.Numerics.Vector2/3/4), so this is a named method
        /// rather than an <c>explicit operator</c> as in Stride's original API.
        /// </remarks>
        public Vector2 ToVector2() => new(vec4.X, vec4.Y);

        /// <summary>
        /// Converts to a <see cref="Vector3"/>, dropping W.
        /// </summary>
        public Vector3 ToVector3() => new(vec4.X, vec4.Y, vec4.Z);

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        public static void Add(ref readonly Vector4 left, ref readonly Vector4 right, out Vector4 result)
        {
            result = Vector4.Add(left, right);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        public static void Subtract(ref readonly Vector4 left, ref readonly Vector4 right, out Vector4 result)
        {
            result = Vector4.Subtract(left, right);
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        public static void Negate(ref readonly Vector4 value, out Vector4 result)
        {
            result = Vector4.Negate(value);
        }

        /// <summary>
        /// Returns a <see cref="Vector4"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 4D triangle.
        /// </summary>
        public static void Barycentric(ref readonly Vector4 value1, ref readonly Vector4 value2, ref readonly Vector4 value3, float amount1, float amount2, out Vector4 result)
        {
            result = new Vector4(
                (value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
                (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)),
                (value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)),
                (value1.W + (amount1 * (value2.W - value1.W))) + (amount2 * (value3.W - value1.W)));
        }

        /// <summary>
        /// Returns a <see cref="Vector4"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 4D triangle.
        /// </summary>
        public static Vector4 Barycentric(Vector4 value1, Vector4 value2, Vector4 value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out var result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        public static void SmoothStep(ref readonly Vector4 start, ref readonly Vector4 end, float amount, out Vector4 result)
        {
            amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
            amount = (amount * amount) * (3.0f - (2.0f * amount));

            result.X = start.X + ((end.X - start.X) * amount);
            result.Y = start.Y + ((end.Y - start.Y) * amount);
            result.Z = start.Z + ((end.Z - start.Z) * amount);
            result.W = start.W + ((end.W - start.W) * amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        public static Vector4 SmoothStep(Vector4 start, Vector4 end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        public static void Hermite(ref readonly Vector4 value1, ref readonly Vector4 tangent1, ref readonly Vector4 value2, ref readonly Vector4 tangent2, float amount, out Vector4 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = (cubed - (2.0f * squared)) + amount;
            float part4 = cubed - squared;

            result.X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4);
            result.Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4);
            result.Z = (((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4);
            result.W = (((value1.W * part1) + (value2.W * part2)) + (tangent1.W * part3)) + (tangent2.W * part4);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
        {
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        public static void CatmullRom(ref readonly Vector4 value1, ref readonly Vector4 value2, ref readonly Vector4 value3, ref readonly Vector4 value4, float amount, out Vector4 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;

            result.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) +
                (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) +
                ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));

            result.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) +
                (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) +
                ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));

            result.Z = 0.5f * ((((2.0f * value2.Z) + ((-value1.Z + value3.Z) * amount)) +
                (((((2.0f * value1.Z) - (5.0f * value2.Z)) + (4.0f * value3.Z)) - value4.Z) * squared)) +
                ((((-value1.Z + (3.0f * value2.Z)) - (3.0f * value3.Z)) + value4.Z) * cubed));

            result.W = 0.5f * ((((2.0f * value2.W) + ((-value1.W + value3.W) * amount)) +
                (((((2.0f * value1.W) - (5.0f * value2.W)) + (4.0f * value3.W)) - value4.W) * squared)) +
                ((((-value1.W + (3.0f * value2.W)) - (3.0f * value3.W)) + value4.W) * cubed));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        public static Vector4 CatmullRom(Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4, float amount)
        {
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out var result);
            return result;
        }

        /// <summary>
        /// Moves the first vector4 to the second one in a straight line.
        /// </summary>
        /// <param name="from">The first point.</param>
        /// <param name="to">The second point.</param>
        /// <param name="maxTravelDistance">The rate at which the first point is going to move towards the second point.</param>
        public static Vector4 MoveTo(in Vector4 from, in Vector4 to, float maxTravelDistance)
        {
            Vector4 distance = Vector4.Subtract(to, from);

            float length = distance.Length();

            if (maxTravelDistance >= length || length == 0)
            {
                return to;
            }
            else
            {
                var v = 1f / length * maxTravelDistance;
                return new Vector4(from.X + (distance.X * v), from.Y + (distance.Y * v), from.Z + (distance.Z * v), from.W + (distance.W * v));
            }
        }

        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref readonly Vector4 vector, ref readonly Quaternion rotation, out Vector4 result)
        {
            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;

            result = new Vector4(
                (vector.X * (1.0f - yy - zz)) + (vector.Y * (xy - wz)) + (vector.Z * (xz + wy)),
                (vector.X * (xy + wz)) + (vector.Y * (1.0f - xx - zz)) + (vector.Z * (yz - wx)),
                (vector.X * (xz - wy)) + (vector.Y * (yz + wx)) + (vector.Z * (1.0f - xx - yy)),
                vector.W);
        }

        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(Vector4 vector, Quaternion rotation)
        {
            Transform(ref vector, ref rotation, out var result);
            return result;
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector4[] source, ref readonly Quaternion rotation, Vector4[] destination)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;

            float num1 = 1.0f - yy - zz;
            float num2 = xy - wz;
            float num3 = xz + wy;
            float num4 = xy + wz;
            float num5 = 1.0f - xx - zz;
            float num6 = yz - wx;
            float num7 = xz - wy;
            float num8 = yz + wx;
            float num9 = 1.0f - xx - yy;

            for (int i = 0; i < source.Length; ++i)
            {
                destination[i] = new Vector4(
                    (source[i].X * num1) + (source[i].Y * num2) + (source[i].Z * num3),
                    (source[i].X * num4) + (source[i].Y * num5) + (source[i].Z * num6),
                    (source[i].X * num7) + (source[i].Y * num8) + (source[i].Z * num9),
                    source[i].W);
            }
        }

        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref readonly Vector4 vector, ref readonly Matrix transform, out Vector4 result)
        {
            result = new Vector4(
                (vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31) + (vector.W * transform.M41),
                (vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32) + (vector.W * transform.M42),
                (vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33) + (vector.W * transform.M43),
                (vector.X * transform.M14) + (vector.Y * transform.M24) + (vector.Z * transform.M34) + (vector.W * transform.M44));
        }

        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(Vector4 vector, Matrix transform)
        {
            Transform(ref vector, ref transform, out var result);
            return result;
        }

        /// <summary>
        /// Transforms an array of 4D vectors by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector4[] source, ref readonly Matrix transform, Vector4[] destination)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Transform(ref source[i], in transform, out destination[i]);
            }
        }
        
        /// <summary>
        /// Deconstructs the vector's components into named variables.
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        /// <param name="z">The Z component</param>
        /// <param name="w">The W component</param>
        public void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = vec4.X;
            y = vec4.Y;
            z = vec4.Z;
            w = vec4.W;
        }
    }
}