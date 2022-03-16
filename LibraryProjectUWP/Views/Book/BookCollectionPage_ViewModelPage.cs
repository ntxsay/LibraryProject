using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        public class BookCollectionPageVM : INotifyPropertyChanged
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

            private ObservableCollection<TaskVM> _TaskList = new ObservableCollection<TaskVM>();
            public ObservableCollection<TaskVM> TaskList
            {
                get => this._TaskList;
                set
                {
                    if (_TaskList != value)
                    {
                        this._TaskList = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private ObservableCollection<SideBarItemHeaderVM> _ItemsSideBarHeader = new ObservableCollection<SideBarItemHeaderVM>();
            public ObservableCollection<SideBarItemHeaderVM> ItemsSideBarHeader
            {
                get => this._ItemsSideBarHeader;
                set
                {
                    if (_ItemsSideBarHeader != value)
                    {
                        this._ItemsSideBarHeader = value;
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


            private bool _IsSplitViewOpen;
            public bool IsSplitViewOpen
            {
                get => this._IsSplitViewOpen;
                set
                {
                    if (_IsSplitViewOpen != value)
                    {
                        this._IsSplitViewOpen = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public const double MinSplitViewWidth = 400;

            private double _SplitViewWidth = MinSplitViewWidth;
            public double SplitViewWidth
            {
                get => this._SplitViewWidth;
                set
                {
                    if (_SplitViewWidth != value)
                    {
                        this._SplitViewWidth = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private Visibility _SearchingLibraryVisibility = Visibility.Visible;
            public Visibility SearchingLibraryVisibility
            {
                get => this._SearchingLibraryVisibility;
                set
                {
                    if (_SearchingLibraryVisibility != value)
                    {
                        this._SearchingLibraryVisibility = value;
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

            private List<ContactVM> _ContactViewModelList;
            public List<ContactVM> ContactViewModelList
            {
                get => this._ContactViewModelList;
                set
                {
                    if (_ContactViewModelList != value)
                    {
                        this._ContactViewModelList = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private List<ContactVM> _AuthorViewModelList;
            public List<ContactVM> AuthorViewModelList
            {
                get => this._AuthorViewModelList;
                set
                {
                    if (_AuthorViewModelList != value)
                    {
                        this._AuthorViewModelList = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private BibliothequeVM _ParentLibrary;
            public BibliothequeVM ParentLibrary
            {
                get => this._ParentLibrary;
                set
                {
                    if (_ParentLibrary != value)
                    {
                        this._ParentLibrary = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            private string _BackgroundImagePath = EsGeneral.BookDefaultBackgroundImage;
            public string BackgroundImagePath
            {
                get => this._BackgroundImagePath;
                set
                {
                    if (this._BackgroundImagePath != value)
                    {
                        this._BackgroundImagePath = value;
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
