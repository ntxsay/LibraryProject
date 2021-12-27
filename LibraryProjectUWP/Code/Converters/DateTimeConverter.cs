using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class DateTimeToStringDisplay : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter is string dateStringFormat)
                {
                    if (value is DateTimeOffset dateTimeOffset)
                    {
                        return dateTimeOffset.Date.ToString(dateStringFormat);
                    }
                    else if (value is DateTime dateTime)
                    {
                        return dateTime.ToString(dateStringFormat);
                    }
                }
                

                return "date inconnue";
            }
            catch (Exception)
            {
                return "date inconnue";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
