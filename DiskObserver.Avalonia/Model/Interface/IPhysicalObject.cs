using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalObject : IDisposable {
        public ObservableCollection<IPhysicalObject>? PhysicalObjects { get; }
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public bool IsVisibleInTree { get; }    
        public IPhysicalObject? ParentPhysicalObject { get; }
        public void LazyInit();
        public void GetHeavyFiles(List<IFile> heavyFiles, int maxCount);
        void ChangePath(string path);
        void Delete();
    }
}
