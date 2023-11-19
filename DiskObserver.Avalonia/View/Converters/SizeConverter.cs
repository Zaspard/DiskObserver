using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace DiskObserver.View.Converters {
    public class SizeConverter : MarkupExtension, IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {

            if (value is long bytes) {

                if (bytes < 1024)
                    return $"{bytes} Byte";

                double kbytes = bytes / 1024.0;
                if (kbytes < 1024)
                    return $"{Math.Round(kbytes, 2)} KB";

                double mbytes = kbytes / 1024.0;
                if (mbytes < 1024)
                    return $"{Math.Round(mbytes, 2)} MB";

                double gbytes = mbytes / 1024.0;
                if (gbytes < 1024)
                    return $"{Math.Round(gbytes, 2)} GB";

                double tbytes = gbytes / 1024.0;
                return $"{Math.Round(tbytes, 2)} TB";
            }
            else
                return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static SizeConverter? sizeConverter = null;
        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (sizeConverter == null)
                sizeConverter = new SizeConverter();

            return sizeConverter;
        }
    }
}
