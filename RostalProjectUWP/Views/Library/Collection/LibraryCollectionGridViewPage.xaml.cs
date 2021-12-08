using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.ViewModels.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Library.Collection
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionGridViewPage : Page
    {
        public LibraryCollectionPageVM ViewModelPage { get; set; } = new LibraryCollectionPageVM();
        private LibraryCollectionParentChildParamsVM _libraryParameters;
        public LibraryCollectionGridViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryCollectionParentChildParamsVM libraryParameters)
            {
                _libraryParameters = libraryParameters;
                ViewModelPage.ViewModelList = _libraryParameters.ViewModelList?.ToList();
                InitializeModelList();
            }
        }

        private void InitializeModelList()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                this.ItemsGroupOnStartUp();

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region Groups
        private void ItemsGroupOnStartUp()
        {
            try
            {
                var groupItems = GetGroupedItemsByNone;
                if (groupItems == null || !groupItems.Any())
                {
                    return;
                }

                if (ViewModelPage.GroupedRelatedViewModel.Collection != null)
                {
                    foreach (IGrouping<string, BibliothequeVM> item in groupItems)
                    {
                        ViewModelPage.GroupedRelatedViewModel.Collection.Add(item);
                    }
                }
                else
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(groupItems);
                    this.Bindings.Update();
                }

                //ViewModelPage.GroupedRelatedViewModel.GroupedBy = LibraryGroupVM.GroupBy.None;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<IGrouping<string, BibliothequeVM>> GetGroupedItemsByNone
        {
            get
            {
                try
                {
                    if (ViewModelPage.ViewModelList == null || !this.ViewModelPage.ViewModelList.Any())
                    {
                        return Enumerable.Empty<IGrouping<string, BibliothequeVM>>();
                    }

                    var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, ViewModelPage.GroupedRelatedViewModel.OrderedBy, ViewModelPage.GroupedRelatedViewModel.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems == null || !GroupingItems.Any()) return Enumerable.Empty<IGrouping<string, BibliothequeVM>>();

                    return GroupingItems;
                }
                catch (Exception)
                {
                    return Enumerable.Empty<IGrouping<string, BibliothequeVM>>();
                }
            }
        }

        private void GroupItemsByNone()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, ViewModelPage.GroupedRelatedViewModel.OrderedBy, ViewModelPage.GroupedRelatedViewModel.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    ViewModelPage.GroupedRelatedViewModel.GroupedBy = LibraryGroupVM.GroupBy.None;
                    this.Bindings.Update();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        private void GroupItemsByAlphabetic()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, ViewModelPage.GroupedRelatedViewModel.OrderedBy, ViewModelPage.GroupedRelatedViewModel.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Name.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    ViewModelPage.GroupedRelatedViewModel.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                    this.Bindings.Update();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GroupByDebutDiffusionYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, ViewModelPage.GroupedRelatedViewModel.OrderedBy, ViewModelPage.GroupedRelatedViewModel.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    ViewModelPage.GroupedRelatedViewModel.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
                    this.Bindings.Update();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Group-Orders
        private IEnumerable<BibliothequeVM> OrderItems(IEnumerable<BibliothequeVM> Collection, LibraryGroupVM.OrderBy OrderBy = LibraryGroupVM.OrderBy.Croissant, LibraryGroupVM.SortBy SortBy = LibraryGroupVM.SortBy.Name)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == LibraryGroupVM.SortBy.Name)
                {
                    if (OrderBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name);
                    }
                    else if (OrderBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name);
                    }
                }
                else if (SortBy == LibraryGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.OrderByDescending(o => o.DateAjout);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<BibliothequeVM>();
            }
        }

        #endregion

        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GridViewItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class LibraryCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        private double _loadItemPurcentValue;

        public double LoadItemPurcentValue
        {
            get => this._loadItemPurcentValue;
            set
            {
                if (_loadItemPurcentValue != value)
                {
                    this._loadItemPurcentValue = value;
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

        private int _countItems;
        public int CountItems
        {
            get => this._countItems;
            set
            {
                if (_countItems != value)
                {
                    this._countItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _countSelectedItems;
        public int CountSelectedItems
        {
            get => this._countSelectedItems;
            set
            {
                if (_countSelectedItems != value)
                {
                    this._countSelectedItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private List<BibliothequeVM> _ViewModelList;
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
