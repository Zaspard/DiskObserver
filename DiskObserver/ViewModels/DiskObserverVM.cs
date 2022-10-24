using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DiskObserver.ViewModels {
    public sealed class DiskObserverVM : BaseModel, IDisposable {
        public RelayCommand DisplayParentPhysicalObjectCommand => new RelayCommand(obj => DisplayPhysicalObject(DisplayedPhysicalObject?.ParentPhysicalObject));

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

        public void DisplayPhysicalObject(IPhysicalObject physicalObject) {
            if (physicalObject is IFile || physicalObject == null)
                return;

            physicalObject.LazyInit();
            DisplayedPhysicalObject = physicalObject;
        }

        public async void FindAllHeavyFiles(IPhysicalObject physicalObject) {
            IsEnable = false;

            List<IFile> heavyFiles = new();
            await Task.Run(() => {
                physicalObject.GetHeavyFiles(heavyFiles, 50);
            });

            IPhysicalObject head = new DirectoryModel();
            foreach(IFile file in heavyFiles) {
                head.PhysicalObjects.Add(file);
            }

            DisplayedPhysicalObject = head;
            IsEnable = true;
        }

        public void OpenInExplorer(IPhysicalObject physicalObject) {
            string argument = "/select, \"" + physicalObject.Path + "\"";

            Process.Start("explorer.exe", argument);
        }
    }
}
