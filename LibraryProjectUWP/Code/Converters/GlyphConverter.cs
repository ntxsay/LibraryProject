using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Code.Converters
{
    public class MinusPlusGlyphToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is bool boolean)
                {
                    if (parameter == null || parameter is string invert && invert != "invert")
                    {
                        return boolean ? "\uECC8" : "\uECC9";
                    }
                    else
                    {
                        return boolean ? "\uECC9" : "\uECC8";
                    }
                }

                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
