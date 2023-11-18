using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

#nullable enable

namespace DiskObserver.Avalonia.Model.Implementation {
    public sealed class PhysicalDisk : BaseModel, IPhysicalDisk {
        public bool IsVisibleInTree => true;
        public IPhysicalObject? ParentPhysicalObject { get; private set; } = null;
        ObservableCollection<IPhysicalObject>? _physicalObjects = null;
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

        string _name = "";
        public string Name {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        string _format = "";
        public string Format {
            get => _format;
            private set
            {
                _format = value;
                OnPropertyChanged(nameof(Format));
            }
        }

        private long _totalMemory;
        public long TotalMemory {
            get => _totalMemory;
            private set
            {
                _totalMemory = value;
                OnPropertyChanged(nameof(TotalMemory));
            }
        }

        private long _freeMemory;
        public long FreeMemory {
            get => _freeMemory;
            private set
            {
                _freeMemory = value;
                OnPropertyChanged(nameof(FreeMemory));
            }
        }

        private bool _isNotLoad;
        public bool IsNotReady {
            get => _isNotLoad;
            private set
            {
                _isNotLoad = value;
                OnPropertyChanged(nameof(IsNotReady));
            }
        }

        private string _path = "";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        bool _isExpanded = false;
        public bool IsExpanded {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }


        private DriveInfo _driveInfo;
        public PhysicalDisk(DriveInfo driveInfo) {
            _driveInfo = driveInfo;
            Path = driveInfo.Name;
            Name = driveInfo.Name;
            if (driveInfo.IsReady) {
                Format = driveInfo.DriveFormat;
                TotalMemory = driveInfo.TotalSize;
                FreeMemory = driveInfo.AvailableFreeSpace;
            }

            LazyInit();
        }

        public void Dispose() {
            _driveInfo = null;

            if (_physicalObjects != null && _physicalObjects.Count > 0) {

                foreach (IPhysicalObject physicalObject in _physicalObjects) {
                    physicalObject.Dispose();
                }
                _physicalObjects.Clear();
            }
        }

        public void RefreshProperty() {
            FreeMemory = _driveInfo.AvailableFreeSpace;
        }

        bool _inited = false;
        public void LazyInit() {

            if (_inited || !_driveInfo.IsReady)
                return;

            foreach (DirectoryInfo directoryInfo in _driveInfo.RootDirectory.GetDirectories()) {
                if (!PhysicalObjects.Any(x => x.Path == directoryInfo.FullName)) {
                    PhysicalObjects.Add(new DirectoryModel(directoryInfo, this));
                }
            }

            foreach (FileInfo fileInfo in _driveInfo.RootDirectory.GetFiles()) {
                if (!PhysicalObjects.Any(x => x.Path == fileInfo.FullName)) {
                    PhysicalObjects.Add(new FileModel(fileInfo, this));
                }
            }

            _inited = true;
        }

        public void GetHeavyFiles(List<IFile> aHeavyFiles, int aMaxCount) {
            if (!_driveInfo.IsReady)
                return;

            if (!_inited)
                LazyInit();

            foreach (IPhysicalObject physicalObject in PhysicalObjects) {
                physicalObject.GetHeavyFiles(aHeavyFiles, aMaxCount);
            }
        }

        public void ChangePath(string path) => throw new NotImplementedException();
        public void Delete() => throw new NotImplementedException();
    }
}
