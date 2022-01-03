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
    internal class NameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = Colors.Black;

            if (value != null)
            {
                var hash = value.GetHashCode();

                var rnd = new Random(hash);

                color = Color.FromArgb(255, (byte)rnd.Next(64, 192), (byte)rnd.Next(64, 192), (byte)rnd.Next(64, 192));
            }

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
