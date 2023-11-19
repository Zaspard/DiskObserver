using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;

#nullable disable

namespace DiskObserver.ViewModels {
    public sealed class DiskObserverVM : BaseModel, IDisposable {
        public ReactiveCommand<IPhysicalObject, Unit> DisplayPhysicalObjectCommand => 
                                                        ReactiveCommand.Create((IPhysicalObject physicalObject) => DisplayPhysicalObject(physicalObject));
        public ReactiveCommand<IPhysicalObject, Unit> RenameItemCommand =>
                                                        ReactiveCommand.Create((IPhysicalObject physicalObject) => EnableRenameModeInItem(physicalObject));     
        public ReactiveCommand<string, Unit> TryMoveToPathCommand =>
                                                        ReactiveCommand.Create((string path) => TryMoveToPath(path));
        public ReactiveCommand<Unit, Unit> MoveUpCommand =>
                                                ReactiveCommand.Create(() => DisplayPhysicalObject(DisplayedPhysicalObject?.ParentPhysicalObject));
        public ReactiveCommand<Unit, Unit> MoveBackCommand =>
                                                ReactiveCommand.Create(TryMoveBack);
        public ReactiveCommand<Unit, Unit> MoveForwardCommand =>
                                        ReactiveCommand.Create(TryMoveForward);

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; } = new();
        QuickAccessModel _quickAccessModel;

        private IPhysicalObject _displayedPhysicalObject;
        public IPhysicalObject DisplayedPhysicalObject {
            get => _displayedPhysicalObject;
            set
            {
                _displayedPhysicalObject = value;
                OnPropertyChanged(nameof(DisplayedPhysicalObject));
            }
        }

        private bool _isEnable = true;
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        private SortMode _sortMode = SortMode.Name;
        public SortMode SortMode {
            get => _sortMode;
            set
            {
                _sortMode = value;
                OnPropertyChanged(nameof(SortMode));
            }
        }

        private GroupMode _groupMode = GroupMode.None;
        public GroupMode GroupMode {
            get => _groupMode;
            set
            {
                _groupMode = value;
                OnPropertyChanged(nameof(GroupMode));
            }
        }

        private ViewMode _viewMode = ViewMode.Details;
        public ViewMode ViewMode {
            get => _viewMode;
            set
            {
                _viewMode = value;
                OnPropertyChanged(nameof(ViewMode));
            }
        }

        private bool _isSortAscending = true;
        public bool IsSortAscending {
            get => _isSortAscending;
            set
            {
                _isSortAscending = value;
                OnPropertyChanged(nameof(IsSortAscending));
            }
        }

        private bool _isGroupAscending = true;
        public bool IsGroupAscending {
            get => _isGroupAscending;
            set
            {
                _isGroupAscending = value;
                OnPropertyChanged(nameof(IsGroupAscending));
            }
        }

        private bool _isDisplayedHead = true;
        public bool IsDisplayedHead {
            get => _isDisplayedHead;
            set
            {
                _isDisplayedHead = value;
                OnPropertyChanged(nameof(IsDisplayedHead));
            }
        }

