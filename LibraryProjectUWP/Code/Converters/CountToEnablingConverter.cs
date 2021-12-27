using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class CountToEnablingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter)
                {
                    return counter > 0 ? true : false;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CountOneToEnablingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter)
                {
                    if (counter == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
