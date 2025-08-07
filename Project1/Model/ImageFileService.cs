using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace UII.Model
{
    public class ImageFileService
    {
        private readonly List<string> _imagePaths = new List<string>();
        private int _currentIndex = -1;

        public ImageFileService()
        {
            string imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (Directory.Exists(imageDirectory))
            {
                var files = Directory.GetFiles(imageDirectory, "*.jpg")
                                     .Concat(Directory.GetFiles(imageDirectory, "*.png"));
                _imagePaths.AddRange(files);
            }
            if (_imagePaths.Count > 0)
            {
                _currentIndex = 0;
            }
        }

        public void LoadImages(string[] paths)
        {
            _imagePaths.Clear();
            _imagePaths.AddRange(paths);
            _currentIndex = 0;
        }
        private BitmapImage LoadBitmapImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public BitmapImage GetCurrentImage()
        {
            if (_imagePaths.Count == 0 || _currentIndex < 0) return null;
            return LoadBitmapImage(_imagePaths[_currentIndex]);
        }

        public BitmapImage GetNextImage()
        {
            if (_imagePaths.Count == 0) return null;
            _currentIndex = (_currentIndex + 1) % _imagePaths.Count;
            return LoadBitmapImage(_imagePaths[_currentIndex]);
        }

        public BitmapImage GetPreviousImage()
        {
            if (_imagePaths.Count == 0) return null;
            _currentIndex = (_currentIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
            return LoadBitmapImage(_imagePaths[_currentIndex]);
        }

        public bool HasImages()
        {
            return _imagePaths.Any();
        }
    }
}