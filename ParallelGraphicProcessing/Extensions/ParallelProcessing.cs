using SkiaSharp;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ParallelGraphicProcessing.Extensions;

public class ParallelProcessing {
    private const uint RED_MASK = 0xFFFF0000;
    private const int SIMD_VECTOR_SIZE = 8;
    private const int CANCELLATION_CHECK_FREQUENCY = 1000;

    public static async Task<SKBitmap> Filter(SKBitmap? source, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(source);

        SKBitmap filteredBitmap = await Task.Run(() => {
            var result = new SKBitmap(source.Width, source.Height);

            using var sourcePixels = source.PeekPixels();
            using var resultPixels = result.PeekPixels();

            IntPtr sourcePtr = sourcePixels.GetPixels();
            IntPtr resultPtr = resultPixels.GetPixels();

            int totalPixels = source.Width * source.Height;
            const int bytesPerPixel = 4;
            const int chunkSize = 4096;

            Parallel.For(0, (totalPixels + chunkSize - 1) / chunkSize,
                new ParallelOptions { CancellationToken = cancellationToken },
                chunkIndex => {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    int startPixel = chunkIndex * chunkSize;
                    int endPixel = Math.Min(startPixel + chunkSize, totalPixels);

                    for (int i = startPixel; i < endPixel; i++) {
                        int byteOffset = i * bytesPerPixel;

                        Marshal.WriteByte(resultPtr + byteOffset, 0);
                        Marshal.WriteByte(resultPtr + byteOffset + 1, 0);
                        Marshal.WriteByte(resultPtr + byteOffset + 2,
                            Marshal.ReadByte(sourcePtr + byteOffset + 2));
                        Marshal.WriteByte(resultPtr + byteOffset + 3,
                            Marshal.ReadByte(sourcePtr + byteOffset + 3));
                    }
                }
            );

            return result;
        }, cancellationToken);

        return filteredBitmap;
    }

    public static async Task<SKBitmap> FilterUnsafe(SKBitmap? source, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(source);
        return await Task.Run(() => ApplyRedFilterInternal(source, cancellationToken), cancellationToken);
    }

    private static SKBitmap ApplyRedFilterInternal(SKBitmap source, CancellationToken cancellationToken) {
        var result = new SKBitmap(source.Width, source.Height);
        var totalPixels = source.Width * source.Height;

        unsafe {
            using var sourcePixels = source.PeekPixels();
            using var targetPixels = result.PeekPixels();

            var srcPtr = (uint*)sourcePixels.GetPixels();
            var dstPtr = (uint*)targetPixels.GetPixels();
            var maskVector = Vector256.Create(RED_MASK);

            int vectorCount = totalPixels / SIMD_VECTOR_SIZE;

            Parallel.For(0, vectorCount, new ParallelOptions {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, i => {
                if (i % CANCELLATION_CHECK_FREQUENCY == 0) {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var offset = i * SIMD_VECTOR_SIZE;
                var inputVector = Avx2.LoadVector256(srcPtr + offset);
                var resultVector = Avx2.And(inputVector, maskVector);
                Avx2.Store(dstPtr + offset, resultVector);
            });

            for (var i = vectorCount * SIMD_VECTOR_SIZE; i < totalPixels; i++) {
                dstPtr[i] = srcPtr[i] & RED_MASK;
            }
        }

        return result;
    }
}