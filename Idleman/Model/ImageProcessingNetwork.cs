﻿using System;
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

namespace Idleman.Model
{
    public class ImageProcessingNetwork
    {
        // The head of the dataflow network.
        ITargetBlock<string> headBlock = null;

        // Enables the user interface to signal cancellation to the network.
        CancellationTokenSource cancellationTokenSource;

        public System.Windows.Controls.Image image { get; set; }

        public System.Windows.Controls.Button toolStripButton1 { get; set; }

        public System.Windows.Controls.Button toolStripButton2 { get; set; }

        public ImageProcessingNetwork(System.Windows.Controls.Image picture, 
                                      System.Windows.Controls.Button button1, 
                                      System.Windows.Controls.Button button2,
                                      ITargetBlock<string> targetBlock)
        {
            image = picture;
            toolStripButton1 = button1;
            toolStripButton2 = button2;
            headBlock = targetBlock;
            
        }

        // Creates the image processing dataflow network and returns the
        // head node of the network.
        public ITargetBlock<string> CreateImageProcessingNetwork(CancellationTokenSource token)
        {
            //
            // Create the dataflow blocks that form the network.
            //
            cancellationTokenSource = token;

            // Create a dataflow block that takes a folder path as input
            // and returns a collection of Bitmap objects.
            var loadBitmaps = new TransformBlock<string, IEnumerable<Bitmap>>(path =>
            {
                try
                {
                    return LoadBitmaps(path);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing the empty collection
                    // to the next stage of the network.
                    return Enumerable.Empty<Bitmap>();
                }
            });

            // Create a dataflow block that takes a collection of Bitmap objects
            // and returns a single composite bitmap.
            var createCompositeBitmap = new TransformBlock<IEnumerable<Bitmap>, Bitmap>(bitmaps =>
            {
                try
                {
                    return CreateCompositeBitmap(bitmaps);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation by passing null to the next stage 
                    // of the network.
                    return null;
                }
            });

            // Create a dataflow block that displays the provided bitmap on the form.
            var displayCompositeBitmap = new ActionBlock<Bitmap>(bitmap =>
                {
                    // Display the bitmap.
                    image.Stretch = System.Windows.Media.Stretch.Uniform;
                    image.Source = bitmap.ToImageSource();

                    // Enable the user to select another folder.
                    toolStripButton1.IsEnabled = true;
                    toolStripButton2.IsEnabled = false;
                    //Cursor = DefaultCursor;
                },
                // Specify a task scheduler from the current synchronization context
                // so that the action runs on the UI thread.
                new ExecutionDataflowBlockOptions
                {
                    TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
                });

            // Create a dataflow block that responds to a cancellation request by 
            // displaying an image to indicate that the operation is cancelled and 
            // enables the user to select another folder.
            var operationCancelled = new ActionBlock<object>(delegate
                {
                    // Display the error image to indicate that the operation
                    // was cancelled.
                    image.Stretch = System.Windows.Media.Stretch.Uniform;
                    image.Source =
                        new Bitmap(
                            "D:\\dev\\csharp\\projects\\Idleman\\Idleman\\assets\\images\\zombidle\\monsters\\11-carl-the-monolith.png").ToImageSource();

                    // Enable the user to select another folder.
                    toolStripButton1.IsEnabled = true;
                    toolStripButton2.IsEnabled = false;
                    //Cursor = DefaultCursor;
                },
                // Specify a task scheduler from the current synchronization context
                // so that the action runs on the UI thread.
                new ExecutionDataflowBlockOptions
                {
                    TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
                });

            //
            // Connect the network.
            //

            // Link loadBitmaps to createCompositeBitmap. 
            // The provided predicate ensures that createCompositeBitmap accepts the 
            // collection of bitmaps only if that collection has at least one member.
            loadBitmaps.LinkTo(createCompositeBitmap, bitmaps => bitmaps.Any());

            // Also link loadBitmaps to operationCancelled.
            // When createCompositeBitmap rejects the message, loadBitmaps 
            // offers the message to operationCancelled.
            // operationCancelled accepts all messages because we do not provide a 
            // predicate.
            loadBitmaps.LinkTo(operationCancelled);

            // Link createCompositeBitmap to displayCompositeBitmap. 
            // The provided predicate ensures that displayCompositeBitmap accepts the 
            // bitmap only if it is non-null.
            createCompositeBitmap.LinkTo(displayCompositeBitmap, bitmap => bitmap != null);

            // Also link createCompositeBitmap to operationCancelled. 
            // When displayCompositeBitmap rejects the message, createCompositeBitmap 
            // offers the message to operationCancelled.
            // operationCancelled accepts all messages because we do not provide a 
            // predicate.
            createCompositeBitmap.LinkTo(operationCancelled);

            // Return the head of the network.
            return loadBitmaps;
        }

