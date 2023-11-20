namespace DiskObserver.Utils {

    public enum MoveMode {
        Normal = 0,
        Back = 1,
        Forward = 2,
    }

    public enum SortMode {
        Name = 0,
        LastWrite = 1,
        Type = 2,
        Size = 3,
    }

    public enum GroupMode {
        None = 0,
        Name = 1,
        LastWrite = 2,
        Type = 3,
        Size = 4,
    }

    public enum ViewMode {
        ExtraLargeIcons = 0,
        LargeIcons = 1,
        MediumIcons = 2,
        SmallIcons = 3,
        List = 4,
        Details = 5,
        Tiles = 6,
        Content = 7,
    }
}
