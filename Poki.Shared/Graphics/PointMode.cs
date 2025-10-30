namespace Poki.Shared;

/// <summary>Possible values to interpret the incoming array of points for the <see cref="M:SkiaSharp.SKCanvas.DrawPoints(SkiaSharp.SKPointMode,SkiaSharp.SKPoint[],SkiaSharp.SKPaint)" /> method.</summary>
public enum PointMode
{
    /// <summary>Interpret the data as coordinates for points.</summary>
    Points,
    /// <summary>Interpret the data as coordinates for lines.</summary>
    Lines,
    /// <summary>Interpret the data as coordinates for polygons.</summary>
    Polygon,
}