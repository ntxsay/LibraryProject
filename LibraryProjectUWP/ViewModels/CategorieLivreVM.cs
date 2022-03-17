using LibraryProjectUWP.Code;
using LibraryProjectUWP.ViewModels.Book;
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
    public class CategorieLivreVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        private string _Name { get; set; }
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

        private ObservableCollection<SubCategorieLivreVM> _SubCategorieLivres = new ObservableCollection<SubCategorieLivreVM>();
        public ObservableCollection<SubCategorieLivreVM> SubCategorieLivres
        {
            get => _SubCategorieLivres;
            set
            {
                if (_SubCategorieLivres != value)
                {
                    _SubCategorieLivres = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<LivreVM> Livres { get; set; }
        private List<long> _BooksId = new List<long>();
        public List<long> BooksId
        {
            get => _BooksId;
            set
            {
                if (_BooksId != value)
                {
                    _BooksId = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsExpanded = true;
        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SubCategorieLivreVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long Id { get; set; }
        public long IdCategorie { get; set; }
        private string _Name { get; set; }
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

        public List<LivreVM> Livres { get; set; }

        private List<long> _BooksId = new List<long>();
        public List<long> BooksId
        {
            get => _BooksId;
            set
            {
                if (_BooksId != value)
                {
                    _BooksId = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsExpanded = true;
        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
