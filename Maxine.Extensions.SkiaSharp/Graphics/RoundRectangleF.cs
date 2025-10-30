using System.Collections;
using System.Drawing;
using JetBrains.Annotations;
using SkiaSharp;

namespace Poki.Shared.Graphics;

/// <summary>
/// Represents a rounded rectangle.
/// </summary>
[PublicAPI]
public struct RoundRectangleF
{
    private RectangleF _rect;

    public RectangleF Rect
    {
        get => _rect;
        set => _rect = value;
    }

    public PointF UpperLeft { get; set; }
    public PointF UpperRight { get; set; }
    public PointF LowerLeft { get; set; }
    public PointF LowerRight { get; set; }

    public Enumerable Radii => new(this);

    public PointF SimpleRadii
    {
        get => UpperLeft;
        set
        {
            UpperLeft = value;
            UpperRight = value;
            LowerLeft = value;
            LowerRight = value;
        }
    }

    public float X
    {
        get => _rect.X;
        set => _rect.X = value;
    }

    public float Y
    {
        get => _rect.Y;
        set => _rect.Y = value;
    }

    public float Width
    {
        get => _rect.Width;
        set => _rect.Width = value;
    }

    public float Height
    {
        get => _rect.Height;
        set => _rect.Height = value;
    }

    public bool IsSimple => UpperLeft == UpperRight && UpperLeft == LowerLeft && UpperLeft == LowerRight;

    public struct Enumerable : IEnumerable<PointF>, IEnumerator<PointF>
    {
        private readonly RoundRectangleF _rect;
        private int _index;

        public IEnumerator<PointF> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal Enumerable(RoundRectangleF rect)
        {
            _rect = rect;
        }

        public bool MoveNext()
        {
            if (_index < 3)
            {
                _index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = 0;
        }

        public PointF Current => _index switch
        {
            0 => _rect.UpperLeft,
            1 => _rect.UpperRight,
            2 => _rect.LowerLeft,
            3 => _rect.LowerRight,
            _ => throw new ArgumentOutOfRangeException()
        };

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

    public RoundRectangleF(RectangleF rect, float xRadius, float yRadius)
    {
        _rect = rect;
        UpperLeft = UpperRight = LowerLeft = LowerRight = new PointF(xRadius, yRadius);
    }

    public RoundRectangleF(RectangleF rect, PointF radius)
    {
        _rect = rect;
        UpperLeft = UpperRight = LowerLeft = LowerRight = radius;
    }

    public RoundRectangleF(RectangleF rect, PointF upperLeft, PointF upperRight, PointF lowerLeft, PointF lowerRight)
    {
        _rect = rect;
        UpperLeft = upperLeft;
        UpperRight = upperRight;
        LowerLeft = lowerLeft;
        LowerRight = lowerRight;
    }

    public RoundRectangleF(RectangleF rect, float radius) : this(rect, radius, radius)
    {
    }

    public RoundRectangleF(RectangleF rect) : this(rect, 0, 0)
    {
    }

    public RoundRectangleF(float x, float y, float width, float height, float radiusX, float radiusY) : this(new RectangleF(x, y, width, height), radiusX, radiusY)
    {
    }

    public static RoundRectangleF Oval(RectangleF rect)
    {
        return new RoundRectangleF(rect, rect.Width / 2, rect.Height / 2);
    }

    public static RoundRectangleF Oval(float cx, float cy, float rx, float ry)
    {
        return Oval(new RectangleF(cx - rx, cy - ry, rx * 2, ry * 2));
    }
    
    /// <summary>Gets a value indicating whether all four corners are circular (with the x- and y-axis equal).</summary>
    /// <value />
    /// <remarks />
    public bool AllCornersCircular => CheckAllCornersCircular(0.00024414062f);

    /// <param name="tolerance">The difference in the axis allowed before the corners are no longer circular.</param>
    /// <summary>Check to see whether all four corners are circular (with the x- and y-axis equal).</summary>
    /// <returns />
    /// <remarks />
    public bool CheckAllCornersCircular(float tolerance)
    {
        var radii1 = UpperLeft;
        var radii2 = UpperRight;
        var radii3 = LowerRight;
        var radii4 = LowerLeft;
        return NearlyEqual(radii1.X, radii1.Y, tolerance) && NearlyEqual(radii2.X, radii2.Y, tolerance) && NearlyEqual(radii3.X, radii3.Y, tolerance) && NearlyEqual(radii4.X, radii4.Y, tolerance);
    }

    private static bool NearlyEqual(float a, float b, float tolerance) => (double) Math.Abs(a - b) <= tolerance;

    /// <param name="pos">The amount to translate the rectangle by.</param>
    /// <summary>Translate the rectangle by the specified amount.</summary>
    public void Offset(SKPoint pos) => Offset(pos.X, pos.Y);

    /// <param name="dx">The amount to translate the rectangle by along the x-axis.</param>
    /// <param name="dy">The amount to translate the rectangle by along the y-axis.</param>
    /// <summary>Translate the rectangle by the specified amount.</summary>
    public void Offset(float dx, float dy)
    {
        _rect.X += dx;
        _rect.Y += dy;
    }
}