using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Mathematics;

public static class Vector2Extensions
{
    extension(ref Vector2 vec)
    {
        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float length = vec.Length();
            if (length > MathUtil.ZeroTolerance)
            {
                float inverse = 1.0f / length;
                vec.X *= inverse;
                vec.Y *= inverse;
            }
        }
    }

    extension(Vector2 vec)
    {

        /// <summary>
        /// Moves the first vector2 to the second one in a straight line.
        /// </summary>
        /// <param name="from">The first point.</param>
        /// <param name="to">The second point.</param>
        /// <param name="maxTravelDistance">The rate at which the first point is going to move towards the second point.</param>
        public static Vector2 MoveTo(in Vector2 from, in Vector2 to, float maxTravelDistance)
        {
            Vector2 distance = Vector2.Subtract(to, from);

            float length = distance.Length();

            if (maxTravelDistance >= length || length == 0)
                return to;
            else
                return new Vector2(from.X + distance.X / length * maxTravelDistance, from.Y + distance.Y / length * maxTravelDistance);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(float scale, Vector2 value)
        {
            return new Vector2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 value, float scale)
        {
            return new Vector2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value, float scale)
        {
            return new Vector2(value.X / scale, value.Y / scale);
        }

        /// <summary>
        /// Divides a numerator by a vector.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(float numerator, Vector2 value)
        {
            return new Vector2(numerator / value.X, numerator / value.Y);
        }

        /// <summary>
        /// Divides a vector by the given vector, component-wise.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="by">The by.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value, Vector2 by)
        {
            return new Vector2(value.X / by.X, value.Y / by.Y);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <param name="result">When the method completes, contains the modulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Modulate(ref readonly Vector2 left, ref readonly Vector2 right, out Vector2 result)
        {
            result = new Vector2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <returns>The modulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Modulate(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <param name="result">When the method completes, contains the demodulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Demodulate(ref readonly Vector2 left, ref readonly Vector2 right, out Vector2 result)
        {
            result = new Vector2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <returns>The demodulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Demodulate(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Transforms a 2D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref readonly Vector2 vector, ref readonly Quaternion rotation, out Vector2 result)
        {
            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float yy = rotation.Y * y;
            float zz = rotation.Z * z;

            result = new Vector2((vector.X * (1.0f - yy - zz)) + (vector.Y * (xy - wz)), (vector.X * (xy + wz)) + (vector.Y * (1.0f - xx - zz)));
        }

        /// <summary>
        /// Transforms a 2D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector2 Transform(Vector2 vector, Quaternion rotation)
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
        public static void Transform(Vector2[] source, ref readonly Quaternion rotation, Vector2[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float yy = rotation.Y * y;
            float zz = rotation.Z * z;

            float num1 = (1.0f - yy - zz);
            float num2 = (xy - wz);
            float num3 = (xy + wz);
            float num4 = (1.0f - xx - zz);

            for (int i = 0; i < source.Length; ++i)
            {
                destination[i] = new Vector2(
                    (source[i].X * num1) + (source[i].Y * num2),
                    (source[i].X * num3) + (source[i].Y * num4));
            }
        }

        /// <summary>
        /// Transforms a 2D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref readonly Vector2 vector, ref readonly Matrix transform, out Vector4 result)
        {
            result = new Vector4(
                (vector.X * transform.M11) + (vector.Y * transform.M21) + transform.M41,
                (vector.X * transform.M12) + (vector.Y * transform.M22) + transform.M42,
                (vector.X * transform.M13) + (vector.Y * transform.M23) + transform.M43,
                (vector.X * transform.M14) + (vector.Y * transform.M24) + transform.M44);
        }

        /// <summary>
        /// Transforms a 2D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(Vector2 vector, Matrix transform)
        {
            Transform(ref vector, ref transform, out var result);
            return result;
        }

        /// <summary>
        /// Transforms an array of 2D vectors by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector2[] source, ref readonly Matrix transform, Vector4[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Transform(ref source[i], in transform, out destination[i]);
            }
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        public static void Add(ref readonly Vector2 left, ref readonly Vector2 right, out Vector2 result)
        {
            result = Vector2.Add(left, right);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        public static void Subtract(ref readonly Vector2 left, ref readonly Vector2 right, out Vector2 result)
        {
            result = Vector2.Subtract(left, right);
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        public static void Negate(ref readonly Vector2 value, out Vector2 result)
        {
            result = Vector2.Negate(value);
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        public static void Barycentric(ref readonly Vector2 value1, ref readonly Vector2 value2, ref readonly Vector2 value3, float amount1, float amount2, out Vector2 result)
        {
            result = new Vector2(
                (value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
                (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)));
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out var result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        public static void SmoothStep(ref readonly Vector2 start, ref readonly Vector2 end, float amount, out Vector2 result)
        {
            amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
            amount = (amount * amount) * (3.0f - (2.0f * amount));

            result.X = start.X + ((end.X - start.X) * amount);
            result.Y = start.Y + ((end.Y - start.Y) * amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        public static Vector2 SmoothStep(Vector2 start, Vector2 end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        public static void Hermite(ref readonly Vector2 value1, ref readonly Vector2 tangent1, ref readonly Vector2 value2, ref readonly Vector2 tangent2, float amount, out Vector2 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = (cubed - (2.0f * squared)) + amount;
            float part4 = cubed - squared;

            result.X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4);
            result.Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
        {
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        public static void CatmullRom(ref readonly Vector2 value1, ref readonly Vector2 value2, ref readonly Vector2 value3, ref readonly Vector2 value4, float amount, out Vector2 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;

            result.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) +
            (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) +
            ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));

            result.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) +
                (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) +
                ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        public static void TransformNormal(ref readonly Vector2 normal, ref readonly Matrix transform, out Vector2 result)
        {
            result = new Vector2(
                (normal.X * transform.M11) + (normal.Y * transform.M21),
                (normal.X * transform.M12) + (normal.Y * transform.M22));
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        public static Vector2 TransformNormal(Vector2 normal, Matrix transform)
        {
            TransformNormal(ref normal, ref transform, out var result);
            return result;
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        public static void TransformCoordinate(ref readonly Vector2 coordinate, ref readonly Matrix transform, out Vector2 result)
        {
            Transform(in coordinate, in transform, out Vector4 v4);
            float invW = 1.0f / v4.W;
            result = new Vector2(v4.X * invW, v4.Y * invW);
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        public static Vector2 TransformCoordinate(Vector2 coordinate, Matrix transform)
        {
            TransformCoordinate(ref coordinate, ref transform, out var result);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="Vector3"/> with Z = 0.
        /// </summary>
        /// <remarks>
        /// C# does not allow conversion operators to be declared in an extension block for a type
        /// this project does not own (System.Numerics.Vector2/3/4), so this is a named method
        /// rather than an <c>explicit operator</c> as in Stride's original API.
        /// </remarks>
        public Vector3 ToVector3() => new(vec, 0.0f);

        /// <summary>
        /// Converts to a <see cref="Vector4"/> with Z = W = 0.
        /// </summary>
        public Vector4 ToVector4() => new(vec, 0.0f, 0.0f);

        /// <summary>
        /// Deconstructs the vector's components into named variables.
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        public void Deconstruct(out float x, out float y)
        {
            x = vec.X;
            y = vec.Y;
        }

    }
}