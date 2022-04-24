using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Categories;
using LibraryProjectUWP.Views.Collection;
using LibraryProjectUWP.Views.Contact;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using LibraryProjectUWP.Views.Book.SubViews;
using LibraryProjectUWP.Code.Services.Web;
using LibraryProjectUWP.Views.UserControls;
using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.Code.Services.UI;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookCollectionPage : Page
    {
        public BookCollectionPageVM ViewModelPage { get; set; } = new BookCollectionPageVM();
        public LibraryToBookNavigationDriverVM Parameters { get; private set; }
        readonly EsBook esBook = new EsBook();
        readonly UiServices uiServices = new UiServices();

        public BookCollectionPage()
        {
            this.InitializeComponent();
        }

        public BookCollectionSubPage BookCollectionSubPage => FrameContainer.Content as BookCollectionSubPage;
        public ImportBookExcelSubPage ImportBookExcelSubPage => FrameContainer.Content as ImportBookExcelSubPage;
        public ImportBookFileSubPage ImportBookFileSubPage => FrameContainer.Content as ImportBookFileSubPage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryToBookNavigationDriverVM parameters)
            {
                Parameters = parameters;
                ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
            }
        }
        

        #region Loading
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                EsLibrary esLibrary = new EsLibrary(); 
                ViewModelPage.BackgroundImagePath = await esLibrary.GetBookCollectionBackgroundImagePathAsync(Parameters.ParentLibrary.Guid);
                await InitializeBackgroundImagesync();
                OpenBookCollection();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                //_parameters.ParentLibrary.Books.Clear();
                //_parameters.ParentLibrary.Books.Clear();
                Parameters.ParentLibrary.CountBooks = 0;
                ViewModelPage = null;
                //_parameters = null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ReloadDataXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.InitializeData(true);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
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

        private async Task LoadDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
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
                ViewModelPage.SearchingLibraryVisibility = Visibility.Collapsed;
                if (ViewModelPage.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    await this.GridViewMode(firstLoad);
                }
                else if (ViewModelPage.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    await this.DataGridViewMode(firstLoad);
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
        private async void Lv_SelectedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem is LivreVM viewModel)
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.SearchViewModel(viewModel);
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

        private void Btn_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.SelectAll();
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.DeSelectAll();
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
        private async void ABTBtn_GridViewMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton)
                {
                    if (ViewModelPage.DataViewMode == Code.DataViewModeEnum.GridView)
                    {
                        if (toggleButton.IsChecked != true)
                        {
                            toggleButton.IsChecked = true;
                        }
                        return;
                    }
                    await this.GridViewMode(false);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ABTBtn_DataGridViewMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton)
                {
                    if (ViewModelPage.DataViewMode == Code.DataViewModeEnum.DataGridView)
                    {
                        if (toggleButton.IsChecked != true)
                        {
                            toggleButton.IsChecked = true;
                        }
                        return;
                    }
                    await this.DataGridViewMode(false);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task GridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GridViewMode(firstLoad);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task DataGridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.DataGridViewMode(firstLoad);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = FrameContainer.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Sort - Group - Order
        private void TMFI_GroupByLetter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem item)
            {
                if (this.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.Letter)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                    return;
                }
                this.GroupItemsByAlphabetic();
            }
        }

        private void TMFI_GroupByCreationYear_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem item)
            {
                if (this.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.CreationYear)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                    return;
                }
                this.GroupByCreationYear();
            }
        }

        private void TMFI_GroupByParutionYear_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem item)
            {
                if (this.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.ParutionYear)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                    return;
                }
                this.GroupByParutionYear();
            }
        }

        private void TMFI_GroupByNone_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem item)
            {
                if (this.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.None)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                    return;
                }
                this.GroupItemsByNone();
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
                        if (item is PivotItem pivotItem && pivotItem.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                        {
                            if (itemHeader.Guid == headerVM.IdItem)
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
        
        #region Médias
        private async void ChangeBackgroundImageXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                EsLibrary esLibrary = new EsLibrary();
                var result = await esLibrary.ChangeBookCollectionBackgroundImageAsync(Parameters.ParentLibrary.Guid);
                if (!result.IsSuccess)
                {
                    return;
                }

                ViewModelPage.BackgroundImagePath = result.Result?.ToString() ?? EsGeneral.BookCollectionDefaultBackgroundImage;
                await InitializeBackgroundImagesync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task InitializeBackgroundImagesync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.BackgroundImagePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

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
        #endregion

        #region New Book
        private void NewBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            NewBook(null);
        }

        private async void NewEditBookUC_Create_CreateItemRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    LivreVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Book.CreateAsync(newViewModel, Parameters.ParentLibrary.Id);
                    if (creationResult.IsSuccess)
                    {
                        Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Actualisation du catalogue des livres en cours...",
                        });

                        DispatcherTimer dispatcherTimer = new DispatcherTimer()
                        {
                            Interval = new TimeSpan(0, 0, 0, 1),
                        };

                        dispatcherTimer.Tick += async (t, f) =>
                        {
                            await this.RefreshItemsGrouping(true, GetSelectedPage);

                            DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                            {
                                Interval = new TimeSpan(0, 0, 0, 2),
                            };

                            dispatcherTimer2.Tick += (s, i) =>
                            {
                                Parameters.MainPage.CloseBusyLoader();
                                dispatcherTimer2.Stop();
                            };
                            dispatcherTimer2.Start();

                            dispatcherTimer.Stop();
                        };

                        dispatcherTimer.Start();

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
                    IdLibrary = Parameters.ParentLibrary.Id,
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

        private void NewBook(LivreVM viewModel = null)
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
                    if (viewModel != null)
                    {
                        viewModel.IdLibrary = Parameters.ParentLibrary.Id;
                    }

                    NewEditBookUC userControl = new NewEditBookUC(new ManageBookParametersDriverVM()
                    {
                        ParentPage = this,
                        EditMode = Code.EditMode.Create,
                        CurrentViewModel = viewModel ?? new LivreVM()
                        {
                            IdLibrary = Parameters.ParentLibrary.Id,
                        }
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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
                        CurrentViewModel = DbServices.Book.DeepCopy(viewModel),
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_Edit_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditBookUC_Edit_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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
                        CompleteBookInfos(sender._parameters.CurrentViewModel);
                        await esBook.SaveBookViewModelAsync(sender._parameters.CurrentViewModel);

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
        private async void ImportBookFromWebSiteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var dialog = new ImportBookFromUrlCD()
                {
                    Title = "Importer un livre depuis Amazon"
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    string url = dialog.Tbx_Url.Text?.Trim();
                    if (url == null || url.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        Logs.Log(m, $"Une url valide doit être renseignée.");
                        return;
                    }

                    if (url.Contains("amazon"))
                    {
                        htmlServices htmlservices = new htmlServices();
                        var viewModel = await htmlservices.GetBookFromAmazonAsync(new Uri(url), new LivreVM());
                        NewBook(viewModel);
                    }
                    else
                    {
                        Logs.Log(m, $"L'url doit provenir d'Amazon.");
                        return;
                    }
                }
                else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ImportBookFromExcelFileXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var storageFile = await Files.OpenStorageFileAsync(Files.ExcelExtensions);
                if (storageFile == null)
                {
                    Logs.Log(m, $"Vous devez sélectionner un fichier de type Microsoft Excel.");
                    return;
                }

                this.ImportBookFromExcelFile(storageFile);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ImportBookFromJsonFileXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var storageFile = await Files.OpenStorageFileAsync(Files.BookExtensions);
                if (storageFile == null)
                {
                    Logs.Log(m, $"Vous devez sélectionner un type de fichier valide.");
                    return;
                }

                var viewModels = (await esBook.OpenBooksFromFileAsync(storageFile))?.ToList();
                if (viewModels != null && viewModels.Any())
                {
                    viewModels.ForEach((book) => this.CompleteBookInfos(book));

                    if (viewModels.Count() == 1)
                    {
                        NewBook(viewModels.First());
                    }
                    else
                    {
                        ImportBookFromFile(viewModels, storageFile);
                        OpenImportBookFromFile(viewModels);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void ImportBookFromFile(IEnumerable<LivreVM> viewModelList, StorageFile file)
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
                    ImportBookFromFileUC userControl = new ImportBookFromFileUC(new ImportBookParametersDriverVM()
                    {
                        ParentPage = this,
                        //ViewModelList = viewModelList,
                        File = file,
                    });

                    userControl.CancelModificationRequested += ImportBookFromFileUC_CancelModificationRequested;
                    userControl.ImportDataRequested += ImportBookFromFileUC_ImportDataRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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

        private void ImportBookFromFileUC_ImportDataRequested(ImportBookFromFileUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                List<LivreVM> newViewModelList = sender.ViewModelPage.NewViewModel;
                ImportBooksTask importBooksTask = new ImportBooksTask(Parameters.MainPage, Parameters.ParentLibrary.Id);
                importBooksTask.AfterTaskCompletedRequested += ImportBooksTask_AfterTaskCompletedRequested;
                importBooksTask.InitializeWorker(newViewModelList);

                sender.CancelModificationRequested -= ImportBookFromFileUC_CancelModificationRequested;
                sender.ImportDataRequested -= ImportBookFromFileUC_ImportDataRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


        private void ImportBookFromFileUC_CancelModificationRequested(ImportBookFromFileUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= ImportBookFromFileUC_CancelModificationRequested;
                sender.ImportDataRequested -= ImportBookFromFileUC_ImportDataRequested;

                this.RemoveItemToSideBar(sender);
                this.OpenBookCollection();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void ImportBookFromExcelFile(StorageFile excelFile)
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
                    ImportBookFromExcelUC userControl = new ImportBookFromExcelUC()
                    {  
                        ViewModelPivotItem = new ImportBookFromExcelUCVM()
                        {
                            FileStorage = excelFile,
                            ParentPage = this,
                        },
                    };

                    userControl.CancelModificationRequested += ImportBookFromExcelUC_CancelModificationRequested;
                    userControl.ImportDataRequested += ImportBookFromExcelUC_ImportDataRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPivotItem.Glyph,
                        Title = userControl.ViewModelPivotItem.Header,
                        IdItem = userControl.ViewModelPivotItem.ItemGuid,
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

        private void ImportBookFromExcelUC_ImportDataRequested(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                List<LivreVM> newViewModelList = sender.ViewModelPivotItem.NewViewModel;
                ImportBooksTask importBooksTask = new ImportBooksTask(Parameters.MainPage, Parameters.ParentLibrary.Id);
                importBooksTask.AfterTaskCompletedRequested += ImportBooksTask_AfterTaskCompletedRequested;
                importBooksTask.InitializeWorker(newViewModelList);

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

        private void ImportBookFromExcelUC_CancelModificationRequested(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= ImportBookFromExcelUC_CancelModificationRequested;
                sender.ImportDataRequested -= ImportBookFromExcelUC_ImportDataRequested;

                this.RemoveItemToSideBar(sender);
                this.OpenBookCollection();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ImportBooksTask_AfterTaskCompletedRequested(ImportBooksTask sender, object e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.OpenBookCollection();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Book Exemplary
        public void NewBookExemplary(LivreVM viewModel, SideBarInterLinkVM parentReferences = null)
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
                        ParentBook = viewModel,
                        CurrentViewModel = new LivreExemplaryVM()
                        {
                            Etat = new LivreEtatVM()
                            {
                                TypeVerification = Code.BookTypeVerification.Entree,
                            }
                        }
                    });

                    if (parentReferences != null)
                    {
                        userControl.ViewModelPage.ParentReferences = parentReferences;
                    }

                    userControl.CancelModificationRequested += NewEditBookExemplaryUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookExemplaryUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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

                    var creationResult = await DbServices.Book.CreateExemplaryAsync(sender._parameters.ParentBook.Id, newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.ParentReferences != null)
                        {
                            if (sender.ViewModelPage.ParentReferences.ParentType == typeof(BookExemplaryListUC))
                            {

                            }
                            else if (sender.ViewModelPage.ParentReferences.ParentType == typeof(NewEditBookPretUC))
                            {
                                var item = uiServices.GetNewEditBookPretUCSideBarByGuid(PivotRightSideBar, sender.ViewModelPage.ParentReferences.ParentGuid);
                                if (item != null)
                                {
                                    item.LoadDataAsync();
                                    SelectItemSideBar(item);
                                }
                            }
                        }
                        sender.Close();
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

        public void OpenBookExemplaryList(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel != null)
                {
                    if (PivotRightSideBar.Items.FirstOrDefault(f => f is BookExemplaryListUC) is BookExemplaryListUC checkedItem)
                    {
                        if (checkedItem._parameters.ParentBook.Id == viewModel.Id)
                        {
                            BookExemplaryList(viewModel);
                        }
                        else
                        {
                            checkedItem.Close();
                            BookExemplaryList(viewModel);
                        }
                    }
                    else
                    {
                        BookExemplaryList(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void BookExemplaryList(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is BookExemplaryListUC);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                    if (checkedItem is BookExemplaryListUC item)
                    {
                        item.InitializeData();
                    }
                }
                else
                {
                    BookExemplaryListUC userControl = new BookExemplaryListUC(new BookExemplaryListParametersDriverVM()
                    {
                        ParentPage = this,
                        ParentBook = viewModel,
                    });

                    userControl.CancelModificationRequested += BookExemplaryListUC_CancelModificationRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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

        private void BookExemplaryListUC_CancelModificationRequested(BookExemplaryListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= BookExemplaryListUC_CancelModificationRequested;
                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Book Pret
        public void OpenBookPretList(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel != null)
                {
                    if (PivotRightSideBar.Items.FirstOrDefault(f => f is BookPretListUC) is BookPretListUC checkedItem)
                    {
                        if (checkedItem._parameters.ParentBook.Id == viewModel.Id)
                        {
                            BookPretList(viewModel);
                        }
                        else
                        {
                            checkedItem.Close();
                            BookPretList(viewModel);
                        }
                    }
                    else
                    {
                        BookPretList(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void BookPretList(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is BookPretListUC) is BookPretListUC checkedItem)
                {
                    this.SelectItemSideBar(checkedItem);
                    checkedItem.InitializeData();
                }
                else
                {
                    BookPretListUC userControl = new BookPretListUC(new BookPretListParametersDriverVM()
                    {
                        ParentPage = this,
                        ParentBook = viewModel,
                    });

                    userControl.CancelModificationRequested += BookPretListUC_CancelModificationRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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

        public void NewBookPret(LivreVM viewModel, LivrePretVM livrePretVM = null, EditMode editMode = EditMode.Create, SideBarInterLinkVM parentReferences = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookPretUC item && item.ViewModelPage.EditMode == editMode) is NewEditBookPretUC checkedItem)
                {
                    checkedItem.InitializeSideBar(this, viewModel, livrePretVM, editMode);
                    this.SelectItemSideBar(checkedItem);
                }
                else
                {
                    NewEditBookPretUC userControl = new NewEditBookPretUC();
                    userControl.InitializeSideBar(this, viewModel, livrePretVM , editMode);

                    if (parentReferences != null)
                    {
                        userControl.ViewModelPage.ParentReferences = parentReferences;
                    }

                    userControl.CancelModificationRequested += NewEditBookPretUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookPretUC_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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

        private async void NewEditBookPretUC_CreateItemRequested(NewEditBookPretUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender.ViewModelPage != null)
                {
                    LivrePretVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Book.CreatePretAsync(sender.ViewModelPage.ViewModel.IdBookExemplary, newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.ParentReferences != null)
                        {
                            if (sender.ViewModelPage.ParentReferences.ParentType == typeof(BookPretListUC))
                            {
                                var item = uiServices.GetBookPretListUCSideBarByGuid(PivotRightSideBar, sender.ViewModelPage.ParentReferences.ParentGuid);
                                if (item != null)
                                {
                                    item.InitializeData();
                                    SelectItemSideBar(item);
                                }
                            }
                        }
                        sender.Close();
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

                sender.ViewModelPage.ViewModel = new LivrePretVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditBookPretUC_CancelModificationRequested(NewEditBookPretUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditBookPretUC_CancelModificationRequested;
                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


        private void BookPretListUC_CancelModificationRequested(BookPretListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= BookPretListUC_CancelModificationRequested;
                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion
        private void ExportAllBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ExportAllBooksTask exportAllBooksTask = new ExportAllBooksTask(Parameters.MainPage) 
                { 
                    UseIntervalAfterFinish = false
                };
                exportAllBooksTask.InitializeWorker(Parameters.ParentLibrary);
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

        #region Delete Book
        
        private void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.DeleteAllSelected();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task DeleteBookAsync(IEnumerable<LivreVM> viewModelList)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ABBtn_SelectedItems.Flyout.Hide();
                
                if (viewModelList == null || !viewModelList.Any())
                {
                    return;
                }

                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    var dialog = new DeleteBookCD(viewModelList);

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary || result == ContentDialogResult.Secondary)
                    {
                        DeleteManyBooksTask deleteManyBooksTask = new DeleteManyBooksTask(Parameters.MainPage)
                        {
                            CloseBusyLoaderAfterFinish = false,
                        };

                        deleteManyBooksTask.AfterTaskCompletedRequested += async (s, e) =>
                        {
                            ViewModelPage.SelectedItems.Clear();
                            ViewModelPage.SelectedItems = new List<LivreVM>();

                            var busyLoader = Parameters.MainPage.GetBusyLoader ;
                            if (busyLoader != null)
                            {
                                busyLoader.TbcTitle.Text = "Actualisation du catalogue des livres en cours...";
                            }

                            await this.RefreshItemsGrouping(true, 1, true, ViewModelPage.ResearchBook);

                            DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                            {
                                Interval = new TimeSpan(0, 0, 0, 2),
                            };

                            dispatcherTimer2.Tick += (f, i) =>
                            {
                                Parameters.MainPage.CloseBusyLoader();
                                dispatcherTimer2.Stop();
                            };
                            dispatcherTimer2.Start();
                        };

                        if (result == ContentDialogResult.Primary)
                        {
                            bool isSaved = await esBook.SaveBookViewModelAsAsync(viewModelList);
                            if (isSaved)
                            {
                                deleteManyBooksTask.InitializeWorker(viewModelList);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            deleteManyBooksTask.InitializeWorker(viewModelList);
                        }
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
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

        #endregion

        #region Contact
        private void MFI_NewPersonne_Click(object sender, RoutedEventArgs e)
        {
            this.NewContact(ContactType.Human, ContactRole.Adherant, string.Empty, string.Empty);
        }

        private void MFI_NewSociety_Click(object sender, RoutedEventArgs e)
        {
            this.NewContact(ContactType.Society, ContactRole.EditorHouse, string.Empty, string.Empty);
        }

        internal void NewContact(ContactType contactType, ContactRole contactRole, string prenom = null, string nomNaissance = null, string societyName = null, Guid? guid = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == EditMode.Create && item.ViewModelPage.ViewModel.ContactType == contactType);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditContactUC userControl = new NewEditContactUC()
                    {
                        ViewModelPage = new NewEditContactUCVM()
                        {
                            EditMode = EditMode.Create,
                            ViewModel = new ContactVM()
                            {
                                ContactRole = contactRole,
                                ContactType = contactType,
                                NomNaissance = nomNaissance,
                                Prenom = prenom,
                                SocietyName = societyName,
                            },
                        }
                    };

                    userControl.LoadControl();

                    if (guid != null)
                    {
                        userControl.ViewModelPage.ParentGuid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditContactUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditContactUC_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
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
                ContactVM newViewModel = sender.ViewModelPage.ViewModel;

                var creationResult = await DbServices.Contact.CreateAsync(newViewModel);
                if (creationResult.IsSuccess)
                {
                    newViewModel.Id = creationResult.Id;
                    sender.ViewModelPage.ResultMessageTitle = "Succès";
                    sender.ViewModelPage.ResultMessage = creationResult.Message;
                    sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                    sender.ViewModelPage.IsResultMessageOpen = true;

                    if (sender.ViewModelPage.ParentGuid != null)
                    {
                        var bookManager = GetBookSideBarByGuid((Guid)sender.ViewModelPage.ParentGuid);
                        if (bookManager != null)
                        {
                            bookManager.ViewModelPage.ViewModel.Auteurs.Add(newViewModel);
                            NewEditContactUC_CancelModificationRequested(sender, e);
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
                        IdItem = userControl.ViewModelPage.ItemGuid,
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
        private void DisplayAuthorListXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }
        #endregion

        #region Editeurs
        private void DisplayEditorListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }
        #endregion

        #region Search
        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                ViewModelPage.ResearchBook = new ResearchBookVM()
                {
                    IdLibrary = Parameters.ParentLibrary.Id,
                    Term = sender.Text.Trim(),
                    TermParameter = Code.Search.Book.Terms.Contains,
                    SearchIn = new ObservableCollection<Code.Search.Book.In>()
                    {
                        Code.Search.Book.In.MainTitle,
                        Code.Search.Book.In.OtherTitle,
                        Code.Search.Book.In.Author
                    },
                };

                GroupItemsBySearch(ViewModelPage.ResearchBook);
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
                    if (item.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                    {
                        foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                        {
                            if (cmbxItem is SideBarItemHeaderVM headerVM)
                            {
                                if (itemHeader.Guid == headerVM.IdItem)
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

        private void SelectItemSideBar(PivotItem item)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!this.PivotRightSideBar.Items.Contains(item))
                {
                    return;
                }

                this.PivotRightSideBar.SelectedItem = item;

                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {
                    if (item.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                    {
                        foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                        {
                            if (cmbxItem is SideBarItemHeaderVM headerVM)
                            {
                                if (itemHeader.Guid == headerVM.IdItem)
                                {
                                    this.CmbxSideBarItemTitle.SelectedItem = headerVM;
                                    return;
                                }
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


        public ImportBookFromExcelUC GetImportBookFromExcelUC()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is ImportBookFromExcelUC);
                    if (itemPivot != null)
                    {
                        return itemPivot as ImportBookFromExcelUC;
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

        public ImportBookFromFileUC GetImportBookFromFileUC()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is ImportBookFromFileUC);
                    if (itemPivot != null)
                    {
                        return itemPivot as ImportBookFromFileUC;
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

        private NewEditBookExemplaryUC GetBookExemplarySideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookExemplaryUC item && item.ViewModelPage.ItemGuid == guid);
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
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is CategoriesListUC item && item.ViewModelPage.ItemGuid == guid);
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

        private BookExemplaryListUC GetBookExemplariesSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is BookExemplaryListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as BookExemplaryListUC;
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

        private BookPretListUC GetBookPretListSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is BookPretListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as BookPretListUC;
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
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC item && item.ViewModelPage.ItemGuid == guid);
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

        private CollectionListUC GetCollectionListSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is CollectionListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as CollectionListUC;
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
                //if (viewModel == null)
                //{
                //    return null;
                //}

                //if (this.PivotItems.SelectedItem != null)
                //{
                //    if (this.PivotItems.SelectedItem is IGrouping<string, LivreVM> group && group.Any(f => f == viewModel))
                //    {

                //        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                //        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                //        while (gridView != null && gridView.Name != "GridViewItems")
                //        {
                //            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                //            if (gridView == null)
                //            {
                //                return null;
                //            }
                //            else
                //            {
                //                if (gridView.Name == "GridViewItems")
                //                {
                //                    break;
                //                }
                //            }
                //        }

                //        if (gridView != null)
                //        {
                //            foreach (var gridViewItem in gridView.Items)
                //            {
                //                if (gridViewItem is LivreVM _viewModel && _viewModel == viewModel)
                //                {
                //                    if (gridView.SelectedItem != gridViewItem)
                //                    {
                //                        gridView.SelectedItem = gridViewItem;
                //                    }

                //                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                //                    return SelectImageFromContainer(_gridViewItemContainer);
                //                }
                //            }
                //        }
                //    }
                //}

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

        //public GridView GetSelectedGridView(string gridViewName = "GridViewItems")
        //{
        //    try
        //    {
        //        if (this.PivotItems.SelectedItem == null)
        //        {
        //            return null;
        //        }

        //        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
        //        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
        //        while (gridView != null && gridView.Name != gridViewName)
        //        {
        //            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
        //            if (gridView == null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                if (gridView.Name == gridViewName)
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        return gridView;
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return null;
        //    }
        //}

        //private DataGrid GetSelectedDataGridItems(string dataGridName = "DataGridItems")
        //{
        //    try
        //    {
        //        if (this.PivotItems.SelectedItem == null)
        //        {
        //            return null;
        //        }

        //        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
        //        DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container);
        //        while (dataGrid != null && dataGrid.Name != dataGridName)
        //        {
        //            dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(dataGrid);
        //            if (dataGrid == null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                if (dataGrid.Name == dataGridName)
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        return dataGrid;
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return null;
        //    }
        //}
        #endregion
        
        private async void Slider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                if (sender is Slider slider)
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.RefreshItemsGrouping();
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

        
    }
}
