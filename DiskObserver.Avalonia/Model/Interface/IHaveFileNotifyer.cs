namespace DiskObserver.Model.Interface {
    public interface IHaveFileNotifyer {
        public bool IgnoreAllNotify { get; set; }
        public void TrySubcribe();
        public void DisposeSubscriber();
    }
}
