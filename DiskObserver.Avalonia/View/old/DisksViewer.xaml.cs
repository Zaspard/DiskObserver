using Avalonia.Controls;
using Avalonia.Interactivity;
using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.ViewModels;

namespace DiskObserver.Avalonia.View.old
{
    /// <summary>
    /// Interaction logic for DisksViewer.xaml
    /// </summary>
    //public partial class DisksViewer : UserControl {
    //    public DisksViewer() {
    //        InitializeComponent();
    //    }

    //    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    //        if (this.DataContext is DiskObserverVM diskObserverVM
    //              && e.AddedItems.Count > 0 && e.AddedItems[0] is IPhysicalObject physicalObject) {
    //            diskObserverVM.DisplayPhysicalObject(physicalObject);
    //        }
    //    }

    //    private void FindAllHevyFiles_Click(object sender, RoutedEventArgs e) {
    //        if (this.DataContext is DiskObserverVM diskObserverVM 
    //            && sender is Control control 
    //            && control.DataContext is IPhysicalObject physicalObject) {
    //            diskObserverVM.FindAllHeavyFiles(physicalObject);
    //        }
    //    }
    //}
}
