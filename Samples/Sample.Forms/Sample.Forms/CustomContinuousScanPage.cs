using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace Sample.Forms
{
	public class CustomContinuousScanPage : ContentPage
	{
		ZXingScannerView zxing;
		ZXingDefaultOverlay overlay;

		public CustomContinuousScanPage() : base()
		{
			zxing = new ZXingScannerView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				AutomationId = "zxingScannerView",
                ZoomFactor = 1.5
            };
			zxing.OnScanResult += (result) =>
				Device.BeginInvokeOnMainThread(async () =>
				{
					// Stop analysis until we navigate away so we don't keep reading barcodes
					zxing.IsAnalyzing = false;

					// Show an alert
					await DisplayAlert("Scanned Barcode", result.Text, "OK");
				});

			overlay = new ZXingDefaultOverlay
			{
				TopText = "Hold your phone up to the barcode",
				BottomText = "Scanning will happen automatically",
				ShowFlashButton = zxing.HasTorch,
				AutomationId = "zxingDefaultOverlay",
			};
			overlay.FlashButtonClicked += (sender, e) =>
			{
				zxing.IsTorchOn = !zxing.IsTorchOn;
			};
			
			var grid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			var stopButton = new Button
			{
				WidthRequest = 100,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.End,
				Text = "disable",
				Command = new Command(() => zxing.IsScanning = false)
			};

			var cancelButton = new Button
			{
				WidthRequest = 100,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.End,
				Text = "cancel",
				Command = new Command(async () => await Navigation.PopAsync())
			};

			var startButton = new Button
			{
				WidthRequest = 100,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.End,
				Text = "enable",
				Command = new Command(() => zxing.IsScanning = true)
			};
			grid.Children.Add(zxing);
			grid.Children.Add(overlay);

            var zoomSlider = new Slider
            {
                Maximum = 5.0,
                Minimum = 1.0,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Value = zxing.ZoomFactor
            };
            zxing.SetBinding(ZXingScannerView.ZoomFactorProperty, new Binding("Value", BindingMode.TwoWay, null, null, null, zoomSlider));

            var optionsStackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };
            var buttonArea = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            buttonArea.Children.Add(startButton);
            buttonArea.Children.Add(cancelButton);
            buttonArea.Children.Add(stopButton);

            optionsStackLayout.Children.Add(zoomSlider);
            optionsStackLayout.Children.Add(buttonArea);

            grid.Children.Add(optionsStackLayout);

            // The root page of your application
            Content = grid;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			zxing.IsScanning = true;
		}

		protected override void OnDisappearing()
		{
			zxing.IsScanning = false;

			base.OnDisappearing();
		}
	}
}
