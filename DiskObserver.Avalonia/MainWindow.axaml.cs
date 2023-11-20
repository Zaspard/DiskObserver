using Avalonia.Controls;
using DiskObserver.ViewModels;

namespace DiskObserver {
    public partial class MainWindow : Window {
        DiskObserverVM? _diskObserverVM;
        public MainWindow(DiskObserverVM diskObserverVM) {
            _diskObserverVM = diskObserverVM;   
            InitializeComponent();
            DataContext = diskObserverVM;

            Closed += MainWindow_Closed;     
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e) {
            if (_diskObserverVM != null) 
                _diskObserverVM.Dispose();

            _diskObserverVM = null;
        }
    }
}
