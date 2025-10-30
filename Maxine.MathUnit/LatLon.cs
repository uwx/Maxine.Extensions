using System;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct LatLon(Angle Latitude, Angle Longitude)
{
    // Radius of the Earth, ellipsoid, meters, WGS-84
    private const double R = 6_378_137;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LatLon(double lat, double lon) : this(Angle.FromDegrees(lat), Angle.FromDegrees(lon))
    {
    }

    public (double x, double y) ToSphericalMercator() => SphericalMercator2.FromLatLon(Latitude, Longitude);

    // https://gis.stackexchange.com/a/2980
    /// <summary>
    /// Offsets this <see cref="LatLon"/> by cartesian coordinates.
    /// </summary>
    /// <param name="east">The west-east offset. Negative values go west, positive values go east.</param>
    /// <param name="south">The north-south offset. Negative values go north, positive values go south.</param>
    public LatLon Move(Length east, Length south)
    {
        var (lat, lon) = this;
        
        //offsets in meters
        var dn = -south.Meters;
        var de = east.Meters;

        // coordinate offsets in radians
        var dLat = dn / R;
        var dLon = de / (R * Angle.Cos(lat));

        // resulting position
        var latO = lat.AddRadians(dLat);
        var lonO = lon.AddRadians(dLon);

        return new LatLon(latO, lonO);
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
    //     var tanB = axisRatio * Angle.Tan(lat);
    //     var B = Math.Atan(tanB);
    //
    //     var lengthOfLonDegreeAtLat = (Math.PI / 180) * equatorialRadius * Math.Cos(B);
    // }

    // http://www.movable-type.co.uk/scripts/latlong.html
    /// <summary>
    /// Gets the distance in meters from this <see cref="LatLon"/> to another <see cref="LatLon"/>.
    /// <p/>
    /// Uses haversine formula: a = sin²(Δφ/2) + cosφ1·cosφ2 · sin²(Δλ/2); d = 2 · atan2(√a, √(a-1)).
    /// </summary>
    /// <param name="destination">The destination point</param>
    /// <returns>The between this point and destination point, in meters</returns>
    public Length DistanceTo(LatLon destination)
    {
        // a = sin²(Δφ/2) + cos(φ1)⋅cos(φ2)⋅sin²(Δλ/2)
        // δ = 2·atan2(√(a), √(1−a))
        // see mathforum.org/library/drmath/view/51879.html for derivation

        var (lat, lon) = this;
        var (destLat, destLon) = destination;
        var (deltaLat, deltaLon) = destination - this;

        var a = Angle.Sin(deltaLat / 2) * Angle.Sin(deltaLat / 2) + Angle.Cos(lat) * Angle.Cos(destLat) * Angle.Sin(deltaLon / 2) * Angle.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c;

        return d.Meters();
    }
    
    public Angle InitialBearingTo(LatLon point)
    {
        if (this == point) return Angle.NaN;
        
        // tanθ = sinΔλ⋅cosφ2 / cosφ1⋅sinφ2 − sinφ1⋅cosφ2⋅cosΔλ
        // see mathforum.org/library/drmath/view/55417.html for derivation

        var (lat, lon) = this;
        var (destLat, destLon) = point;

        var deltaLon = (destLon - lon);

        var x = Angle.Cos(lat) * Angle.Sin(destLat) - Angle.Sin(lat) * Angle.Cos(destLat) * Angle.Cos(deltaLon);
        var y = Angle.Sin(deltaLon) * Angle.Cos(destLat);
        var bearing = Angle.Atan2(y, x);

        return bearing.Normalize();
    }

    public LatLon MoveTowardsBearing(Length distance, Angle bearing)
    {
        // sinφ2 = sinφ1⋅cosδ + cosφ1⋅sinδ⋅cosθ
        // tanΔλ = sinθ⋅sinδ⋅cosφ1 / cosδ−sinφ1⋅sinφ2
        // see mathforum.org/library/drmath/view/52049.html for derivation
        
        var δ = (distance.Meters / R).Radians(); // angular distance in radians

        var (lat, lon) = this;

        var sinφ2 = Angle.Sin(lat) * Angle.Cos(δ) + Angle.Cos(lat) * Angle.Sin(δ) * Angle.Cos(bearing);
        var φ2 = Angle.Asin(sinφ2);
        var y = Angle.Sin(bearing) * Angle.Sin(δ) * Angle.Cos(lat);
        var x = Angle.Cos(δ) - Angle.Sin(lat) * sinφ2;
        var λ2 = lon + Angle.Atan2(y, x);

        return new LatLon(φ2, λ2);
    }

    public LatLon MoveTowards(LatLon destination, Length maxDistanceDelta)
    {
        if (DistanceTo(destination) < maxDistanceDelta)
        {
            return destination;
        }
        
        var bearing = InitialBearingTo(destination);
        return MoveTowardsBearing(maxDistanceDelta, bearing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator -(LatLon a, LatLon b) => new(a.Latitude - b.Latitude, a.Longitude - b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator -(LatLon a) => new(-a.Latitude, -a.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator +(LatLon a, LatLon b) => new(a.Latitude + b.Latitude, a.Longitude + b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator /(LatLon a, LatLon b) => new(a.Latitude / b.Latitude, a.Longitude / b.Longitude);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator *(LatLon a, double scalar) => new(a.Latitude * scalar, a.Longitude * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator /(LatLon a, double scalar) => new(a.Latitude / scalar, a.Longitude / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LatLon operator *(double scalar, LatLon a) => new(a.Latitude * scalar, a.Longitude * scalar);

    public static LatLon FromSphericalMercator(double x, double y)
    {
        return SphericalMercator2.ToLatLon(x, y);
    }

    public override string ToString()
    {
        var dirNS = Angle.Sign(Latitude) < 0 ? 'S' : 'N';
        var dirWE = Angle.Sign(Longitude) < 0 ? 'W' : 'E';

        return $"{ToDms(Latitude)} {dirNS}, {ToDms(Longitude)} {dirWE}";
    }

    // https://stackoverflow.com/a/4505023
    public static string ToDms(Angle angle)
    {
        // 41°24'12.2"N 2°10'26.5"E

        var value = angle.Degrees;

        value = Math.Abs(value);

        var degrees = Math.Truncate(value);

        value = (value - degrees) * 60;

        var minutes = Math.Truncate(value);
        var seconds = (value - minutes) * 60;

        return $@"{degrees}°{minutes}'{seconds:F1}""";
    }
}