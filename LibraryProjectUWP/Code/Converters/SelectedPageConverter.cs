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
    internal class SelectedPageConverter
    {
    }
    public class SelectedPageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is bool viewModel)
                {
                    return viewModel ? new SolidColorBrush(Colors.DeepSkyBlue) : new SolidColorBrush(Colors.Transparent);
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
