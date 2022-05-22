using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LibraryToBookNavigationDriverVM
    {
        public MainPage MainPage { get; set; }
        public BibliothequeVM ParentLibrary { get; set; }
        public LibraryCollectionPage ParentPage { get; set; }
    }
}
