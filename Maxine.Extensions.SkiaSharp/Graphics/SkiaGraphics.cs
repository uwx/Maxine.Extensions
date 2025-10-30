using System.Drawing;
using SkiaSharp;

namespace Poki.Shared.Graphics;

public sealed class SkiaGraphics : IGraphics
{
    private static readonly SKTypeface Typeface = SKTypeface.FromFile("resources/FOT-UDKakugo_Large Pr6N DB.otf")
        ?? throw new InvalidOperationException("Could not load Poki typeface");

    // private static readonly SKShaper FontShaper = new(Typeface);
    private static readonly CustomSKShaper CustomFontShaper = new(Typeface);

    private readonly SKSurface _surface;
    
    public SKCanvas Canvas => _surface.Canvas;
    public SKFontMetrics FontMetrics => _fillPaint.FontMetrics;

    private readonly SKPaint _fillPaint;
    private readonly SKPaint _drawPaint;
    
    public SkiaGraphics(
        int width,
        int height,
        SKColorType? colorType = null,
        SKAlphaType alphaType = SKAlphaType.Premul,
        SKColorSpace? colorspace = null
    )
    {
        _surface = SKSurface.Create(new SKImageInfo(
            width,
            height,
            colorType ?? SKImageInfo.PlatformColorType,
            alphaType,
            colorspace
        ));

        _fillPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            LcdRenderText = true,
            SubpixelText = true,
            HintingLevel = SKPaintHinting.Normal,
            TextAlign = SKTextAlign.Left,
            TextSize = 12,
            Typeface = Typeface
        };

