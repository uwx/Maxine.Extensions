using Microsoft.Xna.Framework;

namespace Maxine.Extensions.Mathematics;

public static class PlaneExtensions
{
    extension(ref Plane self)
    {
        /// <summary>
        /// Negates a plane by negating all its coefficients, which result in a plane in opposite direction.
        /// </summary>
        public void Negate()
        {
            self.Normal.X = -self.Normal.X;
            self.Normal.Y = -self.Normal.Y;
            self.Normal.Z = -self.Normal.Z;
            self.D = -self.D;
        }

        /// <summary>
        /// Changes the coefficients of the normal vector of the plane to make it of unit length.
        /// </summary>
        public void Normalize()
        {
            float magnitude = 1.0f / MathF.Sqrt((self.Normal.X * self.Normal.X) + (self.Normal.Y * self.Normal.Y) + (self.Normal.Z * self.Normal.Z));

            self.Normal.X *= magnitude;
            self.Normal.Y *= magnitude;
            self.Normal.Z *= magnitude;
            self.D *= magnitude;
        }
    }
    
    extension(Plane self)
    {
        /// <summary>
        /// Creates a plane of unit length.
        /// </summary>
        /// <param name="normalX">The X component of the normal.</param>
        /// <param name="normalY">The Y component of the normal.</param>
        /// <param name="normalZ">The Z component of the normal.</param>
        /// <param name="planeD">The distance of the plane along its normal from the origin.</param>
        /// <param name="result">When the method completes, contains the normalized plane.</param>
        public static void Normalize(float normalX, float normalY, float normalZ, float planeD, out Plane result)
        {
            float magnitude = 1.0f / MathF.Sqrt((normalX * normalX) + (normalY * normalY) + (normalZ * normalZ));

            result.Normal.X = normalX * magnitude;
            result.Normal.Y = normalY * magnitude;
            result.Normal.Z = normalZ * magnitude;
            result.D = planeD * magnitude;
        }

