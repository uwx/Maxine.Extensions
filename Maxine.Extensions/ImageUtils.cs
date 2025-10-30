using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace Maxine.Extensions;

#pragma warning disable CA1416

public class ImageUtils
{
	[SupportedOSPlatform("windows")]
    public static void GetAlpha(string imageFile, out bool cutout, out bool trans)
    {
	    using var bmp = (Bitmap)Image.FromFile(imageFile);
	    // ReSharper disable BitwiseOperatorOnEnumWithoutFlags

	    cutout = false;
	    trans = false;

	    if ((bmp.PixelFormat & PixelFormat.Alpha) != 0 || (bmp.PixelFormat & PixelFormat.PAlpha) != 0)
	    {
		    var locked = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

		    try
		    {
			    GetHasAlpha(locked, out cutout, out trans);
		    }
		    finally
		    {
			    bmp.UnlockBits(locked);
		    }
	    }
    }

	[SupportedOSPlatform("windows")]
    private static unsafe void GetHasAlpha(BitmapData locked, out bool mask, out bool trans)
	{
		mask = false;
		trans = false;
	                            
		var depthBytes = Image.GetPixelFormatSize(locked.PixelFormat) / 8;

		var ptr = (byte*)locked.Scan0;
		// var span = new Span<byte>(ptr, locked.Stride * locked.Height * depthBytes);
	                            
		for (var y = 0; y < locked.Height; ++y)
		{
			for (var x = 0; x < locked.Width; ++x)
			{
				var data = ptr + y * locked.Stride + x * depthBytes;

				var a = locked.PixelFormat switch
				{
					PixelFormat.Format24bppRgb => byte.MaxValue,
					PixelFormat.Format32bppRgb => byte.MaxValue,
					PixelFormat.Format32bppArgb => data[3],
					PixelFormat.Format32bppPArgb => data[3],
					PixelFormat.Format48bppRgb => byte.MaxValue,
					PixelFormat.Format64bppArgb => (byte)((((ushort*)data)[3] / (double)ushort.MaxValue) * byte.MaxValue),
					PixelFormat.Format64bppPArgb => (byte)((((ushort*)data)[3] / (double)ushort.MaxValue) * byte.MaxValue),
					_ => throw new InvalidOperationException() //byte.MaxValue
				};

				if (a != byte.MaxValue)
				{
					if (a == 0)
					{
						mask = true;
					}
					else
					{
						trans = true;
						return;
					}
				}

				//data is a pointer to the first byte of the 3-byte color data
				//data[0] = blueComponent;
				//data[1] = greenComponent;
				//data[2] = redComponent;
			}
		}
	}
}