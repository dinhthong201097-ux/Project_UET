using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UII.Command;
using Microsoft.Win32;
using UII.Model;

namespace UII.ViewModel
{
    public class TeachingVM : BaseViewModel
    {
        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
                ((RelayCommand)NextImageCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousImageCommand).RaiseCanExecuteChanged();

                ((RelayCommand)InspectProductCommand).RaiseCanExecuteChanged();
            }
        }

        private double _roiX;
        public double RoiX
        {
            get => _roiX;
            set
            {
                _roiX = value;
                OnPropertyChanged();
            }
        }

        private double _roiY;
        public double RoiY
        {
            get => _roiY;
            set
            {
                _roiY = value;
                OnPropertyChanged();
            }
        }

        private double _roiWidth;
        public double RoiWidth
        {
            get => _roiWidth;
            set
            {
                _roiWidth = value;
                OnPropertyChanged();
            }
        }

        private double _roiHeight;
        public double RoiHeight
        {
            get => _roiHeight;
            set
            {
                _roiHeight = value;
                OnPropertyChanged();
            }
        }

        private string _inspectionResult;
        public string InspectionResult
        {
            get => _inspectionResult;
            set
            {
                _inspectionResult = value;
                OnPropertyChanged();
            }
        }


        public ICommand OpenImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand InspectProductCommand { get; }
        public ICommand SaveSettingsCommand { get; }


        private readonly ImageFileService _imageFileService;
        private readonly InspectionService _inspectionService;


        public TeachingVM()
        {
            _imageFileService = new ImageFileService();
            _inspectionService = new InspectionService();

            OpenImageCommand = new RelayCommand(OpenImage);
            NextImageCommand = new RelayCommand(NextImage, CanNavigate);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanNavigate);
            InspectProductCommand = new RelayCommand(InspectProduct, CanInspect);
            RoiWidth = 150;
            RoiHeight = 50;
            //SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void OpenImage(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string[] filePaths = openFileDialog.FileNames;
                if (filePaths.Length > 0)
                {
                    _imageFileService.LoadImages(filePaths);

                    CurrentImage = _imageFileService.GetCurrentImage();

                }
            }

        }

        private void NextImage(object parameter)
        {
            CurrentImage = _imageFileService.GetNextImage();
        }

        private void PreviousImage(object parameter)
        {
            CurrentImage = _imageFileService.GetPreviousImage();
        }

        private bool CanNavigate(object parameter)
        {
            return _imageFileService.HasImages();
        }

        private void InspectProduct(object parameter)
        {
            // Tạo một đối tượng Rect từ các thuộc tính ROI đã được Binding
            var roiRect = new OpenCvSharp.Rect(
                (int)RoiX,
                (int)RoiY,
                (int)RoiWidth,
                (int)RoiHeight
            );

            // THAY ĐỔI: Gọi phương thức PerformInspection với tham số ROI
            var visualResult = _inspectionService.PerformInspection(CurrentImage, roiRect);

            if (visualResult.ResultImage != null)
            {
                CurrentImage = visualResult.ResultImage;
            }

            InspectionResult = $"Đếm được: {visualResult.Result.SpotCount} đốm tròn\nPhán đoán: {visualResult.Result.Judgment}";
        }

        private bool CanInspect(object parameter)
        {
            return CurrentImage != null;
        }
    }
}
