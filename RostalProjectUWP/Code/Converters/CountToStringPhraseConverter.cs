using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RostalProjectUWP.Code.Converters
{
    internal class CountToStringPhrase : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter && parameter is string phrase)
                {
                    if (phrase.Contains("counter"))
                    {
                        return phrase.Replace("counter", counter.ToString());
                    }

                    return null;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
