using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Media.Ocr;
using Emgu.CV;
using Emgu.CV.Structure;
using Idleman.Extensions;
using Utility;

namespace Idleman.Tests
{
    class Capturing
    {
        public static async void CaptureZombidleWindowTest(int iterations = 20, int delay = 250)
        {
            for (var i = 1; i <= iterations; i++)
            {
                Logging.Log($"Count: {i}");
                System.Threading.Thread.Sleep(delay);
                var softwareBitmap = await Capture.TestCaptureZombidle();
            }
        }

        public static async void CaptureZombidleWindowTest3(int iterations = 20, int delay = 250)
        {
            var capture = new Capture
            {
                OcrEngine = OcrEngine.TryCreateFromUserProfileLanguages(),
                Screen = new Screen()
            };
            for (var i = 1; i <= iterations; i++)
            {
                Logging.Log($"Count: {i}");
                await capture.TestCaptureZombidle3();
            }
        }

        public static async Task<ImageSource> DuplicateZombidleWindow(int iterations = 6000)
        {
            var capture = new Capture
            {
                Screen = new Screen()
            };
            var bitmap = await capture.Screen.GetBitmap();
            return bitmap?.ToImageSource();
        }


        public static async Task<Image<Bgr, byte>> CaptureSubImageTest()
        {
            var capture = new Capture
            {
                OcrEngine = OcrEngine.TryCreateFromUserProfileLanguages(),
                Screen = new Screen()
            };

            return await capture.TestImageMatch();
        }

        public static async Task<Image<Bgr, byte>> CaptureSubImageTest2()
        {
            var capture = new Capture
            {
                OcrEngine = OcrEngine.TryCreateFromUserProfileLanguages(),
                Screen = new Screen()
            };

            return await capture.TestImageMatch2();
        }
    }
}
