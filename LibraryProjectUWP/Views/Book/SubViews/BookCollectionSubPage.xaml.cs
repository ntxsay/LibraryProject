﻿using LibraryProjectUWP.Code;
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
using System.Collections.ObjectModel;
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
        public BookCollectionSubPageVM ViewModelPage { get; set; }
        readonly EsBook esBook = new EsBook();
        readonly UiServices uiServices = new UiServices();
        public CommonView CommonView { get; private set; }
        public BookCollectionPage ParentPage { get; private set; }
        public long IdLibrary { get; set; }
        public BookCollectionSubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookCollectionPage parameters)
            {
                ViewModelPage = new BookCollectionSubPageVM(parameters);
                ParentPage = parameters;
                CommonView = new CommonView(ParentPage, this);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EsLibrary esLibrary = new EsLibrary();
            ParentPage.ViewModelPage.BackgroundImagePath = await esLibrary.GetBookCollectionBackgroundImagePathAsync(ParentPage.Parameters.ParentLibrary.Guid);
            await ParentPage.InitializeBackgroundImagesync();
            if (ParentPage.Parameters.ParentLibrary.Id != IdLibrary)
            {
                IdLibrary = ParentPage.Parameters.ParentLibrary.Id;
                InitializeData(true);
            }
            else if (ViewModelPage.GroupedRelatedViewModel == null || ViewModelPage.GroupedRelatedViewModel.Collection == null || !ViewModelPage.GroupedRelatedViewModel.Collection.Any())
            {
                InitializeData(true);
            }
            else
            {
                ParentPage.ViewModelPage.NbItems = ViewModelPage.NbItems;
                ParentPage.ViewModelPage.NbElementDisplayed = ViewModelPage.NbElementDisplayed;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //GC.Collect();
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

                        await CommonView.RefreshItemsGrouping(this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchItem);
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

                        await CommonView.RefreshItemsGrouping(this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchItem);
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

                    if (ParentPage.ViewModelPage.SelectedItems.Any() && ParentPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            foreach (var item in collection)
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

                    if (ParentPage.ViewModelPage.SelectedItems.Any() && ParentPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            foreach (var item in collection)
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

        private async void DataGridItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid && dataGrid.SelectedItem is LivreVM viewModel)
                {
                    await ParentPage.NewEditBookAsync(viewModel, EditMode.Edit);
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
                    ParentPage.ViewModelPage.SelectedItems = new ObservableCollection<object>();
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
                    var items = gridView.SelectedItems.Cast<object>();
                    ParentPage.ViewModelPage.SelectedItems = new ObservableCollection<object>(items);
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
                    var items = dataGrid.SelectedItems.Cast<object>();
                    ParentPage.ViewModelPage.SelectedItems = new ObservableCollection<object>(items);
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
                if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.GridView)
                {
                    var gridViewItems = uiServices.GetSelectedGridViewFromPivotTemplate(this.PivotItems, "GridViewItems");
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectAll();
                    }
                }
                else if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.DataGridView)
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
                if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.GridView)
                {
                    var gridViewItems = uiServices.GetSelectedGridViewFromPivotTemplate(this.PivotItems, "GridViewItems");
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectedItems.Clear();
                    }
                }
                else if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.DataGridView)
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

        public async Task DeleteAllSelected()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any() && ParentPage.ViewModelPage.SelectedItems.FirstOrDefault() is LivreVM)
                {
                    await ParentPage.DeleteBookAsync(ParentPage.ViewModelPage.SelectedItems.Cast<LivreVM>());
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

        private async void EditBookInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is LivreVM viewModel)
            {
                await ParentPage.NewEditBookAsync(viewModel, EditMode.Edit);
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
                        await CommonView.RefreshItemsGrouping(page, true, ParentPage.ViewModelPage.ResearchItem);
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
        public void SearchViewModel(LivreVM viewModel)
        {
            if (viewModel == null) return;
            if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.GridView)
            {
                var gridViewItem = uiServices.GetSelectedGridViewItem<LivreVM>(viewModel.Id, PivotItems);
                if (gridViewItem != null)
                {
                    var pivotItemContainer = PivotItems.ContainerFromIndex(PivotItems.SelectedIndex);
                    var scrollViewer = FindVisualChild<ScrollViewer>(pivotItemContainer, "scrollItems");
                    if (scrollViewer != null)
                    {
                        _ = scrollViewer.ScrollToElement(gridViewItem, true, false);
                        OpenFlyoutSearchedItemGridView(gridViewItem);
                    }
                }
            }
            else if (ParentPage.ViewModelPage.DataViewMode == DataViewModeEnum.DataGridView)
            {
                _ = uiServices.SelectDataGridItem<LivreVM>(viewModel.Id, PivotItems);
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

                var grid = FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    if (grid.Children.FirstOrDefault(f => f is Grid _gridActions && _gridActions.Name == "GridActions") is Grid gridActions)
                    {
                        if (gridActions.Children.FirstOrDefault(f => f is Button _buttonActions && _buttonActions.Name == "BtnActions") is Button buttonActions)
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
        #endregion

        public void CompleteBookInfos(long idBook)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var viewModel = uiServices.SearchViewModelInCurrentDataGrid(this.PivotItems, idBook, "DataGridItems", false);
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

                if (viewModel.Publication != null)
                {
                    if (viewModel.Publication.Editeurs != null && viewModel.Publication.Editeurs.Any())
                    {
                        viewModel.Publication.EditeursStringList = StringHelpers.JoinStringArray(viewModel.Publication.Editeurs?.Select(s => s.SocietyName)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }

                    if (viewModel.Publication.Collections != null && viewModel.Publication.Collections.Any())
                    {
                        viewModel.Publication.CollectionsStringList = StringHelpers.JoinStringArray(viewModel.Publication.Collections?.Select(s => s.Name)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }
                }

                if (viewModel.Format != null)
                {
                    viewModel.Format.Dimensions = LibraryHelpers.Book.GetDimensionsInCm(viewModel.Format.Hauteur, viewModel.Format.Largeur, viewModel.Format.Epaisseur);
                }
                viewModel.ClassificationAge.GetClassificationAge();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
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
                        if (viewboxThumbnailContainer.Child is Border border)
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
