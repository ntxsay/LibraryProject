using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class SizeDiviserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is double size && parameter is string stringDiviser)
                {
                    string[] split = stringDiviser.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (split != null && split.Length == 2)
                    {
                        double diviser = System.Convert.ToDouble(split[0]);
                        double spacing = System.Convert.ToDouble(split[1]);

                        double calc = size / diviser;
                        calc -= spacing;
                        return calc;
                    }
                }

                throw new NotSupportedException();
            }
            catch (Exception)
            {
                return 0d;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
