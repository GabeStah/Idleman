using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using Idleman.Extensions;
using Utility;
using Timer = System.Threading.Timer;

namespace Idleman.Model
{
    public class CaptureProcessingNetwork
    {
        // The head of the dataflow network.
        ITargetBlock<string> HeadBlock = null;

        // Enables the user interface to signal cancellation to the network.
        CancellationTokenSource cancellationTokenSource;

        public System.Windows.Controls.Image Image { get; set; }

        public System.Windows.Controls.Button CancelButton { get; set; }

        public Screen Screen { get; set; }

        public Bitmap TestTargetBitmap { get; set; }

        public CaptureProcessingNetwork(System.Windows.Controls.Image image,
            System.Windows.Controls.Button cancelButton,
            ITargetBlock<string> targetBlock)
        {
            Image = image;
            CancelButton = cancelButton;
            HeadBlock = targetBlock;
            Screen = new Screen();

            TestTargetBitmap =
                new Bitmap(
                    "D:\\dev\\csharp\\projects\\Idleman\\Idleman\\assets\\images\\zombidle\\monsters\\11-carl-the-monolith.png");
        }

        public static void PerformCaptureAndOutput(object state)
        {
            Logging.Log($"{DateTime.Now} - Capturing Process Network!");
        }

        // Creates the image processing dataflow network and returns the
        // head node of the network.
        public ITargetBlock<string> CreateCaptureProcessingNetwork(CancellationTokenSource token)
        {
            //
            // Create the dataflow blocks that form the network.
            //
            cancellationTokenSource = token;

            // Set sync context.
            TaskScheduler syncScheduler;
            syncScheduler = SynchronizationContext.Current != null ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current;

            // Create block that gets screen capture.
            var capturedBitmaps = new TransformBlock<string, Bitmap>(path =>
                {
                    try
                    {
                        return GetScreenBitmap();
                    }
                    catch (OperationCanceledException)
                    {
                        // Handle cancellation by passing the empty collection
                        // to the next stage of the network.
                        return null;
                    }
                });

            var createTargettedBitmap = new TransformBlock<Bitmap, Bitmap>(bitmap =>
                {
                    try
                    {
                        return Capture.FindMarker(bitmap, TestTargetBitmap);
                    }
                    catch (OperationCanceledException exception)
                    {
                        return null;
                    }
                });

            var displayTargettedBitmap = new ActionBlock<Bitmap>(bitmap =>
                {
                    Image.Stretch = System.Windows.Media.Stretch.Uniform;
                    Image.Source = bitmap.ToImageSource();
                    CancelButton.IsEnabled = true;
                },
                new ExecutionDataflowBlockOptions
                {
                    TaskScheduler = syncScheduler
                });

            // Link capturedBitmaps to createTargettedBitmap, if not null.
            capturedBitmaps.LinkTo(createTargettedBitmap, bitmap => bitmap != null);

            // Link display after creation of targetted bitmap.
            createTargettedBitmap.LinkTo(displayTargettedBitmap, bitmap => bitmap != null);

            // Create block that 
            return capturedBitmaps;
        }

        Bitmap GetScreenBitmap()
        {
            // Throw OperationCanceledException if cancellation is requested.
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                return (Bitmap) Screen.Window.Image;
            }
            catch (Exception)
            {
                // TODO: A complete application might handle the error.
            }
            return null;
        }
    }
}
