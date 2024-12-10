using SkiaSharp;

namespace ParallelGraphicProcessing.Extensions;

public class AsyncProcessing {
    public static async Task<SKBitmap> Filter(SKBitmap? originalBitmap, CancellationToken token) {
        ArgumentNullException.ThrowIfNull(originalBitmap);

        var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

        await Task.Run(() => {
            for (int y = 0; y < originalBitmap.Height; y++) {
                for (int x = 0; x < originalBitmap.Width; x++) {
                    if (token.IsCancellationRequested) {
                        token.ThrowIfCancellationRequested();
                    }

                    SKColor color = originalBitmap.GetPixel(x, y);
                    var redColor = new SKColor(color.Red, 0, 0, color.Alpha);
                    filteredBitmap.SetPixel(x, y, redColor);
                }
            }
        }, token);

        return filteredBitmap;
    }
}