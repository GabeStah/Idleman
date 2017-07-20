using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using BitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;
using BitmapEncoder = Windows.Graphics.Imaging.BitmapEncoder;

namespace Idleman.Extensions
{
    public static class Bitmaps
    {
        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Extension to convert to BitmapImage.
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa"/>
        /// <param name="bitmap">Bitmap to be convereted.</param>
        /// <returns>Converted BitmapImage instance.</returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static byte[] ToBytes(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                var source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            finally { DeleteObject(handle); }
        }

        /// <summary>
        /// Converts passed MemoryStream to IRandomAccessStream.
        /// </summary>
        /// <param name="memoryStream">MemoryStream to convert.</param>
        /// <returns>Converted RandomAccessStream.</returns>
        public static async Task<IRandomAccessStream> ToRandomAccessStream(this MemoryStream memoryStream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();

            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            var dataWriter = new DataWriter(outputStream);
            var task = new Task(() => dataWriter.WriteBytes(memoryStream.ToArray()));
            task.Start();

            await task;
            await dataWriter.StoreAsync();
            await outputStream.FlushAsync();

            return randomAccessStream;
        }

        /// <summary>
        /// Convert Bitmap to SoftwareBitmap.
        /// Beneficial for use in UWP framework and API.
        /// </summary>
        /// <param name="bitmap">Bitmap to be converted.</param>
        /// <returns>Converted SoftwareBitmap instance.</returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmap(this Bitmap bitmap)
        {
            try
            {
                // Create new MemoryStream for original bitmap data.
                using (var memoryStream = new MemoryStream())
                {
                    // Save data to memory stream.
                    bitmap.Save(memoryStream, ImageFormat.Bmp);
                    // Reset position.
                    //memoryStream.Position = 0;
                    // Convert MemoryStream to RandomAccessStream.
                    using (var randomAccessStream = await memoryStream.ToRandomAccessStream())
                    {
                        // Create the decoder from the stream.
                        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

                        // Get the SoftwareBitmap representation of the file.
                        return await decoder.GetSoftwareBitmapAsync();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Convert BitmapImage to SoftwareBitmap.
        /// </summary>
        /// <param name="bitmap">BitmapImage to be convereted.</param>
        /// <returns>Converted SoftwareBitmap instance.</returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmap(this BitmapImage bitmap)
        {
            try
            {
                using (var stream = await ToRandomAccessStream((MemoryStream) bitmap.StreamSource))
                {
                    // Create the decoder from the stream
                    var decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file
                    return await decoder.GetSoftwareBitmapAsync();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }
    }
}
