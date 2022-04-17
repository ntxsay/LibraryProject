using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.Code.DataTemplatesSelector
{
    public class ContactTypeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PersonTemplate { get; set; }
        public DataTemplate EnterpriseTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            try
            {
                if (item is ContactVM viewModel)
                {
                    if (viewModel.ContactType == ContactType.Society)
                    {
                        return EnterpriseTemplate;
                    }
                    else
                    {
                        return PersonTemplate;
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
