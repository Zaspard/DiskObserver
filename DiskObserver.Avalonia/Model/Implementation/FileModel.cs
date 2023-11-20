using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable enable

namespace DiskObserver.Model.Implementation {
    public class FileModel : BaseModel, IFile, ICanRename {
        public bool IsVisibleInTree => false;

        private string _name = "";
        public string Name {
            get => _name;
            set
            {
                if (_name == value || (_isRenameMode && !TryRename(value))) {
                    IsRenameMode = false;
                    return;
                }

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
        private string _type = "";
        public string Type {
            get => _type;
            private set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
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

        public DateTime _lastWrite;
        public DateTime LastWrite { 
            get => _lastWrite;
            set
            {
                _lastWrite = value;
                OnPropertyChanged(nameof(LastWrite));   
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
                Type = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);

            Size = fileInfo.Length;
            LastWrite = fileInfo.LastWriteTime;
        }

        bool TryRename(string newName) {

            if (ParentPhysicalObject == null || ParentPhysicalObject.PhysicalObjects == null)
                return false;


            var haveFileNotifyer = ParentPhysicalObject as IHaveFileNotifyer;
            if (haveFileNotifyer != null)
                haveFileNotifyer.IgnoreAllNotify = true;

            foreach (var item in ParentPhysicalObject.PhysicalObjects) {

                if (item == this)
                    continue;

                if (string.Equals(item.Name, newName, StringComparison.OrdinalIgnoreCase)) {
                    IsRenameMode = false;
                    return false;
                }
            }

            string newPath = Path.Remove(Path.Length - Name.Length) + newName;

            try {
                FileInfo fileInfo = new FileInfo(Path);
                fileInfo.MoveTo(newPath);
            }
            catch (Exception ex) {
                _ = ex;
                IsRenameMode = false;
                return false;
            }

            ChangePath(newPath);

            if (haveFileNotifyer != null)
                haveFileNotifyer.IgnoreAllNotify = false;

            IsRenameMode = false;
            return true;
        }
    }
}
