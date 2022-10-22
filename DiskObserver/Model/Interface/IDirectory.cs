namespace DiskObserver.Model.Interface {
    public interface IDirectory : IPhysicalObject {
        public string Name { get; set; }
        public bool IsHidden { get; }
        public long Size { get; }
    }
}
