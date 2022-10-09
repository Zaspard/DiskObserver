using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskObserver.Model.Interface {
    public interface IFile : IPhysicalObject {
        public string Name { get; set; }
        public bool IsHidden { get; }
        public long Size { get; }
        public string Format { get; }
    }
}
