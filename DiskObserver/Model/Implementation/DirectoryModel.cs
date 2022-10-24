using DiskObserver.Model.Interface;
using DiskObserver.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

#nullable disable

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
        }

        public DirectoryModel() {

        }

        public void Dispose() {
            ParentPhysicalObject = null;

            foreach (var item in PhysicalObjects)
                item.Dispose();

            PhysicalObjects.Clear();
        }


        bool _inited = false;
        public void LazyInit() {

            if (_inited)
                return;

            var aDirectoryInfo = new DirectoryInfo(_path);

            DirectoryInfo[] directoryInfos = null;
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
#pragma warning disable CA2200 // Rethrow to preserve stack details
                throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }


            if (directoryInfos != null) {

                foreach (DirectoryInfo directoryInfo in directoryInfos) {
                    DirectoryModel directoryModel = new DirectoryModel(directoryInfo, this);
                    PhysicalObjects.Add(directoryModel);
                }
            }

            FileInfo[] fileInfos = null;
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
#pragma warning disable CA2200 // Rethrow to preserve stack details
                throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }

            if (fileInfos != null) {
                foreach (FileInfo fileInfo in fileInfos) {
                    FileModel fileModel = new FileModel(fileInfo, this);
                    PhysicalObjects.Add(fileModel);
                }
            }

            _inited = true;
        }

        public void Delete() {

        }

        public void GetHeavyFiles(List<IFile> aHeavyFiles, int aMaxCount) {
            if (!_inited)
                LazyInit();

            foreach (IPhysicalObject physicalObject in PhysicalObjects) {
                physicalObject.GetHeavyFiles(aHeavyFiles, aMaxCount);
            }
        }
    }
}
