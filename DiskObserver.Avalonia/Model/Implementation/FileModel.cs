using DiskObserver.Avalonia.Model.Interface;
using DiskObserver.Avalonia.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable disable

namespace DiskObserver.Avalonia.Model.Implementation {
    public class FileModel : BaseModel, IFile {
        private string _name = "";
        public string Name {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _isHidden;
        public bool IsHidden {
            get => _isHidden;
            private set
            {
                _isHidden = value;
                OnPropertyChanged(nameof(IsHidden));
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

        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; } = null;
        public IPhysicalObject ParentPhysicalObject{ get; private set; }
        public FileModel(FileInfo aFileInfo, IPhysicalObject _parentObject) {
            ParentPhysicalObject = _parentObject;
            Path = aFileInfo.FullName;

            Name = aFileInfo.Name;
            IsHidden = aFileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            if (aFileInfo.Extension?.Length > 0)
                Format = aFileInfo.Extension.Substring(1, aFileInfo.Extension.Length - 1);

            Size = aFileInfo.Length;
        }

        public void Dispose() {
            ParentPhysicalObject = null;
        }

        public void Delete() {

        }

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
    }
}