        public DiskObserverVM() {
            _quickAccessModel = new QuickAccessModel();
            PhysicalObjects.Add(_quickAccessModel);

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives) {
                PhysicalObjects.Add(new PhysicalDisk(driveInfo));
            }
        }

        public void Dispose() {
            _quickAccessModel = null;

            if (PhysicalObjects != null) {
                foreach (IPhysicalObject item in PhysicalObjects)
                    item.Dispose();
            }

            PhysicalObjects.Clear();
            Clipboard.Clear();
        }

        public void DisplayPhysicalObject(IPhysicalObject physicalObject, MoveMode moveMode = MoveMode.Normal) {
            if (physicalObject is IFile || physicalObject == null)
                return;


            if (DisplayedPhysicalObject != null && moveMode != MoveMode.Back) {
                _backStack.Push(DisplayedPhysicalObject.Path);
            }

            physicalObject.LazyInit();
            DisplayedPhysicalObject = physicalObject;

            if (moveMode == MoveMode.Normal) {
                _forwardStack.Clear();
            }
        }

        internal void AddPhysicalObjectToQuickAccess(IPhysicalObject item) {
            if (!_quickAccessModel.PhysicalObjects.Contains(item)) {
                _quickAccessModel.PhysicalObjects.Add(item);
            }
        }

        internal void RemovePhysicalObjectFromQuickAccess(IPhysicalObject item) {
            if (_quickAccessModel.PhysicalObjects.Contains(item)) {
                _quickAccessModel.PhysicalObjects.Remove(item);
            }
        }

        internal void EnableRenameModeInItem(IPhysicalObject physicalObject) {
            if (physicalObject is IFile file)
                file.IsRenameMode = true;
            else if (physicalObject is IDirectory directory)
                directory.IsRenameMode = true;
        }

        internal void ShowPropertyItem(IPhysicalObject physicalObject) {
            //TODO: Some shit window?
        }


        List<(bool, string, bool)> Clipboard = new();

        internal void Copy(IPhysicalObject physicalObject) {
            Clipboard.Clear();
            Clipboard.Add((physicalObject is IFile, physicalObject.Path, false));
        }
        internal void Copy(IEnumerable<IPhysicalObject> physicalObjects) {
            Clipboard.Clear();
            foreach (var item in physicalObjects) {
                Clipboard.Add((item is IFile, item.Path, false));
            }
        }
        internal void Cut(IPhysicalObject physicalObject) {
            Clipboard.Clear();
            Clipboard.Add((physicalObject is IFile, physicalObject.Path, true));
        }
        internal void Cut(IEnumerable<IPhysicalObject> physicalObjects) {
            Clipboard.Clear();
            foreach (var item in physicalObjects) {
                Clipboard.Add((item is IFile, item.Path, true));
            }
        }
        internal void Paste(IPhysicalObject physicalObject) {

            if (Clipboard.Count <= 0)
                return;

            var haveFileNotifyer = physicalObject as IHaveFileNotifyer;
            if (haveFileNotifyer != null)
                haveFileNotifyer.IgnoreAllNotify = true;

            foreach ((bool isFile, string path, bool removeFlag) in Clipboard) {

                if (isFile) {

                    string nameItem = Path.GetFileName(path);
                    string destPath = Path.Combine(physicalObject.Path, nameItem);
                    File.Copy(path, destPath, true);

                    FileInfo fileInfo = new FileInfo(destPath);
                    FileModel fileModel = new FileModel(fileInfo, physicalObject);
                    physicalObject.PhysicalObjects!.Add(fileModel);

                    if (removeFlag) {
                        File.Delete(path);
                    }
                }
                else {

                    string nameItem = Path.GetFileName(path);
                    string sourcePath = Path.Combine(physicalObject.Path, nameItem);
                    CopyFilesRecursively(path, sourcePath);

                    var directoryInfo = new DirectoryInfo(sourcePath);  
                    DirectoryModel directoryModel = new DirectoryModel(directoryInfo, physicalObject);
                    physicalObject.PhysicalObjects!.Add(directoryModel);

                    if (removeFlag) {
                        Directory.Delete(path, true);
                    }
                }
            }

            if (haveFileNotifyer != null)
                haveFileNotifyer.IgnoreAllNotify = false;

            Clipboard.Clear();
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath) {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        internal void DeleteItem(IPhysicalObject physicalObject) {
            physicalObject.Delete();
        }

        void TryMoveToPath(string path, MoveMode moveMode = MoveMode.Normal) {
            if (path == DisplayedPhysicalObject?.Path)
                return;

            if (Directory.Exists(path)) {

                var root = Path.GetPathRoot(path);
                IPhysicalObject physicalObject = PhysicalObjects?.FirstOrDefault(x => x.Path == root) as IPhysicalDisk;
                if (physicalObject != null) {

                    var paths = path.Remove(0, root.Length).Split(Path.DirectorySeparatorChar);
                    foreach (var p in paths) {

                        physicalObject = physicalObject.PhysicalObjects?.FirstOrDefault(x => string.Equals(x.Name, p, StringComparison.OrdinalIgnoreCase));
                        if (physicalObject == null)
                            break;
                    }

                    if (physicalObject != null) {
                        DisplayPhysicalObject(physicalObject, moveMode);
                    }
                }
            }
            else if (File.Exists(path)) {
                //TODO: Open?
            }
            else {
                //TODO: Refresh
                OnPropertyChanged(nameof(DisplayedPhysicalObject));
            }
        }

        Stack<string> _backStack = new();
        Stack<string> _forwardStack = new();
        void TryMoveBack() {

            if (_backStack.Count <= 0)
                return;

            string backPath = _backStack.Pop();

            string nowPath = DisplayedPhysicalObject?.Path;
            if (nowPath != null) {
                _forwardStack.Push(nowPath);
            }

            TryMoveToPath(backPath, MoveMode.Back);
        }

        void TryMoveForward() {

            if (_forwardStack.Count <= 0)
                return;

            string forwardPath = _forwardStack.Pop();
            TryMoveToPath(forwardPath, MoveMode.Forward);
        }
    }
}
