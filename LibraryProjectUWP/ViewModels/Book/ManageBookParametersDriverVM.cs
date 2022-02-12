using LibraryProjectUWP.Code;
using LibraryProjectUWP.Views.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class ManageBookParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    public class ImportBookParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public BookCollectionPage ParentPage { get; set; }
        public StorageFile ExcelFile { get; set; }
    }

    public class BookImportDataTableVM
    {
        public int ColumnIndex { get; set; }
        public string ColumnName { get; set; }
        public string RowName { get; set; }
    }

    public class ManageBookExemplaryParametersDriverVM
    {
        public IEnumerable<LivreExemplaryVM> ExemplaryViewModelList { get; set; }
        public LivreExemplaryVM CurrentViewModel { get; set; }
        public LivreVM Parent { get; set; }
        public EditMode EditMode { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    public class BookListParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
    }

    [Obsolete]
    public class BookCategorieParametersDriverVM
    {
        public BibliothequeVM ParentLibrary { get; set; }
        public LivreVM CurrentViewModel { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }
}
