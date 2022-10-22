using DiskObserver.Model.Interface;
using DiskObserver.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DiskObserver.View {
    /// <summary>
    /// Interaction logic for DisksViewer.xaml
    /// </summary>
    public partial class DisksViewer : UserControl {
        public DisksViewer() {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.DataContext is DiskObserverVM diskObserverVM
                  && e.AddedItems.Count > 0 && e.AddedItems[0] is IPhysicalObject physicalObject) {
                diskObserverVM.DisplayPhysicalObject(physicalObject);
            }
        }
    }
}
