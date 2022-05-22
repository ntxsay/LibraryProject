using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryProjectUWP.Code;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Library;

namespace LibraryProjectUWP.ViewModels.General
{

    public class ManageBookParametersVM
    {
        public LivreVM ViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public string ImageBackgroundPath { get; set; }
        public ManageContainerPage ParentPage { get; set; }
    }

    public class ManageLibraryParametersVM
    {
        public IEnumerable<BibliothequeVM> ViewModelList { get; set; }
        public EditMode EditMode { get; set; }
        public string ImageBackgroundPath { get; set; }
        public ManageContainerPage ParentPage { get; set; }
    }

    public class ManageBookParentChildVM
    {
        public LivreVM ViewModel { get; set; }
        //public ManageBookPage ParentPage { get; set; }
        public ManageBookParametersVM Parameters { get; set; }
    }

    public class ManageLibraryParentChildVM
    {
        public BibliothequeVM ViewModel { get; set; }
        public IEnumerable<BibliothequeVM> ViewModelList { get; set; }
        //public ManageLibraryPage ParentPage { get; set; }
        public ManageLibraryParametersVM Parameters { get; set; }
    }

    public class LibraryCollectionParentChildParamsVM
    {
        public IEnumerable<BibliothequeVM> ViewModelList { get; set; }
        public IGrouping<string, BibliothequeVM> SelectedGroupedItem { get; set; }
        public LibraryCollectionPage ParentPage { get; set; }
    }

    public class BookCollectionParentChildParamsVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

}
