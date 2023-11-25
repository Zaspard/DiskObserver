using DiskObserver.Model.Interface;
using DiskObserver.ViewModels;
using System.Windows.Input;

namespace DiskObserver.Test {
    public class TestDiskObserver {

        [Test]
        public void Test_CreateAndDisplay() {
            var diskObserver = new DiskObserverVM();
            diskObserver.DisplayPhysicalObject(diskObserver.PhysicalObjects?.FirstOrDefault());
            diskObserver.Dispose();
        }

        [Test]
        public void Test_MoveToAndForwardTo() {
            var diskObserver = new DiskObserverVM();

            var firstDisk = diskObserver.PhysicalObjects?.FirstOrDefault(x => x.PhysicalObjects?.Any() == true);
            if (firstDisk != null) {

                diskObserver.DisplayPhysicalObject(firstDisk);
                string startPosition = diskObserver.DisplayedPhysicalObject.Path;
                while (true) {

                    var item = diskObserver.DisplayedPhysicalObject
                                            .PhysicalObjects?.FirstOrDefault(x => x is IDirectory);
                    if(item == null)
                        break;


                     diskObserver.DisplayPhysicalObject(item);
                }

                string endPosition = diskObserver.DisplayedPhysicalObject.Path;

                Assert.IsTrue(((ICommand)diskObserver.MoveBackCommand).CanExecute(null));
                while (diskObserver.DisplayedPhysicalObject != firstDisk) {
                    diskObserver.MoveBackCommand?.Execute().Subscribe();
                }

                Assert.AreEqual(startPosition, diskObserver.DisplayedPhysicalObject.Path);

                Assert.IsTrue(((ICommand)diskObserver.MoveForwardCommand).CanExecute(null));
                while (endPosition != diskObserver.DisplayedPhysicalObject.Path) {
                    var result = diskObserver.MoveForwardCommand?.Execute().Subscribe();
                }

            }
            diskObserver.Dispose();
        }

        [Test]
        public void Test_SelectedItem() {
            var diskObserver = new DiskObserverVM();

            var firstDisk = diskObserver.PhysicalObjects?.FirstOrDefault(x => x.PhysicalObjects?.Any() == true);
            if (firstDisk != null) {

                diskObserver.DisplayPhysicalObject(firstDisk);

                Assert.IsTrue(((ICommand)diskObserver.SelectAllCommand).CanExecute(null));
                diskObserver.SelectAllCommand?.Execute().Subscribe();

                Assert.AreEqual(diskObserver.DisplayedPhysicalObject.PhysicalObjects?.Count(), diskObserver.SelectedItemsInFilesViewer.Count());

                Assert.IsTrue(((ICommand)diskObserver.UnselectAllCommand).CanExecute(null));
                diskObserver.UnselectAllCommand?.Execute().Subscribe();

                Assert.AreEqual(0, diskObserver.SelectedItemsInFilesViewer.Count());
            }

            diskObserver.Dispose();
        }
    }
}