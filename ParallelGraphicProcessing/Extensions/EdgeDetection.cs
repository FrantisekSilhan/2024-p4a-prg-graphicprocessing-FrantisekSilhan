using SkiaSharp;
using System.Numerics;

namespace ParallelGraphicProcessing.Extensions;

public static class EdgeDetection {
    private static readonly float[] GrayscaleFactors = { 0.299f, 0.587f, 0.114f };
    private static readonly float[,] SobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
    private static readonly float[,] SobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

    public static SKBitmap DetectEdges(SKBitmap? input, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(input);

        var width = input.Width;
        var height = input.Height;

        using var grayscale = new FloatArray2D(width, height);
        using var gradientX = new FloatArray2D(width, height);
        using var gradientY = new FloatArray2D(width, height);

        ConvertToGrayscaleParallel(input, grayscale, cancellationToken);
        ApplySobelOperatorsParallel(grayscale, gradientX, gradientY, cancellationToken);
        return CreateOutputBitmapParallel(width, height, gradientX, gradientY, cancellationToken);
    }

    private static unsafe void ConvertToGrayscaleParallel(SKBitmap input, FloatArray2D grayscale,
                                                          CancellationToken token) {
        var info = input.Info;
        var pixels = input.GetPixels();
        Parallel.For(0, info.Height, new ParallelOptions { CancellationToken = token }, y => {
            byte* row = (byte*)pixels.ToPointer() + y * info.RowBytes;
            for (int x = 0; x < info.Width; x++) {
                int i = x * 4;
                grayscale[x, y] = Vector3.Dot(
                    new Vector3(row[i + 2], row[i + 1], row[i]),
                    new Vector3(GrayscaleFactors[0], GrayscaleFactors[1], GrayscaleFactors[2]));
            }
        });
    }

    private static void ApplySobelOperatorsParallel(FloatArray2D grayscale, FloatArray2D gradientX,
                                                    FloatArray2D gradientY, CancellationToken token) {
        int height = grayscale.Height - 1;
        int width = grayscale.Width - 1;

        Parallel.For(1, height, new ParallelOptions { CancellationToken = token }, y => {
            for (int x = 1; x < width; x++) {
                (gradientX[x, y], gradientY[x, y]) = ComputeGradients(grayscale, x, y);
            }
        });
    }

    private static (float gx, float gy) ComputeGradients(FloatArray2D grayscale, int x, int y) {
        float gx = 0, gy = 0;

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                float pixel = grayscale[x + i, y + j];
                gx += pixel * SobelX[i + 1, j + 1];
                gy += pixel * SobelY[i + 1, j + 1];
            }
        }

        return (gx, gy);
    }

    private static unsafe SKBitmap CreateOutputBitmapParallel(int width, int height, FloatArray2D gradientX,
                                                              FloatArray2D gradientY, CancellationToken token) {
        var output = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        float maxGradient = ComputeMaxGradient(gradientX, gradientY, token);
        var pixels = output.GetPixels();

        Parallel.For(0, height, new ParallelOptions { CancellationToken = token }, y => {
            byte* row = (byte*)pixels.ToPointer() + y * output.RowBytes;
            for (int x = 0; x < width; x++) {
                float magnitude = MathF.Sqrt(
                    gradientX[x, y] * gradientX[x, y] +
                    gradientY[x, y] * gradientY[x, y]);

                byte intensity = (byte)(magnitude / maxGradient * 255);
                int i = x * 4;
                row[i] = row[i + 1] = row[i + 2] = intensity;
                row[i + 3] = 255;
            }
        });

        return output;
    }

    private static float ComputeMaxGradient(FloatArray2D gradientX, FloatArray2D gradientY, CancellationToken token) {
        float maxGradient = 0;
        object lockObj = new object();

        Parallel.For(1, gradientX.Height - 1, new ParallelOptions { CancellationToken = token }, y => {
            float localMax = 0;
            for (int x = 1; x < gradientX.Width - 1; x++) {
                float magnitude = MathF.Sqrt(
                    gradientX[x, y] * gradientX[x, y] +
                    gradientY[x, y] * gradientY[x, y]);
                localMax = Math.Max(localMax, magnitude);
            }

            lock (lockObj) {
                maxGradient = Math.Max(maxGradient, localMax);
            }
        });

        return maxGradient;
    }
}

public sealed class FloatArray2D : IDisposable {
    private readonly float[,] _data;
    public int Width { get; }
    public int Height { get; }

    public FloatArray2D(int width, int height) {
        Width = width;
        Height = height;
        _data = new float[width, height];
    }

    public float this[int x, int y] {
        get => _data[x, y];
        set => _data[x, y] = value;
    }

    public void Dispose() {
    }
}