using ImageProcessingTool.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace ImageProcessingTool.ViewModels {
    using ImageProcessingTool.Helpers;

    public class MainViewModel : INotifyPropertyChanged {
        private readonly ImageProcessingService _imageProcessingService;
        private Bitmap _bitmap;

        public ICommand LoadImageCommand { get; }
        public ICommand SaveImageCommand { get; }
        public ICommand ConvertToGrayscaleCommand { get; }

        public MainViewModel() {
            _imageProcessingService = new ImageProcessingService();
            LoadImageCommand = new RelayCommand(LoadImage);
            SaveImageCommand = new RelayCommand(SaveImage, CanSaveImage);
            ConvertToGrayscaleCommand = new RelayCommand(ConvertToGrayscale, CanConvertToGrayscale);
        }

        private void LoadImage() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true) {
                _bitmap = _imageProcessingService.LoadImage(openFileDialog.FileName);
                OnPropertyChanged(nameof(ImageLoaded));
            }
        }

        private void SaveImage() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg"
            };

            if (saveFileDialog.ShowDialog() == true) {
                using (var outputStream = new FileStream(saveFileDialog.FileName, FileMode.Create)) {
                    var format = saveFileDialog.FilterIndex == 1 ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg;
                    _imageProcessingService.SaveImage(_bitmap, saveFileDialog.FileName, format);
                }
            }
        }

        private void ConvertToGrayscale() {
            if (_bitmap != null) {
                _bitmap = _imageProcessingService.ConvertToGrayscale(_bitmap);
                OnPropertyChanged(nameof(ImageLoaded));
            }
        }

        private bool CanSaveImage() => _bitmap != null;
        private bool CanConvertToGrayscale() => _bitmap != null;

        public bool ImageLoaded => _bitmap != null;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
