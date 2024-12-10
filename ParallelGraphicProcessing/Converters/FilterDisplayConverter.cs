using System.Globalization;

namespace ParallelGraphicProcessing.Converters;

public class FilterDisplayConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string item = value as string;
        return item?.Split(':')[1] ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}