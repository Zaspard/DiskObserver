using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiskObserver.Avalonia.Model.Interface {
    public interface IPhysicalObject : IDisposable {
        public ObservableCollection<IPhysicalObject>? PhysicalObjects { get; }
        public string Path { get; }
        public bool IsVisibleInTree { get; }    
        public IPhysicalObject? ParentPhysicalObject { get; }
        public void LazyInit();
        public void GetHeavyFiles(List<IFile> heavyFiles, int maxCount);
    }
}
