using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.Code.DataTemplatesSelector
{
    public class LibraryThumbnailItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LibraryItemTemplate { get; set; }
        public DataTemplate BookItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            try
            {
                if (item is BibliothequeVM)
                {
                    return LibraryItemTemplate;
                }
                else if (item is LivreVM)
                {
                    return BookItemTemplate;
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
