using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable disable

namespace DiskObserver.Avalonia.Utils {
    public class BaseModel : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
