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
using System.Linq;

namespace ImageProcessingTool.ViewModels {
    public class MainViewModel : INotifyPropertyChanged {
        private readonly ImageProcessingService _imageProcessingService;
        private ObservableCollection<Bitmap> _images;
        private Bitmap _currentImage;
        private BitmapImage _displayedImage;
        private string _currentFileName;
        private string _notificationMessage;

        public ICommand LoadImagesCommand { get; }
        public ICommand SaveImagesCommand { get; }
        public ICommand ConvertToGrayscaleCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }

        public MainViewModel() {
            _imageProcessingService = new ImageProcessingService();
            _images = new ObservableCollection<Bitmap>();

            LoadImagesCommand = new RelayCommand(LoadImages);
            SaveImagesCommand = new RelayCommand(SaveImages, CanSaveImages);
            ConvertToGrayscaleCommand = new RelayCommand(ConvertToGrayscale, CanConvertToGrayscale);
            PreviousImageCommand = new RelayCommand(ShowPreviousImage, CanShowPreviousImage);
            NextImageCommand = new RelayCommand(ShowNextImage, CanShowNextImage);
        }

        public BitmapImage DisplayedImage {
            get => _displayedImage;
            set {
                _displayedImage = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFileName {
            get => _currentFileName;
            set {
                _currentFileName = value;
                OnPropertyChanged();
            }
        }

        public string SaveButtonText {
            get => _images.Count > 1 ? "Save Images" : "Save Image";
        }

        public string NotificationMessage {
            get => _notificationMessage;
            set {
                _notificationMessage = value;
                OnPropertyChanged();
            }
        }

        private void LoadImages() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Multiselect = true // Allow multiple file selection
            };

            if (openFileDialog.ShowDialog() == true) {
                _images.Clear();
                foreach (var filePath in openFileDialog.FileNames) {
                    var bitmap = _imageProcessingService.LoadImage(filePath);
                    _images.Add(bitmap);
                }
                UpdateCurrentImage();
                ((RelayCommand)SaveImagesCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ConvertToGrayscaleCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
                ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(SaveButtonText));

                // Update notification message
                NotificationMessage = $"{_images.Count} image(s) loaded successfully.";
            }
        }

        private void SaveImages() {
            if (_images.Count == 0)
                return;

            if (_images.Count == 1) {
                SaveSingleImage();
            }
            else {
                SaveMultipleImages();
            }

            // Update notification message
            NotificationMessage = $"{_images.Count} image(s) saved successfully.";
        }

        private void SaveSingleImage() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == true) {
                _imageProcessingService.SaveImage(_currentImage, saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SaveMultipleImages() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "ZIP Archive|*.zip"
            };

            if (saveFileDialog.ShowDialog() == true) {
                _imageProcessingService.SaveImagesAsZip(_images, saveFileDialog.FileName);
            }
        }

        private void ConvertToGrayscale() {
            if (_images.Count == 0)
                return;

            for (int i = 0; i < _images.Count; i++) {
                _images[i] = _imageProcessingService.ConvertToGrayscale(_images[i]);
            }
            UpdateCurrentImage();

            // Update notification message
            NotificationMessage = $"Converted {_images.Count} image(s) to grayscale.";
        }

        private void UpdateCurrentImage() {
            if (_images.Count > 0) {
                _currentImage = _images[0];
                DisplayedImage = ConvertBitmapToBitmapImage(_currentImage);
                CurrentFileName = $"Image 1"; // Update as needed to reflect current file name
            }
        }

        private void ShowPreviousImage() {
            if (_images.Count == 0)
                return;

            int currentIndex = _images.IndexOf(_currentImage);
            if (currentIndex > 0) {
                _currentImage = _images[currentIndex - 1];
                DisplayedImage = ConvertBitmapToBitmapImage(_currentImage);
                CurrentFileName = $"Image {currentIndex}"; // Update as needed to reflect current file name
            }
            ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
        }

        private void ShowNextImage() {
            if (_images.Count == 0)
                return;

            int currentIndex = _images.IndexOf(_currentImage);
            if (currentIndex < _images.Count - 1) {
                _currentImage = _images[currentIndex + 1];
                DisplayedImage = ConvertBitmapToBitmapImage(_currentImage);
                CurrentFileName = $"Image {currentIndex + 2}"; // Update as needed to reflect current file name
            }
            ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
        }

        private bool CanSaveImages() {
            return _images.Count > 0;
        }

        private bool CanConvertToGrayscale() {
            return _images.Count > 0;
        }

        private bool CanShowPreviousImage() {
            return _images.IndexOf(_currentImage) > 0;
        }

        private bool CanShowNextImage() {
            return _images.IndexOf(_currentImage) < _images.Count - 1;
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
