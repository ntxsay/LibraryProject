using LibraryProjectUWP.Code;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.PrincipalPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class ManageBookParametersDriverVM
    {
        public LivreVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    public class ImportBookParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public BookCollectionPage ParentPage { get; set; }
        public StorageFile File { get; set; }
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
        public LivreVM ParentBook { get; set; }
        public EditMode EditMode { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    public class BookPretListParametersDriverVM
    {
        public LivreVM ParentBook { get; set; }
        [Obsolete]
        public IEnumerable<LivrePretVM> ViewModelList { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    [Obsolete("N'est pas utilisé donc sera supprimé par la suite")]
    public class BookListParametersDriverVM
    {
        public IEnumerable<LivreVM> ViewModelList { get; set; }
        public LivreVM CurrentViewModel { get; set; }
    }

    public class BookExemplaryListParametersDriverVM
    {
        [Obsolete]
        public long BookId { get; set; }
        [Obsolete]
        public string BookTitle { get; set; }
        public LivreVM ParentBook { get; set; }
        [Obsolete]
        public IEnumerable<LivreExemplaryVM> ViewModelList { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

    public class BookSubPageParametersDriverVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public BookCollectionPage ParentPage { get; set; }
        public DataTable DataTable { get; set; }
        private ObservableCollection<LivreVM> _ViewModelList = new ObservableCollection<LivreVM>();
        public ObservableCollection<LivreVM> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (_ViewModelList != value)
                {
                    this._ViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
