using DiskObserver.ViewModels;
using System.Windows;

namespace DiskObserver {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new DiskObserverVM();
        }
    }
}
