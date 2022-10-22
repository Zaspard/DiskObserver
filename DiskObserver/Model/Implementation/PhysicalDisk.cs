using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace DiskObserver.Model.Implementation {
    public sealed class PhysicalDisk : BaseModel, IPhysicalDisk {

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; } = new();

        string _name = "";
        public string Name {
            get => _name;
            private set
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

        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public IPhysicalObject ParentPhysicalObject { get; private set; } = null;


        private DriveInfo _driveInfo;
        public PhysicalDisk(DriveInfo driveInfo) {
            _driveInfo = driveInfo;
            Path = driveInfo.Name;
            Name = driveInfo.Name;
            if (driveInfo.IsReady) {
                Format = driveInfo.DriveFormat;
                TotalMemory = driveInfo.TotalSize;
                FreeMemory = driveInfo.AvailableFreeSpace;
                LoadInfo();
            }
        }

        public void Dispose() {
            _driveInfo = null;

            foreach(IPhysicalObject physicalObject in PhysicalObjects) {
                physicalObject.Dispose();
            }

            PhysicalObjects.Clear();
        }

        public async void LoadInfo() {
            foreach (DirectoryInfo directoryInfo in _driveInfo.RootDirectory.GetDirectories()) {
                await Task.Run(() => {
                    PhysicalObjects.Add(new DirectoryModel(directoryInfo, this));
                });
            }

            foreach (FileInfo fileInfo in _driveInfo.RootDirectory.GetFiles()) {
                PhysicalObjects.Add(new FileModel(fileInfo, this));
            }
        }

        public void RefreshProperty() {
            FreeMemory = _driveInfo.AvailableFreeSpace;
        }
    }
}
