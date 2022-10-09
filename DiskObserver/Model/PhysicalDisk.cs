using DiskObserver.Utils;
using System;
using System.IO;

#nullable enable

namespace DiskObserver.Model {
    public sealed class PhysicalDisk : BaseModel, IDisposable {

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


        private DriveInfo _driveInfo;
        public PhysicalDisk(DriveInfo driveInfo) {

            _driveInfo = driveInfo;
            Name = driveInfo.Name;
            if (driveInfo.IsReady) {
                Format = driveInfo.DriveFormat;
                TotalMemory = driveInfo.TotalSize;
                FreeMemory = driveInfo.AvailableFreeSpace;
            }
        }

        public void Dispose() {
            _driveInfo = null;
        }

        public void RefreshProperty() {
            FreeMemory = _driveInfo.AvailableFreeSpace;
        }
    }
}