        _drawPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
        };
    }

    public void SetColor(Color color)
    {
        _fillPaint.Color = color.ToSKColor();
        _drawPaint.Color = color.ToSKColor();
    }

    public void SetFontSize(float size)
    {
        _fillPaint.TextSize = size;
    }

    public void SetTextAlign(TextAlignment textAlign)
    {
        _fillPaint.TextAlign = (SKTextAlign)textAlign;
    }
    
    public void Translate(float dx, float dy)
    {
        Canvas.Translate(dx, dy);
    }

    public void Clear(Color color) => Canvas.Clear(color.ToSKColor());

    public void DrawLine(float x0, float y0, float x1, float y1) => Canvas.DrawLine(x0, y0, x1, y1, _drawPaint);

    public void DrawRect(float x, float y, float w, float h) => Canvas.DrawRect(x, y, w, h, _drawPaint);

    public void DrawRoundRect(RoundRectangleF rect)
    {
        if (rect.IsSimple)
        {
            Canvas.DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, rect.SimpleRadii.X, rect.SimpleRadii.Y, _drawPaint);
        }
        else
        {
            Canvas.DrawRoundRect(rect.ToSKRoundRect(), _drawPaint);
        }
    }

    public void DrawOval(float cx, float cy, float rx, float ry) => Canvas.DrawOval(cx, cy, rx, ry, _drawPaint);

    public void DrawCircle(float cx, float cy, float radius) => Canvas.DrawCircle(cx, cy, radius, _drawPaint);

    /// <param name="path">The path to draw.</param>
    /// <summary>Draws a path in the canvas.</summary>
    public void DrawPath(SKPath path) => Canvas.DrawPath(path, _drawPaint);

    public void DrawPoints(PointMode mode, PointF[] points) => Canvas.DrawPoints((SKPointMode) mode, Array.ConvertAll(points, static e => new SKPoint(e.X, e.Y)), _drawPaint);

    public void DrawPoint(float x, float y) => Canvas.DrawPoint(x, y, _drawPaint);

    /// <param name="image">The image to draw.</param>
    /// <param name="p">The destination coordinates for the image.</param>
    /// <param name="paint">The paint to use when drawing the image.</param>
    /// <summary>Draws an image on the canvas.</summary>
    /// <remarks />
    public void DrawImage(SKImage image, PointF p, SKPaint? paint = null) => Canvas.DrawImage(image, p.ToSKPoint(), paint);

    /// <param name="image">The image to draw.</param>
    /// <param name="x">The destination x-coordinate for the image.</param>
    /// <param name="y">The destination y-coordinate for the image.</param>
    /// <param name="paint">The paint to use when drawing the image.</param>
    /// <summary>Draws an image on the canvas.</summary>
    /// <remarks />
    public void DrawImage(SKImage image, float x, float y, SKPaint? paint = null) => Canvas.DrawImage(image, x, y, paint);

    /// <param name="image">The image to draw.</param>
    /// <param name="dest">The region to draw the image into.</param>
    /// <param name="paint">The paint to use when drawing the image.</param>
    /// <summary>Draws an image on the canvas.</summary>
    /// <remarks />
    public void DrawImage(SKImage image, RectangleF dest, SKPaint? paint = null) => Canvas.DrawImage(image, dest.ToSKRect(), paint);

    /// <param name="image">The image to draw.</param>
    /// <param name="source">The source region to copy.</param>
    /// <param name="dest">The region to draw the image into.</param>
    /// <param name="paint">The paint to use when drawing the image.</param>
    /// <summary>Draws an image on the canvas.</summary>
    /// <remarks />
    public void DrawImage(SKImage image, RectangleF source, RectangleF dest, SKPaint? paint = null) => Canvas.DrawImage(image, source.ToSKRect(), dest.ToSKRect(), paint);

    /// <param name="picture">The picture to draw.</param>
    /// <param name="x">The destination x-coordinate for the picture.</param>
    /// <param name="y">The destination y-coordinate for the picture.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a picture on the canvas.</summary>
    /// <remarks />
    public void DrawPicture(SKPicture picture, float x, float y, SKPaint? paint = null) => Canvas.DrawPicture(picture, x, y, paint);

    /// <param name="picture">The picture to draw.</param>
    /// <param name="p">The destination coordinates for the picture.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a picture on the canvas.</summary>
    /// <remarks />
    public void DrawPicture(SKPicture picture, PointF p, SKPaint? paint = null) => Canvas.DrawPicture(picture, p.ToSKPoint(), paint);

    /// <param name="picture">The picture to draw.</param>
    /// <param name="matrix">The matrix to apply while painting.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a picture on the canvas.</summary>
    /// <remarks>
    /// This is equivalent to calling <see cref="SKCanvas.Save"/>, followed by
    /// <see cref="SKCanvas.Concat"/> specified `matrix`,
    /// <see cref="SKCanvas.DrawPicture(SkiaSharp.SKPicture,SkiaSharp.SKPaint)"/>
    /// and then <see cref="SKCanvas.Restore"/>.
    /// 
    /// If paint is non-null, the picture is drawn into a temporary buffer, and then
    /// the paint's alpha, color filter, image filter, blend mode are applied to that
    /// buffer as it is drawn to the canvas.
    /// </remarks>
    public void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint? paint = null) => Canvas.DrawPicture(picture, ref matrix, paint);

    /// <param name="picture">The picture to draw.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a picture on the canvas.</summary>
    /// <remarks />
    public void DrawPicture(SKPicture picture, SKPaint? paint = null) => Canvas.DrawPicture(picture, paint);

    /// <param name="drawable">The drawable to draw.</param>
    /// <param name="matrix">The matrix to apply while painting.</param>
    /// <summary>Draws a drawable on the canvas.</summary>
    /// <remarks />
    public void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix) => Canvas.DrawDrawable(drawable, ref matrix);

    /// <param name="drawable">The drawable to draw.</param>
    /// <param name="x">The destination x-coordinate for the drawable.</param>
    /// <param name="y">The destination y-coordinate for the drawable.</param>
    /// <summary>Draws a drawable on the canvas.</summary>
    /// <remarks />
    public void DrawDrawable(SKDrawable drawable, float x, float y) => Canvas.DrawDrawable(drawable, x, y);

    /// <param name="drawable">The drawable to draw.</param>
    /// <param name="p">The destination coordinates for the drawable.</param>
    /// <summary>Draws a drawable on the canvas.</summary>
    /// <remarks />
    public void DrawDrawable(SKDrawable drawable, PointF p) => Canvas.DrawDrawable(drawable, p.ToSKPoint());

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="p">The destination coordinates for the bitmap.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, PointF p, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, p.ToSKPoint(), paint);

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="x">The destination x-coordinate for the bitmap.</param>
    /// <param name="y">The destination y-coordinate for the bitmap.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, float x, float y, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, x, y, paint);

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="dest">The region to draw the bitmap into.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, SKRect dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, dest, paint);

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="dest">The region to draw the bitmap into.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, Rectangle dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, dest.ToSKRect(), paint);

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="source">The source region to copy.</param>
    /// <param name="dest">The region to draw the bitmap into.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, SKRect source, SKRect dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, source, dest, paint);

    /// <param name="bitmap">The bitmap to draw.</param>
    /// <param name="source">The source region to copy.</param>
    /// <param name="dest">The region to draw the bitmap into.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a bitmap on the canvas.</summary>
    /// <remarks />
    public void DrawBitmap(SKBitmap bitmap, Rectangle source, Rectangle dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, source.ToSKRect(), dest.ToSKRect(), paint);

    /// <param name="surface">The surface to draw.</param>
    /// <param name="p">The destination coordinates for the surface.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a surface on the canvas.</summary>
    /// <remarks />
    public void DrawSurface(SKSurface surface, PointF p, SKPaint? paint = null) => Canvas.DrawSurface(surface, p.ToSKPoint(), paint);

    /// <param name="surface">The surface to draw.</param>
    /// <param name="x">The destination x-coordinate for the surface.</param>
    /// <param name="y">The destination y-coordinate for the surface.</param>
    /// <param name="paint">The paint to use when drawing the picture.</param>
    /// <summary>Draws a surface on the canvas.</summary>
    /// <remarks />
    public void DrawSurface(SKSurface surface, float x, float y, SKPaint? paint = null) => Canvas.DrawSurface(surface, x, y, paint);

    /// <param name="text">The text blob to draw.</param>
    /// <param name="x">The x-coordinate of the origin of the text being drawn.</param>
    /// <param name="y">The y-coordinate of the origin of the text being drawn.</param>
    /// <summary>Draws a text blob on the canvas at the specified coordinates.</summary>
    /// <remarks />
    public void DrawString(SKTextBlob text, float x, float y) => Canvas.DrawText(text, x, y, _fillPaint);

    public void DrawString(string text, PointF p) => DrawAlignedShapedText(text, p.X, p.Y);

    public void DrawString(string text, float x, float y) => DrawAlignedShapedText(text, x, y);

    /// <param name="text">The text to draw.</param>
    /// <param name="x">The x-coordinate of the origin of the text being drawn.</param>
    /// <param name="y">The y-coordinate of the origin of the text being drawn.</param>
    /// <param name="font">The font to draw the text with.</param>
    /// <summary>Draws text on the canvas at the specified coordinates.</summary>
    public void DrawString(string text, float x, float y, SKFont font) => Canvas.DrawText(text, x, y, font, _fillPaint);

    public void FillRect(float x, float y, float w, float h) => Canvas.DrawRect(x, y, w, h, _fillPaint);

    /// <param name="rect">The rounded rectangle to draw.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    /// <remarks>The paint to use when drawing the rounded rectangle.</remarks>
    public void FillRoundRect(RoundRectangleF rect)
    {
        if (rect.IsSimple)
        {
            Canvas.DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, rect.SimpleRadii.X, rect.SimpleRadii.Y, _fillPaint);
        }
        else
        {
            Canvas.DrawRoundRect(rect.ToSKRoundRect(), _fillPaint);
        }
    }

    public void FillRoundRect(float x, float y, float w, float h, float rx, float ry) => Canvas.DrawRoundRect(x, y, w, h, rx, ry, _fillPaint);

    public void FillOval(float cx, float cy, float rx, float ry) => Canvas.DrawOval(cx, cy, rx, ry, _fillPaint);

    public void FillCircle(float cx, float cy, float radius) => Canvas.DrawCircle(cx, cy, radius, _fillPaint);

    public void FillCircle(PointF c, float radius) => Canvas.DrawCircle(c.ToSKPoint(), radius, _fillPaint);

    public void DrawMultilineString(string text, float x, float y, float addSpacing = 0)
    {
	    var metrics = _fillPaint.FontMetrics;
	    var lineHeight = -metrics.Ascent;

        var textY = y;
        
	    foreach (var line in text.Split('\n'))
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                DrawAlignedShapedText(text, x, textY);
            }

            textY += lineHeight + addSpacing;
        }
    }

    /// <summary>
    /// Copy of DrawShapedText using <see cref="CustomSKShaper"/> that supports aligned text.
    /// </summary>
    /// <param name="text">The text</param>
    /// <param name="x">The origin X coordinate of the text</param>
    /// <param name="y">The origin Y coordinate of the text</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If the <see cref="_fillPaint"/>'s text alignment format is not <see cref="SKTextAlign.Left"/>,
    /// <see cref="SKTextAlign.Center"/>, <see cref="SKTextAlign.Right"/>
    /// </exception>
    private void DrawAlignedShapedText(string text, float x, float y)
    {
        var textAlign = _fillPaint.TextAlign;

        using var font = _fillPaint.ToFont();
        font.Typeface = CustomFontShaper.Typeface;

        // shape the text
        var result = CustomFontShaper.Shape(text, x, y, _fillPaint);

        // create the text blob
        using var builder = new SKTextBlobBuilder();
        var run = builder.AllocatePositionedRun(font, result.Codepoints.Length);

        // copy the glyphs
        var g = run.GetGlyphSpan();
        var p = run.GetPositionSpan();
        for (var i = 0; i < result.Codepoints.Length; i++)
        {
            g[i] = (ushort)result.Codepoints[i];
            p[i] = result.Points[i];
        }

        // build
        using var textBlob = builder.Build();

        var width = result.Width;
        var xOffset = textAlign switch
        {
            SKTextAlign.Left => 0,
            SKTextAlign.Center => -(width / 2f),
            SKTextAlign.Right => -width,
            _ => throw new ArgumentOutOfRangeException(nameof(textAlign), textAlign, "Unknown alignment type")
        };
        
        // draw the text
        Canvas.DrawText(textBlob, xOffset, 0.0f, _fillPaint);
    }

    /// <summary>
    /// Draws a bitmap on the canvas, centered on an XY coordinate.
    /// </summary>
    /// <param name="bitmap">The bitmap to draw</param>
    /// <param name="x">The origin X coordinate</param>
    /// <param name="y">The origin Y coordinate</param>
    public void DrawBitmapCentered(SKBitmap bitmap, float x, float y)
    {
        Canvas.DrawBitmap(bitmap, x - (bitmap.Width / 2f), y - (bitmap.Height / 2f));
    }

    public void Dispose()
    {
        _surface.Dispose();
        _fillPaint.Dispose();
        _drawPaint.Dispose();
    }
    
    public Stream SaveToStream(ImageFormat format = ImageFormat.Png, int quality = 100)
    {
        using var image = _surface.Snapshot();
        return image.Encode((SKEncodedImageFormat)format, quality).AsStream(true);
    }
}