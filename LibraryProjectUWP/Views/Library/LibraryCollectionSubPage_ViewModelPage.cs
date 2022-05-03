using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Views.Library
{
    public sealed partial class LibraryCollectionSubPage
    {
        public class LibraryCollectionPageVM : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public ToastContentBuilder OperationRunning
            {
                get => new ToastContentBuilder()
                            .AddText($"Une tâche en cours d'exécution")
                            .AddText("Une ou plusieurs tâches d'arrière-plan sont en cours d'exécution, nous vous prions de patienter quelques instants.");
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

            private BibliothequeVM _SearchedViewModel = null;
            public BibliothequeVM SearchedViewModel
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

            private int _MaxItemsPerPage = 20;
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

            private ICollection<BibliothequeVM> _SelectedItems = new List<BibliothequeVM>();
            public ICollection<BibliothequeVM> SelectedItems
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

            private LibraryGroupVM _GroupedRelatedViewModel = new LibraryGroupVM();
            public LibraryGroupVM GroupedRelatedViewModel
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

            private LibraryGroupVM.GroupBy _GroupedBy = LibraryGroupVM.GroupBy.None;
            public LibraryGroupVM.GroupBy GroupedBy
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

            private LibraryGroupVM.SortBy _SortedBy = LibraryGroupVM.SortBy.Name;
            public LibraryGroupVM.SortBy SortedBy
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

            private LibraryGroupVM.OrderBy _OrderedBy = LibraryGroupVM.OrderBy.Croissant;
            public LibraryGroupVM.OrderBy OrderedBy
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

            private List<BibliothequeVM> _ViewModelList = new List<BibliothequeVM>();
            public List<BibliothequeVM> ViewModelList
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
