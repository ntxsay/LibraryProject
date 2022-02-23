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
    public class BookExemplaryDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ExemplaryTemplate { get; set; }
        public DataTemplate ExemplaryEtatTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            try
            {
                if (item is LivreExemplaryVM)
                {
                    return ExemplaryTemplate;
                }
                else if (item is LivreEtatVM)
                {
                    return ExemplaryEtatTemplate;
                }

                return null;
                //return base.SelectTemplateCore(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
