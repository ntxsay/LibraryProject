using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class ManageBookParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class BookListParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
    }
}
