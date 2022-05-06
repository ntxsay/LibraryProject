﻿using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.UI;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Library
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionSubPage : Page
    {
        public LibraryCollectionPageVM ViewModelPage { get; set; } = new LibraryCollectionPageVM();
        public CommonView CommonView { get; private set; }
        public BookCollectionPage ParentPage { get; private set; }
        EsLibrary esLibrary = new EsLibrary();
        UiServices uiServices = new UiServices();
        public MainPage MainPage { get; private set; }


        public LibraryCollectionSubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookCollectionPage parameters)
            {
                ParentPage = parameters;
                CommonView = new CommonView(ParentPage, this);
            }
        }

        #region Loading
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataAsync(true);
        }

        private void ReloadDataXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            LoadDataAsync(false);
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

        private void LoadDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                InitializeData(firstLoad);
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

                //this.GridViewMode(firstLoad);
                //this.InitializeCountBookWorker();
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
        #endregion

        #region ContextMenu
        private void EditLibraryInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    var result = await esLibrary.ChangeLibraryItemJaquetteAsync(viewModel);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? EsLibrary.LibraryDefaultJaquette;
                    var image = uiServices.GetSelectedThumbnailImage<BibliothequeVM>(viewModel.Id, PivotItems, "GridViewItems");
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
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    var suggestedFileName = $"Rostalotheque_Bibliotheque_{viewModel.Name}_{DateTime.Now:yyyyMMddHHmmss}";

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
        #endregion


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
                    ParentPage.OpenBookCollection(viewModel);
                    //MainPage.BookCollectionNavigationAsync(viewModel, null);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid && dataGrid.SelectedItem is BibliothequeVM viewModel)
                {
                    MainPage.BookCollectionNavigationAsync(viewModel, null);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Paginations
        private void GotoPageXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is int page)
                {
                    //GotoPage(page);
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
                        //var selectedPage = this.GetSelectedPage - 1;
                        //if (selectedPage >= 1)
                        //{
                        //    //this.GotoPage(selectedPage);
                        //}
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.D)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        //var selectedPage = this.GetSelectedPage;
                        //this.GotoPage(selectedPage + 1);
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

        #endregion

        #region Loading BacKGroundWorker
        private BackgroundWorker WorkerLoadLibraries;
        public void InitializeLoadingLibrariesWorker()
        {
            try
            {
                if (WorkerLoadLibraries == null)
                {
                    WorkerLoadLibraries = new BackgroundWorker()
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true,
                    };

                    WorkerLoadLibraries.ProgressChanged += WorkerLoadLibraries_ProgressChanged;
                    WorkerLoadLibraries.DoWork += WorkerLoadLibraries_DoWork; ;
                    WorkerLoadLibraries.RunWorkerCompleted += WorkerLoadLibraries_RunWorkerCompleted; ;
                }

                if (WorkerLoadLibraries != null)
                {
                    if (!WorkerLoadLibraries.IsBusy)
                    {
                        ViewModelPage.ViewModelList.Clear();
                        this.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Chargement des bibliothèques en cours.",
                            CancelButtonText = "Annuler le chargement",
                            CancelButtonVisibility = Visibility.Visible,
                            CancelButtonCallback = () =>
                            {
                                if (WorkerLoadLibraries.IsBusy)
                                {
                                    WorkerLoadLibraries.CancelAsync();
                                }
                            },
                            OpenedLoaderCallback = () => WorkerLoadLibraries.RunWorkerAsync(),
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerLoadLibraries_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    using (Task<IEnumerable<long>> task = DbServices.Library.GetIdLibrariesAsync())
                    {
                        task.Wait();
                        int ModelCount = task.Result.Count();
                        double progressPercentage;
                        int count = 0;

                        List<BibliothequeVM> bibliothequeVMs = new List<BibliothequeVM>();
                        foreach (var id in task.Result)
                        {
                            using (Task<BibliothequeVM> getLibraryTask = DbServices.Library.SingleVMAsync(id))
                            {
                                getLibraryTask.Wait();
                                var viewModel = getLibraryTask.Result;
                                if (viewModel != null)
                                {
                                    using (Task<string> jaquetteTask = esLibrary.GetLibraryItemJaquettePathAsync(viewModel))
                                    {
                                        jaquetteTask.Wait();
                                        viewModel.JaquettePath = !jaquetteTask.Result.IsStringNullOrEmptyOrWhiteSpace() ? jaquetteTask.Result : EsGeneral.LibraryDefaultJaquette;
                                    }

                                    bibliothequeVMs.Add(viewModel);
                                }

                                var NumberModel = count + 1;
                                double Operation = (double)NumberModel / (double)ModelCount;
                                progressPercentage = Operation * 100;
                                int ProgressValue = Convert.ToInt32(progressPercentage);

                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(500);
                                    worker.ReportProgress(ProgressValue, viewModel);
                                }
                                count++;
                            }
                        }

                        e.Result = bibliothequeVMs.ToArray();
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

        private void WorkerLoadLibraries_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState != null && e.UserState is BibliothequeVM bibliothequeVM)
                {
                    //Progress bar/text
                    var busyLoader = this.MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des bibliothèques chargées.\nBibliothèque en cours : \"{bibliothequeVM.Name}\"";
                        if (busyLoader.BtnCancel.Visibility != Visibility.Visible)
                            busyLoader.BtnCancel.Visibility = Visibility.Visible;

                        //ViewModelPage.ViewModelList.Add(bibliothequeVM);
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

        private void WorkerLoadLibraries_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var busyLoader = this.MainPage.GetBusyLoader;
                if (busyLoader != null)
                {
                    // Si erreur
                    if (e.Error != null)
                    {
                        busyLoader.TbcTitle.Text = $"Le chargement des bibliothèques s'est terminé avec l'erreur :\n\"{e.Error.Message}\"\n\nActualisation du catalogue des livres en cours...";
                    }
                    else if (e.Cancelled)
                    {
                        busyLoader.TbcTitle.Text = $"Le chargement des bibliothèques a été annulé par l'utilisateur\nActualisation du catalogue des livres en cours...";
                    }
                    else
                    {
                        if (e.Result != null && e.Result is BibliothequeVM[] viewModelList)
                        {
                            busyLoader.TbcTitle.Text = $"{viewModelList.Length} {(viewModelList.Length > 1 ? "bibliothèques ont été chargées" : "bibliothèque a été chargée")}.";
                        }
                    }

                    if (busyLoader.BtnCancel.Visibility != Visibility.Collapsed)
                        busyLoader.BtnCancel.Visibility = Visibility.Collapsed;
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    this.MainPage.CloseBusyLoader();
                    await this.GridViewMode(true);
                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();

                WorkerLoadLibraries.Dispose();
                WorkerLoadLibraries = null;
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
                    ProgessText = $"Mise à jour du catalogue des bibliothèques en cours...",
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

                        await CommonView.RefreshItemsGrouping(this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchBook);
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

                        await CommonView.RefreshItemsGrouping(this.GetSelectedPage, firstLoad, ParentPage.ViewModelPage.ResearchBook);
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

        #endregion

    }
}