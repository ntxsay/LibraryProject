using LibraryProjectUWP.Code;
using LibraryProjectUWP.Views.Book;
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
        public BookCollectionPage ParentPage { get; set; }
    }

    public class BookListParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
    }

    public class BookCategorieParametersDriverVM
    {
        public BibliothequeVM ParentLibrary { get; set; }
        public LivreVM CurrentViewModel { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }
}
