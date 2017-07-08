using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Emgu.CV;
using Emgu.CV.Structure;
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


        public static async Task<Image<Bgr, byte>> CaptureSubImageTest()
        {
            var capture = new Capture
            {
                OcrEngine = OcrEngine.TryCreateFromUserProfileLanguages(),
                Screen = new Screen()
            };

            return await capture.TestImageMatch();
        }
    }
}
