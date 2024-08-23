using System.Collections.Generic;
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

        // Convert the image to grayscale
        public Bitmap ConvertToGrayscale(Bitmap bitmap) {
            // Create a grayscale version of the bitmap
            Bitmap grayBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++) {
                for (int x = 0; x < bitmap.Width; x++) {
                    Color originalColor = bitmap.GetPixel(x, y);
                    int alpha = originalColor.A;
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                    Color grayColor = Color.FromArgb(alpha, grayValue, grayValue, grayValue);
                    grayBitmap.SetPixel(x, y, grayColor);
                }
            }

            return grayBitmap;
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
