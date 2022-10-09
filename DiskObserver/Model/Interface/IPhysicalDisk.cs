using System;
using System.Collections.ObjectModel;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalDisk : IDisposable {
        public string Name { get; }
        public string Format { get; }
        public long TotalMemory { get; }
        public long FreeMemory { get; }
        public bool IsNotReady { get; }

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; }

        public void LoadInfo();
        public void RefreshProperty();
    }
}
