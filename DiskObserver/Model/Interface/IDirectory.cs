using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskObserver.Model.Interface {
    public interface IDirectory : IPhysicalObject {
        public string Name { get; set; }
        public bool IsHidden { get; }
        public long Size { get; }

        public ObservableCollection<IPhysicalObject> PhysicalObjects { get; set; }
    }
}
