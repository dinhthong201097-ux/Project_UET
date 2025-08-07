using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace UII.Model
{
    public class ImageFolderService
    {
        private List<string> _imagePaths;
        private int _currentIndex = -1;

        public bool IsFolderLoaded => _imagePaths != null && _imagePaths.Any();

        public void LoadImagesFromFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
                _imagePaths = Directory.GetFiles(folderPath)
                                      .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                      .ToList();
                _currentIndex = -1; 
            }
            else
            {
                _imagePaths = new List<string>();
            }
        }

        public BitmapImage GetNextImage()
        {
            if (!IsFolderLoaded)
            {
                return null;
            }

            _currentIndex++;
            if (_currentIndex >= _imagePaths.Count)
            {
                _currentIndex = 0; 
            }

            return LoadImageFromPath(_imagePaths[_currentIndex]);
        }

        private BitmapImage LoadImageFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}