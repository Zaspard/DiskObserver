using Avalonia;
using Avalonia.Data.Converters;
using DiskObserver.Avalonia.Model.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DiskObserver.Avalonia.View.Converters {
    public class NameConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {

            if (value is IEnumerable<IPhysicalObject> collection) {
                return collection.OrderBy(x => x.Name);
            }

            return AvaloniaProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
