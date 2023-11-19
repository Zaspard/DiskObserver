using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Reactive;
using Avalonia.Threading;
using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using DiskObserver.ViewModels;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace DiskObserver.View {
    public partial class FilesViewer : UserControl {

        public static readonly StyledProperty<ObservableCollection<IPhysicalObject>> ItemsSourceProperty
                             = AvaloniaProperty.Register<FilesViewer, ObservableCollection<IPhysicalObject>>(nameof(ItemsSource), defaultBindingMode: BindingMode.TwoWay);

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

        public ObservableCollection<IPhysicalObject> ItemsSource {
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
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<ObservableCollection<IPhysicalObject>>>((e) => ItemsSourcePropertyChanged(e)));

            SortModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<SortMode>>((e) => ViewPropertyChanged(e.Sender as FilesViewer)));

            GroupModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<GroupMode>>((e) => ViewPropertyChanged(e.Sender as FilesViewer)));

            ViewModeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<ViewMode>>((e) => ViewPropertyChanged(e.Sender as FilesViewer)));

            IsSortAscendingProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>((e) => ViewPropertyChanged(e.Sender as FilesViewer)));

            IsGroupAscendingProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>((e) => ViewPropertyChanged(e.Sender as FilesViewer)));
        }

        public FilesViewer() {
            InitializeComponent();
        }

        DiskObserverVM? ViewModel => DataContext as DiskObserverVM;
        private void Copy_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (listBox.SelectedItems == null || listBox.SelectedItems.Count <= 0)
                return;

            var list = new List<IPhysicalObject>();
            foreach (var item in listBox.SelectedItems) {

                if (item is IPhysicalObject physicalObject)
                    list.Add(physicalObject);
            }
            ViewModel.Copy(list);
        }

        private void Cut_PointerReleased(object? sender, PointerReleasedEventArgs e) {
            if (ViewModel == null)
                return;

            if (listBox.SelectedItems == null || listBox.SelectedItems.Count <= 0)
                return;

            var list = new List<IPhysicalObject>();
            foreach (var item in listBox.SelectedItems) {

                if (item is IPhysicalObject physicalObject)
                    list.Add(physicalObject);
            }
            ViewModel.Cut(list);
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

            if (listBox.SelectedItems == null || listBox.SelectedItems.Count <= 0)
                return;

            foreach (var item in listBox.SelectedItems) {

                if (item is IPhysicalObject physicalObject)
                    ViewModel.DeleteItem(physicalObject);
            }
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
        private void Grid_DoubleTapped(object? sender, TappedEventArgs e) {
            if (ViewModel == null)
                return;

            if(sender is Grid grid && grid.DataContext is IPhysicalObject physicalObject) {
                ViewModel.DisplayPhysicalObject(physicalObject);
            }
        }

        private void LixtBox_KeyDown(object? sender, KeyEventArgs e) {
            if (ViewModel == null)
                return;

            if (e.Key == Key.Enter && listBox.SelectedItem is IPhysicalObject physicalObject) {

                if (listBox.SelectedItem is ICanRename canRename && canRename.IsRenameMode)
                    return;

                ViewModel.DisplayPhysicalObject(physicalObject);
            }
        }


        static void ItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs<ObservableCollection<IPhysicalObject>> e) {

            if (e.Sender is FilesViewer filesViewer) {

                if (e.OldValue.Value != null) {
                    e.OldValue.Value.CollectionChanged -= filesViewer.Value_CollectionChanged;
                }

                if (e.NewValue.Value != null) {
                    e.NewValue.Value.CollectionChanged += filesViewer.Value_CollectionChanged;
                }

                filesViewer.RefreshView();
            }
        }

        void Value_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            RefreshView();
        }

        static void ViewPropertyChanged(FilesViewer? filesViewer) {

            if (filesViewer == null)
                return;

            filesViewer.RefreshView();
        }


        List<IPhysicalObject>? GetCollection() {

            if (ItemsSource == null || ItemsSource.Count() <= 0)
                return null;

            Dictionary<string, List<IPhysicalObject>> group = new();
            if (GroupMode != GroupMode.None) {

                foreach (var item in ItemsSource) {

                    string key = "";
                    if (GroupMode == GroupMode.Name) {
                        key = item.Name.First().ToString().ToLower();
                    }
                    else if (GroupMode == GroupMode.LastWrite) {

                        DateTime today = DateTime.Today;

                        var yesterday = today.AddDays(-1);
                        var month = today.Month;
                        var year = today.Year;

                        if (item.LastWrite >= today)
                            key = "today";
                        else if(item.LastWrite >= yesterday)
                            key = "yesterday";
                        else if (item.LastWrite.Month >= month)
                            key = "month";
                        else if (item.LastWrite.Year >= year)
                            key = "year";
                        else
                            key = "other";

                    }
                    else if (GroupMode == GroupMode.Type) {
                        key = item.Type;
                    }
                    else if (GroupMode == GroupMode.Size) {

                        if (item is IDirectory)
                            key = "";
                        else {
                             
                            if (item.Size < 1024) {
                                key = $"Byte";
                                goto endif;
                            }

                            double kbytes = item.Size / 1024.0;
                            if (kbytes < 1024) {
                                key = $"KB";
                                goto endif;
                            }

                            double mbytes = kbytes / 1024.0;
                            if (mbytes < 1024) {
                                key = $"MB";
                                goto endif;
                            }

                            double gbytes = mbytes / 1024.0;
                            if (gbytes < 1024) {
                                key = $"GB";
                                goto endif;
                            }

                            double tbytes = gbytes / 1024.0;
                            key = $"TB";
                        }

                    endif:
                        ;
                    }

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
            Dispatcher.UIThread.Invoke(new Action(() => {
                listBox.ItemsSource = null;
                listBox.ItemTemplate = CreateDataTypeSelector();
                listBox.ItemsSource = GetCollection();
            }));
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
