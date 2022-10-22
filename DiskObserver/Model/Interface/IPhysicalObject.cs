using System;
using System.Collections.ObjectModel;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalObject : IDisposable {
        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; }
        public string Path { get; }
        public IPhysicalObject ParentPhysicalObject { get; }
    }
}
