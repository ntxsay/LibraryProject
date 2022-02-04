using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.ViewModels.UI;
using LibraryProjectUWP.Views.Library.Manage;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Library.Collection
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionGridViewPage : Page
    {
        public LibraryCollectionGridViewPageVM ViewModelPage { get; set; } = new LibraryCollectionGridViewPageVM();
        private LibraryCollectionParentChildParamsVM _libraryParameters;
        EsLibrary esLibrary = new EsLibrary();
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
            }
        }

        #region Loading
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync(true);
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
                var itemList = await DbServices.Library.AllVMAsync();
                ViewModelPage.ViewModelList = itemList?.ToList() ?? new List<BibliothequeVM>(); ;
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
                if (ViewModelPage.ViewModelList != null && ViewModelPage.ViewModelList.Any())
                {
                    foreach (var item in ViewModelPage.ViewModelList)
                    {
                        string combinedPath = await esLibrary.GetLibraryItemJaquettePathAsync(item);
                        item.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : EsGeneral.LibraryDefaultJaquette;
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

        private void GridViewItems_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is GridView gridView)
                {
                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel.Id == item.Id && !gridView.SelectedItems.Contains(item))
                                {
                                    gridView.SelectedItems.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                    gridView.SelectionChanged += GridViewItems_SelectionChanged;
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
                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (dataGridItem is BibliothequeVM _viewModel && _viewModel.Id == item.Id && !dataGrid.SelectedItems.Contains(item))
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

        public void RefreshItemsGrouping()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ViewModelPage.GroupedBy)
                {
                    case LibraryGroupVM.GroupBy.None:
                        this.GroupItemsByNone();
                        break;
                    case LibraryGroupVM.GroupBy.Letter:
                        this.GroupItemsByAlphabetic();
                        break;
                    case LibraryGroupVM.GroupBy.CreationYear:
                        this.GroupByCreationYear();
                        break;
                    default:
                        this.GroupItemsByNone();
                        break;
                }
            }
            catch (Exception ex)
            {
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



        #region Selection
        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    this.ViewModelPage.SelectedItems = new List<BibliothequeVM>();
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
                    this.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<BibliothequeVM>().ToList();
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
                    this.ViewModelPage.SelectedItems = dataGrid.SelectedItems.Cast<BibliothequeVM>().ToList();
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
                if (sender is ListView listView && listView.SelectedItem is BibliothequeVM viewModel)
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

        #region Functions
        private Image GetSelectedThumbnailImage(BibliothequeVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                if (this.PivotItems.SelectedItem != null)
                {
                    if (this.PivotItems.SelectedItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f == viewModel))
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
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel == viewModel)
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


        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    InitializeModelList();
        //}

        //private void InitializeModelList()
        //{
        //    MethodBase m = MethodBase.GetCurrentMethod();
        //    try
        //    {
        //        _libraryParameters.ParentPage.RefreshItemsGrouping();
        //        this.PivotItems.SelectedIndex = _libraryParameters.ParentPage.ViewModelPage.SelectedPivotIndex;
        //        this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        //private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (sender is Pivot pivot)
        //        {
        //            _libraryParameters.ParentPage.ViewModelPage.SelectedItems = new List<BibliothequeVM>();
        //            _libraryParameters.ParentPage.ViewModelPage.SelectedPivotIndex = pivot.SelectedIndex;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        //private void GridViewItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (sender is GridView gridView)
        //        {
        //            _libraryParameters.ParentPage.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<BibliothequeVM>().ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        /// <summary>
        /// Ouvre la liste des livre de la bibliothèque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Viewbox viewbox && viewbox.Tag is BibliothequeVM viewModel)
                {
                    VisualViewHelpers.MainControlsUI mainControlsUI = new VisualViewHelpers.MainControlsUI();
                    var mainPage = mainControlsUI.GetMainPage;
                    if (mainPage != null)
                    {
                        mainPage.BookCollectionNavigationAsync(viewModel);
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

        //private async void Image_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (sender is Image imageCtrl && imageCtrl.Tag is string path)
        //        {
        //            var bitmapImage = await Files.BitmapImageFromFileAsync(path);
        //            imageCtrl.Source = bitmapImage;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        #region Groups
        public void GroupItemsByNone()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.None;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        public void GroupItemsByAlphabetic()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Name.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
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

        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    EsLibrary esLibrary = new EsLibrary();
                    var result = await esLibrary.ChangeLibraryItemJaquetteAsync(viewModel);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? EsLibrary.LibraryDefaultJaquette;
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

        private async void ExportLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    //await _commonView.ExportLibraryAsync(viewModel);
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
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    //_commonView.DeleteLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        private void EditLibraryInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    //_commonView.EditLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SearchViewModel(BibliothequeVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f == viewModel))
                    {
                        if (this.PivotItems.SelectedItem != pivotItem)
                        {
                            this.PivotItems.SelectedItem = pivotItem;
                        }

                        var _container = this.PivotItems.ContainerFromItem(pivotItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return;
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
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    OpenFlyoutSearchedItem(_gridViewItemContainer);
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

        private void OpenFlyoutSearchedItem(DependencyObject _gridViewItemContainer)
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
                            buttonActions.Flyout.ShowAt(buttonActions,new FlyoutShowOptions() 
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

        private void MenuFlyoutSubItem_Categorie_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutSubItem subItem && subItem.Tag is BibliothequeVM viewModel)
                {
                    //_commonView.PopulateCategoriesMenuItems(subItem, viewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        


    }

    public class LibraryCollectionGridViewPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
