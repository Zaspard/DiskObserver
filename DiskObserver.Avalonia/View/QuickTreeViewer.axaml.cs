using Avalonia.Controls;
using Avalonia.Input;
using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.ViewModels;

namespace DiskObserver.Avalonia.View {
    public partial class QuickTreeViewer : UserControl {
        public QuickTreeViewer() {
            InitializeComponent();
        }

        DiskObserverVM? ViewModel => DataContext as DiskObserverVM;
        private void Copy_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.Copy(physicalObject);
        }
        private void Cut_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.Cut(physicalObject);
        }
        private void Paste_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.Paste(physicalObject);
        }
        private void Rename_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.RenameItem(physicalObject);
        }
        private void Properties_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.ShowPropertyItem(physicalObject);
        }
        private void AddToQuickAccess_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.AddPhysicalObjectToQuickAccess(physicalObject);
        }

        private void RemoveFromQuickAccess_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.RemovePhysicalObjectFromQuickAccess(physicalObject);
        }

    }
}
