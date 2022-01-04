using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Author
{

    public class ManageAuthorParametersDriverVM
    {
        public IEnumerable<AuthorVM> ViewModelList { get; set; }
        public AuthorVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class AuthorListParametersDriverVM
    {
        public IEnumerable<AuthorVM> ViewModelList { get; set; }
        public AuthorVM CurrentViewModel { get; set; }
    }
}
