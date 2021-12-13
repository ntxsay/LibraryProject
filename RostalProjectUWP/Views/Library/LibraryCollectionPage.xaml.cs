using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Db;
using RostalProjectUWP.Code.Services.ES;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.ViewModels.Library;
using RostalProjectUWP.Views.Library.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Library
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionPage : Page
    {
        public LibraryCollectionPageVM ViewModelPage { get; set; } = new LibraryCollectionPageVM();
        public LibraryCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var libraryList = await DbServices.Library.AllVMAsync();
                ViewModelPage.ViewModelList = libraryList?.ToList();
                await InitializeDataAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(imageCtrl?.Tag?.ToString());
                    imageCtrl.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task InitializeDataAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ViewModelList != null && ViewModelPage.ViewModelList.Any())
                {
                    EsLibrary esLibrary = new EsLibrary();
                    foreach (var library in ViewModelPage.ViewModelList)
                    {
                        string combinedPath = await esLibrary.GetLibraryItemJaquettePathAsync(library);
                        library.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    }
                }

                ViewModelPage.SearchingLibraryVisibility = Visibility.Collapsed;
                NavigateToView(typeof(LibraryCollectionGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                ViewModelPage.IsGridView = true;
                ViewModelPage.IsDataGridView = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region Navigation
        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = FramePartialView.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewCollectionXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!(FramePartialView.Content is LibraryCollectionGridViewPage))
                {
                    NavigateToView(typeof(LibraryCollectionGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                }

                ViewModelPage.IsGridView = true;
                ViewModelPage.IsDataGridView = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridViewCollectionXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!(FramePartialView.Content is LibraryCollectionDataGridViewPage))
                {
                    NavigateToView(typeof(LibraryCollectionDataGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                }

                ViewModelPage.IsGridView = false;
                ViewModelPage.IsDataGridView = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Sort - Group - Order
        private void GroupByLetterXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupItemsByAlphabetic();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupItemsByAlphabetic();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GroupByCreationYearXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupByCreationYear();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupByCreationYear();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GroupByNoneXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupItemsByNone();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupItemsByNone();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void OrderByCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.Croissant;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void OrderByDCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.DCroissant;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SortByNameXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = LibraryGroupVM.SortBy.Name;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SortByDateCreationXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = LibraryGroupVM.SortBy.DateCreation;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void RefreshItemsGrouping()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    switch (ViewModelPage.GroupedBy)
                    {
                        case LibraryGroupVM.GroupBy.None:
                            libraryCollectionGridViewPage.GroupItemsByNone();
                            break;
                        case LibraryGroupVM.GroupBy.Letter:
                            libraryCollectionGridViewPage.GroupItemsByAlphabetic();
                            break;
                        case LibraryGroupVM.GroupBy.CreationYear:
                            libraryCollectionGridViewPage.GroupByCreationYear();
                            break;
                        default:
                            libraryCollectionGridViewPage.GroupItemsByNone();
                            break;
                    }
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    switch (ViewModelPage.GroupedBy)
                    {
                        case LibraryGroupVM.GroupBy.None:
                            libraryCollectionDataGridViewPage.GroupItemsByNone();
                            break;
                        case LibraryGroupVM.GroupBy.Letter:
                            libraryCollectionDataGridViewPage.GroupItemsByAlphabetic();
                            break;
                        case LibraryGroupVM.GroupBy.CreationYear:
                            libraryCollectionDataGridViewPage.GroupByCreationYear();
                            break;
                        default:
                            libraryCollectionDataGridViewPage.GroupItemsByNone();
                            break;
                    }
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Search
        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var FilteredItems = new List<BibliothequeVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");
                foreach (var value in ViewModelPage.ViewModelList)
                {
                    if (value.Name == null) continue;
                    var found = splitSearchTerm.All((key) => {
                        return value.Name.Contains(key.ToLower());
                    });

                    if (found)
                    {
                        FilteredItems.Add(value);
                    }
                }

                if (FilteredItems.Count == 0)
                {
                    FilteredItems.Add(new BibliothequeVM()
                    {
                        Id = -1,
                        Name = "Aucun résultat trouvé",
                    });
                }
                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchItem_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is BibliothequeVM value)
                {
                    sender.Text = value.Name;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null &&  args.ChosenSuggestion is BibliothequeVM viewModel)
                {
                    this.SearchViewModel(viewModel);
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchViewModel(BibliothequeVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.SearchViewModel(viewModel);
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
        
    }

    public class LibraryCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


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

        private bool _IsDataGridView;
        public bool IsDataGridView
        {
            get => this._IsDataGridView;
            set
            {
                if (_IsDataGridView != value)
                {
                    this._IsDataGridView = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsGridView;
        public bool IsGridView
        {
            get => this._IsGridView;
            set
            {
                if (_IsGridView != value)
                {
                    this._IsGridView = value;
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
