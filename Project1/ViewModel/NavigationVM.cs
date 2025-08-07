using UII.Command;
using UII.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UII.ViewModel
{
    public class NavigationVM : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand TeachingCommand { get; set; }
        public ICommand InterfaceCommand { get; set; }
        public ICommand DataCommand { get; set; }
        public ICommand HistoryCommand { get; set; }
        public ICommand ShutdownCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Teaching(object obj) => CurrentView = new TeachingVM();
        private void Interface(object obj) => CurrentView = new InterfaceVM();
        private void Data(object obj) => CurrentView = new DataVM();
        private void History(object obj) => CurrentView = new HistoryVM();
        private void Shutdown(object obj)
        {
            MessageBoxResult ret = MessageBox.Show("Bạn có muốn thoát chương trình?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ret == MessageBoxResult.Yes)
                Environment.Exit(0);
        }

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            TeachingCommand = new RelayCommand(Teaching);
            InterfaceCommand = new RelayCommand(Interface);
            DataCommand = new RelayCommand(Data);
            HistoryCommand = new RelayCommand(History);
            ShutdownCommand = new RelayCommand(Shutdown);

            CurrentView = new TeachingVM();

        }
    }
}
