using SkiaSharp;

namespace ParallelGraphicProcessing.Extensions;

public class SynchronousProcessing {
    public static unsafe SKBitmap Filter(SKBitmap? originalBitmap) {
        if (originalBitmap == null) return new SKBitmap();

        var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
        using (var srcPixmap = originalBitmap.PeekPixels())
        using (var dstPixmap = filteredBitmap.PeekPixels()) {
            IntPtr srcPtr = srcPixmap.GetPixels();
            IntPtr dstPtr = dstPixmap.GetPixels();
            int rowBytes = srcPixmap.RowBytes;
            for (int y = 0; y < originalBitmap.Height; y++) {
                IntPtr srcRow = srcPtr + y * rowBytes;
                IntPtr dstRow = dstPtr + y * rowBytes;
                byte* srcPixel = (byte*)srcRow;
                byte* dstPixel = (byte*)dstRow;
                for (int x = 0; x < originalBitmap.Width; x++) {
                    byte red = srcPixel[2];
                    byte alpha = srcPixel[3];

                    dstPixel[0] = 0;
                    dstPixel[1] = 0;
                    dstPixel[2] = red;
                    dstPixel[3] = alpha;

                    srcPixel += 4;
                    dstPixel += 4;
                }
            }
        }

        return filteredBitmap;
    }

    public static unsafe SKBitmap FilterUnsafe(SKBitmap? originalBitmap) {
        if (originalBitmap == null) return new SKBitmap();

        var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
        using (var srcPixmap = originalBitmap.PeekPixels())
        using (var dstPixmap = filteredBitmap.PeekPixels()) {
            IntPtr srcPtr = srcPixmap.GetPixels();
            IntPtr dstPtr = dstPixmap.GetPixels();
            int rowBytes = srcPixmap.RowBytes;
            int height = originalBitmap.Height;
            int width = originalBitmap.Width;
            for (int y = 0; y < originalBitmap.Height; y++) {
                IntPtr srcRow = srcPtr + y * rowBytes;
                IntPtr dstRow = dstPtr + y * rowBytes;
                byte* srcPixel = (byte*)srcRow;
                byte* dstPixel = (byte*)dstRow;
                for (int x = 0; x < originalBitmap.Width; x++) {
                    byte red = srcPixel[2];
                    byte alpha = srcPixel[3];

                    dstPixel[0] = 0;
                    dstPixel[1] = 0;
                    dstPixel[2] = red;
                    dstPixel[3] = alpha;

                    srcPixel += 4;
                    dstPixel += 4;
                }
            }
        }

        return filteredBitmap;
    }
}