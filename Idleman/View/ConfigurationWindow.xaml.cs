using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Idleman.Extensions;
using Idleman.Model;
using Idleman.Tests;
using Utility;
using Cursors = System.Windows.Input.Cursors;
using Path = System.Windows.Shapes.Path;
using Timer = System.Threading.Timer;

namespace Idleman.View
{
    class SecondThreadConcern
    {
        public static void LongWork(IProgress<string> progress, Screen screen, System.Windows.Controls.Image captureImage)
        {
            for (var i = 0; i < 100; i++)
            {
                Task.Delay(500).Wait();
                var bitmap = (Bitmap)screen.Window.Image;
                var source = bitmap.ToImageSource();
                source.Freeze();
                Logging.Log($"Dispatch has access?: {Dispatcher.CurrentDispatcher.CheckAccess()}");
                Logging.Log(Dispatcher.CurrentDispatcher.ToString());
                Logging.Log(Dispatcher.CurrentDispatcher.Thread.IsBackground);
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {//this refer to form in WPF application 
                    captureImage.Source = source;
                });
                progress.Report(i.ToString());
            }
        }
    }

    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public Screen Screen { get; set; }
        private readonly SynchronizationContext _synchronizationContext;
        private DateTime _previousTime = DateTime.Now;
        public DispatcherTimer DispatchTimer { get; set; }
        private const int RefreshRate = 25;
        public Capture Capture { get; set; } = new Capture();
        public CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
        public Singleton Singleton { get; set; } = Singleton.Instance;

        public ConfigurationWindow()
        {
            InitializeComponent();
            this._synchronizationContext = SynchronizationContext.Current;

            // Test threading
            var thread = new ThreadService();

            // Set new screen.
            Screen = new Screen();

            //DispatchTimer = new DispatcherTimer(
            //    TimeSpan.FromMilliseconds(RefreshRate), 
            //    DispatcherPriority.Background,
            //    DispatchTimer_TickAsync,
            //    Application.Current.Dispatcher);
            // TODO: Reenabled timer.
            //DispatchTimer.Start();

            // Setup processing network
            ImageProcessingNetwork = new ImageProcessingNetwork(CaptureImage, ChooseFolderButton, CancelButton, HeadBlock);

            while (true)
            {
                Thread.Sleep(1500);
                var (index, value) = Singleton.PopValue();
                if (index > -1)
                {
                    Logging.Log($"[{DateTime.Now.ToLongTimeString()}] Main Thread iterating Singleton, popping #{value} at {index} index");
                }
            }

            // Setup capture processing network
            //CaptureProcessingNetwork = new CaptureProcessingNetwork(CaptureImage, CancelButton, HeadBlock);
        }

        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            //await Task.Run(() =>
            //{
            // TODO: Check for ArgumentException on System.Window.Image call.
            //var image = (Bitmap)Screen.Window.Image;
            //var source = image.ToImageSource();
            //source.Freeze();

            UpdateUi();

            //});
        }

        private async void DispatchTimer_TickAsync(object sender, EventArgs e)
        {
            await UpdateUiAsync();
        }

        public async Task UpdateUiAsync()
        {
            while (!TokenSource.Token.IsCancellationRequested)
            {
                await Task.Run((Action) UpdateCaptureImage);
            }
        }

        public void UpdateCaptureImage()
        {
            //var source = Capture.TestImageMatch().Result?.ToBitmap().ToImageSource();
            //source?.Freeze();
            //CaptureImage.Source = source;

            _synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                var source = Capture.TestImageMatch().Result?.ToBitmap().ToImageSource();
                source?.Freeze();
                CaptureImage.Source = source;
                //label.Content = @"Counter " + (int)o;
            }), "");
        }

        public void UpdateUi()
        {
            //var timeNow = DateTime.Now;

            //if ((DateTime.Now - _previousTime).Milliseconds <= 10) return;

            //Screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();

            _synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                Task.Delay(15);
                var source = Capture.TestImageMatch().Result?.ToBitmap().ToImageSource();
                source?.Freeze();
                CaptureImage.Source = source;
                //label.Content = @"Counter " + (int)o;
            }), "");

            //_previousTime = timeNow;
        }

        public void UpdateUiSource(ImageSource source)
        {
            //var timeNow = DateTime.Now;

            //if ((DateTime.Now - _previousTime).Milliseconds <= 10) return;

            //Screenshot.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();

            _synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                CaptureImage.Source = source;
                //label.Content = @"Counter " + (int)o;
            }), source);

            //_previousTime = timeNow;
        }

        public void UpdateUi2()
        {
            //var timeNow = DateTime.Now;

            //if ((DateTime.Now - _previousTime).Milliseconds <= 10) return;

            

            _synchronizationContext.Post(new SendOrPostCallback(async o =>
            {
                //var image = (Bitmap) Capture.Screen.Window.Image;
                //var source = image.ToImageSource();
                //source.Freeze();
                var image = await Task.Run(() => Capture.TestImageMatch2());
                if (image != null)
                {
                    CaptureImage.Source = image.ToBitmap().ToImageSource();
                }
                //label.Content = @"Counter " + (int)o;
            }), "");

            //_previousTime = timeNow;
        }

        private async void MarkerCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var count = 0;

            await Task.Run(() =>
            {
                for (var i = 0; i <= 50000; i++)
                {
                    // TODO: Check for ArgumentException on System.Window.Image call.
                    Thread.Sleep(250);
                    //Task.Delay(5000);
                    var image = (Bitmap) Screen.Window.Image;
                    var source = image.ToImageSource();
                    source.Freeze();
                    UpdateUiSource(source);
                    count = i;
                }
            });

            label.Content = @"Counter " + count;


            //var progress = new Progress<string>(s => label.Content = s);
            //Logging.Log($"Dispatch has access?: {Dispatcher.CurrentDispatcher.CheckAccess()}");
            //Logging.Log(Dispatcher.CurrentDispatcher.ToString());
            //Logging.Log(Dispatcher.CurrentDispatcher.Thread.IsBackground);
            //await Task.Factory.StartNew(() => SecondThreadConcern.LongWork(progress, Screen, CaptureImage), TaskCreationOptions.LongRunning);
            //label.Content = "Completed";




            //for (var i = 1; i <= 1000; i++)
            //{
            //    var task = Task.Run<ImageSource>(() =>
            //    {

            //        var bitmap = (Bitmap)Screen.Window.Image;
            //        return bitmap.ToImageSource();
            //    });
            //    CaptureImage.Source = task.Result;
            //    //Thread.Sleep(25);
            //}
            //CaptureImage.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
            //Task.Run(() =>
            //{
            //    for (var i = 1; i <= 1000; i++)
            //    {
            //        var bitmap = (Bitmap) Screen.Window.Image;
            //        var source = bitmap.ToImageSource();
            //        source.Freeze();
            //        CaptureImage.Source = source;
            //    }
            //});
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    for (var i = 1; i <= 1000; i++)
            //    {
            //        var bitmap = (Bitmap)Screen.Window.Image;
            //        CaptureImage.Source = bitmap.ToImageSource();
            //    }
            //}));
        }

        //private void MarkerCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    for (var i = 1; i <= 1000; i++)
        //    {
        //        var task = Task.Run<ImageSource>(() =>
        //        {
                    
        //            var bitmap = (Bitmap) Screen.Window.Image;
        //            return bitmap.ToImageSource();
        //        });
        //        CaptureImage.Source = task.Result;
        //        //Thread.Sleep(25);
        //    }
        //    //CaptureImage.Source = Capturing.CaptureSubImageTest().Result?.ToBitmap().ToImageSource();
        //        //Task.Run(() =>
        //        //{
        //        //    for (var i = 1; i <= 1000; i++)
        //        //    {
        //        //        var bitmap = (Bitmap) Screen.Window.Image;
        //        //        var source = bitmap.ToImageSource();
        //        //        source.Freeze();
        //        //        CaptureImage.Source = source;
        //        //    }
        //        //});
        //    //Dispatcher.BeginInvoke(new Action(() =>
        //    //{
        //    //    for (var i = 1; i <= 1000; i++)
        //    //    {
        //    //        var bitmap = (Bitmap)Screen.Window.Image;
        //    //        CaptureImage.Source = bitmap.ToImageSource();
        //    //    }
        //    //}));
        //}

        private void CaptureImage_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var bitmap = (Bitmap) Screen.Window.Image;
            CaptureImage.Source = bitmap.ToImageSource();
        }

        private void ChooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a FolderBrowserDialog object to enable the user to 
            // select a folder.
            FolderBrowserDialog dlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = false
            };

            

            // Set the selected path to the common Sample Pictures folder
            // if it exists.
            string initialDirectory = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures),
                "Sample Pictures");
            if (Directory.Exists(initialDirectory))
            {
                dlg.SelectedPath = initialDirectory;
            }

            // Show the dialog and process the dataflow network.
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Create a new CancellationTokenSource object to enable 
                // cancellation.
                CancellationTokenSource = new CancellationTokenSource();

                // Create the image processing network if needed.
                if (HeadBlock == null)
                {
                    HeadBlock = ImageProcessingNetwork.CreateImageProcessingNetwork(CancellationTokenSource);
                }

                // Post the selected path to the network.
                HeadBlock.Post(dlg.SelectedPath);

                // Enable the Cancel button and disable the Choose Folder button.
                ChooseFolderButton.IsEnabled = false;
                CancelButton.IsEnabled = true;

                // Show a wait cursor.
                Cursor = Cursors.Wait;
            }
        }

        private void TestCaptureProcessingNetwork()
        {
            // Create a new CancellationTokenSource object to enable 
            // cancellation.
            CancellationTokenSource = new CancellationTokenSource();

            if (CaptureHeadBlock == null)
            {
                CaptureHeadBlock = CaptureProcessingNetwork.CreateCaptureProcessingNetwork(CancellationTokenSource);
            }

            // Post to the network.
            CaptureHeadBlock.Post("");
        }

        public ITargetBlock<string> HeadBlock { get; set; }

        public ITargetBlock<string> CaptureHeadBlock { get; set; }

        public ImageProcessingNetwork ImageProcessingNetwork { get; set; }

        public CaptureProcessingNetwork CaptureProcessingNetwork { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public Timer Timer1 { get; set; }
        public Timer Timer2 { get; set; }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource.Cancel();
        }

        private void CaptureTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Begin test
            //TestCaptureProcessingNetwork();

            Timer1 = new Timer(_ =>
                TestCaptureProcessingNetwork(),
                null,
                1000,
                250
            );

            //Timer2 = new Timer(CaptureProcessingNetwork.OutputMessage, "A state", 0, 500);
        }
    }
}
