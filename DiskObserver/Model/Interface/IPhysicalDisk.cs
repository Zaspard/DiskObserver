using System;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalDisk : IPhysicalObject, IDisposable {
        public string Name { get; }
        public string Format { get; }
        public long TotalMemory { get; }
        public long FreeMemory { get; }
        public bool IsNotReady { get; }
        public void RefreshProperty();
    }
}
