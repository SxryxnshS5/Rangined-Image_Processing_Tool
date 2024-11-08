using ImageProcessingTool.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ImageProcessingTool.Helpers;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace ImageProcessingTool.ViewModels {

    public class MainViewModel : INotifyPropertyChanged {
        private readonly ImageProcessingService imageProcessingService;
        private ObservableCollection<Bitmap> images;
        private Bitmap currentImage;
        private BitmapImage displayedImage;
        private string currentFileName;
        private string notificationMessage;

        public ICommand LoadImagesCommand { get; }
        public ICommand SaveImagesCommand { get; }
        public ICommand ConvertToGrayscaleCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }

        public MainViewModel() {
            imageProcessingService = new ImageProcessingService();
            images = new ObservableCollection<Bitmap>();

            LoadImagesCommand = new RelayCommand(LoadImages);
            SaveImagesCommand = new RelayCommand(SaveImages, CanSaveImages);
            ConvertToGrayscaleCommand = new RelayCommand(ConvertToGrayscale, CanConvertToGrayscale);
            PreviousImageCommand = new RelayCommand(ShowPreviousImage, CanShowPreviousImage);
            NextImageCommand = new RelayCommand(ShowNextImage, CanShowNextImage);
        }

        public BitmapImage DisplayedImage {
            get => displayedImage;
            set {
                displayedImage = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFileName {
            get => currentFileName;
            set {
                currentFileName = value;
                OnPropertyChanged();
            }
        }

        public string SaveButtonText {
            get => images.Count > 1 ? "Save Images" : "Save Image";
        }

        public string NotificationMessage {
            get => notificationMessage;
            set {
                notificationMessage = value;
                OnPropertyChanged();
            }
        }

        private void LoadImages() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Multiselect = true // Allow multiple file selection
            };

            if (openFileDialog.ShowDialog() == true) {
                images.Clear();
                foreach (var filePath in openFileDialog.FileNames) {
                    var bitmap = imageProcessingService.LoadImage(filePath);
                    images.Add(bitmap);
                }
                UpdateCurrentImage();
                ((RelayCommand)SaveImagesCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ConvertToGrayscaleCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
                ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(SaveButtonText));

                // Update notification message
                NotificationMessage = $"{images.Count} image(s) loaded successfully.";
            }
        }

        private void SaveImages() {
            if (images.Count == 0)
                return;

            if (images.Count == 1) {
                SaveSingleImage();
            }
            else {
                SaveMultipleImages();
            }

            // Update notification message
            NotificationMessage = $"{images.Count} image(s) saved successfully.";
        }

        private void SaveSingleImage() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == true) {
                imageProcessingService.SaveImage(currentImage, saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveMultipleImages() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "ZIP Archive|*.zip"
            };

            if (saveFileDialog.ShowDialog() == true) {
                imageProcessingService.SaveImagesAsZip(images, saveFileDialog.FileName);
            }
        }

        private void ConvertToGrayscale() {
            if (images.Count == 0)
                return;

            // Start timing
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < images.Count; i++) {
                images[i] = imageProcessingService.ConvertToGrayscale(images[i]);
            }
            UpdateCurrentImage();

            // Stop timing
            stopwatch.Stop();

            // Update properties with timing and notification
            NotificationMessage = $"Converted {images.Count} image(s) to grayscale in {stopwatch.Elapsed.TotalSeconds:F2} seconds.";
        }

        private void UpdateCurrentImage() {
            if (images.Count > 0) {
                currentImage = images[0];
                DisplayedImage = ConvertBitmapToBitmapImage(currentImage);
                CurrentFileName = $"Image 1"; // Update as needed to reflect current file name
            }
        }

        private void ShowPreviousImage() {
            if (images.Count == 0)
                return;

            int currentIndex = images.IndexOf(currentImage);
            if (currentIndex > 0) {
                currentImage = images[currentIndex - 1];
                DisplayedImage = ConvertBitmapToBitmapImage(currentImage);
                CurrentFileName = $"Image {currentIndex}"; // Update as needed to reflect current file name
            }
            ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
        }

        private void ShowNextImage() {
            if (images.Count == 0)
                return;

            int currentIndex = images.IndexOf(currentImage);
            if (currentIndex < images.Count - 1) {
                currentImage = images[currentIndex + 1];
                DisplayedImage = ConvertBitmapToBitmapImage(currentImage);
                CurrentFileName = $"Image {currentIndex + 2}"; // Update as needed to reflect current file name
            }
            ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
        }

        private bool CanSaveImages() {
            return images.Count > 0;
        }

        private bool CanConvertToGrayscale() {
            return images.Count > 0;
        }

        private bool CanShowPreviousImage() {
            return images.IndexOf(currentImage) > 0;
        }

        private bool CanShowNextImage() {
            return images.IndexOf(currentImage) < images.Count - 1;
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap) {
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
