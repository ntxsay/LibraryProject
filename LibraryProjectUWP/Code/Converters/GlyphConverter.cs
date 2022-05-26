using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Code.Converters
{
    public class FalseTrueGlyphToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is bool boolean && parameter is string para)
                {
                    var split = StringHelpers.SplitWord(para, new char[] { '|' }, true);
                    if (split != null && split.Length == 2)
                    {
                        /* 0 => false
                           1 => true */

                        char trueVal = (char)(System.Convert.ToInt32(split[1], 16));
                        char falseVal = (char)(System.Convert.ToInt32(split[0], 16));
                        return boolean ? trueVal.ToString() : falseVal.ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
