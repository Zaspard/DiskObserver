using System.IO;

namespace DiskObserver.Model.Interface {
    public interface IFile : IPhysicalObject {
        public bool IsVisible { get; }
        public bool IsRenameMode { get; set; }
        public string Type { get; }
        void RefreshProperty(FileInfo? fileInfo = null);
    }
}