        // Loads all bitmap files that exist at the provided path.
        IEnumerable<Bitmap> LoadBitmaps(string path)
        {
            List<Bitmap> bitmaps = new List<Bitmap>();

            // Load a variety of image types.
            foreach (string bitmapType in
                new string[] { "*.bmp", "*.gif", "*.jpg", "*.png", "*.tif" })
            {
                // Load each bitmap for the current extension.
                foreach (string fileName in Directory.GetFiles(path, bitmapType))
                {
                    // Throw OperationCanceledException if cancellation is requested.
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        // Add the Bitmap object to the collection.
                        bitmaps.Add(new Bitmap(fileName));
                    }
                    catch (Exception)
                    {
                        // TODO: A complete application might handle the error.
                    }
                }
            }
            return bitmaps;
        }

        // Creates a composite bitmap from the provided collection of Bitmap objects.
        // This method computes the average color of each pixel among all bitmaps
        // to create the composite image.
        Bitmap CreateCompositeBitmap(IEnumerable<Bitmap> bitmaps)
        {
            Bitmap[] bitmapArray = bitmaps.ToArray();

            // Compute the maximum width and height components of all 
            // bitmaps in the collection.
            Rectangle largest = new Rectangle();
            foreach (var bitmap in bitmapArray)
            {
                if (bitmap.Width > largest.Width)
                    largest.Width = bitmap.Width;
                if (bitmap.Height > largest.Height)
                    largest.Height = bitmap.Height;
            }

            // Create a 32-bit Bitmap object with the greatest dimensions.
            Bitmap result = new Bitmap(largest.Width, largest.Height,
                PixelFormat.Format32bppArgb);

            // Lock the result Bitmap.
            var resultBitmapData = result.LockBits(
                new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly,
                result.PixelFormat);

            // Lock each source bitmap to create a parallel list of BitmapData objects.
            var bitmapDataList = (from bitmap in bitmapArray
                    select bitmap.LockBits(
                        new Rectangle(new Point(), bitmap.Size),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb))
                .ToList();

            // Compute each column in parallel.
            Parallel.For(0, largest.Width, new ParallelOptions
                {
                    CancellationToken = cancellationTokenSource.Token
                },
                i =>
                {
                    // Compute each row.
                    for (int j = 0; j < largest.Height; j++)
                    {
                        // Counts the number of bitmaps whose dimensions
                        // contain the current location.
                        int count = 0;

                        // The sum of all alpha, red, green, and blue components.
                        int a = 0, r = 0, g = 0, b = 0;

                        // For each bitmap, compute the sum of all color components.
                        foreach (var bitmapData in bitmapDataList)
                        {
                            // Ensure that we stay within the bounds of the image.
                            if (bitmapData.Width > i && bitmapData.Height > j)
                            {
                                unsafe
                                {
                                    byte* row = (byte*)(bitmapData.Scan0 + (j * bitmapData.Stride));
                                    byte* pix = (byte*)(row + (4 * i));
                                    a += *pix; pix++;
                                    r += *pix; pix++;
                                    g += *pix; pix++;
                                    b += *pix;
                                }
                                count++;
                            }
                        }

                        unsafe
                        {
                            // Compute the average of each color component.
                            a /= count;
                            r /= count;
                            g /= count;
                            b /= count;

                            // Set the result pixel.
                            byte* row = (byte*)(resultBitmapData.Scan0 + (j * resultBitmapData.Stride));
                            byte* pix = (byte*)(row + (4 * i));
                            *pix = (byte)a; pix++;
                            *pix = (byte)r; pix++;
                            *pix = (byte)g; pix++;
                            *pix = (byte)b;
                        }
                    }
                });

            // Unlock the source bitmaps.
            for (int i = 0; i < bitmapArray.Length; i++)
            {
                bitmapArray[i].UnlockBits(bitmapDataList[i]);
            }

            // Unlock the result bitmap.
            result.UnlockBits(resultBitmapData);

            // Return the result.
            return result;
        }
    }
}
