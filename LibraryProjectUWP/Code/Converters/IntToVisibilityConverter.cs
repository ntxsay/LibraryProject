using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int int32)
                {
                    return int32 > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value is long int64)
                {
                    return int64 > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value is short int16)
                {
                    return int16 > 0 ? Visibility.Visible : Visibility.Collapsed;
                }

                return Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToInvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter)
                {
                    return counter > 0 ? Visibility.Collapsed : Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
