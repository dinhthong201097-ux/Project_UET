
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace UII.Model
{
    public class InspectionResult
    {
        public int SpotCount { get; set; }
        public string Judgment { get; set; }
    }

    public class InspectionVisualResult
    {
        public InspectionResult Result { get; set; }
        public BitmapImage ResultImage { get; set; }
    }

    public class InspectionService
    {
        public InspectionVisualResult PerformInspection(BitmapImage bitmapImage, Rect roiRect)
        {
            if (bitmapImage == null)
            {
                var result = new InspectionResult { SpotCount = 0, Judgment = "KHÔNG ĐẠT (Không có ảnh)" };
                return new InspectionVisualResult { Result = result, ResultImage = null };
            }

            int spotsFound = 0;
            string judgment = "ĐẠT";
            BitmapImage resultImage = null;

            try
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    using (var bitmap = new System.Drawing.Bitmap(stream))
                    {
                        using (var mat = bitmap.ToMat())
                        using (var bgrMat = new Mat())
                        using (var resultMat = new Mat())
                        {
                            Cv2.CvtColor(mat, bgrMat, ColorConversionCodes.BGRA2BGR);
                            bgrMat.CopyTo(resultMat);

                            Rect finalRoi = roiRect.Intersect(new Rect(0, 0, bgrMat.Width, bgrMat.Height));

                            Mat processedMat;

                            if (finalRoi.Width > 0 && finalRoi.Height > 0)
                            {
                                processedMat = new Mat(bgrMat, finalRoi);
                            }
                            else
                            {
                                processedMat = bgrMat;
                                finalRoi = new Rect(0, 0, bgrMat.Width, bgrMat.Height);
                            }

                            using (var grayImage = new Mat())
                            using (var binaryImage = new Mat())
                            {
                                Cv2.CvtColor(processedMat, grayImage, ColorConversionCodes.BGR2GRAY);
                                Cv2.Threshold(grayImage, binaryImage, 200, 255, ThresholdTypes.BinaryInv);

                                Cv2.FindContours(binaryImage, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy,
                                                 RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                                foreach (var contour in contours)
                                {
                                    double area = Cv2.ContourArea(contour);
                                    double perimeter = Cv2.ArcLength(contour, true);
                                    if (perimeter > 0)
                                    {
                                        double circularity = 4 * Math.PI * area / (perimeter * perimeter);
                                        if (area > 100 && circularity > 0.5)
                                        {
                                            spotsFound++;

                                            var shiftedContour = new OpenCvSharp.Point[contour.Length];
                                            for (int i = 0; i < contour.Length; i++)
                                            {
                                                shiftedContour[i] = new OpenCvSharp.Point(contour[i].X + finalRoi.X, contour[i].Y + finalRoi.Y);
                                            }
                                            Cv2.DrawContours(resultMat, new[] { shiftedContour }, -1, new Scalar(0, 255, 0), 2);
                                        }
                                    }
                                }
                            }

                            using (var tempBitmap = resultMat.ToBitmap())
                            {
                                using (var outStream = new MemoryStream())
                                {
                                    tempBitmap.Save(outStream, System.Drawing.Imaging.ImageFormat.Bmp);
                                    outStream.Seek(0, SeekOrigin.Begin);
                                    resultImage = new BitmapImage();
                                    resultImage.BeginInit();
                                    resultImage.CacheOption = BitmapCacheOption.OnLoad;
                                    resultImage.StreamSource = outStream;
                                    resultImage.EndInit();
                                    resultImage.Freeze();
                                }
                            }

                            if (processedMat != bgrMat)
                            {
                                processedMat.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LỖI TRONG INSPECTION: {ex.Message}");
                judgment = $"LỖI: {ex.Message}";
            }

            if (spotsFound == 2 || spotsFound == 3)
            {
                judgment = "ĐẠT";
            }
            else
            {
                judgment = "KHÔNG ĐẠT";
            }

            var inspectionResult = new InspectionResult
            {
                SpotCount = spotsFound,
                Judgment = judgment
            };

            return new InspectionVisualResult
            {
                Result = inspectionResult,
                ResultImage = resultImage
            };
        }
    }
}