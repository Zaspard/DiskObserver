using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace DiskObserver.Tests {
    public class FindAllHeavyFiles {

        [Test]
        public void CheckTestFolder1Model() {
            string path = Assembly.GetExecutingAssembly().Location;
            Assert.IsNotNull(path, "Assert path is null");

            DirectoryInfo directoryInfo = Directory.GetParent(path);
            Assert.IsNotNull(directoryInfo, "DirectoryInfo path is null");

            DirectoryModel directoryModel = new(directoryInfo, null);
            Assert.IsNotNull(directoryModel, "DirectoryModel path is null");

            directoryModel.LazyInit();
            Assert.NotZero(directoryModel.PhysicalObjects.Count, "DirectoryModel have 0 items");

            //directoryModel.GetHeavyFiles();
            //IPhysicalObject fileModel = directoryModel.PhysicalObjects.FirstOrDefault(x => x.Path == path);
            //Assert.IsNotNull(path, "DirectoryModel not contains file");
        }
    }
}
