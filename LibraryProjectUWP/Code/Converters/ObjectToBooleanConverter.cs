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
    

    public class ObjectToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is DataViewModeEnum dataViewMode && parameter is string viewName)
                {
                    switch (dataViewMode)
                    {
                        case DataViewModeEnum.DataGridView:
                            return viewName == "DataGridView";
                        case DataViewModeEnum.GridView:
                            return viewName == "GridView";
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
