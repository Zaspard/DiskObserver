using System;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalDisk : IPhysicalObject, IDisposable {
        public string Format { get; }
        public long TotalMemory { get; }
        public long FreeMemory { get; }
        public bool IsNotReady { get; }
        public bool IsExpanded { get; }
        public void RefreshProperty();
    }
}
