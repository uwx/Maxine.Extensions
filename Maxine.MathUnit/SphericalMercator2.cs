namespace MathUnit;

public static class SphericalMercator2
{
    private const double Radius = 6_378_137;
    private const double D2R = Math.PI/180;
    private const double HalfPi = Math.PI/2;

    public static (double x, double y) FromLatLon(Angle lat, Angle lon)
    {
        var x = Radius * lon.Radians;
        var y = Radius * Math.Log(Math.Tan(Math.PI * 0.25 + lat.Radians * 0.5));

        return (x, y);
    }

    public static LatLon ToLatLon(double x, double y)
    {
        var ts = Math.Exp(-y / Radius);
        var latRadians = HalfPi - 2 * Math.Atan(ts);

        var lonRadians = x / Radius;

        return new LatLon(latRadians.Radians(), lonRadians.Radians());
    }
}