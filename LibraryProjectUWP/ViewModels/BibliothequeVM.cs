using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels
{
    public class BibliothequeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public long Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        private DateTime _DateAjout = DateTime.UtcNow;
        public DateTime DateAjout
        {
            get => _DateAjout;
            set
            {
                if (_DateAjout != value)
                {
                    _DateAjout = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _DateEdition;
        public DateTime? DateEdition
        {
            get => _DateEdition;
            set
            {
                if (_DateEdition != value)
                {
                    _DateEdition = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Description;
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }
        private List<LivreVM> _Books = new List<LivreVM>();
        [Obsolete]
        public List<LivreVM> Books
        {
            get => _Books;
            set
            {
                if (_Books != value)
                {
                    _Books = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _CountBooks;
        public long CountBooks
        {
            get => _CountBooks;
            set
            {
                if (_CountBooks != value)
                {
                    _CountBooks = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _CountUnCategorizedBooks;
        public long CountUnCategorizedBooks
        {
            get => _CountUnCategorizedBooks;
            set
            {
                if (_CountUnCategorizedBooks != value)
                {
                    _CountUnCategorizedBooks = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _CountNotInCollectionBooks;
        public long CountNotInCollectionBooks
        {
            get => _CountNotInCollectionBooks;
            set
            {
                if (_CountNotInCollectionBooks != value)
                {
                    _CountNotInCollectionBooks = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CategorieLivreVM> _Categories = new ObservableCollection<CategorieLivreVM>();
        public ObservableCollection<CategorieLivreVM> Categories
        {
            get => _Categories;
            set
            {
                if (_Categories != value)
                {
                    _Categories = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CollectionVM> _Collections = new ObservableCollection<CollectionVM>();
        public ObservableCollection<CollectionVM> Collections
        {
            get => _Collections;
            set
            {
                if (_Collections != value)
                {
                    _Collections = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _JaquettePath = EsLibrary.LibraryDefaultJaquette;
        public string JaquettePath
        {
            get => this._JaquettePath;
            set
            {
                if (this._JaquettePath != value)
                {
                    this._JaquettePath = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
