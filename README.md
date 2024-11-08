

---

# Rangined Image Processing Tool

**Rangined Image Processing Tool** is a versatile software designed to facilitate various image processing tasks such as loading, saving, grayscale conversion, and batch image compression into a ZIP archive. Built using C# and XAML, it offers a simple yet powerful interface for managing and manipulating images.

## Latest Release Highlights (v1.1.1)

The latest release, **v1.1.1**, brings substantial performance improvements and new features:

- **Multithreading**: Images are now processed in parallel during batch operations, significantly reducing processing time.
- **Optimized Pixel Manipulation**: Grayscale conversion is faster than ever, thanks to `Bitmap.LockBits` and direct memory access.
- **Conversion Speed**: Processing speed has improved by **98.2%**—batch converting 100 images now takes just 0.18 seconds (down from 9.96 seconds).
- **Conversion Time Display**: Each conversion now displays the time taken, giving clear insights into processing performance.

## Features

- **Load and Save Images**: Easily load image files into the tool and save them back in different formats.
- **Grayscale Conversion**: Convert colored images into grayscale using a fast and efficient algorithm.
- **Batch Compression**: Compress multiple images into a ZIP file for easier storage and sharing.
  
## Upcoming Improvements

The next steps for this project will focus on:

- **Enhanced UI/UX**: Improving the interface for a more intuitive and visually appealing user experience.
- **Advanced Image Modifications**: Adding functionalities such as brightness adjustment, saturation control, and other image enhancement tools.

## Installation

1. Download the executable file from the latest release page.
2. Run the executable directly on your system.

## Usage

### Load an Image

To load an image, use the `LoadImage()` method:

```csharp
var imageProcessingService = new ImageProcessingService();
Bitmap image = imageProcessingService.LoadImage("path/to/image.png");
```

### Save an Image

To save a loaded or processed image, use the `SaveImage()` method:

```csharp
imageProcessingService.SaveImage(image, "path/to/output.png", ImageFormat.Png);
```

### Convert Image to Grayscale

You can convert any image to grayscale using the `ConvertToGrayscale()` method:

```csharp
Bitmap grayscaleImage = imageProcessingService.ConvertToGrayscale(image);
```

### Save Multiple Images as ZIP

To save multiple images into a ZIP file, use the `SaveImagesAsZip()` method:

```csharp
List<Bitmap> images = new List<Bitmap> { image1, image2, image3 };
imageProcessingService.SaveImagesAsZip(images, "path/to/output.zip");
```

## Dependencies

- **.NET Framework or .NET Core**: Required for running C# applications.
- **System.Drawing**: For handling image operations.
- **System.IO.Compression**: For ZIP file compression.

## Contributing

As a solo developer, I'm open to any feedback, suggestions, or bug reports! Feel free to open an issue or submit a pull request if you'd like to contribute.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.

## Contact

For inquiries or suggestions, please open an issue on the GitHub repository.

---
