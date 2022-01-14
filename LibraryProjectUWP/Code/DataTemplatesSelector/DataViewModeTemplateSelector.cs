using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.Code.DataTemplatesSelector
{
    public class DataViewModeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GridViewTemplate { get; set; }
        public DataTemplate DataGridViewTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                
                var _Container = VisualViewHelpers.FindVisualAncestor<Pivot>(container);
                if (_Container != null && _Container.Tag is DataViewModeEnum dataViewMode)
                {
                    if (dataViewMode == DataViewModeEnum.DataGridView)
                    {
                        return DataGridViewTemplate;
                    }
                    else if (dataViewMode == DataViewModeEnum.GridView)
                    {
                        return GridViewTemplate;
                    }
                }

                return base.SelectTemplateCore(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
