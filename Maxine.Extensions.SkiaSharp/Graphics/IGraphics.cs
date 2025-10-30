using System.Drawing;
using JetBrains.Annotations;

namespace Poki.Shared.Graphics;

/// <summary>The various formats used by <see cref="IGraphics.SaveToStream"/>.</summary>
public enum ImageFormat
{
    /// <summary>The BMP image format.</summary>
    Bmp,
    /// <summary>The GIF image format.</summary>
    Gif,
    /// <summary>The ICO image format.</summary>
    Ico,
    /// <summary>The JPEG image format.</summary>
    Jpeg,
    /// <summary>The PNG image format.</summary>
    Png,
    /// <summary>The WBMP image format.</summary>
    Wbmp,
    /// <summary>The WebP image format.</summary>
    Webp,
    /// <summary>The PKM image format.</summary>
    Pkm,
    /// <summary>The KTX image format.</summary>
    Ktx,
    /// <summary>The ASTC image format.</summary>
    Astc,
    /// <summary>The Adobe DNG image format.</summary>
    Dng,
    /// <summary>The HEIF or High Efficiency Image File format.</summary>
    Heif,
    /// <summary>To be added.</summary>
    Avif,
}

[PublicAPI]
public interface IGraphics : IDisposable
{
    void SetColor(Color color);
    void SetFontSize(float size);
    void SetTextAlign(TextAlignment alignment);

    /// <param name="dx">The distance to translate in the x-direction</param>
    /// <param name="dy">The distance to translate in the y-direction.</param>
    /// <summary>Pre-concatenates the current matrix with the specified translation.</summary>
    void Translate(float dx, float dy);

    /// <param name="point">The distance to translate.</param>
    /// <summary>Pre-concatenates the current matrix with the specified translation.</summary>
    void Translate(PointF point) => Translate(point.X, point.Y);

    /// <param name="color">The color to clear with.</param>
    /// <summary>Cleats the canvas with the specified color.</summary>
    void Clear(Color color);

    /// <param name="p0">The first point coordinates.</param>
    /// <param name="p1">The second point coordinates.</param>
    /// <summary>Draws a line on the canvas.</summary>
    void DrawLine(PointF p0, PointF p1) => DrawLine(p0.X, p0.Y, p1.X, p1.Y);

    /// <param name="x0">The first point x-coordinate.</param>
    /// <param name="y0">The first point y-coordinate.</param>
    /// <param name="x1">The second point x-coordinate.</param>
    /// <param name="y1">The second point y-coordinate.</param>
    /// <summary>Draws a line on the canvas.</summary>
    void DrawLine(float x0, float y0, float x1, float y1);

    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="w">The rectangle width.</param>
    /// <param name="h">The rectangle height.</param>
    /// <summary>Draws a rectangle in the canvas.</summary>
    void DrawRect(float x, float y, float w, float h);

    /// <param name="rect">The rectangle to draw.</param>
    /// <summary>Draws a rectangle in the canvas.</summary>
    void DrawRect(RectangleF rect) => DrawRect(rect.X, rect.Y, rect.Width, rect.Height);

    /// <param name="rect">The rounded rectangle to draw.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    void DrawRoundRect(RoundRectangleF rect);

