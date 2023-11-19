using System;
using System.IO;

namespace DiskObserver.Model.Interface {
    public interface IFile : IPhysicalObject {
        public bool IsVisible { get; }
        public bool IsRenameMode { get; set; }
        public long Size { get; }
        public string Format { get; }
        public DateTime LastWrite { get; }
        void RefreshProperty(FileInfo? fileInfo = null);
    }
}
