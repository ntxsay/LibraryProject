using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.Views.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class ContactCollectionParentChildParamsVM
    {
        public IEnumerable<ContactVM> ViewModelList { get; set; }
        public ContactCollectionPage ParentPage { get; set; }
    }
}
