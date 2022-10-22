using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

#nullable enable

namespace DiskObserver.Model.Implementation {
    public class DirectoryModel : BaseModel, IDirectory {

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            private set
            {
                _isHidden = value;
                OnPropertyChanged(nameof(IsHidden));
            }
        }
        private long _size;
        public long Size
        {
            get => _size;
            private set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
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

        public IPhysicalObject ParentPhysicalObject { get; private set; }

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; } = new();
        
        public DirectoryModel(DirectoryInfo aDirectoryInfo, IPhysicalObject _parentObject) {
            ParentPhysicalObject = _parentObject;
            Path = aDirectoryInfo.FullName;

            Name = aDirectoryInfo.Name;
            IsHidden = aDirectoryInfo.Attributes.HasFlag(FileAttributes.Hidden);

            LoadInfo(aDirectoryInfo);
        }

        public void Dispose() {
            ParentPhysicalObject = null;

            foreach (var item in PhysicalObjects)
                item.Dispose();

            PhysicalObjects.Clear();
        }

        public void LoadInfo(DirectoryInfo aDirectoryInfo) {

            long size = 0;
            DirectoryInfo[]? directoryInfos = null;
            try {
                directoryInfos = aDirectoryInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex) {
                throw ex;
            }


            if (directoryInfos != null) {

                foreach (DirectoryInfo directoryInfo in directoryInfos) {
                    DirectoryModel directoryModel = new DirectoryModel(directoryInfo, this);
                    PhysicalObjects.Add(directoryModel);
                    size += directoryModel.Size;
                }
            }

            FileInfo[]? fileInfos = null;
            try {
                fileInfos = aDirectoryInfo.GetFiles();
            }
            catch (UnauthorizedAccessException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex) {
                _ = ex;
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex) {
                throw ex;
            }

            if (fileInfos != null) {
                foreach (FileInfo fileInfo in fileInfos) {
                    FileModel fileModel = new FileModel(fileInfo, this);
                    PhysicalObjects.Add(fileModel);
                    size += fileModel.Size;
                }
            }

            Size = size;
        }

        public void Delete() {

        }
    }
}