        /// <summary>
        /// Changes the coefficients of the normal vector of the plane to make it of unit length.
        /// </summary>
        /// <param name="plane">The source plane.</param>
        /// <param name="result">When the method completes, contains the normalized plane.</param>
        public static void Normalize(ref readonly Plane plane, out Plane result)
        {
            float magnitude = 1.0f / MathF.Sqrt((plane.Normal.X * plane.Normal.X) + (plane.Normal.Y * plane.Normal.Y) + (plane.Normal.Z * plane.Normal.Z));

            result.Normal.X = plane.Normal.X * magnitude;
            result.Normal.Y = plane.Normal.Y * magnitude;
            result.Normal.Z = plane.Normal.Z * magnitude;
            result.D = plane.D * magnitude;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="point">Any point that lies along the plane.</param>
        /// <param name="normal">The normal vector to the plane.</param>
        public static Plane FromPointAndNormal(Vector3 point, Vector3 normal)
        {
            var plane = new Plane
            {
                Normal = normal
            };
            Vector3.Dot(in normal, in point, out plane.D);
            return plane;
        }
        
        /// <summary>
        /// Determines if there is an intersection between the current object and a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref readonly Vector3 point)
        {
            return CollisionHelper.PlaneIntersectsPoint(ref self, in point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref readonly Ray ray)
        {
            float distance;
            return CollisionHelper.RayIntersectsPlane(in ray, ref self, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref readonly Ray ray, out float distance)
        {
            return CollisionHelper.RayIntersectsPlane(in ray, ref self, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref readonly Ray ray, out Vector3 point)
        {
            return CollisionHelper.RayIntersectsPlane(in ray, ref self, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref readonly Plane plane)
        {
            return CollisionHelper.PlaneIntersectsPlane(ref self, in plane);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="line">When the method completes, contains the line of intersection
        /// as a <see cref="Ray"/>, or a zero ray if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref readonly Plane plane, out Ray line)
        {
            return CollisionHelper.PlaneIntersectsPlane(ref self, in plane, out line);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triagnle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref readonly Vector3 vertex1, ref readonly Vector3 vertex2, ref readonly Vector3 vertex3)
        {
            return CollisionHelper.PlaneIntersectsTriangle(ref self, in vertex1, in vertex2, in vertex3);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref readonly BoundingBox box)
        {
            return CollisionHelper.PlaneIntersectsBox(ref self, in box);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref readonly BoundingSphere sphere)
        {
            return CollisionHelper.PlaneIntersectsSphere(ref self, in sphere);
        }


        /// <summary>
        /// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
        /// </summary>
        /// <param name="left">The source plane.</param>
        /// <param name="right">The source vector.</param>
        /// <param name="result">When the method completes, contains the dot product of a specified vector and the normal of the Plane plus the distance value of the plane.</param>
        public static void DotCoordinate(ref readonly Plane left, ref readonly Vector3 right, out float result)
        {
            result = (left.Normal.X * right.X) + (left.Normal.Y * right.Y) + (left.Normal.Z * right.Z) + left.D;
        }

        /// <summary>
        /// Projects a point onto a plane.
        /// </summary>
        /// <param name="plane">The plane to project the point to.</param>
        /// <param name="point">The point to project.</param>
        /// <param name="result">The projected point.</param>
        public static void Project(ref readonly Plane plane, ref readonly Vector3 point, out Vector3 result)
        {
            Plane.DotCoordinate(in plane, in point, out var distance);

            // compute: point - distance * plane.Normal
            Vector3.Multiply(in plane.Normal, distance, out result);
            Vector3.Subtract(in point, in result, out result);
        }

        /// <summary>
        /// Projects a point onto a plane.
        /// </summary>
        /// <param name="plane">The plane to project the point to.</param>
        /// <param name="point">The point to project.</param>
        /// <returns>The projected point.</returns>
        public static Vector3 Project(Plane plane, Vector3 point)
        {
            Project(ref plane, ref point, out var result);
            return result;
        }


        /// <summary>
        /// Scales a plane by the given value.
        /// </summary>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <param name="plane">The plane to scale.</param>
        /// <returns>The scaled plane.</returns>
        public static Plane operator *(float scale, Plane plane)
        {
            return new Plane(plane.Normal.X * scale, plane.Normal.Y * scale, plane.Normal.Z * scale, plane.D * scale);
        }

        /// <summary>
        /// Scales a plane by the given value.
        /// </summary>
        /// <param name="plane">The plane to scale.</param>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <returns>The scaled plane.</returns>
        public static Plane operator *(Plane plane, float scale)
        {
            return new Plane(plane.Normal.X * scale, plane.Normal.Y * scale, plane.Normal.Z * scale, plane.D * scale);
        }

        /// <summary>
        /// Scales the plane by the given scaling factor.
        /// </summary>
        /// <param name="value">The plane to scale.</param>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <param name="result">When the method completes, contains the scaled plane.</param>
        public static void Multiply(ref readonly Plane value, float scale, out Plane result)
        {
            result.Normal.X = value.Normal.X * scale;
            result.Normal.Y = value.Normal.Y * scale;
            result.Normal.Z = value.Normal.Z * scale;
            result.D = value.D * scale;
        }

        /// <summary>
        /// Scales the plane by the given scaling factor.
        /// </summary>
        /// <param name="value">The plane to scale.</param>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <returns>The scaled plane.</returns>
        public static Plane Multiply(Plane value, float scale)
        {
            return new Plane(value.Normal.X * scale, value.Normal.Y * scale, value.Normal.Z * scale, value.D * scale);
        }
        
        /// <summary>
        /// Negates a plane by negating all its coefficients, which result in a plane in opposite direction.
        /// </summary>
        /// <returns>The negated plane.</returns>
        public static Plane operator -(Plane plane)
        {
            return new Plane(-plane.Normal.X, -plane.Normal.Y, -plane.Normal.Z, -plane.D);
        }
        
        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <param name="result">When the method completes, contains the transformed plane.</param>
        public static void Transform(ref readonly Plane plane, ref readonly Quaternion rotation, out Plane result)
        {
            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;
            float wx = rotation.W * x2;
            float wy = rotation.W * y2;
            float wz = rotation.W * z2;
            float xx = rotation.X * x2;
            float xy = rotation.X * y2;
            float xz = rotation.X * z2;
            float yy = rotation.Y * y2;
            float yz = rotation.Y * z2;
            float zz = rotation.Z * z2;

            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;

            result.Normal.X = ((x * ((1.0f - yy) - zz)) + (y * (xy - wz))) + (z * (xz + wy));
            result.Normal.Y = ((x * (xy + wz)) + (y * ((1.0f - xx) - zz))) + (z * (yz - wx));
            result.Normal.Z = ((x * (xz - wy)) + (y * (yz + wx))) + (z * ((1.0f - xx) - yy));
            result.D = plane.D;
        }

        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(Plane plane, Quaternion rotation)
        {
            Plane result;
            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;
            float wx = rotation.W * x2;
            float wy = rotation.W * y2;
            float wz = rotation.W * z2;
            float xx = rotation.X * x2;
            float xy = rotation.X * y2;
            float xz = rotation.X * z2;
            float yy = rotation.Y * y2;
            float yz = rotation.Y * z2;
            float zz = rotation.Z * z2;

            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;

            result.Normal.X = ((x * ((1.0f - yy) - zz)) + (y * (xy - wz))) + (z * (xz + wy));
            result.Normal.Y = ((x * (xy + wz)) + (y * ((1.0f - xx) - zz))) + (z * (yz - wx));
            result.Normal.Z = ((x * (xz - wy)) + (y * (yz + wx))) + (z * ((1.0f - xx) - yy));
            result.D = plane.D;

            return result;
        }

        /// <summary>
        /// Transforms an array of normalized planes by a quaternion rotation.
        /// </summary>
        /// <param name="planes">The array of normalized planes to transform.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="planes"/> is <c>null</c>.</exception>
        public static void Transform(Plane[] planes, ref readonly Quaternion rotation)
        {
            if (planes == null)
                throw new ArgumentNullException(nameof(planes));

            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;
            float wx = rotation.W * x2;
            float wy = rotation.W * y2;
            float wz = rotation.W * z2;
            float xx = rotation.X * x2;
            float xy = rotation.X * y2;
            float xz = rotation.X * z2;
            float yy = rotation.Y * y2;
            float yz = rotation.Y * z2;
            float zz = rotation.Z * z2;

            for (int i = 0; i < planes.Length; ++i)
            {
                float x = planes[i].Normal.X;
                float y = planes[i].Normal.Y;
                float z = planes[i].Normal.Z;

                /*
                 * Note:
                 * Factor common arithmetic out of loop.
                */
                planes[i].Normal.X = ((x * ((1.0f - yy) - zz)) + (y * (xy - wz))) + (z * (xz + wy));
                planes[i].Normal.Y = ((x * (xy + wz)) + (y * ((1.0f - xx) - zz))) + (z * (yz - wx));
                planes[i].Normal.Z = ((x * (xz - wy)) + (y * (yz + wx))) + (z * ((1.0f - xx) - yy));
            }
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="transformation">The transformation matrix.</param>
        /// <param name="result">When the method completes, contains the transformed plane.</param>
        public static void Transform(ref readonly Plane plane, ref readonly Matrix transformation, out Plane result)
        {
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;

            Matrix.Invert(transformation, out var inverse);

            result.Normal.X = (((x * inverse.M11) + (y * inverse.M12)) + (z * inverse.M13)) + (d * inverse.M14);
            result.Normal.Y = (((x * inverse.M21) + (y * inverse.M22)) + (z * inverse.M23)) + (d * inverse.M24);
            result.Normal.Z = (((x * inverse.M31) + (y * inverse.M32)) + (z * inverse.M33)) + (d * inverse.M34);
            result.D = (((x * inverse.M41) + (y * inverse.M42)) + (z * inverse.M43)) + (d * inverse.M44);
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="transformation">The transformation matrix.</param>
        /// <returns>When the method completes, contains the transformed plane.</returns>
        public static Plane Transform(Plane plane, Matrix transformation)
        {
            Plane result;
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;

            transformation = Matrix.Invert(transformation);
            result.Normal.X = (((x * transformation.M11) + (y * transformation.M12)) + (z * transformation.M13)) + (d * transformation.M14);
            result.Normal.Y = (((x * transformation.M21) + (y * transformation.M22)) + (z * transformation.M23)) + (d * transformation.M24);
            result.Normal.Z = (((x * transformation.M31) + (y * transformation.M32)) + (z * transformation.M33)) + (d * transformation.M34);
            result.D = (((x * transformation.M41) + (y * transformation.M42)) + (z * transformation.M43)) + (d * transformation.M44);

            return result;
        }

        /// <summary>
        /// Transforms an array of normalized planes by a matrix.
        /// </summary>
        /// <param name="planes">The array of normalized planes to transform.</param>
        /// <param name="transformation">The transformation matrix.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="planes"/> is <c>null</c>.</exception>
        public static void Transform(Plane[] planes, ref readonly Matrix transformation)
        {
            if (planes == null)
                throw new ArgumentNullException(nameof(planes));

            Matrix inverse;
            Matrix.Invert(transformation, out inverse);

            for (int i = 0; i < planes.Length; ++i)
            {
                Transform(ref planes[i], in transformation, out planes[i]);
            }
        }
    }
}

public static class PlaneExtensions2
{
    extension(Plane self)
    {
        /// <summary>
        /// Negates a plane by negating all its coefficients, which result in a plane in opposite direction.
        /// </summary>
        /// <param name="plane">The source plane.</param>
        /// <param name="result">When the method completes, contains the flipped plane.</param>
        public static void Negate(ref readonly Plane plane, out Plane result)
        {
            result.Normal.X = -plane.Normal.X;
            result.Normal.Y = -plane.Normal.Y;
            result.Normal.Z = -plane.Normal.Z;
            result.D = -plane.D;
        }

        /// <summary>
        /// Negates a plane by negating all its coefficients, which result in a plane in opposite direction.
        /// </summary>
        /// <param name="plane">The source plane.</param>
        /// <returns>The flipped plane.</returns>
        public static Plane Negate(Plane plane)
        {
            return new Plane(-plane.Normal.X, -plane.Normal.Y, -plane.Normal.Z, -plane.D);
        }
    }
}