using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Categories;
using LibraryProjectUWP.Views.Collection;
using LibraryProjectUWP.Views.Contact;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LibraryProjectUWP.Code.Extensions;
using System.Diagnostics;
using Windows.UI.Core;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookCollectionPage : Page
    {
        public BookCollectionPageVM ViewModelPage { get; set; } = new BookCollectionPageVM();
        public LibraryToBookNavigationDriverVM _parameters;
        readonly EsBook esBook = new EsBook();

        public BookCollectionPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryToBookNavigationDriverVM parameters)
            {
                _parameters = parameters;
                ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
                ViewModelPage.BackgroundImagePath = await esBook.GetBookCollectionBackgroundImagePathAsync();
            }
        }

        #region Loading
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync(true);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                _parameters.ParentLibrary.Books.Clear();
                _parameters.ParentLibrary.CountBooks = 0;
                ViewModelPage = null;
                //_parameters = null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ReloadDataXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await LoadDataAsync(false);
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

        private async Task LoadDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _parameters.ParentPage?.InitializeSearchingBookWorker(_parameters.ParentLibrary);
                //var bookList = await DbServices.Book.MultipleVmWithIdLibraryAsync(_parameters.ParentLibrary.Id);
                //_parameters.ParentLibrary.Books = bookList?.ToList() ?? new List<LivreVM>(); ;
                await InitializeDataAsync(firstLoad);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task InitializeDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (_parameters.ParentLibrary.Books != null && _parameters.ParentLibrary.Books.Any())
                {
                    EsBook esBook = new EsBook();
                    foreach (var book in _parameters.ParentLibrary.Books)
                    {
                        string combinedPath = await esBook.GetBookItemJaquettePathAsync(book);
                        book.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    }
                }


                ViewModelPage.SearchingLibraryVisibility = Visibility.Collapsed;
                this.GridViewMode(firstLoad);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void InitializePages()
        {
            Button btn = new Button();
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.PagesList.Clear();

                if (_parameters.ParentLibrary.Books != null && _parameters.ParentLibrary.Books.Any())
                {
                    int nbPageDefault = _parameters.ParentLibrary.Books.Count() / ViewModelPage.MaxItemsPerPage;
                    double nbPageExact = _parameters.ParentLibrary.Books.Count() / Convert.ToDouble(ViewModelPage.MaxItemsPerPage);
                    int nbPageRounded = nbPageExact > nbPageDefault ? nbPageDefault + 1 : nbPageDefault;
                    ViewModelPage.CountPages = nbPageRounded;

                    for (int i = 0; i < ViewModelPage.CountPages; i++)
                    {
                        ViewModelPage.PagesList.Add(new PageSystemVM()
                        {
                            CurrentPage = i + 1,
                            IsPageSelected = i == 0,
                            BackgroundColor = i == 0 ? Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush : Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is GridView gridView)
                {
                    if (ViewModelPage.SearchedViewModel != null)
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            if (gridViewItem is LivreVM _viewModel && _viewModel.Id == ViewModelPage.SearchedViewModel.Id)
                            {
                                if (gridView.SelectedItem != gridViewItem)
                                {
                                    gridView.SelectedItem = gridViewItem;
                                }

                                var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                OpenFlyoutSearchedItemGridView(_gridViewItemContainer);
                                break;
                            }
                        }
                    }

                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (gridViewItem is LivreVM _viewModel && _viewModel.Id == item.Id && !gridView.SelectedItems.Contains(item))
                                {
                                    gridView.SelectedItems.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                    gridView.SelectionChanged += GridViewItems_SelectionChanged;
                    gridView.Focus(FocusState.Pointer);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    if (ViewModelPage.SearchedViewModel != null)
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            if (dataGridItem is LivreVM _viewModel && _viewModel.Id == ViewModelPage.SearchedViewModel.Id)
                            {
                                if (dataGrid.SelectedItem != dataGridItem)
                                {
                                    dataGrid.SelectedItem = dataGridItem;
                                }
                            }
                        }
                    }

                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (dataGridItem is LivreVM _viewModel && _viewModel.Id == item.Id && !dataGrid.SelectedItems.Contains(item))
                                {
                                    dataGrid.SelectedItems.Add(item);
                                    break;
                                }
                            }
                        }
                    }

                    dataGrid.SelectionChanged += DataGridItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    dataGrid.SelectionChanged -= DataGridItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is GridView gridView)
                {
                    gridView.SelectionChanged -= GridViewItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void PivotItems_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            try
            {
                if (ViewModelPage.SearchedViewModel == null)
                {
                    return;
                }

                foreach (var pivotItem in sender.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == ViewModelPage.SearchedViewModel.Id))
                    {
                        if (this.PivotItems.SelectedItem != pivotItem)
                        {
                            this.PivotItems.SelectedItem = pivotItem;
                        }
                        break;

                    }
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

        #region Selection
        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    this.ViewModelPage.SelectedItems = new List<LivreVM>();
                    this.ViewModelPage.SelectedPivotIndex = pivot.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is GridView gridView)
                {
                    this.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<LivreVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    this.ViewModelPage.SelectedItems = dataGrid.SelectedItems.Cast<LivreVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Lv_SelectedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem is LivreVM viewModel)
                {
                    SearchViewModel(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Btn_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = this.GetSelectedGridView();
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectAll();
                    }
                }
                else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = this.GetSelectedDataGridItems();
                    if (dataGridItems != null)
                    {
                        foreach (var dataGridItem in dataGridItems.ItemsSource)
                        {
                            if (!dataGridItems.SelectedItems.Contains(dataGridItem))
                            {
                                dataGridItems.SelectedItems.Add(dataGridItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Btn_UnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = this.GetSelectedGridView();
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectedItems.Clear();
                    }
                }
                else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = this.GetSelectedDataGridItems();
                    if (dataGridItems != null)
                    {
                        dataGridItems.SelectedItems.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Paginations
        private void GotoPageXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is int page)
                {
                    GotoPage(page);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void GridViewItems_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (e.Key == Windows.System.VirtualKey.Q)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        var selectedPage = this.GetSelectedPage - 1;
                        if (selectedPage >= 1)
                        {
                            this.GotoPage(selectedPage);
                        }
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.D)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        var selectedPage = this.GetSelectedPage;
                        this.GotoPage(selectedPage + 1);
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.Z)
                {
                    if (sender is GridView gridView && gridView.Items.Count > 0)
                    {
                        gridView.SelectedItem = gridView.Items[0];
                    }
                }
                else if (e.Key == Windows.System.VirtualKey.S)
                {
                    if (sender is GridView gridView && gridView.Items.Count > 0)
                    {
                        gridView.SelectedItem = gridView.Items[gridView.Items.Count - 1];
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        } 
        #endregion

        #region Item MenuFlyout
        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    EsBook esBook = new EsBook();
                    var result = await esBook.ChangeBookItemJaquetteAsync(viewModel);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    var image = GetSelectedThumbnailImage(viewModel);
                    if (image != null)
                    {
                        var bitmapImage = await Files.BitmapImageFromFileAsync(viewModel.JaquettePath);
                        image.Source = bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ExportThisBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    await this.ExportThisBookAsync(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    // _commonView.DeleteLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        private void EditBookInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    this.EditBook(viewModel);
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

        #region Navigation
        private void ABTBtn_GridViewMode_Click(object sender, RoutedEventArgs e)
        {
            this.GridViewMode(true);
        }

        private void ABTBtn_GridViewMode_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton && toggleButton.IsChecked != true &&
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    toggleButton.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ABTBtn_DataGridViewMode_Click(object sender, RoutedEventArgs e)
        {
            this.DataGridViewMode(true);
        }

        private void ABTBtn_DataGridViewMode_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton && toggleButton.IsChecked != true &&
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    toggleButton.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                if (firstLoad)
                {
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode = Code.DataViewModeEnum.GridView;
                }
                this.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                if (firstLoad)
                {
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode = Code.DataViewModeEnum.DataGridView;
                }
                this.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
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
            this.GroupItemsByAlphabetic();
        }

        private void GroupByCreationYearXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupByCreationYear();
        }

        private void GroupByParutionYearXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupByParutionYear();
        }

        private void GroupByNoneXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupItemsByNone();
        }

        public void GroupItemsByNone(int goToPage = 1)
        {
            try
            {
                if (_parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        public void GroupItemsByAlphabetic(int goToPage = 1)
        {
            try
            {
                if (_parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear(int goToPage = 1)
        {
            try
            {
                if (_parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByParutionYear(int goToPage = 1)
        {
            try
            {
                if (_parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Publication.DateParution?.Year.ToString() ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.ParutionYear;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private IEnumerable<LivreVM> OrderItems(IEnumerable<LivreVM> Collection, BookGroupVM.OrderBy OrderBy = BookGroupVM.OrderBy.Croissant, BookGroupVM.SortBy SortBy = BookGroupVM.SortBy.Name)
        {
            try
            {
                if (Collection == null || !Collection.Any())
                {
                    return null;
                }

                if (SortBy == BookGroupVM.SortBy.Name)
                {
                    if (OrderBy == BookGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle);
                    }
                    else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle);
                    }
                }
                else if (SortBy == BookGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == BookGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
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
                return Enumerable.Empty<LivreVM>();
            }
        }


        private void OrderByCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = BookGroupVM.OrderBy.Croissant;
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
                ViewModelPage.OrderedBy = BookGroupVM.OrderBy.DCroissant;
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
                ViewModelPage.SortedBy = BookGroupVM.SortBy.Name;
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
                ViewModelPage.SortedBy = BookGroupVM.SortBy.DateCreation;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void RefreshItemsGrouping(int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ViewModelPage.GroupedBy)
                {
                    case BookGroupVM.GroupBy.None:
                        this.GroupItemsByNone(goToPage);
                        break;
                    case BookGroupVM.GroupBy.Letter:
                        this.GroupItemsByAlphabetic(goToPage);
                        break;
                    case BookGroupVM.GroupBy.CreationYear:
                        this.GroupByCreationYear(goToPage);
                        break;
                    case BookGroupVM.GroupBy.ParutionYear:
                        this.GroupByParutionYear(goToPage);
                        break;
                    default:
                        this.GroupItemsByNone(goToPage);
                        break;
                }

                if (resetPage)
                {
                    this.InitializePages();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion


        #region SideBar
        private void CmbxSideBarItemTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ComboBox cmbx && cmbx.SelectedItem is SideBarItemHeaderVM headerVM)
                {
                    foreach (var item in this.PivotRightSideBar.Items)
                    {
                        if (item is NewEditBookExemplaryUC bookExemplaryUC)
                        {
                            if (bookExemplaryUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                        else if (item is NewEditBookUC newEditBookUC)
                        {
                            if (newEditBookUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                        else if (item is NewEditCollectionUC newEditCollectionUC)
                        {
                            if (newEditCollectionUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                        else if (item is NewEditContactUC newEditContactUC)
                        {
                            if (newEditContactUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                        else if (item is ContactListUC contactListUC)
                        {
                            if (contactListUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                        else if (item is CollectionListUC collectionListUC)
                        {
                            if (collectionListUC.IdItem == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Events
        private async void Btn_Collection_Categorie_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.SelectedItems.Count > 0)
                {
                    var pivotItem = GetCategorieListSideBar();
                    if (pivotItem != null && pivotItem.ViewModelPage.SelectedCategorie != null)
                    {
                        if (pivotItem.ViewModelPage.SelectedCategorie is CategorieLivreVM categorie)
                        {
                            await AddBookToCategorie(pivotItem, ViewModelPage.SelectedItems.Select(s => s.Id), categorie);
                        }
                        else if (pivotItem.ViewModelPage.SelectedCategorie is SubCategorieLivreVM subCategorie)
                        {
                            await AddBookToCategorie(pivotItem, ViewModelPage.SelectedItems.Select(s => s.Id), pivotItem.GetParentCategorie(), subCategorie);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region New Book
        private void NewBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditBookUC userControl = new NewEditBookUC(new ManageBookParametersDriverVM()
                    {
                        ParentPage = this,
                        EditMode = Code.EditMode.Create,
                        ViewModelList = _parameters.ParentLibrary.Books,
                        CurrentViewModel = new LivreVM()
                        {
                            IdLibrary = _parameters.ParentLibrary.Id,
                        }
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditBookUC_Create_CreateItemRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    LivreVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Book.CreateAsync(newViewModel, _parameters.ParentLibrary.Id);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        this.CompleteBookInfos(newViewModel);
                        _parameters.ParentLibrary.Books.Add(newViewModel);
                        this.RefreshItemsGrouping();

                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

                sender.ViewModelPage.ViewModel = new LivreVM()
                {
                    IdLibrary = _parameters.ParentLibrary.Id,
                };
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditBookUC_Create_CancelModificationRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditBookUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditBookUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Edit Book
        public void EditBook(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditBookUC userControl = new NewEditBookUC(new ManageBookParametersDriverVM()
                    {
                        ParentPage = this,
                        EditMode = Code.EditMode.Edit,
                        ViewModelList = _parameters.ParentLibrary.Books,
                        CurrentViewModel = viewModel,
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_Edit_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditBookUC_Edit_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditBookUC_Edit_CancelModificationRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditBookUC_Edit_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditBookUC_Edit_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditBookUC_Edit_UpdateItemRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    var updatedViewModel = sender.ViewModelPage.ViewModel;

                    var updateResult = await DbServices.Book.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._parameters.CurrentViewModel.Copy(updatedViewModel);

                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditBookUC_Edit_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditBookUC_Edit_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Import Book
        public void ImportBook(StorageFile excelFile)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is ImportBookFromExcelUC);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    ImportBookFromExcelUC userControl = new ImportBookFromExcelUC(new ImportBookParametersDriverVM()
                    {
                        ParentPage = this,
                        ExcelFile = excelFile,
                        ViewModelList = _parameters.ParentLibrary.Books,
                    });

                    userControl.CancelModificationRequested += ImportBookFromExcelUC_CancelModificationRequested;
                    userControl.ImportDataRequested += ImportBookFromExcelUC_ImportDataRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ImportBookFromExcelUC_ImportDataRequested(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                List<LivreVM> newViewModelList = sender.ViewModelPage.NewViewModel;
                foreach (var newViewModel in newViewModelList)
                {
                    if (newViewModel.Auteurs != null && newViewModel.Auteurs.Any())
                    {
                        List<ContactVM> contactVMs = new List<ContactVM>();
                        foreach (var author in newViewModel.Auteurs)
                        {
                            var auteurResult = await DbServices.Contact.CreateAsync(author);
                            if (auteurResult.IsSuccess)
                            {
                                author.Id = auteurResult.Id;
                                contactVMs.Add(author);
                            }
                            else
                            {
                                //Erreur
                                sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                                sender.ViewModelPage.ResultMessage += "\n" + auteurResult.Message;
                                sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                                sender.ViewModelPage.IsResultMessageOpen = true;
                                continue;
                            }
                        }

                        newViewModel.Auteurs = contactVMs.Count > 0 ? new ObservableCollection<ContactVM>(contactVMs) : new ObservableCollection<ContactVM>();
                    }

                    if (newViewModel.Publication != null)
                    {
                        if (newViewModel.Publication.Collections != null && newViewModel.Publication.Collections.Any())
                        {
                            List<CollectionVM> collectionVMs = new List<CollectionVM>();
                            foreach (var collection in newViewModel.Publication.Collections)
                            {
                                var collectionResult = await DbServices.Collection.CreateAsync(collection, _parameters.ParentLibrary.Id);
                                if (collectionResult.IsSuccess)
                                {
                                    //collection.IdLibrary = _parameters.ParentLibrary.Id;
                                    collection.Id = collectionResult.Id;
                                    collectionVMs.Add(collection);
                                }
                                else
                                {
                                    //Erreur
                                    sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                                    sender.ViewModelPage.ResultMessage += "\n" + collectionResult.Message;
                                    sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                                    sender.ViewModelPage.IsResultMessageOpen = true;
                                    continue;
                                }
                            }
                            newViewModel.Publication.Collections = collectionVMs.Count > 0 ? new ObservableCollection<CollectionVM>(collectionVMs) : new ObservableCollection<CollectionVM>();
                        }
                    }

                    var creationResult = await DbServices.Book.CreateAsync(newViewModel, _parameters.ParentLibrary.Id);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        this.CompleteBookInfos(newViewModel);
                        _parameters.ParentLibrary.Books.Add(newViewModel);

                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
                

                sender.CancelModificationRequested -= ImportBookFromExcelUC_CancelModificationRequested;
                sender.ImportDataRequested -= ImportBookFromExcelUC_ImportDataRequested;

                this.RemoveItemToSideBar(sender);
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ImportBookFromExcelUC_CancelModificationRequested(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= ImportBookFromExcelUC_CancelModificationRequested;
                sender.ImportDataRequested -= ImportBookFromExcelUC_ImportDataRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Book Exemplary
        private void AddBookExemplaryXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookExemplaryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditBookExemplaryUC userControl = new NewEditBookExemplaryUC(new ManageBookExemplaryParametersDriverVM()
                    {
                        ParentPage = this,
                        EditMode = Code.EditMode.Create,
                        //ViewModelList = _parameters.ParentLibrary.Books,
                        Parent = args.Parameter as LivreVM,
                        CurrentViewModel = new LivreExemplaryVM()
                        {
                            Etat = new LivreEtatVM()
                            {
                                TypeVerification = Code.BookTypeVerification.Entree,
                            }
                        }
                    });

                    userControl.CancelModificationRequested += NewEditBookExemplaryUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookExemplaryUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditBookExemplaryUC_Create_CreateItemRequested(NewEditBookExemplaryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    LivreExemplaryVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Book.CreateExemplaryAsync(sender._parameters.Parent.Id, newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        this.RefreshItemsGrouping();

                        sender.ViewModelPage.ResultMessageTitle = "Succeès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

                sender.ViewModelPage.ViewModel = new LivreExemplaryVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditBookExemplaryUC_Create_CancelModificationRequested(NewEditBookExemplaryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditBookExemplaryUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditBookExemplaryUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void BookExemplaryListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is BookExemplaryListUC item);
                    if (checkedItem != null)
                    {
                        this.PivotRightSideBar.SelectedItem = checkedItem;
                    }
                    else
                    {
                        BookExemplaryListUC userControl = new BookExemplaryListUC(new BookExemplaryListParametersDriverVM()
                        {
                            ParentPage = this,
                            BookId = viewModel.Id,
                            BookTitle = viewModel.MainTitle,
                            
                        });

                        //userControl.CancelModificationRequested += NewEditBookExemplaryUC_Create_CancelModificationRequested;
                        //userControl.CreateItemRequested += NewEditBookExemplaryUC_Create_CreateItemRequested;

                        this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                        {
                            Glyph = userControl.ViewModelPage.Glyph,
                            Title = userControl.ViewModelPage.Header,
                            IdItem = userControl.IdItem,
                        });
                    }
                    this.ViewModelPage.IsSplitViewOpen = true;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        private async void ExportAllBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (_parameters.ParentLibrary.Books != null)
                {
                    var suggestedFileName = $"Rostalotheque_Livres_All_{DateTime.Now:yyyyMMddHHmmss}";

                    var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                    if (savedFile == null)
                    {
                        Logs.Log(m, "Le fichier n'a pas pû être créé.");
                        return;
                    }

                    //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(_parameters.ParentLibrary.Books, savedFile);// savedFile.Path
                    if (isFileSaved == false)
                    {
                        Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ChangeBackgroundImageXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var result = await esBook.ChangeBookCollectionBackgroundImageAsync();
                if (!result.IsSuccess)
                {
                    return;
                }

                ViewModelPage.BackgroundImagePath = result.Result?.ToString() ?? EsGeneral.BookDefaultBackgroundImage;
                if (ImageBackground != null)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(ViewModelPage.BackgroundImagePath);
                    ImageBackground.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task ExportThisBookAsync(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel != null)
                {
                    var suggestedFileName = $"Rostalotheque_Livre_{viewModel.MainTitle}_{DateTime.Now:yyyyMMddHHmmss}";

                    var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                    if (savedFile == null)
                    {
                        Logs.Log(m, "Le fichier n'a pas pû être créé.");
                        return;
                    }

                    //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(viewModel, savedFile);// savedFile.Path
                    if (isFileSaved == false)
                    {
                        Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region Contact
        internal async Task NewFreeContactAsync(string prenom, string nomNaissance, Guid? guid = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Contact.AllVMAsync();
                    var parameters = new ManageContactParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ContactType = Code.ContactType.Enterprise,
                        ContactTypeVisibility = Visibility.Visible,
                        ViewModelList = itemList,
                        CurrentViewModel = new ContactVM()
                        {
                            TitreCivilite = CivilityHelpers.MPoint,
                            NomNaissance = nomNaissance,
                            Prenom = prenom,
                        },
                    };

                    parameters.CurrentViewModel.ContactType = parameters.ContactType;

                    NewEditContactUC userControl = new NewEditContactUC(parameters);

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditFreeContactUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditFreeContactUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditFreeContactUC_Create_CreateItemRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    ContactVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Contact.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetBookExemplarySideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ViewModel.ContactSource = newViewModel;
                                NewEditFreeContactUC_Create_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditFreeContactUC_Create_CancelModificationRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditFreeContactUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditFreeContactUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewContactXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._parameters.ContactType == Code.ContactType.Adherant);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var contactsList = await DbServices.Contact.AllVMAsync();
                    var itemList = await DbServices.Contact.MultipleVMAsync(Code.ContactType.Adherant);
                    var parameters = new ManageContactParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ContactType = Code.ContactType.Adherant,
                        ViewModelList = itemList,
                        CurrentViewModel = new ContactVM() { TitreCivilite = CivilityHelpers.MPoint },
                    };

                    parameters.CurrentViewModel.ContactType = parameters.CurrentViewModel.ContactType;

                    NewEditContactUC userControl = new NewEditContactUC(parameters);


                    userControl.CancelModificationRequested += NewEditContactUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditContactUC_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditContactUC_CreateItemRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    ContactVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Contact.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        //ViewModelPage.ContactViewModelList.Add(newViewModel);
                        sender.ViewModelPage.ResultMessageTitle = "Succeès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditContactUC_CancelModificationRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditContactUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditContactUC_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void DisplayContactListXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(ContactListUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    IList<ContactVM> contactsList = await DbServices.Contact.AllVMAsync();
                    //if (ViewModelPage.ContactViewModelList == null || !ViewModelPage.ContactViewModelList.Any())
                    //{
                    //    ViewModelPage.ContactViewModelList = contactsList?.ToList();
                    //}

                    ContactListUC userControl = new ContactListUC(new ContactListParametersDriverVM()
                    {
                        ViewModelList = contactsList?.ToList(), //ViewModelPage.ContactViewModelList,
                        CurrentViewModel = new ContactVM()
                        {
                            TitreCivilite = CivilityHelpers.MPoint,
                        }
                    });


                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Author
        private async void NewAuthorXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await this.NewAuthorAsync(string.Empty, string.Empty);
        }

        internal async Task NewAuthorAsync(string prenom, string nomNaissance, Guid? guid = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._parameters.ContactType == Code.ContactType.Author);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Contact.MultipleVMAsync(Code.ContactType.Author);
                    ViewModelPage.AuthorViewModelList = itemList?.ToList();
                    var parameters = new ManageContactParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ContactType = Code.ContactType.Author,
                        ViewModelList = itemList,
                        CurrentViewModel = new ContactVM()
                        {
                            TitreCivilite = CivilityHelpers.MPoint,
                            NomNaissance = nomNaissance,
                            Prenom = prenom,
                        },
                    };

                    parameters.CurrentViewModel.ContactType = parameters.ContactType;

                    NewEditContactUC userControl = new NewEditContactUC(parameters);

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditAuthorUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditAuthorUC_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditAuthorUC_CreateItemRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    ContactVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Contact.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        //ViewModelPage.AuthorViewModelList.Add(newViewModel);
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetBookSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ViewModel.Auteurs.Add(newViewModel);
                                NewEditAuthorUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditAuthorUC_CancelModificationRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditAuthorUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditAuthorUC_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DisplayAuthorListXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }


        #endregion

        #region Collection
        private async void NewCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await NewCollectionAsync(string.Empty);
        }

        internal async Task NewCollectionAsync(string partName, Guid? guid = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCollectionUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Collection.AllVMAsync();
                    NewEditCollectionUC userControl = new NewEditCollectionUC(new ManageCollectionParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ViewModelList = itemList,
                        //ParentLibrary = _parameters.ParentLibrary,
                        CurrentViewModel = new CollectionVM()
                        {
                            IdLibrary = _parameters.ParentLibrary.Id,
                            Name = partName,
                        }
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditCollectionUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditCollectionUC_Create_CreateItemRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    CollectionVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Collection.CreateAsync(newViewModel, _parameters.ParentLibrary.Id);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succeès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetBookSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ViewModel.Publication.Collections.Add(newViewModel);
                                NewEditCollectionUC_Create_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

                sender.ViewModelPage.ViewModel = new CollectionVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditCollectionUC_Create_CancelModificationRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditCollectionUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditCollectionUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void DisplayCollectionListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(CollectionListUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    IList<CollectionVM> itemList = await DbServices.Collection.MultipleVmInLibraryAsync(_parameters.ParentLibrary.Id, Code.CollectionTypeEnum.Collection);
                    CollectionListUC userControl = new CollectionListUC(new CollectionListParametersDriverVM()
                    {
                        ParentLibrary = _parameters.ParentLibrary,
                        ViewModelList = itemList?.ToList(), //ViewModelPage.ContactViewModelList,
                    });

                    userControl.CancelModificationRequested += CollectionListUC_CancelModificationRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void CollectionListUC_CancelModificationRequested(CollectionListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= CollectionListUC_CancelModificationRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Editeurs
        private async void NewEditorXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await NewEditorAsync(string.Empty);
        }

        internal async Task NewEditorAsync(string partName, Guid? guid = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._parameters.ContactType == Code.ContactType.EditorHouse);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Contact.MultipleVMAsync(Code.ContactType.EditorHouse);
                    var parameters = new ManageContactParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ContactType = Code.ContactType.EditorHouse,
                        ViewModelList = itemList,
                        CurrentViewModel = new ContactVM() { ContactType = Code.ContactType.EditorHouse },
                    };

                    if (parameters.ContactType == Code.ContactType.EditorHouse)
                    {
                        parameters.CurrentViewModel.SocietyName = partName;
                    }

                    NewEditContactUC userControl = new NewEditContactUC(parameters);


                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditEditorUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditEditorUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditEditorUC_Create_CreateItemRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    ContactVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Contact.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succeès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetBookSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ViewModel.Publication.Editeurs.Add(newViewModel);
                                NewEditEditorUC_Create_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }

                sender.ViewModelPage.ViewModel = new ContactVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditEditorUC_Create_CancelModificationRequested(NewEditContactUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditEditorUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditEditorUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DisplayEditorListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }
        #endregion

        #region Events Categorie
        private void DisplayCategorieListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(CategoriesListUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    CategoriesListUC userControl = new CategoriesListUC(new CategorieParameterDriverVM()
                    {
                        BookPage = this,
                        ParentLibrary = ViewModelPage.ParentLibrary,
                    });

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


        private async Task AddBookToCategorie(CategoriesListUC categoriesListUC, IEnumerable<long> idBooks, CategorieLivreVM selectedCategorie, SubCategorieLivreVM selectedSubCategorie = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var creationResult = await DbServices.Categorie.CreateCategorieConnectorAsync(idBooks, selectedCategorie, selectedSubCategorie);
                if (creationResult.IsSuccess)
                {
                    categoriesListUC.ViewModelPage.ResultMessageTitle = "Succès";
                    categoriesListUC.ViewModelPage.ResultMessage = creationResult.Message;
                    categoriesListUC.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                    categoriesListUC.ViewModelPage.IsResultMessageOpen = true;

                    await UpdateLibraryCategoriesAsync();
                    //if (selectedSubCategorie != null)
                    //{
                    //    selectedSubCategorie.BooksId = (await DbServices.Categorie.GetBooksIdInSubCategorie(selectedSubCategorie.Id)).ToList();
                    //}
                    //else
                    //{
                    //    selectedCategorie.BooksId = (await DbServices.Categorie.GetBooksIdInCategorie(selectedCategorie.Id)).ToList();
                    //}

                }
                else
                {
                    //Erreur
                    categoriesListUC.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                    categoriesListUC.ViewModelPage.ResultMessage = creationResult.Message;
                    categoriesListUC.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    categoriesListUC.ViewModelPage.IsResultMessageOpen = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task UpdateLibraryCategoriesAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (_parameters.ParentLibrary != null && _parameters.ParentLibrary.Categories.Any())
                {
                    _parameters.ParentLibrary.Categories.Clear();
                    var categorieList = await DbServices.Categorie.MultipleVmAsync(_parameters.ParentLibrary.Id);
                    if (categorieList != null && categorieList.Any())
                    {
                        foreach (var category in categorieList)
                        {
                            _parameters.ParentLibrary.Categories.Add(category);
                        }

                        await DbServices.Categorie.AddSubCategoriesToCategoriesVmAsync(_parameters.ParentLibrary.Categories);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void AddNewCategory(BibliothequeVM parentLibrary, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._categorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ParentLibrary = parentLibrary,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditCategoryUC_CreateItemRequested;


                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void EditCategory(BibliothequeVM parentLibrary, CategorieLivreVM currentCategorie, Guid? guid = null)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit && item._categorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                    {
                        CurrentCategorie = currentCategorie,
                        EditMode = Code.EditMode.Edit,
                        ParentLibrary = parentLibrary,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditCategoryUC_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }

                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private async void NewEditCategoryUC_CreateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._categorieParameters != null)
                {
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var newViewModel = new CategorieLivreVM()
                    {
                        IdLibrary = sender._categorieParameters.ParentLibrary.Id,
                        Name = value,
                        Description = description,
                    };

                    var creationResult = await DbServices.Categorie.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ParentLibrary.Categories.Add(newViewModel);
                                NewEditCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void NewEditCategoryUC_UpdateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._categorieParameters != null)
                {
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var updatedViewModel = new CategorieLivreVM()
                    {
                        Id = sender._categorieParameters.CurrentCategorie.Id,
                        IdLibrary = sender._categorieParameters.ParentLibrary.Id,
                        Name = value,
                        Description = description,
                    };

                    var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                var item = bookManager.ViewModelPage.ParentLibrary.Categories.SingleOrDefault(s => s.Id == sender._categorieParameters.CurrentCategorie.Id);
                                if (item != null)
                                {
                                    item.Name = updatedViewModel.Name;
                                    item.Description = updatedViewModel.Description;
                                }
                                NewEditCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CreateItemRequested -= NewEditCategoryUC_CreateItemRequested;
                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditCategoryUC_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Events Sub Categorie
        public void AddNewSubCategory(CategorieLivreVM currentCategorieParent, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._subCategorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        Categorie = currentCategorieParent,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditSubCategoryUC_CreateItemRequested;


                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void EditSubCategory(CategorieLivreVM parentCategorie, SubCategorieLivreVM currentSubCategorie, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit && item._subCategorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Edit,
                        Categorie = parentCategorie,
                        CurrentSubCategorie = currentSubCategorie,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditSubCategoryUC_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditSubCategoryUC_CreateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._subCategorieParameters != null)
                {
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var newViewModel = new SubCategorieLivreVM()
                    {
                        IdCategorie = sender._subCategorieParameters.Categorie.Id,
                        Name = value,
                        Description = description,
                    };

                    var creationResult = await DbServices.Categorie.CreateSubCategorieAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        sender._subCategorieParameters.Categorie.SubCategorieLivres.Add(newViewModel);

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                            if (bookManager != null)
                            {
                                //bookManager.ViewModelPage.ParentLibrary.Categories.sub.Add(newViewModel);
                                NewEditSubCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void NewEditSubCategoryUC_UpdateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._subCategorieParameters != null)
                {
                    var newValue = sender.ViewModelPage.Value?.Trim();
                    var newDescription = sender.ViewModelPage.Description?.Trim();
                    var updatedViewModel = new SubCategorieLivreVM()
                    {
                        Id = sender._subCategorieParameters.CurrentSubCategorie.Id,
                        IdCategorie = sender._subCategorieParameters.Categorie.Id,
                        Name = newValue,
                        Description = newDescription,
                    };

                    var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            var item = sender._subCategorieParameters.Categorie.SubCategorieLivres.SingleOrDefault(s => s.Id == sender._subCategorieParameters.CurrentSubCategorie.Id);
                            if (item != null)
                            {
                                item.Name = updatedViewModel.Name;
                                item.Description = updatedViewModel.Description;
                            }
                            NewEditSubCategoryUC_CancelModificationRequested(sender, e);
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditSubCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CreateItemRequested -= NewEditSubCategoryUC_CreateItemRequested;
                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditSubCategoryUC_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
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
                //if (ViewModelPage.SearchedViewModel != null)
                //{
                //    ViewModelPage.SearchedViewModel = null;
                //}

                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || _parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                var FilteredItems = new List<LivreVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in _parameters.ParentLibrary.Books)
                {
                    if (value.MainTitle.IsStringNullOrEmptyOrWhiteSpace()) continue;

                    var found = splitSearchTerm.All((key) =>
                    {
                        return value.MainTitle.ToLower().Contains(key.ToLower());
                    });

                    if (found)
                    {
                        FilteredItems.Add(value);
                    }
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
                if (args.SelectedItem != null && args.SelectedItem is LivreVM value)
                {
                    sender.Text = value.MainTitle;
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
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is LivreVM viewModel)
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

        public void SearchViewModel(LivreVM viewModel)
        {
            if (viewModel == null) return;
            if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
            {
                SearchViewModelGridView(viewModel);
            }
            else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
            {
                SearchViewModelDataGridView(viewModel);
            }
        }

        public void SearchViewModelGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                foreach (var pageVm in ViewModelPage.PagesList)
                {
                    var search = GetPaginatedItems(pageVm.CurrentPage);
                    if (search != null && search.Any(f => f.Id == viewModel.Id))
                    {
                        ViewModelPage.SearchedViewModel = viewModel;
                        GotoPage(pageVm.CurrentPage);
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void OpenFlyoutSearchedItemGridView(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Grid gridActions = grid.Children.FirstOrDefault(f => f is Grid _gridActions && _gridActions.Name == "GridActions") as Grid;
                    if (gridActions != null)
                    {
                        Button buttonActions = gridActions.Children.FirstOrDefault(f => f is Button _buttonActions && _buttonActions.Name == "BtnActions") as Button;
                        if (buttonActions != null)
                        {
                            buttonActions.Flyout.ShowAt(buttonActions, new FlyoutShowOptions()
                            {
                                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                                ShowMode = FlyoutShowMode.Auto
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchViewModelDataGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                foreach (var pageVm in ViewModelPage.PagesList)
                {
                    var search = GetPaginatedItems(pageVm.CurrentPage);
                    if (search != null && search.Any(f => f.Id == viewModel.Id))
                    {
                        ViewModelPage.SearchedViewModel = viewModel;
                        GotoPage(pageVm.CurrentPage);
                        return;
                    }
                }

                return;
                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == viewModel.Id))
                    {
                        if (this.PivotItems.SelectedItem != pivotItem)
                        {
                            this.PivotItems.SelectedItem = pivotItem;
                        }

                        var _container = this.PivotItems.ContainerFromItem(pivotItem);
                        DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container);
                        while (dataGrid != null && dataGrid.Name != "DataGridItems")
                        {
                            dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(dataGrid);
                            if (dataGrid == null)
                            {
                                return;
                            }
                            else
                            {
                                if (dataGrid.Name == "DataGridItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (dataGrid != null)
                        {
                            foreach (var dataGridItem in dataGrid.ItemsSource)
                            {
                                if (dataGridItem is LivreVM _viewModel && _viewModel == viewModel)
                                {
                                    if (dataGrid.SelectedItem != dataGridItem)
                                    {
                                        dataGrid.SelectedItem = dataGridItem;
                                    }
                                }
                            }
                        }
                    }
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


        #region Functions
        public void GotoPage(int page)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                foreach (var pageVm in ViewModelPage.PagesList)
                {
                    if (pageVm.CurrentPage != page && pageVm.IsPageSelected == true)
                    {
                        pageVm.IsPageSelected = false;
                        pageVm.BackgroundColor = Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush;
                    }
                    else if (pageVm.CurrentPage == page && pageVm.IsPageSelected == false)
                    {
                        pageVm.IsPageSelected = true;
                        pageVm.BackgroundColor = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush;
                    }
                }
                this.RefreshItemsGrouping(page, false);
                var buttonsPage = VisualViewHelpers.FindVisualChilds<Button>(this.itemControlPageList);
                if (buttonsPage != null && buttonsPage.Any())
                {
                    var buttonPage = buttonsPage.FirstOrDefault(f => f.CommandParameter is int commandPage && commandPage == page);
                    if (buttonPage != null)
                    {
                        scrollVPages.ScrollToElement(buttonPage, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private int GetSelectedPage
        {
            get
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    var selectedPage = ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                    return selectedPage;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return 1;
                }
            }
        }

        private IEnumerable<LivreVM> GetPaginatedItems()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var selectedPage = ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                return GetPaginatedItems(selectedPage);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<LivreVM>();
            }
        }

        private IEnumerable<LivreVM> GetPaginatedItems(int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                IEnumerable<LivreVM> itemsPage = Enumerable.Empty<LivreVM>();

                //Si la séquence contient plus d'items que le nombre max éléments par page
                if (_parameters.ParentLibrary.Books.Count > ViewModelPage.MaxItemsPerPage)
                {
                    //Si la première page (ou moins ^^')
                    if (goToPage <= 1)
                    {
                        itemsPage = _parameters.ParentLibrary.Books.Take(ViewModelPage.MaxItemsPerPage);
                    }
                    else //Si plus que la première page
                    {
                        var nbItemsToSkip = ViewModelPage.MaxItemsPerPage * (goToPage - 1);
                        if (_parameters.ParentLibrary.Books.Count >= nbItemsToSkip)
                        {
                            var getRest = _parameters.ParentLibrary.Books.Skip(nbItemsToSkip);
                            //Si reste de la séquence contient plus d'items que le nombre max éléments par page
                            if (getRest.Count() > ViewModelPage.MaxItemsPerPage)
                            {
                                itemsPage = getRest.Take(ViewModelPage.MaxItemsPerPage);
                            }
                            else
                            {
                                itemsPage = getRest;
                            }
                        }
                    }
                }
                else //Si la séquence contient moins ou le même nombre d'items que le nombre max éléments par page
                {
                    itemsPage = _parameters.ParentLibrary.Books;
                }

                return itemsPage;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<LivreVM>();
            }
        }

        private void CompleteBookInfos(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.TitresOeuvre != null && viewModel.TitresOeuvre.Any())
                {
                    viewModel.TitresOeuvreStringList = StringHelpers.JoinStringArray(viewModel.TitresOeuvre?.Select(s => s)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                }

                if (viewModel.Auteurs != null && viewModel.Auteurs.Any())
                {
                    viewModel.AuteursStringList = StringHelpers.JoinStringArray(viewModel.Auteurs?.Select(s => $"{s.NomNaissance} {s.Prenom}")?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                }

                if (viewModel.Publication.Editeurs != null && viewModel.Publication.Editeurs.Any())
                {
                    viewModel.Publication.EditeursStringList = StringHelpers.JoinStringArray(viewModel.Publication.Editeurs?.Select(s => s.SocietyName)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                }

                if (viewModel.Publication.Collections != null && viewModel.Publication.Collections.Any())
                {
                    viewModel.Publication.CollectionsStringList = StringHelpers.JoinStringArray(viewModel.Publication.Collections?.Select(s => s.Name)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        private void AddItemToSideBar(PivotItem item, SideBarItemHeaderVM sideBarItem)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotRightSideBar.Items.Add(item);
                this.PivotRightSideBar.SelectedItem = item;
                ViewModelPage.ItemsSideBarHeader.Add(sideBarItem);
                this.CmbxSideBarItemTitle.SelectedItem = sideBarItem;
                if (PivotRightSideBar.Items.Count >= 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void RemoveItemToSideBar(PivotItem item)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }

                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {

                    foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                    {
                        if (cmbxItem is SideBarItemHeaderVM headerVM)
                        {
                            if (item is NewEditBookExemplaryUC bookExemplaryUC)
                            {
                                if (bookExemplaryUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                            else if (item is NewEditBookUC newEditBookUC)
                            {
                                if (newEditBookUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                            else if (item is NewEditCollectionUC newEditCollectionUC)
                            {
                                if (newEditCollectionUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                            else if (item is NewEditContactUC newEditContactUC)
                            {
                                if (newEditContactUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                            else if (item is ContactListUC contactListUC)
                            {
                                if (contactListUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                            else if (item is CollectionListUC collectionListUC)
                            {
                                if (collectionListUC.IdItem == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                        }
                    }
                }

                this.PivotRightSideBar.Items.Remove(item);
                if (PivotRightSideBar.Items.Count < 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private NewEditBookExemplaryUC GetBookExemplarySideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookExemplaryUC item && item.ViewModelPage.Guid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as NewEditBookExemplaryUC;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private CategoriesListUC GetCategorieListSideBar()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is CategoriesListUC);
                    if (itemPivot != null)
                    {
                        return itemPivot as CategoriesListUC;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private CategoriesListUC GetCategorieListSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is CategoriesListUC item && item.ViewModelPage.Guid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as CategoriesListUC;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private NewEditBookUC GetBookSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC item && item.ViewModelPage.Guid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as NewEditBookUC;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private Image GetSelectedThumbnailImage(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                if (this.PivotItems.SelectedItem != null)
                {
                    if (this.PivotItems.SelectedItem is IGrouping<string, LivreVM> group && group.Any(f => f == viewModel))
                    {

                        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return null;
                            }
                            else
                            {
                                if (gridView.Name == "GridViewItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is LivreVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    return SelectImageFromContainer(_gridViewItemContainer);
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private Image SelectImageFromContainer(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return null;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Viewbox viewboxThumbnailContainer = grid.Children.FirstOrDefault(f => f is Viewbox _viewboxThumbnailContainer && _viewboxThumbnailContainer.Name == "ViewboxSimpleThumnailDatatemplate") as Viewbox;
                    if (viewboxThumbnailContainer != null)
                    {
                        Border border = viewboxThumbnailContainer.Child as Border;
                        if (border != null)
                        {
                            Grid gridImageContainer = border.Child as Grid;
                            if (gridImageContainer != null)
                            {
                                Image image = gridImageContainer.Children.FirstOrDefault(f => f is Image _image) as Image;
                                return image;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        public GridView GetSelectedGridView(string gridViewName = "GridViewItems")
        {
            try
            {
                if (this.PivotItems.SelectedItem == null)
                {
                    return null;
                }

                var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                while (gridView != null && gridView.Name != gridViewName)
                {
                    gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                    if (gridView == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (gridView.Name == gridViewName)
                        {
                            break;
                        }
                    }
                }

                return gridView;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private DataGrid GetSelectedDataGridItems(string dataGridName = "DataGridItems")
        {
            try
            {
                if (this.PivotItems.SelectedItem == null)
                {
                    return null;
                }

                var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container);
                while (dataGrid != null && dataGrid.Name != dataGridName)
                {
                    dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(dataGrid);
                    if (dataGrid == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (dataGrid.Name == dataGridName)
                        {
                            break;
                        }
                    }
                }

                return dataGrid;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }
        #endregion


        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Image_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private async void ImageBackground_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(ViewModelPage.BackgroundImagePath);
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

        private void ScrollItems_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                if (sender is ScrollViewer scrollViewer)
                {
                    
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Slider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                if (sender is Slider slider)
                {
                    RefreshItemsGrouping();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        
    }

    public class BookCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        private UserControl _SplitViewContent;
        public UserControl SplitViewContent
        {
            get => this._SplitViewContent;
            set
            {
                if (_SplitViewContent != value)
                {
                    this._SplitViewContent = value;
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
