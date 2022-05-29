using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Library;
using LibraryProjectUWP.Views.PrincipalPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Categorie
{
    public class CategorieParameterDriverVM
    {
        public BibliothequeVM ParentLibrary { get; set; }
        public LivreVM CurrentViewModel { get; set; }
        public BookCollectionPage BookPage { get; set; }
        public LibraryCollectionPage LibraryPage { get; set; }
    }
}
