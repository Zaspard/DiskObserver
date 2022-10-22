using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DiskObserver.ViewModels {
    public sealed class DiskObserverVM : BaseModel, IDisposable {
        public RelayCommand DisplayParentPhysicalObjectCommand => new RelayCommand(obj => DisplayPhysicalObject(DisplayedPhysicalObject.ParentPhysicalObject));

        //TODO: handled command
        //public RelayCommand DisplayPhysicalObjectCommand => new RelayCommand(obj => DisplayPhysicalObject());

        //Contains only IPhysicalDisk
        public ObservableCollection<IPhysicalObject> PhysicalDisks { get; set; } = new();

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

        public DiskObserverVM() {

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives) {
                PhysicalDisks.Add(new PhysicalDisk(driveInfo));
            }
        }

        public void Dispose() {
            foreach (IPhysicalObject physicalDisk in PhysicalDisks)
                physicalDisk.Dispose();

            PhysicalDisks.Clear();
        }

        public void Refrech() {

        }

        public void DisplayPhysicalObject(IPhysicalObject physicalObject) {
            if (physicalObject is IFile)
                return;

            physicalObject.LazyInit();
            DisplayedPhysicalObject = physicalObject;
        }
    }
}
