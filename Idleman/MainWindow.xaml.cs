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
using Idleman.View;
using Utility;
using BitmapImage = System.Windows.Media.Imaging.BitmapImage;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Idleman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapImage BackgroundImage;
        public ConfigurationWindow ConfigurationWindow;

        public MainWindow()
        {
            InitializeComponent();
            ConfigurationWindow = new ConfigurationWindow();
            ConfigurationWindow.Show();

            //var capture = new Capture
            //{
            //    Screen = new Screen()
            //};
            ////Screenshot.SetBinding(Screenshot.Source, "");
            //Screenshot.Source = capture.Screen.GetBitmap()?.ToImageSource();
            //TestScreenRefresh();
            //Screenshot.Stretch = Stretch.Uniform;
            //var blah = true;

            //BackgroundImage = capture.Screen.GetBitmap().ToBitmapImage();
            //BackgroundImage.Freeze();
            //Screenshot.Source = capture.Screen.GetBitmap().ToBitmapImage();
            //Screenshot.Stretch = Stretch.Uniform;
            //Tests.Clicking.ClickZombidleWindowTest();
            //Capture.TestCaptureZombidle();
            //screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();

            //Capturing.DuplicateZombidleWindow();


            //capture.Screen.OnScreenshotChanged += Screen_OnScreenshotChanged;

            ////for (var i = 1; i <= 1000; i++)
            ////{
            //Screenshot.Source = capture.Screen.GetBitmap()?.ToImageSource();
            //}
        }

        private async Task TestScreenRefresh()
        {
            var capture = new Capture
            {
                Screen = new Screen()
            };
            for (var i = 1; i <= 1000; i++)
            {
                Screenshot.Source = await Task.Run(async () =>
                {
                    var bitmap = await capture.Screen.GetBitmap();
                    return bitmap?.ToImageSource();
                });
            }
        }

        /// <summary>
        /// Event handler for Screen.ScreenshotChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Screen_OnScreenshotChanged(object sender, EventArgs e)
        {
            Logging.Log("Screenshot changed!");
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            //var src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri("pack://application:,,,/assets/images/Wide310x150Logo.scale-200.png", UriKind.RelativeOrAbsolute);
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.EndInit();
            //Screenshot.Source = src;
            //Screenshot.Stretch = Stretch.Uniform;
            ////int q = src.PixelHeight;        // Image loads here

            ////await Task.Run(async () =>
            ////{
            ////    var screen = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            ////    var bitmap = screen.GetBitmap();
            ////    screen.SaveScreenshot("screenshot.png");
            ////    screenshot.Source = bitmap.ToImageSource();
            ////});

            //var screen = new Screen("ApolloRuntimeContentWindow", "Zombidle");
            //var bitmap = screen.GetBitmap();
            //screen.SaveScreenshot("screenshot.png");
            //Screenshot.Source = bitmap.ToImageSource();
            //Screenshot.Stretch = Stretch.Uniform;

            //MarkerCanvas.Height = 1000;
            //MarkerCanvas.Width = 1000;

            //var capture = new Capture
            //{
            //    Screen = new Screen()
            //};
            //for (var i = 1; i <= 1000; i++)
            //{
            //    Screenshot.Source = await Task.Run(() =>
            //    {
            //        return capture.Screen.GetBitmap()?.ToImageSource();
            //    });
            //    Screenshot.Stretch = Stretch.UniformToFill;
            //}
            //var blah = true;
        }

        private async void Screenshot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();


            //for (var i = 1; i <= 20; i++)
            //{
            //    Logging.Log($"Count: {i}");
            //System.Threading.Thread.Sleep(0);

            //}

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
            Screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
        }

        private Point startPoint;
        private System.Windows.Shapes.Rectangle rect;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(MarkerCanvas);

            var blah = new System.Windows.Shapes.Rectangle();

            rect = new System.Windows.Shapes.Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.X);
            MarkerCanvas.Children.Add(rect);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || rect == null)
                return;

            var pos = e.GetPosition(MarkerCanvas);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rect = null;
        }
    }
}
