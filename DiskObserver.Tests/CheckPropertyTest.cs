using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace DiskObserver.Avalonia.Tests {
    public class CheckPropertyTest {

        [Test]
        public void CheckFileInfoProperties() {
            string path = Assembly.GetExecutingAssembly().Location;
            Assert.IsNotNull(path, "Assert path is null");

            FileInfo fileInfo = new FileInfo(path);
            Assert.IsNotNull(fileInfo, "FileInfo path is null");

            FileModel fileModel = new(fileInfo, null);
            Assert.IsNotNull(fileModel, "FileModel path is null");

            Assert.AreEqual(fileModel.Size, fileModel.Size);
            Assert.AreEqual(fileModel.Path, fileModel.Path);
            Assert.AreEqual(fileModel.IsHidden, fileModel.IsHidden);
            Assert.AreEqual(fileModel.Format, fileModel.Format);
        }
    }
}
