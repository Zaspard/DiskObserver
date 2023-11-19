using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Reactive;
using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using DiskObserver.ViewModels;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskObserver.View {
    public partial class FilesViewer : UserControl {

        public static readonly StyledProperty<IEnumerable<IPhysicalObject>> ItemsSourceProperty
                             = AvaloniaProperty.Register<FilesViewer, IEnumerable<IPhysicalObject>>(nameof(ItemsSource), defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<SortMode> SortModeProperty
                             = AvaloniaProperty.Register<FilesViewer, SortMode>(nameof(SortMode), SortMode.Name, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<GroupMode> GroupModeProperty
                             = AvaloniaProperty.Register<FilesViewer, GroupMode>(nameof(GroupMode), GroupMode.None, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<ViewMode> ViewModeProperty
                             = AvaloniaProperty.Register<FilesViewer, ViewMode>(nameof(ViewMode), ViewMode.Details, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> IsSortAscendingProperty
                     = AvaloniaProperty.Register<FilesViewer, bool>(nameof(IsSortAscending), defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> IsGroupAscendingProperty
                             = AvaloniaProperty.Register<FilesViewer, bool>(nameof(IsGroupAscending), defaultBindingMode: BindingMode.TwoWay);

        public IEnumerable<IPhysicalObject> ItemsSource {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public SortMode SortMode {
            get => GetValue(SortModeProperty);
            set => SetValue(SortModeProperty, value);
        }
        public GroupMode GroupMode {
            get => GetValue(GroupModeProperty);
            set => SetValue(GroupModeProperty, value);
        }

        public ViewMode ViewMode {
            get => GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public bool IsSortAscending {
            get => GetValue(IsSortAscendingProperty);
            set => SetValue(IsSortAscendingProperty, value);
        }

        public bool IsGroupAscending {
            get => GetValue(IsGroupAscendingProperty);
            set => SetValue(IsGroupAscendingProperty, value);
        }


        static FilesViewer() {

            ItemsSourceProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<IEnumerable<IPhysicalObject>>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));

            SortModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<SortMode>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));

            GroupModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<GroupMode>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));

            ViewModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<ViewMode>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));

            IsSortAscendingProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));

            IsGroupAscendingProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>((e) => ItemsSourcePropertyChanged(e.Sender as FilesViewer)));
        }

        public FilesViewer() {
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
                ViewModel.EnableRenameModeInItem(physicalObject);
        }
        private void Delete_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.DataContext is IPhysicalObject physicalObject)
                ViewModel.DeleteItem(physicalObject);
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

        private void ViewMode_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.CommandParameter is string param
                                            && Enum.TryParse(typeof(ViewMode), param, out var res)) {
                ViewModel.ViewMode = (ViewMode)res;
            }
        }        
        
        private void SortMode_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.CommandParameter is string param
                                            && Enum.TryParse(typeof(SortMode), param, out var res)) {
                ViewModel.SortMode = (SortMode)res;
            }
        }

        private void GroupMode_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (sender is MenuItem menuItem && menuItem.CommandParameter is string param
                                            && Enum.TryParse(typeof(GroupMode), param, out var res)) {
                ViewModel.GroupMode = (GroupMode)res;
            }
        }


        static void ItemsSourcePropertyChanged(FilesViewer? filesViewer) {

            if (filesViewer == null)
                return;

            filesViewer.RefreshView();
        }


        List<IPhysicalObject> GetCollection() {

            if (ItemsSource == null || ItemsSource.Count() <= 0)
                return null;

            Dictionary<string, List<IPhysicalObject>> group = new();
            if (GroupMode != GroupMode.None) {

                foreach (var item in ItemsSource) {

                    string key = GroupMode switch {
                        GroupMode.Name => item.Name,
                        GroupMode.LastWrite => item.LastWrite.ToString(),
                        GroupMode.Size => item.Size.ToString(),
                        GroupMode.Type => item.Type,
                        _ => "",
                    };

                    if (!group.ContainsKey(key))
                        group.Add(key, new());

                    group[key].Add(item);
                }
            }
            else
                group.Add("", ItemsSource.ToList());

            var collection = new List<IPhysicalObject>();
            foreach (var keyValuePairs in IsGroupAscending ? group.OrderBy(x => x.Key) : group.OrderBy(x => x.Key).Reverse()) {

                IEnumerable<IPhysicalObject> sortedCollection = SortMode switch {
                    SortMode.Name => IsSortAscending ? keyValuePairs.Value.OrderBy(x => x.Name) : keyValuePairs.Value.OrderBy(x => x.Name).Reverse(),
                    SortMode.LastWrite => IsSortAscending ? keyValuePairs.Value.OrderBy(x => x.LastWrite) : keyValuePairs.Value.OrderBy(x => x.LastWrite).Reverse(),
                    SortMode.Size => IsSortAscending ? keyValuePairs.Value.OrderBy(x => x.Size) : keyValuePairs.Value.OrderBy(x => x.Size).Reverse(),
                    SortMode.Type => IsSortAscending ? keyValuePairs.Value.OrderBy(x => x.Type) : keyValuePairs.Value.OrderBy(x => x.Type).Reverse(),
                    _ => throw new NotImplementedException()
                };

                collection.AddRange(sortedCollection);
            }

            return collection;
        }

        DataTypeSelector CreateDataTypeSelector() {

            List<IDataTemplate> templates = new List<IDataTemplate>();
            foreach (var item in DataTemplates) {
                if (item is DataTypeContainer dataTypeContainer && dataTypeContainer.Name == ViewMode.ToString()) {
                    templates.AddRange(dataTypeContainer.DataTemplates);
                }
            }

            return new DataTypeSelector(templates);
        }

        void RefreshView() {
            listBox.ItemsSource = null;
            listBox.ItemTemplate = CreateDataTypeSelector();
            listBox.ItemsSource = GetCollection();
        }

        private void MenuItem_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e) {
        }
    }

    public sealed class DataTypeSelector : IDataTemplate {

        List<IDataTemplate> _dataTemplates;
        public DataTypeSelector(List<IDataTemplate> dataTemplates)
        {
            _dataTemplates = dataTemplates;
        }

        public Control? Build(object? param) => _dataTemplates?.First(x => x.Match(param)).Build(param);

        public bool Match(object? data) => _dataTemplates?.Any(x => x.Match(data)) ?? false;
    }

    public sealed class DataTypeContainer : IDataTemplate {

        public string Name { get; set; } = "";
        public List<IDataTemplate> DataTemplates { get; set; } = new();

        public Control? Build(object? param) => throw new NotImplementedException();

        public bool Match(object? data) => false;
    }
}
