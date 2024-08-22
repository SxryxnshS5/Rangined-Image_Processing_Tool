using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

                    // Retain the alpha (transparency) channel
                    int alpha = originalColor.A;

                    // Calculate the grayscale value
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);

                    // Create the grayscale color while keeping the original alpha channel
                    Color grayColor = Color.FromArgb(alpha, grayValue, grayValue, grayValue);
                    grayBitmap.SetPixel(x, y, grayColor);
                }
            }

            return grayBitmap;
        }

    }
}