using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using UII.ViewModel;

namespace UII
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            this.DataContext = new NavigationVM();
        }

        private void btnclick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Background = Brushes.Red;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the status bar with the current time
            lbDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lbTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnLoginClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
