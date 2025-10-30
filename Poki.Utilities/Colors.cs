using System.Drawing;
using System.Runtime.CompilerServices;

namespace Poki.Utilities;

public static class Colors
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Get(int rgb)
    {
        return Color.FromArgb(unchecked((int)(0xFF000000U | rgb)));
    }
}