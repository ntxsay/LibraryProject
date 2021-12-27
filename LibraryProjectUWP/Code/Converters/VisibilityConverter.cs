using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Visibility visibility = value == null ? Visibility.Collapsed : Visibility.Visible;
                //Debug.WriteLine(visibility);
                return visibility;
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

    public class EditModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Visibility visibility = Visibility.Collapsed;
                if (value is EditMode mode)
                {
                    switch (mode)
                    {
                        case EditMode.Create:
                            if (parameter is string stateCreate && stateCreate == "reverse")
                            {
                                return Visibility.Collapsed;
                            }
                            else
                            {
                                return Visibility.Visible;
                            }
                        case EditMode.Edit:
                            if (parameter is string stateEdit && stateEdit == "reverse")
                            {
                                return Visibility.Visible;
                            }
                            else
                            {
                                return Visibility.Collapsed;
                            }
                        default:
                            break;
                    }
                }
                //Debug.WriteLine(visibility);
                return visibility;
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
