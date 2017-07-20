using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;

namespace Idleman.Model
{
    class ThreadService
    {
        internal Thread Thread { get; set; }
        internal int Counter { get; set; }
        internal Bitmap TestTargetBitmap { get; set; }
        internal Singleton Singleton { get; set; }

        public ThreadService()
        {
            TestTargetBitmap =
                new Bitmap(
                    "D:\\dev\\csharp\\projects\\Idleman\\Idleman\\assets\\images\\zombidle\\monsters\\11-carl-the-monolith.png");

            // Get singleton
            Singleton = Singleton.Instance;

            // Create new thread
            Thread = new Thread(
                () => CaptureImage()
            );
            Thread.Name = "TestThread";
            Thread.Start();
        }

        internal void SleepTest()
        {
            Thread.Sleep(Timeout.Infinite);
        }

        internal void LoopTest(int delay = 250)
        {
            while (true)
            {
                Counter++;
                Thread.Sleep(delay);
                Logging.Log($"[{DateTime.Now.ToLongTimeString()}] {Thread.Name} iterating LoopTest() #{Counter}");
            }
        }

        // Capture image
        // Process image
        // Post image to queue
        // UI THREAD: Read image queue and display

        internal void CaptureImage(int delay = 250)
        {
            while (true)
            {
                Thread.Sleep(delay);
                var screen = new Screen();
                // Capture image
                var image = (Bitmap)screen.Window.Image;
                // Process image
                Capture.FindMarker(image, TestTargetBitmap);
                // Post image to queue
                // Test singleton
                Counter++;
                Singleton.AddValue(Counter);
                Logging.Log($"[{DateTime.Now.ToLongTimeString()}] {Thread.Name} iterating CaptureImage() #{Counter}");
            }
        }
    }
}
