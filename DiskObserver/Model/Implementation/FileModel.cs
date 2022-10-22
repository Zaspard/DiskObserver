using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System.Collections.ObjectModel;
using System.IO;

namespace DiskObserver.Model.Implementation {
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
    }
}
