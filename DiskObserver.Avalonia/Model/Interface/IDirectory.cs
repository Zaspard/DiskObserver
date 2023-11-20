namespace DiskObserver.Model.Interface {
    public interface IDirectory : IPhysicalObject {
        public bool IsVisible { get; }
        public bool IsRenameMode { get; set; }
    }
}
