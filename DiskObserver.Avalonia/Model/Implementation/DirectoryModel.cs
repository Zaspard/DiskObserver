using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

#nullable enable

namespace DiskObserver.Model.Implementation {
    public class DirectoryModel : BaseModel, IDirectory, IHaveFileNotifyer {
        public bool IsVisibleInTree => true;
        public IPhysicalObject? ParentPhysicalObject { get; private set; }
        public ObservableCollection<IPhysicalObject>? _physicalObjects { get; set; }
        public ObservableCollection<IPhysicalObject>? PhysicalObjects {
            get
            {
                if (_physicalObjects == null) {
                    _physicalObjects = new();
                    LazyInit();
                }

                return _physicalObjects;
            }
        }


        private string _name = "";
        public string Name {
            get => _name;
            set
            {
                if (_name == value || (_isRenameMode && !TryRename(value))) {
                    IsRenameMode = false;
                    return;
                }

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            private set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private bool _isRenameMode;
        public bool IsRenameMode {
            get => _isRenameMode;
            set
            {
                _isRenameMode = value;
                OnPropertyChanged(nameof(IsRenameMode));
            }
        }

        private string _type = "Folder";
        public string Type {
            get => _type;
            private set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        private long _size = 0;
        public long Size {
            get => _size;
            private set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        private DateTime _lastWrite;
        public DateTime LastWrite {
            get => _lastWrite;
            private set
            {
                _lastWrite = value;
                OnPropertyChanged(nameof(LastWrite));
            }
        }

        private string _path = "";
        public string Path {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public DirectoryModel(DirectoryInfo aDirectoryInfo, IPhysicalObject _parentObject) {
            ParentPhysicalObject = _parentObject;
            ChangePath(aDirectoryInfo.FullName);

            Name = aDirectoryInfo.Name;
            IsVisible = !aDirectoryInfo.Attributes.HasFlag(FileAttributes.Hidden);

            LastWrite = aDirectoryInfo.CreationTime;
        }

        public void Dispose() {
            ParentPhysicalObject = null;

            DisposeSubscriber();

            if (_physicalObjects != null && _physicalObjects.Count > 0) {

                foreach (var item in _physicalObjects.ToList())
                    item.Dispose();

                _physicalObjects.Clear();
            }
        }


        bool _inited = false;
        public void LazyInit() {

            if (_inited)
                return;

            var aDirectoryInfo = new DirectoryInfo(_path);

            DirectoryInfo[]? directoryInfos = null;
            try {
                directoryInfos = aDirectoryInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex) {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }


            if (directoryInfos != null) {

                foreach (DirectoryInfo directoryInfo in directoryInfos) {
                    DirectoryModel directoryModel = new DirectoryModel(directoryInfo, this);
                    PhysicalObjects!.Add(directoryModel);
                }
            }

            FileInfo[]? fileInfos = null;
            try {
                fileInfos = aDirectoryInfo.GetFiles();
            }
            catch (UnauthorizedAccessException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex) {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }

            if (fileInfos != null) {
                foreach (FileInfo fileInfo in fileInfos) {
                    FileModel fileModel = new FileModel(fileInfo, this);
                    PhysicalObjects!.Add(fileModel);
                }
            }

            _inited = true;
        }

        public void Delete() => Directory.Delete(Path, true);

        public void GetHeavyFiles(List<IFile> aHeavyFiles, int aMaxCount) {
            if (!_inited)
                LazyInit();
            
            foreach (IPhysicalObject physicalObject in PhysicalObjects!) {
                physicalObject.GetHeavyFiles(aHeavyFiles, aMaxCount);
            }
        }

        bool TryRename(string newName) {

            if (ParentPhysicalObject == null || ParentPhysicalObject.PhysicalObjects == null)
                return false;

            foreach (var item in ParentPhysicalObject.PhysicalObjects) {

                if (item == this)
                    continue;

                if (string.Equals(item.Name, newName, StringComparison.OrdinalIgnoreCase)) {
                    IsRenameMode = false;
                    return false;
                }
            }

            string newPath = Path.Remove(Path.Length - Name.Length) + newName;

            IgnoreAllNotify = true;
            try {
                var aDirectoryInfo = new DirectoryInfo(Path);
                aDirectoryInfo.MoveTo(newPath);
            }
            catch {
                IsRenameMode = false;
                return false;
            }
            IgnoreAllNotify = false;

            ChangePath(newPath);
            RefreshPathInInnerItems(this, newPath);

            IsRenameMode = false;
            return true;
        }

        static void RefreshPathInInnerItems(IDirectory directory, string newPath) {
            if (directory.PhysicalObjects != null) {
                foreach (var item in directory.PhysicalObjects) {

                    string path = System.IO.Path.Combine(newPath, item.Name);
                    item.ChangePath(path);

                    if (item is IDirectory directoryItem)
                        RefreshPathInInnerItems(directoryItem, path);
                }
            }
        }

        
        public void ChangePath(string path) {
            DisposeSubscriber();

            Path = path;

            TrySubcribe();
        }


        public bool IgnoreAllNotify { get; set; }
        FileSystemWatcher? _fileSystemWatcher;
        public void TrySubcribe() {
            try {
                _fileSystemWatcher = new(Path);
                _fileSystemWatcher.Created += SystemWatcher_Created;
                _fileSystemWatcher.Changed += SystemWatcher_Changed;
                _fileSystemWatcher.Deleted += SystemWatcher_Deleted;
                _fileSystemWatcher.Renamed += SystemWatcher_Renamed;
                _fileSystemWatcher.Filter = "*";
                _fileSystemWatcher.IncludeSubdirectories = false;
                _fileSystemWatcher.EnableRaisingEvents = true;
                _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            }
            catch (Exception ex) {
                _ = ex;

                if (_fileSystemWatcher != null) {
                    _fileSystemWatcher.Created -= SystemWatcher_Created;
                    _fileSystemWatcher.Changed -= SystemWatcher_Changed;
                    _fileSystemWatcher.Deleted -= SystemWatcher_Deleted;
                    _fileSystemWatcher.Renamed -= SystemWatcher_Renamed;
                    _fileSystemWatcher.Dispose();
                }
            }
        }

        public void DisposeSubscriber() {
            if (_fileSystemWatcher != null) {
                _fileSystemWatcher.Created -= SystemWatcher_Created;
                _fileSystemWatcher.Changed -= SystemWatcher_Changed;
                _fileSystemWatcher.Deleted -= SystemWatcher_Deleted;
                _fileSystemWatcher.Renamed -= SystemWatcher_Renamed;
                _fileSystemWatcher.Dispose();
            }
        }

        private void SystemWatcher_Created(object sender, FileSystemEventArgs e) {

            if (IgnoreAllNotify)
                return;

            if (System.IO.Path.Exists(e.FullPath)) {

                DirectoryInfo? newDirectory = null;
                try {
                    newDirectory = new DirectoryInfo(e.FullPath);

                    if (!newDirectory.Exists)
                        newDirectory = null;
                }
                catch (UnauthorizedAccessException ex) {
                    _ = ex;
                    Debug.WriteLine(ex.Message);
                }
                catch (DirectoryNotFoundException ex) {
                    _ = ex;
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex) {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }


                if (newDirectory != null) {
                    DirectoryModel directoryModel = new DirectoryModel(newDirectory, this);
                    PhysicalObjects!.Add(directoryModel);
                    return;
                }

                FileInfo? newFile = null;
                try {
                    newFile = new FileInfo(e.FullPath);
                }
                catch (UnauthorizedAccessException ex) {
                    _ = ex;
                    Debug.WriteLine(ex.Message);
                }
                catch (DirectoryNotFoundException ex) {
                    _ = ex;
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex) {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }

                if (newFile != null) {
                    FileModel fileModel = new FileModel(newFile, this);
                    PhysicalObjects!.Add(fileModel);
                }
            }
        }

        private void SystemWatcher_Changed(object sender, FileSystemEventArgs e) {
            if (IgnoreAllNotify || !_inited)
                return;

            var item = PhysicalObjects?.FirstOrDefault(x => x.Name == e.Name);
            if (item is IFile file) {
                file.RefreshProperty();
            }
        }

        private void SystemWatcher_Renamed(object sender, RenamedEventArgs e) {

            if (IgnoreAllNotify)
                return;

            var item = PhysicalObjects?.FirstOrDefault(x => x.Name == e.OldName);
            if (item != null) {

                if (item is IDirectory directory) {

                    directory.IsRenameMode = false;
                    directory.Name = e.Name;
                    directory.ChangePath(e.FullPath);
                    RefreshPathInInnerItems(directory, e.FullPath);
                }
                else if (item is IFile file) {
                    file.Name = e.Name;
                    file.ChangePath(e.FullPath);
                }
            }
        }

        private void SystemWatcher_Deleted(object sender, FileSystemEventArgs e) {

            if (IgnoreAllNotify)
                return;

            var item = PhysicalObjects?.FirstOrDefault(x => x.Name == e.Name);
            if (item != null) {
                item.Dispose();
                PhysicalObjects!.Remove(item);
            }
        }
    }
}
