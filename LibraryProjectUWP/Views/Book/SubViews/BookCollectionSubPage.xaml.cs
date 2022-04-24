using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.UI;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static LibraryProjectUWP.Code.Helpers.VisualViewHelpers;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book.SubViews
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookCollectionSubPage : Page
    {
        public IEnumerable<LivreVM> GetItems => ViewModelPage.GroupedRelatedViewModel.Collection.SelectMany(s => s.ToList()).Select(q => q).ToList();
        public int CountItems => GetItems?.Count() ?? 0;
        public BookCollectionSubPageVM ViewModelPage { get; set; } = new BookCollectionSubPageVM();
        readonly EsBook esBook = new EsBook();
        readonly UiServices uiServices = new UiServices();
        public BookCollectionPage ParentPage { get; private set; }
        public BookCollectionSubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookCollectionPage parameters)
            {
                ParentPage = parameters;
                //InitializeData(true);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeData(true);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Image_Loaded_1(object sender, RoutedEventArgs e)
        {

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

        public void InitializeData(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ParentPage.Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Mise à jour du catalogue",
                });

                using (BackgroundWorker worker = new BackgroundWorker()
                {
                    WorkerSupportsCancellation = false,
                    WorkerReportsProgress = false,
                })
                {
                    worker.DoWork += (s, e) =>
                    {
                        switch (ParentPage.ViewModelPage.DataViewMode)
                        {
                            case Code.DataViewModeEnum.DataGridView:
                                using (Task task = Task.Run(() => this.DataGridViewMode(firstLoad)))
                                {
                                    task.Wait();
                                }
                                break;
                            case Code.DataViewModeEnum.GridView:
                                using (Task task = Task.Run(() => this.GridViewMode(firstLoad)))
                                {
                                    task.Wait();
                                }
                                break;
                            default:
                                using (Task task = Task.Run(() => this.GridViewMode(firstLoad)))
                                {
                                    task.Wait();
                                }
                                break;
                        }
                    };

                    worker.RunWorkerCompleted += (s, e) =>
                    {
                        DispatcherTimer dispatcherTimer = new DispatcherTimer()
                        {
                            Interval = new TimeSpan(0, 0, 3),
                        };

                        dispatcherTimer.Tick += (t, f) =>
                        {
                            ParentPage.Parameters.MainPage.CloseBusyLoader();
                            dispatcherTimer.Stop();
                            dispatcherTimer = null;
                        };

                        dispatcherTimer.Start();
                    };

                    worker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                        if (ParentPage.ViewModelPage.DataViewMode != Code.DataViewModeEnum.GridView)
                        {
                            ParentPage.ViewModelPage.DataViewMode = Code.DataViewModeEnum.GridView;
                        }

                        await RefreshItemsGrouping(true, this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchBook);
                        this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                        this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
                    });
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task DataGridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                        if (ParentPage.ViewModelPage.DataViewMode != Code.DataViewModeEnum.DataGridView)
                        {
                            ParentPage.ViewModelPage.DataViewMode = Code.DataViewModeEnum.DataGridView;
                        }

                        await RefreshItemsGrouping(true, this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchBook);
                        this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                        this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
                    });
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

                    if (ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            foreach (var item in ParentPage.ViewModelPage.SelectedItems)
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

                    if (ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            foreach (var item in ParentPage.ViewModelPage.SelectedItems)
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

        private void DataGridItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid && dataGrid.SelectedItem is LivreVM viewModel)
                {
                    ParentPage.EditBook(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
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

        #region Selection
        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    ParentPage.ViewModelPage.SelectedItems = new List<LivreVM>();
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
                    ParentPage.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<LivreVM>().ToList();
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
                    ParentPage.ViewModelPage.SelectedItems = dataGrid.SelectedItems.Cast<LivreVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SelectAll()
        {
            try
            {
                if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = uiServices.GetSelectedGridViewFromPivotTemplate(this.PivotItems, "GridViewItems");
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectAll();
                    }
                }
                else if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = uiServices.GetSelectedDataGridFromPivotTemplate(this.PivotItems, "DataGridItems");
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

        public void DeSelectAll()
        {
            try
            {
                if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = uiServices.GetSelectedGridViewFromPivotTemplate(this.PivotItems, "GridViewItems");
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectedItems.Clear();
                    }
                }
                else if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = uiServices.GetSelectedDataGridFromPivotTemplate(this.PivotItems, "DataGridItems");
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

        public async void DeleteAllSelected()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                {
                    await ParentPage.DeleteBookAsync(ParentPage.ViewModelPage.SelectedItems);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Context Menu Item
        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    var result = await esBook.ChangeBookItemJaquetteAsync(viewModel);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? EsGeneral.BookDefaultJaquette;
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

        private void EditBookInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    ParentPage.EditBook(viewModel);
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

        public async Task ExportThisBookAsync(LivreVM viewModel, string suggestName = "model")
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel != null)
                {
                    var suggestedFileName = "model";

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

        private async void DeleteBookXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    await ParentPage.DeleteBookAsync(new LivreVM[] { viewModel });
                }
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
                    ParentPage.OpenBookExemplaryList(viewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewBookPretXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    ParentPage.OpenBookPretList(viewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
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
                    
                    

                    InitializeGotoPageWorker(page);
                }
                //var isOpened = await ParentPage.OpenLoading();
                //if (isOpened)
                //{
                //    if (args.Parameter is int page)
                //    {
                //        InitializeGotoWorker()
                //        await GotoPage(page);
                //    }
                //}
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
                    async () =>
                    {
                        var selectedPage = this.GetSelectedPage - 1;
                        if (selectedPage >= 1)
                        {
                            await this.GotoPage(selectedPage);
                        }
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.D)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        var selectedPage = this.GetSelectedPage;
                        await this.GotoPage(selectedPage + 1);
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

        public async Task GotoPage(int page)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                await this.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal,
                   async () =>
                    {
                        //foreach (var pageVm in ViewModelPage.PagesList)
                        //{
                        //    if (pageVm.CurrentPage != page && pageVm.IsPageSelected == true)
                        //    {
                        //        pageVm.IsPageSelected = false;
                        //        pageVm.BackgroundColor = Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush;
                        //    }
                        //    else if (pageVm.CurrentPage == page && pageVm.IsPageSelected == false)
                        //    {
                        //        pageVm.IsPageSelected = true;
                        //        pageVm.BackgroundColor = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush;
                        //    }
                        //}

                        await this.RefreshItemsGrouping(true, page, true);
                        var buttonsPage = VisualViewHelpers.FindVisualChilds<Button>(this.itemControlPageList);
                        if (buttonsPage != null && buttonsPage.Any())
                        {
                            var buttonPage = buttonsPage.FirstOrDefault(f => f.CommandParameter is int commandPage && commandPage == page);
                            if (buttonPage != null)
                            {
                                scrollVPages.ScrollToElement(buttonPage, false);
                            }
                        }
                    });
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

        private IEnumerable<LivreVM> GetPaginatedItems(IList<LivreVM> viewModelList, int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                IEnumerable<LivreVM> itemsPage = Enumerable.Empty<LivreVM>();
                
                //Si la séquence contient plus d'items que le nombre max éléments par page
                if (viewModelList.Count > ViewModelPage.MaxItemsPerPage)
                {
                    //Si la première page (ou moins ^^')
                    if (goToPage <= 1)
                    {
                        itemsPage = viewModelList.Take(ViewModelPage.MaxItemsPerPage);
                    }
                    else //Si plus que la première page
                    {
                        var nbItemsToSkip = ViewModelPage.MaxItemsPerPage * (goToPage - 1);
                        if (viewModelList.Count >= nbItemsToSkip)
                        {
                            var getRest = viewModelList.Skip(nbItemsToSkip);
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
                    itemsPage = viewModelList;
                }

                return itemsPage;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<LivreVM>();
            }
        }
        #endregion


        #region Search
        public async Task SearchViewModel(LivreVM viewModel)
        {
            if (viewModel == null) return;
            if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.GridView)
            {
                await SearchViewModelGridView(viewModel);
            }
            else if (ParentPage.ViewModelPage.DataViewMode == Code.DataViewModeEnum.DataGridView)
            {
                await SearchViewModelDataGridView(viewModel);
            }
        }

        public async Task SearchViewModelGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }
                var isMatchInCurrent = SearchViewModelInCurrentGridView(viewModel);
                if (!isMatchInCurrent)
                {
                    foreach (var pageVm in ViewModelPage.PagesList)
                    {
                        var search = GetPaginatedItems(ParentPage.Parameters.ParentLibrary.Books, pageVm.CurrentPage);
                        if (search != null && search.Any(f => f.Id == viewModel.Id))
                        {
                            ViewModelPage.SearchedViewModel = viewModel;
                            await GotoPage(pageVm.CurrentPage);
                            return;
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

        public bool SearchViewModelInCurrentGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return false;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == viewModel.Id))
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
                                return false;
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
                                if (gridViewItem is LivreVM _viewModel && _viewModel.Id == viewModel.Id)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    var selectedGridViewitem = _gridViewItemContainer as GridViewItem;

                                    var _selectedPivotItem = PivotItems.ContainerFromItem(pivotItem);
                                    var selectedPivotItem = _selectedPivotItem as PivotItem;

                                    var scrollViewer = VisualViewHelpers.FindVisualChild<ScrollViewer>(selectedPivotItem, "scrollItems");
                                    if (scrollViewer != null)
                                    {
                                        var result = scrollViewer.ScrollToElement(selectedGridViewitem, true, false);
                                        OpenFlyoutSearchedItemGridView(_gridViewItemContainer);
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
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

        private async Task SearchViewModelDataGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                var isMatch = SearchViewModelInCurrentDataGrid(viewModel);
                if (!isMatch)
                {
                    foreach (var pageVm in ViewModelPage.PagesList)
                    {
                        var search = GetPaginatedItems(ParentPage.Parameters.ParentLibrary.Books, pageVm.CurrentPage);
                        if (search != null && search.Any(f => f.Id == viewModel.Id))
                        {
                            ViewModelPage.SearchedViewModel = viewModel;
                            await GotoPage(pageVm.CurrentPage);
                            return;
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

        public bool SearchViewModelInCurrentDataGrid(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return false;
                }

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
                                return false;
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
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }
        #endregion

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
    }
}
