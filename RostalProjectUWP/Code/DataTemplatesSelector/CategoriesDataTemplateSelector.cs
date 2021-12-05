using RostalProjectUWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RostalProjectUWP.Code.DataTemplatesSelector
{
    public class CategoriesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CategorieTemplate { get; set; }
        public DataTemplate SubCategorieTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            try
            {
                if (item is CategorieLivreVM categorie)
                {
                    return CategorieTemplate;
                }
                else if (item is SubCategorieLivreVM subCategorie)
                {
                    return SubCategorieTemplate;
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
