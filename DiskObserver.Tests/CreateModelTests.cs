using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using DiskObserver.ViewModels;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DiskObserver.Tests {
    public class CreateModelTests {

        [Test]
        public void CreateDiskObserverVM() {
            DiskObserverVM diskObserverVM = new();
            Assert.IsNotNull(diskObserverVM);
        }

        [Test]
        public void CreateFileModel() {
            string path = Assembly.GetExecutingAssembly().Location;
            Assert.IsNotNull(path, "Assert path is null");

            FileInfo fileInfo = new FileInfo(path);
            Assert.IsNotNull(fileInfo, "FileInfo path is null");

            FileModel fileModel = new(fileInfo, null);
            Assert.IsNotNull(fileModel, "FileModel path is null");
        }

        [Test]
        public void CreateDirectoryModel() {
            string path = Assembly.GetExecutingAssembly().Location;
            Assert.IsNotNull(path, "Assert path is null");

            DirectoryInfo directoryInfo = Directory.GetParent(path);
            Assert.IsNotNull(directoryInfo, "DirectoryInfo path is null");

            DirectoryModel directoryModel = new(directoryInfo, null);
            Assert.IsNotNull(directoryModel, "DirectoryModel path is null");

            directoryModel.LazyInit();
            Assert.NotZero(directoryModel.PhysicalObjects.Count, "DirectoryModel have 0 items");

            IPhysicalObject fileModel = directoryModel.PhysicalObjects.FirstOrDefault(x => x.Path == path);
            Assert.IsNotNull(path, "DirectoryModel not contains file");
        }
    }
}