using System.Runtime.CompilerServices;
using SkiaSharp;

namespace Poki.Shared;

public readonly ref struct IndexablePixmap
{
    private readonly ReadOnlySpan<SKColor> _pixels;
    private readonly int _width;
    private readonly int _height;

    public SKColor this[int x, int y]
    {
        get
        {
            var width = _width;
            if (x < 0 || x > width || y < 0 || y > _height)
            {
                ThrowArgumentOutOfRange();
            }
            
            return _pixels[y * width + x];
        }
    }

    public int Width => _width;
    public int Height => _height;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentOutOfRange()
    {
        throw new ArgumentOutOfRangeException();
    }

    public IndexablePixmap(SKBitmap bitmap)
    {
        _pixels = bitmap.Pixels;
        _width = bitmap.Width;
        _height = bitmap.Height;
    }
    
    private static bool IsTransparent(SKColor color)
    {
        return color.Alpha == 0;
    }

    public bool IsRowEmpty(int row)
    {
        for (var x = 0; x < Width; ++x)
        {
            if (!IsTransparent(this[x, row])) return false;
        }

        return true;
    }

    public bool IsColumnEmpty(int col)
    {
        for (var y = 0; y < Height; ++y)
        {
            if (!IsTransparent(this[col, y])) return false;
        }

        return true;
    }
}

public static class BitmapUtils
{
    public static SKBitmap TrimWhitespace(this SKBitmap bitmap, bool left = true, bool top = true, bool right = true, bool bottom = true)
    {
        var croppedRectNullable = GetTrimmedRect(bitmap);
        if (croppedRectNullable == null)
        {
            return bitmap;
        }

        var croppedRect = croppedRectNullable.Value;

        return bitmap.Crop(new SKRect(
            left ? croppedRect.Left : 0,
            top ? croppedRect.Top : 0,
            right ? croppedRect.Right : bitmap.Width,
            bottom ? croppedRect.Bottom : bitmap.Height
        ));
    }

    public static SKBitmap Crop(this SKBitmap bitmap, SKRect croppedRect)
    {
        var target = new SKBitmap((int)croppedRect.Width, (int)croppedRect.Height);

        using var canvas = new SKCanvas(target);
        using var img = SKImage.FromBitmap(bitmap);
        canvas.DrawImage(img,
            croppedRect,
            new SKRect(0, 0, croppedRect.Width, croppedRect.Height));

        return target;
    }

    public static bool CanCropTo(this SKBitmap bitmap, SKRect targetRect)
    {
        if (!bitmap.GetBounds().Contains(targetRect))
        {
            return false;
        }
        
        var w = bitmap.Width;
        var h = bitmap.Height;

        var pixels = new IndexablePixmap(bitmap);

        for (var col = 0; col < targetRect.Left; col++)
        {
            if (!pixels.IsColumnEmpty(col)) return false;
        }

        for (var col = (int)targetRect.Right + 1; col < w; col++)
        {
            if (!pixels.IsColumnEmpty(col)) return false;
        }
        
        for (var row = 0; row < targetRect.Top; row++)
        {
            if (!pixels.IsRowEmpty(row)) return false;
        }

        for (var row = (int)targetRect.Bottom + 1; row < h; row++)
        {
            if (!pixels.IsRowEmpty(row)) return false;
        }

        return true;
    }

    private static SKRect GetBounds(this SKBitmap bitmap)
    {
        return new SKRect(0, 0, bitmap.Width, bitmap.Height);
    }

    public static SKRect? GetTrimmedRect(this SKBitmap bitmap)
    {
        var w = bitmap.Width;
        var h = bitmap.Height;

        var pixels = new IndexablePixmap(bitmap);

        var leftMost = 0;
        for (var col = 0; col < w; col++)
        {
            if (pixels.IsColumnEmpty(col)) leftMost = col + 1;
            else break;
        }

        var rightMost = w - 1;
        for (var col = rightMost; col > 0; col--)
        {
            if (pixels.IsColumnEmpty(col)) rightMost = col;
            else break;
        }

        var topMost = 0;
        for (var row = 0; row < h; row++)
        {
            if (pixels.IsRowEmpty(row)) topMost = row + 1;
            else break;
        }

        var bottomMost = h - 1;
        for (var row = bottomMost; row > 0; row--)
        {
            if (pixels.IsRowEmpty(row)) bottomMost = row;
            else break;
        }

        if (rightMost == 0 && bottomMost == 0 && leftMost == w && topMost == h)
        {
            return null;
        }

        return new SKRect(leftMost, topMost, rightMost, bottomMost);
    }
}