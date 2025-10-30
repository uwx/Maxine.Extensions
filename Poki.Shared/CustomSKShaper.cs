using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Buffer = HarfBuzzSharp.Buffer;

namespace Poki.Shared;

// ReSharper disable once InconsistentNaming
public class CustomSKShaper : IDisposable
{
    internal const int FontSizeScale = 1 << 16; // formerly 512

    private readonly Font _font;
    private readonly Buffer _buffer;

    public CustomSKShaper(SKTypeface typeface)
    {
        Typeface = typeface ?? throw new ArgumentNullException(nameof(typeface));

        using (var blob = Typeface.OpenStream(out var index).ToHarfBuzzBlob())
        using (var face = new Face(blob, index))
        {
            face.Index = index;
            face.UnitsPerEm = Typeface.UnitsPerEm;

            _font = new Font(face);
            _font.SetScale(FontSizeScale, FontSizeScale);

            _font.SetFunctionsOpenType();
        }

        _buffer = new Buffer();
    }

    public SKTypeface Typeface { get; }

    public void Dispose()
    {
        _font.Dispose();
        _buffer.Dispose();
    }

    public Result Shape(Buffer buffer, SKPaint paint) =>
        Shape(buffer, 0, 0, paint);

    public Result Shape(Buffer buffer, float xOffset, float yOffset, SKPaint paint)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (paint == null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var xInitialOffset = xOffset;
        var yInitialOffset = yOffset;

        // do the shaping
        _font.Shape(buffer);

        // get the shaping results
        var len = buffer.Length;
        var info = buffer.GlyphInfos;
        var pos = buffer.GlyphPositions;

        // get the sizes
        var textSizeY = paint.TextSize / FontSizeScale;
        var textSizeX = textSizeY * paint.TextScaleX;

        var points = new SKPoint[len];
        var clusters = new uint[len];
        var codepoints = new uint[len];

        for (var i = 0; i < len; i++)
        {
            codepoints[i] = info[i].Codepoint;

            clusters[i] = info[i].Cluster;

            points[i] = new SKPoint(
                xOffset + pos[i].XOffset * textSizeX,
                yOffset - pos[i].YOffset * textSizeY);

            // move the cursor
            xOffset += pos[i].XAdvance * textSizeX;
            yOffset += pos[i].YAdvance * textSizeY;
        }

        return new Result(codepoints, clusters, points, xOffset - xInitialOffset, yOffset - yInitialOffset);
    }

    public Result Shape(string text, SKPaint paint) =>
        Shape(text, 0, 0, paint);

    public Result Shape(string text, float xOffset, float yOffset, SKPaint paint)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Result();
        }

        using (var buffer = new Buffer())
        {
            switch (paint.TextEncoding)
            {
                case SKTextEncoding.Utf8:
                    buffer.AddUtf8(text);
                    break;
                case SKTextEncoding.Utf16:
                    buffer.AddUtf16(text);
                    break;
                case SKTextEncoding.Utf32:
                    buffer.AddUtf32(text);
                    break;
                case SKTextEncoding.GlyphId:
                default:
                    throw new NotSupportedException("TextEncoding of type GlyphId is not supported.");
            }

            buffer.GuessSegmentProperties();

            return Shape(buffer, xOffset, yOffset, paint);
        }
    }

    public readonly record struct Result(
        uint[] Codepoints,
        uint[] Clusters,
        SKPoint[] Points,
        float Width,
        float Height
    )
    {
        public Result() : this(
            Array.Empty<uint>(),
            Array.Empty<uint>(),
            Array.Empty<SKPoint>(),
            0,
            0
        )
        {
        }
    }
}