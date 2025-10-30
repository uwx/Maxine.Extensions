using System.Globalization;
using System.Numerics;
using JetBrains.Annotations;

namespace Maxine.Extensions;

[PublicAPI]
public readonly record struct BoundingBox<T>(
    T? MinX = default, T? MaxX = default,
    T? MinY = default, T? MaxY = default,
    T? MinZ = default, T? MaxZ = default
) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    public T MinX { get; init; } = MinX ?? T.MinValue;
    public T MaxX { get; init; } = MaxX ?? T.MaxValue;
    public T MinY { get; init; } = MinY ?? T.MinValue;
    public T MaxY { get; init; } = MaxY ?? T.MaxValue;
    public T MinZ { get; init; } = MinZ ?? T.MinValue;
    public T MaxZ { get; init; } = MaxZ ?? T.MaxValue;
    
    public T Width => MaxX - MinX;
    public T Height => MaxY - MinY;
    public T Depth => MaxZ - MinZ;

    public Vector3 Min => new(float.CreateTruncating(MinX), float.CreateTruncating(MinY), float.CreateTruncating(MinZ));
    public Vector3 Max => new(float.CreateTruncating(MaxX), float.CreateTruncating(MaxY), float.CreateTruncating(MaxZ));
    
    public BoundingBox(T MinX, T MaxX, T MinY, T MaxY) : this(MinX, MaxX, MinY, MaxY, T.Zero, T.Zero)
    {
    }
    
    public BoundingBox(Vector2 min, Vector2 max) : this(
        T.CreateTruncating(min.X),
        T.CreateTruncating(max.X),
        T.CreateTruncating(min.Y),
        T.CreateTruncating(max.Y),
        T.Zero,
        T.Zero)
    {
    }
    
    public BoundingBox(Vector3 min, Vector3 max) : this(
        T.CreateTruncating(min.X),
        T.CreateTruncating(max.X),
        T.CreateTruncating(min.Y),
        T.CreateTruncating(max.Y),
        T.CreateTruncating(min.Z),
        T.CreateTruncating(max.Z))
    {
    }

    [Pure]
    public BoundingBox<T> UnionWith(T x, T y, T z)
    {
        var minX = T.Min(MinX, x);
        var maxX = T.Max(MaxX, x);
        var minY = T.Min(MinY, y);
        var maxY = T.Max(MaxY, y);
        var minZ = T.Min(MinZ, z);
        var maxZ = T.Max(MaxZ, z);
        return new BoundingBox<T>(minX, maxX, minY, maxY, minZ, maxZ);
    }

    [Pure]
    public BoundingBox<T> UnionWith(Vector3 vec3)
    {
        return UnionWith(T.CreateTruncating(vec3.X), T.CreateTruncating(vec3.Y), T.CreateTruncating(vec3.Z));
    }

    [Pure]
    public BoundingBox<T> UnionWith(BoundingBox<T> bbox)
    {
        var minX = T.Min(MinX, bbox.MinX);
        var maxX = T.Max(MaxX, bbox.MaxX);
        var minY = T.Min(MinY, bbox.MinY);
        var maxY = T.Max(MaxY, bbox.MaxY);
        var minZ = T.Min(MinZ, bbox.MinZ);
        var maxZ = T.Max(MaxZ, bbox.MaxZ);
        return new BoundingBox<T>(minX, maxX, minY, maxY, minZ, maxZ);
    }

    /// <summary>
    /// Reorders min / max and returns a new BoundingBox<T>
    /// </summary>
    /// <returns></returns>
    [Pure]
    public BoundingBox<T> ReorderMinMax()
    {
        return new BoundingBox<T>(
            T.Min(MinX, MaxX), T.Max(MinX, MaxX),
            T.Min(MinY, MaxY), T.Max(MinY, MaxY),
            T.Min(MinZ, MaxZ), T.Max(MinZ, MaxZ)
        );
    }

    [Pure]
    public BoundingBox<T> Scale(T scale)
    {
        return Scale(scale, scale);
    }
    
    [Pure]
    public BoundingBox<T> Scale(T scaleX, T scaleY)
    {
        return Scale(scaleX, scaleY, T.One);
    }
    
    [Pure]
    public BoundingBox<T> Scale(T scaleX, T scaleY, T scaleZ)
    {
        return new BoundingBox<T>(
                MinX - Width * scaleX, MaxX + Width * scaleX,
                MinY - Height * scaleY, MaxY + Height * scaleY,
                MinZ - Depth * scaleZ, MaxZ + Depth * scaleZ
        );
    }

    [Pure]
    public BoundingBox<T> Translate(T x, T y, T z)
    {
        return new BoundingBox<T>(
            MinX + x, MaxX + x,
            MinY + y, MaxY + y,
            MinZ + z, MaxZ + z
        );
    }
    
    /// <summary>
    /// Add padding around bbox (in bbox units)
    /// </summary>
    /// <param name="padding"></param>
    /// <returns></returns>
    [Pure]
    public BoundingBox<T> Pad(T padding)
    {
        return Pad(padding, padding, padding);
    }
    
    /// <summary>
    /// Add padding around bbox (in bbox units)
    /// </summary>
    /// <param name="paddingX"></param>
    /// <param name="paddingY"></param>
    /// <param name="paddingZ"></param>
    /// <returns></returns>
    [Pure]
    public BoundingBox<T> Pad(T paddingX, T paddingY, T paddingZ)
    {
        return new BoundingBox<T>(
            MinX - paddingX, MaxX + paddingX,
            MinY + paddingY, MaxY - paddingY,
            MinZ + paddingZ, MaxZ - paddingZ
        );
    }
    
    [Pure]
    public BoundingBox<T> ScaleAbsolute(T scaleX, T scaleY, T? scaleZ)
    {
        var scaleZOrOne = scaleZ ?? T.One;
        return new BoundingBox<T>(
            MinX * scaleX, MaxX * scaleX,
            MinY * scaleY, MaxY * scaleY,
            MinZ * scaleZOrOne, MaxZ * scaleZOrOne
        );
    }

    public Vector3 Center => new(
        float.CreateTruncating((MaxX - MinX) / T.CreateTruncating(2) + MinX),
        float.CreateTruncating((MaxY - MinY) / T.CreateTruncating(2) + MinY),
        float.CreateTruncating((MaxZ - MinZ) / T.CreateTruncating(2) + MinZ)
    );

    public static bool Contains(BoundingBox<T> bbox, T x, T y)
    {
        return bbox.MinX >= x && x <= bbox.MaxX
                && bbox.MinY >= y && y <= bbox.MaxY;
    }
    
    [Pure]
    public Vector2[] AsPoints()
    {
        return
        [
            new Vector2(float.CreateTruncating(MaxY), float.CreateTruncating(MinX)),
            new Vector2(float.CreateTruncating(MaxY), float.CreateTruncating(MaxX)),
            new Vector2(float.CreateTruncating(MinY), float.CreateTruncating(MaxX)),
            new Vector2(float.CreateTruncating(MinY), float.CreateTruncating(MinX)),
            new Vector2(float.CreateTruncating(MaxY), float.CreateTruncating(MinX))
        ];
    }

    [Pure]
    public static BoundingBox<T> AroundPoint(T lat, T lon, T size)
    {
        return new BoundingBox<T>(lon - size, lon + size, lat - size, lat + size, T.Zero, T.Zero);
    }
    
    [Pure]
    public static BoundingBox<T> AroundPoint(Vector2 point, T size)
    {
        return AroundPoint(T.CreateTruncating(point.X), T.CreateTruncating(point.Y), size);
    }

    public override string ToString()
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"Xmin: {MinX}, Xmax: {MaxX}, Ymin: {MinY}, Ymax: {MaxY}, Zmin: {MinZ}, Zmax: {MaxZ}, Center: {Center.X}, {Center.Y}, {Center.Z}"
        );
    }

    public T Area => (MaxX - MinX) * (MaxY - MinY);
}