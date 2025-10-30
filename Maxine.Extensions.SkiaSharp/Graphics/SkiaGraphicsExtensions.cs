using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace Poki.Shared.Graphics;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class SkiaGraphicsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SKPoint ToSKPoint(this PointF point)
    {
        return new SKPoint(point.X, point.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SKColor ToSKColor(this Color color)
    {
        return new SKColor(unchecked((uint) color.ToArgb()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SKRect ToSKRect(this Rectangle source)
    {
        return new SKRect(source.Left, source.Top, source.Right, source.Bottom);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SKRect ToSKRect(this RectangleF source)
    {
        return new SKRect(source.Left, source.Top, source.Right, source.Bottom);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SKRoundRect ToSKRoundRect(this RoundRectangleF source)
    {
        var skRect = source.Rect.ToSKRect();
        var skRoundRect = new SKRoundRect(skRect);
        skRoundRect.SetRectRadii(skRect, new[]
        {
            source.UpperLeft.ToSKPoint(),
            source.UpperRight.ToSKPoint(),
            source.LowerLeft.ToSKPoint(),
            source.LowerRight.ToSKPoint(),
        });
        return skRoundRect;
    }

}