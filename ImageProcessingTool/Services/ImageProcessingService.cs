using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

namespace ImageProcessingTool.Services {
    public class ImageProcessingService {
        // Load an image file and return a Bitmap
        public Bitmap LoadImage(string filePath) {
            return new Bitmap(filePath);
        }

        // Save a Bitmap image to a file
        public void SaveImage(Bitmap bitmap, string outputPath, ImageFormat format) {
            bitmap.Save(outputPath, format);
        }

        // Convert the image to grayscale using faster pixel manipulation with LockBits
        public Bitmap ConvertToGrayscale(Bitmap bitmap) {
            Bitmap grayBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData grayData = grayBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = Math.Abs(data.Stride) * bitmap.Height;
            byte[] buffer = new byte[bytes];
            byte[] grayBuffer = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, buffer, 0, bytes);

            for (int i = 0; i < buffer.Length; i += 4) {
                byte alpha = buffer[i + 3];
                byte grayValue = (byte)(buffer[i + 2] * 0.3 + buffer[i + 1] * 0.59 + buffer[i] * 0.11);

                grayBuffer[i] = grayValue;
                grayBuffer[i + 1] = grayValue;
                grayBuffer[i + 2] = grayValue;
                grayBuffer[i + 3] = alpha;
            }

            System.Runtime.InteropServices.Marshal.Copy(grayBuffer, 0, grayData.Scan0, bytes);
            bitmap.UnlockBits(data);
            grayBitmap.UnlockBits(grayData);

            return grayBitmap;
        }

        // Convert and save multiple images in parallel
        public List<Bitmap> ConvertImagesToGrayscale(IEnumerable<string> filePaths) {
            List<Bitmap> grayImages = new List<Bitmap>();

            Parallel.ForEach(filePaths, filePath => {
                Bitmap bitmap = LoadImage(filePath);
                Bitmap grayBitmap = ConvertToGrayscale(bitmap);
                lock (grayImages)  // Protect shared resource
                {
                    grayImages.Add(grayBitmap);
                }
            });

            return grayImages;
        }

        // Save images as a ZIP file
        public void SaveImagesAsZip(IEnumerable<Bitmap> images, string zipFilePath) {
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create)) {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create)) {
                    int index = 1;
                    foreach (var image in images) {
                        var fileName = $"Image{index}.png";
                        var entry = archive.CreateEntry(fileName);
                        using (var entryStream = entry.Open()) {
                            using (var bitmapStream = new MemoryStream()) {
                                image.Save(bitmapStream, ImageFormat.Png);
                                bitmapStream.Position = 0;
                                bitmapStream.CopyTo(entryStream);
                            }
                        }
                        index++;
                    }
                }
            }
        }
    }
}
