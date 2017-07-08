using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Ocr;
using Idleman.Extensions;
using Utility;
using RoutedEventArgs = Windows.UI.Xaml.RoutedEventArgs;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;

namespace Idleman
{
    class Capture
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/threading-model
        /// </summary>

        // Bitmap holder of currently loaded image.
        //private WriteableBitmap bitmap;

        // OCR engine instance used to extract text from images.
        public OcrEngine OcrEngine;

        public Screen Screen { get; set; }

        public Capture()
        {
            ////rootPage = MainPage.Current;

            ////this.InitializeComponent();

            //OcrEngine = new OcrEngine();

            //// Load all available languages from OcrLanguage enum in combo box.
            //LanguageList.ItemsSource = Enum.GetNames(typeof(OcrLanguage)).OrderBy(name => name.ToString());
            //LanguageList.SelectedItem = OcrEngine.Language.ToString();
            //LanguageList.SelectionChanged += LanguageList_SelectionChanged;
        }

        public static async Task<SoftwareBitmap> TestCaptureZombidle()
        {
            OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            // If no engine, return void.
            if (ocrEngine == null) return null;
            var screen = new Screen();
            var softwarebitmap = await screen.GetBitmap().ToBitmapImage().ToSoftwareBitmap();
            var ocrResult = await ocrEngine.RecognizeAsync(softwarebitmap);
            Logging.Log(ocrResult.Text);
            return softwarebitmap;
        }

        public static async Task<SoftwareBitmap> TestCaptureZombidle2()
        {
            OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            // If no engine, return void.
            if (ocrEngine == null) return null;
            var screen = new Screen();
            var softwarebitmap = await screen.GetSoftwareBitmap();
            var ocrResult = await ocrEngine.RecognizeAsync(softwarebitmap);
            Logging.Log(ocrResult.Text);
            return softwarebitmap;
        }

        public async Task<SoftwareBitmap> TestCaptureZombidle3()
        {
            // If no engine, return void.
            if (OcrEngine == null || Screen == null) return null;
            var softwarebitmap = await Screen.GetSoftwareBitmap();
            var ocrResult = await OcrEngine.RecognizeAsync(softwarebitmap);
            Logging.Log(ocrResult.Text);
            return softwarebitmap;
        }

        public async Task<Image<Bgr, byte>> TestImageMatch()
        {
            // If no engine, return void.
            if (OcrEngine == null || Screen == null) return null;
            var screenBitmap = Screen.GetBitmap();
            //var ocrResult = await OcrEngine.RecognizeAsync(bitmap);
            //Logging.Log(ocrResult.Text);
            //return bitmap;

            Image<Bgr, byte> source = new Image<Bgr, byte>(screenBitmap);
            Image<Bgr, byte> template = new Image<Bgr, byte>("D:\\dev\\apps\\Idleman\\Idleman\\assets\\images\\zombidle\\monsters\\9-flying-squid.png");
            Image<Bgr, byte> imageToShow = source.Copy();

            using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues;
                double[] maxValues;
                System.Drawing.Point[] minLocations;
                System.Drawing.Point[] maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > 0.8)
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    Rectangle match = new Rectangle(maxLocations[0], template.Size);
                    imageToShow.Draw(match, new Bgr(Color.Red), 3);
                    return imageToShow;
                }
            }
            return null;
        }

        public void TestTesseractOcr()
        {
            // If no engine, return void.
            //if (OcrEngine == null || Screen == null) return null;
            //var screenBitmap = Screen.GetBitmap();
            //var tesseract = new Tesseract();
            //tesseract.Recognize()
            //tesseract.SetImage((Pix)screenBitmap);
            //tesseract.GetUTF8Text();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Starting with the .NET Framework 4.5, the System.Threading.Tasks.Task.Delay method is provided for this purpose,
        /// and you can use it inside another asynchronous method, for example, to implement an asynchronous polling loop.
        /// 
        /// See: https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/implementing-the-task-based-asynchronous-pattern
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task Poll(Uri url, CancellationToken cancellationToken,
            IProgress<bool> progress)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                bool success = false;
                try
                {
                    //await MethodNameAsync();
                    success = true;
                }
                catch { /* ignore errors */ }
                progress.Report(success);
            }
        }

        public async Task<Bitmap> DownloadDataAndRenderImageAsync(
            CancellationToken cancellationToken)
        {
            //var imageData = await DownloadImageDataAsync(cancellationToken);
            byte[] imageData = new byte[0];
            return await RenderAsync(imageData, cancellationToken);
        }

        internal Task<Bitmap> RenderAsync(
            byte[] data, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var bmp = new Bitmap(0, 0);
                for (int y = 0; y < 0; y++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    for (int x = 0; x < 0; x++)
                    {
                        // render pixel [x,y] into bmp
                    }
                }
                return bmp;
            }, cancellationToken);
        }

        //async void CaptureFaceExample(object sender, RoutedEventArgs e)
        //{
        //    // Image from the '3 amigos' film...
        //    var url =
        //        "http://wac.450f.edgecastcdn.net/80450F/screencrush.com/442/files/2012/05/d3ec4c0848f2.png";

        //    using (var httpClient = new System.Net.Http.HttpClient())
        //    {
        //        var bytes = await httpClient.GetByteArrayAsync(url);

        //        using (var randomAccessStream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
        //        {
        //            // Note that AsBuffer here is System.Runtime.InteropServices.WindowsRuntime.AsBuffer extension.
        //            await randomAccessStream.WriteAsync(bytes.AsBuffer());

        //            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(
        //                randomAccessStream);

        //            // I don't think there's a common format between the decoder and the
        //            // face detector so I first go for BGRA8 and then I'll convert it
        //            // to a format supported by the FaceDetector.
        //            using (var bitmap = await decoder.GetSoftwareBitmapAsync(
        //                BitmapPixelFormat.Bgra8,
        //                BitmapAlphaMode.Premultiplied))
        //            {
        //                var detectorFormat = FaceDetector.GetSupportedBitmapPixelFormats().First();

        //                // Convert for detection.
        //                using (var detectorBitmap =
        //                    Windows.Graphics.Imaging.SoftwareBitmap.Convert(bitmap, detectorFormat))
        //                {
        //                    var detector = await Windows.Media.FaceAnalysis.FaceDetector.CreateAsync();

        //                    // Detect.
        //                    var faces = await detector.DetectFacesAsync(detectorBitmap);

        //                    // Report.
        //                    MessageBox.Show($"I see {faces.Count} faces in that image");
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
