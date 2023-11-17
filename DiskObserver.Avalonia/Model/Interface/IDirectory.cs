namespace DiskObserver.Avalonia.Model.Interface {
    public interface IDirectory : IPhysicalObject {
        public string Name { get; set; }
        public bool IsVisible { get; }
        public long Size { get; }
    }
}
