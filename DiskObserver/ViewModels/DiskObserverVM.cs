using DiskObserver.Model;
using DiskObserver.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DiskObserver.ViewModels {
    public sealed class DiskObserverVM : BaseModel, IDisposable {

        public ObservableCollection<PhysicalDisk> PhysicalDisks { get; set; } = new();

        public DiskObserverVM() {

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives) {
                PhysicalDisks.Add(new(driveInfo));
            }
        }

        public void Dispose() {
            foreach (PhysicalDisk physicalDisk in PhysicalDisks)
                physicalDisk.Dispose();

            PhysicalDisks.Clear();
        }

        public void Refrech() {

        }
    }
}
