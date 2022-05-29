using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class ImportItemsFromTablePageDriverVM
    {
        public MainPage MainPage { get; set; }
        public BibliothequeVM ParentLibrary { get; set; }
        //public BookCollectionPage BookCollectionPage { get; set; }
        public DataTable DataTable { get; set; }
    }
}
