using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.UI.Xaml.Media.Imaging;
using Idleman.Extensions;
using Idleman.Tests;
using Utility;
using BitmapImage = System.Windows.Media.Imaging.BitmapImage;

namespace Idleman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Tests.Clicking.ClickZombidleWindowTest();
            //Capture.TestCaptureZombidle();
            screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("pack://application:,,,/assets/images/Wide310x150Logo.scale-200.png", UriKind.RelativeOrAbsolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            //screenshot.Source = src;
            //screenshot.Stretch = Stretch.Uniform;
            //int q = src.PixelHeight;        // Image loads here

            //await Task.Run(async () =>
            //{
            //    var screen = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            //    var bitmap = screen.GetBitmap();
            //    screen.SaveScreenshot("screenshot.png");
            //    screenshot.Source = bitmap.ToImageSource();
            //});

            var screen = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            var bitmap = screen.GetBitmap();
            screen.SaveScreenshot("screenshot.png");
            screenshot.Source = bitmap.ToImageSource();
            screenshot.Stretch = Stretch.Uniform;
        }

        private async void screenshot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            for (var i = 1; i <= 20; i++)
            {
                Logging.Log($"Count: {i}");
                //System.Threading.Thread.Sleep(0);
                screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
            }

            //var screen = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            //var bitmap = screen.GetBitmap();
            ////screen.SaveScreenshot("screenshot.png");
            //screenshot.Source = bitmap.ToImageSource();
            //screenshot.Stretch = Stretch.Uniform;

            //var bitmapImage = bitmap.ToBitmapImage();
            //var softwareBitmap = await bitmapImage.ToSoftwareBitmap();

            //Capturing.CaptureZombidleWindowTest(iterations: 50, delay: 0);
            //Capturing.CaptureZombidleWindowTest3(iterations: 50, delay: 0);
            //screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
            // TODO: Output capture to screenshot image source.
            //screenshot.Source = softwareBitmap.
            //var capture = new Capture();
            //capture.OcrEngine = OcrEngine.TryCreateFromUserProfileLanguages();

            //if (capture.OcrEngine != null)
            //{
            //    // Recognize text from image.
            //    var ocrResult = await capture.OcrEngine.RecognizeAsync();

            //    // Display recognized text.
            //    ExtractedTextBox.Text = ocrResult.Text;

            //    if (ocrResult.TextAngle != null)
            //    {
            //        // If text is detected under some angle in this sample scenario we want to
            //        // overlay word boxes over original image, so we rotate overlay boxes.
            //        TextOverlay.RenderTransform = new RotateTransform
            //        {
            //            Angle = (double)ocrResult.TextAngle,
            //            CenterX = PreviewImage.ActualWidth / 2,
            //            CenterY = PreviewImage.ActualHeight / 2
            //        };
            //    }

            //    // Create overlay boxes over recognized words.
            //    foreach (var line in ocrResult.Lines)
            //    {
            //        Rect lineRect = Rect.Empty;
            //        foreach (var word in line.Words)
            //        {
            //            lineRect.Union(word.BoundingRect);
            //        }

            //        // Determine if line is horizontal or vertical.
            //        // Vertical lines are supported only in Chinese Traditional and Japanese languages.
            //        bool isVerticalLine = lineRect.Height > lineRect.Width;

            //        foreach (var word in line.Words)
            //        {
            //            WordOverlay wordBoxOverlay = new WordOverlay(word);

            //            // Keep references to word boxes.
            //            wordBoxes.Add(wordBoxOverlay);

            //            // Define overlay style.
            //            var overlay = new Border()
            //            {
            //                Style = isVerticalLine ?
            //                    (Style)this.Resources["HighlightedWordBoxVerticalLine"] :
            //                    (Style)this.Resources["HighlightedWordBoxHorizontalLine"]
            //            };

            //            // Bind word boxes to UI.
            //            overlay.SetBinding(Border.MarginProperty, wordBoxOverlay.CreateWordPositionBinding());
            //            overlay.SetBinding(Border.WidthProperty, wordBoxOverlay.CreateWordWidthBinding());
            //            overlay.SetBinding(Border.HeightProperty, wordBoxOverlay.CreateWordHeightBinding());

            //            // Put the filled textblock in the results grid.
            //            TextOverlay.Children.Add(overlay);
            //        }
            //    }

            //    // Rescale word boxes to match current UI size.
            //    UpdateWordBoxTransform();

            //    rootPage.NotifyUser(
            //        "Image is OCRed for " + ocrEngine.RecognizerLanguage.DisplayName + " language.",
            //        NotifyType.StatusMessage);
            //}
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
        }
    }
}
