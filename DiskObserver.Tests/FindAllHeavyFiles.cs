using DiskObserver.Model.Implementation;
using DiskObserver.Model.Interface;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DiskObserver.Tests {
    public class FindAllHeavyFiles {

        [Test]
        [TestCase("TestFolder1", 62)]
        [TestCase("TestFolder2", 150)]
        [TestCase("TestFolder3", 123)]
        public void CheckTestFolderModel(string nameFolder, long sizeFile) {
            string path = Assembly.GetExecutingAssembly().Location;
            Assert.IsNotNull(path, "Assert path is null");
            path = Path.Combine(Path.GetDirectoryName(path), "TestFiles", nameFolder);
            Assert.IsNotNull(path, "Assert path is null");

            Assert.IsTrue(Directory.Exists(path), "Not found directory with test files");

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            Assert.IsNotNull(directoryInfo, "DirectoryInfo path is null");

            DirectoryModel directoryModel = new(directoryInfo, null);
            Assert.IsNotNull(directoryModel, "DirectoryModel path is null");

            directoryModel.LazyInit();
            Assert.NotZero(directoryModel.PhysicalObjects.Count, "DirectoryModel have 0 items");

            List<IFile> files = new();
            directoryModel.GetHeavyFiles(files, 3);

            Assert.NotZero(files.Count, "Can't find files in directory");
            Assert.AreEqual(files[0].Size, sizeFile);
        }
    }
}
