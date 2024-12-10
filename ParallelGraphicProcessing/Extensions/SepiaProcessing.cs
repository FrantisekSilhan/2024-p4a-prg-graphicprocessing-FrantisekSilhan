using SkiaSharp;
namespace ParallelGraphicProcessing.Extensions;
public class SepiaProcessing {
    public static SKBitmap SepiaEffect(SKBitmap? sourceBitmap) {
        ArgumentNullException.ThrowIfNull(sourceBitmap);
        SKBitmap sepiaBitmap = new SKBitmap(sourceBitmap.Width, sourceBitmap.Height);
        sourceBitmap.CopyTo(sepiaBitmap);
        IntPtr pixelsAddr = sepiaBitmap.GetPixels();
        int bytesPerPixel = 4;
        int rowBytes = sepiaBitmap.RowBytes;
        unsafe {
            byte* ptr = (byte*)pixelsAddr.ToPointer();
            for (int y = 0; y < sepiaBitmap.Height; y++) {
                for (int x = 0; x < sepiaBitmap.Width; x++) {
                    int offset = y * rowBytes + x * bytesPerPixel;
                    byte blue = ptr[offset];
                    byte green = ptr[offset + 1];
                    byte red = ptr[offset + 2];

                    byte newRed = (byte)Math.Min(255, (0.393 * red + 0.769 * green + 0.189 * blue));
                    byte newGreen = (byte)Math.Min(255, (0.349 * red + 0.686 * green + 0.168 * blue));
                    byte newBlue = (byte)Math.Min(255, (0.272 * red + 0.534 * green + 0.131 * blue));

                    ptr[offset] = newBlue;
                    ptr[offset + 1] = newGreen;
                    ptr[offset + 2] = newRed;
                    // ptr[offset + 3] is Alpha - leave unchanged
                }
            }
        }
        return sepiaBitmap;
    }
}