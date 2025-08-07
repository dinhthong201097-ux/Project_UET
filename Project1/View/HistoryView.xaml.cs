using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UII.View
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {
        #region Fields
        private string _lastDir;
        private string _startupPath;


        #endregion


        public HistoryView()
        {
            InitializeComponent();
            _startupPath = AppDomain.CurrentDomain.BaseDirectory;

            this.Loaded += (s, e) =>
            {
                OpenFolder(_startupPath);
            };
        }

        #region Methods
        private void OpenFolder(string path)
        {
            _lastDir = path;
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                lsbFileBrowser.Items.Clear();
                lsbFileBrowser.Items.Add(AddListBoxItem("...", true));
                foreach (string dir in dirs)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    lsbFileBrowser.Items.Add(AddListBoxItem(dirInfo.Name, true));
                }
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension == ".txt" || fileInfo.Extension == ".cvs" || fileInfo.Extension == ".png" || fileInfo.Extension == ".jpg" || fileInfo.Extension == ".bmp")
                        lsbFileBrowser.Items.Add(AddListBoxItem(fileInfo.Name, false));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private ListBoxItem AddListBoxItem(string text, bool isFolder)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            Image img = new Image();
            img.Source = isFolder ? LoadImage("Images/folder.png") : LoadImage("Images/file.png");
            img.Width = 20;
            img.Height = 20;

            panel.Children.Add(img);
            panel.Children.Add(new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Left,
            });
            ListBoxItem item = new ListBoxItem();
            item.Content = panel;
            return item;
        }
        public static ImageSource LoadImage(string relativePath)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = File.OpenRead(relativePath);
            bmp.EndInit();

            return bmp;
        }
        #endregion

        #region Events
        private void lsbFileBrowser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lsbFileBrowser.SelectedItem is ListBoxItem lsb)
            {
                StackPanel pnl = (StackPanel)lsb.Content;
                string tmp = pnl.Children.OfType<TextBlock>().FirstOrDefault().Text;
                if (tmp == "...")
                {
                    if (_lastDir != null)
                    {
                        string parentDir = Directory.GetParent(_lastDir)?.FullName;
                        if (parentDir != null)
                        {
                            OpenFolder(parentDir);
                            _lastDir = parentDir;
                        }
                    }
                    return;
                }
                string nextDir;
                if (_lastDir.EndsWith("\\"))
                    nextDir = _lastDir + tmp;
                else
                    nextDir = _lastDir + "\\" + tmp;

                if (Directory.Exists(nextDir))
                {
                    OpenFolder(nextDir);
                    _lastDir = nextDir;
                }
                else if (File.Exists(nextDir))
                {
                    FileInfo fileInfo = new FileInfo(nextDir);
                    if (fileInfo.Extension == ".txt")
                    {
                        using (StreamReader sr = new StreamReader(nextDir))
                        {
                            TextBlockView.Text = sr.ReadToEnd();
                        }
                        TextBlockView.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Process.Start(fileInfo.FullName);
                    }
                }
            }
        }
        private void btnOpenFolderClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string path = _startupPath; ;
            if ((string)btn.Content == "LOG") path = _startupPath + "LOG";
            if ((string)btn.Content == "C:\\") path = "C:\\";
            if ((string)btn.Content == "D:\\") path = "D:\\";

            OpenFolder(path);
        }
        #endregion
    }
}
