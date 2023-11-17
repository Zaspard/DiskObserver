namespace DiskObserver.Avalonia.Model.Interface {
    public interface IFile : IPhysicalObject {
        public string Name { get; set; }
        public bool IsVisible { get; }
        public long Size { get; }
        public string Format { get; }
    }
}
