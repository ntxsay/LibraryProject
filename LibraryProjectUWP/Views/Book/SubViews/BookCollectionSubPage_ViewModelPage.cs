using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookCollectionSubPage
    {
        public class BookCollectionSubPageVM : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            private IEnumerable<CollectionVM> _SelectedCollections = null;
            public IEnumerable<CollectionVM> SelectedCollections
            {
                get => this._SelectedCollections;
                set
                {
                    if (_SelectedCollections != value)
                    {
                        this._SelectedCollections = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private bool _DisplayUnCategorizedBooks = false;
            public bool DisplayUnCategorizedBooks
            {
                get => this._DisplayUnCategorizedBooks;
                set
                {
                    if (_DisplayUnCategorizedBooks != value)
                    {
                        this._DisplayUnCategorizedBooks = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private IEnumerable<object> _SelectedSCategories = null;
            public IEnumerable<object> SelectedSCategories
            {
                get => this._SelectedSCategories;
                set
                {
                    if (_SelectedSCategories != value)
                    {
                        this._SelectedSCategories = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            

            private BookGroupVM _GroupedRelatedViewModel = new BookGroupVM();
            public BookGroupVM GroupedRelatedViewModel
            {
                get => this._GroupedRelatedViewModel;
                set
                {
                    if (this._GroupedRelatedViewModel != value)
                    {
                        this._GroupedRelatedViewModel = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private BookGroupVM.GroupBy _GroupedBy = BookGroupVM.GroupBy.None;
            public BookGroupVM.GroupBy GroupedBy
            {
                get => this._GroupedBy;
                set
                {
                    if (this._GroupedBy != value)
                    {
                        this._GroupedBy = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private BookGroupVM.SortBy _SortedBy = BookGroupVM.SortBy.Name;
            public BookGroupVM.SortBy SortedBy
            {
                get => this._SortedBy;
                set
                {
                    if (this._SortedBy != value)
                    {
                        this._SortedBy = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private BookGroupVM.OrderBy _OrderedBy = BookGroupVM.OrderBy.Croissant;
            public BookGroupVM.OrderBy OrderedBy
            {
                get => this._OrderedBy;
                set
                {
                    if (this._OrderedBy != value)
                    {
                        this._OrderedBy = value;
                        this.OnPropertyChanged();
                    }
                }
            }



            private int _MaxItemsPerPage = 100;
            public int MaxItemsPerPage
            {
                get => this._MaxItemsPerPage;
                set
                {
                    if (_MaxItemsPerPage != value)
                    {
                        this._MaxItemsPerPage = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private int _SelectedPage;
            public int SelectedPage
            {
                get => this._SelectedPage;
                set
                {
                    if (_SelectedPage != value)
                    {
                        this._SelectedPage = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private int _CountPages;
            public int CountPages
            {
                get => this._CountPages;
                set
                {
                    if (_CountPages != value)
                    {
                        this._CountPages = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private LivreVM _SearchedViewModel = null;
            public LivreVM SearchedViewModel
            {
                get => this._SearchedViewModel;
                set
                {
                    if (_SearchedViewModel != value)
                    {
                        this._SearchedViewModel = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private ObservableCollection<PageSystemVM> _PagesList = new ObservableCollection<PageSystemVM>();
            public ObservableCollection<PageSystemVM> PagesList
            {
                get => this._PagesList;
                set
                {
                    if (_PagesList != value)
                    {
                        this._PagesList = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private int _SelectedPivotIndex;
            public int SelectedPivotIndex
            {
                get => this._SelectedPivotIndex;
                set
                {
                    if (_SelectedPivotIndex != value)
                    {
                        this._SelectedPivotIndex = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private object _SelectedCategorie;
            public object SelectedCategorie
            {
                get => this._SelectedCategorie;
                set
                {
                    if (_SelectedCategorie != value)
                    {
                        this._SelectedCategorie = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private ICollection<LivreVM> _SelectedItems = new List<LivreVM>();
            [Obsolete]
            public ICollection<LivreVM> SelectedItems
            {
                get => this._SelectedItems;
                set
                {
                    if (_SelectedItems != value)
                    {
                        this._SelectedItems = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private List<LivreVM> _ViewModelList;
            [Obsolete]
            public List<LivreVM> ViewModelList
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
}
