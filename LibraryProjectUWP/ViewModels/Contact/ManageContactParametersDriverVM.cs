using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraryProjectUWP.ViewModels.Contact
{
    public class ManageContactParametersDriverVM
    {
        public IEnumerable<ContactVM> ViewModelList { get; set; }
        public ContactVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public ContactType ContactType { get; set; }
        public Visibility ContactTypeVisibility { get; set; } = Visibility.Collapsed;
    }

    public class ContactListParametersDriverVM
    {
        public IEnumerable<ContactVM> ViewModelList { get; set; }
        public ContactVM CurrentViewModel { get; set; }
        public ContactType ContactType { get; set; }
    }
}
