using ParallelGraphicProcessing.Extensions;
using SkiaSharp;
using System.Diagnostics;

namespace ParallelGraphicProcessing
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap? originalBitmap;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isProcessing = false;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void PickImageBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Vyberte obrázek",
                    FileTypes = FilePickerFileType.Images // Pouze obrázky
                });
                if (result != null)
                {
                    using (var stream = result.OpenReadAsync().Result)
                    {
                        originalBitmap = SKBitmap.Decode(stream);
                        MyImage.Source = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                    };

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Chyba", ex.Message, "OK");
            }
        }
        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            if (_isProcessing && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
        private void RestoreBtn_Clicked(object sender, EventArgs e)
        {
            if (originalBitmap == null)
            {
                return;
            }
            MyImage.Source = originalBitmap.ToImageSource();
        }

        private unsafe void RedFilterBtn_Clicked(object sender, EventArgs e)
        {
            if (originalBitmap == null)
            {
                DisplayAlert("Chyba", "Obrázek nebyl načten.", "OK");
                return;
            }
            if (_isProcessing)
            {
                DisplayAlert("Chyba", "Operace již probíhá.", "OK");
                return;
            }
            _isProcessing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MyActivityIndicator.IsVisible = true;
            MyActivityIndicator.IsRunning = true;
            CancelBtn.IsEnabled = true;
            try
            {
                var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
                using (var srcPixmap = originalBitmap.PeekPixels())
                using (var dstPixmap = filteredBitmap.PeekPixels())
                {
                    IntPtr srcPtr = srcPixmap.GetPixels();
                    IntPtr dstPtr = dstPixmap.GetPixels();
                    int rowBytes = srcPixmap.RowBytes;
                    int height = originalBitmap.Height;
                    int width = originalBitmap.Width;
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        IntPtr srcRow = srcPtr + y * rowBytes;
                        IntPtr dstRow = dstPtr + y * rowBytes;
                        byte* srcPixel = (byte*)srcRow;
                        byte* dstPixel = (byte*)dstRow;
                        for (int x = 0; x < originalBitmap.Width; x++)
                        {
                            byte red = srcPixel[2];  // R složka
                            byte alpha = srcPixel[3]; // A složka

                            dstPixel[0] = 0;    // B složka
                            dstPixel[1] = 0;    // G složka
                            dstPixel[2] = red;  // R složka
                            dstPixel[3] = alpha; // A složka

                            // Posunutí na další pixel (4 byty na pixel)
                            srcPixel += 4;
                            dstPixel += 4;
                        }
                    }
                }
                MyImage.Source = filteredBitmap.ToImageSource();
            }
            catch (Exception ex)
            {
                DisplayAlert("Chyba", ex.Message, "OK");
            }
            finally
            {
                MyActivityIndicator.IsVisible = false;
                MyActivityIndicator.IsRunning = false;
                CancelBtn.IsEnabled = false;
                _isProcessing = false;
                stopwatch.Stop();
                DisplayAlert("Hotovo", $"Operace trvala {stopwatch.ElapsedMilliseconds} ms.", "OK");
            }
        }

        private async void RedFilterAsyncBtn_Clicked(object sender, EventArgs e)
        {
            if (originalBitmap == null)
            {
                await DisplayAlert("Chyba", "Obrázek nebyl načten.", "OK");
                return;
            }
            if (_isProcessing)
            {
                await DisplayAlert("Chyba", "Operace již probíhá.", "OK");
                return;
            }

            _isProcessing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CancelBtn.IsEnabled = true;
            MyActivityIndicator.IsVisible = true;
            MyActivityIndicator.IsRunning = true;

            try
            {
                var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

                await Task.Run(() =>
                {
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        for (int x = 0; x < originalBitmap.Width; x++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }

                            SKColor color = originalBitmap.GetPixel(x, y);
                            var redColor = new SKColor(color.Red, 0, 0, color.Alpha);
                            filteredBitmap.SetPixel(x, y, redColor);
                        }
                    }
                }, token);
                MyImage.Source = filteredBitmap.ToImageSource();
            }
            catch (OperationCanceledException)
            {
                await DisplayAlert("Zrušeno", "Operace byla zrušena.", "OK");
            }
            finally
            {
                _isProcessing = false;
                MyActivityIndicator.IsRunning = false;
                MyActivityIndicator.IsVisible = false;
                CancelBtn.IsEnabled = false;
                stopwatch.Stop();
                await DisplayAlert("Hotovo", $"Operace trvala {stopwatch.ElapsedMilliseconds} ms.", "OK");
            }
        }

        private unsafe void RedFilterParallelBtn_Clicked(object sender, EventArgs e)
        {
            
        }

        private unsafe void RedFilterUnsafeBtn_Clicked(object sender, EventArgs e)
        {
            if (originalBitmap == null)
            {
                DisplayAlert("Chyba", "Obrázek nebyl načten.", "OK");
                return;
            }
            if (_isProcessing)
            {
                DisplayAlert("Chyba", "Operace již probíhá.", "OK");
                return;
            }
            _isProcessing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MyActivityIndicator.IsVisible = true;
            MyActivityIndicator.IsRunning = true;
            try
            {
                var filteredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
                using (var srcPixmap = originalBitmap.PeekPixels())
                using (var dstPixmap = filteredBitmap.PeekPixels())
                {
                    IntPtr srcPtr = srcPixmap.GetPixels();
                    IntPtr dstPtr = dstPixmap.GetPixels();
                    int rowBytes = srcPixmap.RowBytes;
                    int height = originalBitmap.Height;
                    int width = originalBitmap.Width;
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        IntPtr srcRow = srcPtr + y * rowBytes;
                        IntPtr dstRow = dstPtr + y * rowBytes;
                        byte* srcPixel = (byte*)srcRow;
                        byte* dstPixel = (byte*)dstRow;
                        for (int x = 0; x < originalBitmap.Width; x++)
                        {
                            byte red = srcPixel[2];  // R složka
                            byte alpha = srcPixel[3]; // A složka

                            dstPixel[0] = 0;    // B složka
                            dstPixel[1] = 0;    // G složka
                            dstPixel[2] = red;  // R složka
                            dstPixel[3] = alpha; // A složka

                            // Posunutí na další pixel (4 byty na pixel)
                            srcPixel += 4;
                            dstPixel += 4;
                        }
                    }
                }
                MyImage.Source = filteredBitmap.ToImageSource();
            }
            catch (Exception ex)
            {
                DisplayAlert("Chyba", ex.Message, "OK");
            }
            finally
            {
                MyActivityIndicator.IsVisible = false;
                MyActivityIndicator.IsRunning = false;
                _isProcessing = false;
                stopwatch.Stop();
                DisplayAlert("Hotovo", $"Operace trvala {stopwatch.ElapsedMilliseconds} ms.", "OK");
            }
        }
    }
}
