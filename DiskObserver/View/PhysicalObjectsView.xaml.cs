using DiskObserver.Model.Interface;
using DiskObserver.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiskObserver.View {
    public partial class PhysicalObjectsView : UserControl {
        public PhysicalObjectsView() {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {

                if (this.DataContext is DiskObserverVM diskObserverVM
                                  && sender is FrameworkElement c && c.DataContext is IPhysicalObject physicalObject) {
                    diskObserverVM.DisplayPhysicalObject(physicalObject);
                }

            }
        }

        private void FindAllHevyFiles_Click(object sender, RoutedEventArgs e) {
            if (this.DataContext is DiskObserverVM diskObserverVM
                && sender is Control control
                && control.DataContext is IPhysicalObject physicalObject) {
                diskObserverVM.FindAllHeavyFiles(physicalObject);
            }
        }

        private void OpenInExplorer_Click(object sender, RoutedEventArgs e) {
            if (this.DataContext is DiskObserverVM diskObserverVM
                && sender is Control control
                && control.DataContext is IPhysicalObject physicalObject) {
                diskObserverVM.OpenInExplorer(physicalObject);
            }
        }
    }
}
