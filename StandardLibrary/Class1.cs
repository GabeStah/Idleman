using System;
using System.IO;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Class1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/17298034/converting-writeablebitmap-to-bitmap-in-c-sharp"/>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static async Task<SoftwareBitmap> ToSoftwareBitmap(this BitmapImage bitmapImage)
        {
            //var handle = bitmapImage.GetHbitmap();
            try
            {
                var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Rgba8, (int)bitmapImage.Width,
                    (int)bitmapImage.Height);

                using (IRandomAccessStream stream =
                    await ConvertToRandomAccessStream((MemoryStream)bitmapImage.StreamSource))
                {
                    // Create the decoder from the stream
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    return softwareBitmap;
                }
                var blah = true;
                //new System.IO.Stream().As
                //using (IRandomAccessStream stream = bitmapImage.StreamSource)
                //{

                //}
                //using (var randomAccessStream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                //{
                //    new MemoryStream().AsRandomAccessStream();
                //    // Note that AsBuffer here is System.Runtime.InteropServices.WindowsRuntime.AsBuffer extension.
                //    await randomAccessStream.WriteAsync(bitmapImage.StreamSource);

                //    var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(
                //        randomAccessStream);


                //softwareBitmap.CopyFromBuffer(bitmapImage.);
                //return softwareBitmap;
            }
            finally
            {
                //DeleteObject(handle);
            }
        }
    }
}
