using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.Mobile;

namespace Sample.Forms
{
    public partial class CustomContinuousScanXamlPage : ContentPage, INotifyPropertyChanged
    {
        public CustomContinuousScanXamlPage()
        {
            InitializeComponent();

            barcodeOptions = new MobileBarcodeScanningOptions
            {
                /*
                PossibleFormats = new[] {
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.UPC_A,
                    BarcodeFormat.CODE_39
                },
                */
                //AutoRotate = true,
                DisableAutofocus = false,
                // UseNativeScanning = true
                TryHarder = true,
                AutoRotate = false,
                //PureBarcode = true,
                UseNativeScanning = true,
                // AssumeGS1 = true,

                // Avoid "Too soon between frames" by reducing the delay between analyzing frames.
                DelayBetweenAnalyzingFrames = 5,
                DelayBetweenContinuousScans = 5,

                /*
                CameraResolutionSelector = availableResolutions =>
                {
                    // Choose highest available resolution
                    return availableResolutions.OrderByDescending(r => r.Width).ThenByDescending(r => r.Height).First();
                }
                */
            };

            BindingContext = this;
        }

        MobileBarcodeScanningOptions barcodeOptions;
        public MobileBarcodeScanningOptions BarcodeOptions
        {
            get => barcodeOptions;
        }

        public ICommand FlashCommand => new Command(() => IsTorchOn = !IsTorchOn);
        public ICommand EnableButtonCommand => new Command(() => IsScanning = true);
        public ICommand CancelButtonCommand => new Command(async () => await Navigation.PopAsync());
        public ICommand DisableButtonCommand => new Command(() => IsScanning = false);

        public Command ScanResultCommand => new Command(() =>
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Stop analysis until we navigate away so we don't keep reading barcodes
                IsAnalyzing = false;

                // Show an alert
                await DisplayAlert("Scanned Barcode", result.Text, "OK");
            });
        });

        public bool HasTorch => ScannerView.HasTorch;

        bool isTorchOn = false;
        public bool IsTorchOn
        {
            set { SetProperty(ref isTorchOn, value); }
            get { return isTorchOn; }
        }

        double zoomFactor = 1.5;
        public double ZoomFactor
        {
            set {
                Console.WriteLine($"ZoomFactor: {value}");
                SetProperty(ref zoomFactor, value); }
            get { return zoomFactor; }
        }

        bool isScanning;
        public bool IsScanning
        {
            get => isScanning;
            set => SetProperty(ref isScanning, value);
        }

        bool isAnalyzing;
        public bool IsAnalyzing
        {
            get => isAnalyzing;
            set
            {
                SetProperty(ref isAnalyzing, value);
            }
        }

        ZXing.Result result;
        public ZXing.Result Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            IsScanning = false;

            base.OnDisappearing();
        }
    }
}

