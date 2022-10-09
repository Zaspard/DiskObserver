using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskObserver.Model.Interface {
    public interface IPhysicalObject : IDisposable {

        public void Delete();
    }
}
