using ParallelGraphicProcessing.Extensions;
using SkiaSharp;
using System.Diagnostics;
using Exception = System.Exception;

namespace ParallelGraphicProcessing {
    public partial class MainPage : ContentPage {
        private SKBitmap? _originalBitmap;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isProcessing = false;

        public MainPage() {
            InitializeComponent();
        }

        private async void PickImageBtn_Clicked(object sender, EventArgs e) {
            try {
                var result = await FilePicker.Default.PickAsync(new PickOptions {
                    PickerTitle = "Vyberte obrázek",
                    FileTypes = FilePickerFileType.Images
                });
                if (result != null) {
                    using (var stream = result.OpenReadAsync().Result) {
                        _originalBitmap = SKBitmap.Decode(stream);
                        MyImage.Source = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                    }
                }
            } catch (Exception ex) {
                await DisplayAlert("Chyba", ex.Message, "OK");
            }
        }

        private void CancelBtn_Clicked(object sender, EventArgs e) {
            if (_isProcessing && _cancellationTokenSource != null) {
                _cancellationTokenSource.Cancel();
            }
        }

        private void RestoreBtn_Clicked(object sender, EventArgs e) {
            if (_originalBitmap == null) {
                return;
            }

            MyImage.Source = _originalBitmap.ToImageSource();
        }

        private CancellationToken? StartProcessing(SKBitmap? bitmap, bool processing) {
            if (bitmap == null) {
                DisplayAlert("Chyba", "Obrázek nebyl načten.", "OK");
                return null;
            }

            if (processing) {
                DisplayAlert("Chyba", "Operace již probíhá.", "OK");
                return null;
            }

            _isProcessing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MyActivityIndicator.IsVisible = true;
            MyActivityIndicator.IsRunning = true;
            CancelBtn.IsEnabled = true;

            return token;
        }

        private void StopProcessing() {
            MyActivityIndicator.IsVisible = false;
            MyActivityIndicator.IsRunning = false;
            CancelBtn.IsEnabled = false;
            _isProcessing = false;
        }

        private async void FilterButtonClicked(object? sender, EventArgs e) {
            if (sender is Button button && button.CommandParameter is string selectedFilter) {
                var token = StartProcessing(_originalBitmap, _isProcessing);

                if (token == null) {
                    StopProcessing();
                    return;
                }

                try {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var filteredBitmap = new SKBitmap();

                    switch (selectedFilter) {
                        case "SYNC":
                            filteredBitmap = SynchronousProcessing.Filter(_originalBitmap);
                            break;
                        case "SYNC_UNSAFE":
                            filteredBitmap = SynchronousProcessing.FilterUnsafe(_originalBitmap);
                            break;
                        case "ASYNC":
                            filteredBitmap = await AsyncProcessing.Filter(_originalBitmap, token.Value);
                            break;
                        case "PARALLEL":
                            filteredBitmap = await ParallelProcessing.Filter(_originalBitmap, token.Value);
                            break;
                        case "PARALLEL_UNSAFE":
                            filteredBitmap = await ParallelProcessing.FilterUnsafe(_originalBitmap, token.Value);
                            break;
                        case "EDGE_DETECTION":
                            filteredBitmap = EdgeDetection.DetectEdges(_originalBitmap, token.Value);
                            break;
                        case "SEPIA_EFFECT":
                            filteredBitmap = SepiaProcessing.SepiaEffect(_originalBitmap);
                            break;
                    }

                    stopwatch.Stop();
                    await DisplayAlert("Hotovo", $"Operace trvala {stopwatch?.ElapsedMilliseconds} ms.", "OK");
                    MyImage.Source = filteredBitmap.ToImageSource();
                } catch (Exception ex) {
                    await DisplayAlert("Chyba", ex.Message, "OK");
                } finally {
                    StopProcessing();
                }
            }
        }
    }
}