    /// <param name="x">The x-coordinate of the rectangle.</param>
    /// <param name="y">The y-coordinate of the rectangle.</param>
    /// <param name="w">The rectangle width.</param>
    /// <param name="h">The rectangle height.</param>
    /// <param name="rx">The x-radius of the oval used to round the corners.</param>
    /// <param name="ry">The y-radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    void DrawRoundRect(float x, float y, float w, float h, float rx, float ry) => DrawRoundRect(new RoundRectangleF(x, y, w, h, rx, ry));

    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="rx">The x-radius of the oval used to round the corners.</param>
    /// <param name="ry">The y-radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    void DrawRoundRect(RectangleF rect, float rx, float ry) => DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, rx, ry);

    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="r">The radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    /// <remarks>The paint to use when drawing the rectangle.</remarks>
    void DrawRoundRect(RectangleF rect, SizeF r) => DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, r.Width, r.Height);

    /// <param name="cx">The center x-coordinate.</param>
    /// <param name="cy">The center y-coordinate.</param>
    /// <param name="rx">The vertical radius for the oval.</param>
    /// <param name="ry">The horizontal radius for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void DrawOval(float cx, float cy, float rx, float ry) => DrawRoundRect(RoundRectangleF.Oval(cx, cy, rx, ry));

    /// <param name="c">The center coordinates.</param>
    /// <param name="r">The radius for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void DrawOval(PointF c, SizeF r) => DrawOval(c.X, c.Y, r.Width, r.Height);

    /// <param name="rect">The bounding box for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void DrawOval(RectangleF rect) => DrawOval(rect.X, rect.Y, rect.Width, rect.Height);

    /// <param name="cx">The center x-coordinate.</param>
    /// <param name="cy">The center y-coordinate.</param>
    /// <param name="radius">The radius for the circle.</param>
    /// <summary>Draws a circle on the canvas.</summary>
    void DrawCircle(float cx, float cy, float radius) => DrawOval(cx, cy, radius, radius);

    /// <param name="c">The center coordinates.</param>
    /// <param name="radius">The radius for the circle.</param>
    /// <summary>Draws a circle on the canvas.</summary>
    void DrawCircle(PointF c, float radius) => DrawCircle(c.X, c.Y, radius);

    // /// <param name="path">The path to draw.</param>
    // /// <summary>Draws a path in the canvas.</summary>
    // void DrawPath(SKPath path) => Canvas.DrawPath(path, _drawPaint);

    /// <param name="mode">Determines how the points array will be interpreted: as points, as coordinates to draw lines, or as coordinates of a polygon.</param>
    /// <param name="points">The array of points to draw.</param>
    /// <summary>Draws an array of points, lines or a polygon in the canvas, one at a time.</summary>
    /// <remarks>
    ///                 <format type="text/markdown"><![CDATA[
    /// ## Remarks
    /// 
    /// For <xref:SkiaSharp.SKPointMode.Points>, each point is drawn centered at its
    /// coordinate, and its size is specified by the paint's stroke-width. It draws as
    /// a square, unless the paint's <xref:SkiaSharp.SKPaint.StrokeCap> is
    /// <xref:SkiaSharp.SKStrokeCap.Round>, in which the points are drawn as circles.
    /// 
    /// For <xref:SkiaSharp.SKPointMode.Lines>, each pair of points is drawn as a line
    /// segment, respecting the paint's settings for cap, join and width.
    /// 
    /// For <xref:SkiaSharp.SKPointMode.Polygon>, the entire array is drawn as a
    /// series of connected line segments.
    /// 
    /// Note that, while similar, the line and polygon modes draw slightly differently
    /// than the equivalent path built with a series of move to, line to calls, in
    /// that the path will draw all of its contours at once, with no interactions if
    /// contours intersect each other (think <xref:SkiaSharp.SKBlendMode.Xor>).
    /// ]]></format>
    ///             </remarks>
    void DrawPoints(PointMode mode, PointF[] points);

    /// <param name="p">The coordinates for the point to draw.</param>
    /// <summary>Draws a point in the canvas with the specified color.</summary>
    void DrawPoint(PointF p) => DrawPoint(p.X, p.Y);

    /// <param name="x">The x-coordinate for the point to draw.</param>
    /// <param name="y">The y-coordinate for the point to draw.</param>
    /// <summary>Draws a point in the canvas with the specified color.</summary>
    void DrawPoint(float x, float y);

    // /// <param name="image">The image to draw.</param>
    // /// <param name="p">The destination coordinates for the image.</param>
    // /// <param name="paint">The paint to use when drawing the image.</param>
    // /// <summary>Draws an image on the canvas.</summary>
    // void DrawImage(SKImage image, PointF p, SKPaint? paint = null) => Canvas.DrawImage(image, p, paint);
    //
    // /// <param name="image">The image to draw.</param>
    // /// <param name="x">The destination x-coordinate for the image.</param>
    // /// <param name="y">The destination y-coordinate for the image.</param>
    // /// <param name="paint">The paint to use when drawing the image.</param>
    // /// <summary>Draws an image on the canvas.</summary>
    // void DrawImage(SKImage image, int x, int y, SKPaint? paint = null) => Canvas.DrawImage(image, x, y, paint);
    //
    // /// <param name="image">The image to draw.</param>
    // /// <param name="dest">The region to draw the image into.</param>
    // /// <param name="paint">The paint to use when drawing the image.</param>
    // /// <summary>Draws an image on the canvas.</summary>
    // void DrawImage(SKImage image, RectangleF dest, SKPaint? paint = null) => Canvas.DrawImage(image, dest, paint);
    //
    // /// <param name="image">The image to draw.</param>
    // /// <param name="source">The source region to copy.</param>
    // /// <param name="dest">The region to draw the image into.</param>
    // /// <param name="paint">The paint to use when drawing the image.</param>
    // /// <summary>Draws an image on the canvas.</summary>
    // void DrawImage(SKImage image, RectangleF source, RectangleF dest, SKPaint? paint = null) => Canvas.DrawImage(image, source, dest, paint);
    //
    // /// <param name="picture">The picture to draw.</param>
    // /// <param name="x">The destination x-coordinate for the picture.</param>
    // /// <param name="y">The destination y-coordinate for the picture.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a picture on the canvas.</summary>
    // void DrawPicture(SKPicture picture, int x, int y, SKPaint? paint = null) => Canvas.DrawPicture(picture, x, y, paint);
    //
    // /// <param name="picture">The picture to draw.</param>
    // /// <param name="p">The destination coordinates for the picture.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a picture on the canvas.</summary>
    // void DrawPicture(SKPicture picture, PointF p, SKPaint? paint = null) => Canvas.DrawPicture(picture, p, paint);
    //
    // /// <param name="picture">The picture to draw.</param>
    // /// <param name="matrix">The matrix to apply while painting.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a picture on the canvas.</summary>
    // /// <remarks>
    // /// This is equivalent to calling <see cref="SKCanvas.Save"/>, followed by
    // /// <see cref="SKCanvas.Concat"/> specified `matrix`,
    // /// <see cref="SKCanvas.DrawPicture(SkiaSharp.SKPicture,SkiaSharp.SKPaint)"/>
    // /// and then <see cref="SKCanvas.Restore"/>.
    // /// 
    // /// If paint is non-null, the picture is drawn into a temporary buffer, and then
    // /// the paint's alpha, color filter, image filter, blend mode are applied to that
    // /// buffer as it is drawn to the canvas.
    // /// </remarks>
    // void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint? paint = null) => Canvas.DrawPicture(picture, ref matrix, paint);
    //
    // /// <param name="picture">The picture to draw.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a picture on the canvas.</summary>
    // void DrawPicture(SKPicture picture, SKPaint? paint = null) => Canvas.DrawPicture(picture, paint);
    //
    // /// <param name="drawable">The drawable to draw.</param>
    // /// <param name="matrix">The matrix to apply while painting.</param>
    // /// <summary>Draws a drawable on the canvas.</summary>
    // void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix) => Canvas.DrawDrawable(drawable, ref matrix);
    //
    // /// <param name="drawable">The drawable to draw.</param>
    // /// <param name="x">The destination x-coordinate for the drawable.</param>
    // /// <param name="y">The destination y-coordinate for the drawable.</param>
    // /// <summary>Draws a drawable on the canvas.</summary>
    // void DrawDrawable(SKDrawable drawable, int x, int y) => Canvas.DrawDrawable(drawable, x, y);
    //
    // /// <param name="drawable">The drawable to draw.</param>
    // /// <param name="p">The destination coordinates for the drawable.</param>
    // /// <summary>Draws a drawable on the canvas.</summary>
    // void DrawDrawable(SKDrawable drawable, PointF p) => Canvas.DrawDrawable(drawable, p);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="p">The destination coordinates for the bitmap.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, PointF p, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, p, paint);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="x">The destination x-coordinate for the bitmap.</param>
    // /// <param name="y">The destination y-coordinate for the bitmap.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, int x, int y, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, x, y, paint);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="dest">The region to draw the bitmap into.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, RectangleF dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, dest, paint);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="dest">The region to draw the bitmap into.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, Rectangle dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, dest.ToSKRect(), paint);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="source">The source region to copy.</param>
    // /// <param name="dest">The region to draw the bitmap into.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, RectangleF source, RectangleF dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, source, dest, paint);
    //
    // /// <param name="bitmap">The bitmap to draw.</param>
    // /// <param name="source">The source region to copy.</param>
    // /// <param name="dest">The region to draw the bitmap into.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a bitmap on the canvas.</summary>
    // void DrawBitmap(SKBitmap bitmap, Rectangle source, Rectangle dest, SKPaint? paint = null) => Canvas.DrawBitmap(bitmap, source.ToSKRect(), dest.ToSKRect(), paint);
    //
    // /// <param name="surface">The surface to draw.</param>
    // /// <param name="p">The destination coordinates for the surface.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a surface on the canvas.</summary>
    // void DrawSurface(SKSurface surface, PointF p, SKPaint? paint = null) => Canvas.DrawSurface(surface, p, paint);
    //
    // /// <param name="surface">The surface to draw.</param>
    // /// <param name="x">The destination x-coordinate for the surface.</param>
    // /// <param name="y">The destination y-coordinate for the surface.</param>
    // /// <param name="paint">The paint to use when drawing the picture.</param>
    // /// <summary>Draws a surface on the canvas.</summary>
    // void DrawSurface(SKSurface surface, int x, int y, SKPaint? paint = null) => Canvas.DrawSurface(surface, x, y, paint);
    //
    // /// <param name="text">The text blob to draw.</param>
    // /// <param name="x">The x-coordinate of the origin of the text being drawn.</param>
    // /// <param name="y">The y-coordinate of the origin of the text being drawn.</param>
    // /// <summary>Draws a text blob on the canvas at the specified coordinates.</summary>
    // void DrawString(SKTextBlob text, int x, int y) => Canvas.DrawText(text, x, y, _fillPaint);

    /// <param name="text">The text to draw.</param>
    /// <param name="p">The coordinates of the origin of the text being drawn.</param>
    /// <summary>Draws text on the canvas at the specified coordinates.</summary>
    void DrawString(string text, PointF p) => DrawString(text, p.X, p.Y);

    /// <param name="text">The text to draw.</param>
    /// <param name="x">The x-coordinate of the origin of the text being drawn.</param>
    /// <param name="y">The y-coordinate of the origin of the text being drawn.</param>
    /// <summary>Draws text on the canvas at the specified coordinates.</summary>
    void DrawString(string text, float x, float y);

    // /// <param name="text">The text to draw.</param>
    // /// <param name="x">The x-coordinate of the origin of the text being drawn.</param>
    // /// <param name="y">The y-coordinate of the origin of the text being drawn.</param>
    // /// <param name="font">The font to draw the text with.</param>
    // /// <summary>Draws text on the canvas at the specified coordinates.</summary>
    // void DrawString(string text, int x, int y, SKFont font) => Canvas.DrawText(text, x, y, font, _fillPaint);

    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="w">The rectangle width.</param>
    /// <param name="h">The rectangle height.</param>
    /// <summary>Draws a rectangle in the canvas.</summary>
    void FillRect(float x, float y, float w, float h);

    /// <param name="rect">The rectangle to draw.</param>
    /// <summary>Draws a rectangle in the canvas.</summary>
    void FillRect(RectangleF rect) => FillRect(rect.X, rect.Y, rect.Width, rect.Height);

    /// <param name="rect">The rounded rectangle to draw.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    /// <remarks>The paint to use when drawing the rounded rectangle.</remarks>
    void FillRoundRect(RoundRectangleF rect);

    /// <param name="x">The x-coordinate of the rectangle.</param>
    /// <param name="y">The y-coordinate of the rectangle.</param>
    /// <param name="w">The rectangle width.</param>
    /// <param name="h">The rectangle height.</param>
    /// <param name="rx">The x-radius of the oval used to round the corners.</param>
    /// <param name="ry">The y-radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    void FillRoundRect(float x, float y, float w, float h, float rx, float ry) => FillRoundRect(new RoundRectangleF(x, y, w, h, rx, ry));

    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="rx">The x-radius of the oval used to round the corners.</param>
    /// <param name="ry">The y-radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    void FillRoundRect(RectangleF rect, float rx, float ry) => FillRoundRect(new RoundRectangleF(rect, rx, ry));

    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="r">The radius of the oval used to round the corners.</param>
    /// <summary>Draws a rounded rectangle in the canvas.</summary>
    /// <remarks>The paint to use when drawing the rectangle.</remarks>
    void FillRoundRect(RectangleF rect, SizeF r) => FillRoundRect(rect.X, rect.Y, rect.Width, rect.Height, r.Width, r.Height);

    /// <param name="cx">The center x-coordinate.</param>
    /// <param name="cy">The center y-coordinate.</param>
    /// <param name="rx">The vertical radius for the oval.</param>
    /// <param name="ry">The horizontal radius for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void FillOval(float cx, float cy, float rx, float ry) => FillRoundRect(RoundRectangleF.Oval(cx, cy, rx, ry));

    /// <param name="c">The center coordinates.</param>
    /// <param name="r">The radius for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void FillOval(PointF c, SizeF r) => FillOval(c.X, c.Y, r.Width, r.Height);

    /// <param name="rect">The bounding box for the oval.</param>
    /// <summary>Draws an oval on the canvas.</summary>
    void FillOval(RectangleF rect) => FillOval(rect.X, rect.Y, rect.Width, rect.Height);

    /// <param name="cx">The center x-coordinate.</param>
    /// <param name="cy">The center y-coordinate.</param>
    /// <param name="radius">The radius for the circle.</param>
    /// <summary>Draws a circle on the canvas.</summary>
    void FillCircle(float cx, float cy, float radius) => FillOval(cx, cy, radius, radius);

    /// <param name="c">The center coordinates.</param>
    /// <param name="radius">The radius for the circle.</param>
    /// <summary>Draws a circle on the canvas.</summary>
    void FillCircle(PointF c, float radius) => FillCircle(c.X, c.Y, radius);

    // /// <param name="path">The path to draw.</param>
    // /// <summary>Draws a path in the canvas.</summary>
    // void FillPath(SKPath path) => Canvas.DrawPath(path, _fillPaint);
    
    /// <summary>
    /// Draw the specified text on the canvas, respecting line breaks.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="addSpacing">Additional spacing to add between each line.</param>
    void DrawMultilineString(string text, float x, float y, float addSpacing = 0);

    // /// <summary>
    // /// Draws a bitmap on the canvas, centered on an XY coordinate.
    // /// </summary>
    // /// <param name="bitmap">The bitmap to draw</param>
    // /// <param name="x">The origin X coordinate</param>
    // /// <param name="y">The origin Y coordinate</param>
    // void DrawBitmapCentered(SKBitmap bitmap, float x, float y)
    // {
    //     Canvas.DrawBitmap(bitmap, x - (bitmap.Width / 2f), y - (bitmap.Height / 2f));
    // }

    Stream SaveToStream(ImageFormat format = ImageFormat.Png, int quality = 100);

    void IDisposable.Dispose()
    {
    }
}