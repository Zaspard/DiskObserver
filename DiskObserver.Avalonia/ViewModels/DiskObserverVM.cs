using DiskObserver.Avalonia.Model.Implementation;
using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.Utils;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;

#nullable disable

namespace DiskObserver.Avalonia.ViewModels {
    public sealed class DiskObserverVM : BaseModel, IDisposable {
        public ReactiveCommand<IPhysicalObject, Unit> DisplayPhysicalObjectCommand => 
                                                        ReactiveCommand.Create((IPhysicalObject physicalObject) => DisplayPhysicalObject(physicalObject));
        public ReactiveCommand<Unit, Unit> DisplayParentCommand =>
                                                        ReactiveCommand.Create(() => DisplayPhysicalObject(DisplayedPhysicalObject?.ParentPhysicalObject));

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

        }

        public void DisplayPhysicalObject(IPhysicalObject physicalObject) {
            if (physicalObject is IFile || physicalObject == null)
                return;

            physicalObject.LazyInit();
            DisplayedPhysicalObject = physicalObject;
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

        internal void RenameItem(IPhysicalObject physicalObject) {
        }

        internal void ShowPropertyItem(IPhysicalObject physicalObject) {
        }

        internal void Paste(IPhysicalObject physicalObject) {
        }

        internal void Cut(IPhysicalObject physicalObject) {
        }

        internal void Copy(IPhysicalObject physicalObject) {
        }
    }
}
