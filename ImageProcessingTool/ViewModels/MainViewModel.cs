using ImageProcessingTool.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ImageProcessingTool.Helpers;
using System.Windows.Media.Imaging;

namespace ImageProcessingTool.ViewModels {
    public class MainViewModel : INotifyPropertyChanged {
        private readonly ImageProcessingService _imageProcessingService;
        private Bitmap _bitmap;
        private BitmapImage _displayedImage;
        private string _currentFileName; // Added property

        public ICommand LoadImageCommand { get; }
        public ICommand SaveImageCommand { get; }
        public ICommand ConvertToGrayscaleCommand { get; }

        public MainViewModel() {
            _imageProcessingService = new ImageProcessingService();

            LoadImageCommand = new RelayCommand(LoadImage);
            SaveImageCommand = new RelayCommand(SaveImage, CanSaveImage);
            ConvertToGrayscaleCommand = new RelayCommand(ConvertToGrayscale, CanConvertToGrayscale);
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

        private void LoadImage() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true) {
                _bitmap = _imageProcessingService.LoadImage(openFileDialog.FileName);
                DisplayedImage = ConvertBitmapToBitmapImage(_bitmap);
                CurrentFileName = Path.GetFileName(openFileDialog.FileName); // Set file name

                // Notify the commands that the state has changed
                ((RelayCommand)SaveImageCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ConvertToGrayscaleCommand).RaiseCanExecuteChanged();
            }
        }

        private void SaveImage() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == true) {
                var format = saveFileDialog.FilterIndex == 1 ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg;
                _imageProcessingService.SaveImage(_bitmap, saveFileDialog.FileName, format);
            }
        }

        private void ConvertToGrayscale() {
            _bitmap = _imageProcessingService.ConvertToGrayscale(_bitmap);
            DisplayedImage = ConvertBitmapToBitmapImage(_bitmap);
        }

        private bool CanSaveImage() {
            return _bitmap != null;
        }

        private bool CanConvertToGrayscale() {
            return _bitmap != null;
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap) {
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
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
