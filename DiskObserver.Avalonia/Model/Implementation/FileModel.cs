using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable enable

namespace DiskObserver.Avalonia.Model.Implementation {
    public class FileModel : BaseModel, IFile {
        public bool IsVisibleInTree => false;

        private string _name = "";
        public string Name {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            private set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }


        private bool _isRenameMode;
        public bool IsRenameMode {
            get => _isRenameMode;
            set
            {
                _isRenameMode = value;
                OnPropertyChanged(nameof(IsRenameMode));
            }
        }

        private long _size;
        public long Size {
            get => _size;
            private set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
        private string _format = "";
        public string Format {
            get => _format;
            set
            {
                _format = value;
                OnPropertyChanged(nameof(Format));
            }
        }

        private string _path = "";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public ObservableCollection<IPhysicalObject>? PhysicalObjects { get; set; } = null;
        public IPhysicalObject? ParentPhysicalObject{ get; private set; }
        public FileModel(FileInfo aFileInfo, IPhysicalObject _parentObject) {
            ParentPhysicalObject = _parentObject;
            Path = aFileInfo.FullName;

            Name = aFileInfo.Name;

            RefreshProperty(aFileInfo);
        }

        public void Dispose() {
            ParentPhysicalObject = null;
        }

        public void Delete() => File.Delete(Path);

        public void LazyInit() {

        }

        public void GetHeavyFiles(List<IFile> aHeavyFiles, int aMaxCount) {

            if (aHeavyFiles.Count <= 0) {
                aHeavyFiles.Add(this);
                return;
            }

            if (aHeavyFiles.Count - 1 > aMaxCount) {

                IFile lastFileModel = aHeavyFiles[aHeavyFiles.Count - 1];
                if (Size < lastFileModel.Size)
                    return;

                aHeavyFiles.Remove(lastFileModel);
            }

            int index = -1;
            for (int i = 0; i < aHeavyFiles.Count; i++) {

                IFile fileModel = aHeavyFiles[i];
                if (fileModel.Size < Size) {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return;

            aHeavyFiles.Insert(index, this);
        }

        public void ChangePath(string path) => Path = path;

        public void RefreshProperty(FileInfo? aFileInfo) {

            FileInfo fileInfo = aFileInfo ?? new FileInfo(Path);
            IsVisible = !fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            if (fileInfo.Extension?.Length > 0)
                Format = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);

            Size = fileInfo.Length;
        }
    }
}
