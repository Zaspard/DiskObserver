using DiskObserver.Model.Interface;
using DiskObserver.Utils;
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


        FileInfo _fileInfo;
        public FileModel(FileInfo aFileInfo) {
            Name = aFileInfo.Name;
            IsHidden = aFileInfo.Attributes.HasFlag(FileAttributes.Hidden);
            Format = "";
            Size = aFileInfo.Length;

            _fileInfo = aFileInfo;
        }

        public void Dispose() {

        }

        public void Delete() {

        }
    }
}
