using System.Numerics;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct LatLon<T>(Angle<T> Latitude, Angle<T> Longitude) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    // Radius of the Earth, ellipsoid, meters, WGS-84
    private const double R = 6_378_137;
    private static T _R => T.CreateTruncating(R);
    private static T _2 => T.CreateTruncating(2);
    private static T _60 => T.CreateTruncating(60);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LatLon(T lat, T lon) : this(Angle<T>.FromDegrees(lat), Angle<T>.FromDegrees(lon))
    {
    }
    
    // https://gis.stackexchange.com/a/2980
    /// <summary>
    /// Offsets this <see cref="LatLon"/> by cartesian coordinates.
    /// </summary>
    /// <param name="east">The west-east offset. Negative values go west, positive values go east.</param>
    /// <param name="south">The north-south offset. Negative values go north, positive values go south.</param>
    public LatLon<T> Move(Length<T> east, Length<T> south)
    {
        var (lat, lon) = this;
        
        //offsets in meters
        var dn = -south.Meters;
        var de = east.Meters;

        // coordinate offsets in radians
        var dLat = dn / _R;
        var dLon = de / (_R * Angle<T>.Cos(lat));

        // resulting position
        var latO = lat.AddRadians(dLat);
        var lonO = lon.AddRadians(dLon);

        return new LatLon<T>(latO, lonO);
    }

    // // https://en.wikipedia.org/wiki/Geographic_coordinate_system#Length_of_a_degree
    // public LatLon Move2(Length east, Length south)
    // {
    //     const double equatorialRadius = 6_378_137; // a
    //     const double polarRadius = 6_356_752.3142; // b
    //     const double axisRatio = polarRadius / equatorialRadius; // b / a
    //     
    //     var (lat, lon) = this;
    //
    //     var tanB = axisRatio * Angle<T>.Tan(lat);
    //     var B = T.Atan(tanB);
    //
    //     var lengthOfLonDegreeAtLat = (T.PI / 180) * equatorialRadius * T.Cos(B);
    // }

    // http://www.movable-type.co.uk/scripts/latlong.html
    /// <summary>
    /// Gets the distance in meters from this <see cref="LatLon"/> to another <see cref="LatLon"/>.
    /// <p/>
    /// Uses haversine formula: a = sin²(Δφ/2) + cosφ1·cosφ2 · sin²(Δλ/2); d = 2 · atan2(√a, √(a-1)).
    /// </summary>
    /// <param name="destination">The destination point</param>
    /// <returns>The between this point and destination point, in meters</returns>
    public Length<T> DistanceTo(LatLon<T> destination)
    {
        // a = sin²(Δφ/2) + cos(φ1)⋅cos(φ2)⋅sin²(Δλ/2)
        // δ = 2·atan2(√(a), √(1−a))
        // see mathforum.org/library/drmath/view/51879.html for derivation

        var (lat, lon) = this;
        var (destLat, destLon) = destination;
        var (deltaLat, deltaLon) = destination - this;

        var a = Angle<T>.Sin(deltaLat / _2) * Angle<T>.Sin(deltaLat / _2) + Angle<T>.Cos(lat) * Angle<T>.Cos(destLat) * Angle<T>.Sin(deltaLon / _2) * Angle<T>.Sin(deltaLon / _2);
        var c = _2 * T.Atan2(T.Sqrt(a), T.Sqrt(T.One - a));
        var d = _R * c;

        return d.Meters();
    }
    
    public Angle<T> InitialBearingTo(LatLon<T> point)
    {
        if (this == point) return Angle<T>.NaN;
        
        // tanθ = sinΔλ⋅cosφ2 / cosφ1⋅sinφ2 − sinφ1⋅cosφ2⋅cosΔλ
        // see mathforum.org/library/drmath/view/55417.html for derivation

        var (lat, lon) = this;
        var (destLat, destLon) = point;

        var deltaLon = (destLon - lon);

        var x = Angle<T>.Cos(lat) * Angle<T>.Sin(destLat) - Angle<T>.Sin(lat) * Angle<T>.Cos(destLat) * Angle<T>.Cos(deltaLon);
        var y = Angle<T>.Sin(deltaLon) * Angle<T>.Cos(destLat);
        var bearing = Angle<T>.Atan2(y, x);

        return bearing.Normalize();
    }

    public LatLon<T> MoveTowardsBearing(Length<T> distance, Angle<T> bearing)
    {
        // sinφ2 = sinφ1⋅cosδ + cosφ1⋅sinδ⋅cosθ
        // tanΔλ = sinθ⋅sinδ⋅cosφ1 / cosδ−sinφ1⋅sinφ2
        // see mathforum.org/library/drmath/view/52049.html for derivation
        
        var δ = (distance.Meters / _R).Radians(); // angular distance in radians

        var (lat, lon) = this;

        var sinφ2 = Angle<T>.Sin(lat) * Angle<T>.Cos(δ) + Angle<T>.Cos(lat) * Angle<T>.Sin(δ) * Angle<T>.Cos(bearing);
        var φ2 = Angle<T>.Asin(sinφ2);
        var y = Angle<T>.Sin(bearing) * Angle<T>.Sin(δ) * Angle<T>.Cos(lat);
        var x = Angle<T>.Cos(δ) - Angle<T>.Sin(lat) * sinφ2;
        var λ2 = lon + Angle<T>.Atan2(y, x);

        return new LatLon<T>(φ2, λ2);
    }

    public LatLon<T> MoveTowards(LatLon<T> destination, Length<T> maxDistanceDelta)
    {
        if (DistanceTo(destination) < maxDistanceDelta)
        {
            return destination;
        }
        
        var bearing = InitialBearingTo(destination);
        return MoveTowardsBearing(maxDistanceDelta, bearing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator -(LatLon<T> a, LatLon<T> b) => new(a.Latitude - b.Latitude, a.Longitude - b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator -(LatLon<T> a) => new(-a.Latitude, -a.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator +(LatLon<T> a, LatLon<T> b) => new(a.Latitude + b.Latitude, a.Longitude + b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator /(LatLon<T> a, LatLon<T> b) => new(a.Latitude / b.Latitude, a.Longitude / b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator *(LatLon<T> a, T scalar) => new(a.Latitude * scalar, a.Longitude * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator /(LatLon<T> a, T scalar) => new(a.Latitude / scalar, a.Longitude / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon<T> operator *(T scalar, LatLon<T> a) => new(a.Latitude * scalar, a.Longitude * scalar);

    public override string ToString()
    {
        var dirNS = Angle<T>.Sign(Latitude) < 0 ? 'S' : 'N';
        var dirWE = Angle<T>.Sign(Longitude) < 0 ? 'W' : 'E';

        return $"{ToDms(Latitude)} {dirNS}, {ToDms(Longitude)} {dirWE}";
    }

    // https://stackoverflow.com/a/4505023
    public static string ToDms(Angle<T> angle)
    {
        // 41°24'12.2"N 2°10'26.5"E

        var value = angle.Degrees;

        value = T.Abs(value);

        var degrees = T.Truncate(value);

        value = (value - degrees) * _60;

        var minutes = T.Truncate(value);
        var seconds = (value - minutes) * _60;

        return $@"{degrees}°{minutes}'{seconds:F1}""";
    }
}