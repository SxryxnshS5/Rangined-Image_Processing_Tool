# Rangined Image Processing Tool

**Rangined Image Processing Tool** is a versatile software designed to facilitate various image processing tasks such as loading, saving, grayscale conversion, and batch image compression into a ZIP archive. Built using C# and XAML, it offers a simple yet powerful interface for managing and manipulating images. 

The next step for this project will be to enhance processing speed by implementing multi-threading or exploring other optimization techniques.

## Features

- **Load and Save Images**: Easily load image files into the tool and save them back in different formats.
- **Grayscale Conversion**: Convert colored images into grayscale with a fast and efficient algorithm.
- **Batch Compression**: Compress multiple images into a ZIP file for easier storage and sharing.

## Installation

1. Clone the repository from GitHub:
   ```bash
   git clone https://github.com/SxryxnshS5/Rangined-Image_Processing_Tool.git
   ```
2. Open the project in Visual Studio.
3. Build and run the project.

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

- **.NET Framework** or **.NET Core** for running C# applications.
- **System.Drawing** for handling image operations.
- **System.IO.Compression** for ZIP file compression.

## Contributing

We welcome contributions from the community. To contribute:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Open a pull request describing your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact

For any inquiries or suggestions, feel free to open an issue on the [GitHub repository](https://github.com/SxryxnshS5/Rangined-Image_Processing_Tool).

---

**Rangined Image Processing Tool** - Simplifying your image processing tasks with ease.

