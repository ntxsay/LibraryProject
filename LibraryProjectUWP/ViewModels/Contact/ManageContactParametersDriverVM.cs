using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Contact
{
    public class ManageContactParametersDriverVM
    {
        public IEnumerable<ContactVM> ViewModelList { get; set; }
        public ContactVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class ContactListParametersDriverVM
    {
        public IEnumerable<ContactVM> ViewModelList { get; set; }
        public ContactVM CurrentViewModel { get; set; }
    }
}
