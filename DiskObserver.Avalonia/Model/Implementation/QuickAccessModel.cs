using DiskObserver.Avalonia.Model.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable

namespace DiskObserver.Avalonia.Model.Implementation {
    public sealed class QuickAccessModel : IPhysicalObject {
        public ObservableCollection<IPhysicalObject>? PhysicalObjects { get; set; } = new();

        public string Path => "";

        public bool IsVisibleInTree => true;

        public IPhysicalObject? ParentPhysicalObject => null;

        public void Dispose() {
            if (PhysicalObjects != null && PhysicalObjects.Count > 0) {
                PhysicalObjects.Clear();
                PhysicalObjects = null;
            }
        }

        public void GetHeavyFiles(List<IFile> heavyFiles, int maxCount) {}
        public void LazyInit() {}
    }
}
