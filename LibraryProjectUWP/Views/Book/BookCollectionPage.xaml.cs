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
using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.Views.Library;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.Views.UserControls.TitleBar;
using LibraryProjectUWP.ViewModels.Library;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI;
using LibraryProjectUWP.Views.Library.Manage;
using Windows.Media.Core;
using System.Data;
using LibraryProjectUWP.Views.Common;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookCollectionPage : Page
    {
        public BookCollectionPageVM ViewModelPage { get; set; } = new BookCollectionPageVM();
        public LibraryBookNavigationDriverVM Parameters { get; private set; }
        readonly EsGeneral esGeneral = new EsGeneral();
        readonly EsBook esBook = new EsBook();
        readonly UiServices uiServices = new UiServices();
        DispatcherTimer dispatcherTimerAddNewItem;
        public BookCollectionPage()
        {
            ViewModelPage.PropertyChanged += ViewModelPage_PropertyChanged;
            this.InitializeComponent();
        }

        public BookCollectionSubPage BookCollectionSubPage => FrameContainer.Content as BookCollectionSubPage;
        public ImportBookExcelSubPage ImportBookExcelSubPage => FrameContainer.Content as ImportBookExcelSubPage;
        public ImportBookFileSubPage ImportBookFileSubPage => FrameContainer.Content as ImportBookFileSubPage;
        public ImportItemsFromTablePage ImportItemsFromTablePage => FrameContainer.Content as ImportItemsFromTablePage;

        public bool IsContainsLibraryCollection(out LibraryCollectionSubPage subPage)
        {
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage item)
                {
                    subPage = item;
                    return true;
                }

                subPage = null;
                return false;
            }
            catch (Exception)
            {
                subPage = null;
                return false;
            }
        }

        public bool IsContainsBookCollection(out BookCollectionSubPage subPage)
        {
            try
            {
                if (FrameContainer.Content is BookCollectionSubPage item)
                {
                    subPage = item;
                    return true;
                }

                subPage = null;
                return false;
            }
            catch (Exception)
            {
                subPage = null;
                return false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryBookNavigationDriverVM parameters)
            {
                Parameters = parameters;
            }
        }

        #region Loading
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (Parameters.ParentLibrary == null)
                {
                    OpenLibraryCollection();
                }
                else
                {
                    OpenBookCollection(Parameters.ParentLibrary);
                }
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
                //_parameters.ParentLibrary.Books.Clear();
                //_parameters.ParentLibrary.Books.Clear();
                //ViewModelPage = null;
                //_parameters = null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async void OpenBookCollection(long idLibrary)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                BibliothequeVM viewModel = await DbServices.Library.SingleVMAsync(idLibrary);
                Parameters.ParentLibrary = viewModel;
                if (viewModel != null)
                {
                    OpenBookCollection(viewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenBookCollection(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                Parameters.ParentLibrary = viewModel;
                if (viewModel != null)
                {
                    ViewModelPage.SelectedItems.Clear();
                    ViewModelPage.SelectedItems = new List<object>();
                    this.Lv_SelectedItems.ItemTemplate = (DataTemplate)this.Resources["BookSuggestDataTemplate"];
                    
                    this.ViewModelPage.SelectedItemsMessage = "0 livre(s) sélectionné(s)";
                    this.ViewModelPage.NbItemsTitle = "Nombre total de livre";
                    this.ViewModelPage.NbElementDisplayedTitle = "Nombre de livres affichés";

                    Parameters.MainPage.ChangeAppTitle(new TitleBarLibraryName(viewModel)
                    {
                        Margin = new Thickness(0, 14, 0, 0),
                    });

                    this.NavigateToView(typeof(BookCollectionSubPage), this);
                    InitializeGroupsCmdBarItemsForBookCollection();
                    InitializeSortsCmdBarItemsForBookCollection();
                    InitializeAddsCmdBarItemsForBookCollection();
                    InitializeViewCmdBarItemsForBookCollection();
                    InitializeSecondaryCmdBarItemsForBookCollection();

                    CancelSearch();
                    this.RemoveAllItemToSideBar();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.Parameters.ParentLibrary = null;
                ViewModelPage.SelectedItems.Clear();
                ViewModelPage.SelectedItems = new List<object>();
                this.Lv_SelectedItems.ItemTemplate = (DataTemplate)this.Resources["LibrarySuggestDataTemplate"];
                
                this.ViewModelPage.SelectedItemsMessage = "0 bibliothèque(s) sélectionnée(s)";
                this.ViewModelPage.NbItemsTitle = "Nombre total de bibliothèque";
                this.ViewModelPage.NbElementDisplayedTitle = "Nombre de bibliothèques affichés";

                Parameters.MainPage.ChangeAppTitle(Parameters.MainPage.ViewModelPage.MainTitleBar);
                this.NavigateToView(typeof(LibraryCollectionSubPage), this);
                InitializeGroupsCmdBarItemsForLibraryCollection();
                InitializeSortsCmdBarItemsForLibraryCollection();
                InitializeAddsCmdBarItemsForLibraryCollection();
                InitializeViewCmdBarItemsForLibraryCollection();
                InitializeSecondaryCmdBarItemsForLibraryCollection();
                
                CancelSearch();
                this.RemoveAllItemToSideBar();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenImportBookFromExcel()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(ImportBookExcelSubPage), this);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenImportBookFromFile(IEnumerable<LivreVM> viewModelList)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(ImportBookFileSubPage), new BookSubPageParametersDriverVM()
                {
                    ParentPage = this,
                    ViewModelList = new ObservableCollection<LivreVM>(viewModelList),
                });
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenImportItemsFromTablePage<T, U>(U parentPage, IEnumerable<T> objectList, DataTable dataTable) 
            where T : class 
            where U : Page
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(ImportItemsFromTablePage), new Tuple<U, DataTable, IEnumerable<T>>(parentPage, dataTable, objectList));
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenBookPretSchedule()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(BookPretScheduleSubPage), this);
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
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    libraryCollectionSubPage.InitializeData();
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    bookCollectionSubPage.InitializeData();
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
        #endregion

        #region Selection
        private void ViewModelPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(BookCollectionPageVM.SelectedItems))
                {
                    UpdateCollections();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void UpdateCollections()
        {
            try
            {
                if (ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                {
                    if (ViewModelPage.SelectedItems.FirstOrDefault() is BibliothequeVM)
                    {
                        this.ViewModelPage.SelectedItemsMessage = $"{ViewModelPage.SelectedItems.Count} bibliothèque(s) sélectionnée(s)";
                    }
                    else if (ViewModelPage.SelectedItems.FirstOrDefault() is LivreVM)
                    {
                        this.ViewModelPage.SelectedItemsMessage = $"{ViewModelPage.SelectedItems.Count} livre(s) sélectionné(s)";
                    }
                    else
                    {
                        this.ViewModelPage.SelectedItemsMessage = string.Empty;
                    }
                }
                else
                {
                    if (FrameContainer.Content is LibraryCollectionSubPage)
                    {
                        this.ViewModelPage.SelectedItemsMessage = "0 bibliothèque(s) sélectionnée(s)";
                    }
                    else if (FrameContainer.Content is BookCollectionSubPage)
                    {
                        this.ViewModelPage.SelectedItemsMessage = "0 livre(s) sélectionné(s)";
                    }
                    else
                    {
                        this.ViewModelPage.SelectedItemsMessage = string.Empty;
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

        private void Lv_SelectedItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (sender is ListView listView)
                {
                    if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage && e.ClickedItem is BibliothequeVM bibliothequeVM)
                    {
                        libraryCollectionSubPage.SearchViewModel(bibliothequeVM);
                    }
                    else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage && e.ClickedItem is LivreVM livreVM)
                    {
                        bookCollectionSubPage.SearchViewModel(livreVM);
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
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    libraryCollectionSubPage.SelectAll();
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    bookCollectionSubPage.SelectAll();
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
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    libraryCollectionSubPage.DeSelectAll();
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    bookCollectionSubPage.DeSelectAll();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    await libraryCollectionSubPage.DeleteAllSelected();
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    await bookCollectionSubPage.DeleteAllSelected();
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
                    if (ViewModelPage.DataViewMode == DataViewModeEnum.GridView)
                    {
                        if (toggleButton.IsChecked != true)
                        {
                            toggleButton.IsChecked = true;
                        }
                        return;
                    }

                    await this.ViewMode(DataViewModeEnum.GridView, false);
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
                    await this.ViewMode(DataViewModeEnum.DataGridView, false);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        /// <summary>
        /// Change la disposition des éléments
        /// </summary>
        /// <param name="viewMode">Mode d'affichage</param>
        /// <param name="resetPage"></param>
        /// <returns></returns>
        private async Task ViewMode(DataViewModeEnum viewMode, bool resetPage)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.DataViewMode = viewMode;

                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    await libraryCollectionSubPage.ViewMode(viewMode, resetPage);
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    await bookCollectionSubPage.ViewMode(viewMode, resetPage);
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

        #region Groups
        private void MenuFlyoutCommandGroups_Opened(object sender, object e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.None)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "none") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "none").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.Letter)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "letter") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "letter").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.CreationYear)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "creation-year") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "creation-year").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else
                    {
                        MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem).Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                    }
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.None)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "none") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "none").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.Letter)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "letter") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "letter").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.CreationYear)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "creation-year") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "creation-year").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.ParutionYear)
                    {
                        if (MenuFlyoutCommandGroups.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "parution-year") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() != "parution-year").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else
                    {
                        MenuFlyoutCommandGroups.Items.Where(w => w is ToggleMenuFlyoutItem).Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeGroupsCmdBarItemsForLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandGroups.Items.Clear();

                //Group Letter
                ToggleMenuFlyoutItem TMFIGroupByLetter = new ToggleMenuFlyoutItem()
                {
                    Text = "Lettre",
                    Tag = "letter",
                };
                TMFIGroupByLetter.Click += TMFI_GroupByLetter_Click;

                //Group creation year
                ToggleMenuFlyoutItem TMFIGroupByCreationYear = new ToggleMenuFlyoutItem()
                {
                    Text = "Année de création",
                    Tag = "creation-year",
                };
                TMFIGroupByCreationYear.Click += TMFI_GroupByCreationYear_Click;

                //Group none
                ToggleMenuFlyoutItem TMFIGroupByNone = new ToggleMenuFlyoutItem()
                {
                    Text = "Aucun",
                    Tag = "none",
                };

                TMFIGroupByNone.Click += TMFI_GroupByNone_Click;

                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByCreationYear);
                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByLetter);
                MenuFlyoutCommandGroups.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByNone);
                this.ASB_SearchItem.PlaceholderText = "Rechercher une bibliothèque";
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeGroupsCmdBarItemsForBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandGroups.Items.Clear();

                //Group Letter
                ToggleMenuFlyoutItem TMFIGroupByLetter = new ToggleMenuFlyoutItem()
                {
                    Text = "Lettre",
                    Tag = "letter",
                };
                TMFIGroupByLetter.Click += TMFI_GroupByLetter_Click;

                //Group creation year
                ToggleMenuFlyoutItem TMFIGroupByCreationYear = new ToggleMenuFlyoutItem()
                {
                    Text = "Année de création",
                    Tag = "creation-year",
                };
                TMFIGroupByCreationYear.Click += TMFI_GroupByCreationYear_Click;

                //Group parution year
                ToggleMenuFlyoutItem TMFIGroupByParutionYear = new ToggleMenuFlyoutItem()
                {
                    Text = "Année de parution",
                    Tag = "parution-year",
                };
                TMFIGroupByParutionYear.Click += TMFI_GroupByParutionYear_Click;

                //Group none
                ToggleMenuFlyoutItem TMFIGroupByNone = new ToggleMenuFlyoutItem()
                {
                    Text = "Aucun",
                    Tag = "none"
                };

                TMFIGroupByNone.Click += TMFI_GroupByNone_Click;

                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByParutionYear);
                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByCreationYear);
                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByLetter);
                MenuFlyoutCommandGroups.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandGroups.Items.Add(TMFIGroupByNone);

                this.ASB_SearchItem.PlaceholderText = "Rechercher un livre";
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void TMFI_GroupByLetter_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.Letter)
                {
                    return;
                }
                libraryCollectionSubPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                this.GroupItemsBy($"Groupement des bibliothèques en cours par lettre...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.Letter)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                this.GroupItemsBy($"Groupement des livres en cours par lettre...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_GroupByCreationYear_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.CreationYear)
                {
                    return;
                }

                libraryCollectionSubPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
                this.GroupItemsBy($"Groupement des bibliothèques en cours par année de création...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.CreationYear)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                this.GroupItemsBy($"Groupement des livres en cours par année de création...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_GroupByParutionYear_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.CreationYear)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                this.GroupItemsBy($"Groupement des livres en cours par année de parution...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_GroupByNone_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.GroupedBy == LibraryGroupVM.GroupBy.CreationYear)
                {
                    return;
                }
                libraryCollectionSubPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
                this.GroupItemsBy($"Dégroupement des bibliothèques en cours...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.GroupedBy == BookGroupVM.GroupBy.CreationYear)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                this.GroupItemsBy($"Dégroupement des livres en cours...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }
        #endregion

        #region Sort & Order
        public void InitializeSortsCmdBarItemsForLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandSorts.Items.Clear();

                ToggleMenuFlyoutItem TMFIOrderByCroissant = new ToggleMenuFlyoutItem()
                {
                    Text = "Croissant",
                    Tag = "croissant",
                };
                TMFIOrderByCroissant.Click += TMFI_OrderByCroissant_Click;

                ToggleMenuFlyoutItem TMFIOrderByDCroissant = new ToggleMenuFlyoutItem()
                {
                    Text = "Décroissant",
                    Tag = "dcroissant",
                };
                TMFIOrderByDCroissant.Click += TMFI_OrderByDCroissant_Click;

                ToggleMenuFlyoutItem TMFISortByName = new ToggleMenuFlyoutItem()
                {
                    Text = "Nom",
                    Tag = "name",
                };

                TMFISortByName.Click += TMFI_SortByName_Click;

                ToggleMenuFlyoutItem TMFISortByDateCreation = new ToggleMenuFlyoutItem()
                {
                    Text = "Date de création",
                    Tag = "dateCreation",
                };
                TMFISortByDateCreation.Click += TMFI_SortByDateCreation_Click;

                MenuFlyoutCommandSorts.Items.Add(TMFIOrderByCroissant);
                MenuFlyoutCommandSorts.Items.Add(TMFIOrderByDCroissant);
                MenuFlyoutCommandSorts.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandSorts.Items.Add(TMFISortByName);
                MenuFlyoutCommandSorts.Items.Add(TMFISortByDateCreation);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeSortsCmdBarItemsForBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandSorts.Items.Clear();

                ToggleMenuFlyoutItem TMFIOrderByCroissant = new ToggleMenuFlyoutItem()
                {
                    Text = "Croissant",
                    Tag = "croissant",
                };
                TMFIOrderByCroissant.Click += TMFI_OrderByCroissant_Click;

                ToggleMenuFlyoutItem TMFIOrderByDCroissant = new ToggleMenuFlyoutItem()
                {
                    Text = "Décroissant",
                    Tag = "dcroissant",
                };
                TMFIOrderByDCroissant.Click += TMFI_OrderByDCroissant_Click;

                ToggleMenuFlyoutItem TMFISortByName = new ToggleMenuFlyoutItem()
                {
                    Text = "Titre",
                    Tag = "name",
                };

                TMFISortByName.Click += TMFI_SortByName_Click;

                ToggleMenuFlyoutItem TMFISortByDateCreation = new ToggleMenuFlyoutItem()
                {
                    Text = "Date de création",
                    Tag = "dateCreation",
                };

                TMFISortByDateCreation.Click += TMFI_SortByDateCreation_Click;

                MenuFlyoutCommandSorts.Items.Add(TMFIOrderByCroissant);
                MenuFlyoutCommandSorts.Items.Add(TMFIOrderByDCroissant);
                MenuFlyoutCommandSorts.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandSorts.Items.Add(TMFISortByName);
                MenuFlyoutCommandSorts.Items.Add(TMFISortByDateCreation);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void MenuFlyoutCommandSorts_Opened(object sender, object e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                {
                    if (libraryCollectionSubPage.ViewModelPage.OrderedBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "croissant") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "name" && item.Tag.ToString() != "dateCreation").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (libraryCollectionSubPage.ViewModelPage.OrderedBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "dcroissant") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "name" && item.Tag.ToString() != "dateCreation").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }

                    if (libraryCollectionSubPage.ViewModelPage.SortedBy == LibraryGroupVM.SortBy.Name)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "name") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "croissant" && item.Tag.ToString() != "dcroissant").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (libraryCollectionSubPage.ViewModelPage.SortedBy == LibraryGroupVM.SortBy.DateCreation)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "dateCreation") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "croissant" && item.Tag.ToString() != "dcroissant").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                }
                else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                {
                    if (bookCollectionSubPage.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.Croissant)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "croissant") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "name" && item.Tag.ToString() != "dateCreation").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (bookCollectionSubPage.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "dcroissant") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "name" && item.Tag.ToString() != "dateCreation").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }

                    if (bookCollectionSubPage.ViewModelPage.SortedBy == BookGroupVM.SortBy.Name)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "name") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "croissant" && item.Tag.ToString() != "dcroissant").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
                        }
                    }
                    else if (bookCollectionSubPage.ViewModelPage.SortedBy == BookGroupVM.SortBy.DateCreation)
                    {
                        if (MenuFlyoutCommandSorts.Items.SingleOrDefault(w => w is ToggleMenuFlyoutItem item && item.Tag.ToString() == "dateCreation") is ToggleMenuFlyoutItem currentItem)
                        {
                            currentItem.IsChecked = true;
                            MenuFlyoutCommandSorts.Items.Where(w => w is ToggleMenuFlyoutItem item && item.Tag != currentItem.Tag && item.Tag.ToString() != "croissant" && item.Tag.ToString() != "dcroissant").Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(x => x.IsChecked = false);
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

        private void TMFI_OrderByCroissant_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.OrderedBy == LibraryGroupVM.OrderBy.Croissant)
                {
                    return;
                }
                libraryCollectionSubPage.ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.Croissant;
                this.OrderItemsBy($"Organisation en cours des bibliothèques par ordre croissant...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.Croissant)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.OrderedBy = BookGroupVM.OrderBy.Croissant;
                this.OrderItemsBy($"Organisation en cours des livres par ordre croissant...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_OrderByDCroissant_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.OrderedBy == LibraryGroupVM.OrderBy.DCroissant)
                {
                    return;
                }
                libraryCollectionSubPage.ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.DCroissant;
                this.OrderItemsBy($"Organisation en cours des bibliothèques par ordre décroissant...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.DCroissant)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.OrderedBy = BookGroupVM.OrderBy.DCroissant;
                this.OrderItemsBy($"Organisation en cours des livres par ordre décroissant...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_SortByName_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.SortedBy == LibraryGroupVM.SortBy.Name)
                {
                    return;
                }
                libraryCollectionSubPage.ViewModelPage.SortedBy = LibraryGroupVM.SortBy.Name;
                this.SortItemsBy($"Organisation en cours des bibliothèques par nom ...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.SortedBy == BookGroupVM.SortBy.Name)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.SortedBy = BookGroupVM.SortBy.Name;
                this.SortItemsBy($"Organisation en cours des livres par nom ...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }

        private void TMFI_SortByDateCreation_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
            {
                if (libraryCollectionSubPage.ViewModelPage.SortedBy == LibraryGroupVM.SortBy.DateCreation)
                {
                    return;
                }

                libraryCollectionSubPage.ViewModelPage.SortedBy = LibraryGroupVM.SortBy.DateCreation;
                this.SortItemsBy($"Organisation en cours des bibliothèques par date de création...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
            else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
            {
                if (bookCollectionSubPage.ViewModelPage.SortedBy == BookGroupVM.SortBy.DateCreation)
                {
                    return;
                }
                bookCollectionSubPage.ViewModelPage.SortedBy = BookGroupVM.SortBy.DateCreation;
                this.SortItemsBy($"Organisation en cours des livres par date de création...", GetSelectedPage, true, ViewModelPage.ResearchItem);
            }
        }
        #endregion

        public void InitializeViewCmdBarItemsForLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandView.Items.Clear();

                MenuFlyoutItem MFIDisplayContacts = new MenuFlyoutItem()
                {
                    Text = "Afficher les contacts",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE77b",
                    }
                };
                MFIDisplayContacts.Click += MFIDisplayContacts_Click;


                MenuFlyoutCommandView.Items.Add(MFIDisplayContacts);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        public void InitializeViewCmdBarItemsForBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandView.Items.Clear();

                MenuFlyoutItem MFIDisplayCategories = new MenuFlyoutItem()
                {
                    Text = "Afficher les catégories",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE81E",
                    }
                };
                MFIDisplayCategories.Click += MFIDisplayCategories_Click;

                MenuFlyoutItem MFIDisplayCollections = new MenuFlyoutItem()
                {
                    Text = "Afficher les collections",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE81E",
                    }
                };
                MFIDisplayCollections.Click += MFIDisplayCollections_Click;

                MenuFlyoutItem MFIDisplayContacts = new MenuFlyoutItem()
                {
                    Text = "Afficher les contacts",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE77b",
                    }
                };
                MFIDisplayContacts.Click += MFIDisplayContacts_Click;

                MenuFlyoutItem MFIDisplayBookPretScheduler = new MenuFlyoutItem()
                {
                    Text = "Afficher l'agenda des prêts",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE748",
                    }
                };
                MFIDisplayBookPretScheduler.Click += MFIDisplayBookPretScheduler_Click;

                MenuFlyoutCommandView.Items.Add(MFIDisplayCategories);
                MenuFlyoutCommandView.Items.Add(MFIDisplayCollections);
                MenuFlyoutCommandView.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandView.Items.Add(MFIDisplayContacts);
                MenuFlyoutCommandView.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandView.Items.Add(MFIDisplayBookPretScheduler);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void MFIDisplayContacts_Click(object sender, RoutedEventArgs e)
        {
            DisplayContactsList(null, null);
        }

        private void MFIDisplayBookPretScheduler_Click(object sender, RoutedEventArgs e)
        {
            OpenBookPretSchedule();
        }

        private void MFIDisplayCollections_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayCollectionsList();
        }

        private void MFIDisplayCategories_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayCategoriesList();
        }

        private void MenuFlyoutCommandView_Opened(object sender, object e)
        {

        }

        public void InitializeAddsCmdBarItemsForLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandAdds.Items.Clear();

                MenuFlyoutItem TMFIAddNewItem = new MenuFlyoutItem()
                {
                    Text = "Nouvelle bibliothèque",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE710",
                    }
                };
                TMFIAddNewItem.Click += TMFIAddNewItem_Click;

                MenuFlyoutItem TMFIAddFromFile = new MenuFlyoutItem()
                {
                    Text = "Ouvrir un fichier",
                    IsEnabled = true,
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8B5",
                    }
                };
                TMFIAddFromFile.Click += TMFIAddFromFile_Click;

                MenuFlyoutItem TMFIAddNewHuman = new MenuFlyoutItem()
                {
                    Text = "Ajouter une personne",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE77b",
                    }
                };
                TMFIAddNewHuman.Click += TMFIAddNewHuman_Click;

                MenuFlyoutItem TMFIAddNewSociety = new MenuFlyoutItem()
                {
                    Text = "Ajouter une société",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE731",
                    }
                };
                TMFIAddNewSociety.Click += MFI_NewSociety_Click;

                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewItem);
                MenuFlyoutCommandAdds.Items.Add(TMFIAddFromFile);
                MenuFlyoutCommandAdds.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewHuman);
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewSociety);

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeAddsCmdBarItemsForBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MenuFlyoutCommandAdds.Items.Clear();

                MenuFlyoutSubItem MFSIAddNew = new MenuFlyoutSubItem()
                {
                    Text = "Ajouter un livre",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE736",
                    }
                };

                MenuFlyoutItem TMFIAddNewItem = new MenuFlyoutItem()
                {
                    Text = "Nouveau livre",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE710",
                    }
                };
                TMFIAddNewItem.Click += TMFIAddNewItem_Click;
                MFSIAddNew.Items.Add(TMFIAddNewItem);
                MFSIAddNew.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem TMFIAddFromWebsite = new MenuFlyoutItem()
                {
                    Text = "Lien Amazon (Expérimental)",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uF6FA",
                    }
                };
                TMFIAddFromWebsite.Click += TMFIAddFromWebsite_Click;
                MFSIAddNew.Items.Add(TMFIAddFromWebsite);
                MFSIAddNew.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem TMFIAddFromFile = new MenuFlyoutItem()
                {
                    Text = "Ouvrir un fichier",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8B5",
                    }
                };
                TMFIAddFromFile.Click += TMFIAddFromFile_Click;
                MFSIAddNew.Items.Add(TMFIAddFromFile);

                MenuFlyoutItem TMFIAddFromExcelFile = new MenuFlyoutItem()
                {
                    Text = "Ouvrir un classeur Excel",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8B5",
                    }
                };
                TMFIAddFromExcelFile.Click += TMFIAddFromExcelFile_Click;
                MFSIAddNew.Items.Add(TMFIAddFromExcelFile);

                MenuFlyoutItem TMFIAddNewHuman = new MenuFlyoutItem()
                {
                    Text = "Ajouter une personne",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE77b",
                    }
                };
                TMFIAddNewHuman.Click += TMFIAddNewHuman_Click;

                MenuFlyoutItem TMFIAddNewSociety = new MenuFlyoutItem()
                {
                    Text = "Ajouter une société",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE731",
                    }
                };
                TMFIAddNewSociety.Click += MFI_NewSociety_Click;

                MenuFlyoutItem TMFIAddNewCollection = new MenuFlyoutItem()
                {
                    Text = "Ajouter une collection",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE81E",
                    }
                };
                TMFIAddNewCollection.Click += TMFIAddNewCollection_Click;

                MenuFlyoutCommandAdds.Items.Add(MFSIAddNew);
                MenuFlyoutCommandAdds.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewHuman);
                MenuFlyoutCommandAdds.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewSociety);
                MenuFlyoutCommandAdds.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewCollection);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ABBAddItem_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    if (this.TeachTipNewHelp.IsOpen == false && (dispatcherTimerAddNewItem == null || (dispatcherTimerAddNewItem != null && !dispatcherTimerAddNewItem.IsEnabled)))
                    {
                        dispatcherTimerAddNewItem = new DispatcherTimer()
                        {
                            Interval = new TimeSpan(0, 0, 0, 3),
                        };

                        dispatcherTimerAddNewItem.Tick += (f, i) =>
                        {
                            this.TeachTipNewHelp.IsOpen = true;
                            InitializeAddNewTeachingTip();
                            dispatcherTimerAddNewItem?.Stop();
                        };
                        dispatcherTimerAddNewItem.Start();
                    }
                }
                else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
                {
#warning Implémenter le teachingTip pour les livres
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ABBAddItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (this.dispatcherTimerAddNewItem != null)
                {
                    if (dispatcherTimerAddNewItem?.IsEnabled == true)
                    {
                        dispatcherTimerAddNewItem?.Stop();
                    }

                    this.dispatcherTimerAddNewItem = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void TMFIAddNewCollection_Click(object sender, RoutedEventArgs e)
        {
            NewEditCollection(null, EditMode.Create);
        }

        private void TMFIAddNewHuman_Click(object sender, RoutedEventArgs e)
        {
            this.NewEditContact(EditMode.Create, ContactType.Human, null, null);
        }

        private void MFI_NewSociety_Click(object sender, RoutedEventArgs e)
        {
            this.NewEditContact(EditMode.Create, ContactType.Society, null, null);
        }

        private void MenuFlyoutCommandAdds_Opened(object sender, object e)
        {

        }

        private async void TMFIAddNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage)
            {
                await NewEditLibraryAsync(new BibliothequeVM(), EditMode.Create);
            }
            else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
            {
                await NewEditBookAsync(new LivreVM()
                {
                    IdLibrary = Parameters.ParentLibrary.Id,
                }, EditMode.Create);
            }
        }

        private async void TMFIAddFromWebsite_Click(object sender, RoutedEventArgs e)
        {
            if (FrameContainer.Content is LibraryCollectionSubPage)
            {
#warning Importer une bibliothèque via un site (peut-être)
            }
            else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
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
                            await NewEditBookAsync(viewModel, EditMode.Create);
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
        }

        private async void TMFIAddFromFile_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    var storageFile = await Files.OpenStorageFileAsync(Files.LibraryExtensions);
                    if (storageFile == null)
                    {
                        Logs.Log(m, $"Vous devez sélectionner un type de fichier valide.");
                        return;
                    }

                    var viewModels = (await esGeneral.OpenItemFromFileAsync<BibliothequeVM>(storageFile))?.ToList();
                    if (viewModels != null && viewModels.Any())
                    {
                        ImportLibraryFromFile(viewModels, storageFile);
                    }
                }
                else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
                {
                    var storageFile = await Files.OpenStorageFileAsync(Files.BookExtensions);
                    if (storageFile == null)
                    {
                        Logs.Log(m, $"Vous devez sélectionner un type de fichier valide.");
                        return;
                    }

                    var viewModels = (await esGeneral.OpenItemFromFileAsync<LivreVM>(storageFile))?.ToList();
                    if (viewModels != null && viewModels.Any())
                    {
                        viewModels.ForEach((book) => this.CompleteBookInfos(book));

                        if (viewModels.Count() == 1)
                        {
                            await NewEditBookAsync(viewModels.First(), EditMode.Create);
                        }
                        else
                        {
                            ImportBookFromFile(viewModels, storageFile);
                            OpenImportBookFromFile(viewModels);
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

        private async void TMFIAddFromExcelFile_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
#warning Importer une bibliothèque via un fichier (excel);
                }
                else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
                {
                    var storageFile = await Files.OpenStorageFileAsync(Files.ExcelExtensions);
                    if (storageFile == null)
                    {
                        Logs.Log(m, $"Vous devez sélectionner un fichier de type Microsoft Excel.");
                        return;
                    }

                    this.ImportBookFromExcelFile(storageFile);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeSecondaryCmdBarItemsForLibraryCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MyCommand.SecondaryCommands.Clear();

                AppBarButton AppBarBtnChangeBackground = new AppBarButton()
                {
                    Label = "Modifier l'image d'arrière-plan",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uEB9F",
                    }
                };
                AppBarBtnChangeBackground.Click += AppBarBtnChangeBackground_Click;

                AppBarButton AppBarBtnOpenContentFolder = new AppBarButton()
                {
                    Label = "Ouvrir le dossier contenant",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE838",
                    }
                };
                AppBarBtnOpenContentFolder.Click += AppBarBtnOpenContentFolder_Click;

                AppBarButton AppBarBtnExportAllItems = new AppBarButton()
                {
                    Label = "Exporter toutes les bibliothèques",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uEDE1",
                    }
                };
                AppBarBtnExportAllItems.Click += AppBarBtnExportAllItems_Click;

                //AppBarButton AppBarBtnHelp = new AppBarButton()
                //{
                //    Label = "Aide",
                //    Icon = new FontIcon
                //    {
                //        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //        Glyph = "\uE897",
                //    }
                //};
                //AppBarBtnHelp.Click += AppBarBtnHelp_Click;

                MyCommand.SecondaryCommands.Add(AppBarBtnChangeBackground);
                MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                MyCommand.SecondaryCommands.Add(AppBarBtnOpenContentFolder);
                MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                MyCommand.SecondaryCommands.Add(AppBarBtnExportAllItems);
                //MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                //MyCommand.SecondaryCommands.Add(AppBarBtnHelp);

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeSecondaryCmdBarItemsForBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                MyCommand.SecondaryCommands.Clear();

                AppBarButton AppBarBtnChangeBackground = new AppBarButton()
                {
                    Label = "Modifier l'image d'arrière-plan",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uEB9F",
                    }
                };
                AppBarBtnChangeBackground.Click += AppBarBtnChangeBackground_Click;

                AppBarButton AppBarBtnOpenContentFolder = new AppBarButton()
                {
                    Label = "Ouvrir le dossier contenant",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE838",
                    }
                };
                AppBarBtnOpenContentFolder.Click += AppBarBtnOpenContentFolder_Click;

                AppBarButton AppBarBtnExportAllItems = new AppBarButton()
                {
                    Label = "Exporter tous les livres",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uEDE1",
                    }
                };
                AppBarBtnExportAllItems.Click += AppBarBtnExportAllItems_Click;

                AppBarButton AppBarBtnCloseLibrary = new AppBarButton()
                {
                    Label = "Quitter la bibliothèque",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8BB",
                    }
                };
                AppBarBtnCloseLibrary.Click += AppBarBtnCloseLibrary_Click;

                //AppBarButton AppBarBtnHelp = new AppBarButton()
                //{
                //    Label = "Aide",
                //    Icon = new FontIcon
                //    {
                //        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //        Glyph = "\uE897",
                //    }
                //};
                //AppBarBtnHelp.Click += AppBarBtnHelp_Click;

                MyCommand.SecondaryCommands.Add(AppBarBtnChangeBackground);
                MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                MyCommand.SecondaryCommands.Add(AppBarBtnOpenContentFolder);
                MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                MyCommand.SecondaryCommands.Add(AppBarBtnExportAllItems);
                MyCommand.SecondaryCommands.Add(new AppBarSeparator());
                //MyCommand.SecondaryCommands.Add(AppBarBtnHelp);
                MyCommand.SecondaryCommands.Add(AppBarBtnCloseLibrary);

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void AppBarBtnCloseLibrary_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                bool isClosable = await CloseLibraryAsync();
                if (isClosable)
                {
                    this.RemoveAllItemToSideBar();
                    this.OpenLibraryCollection(); 
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void AppBarBtnHelp_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AppBarBtnExportAllItems_Click(object sender, RoutedEventArgs e)
        {
            ExportAllItems();
        }

        private async void AppBarBtnOpenContentFolder_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = await Folders.OpenApplicationLocalFolderAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void AppBarBtnChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                EsLibrary esLibrary = new EsLibrary();
                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    var result = await esLibrary.ChangeLibraryCollectionBackgroundImageAsync();
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    ViewModelPage.BackgroundImagePath = result.Result?.ToString() ?? EsGeneral.LibraryCollectionDefaultBackgroundImage;
                    await InitializeBackgroundImagesync();
                }
                else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
                {
                    var result = await esLibrary.ChangeBookCollectionBackgroundImageAsync(Parameters.ParentLibrary.Guid);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    ViewModelPage.BackgroundImagePath = result.Result?.ToString() ?? EsGeneral.BookCollectionDefaultBackgroundImage;
                    await InitializeBackgroundImagesync();
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
        public async Task InitializeBackgroundImagesync()
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
            
        }

        #endregion

        #region New-Edit Book
        public async Task NewEditBookAsync(LivreVM viewModel, EditMode editMode = EditMode.Create)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.IdLibrary == null || viewModel.IdLibrary < 1)
                {
                    viewModel.IdLibrary = Parameters.ParentLibrary.Id;
                }

                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC item && item.ViewModelPage.EditMode == editMode) is NewEditBookUC checkedItem)
                {
                    var isModificationStateChecked = await checkedItem.CheckModificationsStateAsync();
                    if (isModificationStateChecked)
                    {
                        checkedItem.InitializeSideBar(Parameters.ParentLibrary.Id, this, viewModel, editMode);
                        this.SelectItemSideBar(checkedItem);
                    }
                }
                else
                {
                    NewEditBookUC userControl = new NewEditBookUC();
                    userControl.InitializeSideBar(Parameters.ParentLibrary.Id, this, viewModel, editMode);

                    userControl.CancelModificationRequested += NewEditBookUC_CancelModificationRequested;
                    userControl.ExecuteTaskRequested += NewEditBookUC_ExecuteTaskRequested;

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

        private void NewEditBookUC_ExecuteTaskRequested(NewEditBookUC sender, LivreVM originalViewModel, OperationStateVM e)
        {
            this.RemoveItemToSideBar(sender);
        }

        private void NewEditBookUC_CancelModificationRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            this.RemoveItemToSideBar(sender);
        }

        public async Task NewEditLibraryAsync(BibliothequeVM viewModel, EditMode editMode = EditMode.Create)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditLibraryUC item && item.ViewModelPage.EditMode == editMode) is NewEditLibraryUC checkedItem)
                {
                    var isModificationStateChecked = await checkedItem.CheckModificationsStateAsync();
                    if (isModificationStateChecked)
                    {
                        checkedItem.InitializeSideBar(this, viewModel, editMode);
                        this.SelectItemSideBar(checkedItem);
                    }
                }
                else
                {
                    NewEditLibraryUC userControl = new NewEditLibraryUC();
                    userControl.InitializeSideBar(this, viewModel, editMode);

                    userControl.CancelModificationRequested += NewEditLibraryUC_CancelModificationRequested;
                    userControl.ExecuteTaskRequested += NewEditLibraryUC_ExecuteTaskRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ItemGuid,
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

        private async void NewEditLibraryUC_ExecuteTaskRequested(NewEditLibraryUC sender, BibliothequeVM originalViewModel, OperationStateVM e)
        {
            if (e.IsSuccess)
            {
                await this.RefreshItemsGrouping(this.GetSelectedPage, true, this.ViewModelPage.ResearchItem);
                this.RemoveItemToSideBar(sender);
            }
        }

        private void NewEditLibraryUC_CancelModificationRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            this.RemoveItemToSideBar(sender);
        }
        #endregion

        #region Import Book
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

                var viewModels = (await esGeneral.OpenItemFromFileAsync<LivreVM>(storageFile))?.ToList();
                if (viewModels != null && viewModels.Any())
                {
                    viewModels.ForEach((book) => this.CompleteBookInfos(book));

                    if (viewModels.Count() == 1)
                    {
                        await NewEditBookAsync(viewModels.First(), EditMode.Create);
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
                ImportBooksOrLibrariesTask importBooksTask = new ImportBooksOrLibrariesTask(Parameters.MainPage, Parameters.ParentLibrary.Id);
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
                this.OpenBookCollection(Parameters.ParentLibrary);
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
                ImportBooksOrLibrariesTask importBooksTask = new ImportBooksOrLibrariesTask(Parameters.MainPage, Parameters.ParentLibrary.Id)
                {
                    CloseBusyLoaderAfterFinish = true,
                };
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
                this.OpenBookCollection(Parameters.ParentLibrary);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        private void ImportBooksTask_AfterTaskCompletedRequested(ImportBooksOrLibrariesTask sender, object e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.OpenBookCollection(sender.IdLibrary);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void ImportLibraryFromFile(IEnumerable<BibliothequeVM> viewModelList, StorageFile file)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModelList == null || !viewModelList.Any())
                {
                    return;
                }

                DataTable dataTable = PropertyHelpers.CreateDataTableOfObject(viewModelList, new string[]
                {
                    nameof(BibliothequeVM.Id),
                    nameof(BibliothequeVM.Guid),
                    nameof(BibliothequeVM.DateEdition),
                    nameof(BibliothequeVM.DateAjout),
                    nameof(BibliothequeVM.Collections),
                    nameof(BibliothequeVM.Books),
                    nameof(BibliothequeVM.CountNotInCollectionBooks),
                    nameof(BibliothequeVM.JaquettePath),
                    nameof(BibliothequeVM.Categories),
                    nameof(BibliothequeVM.CountUnCategorizedBooks),
                });

                OpenImportItemsFromTablePage(this, viewModelList, dataTable);
                ImportItemsFromFile(viewModelList, file);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void ImportItemsFromFile<T>(IEnumerable<T> objectList, StorageFile file = null) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is ImportItemsFromFileSideBar item) is ImportItemsFromFileSideBar checkedItem)
                {
                    checkedItem.InitializeSideBar(this, objectList, file);
                    this.SelectItemSideBar(checkedItem);
                }
                else
                {
                    ImportItemsFromFileSideBar userControl = new ImportItemsFromFileSideBar();
                    userControl.InitializeSideBar(this, objectList, file);

                    userControl.CancelModificationRequested += ImportItemsFromFileSideBar_CancelModificationRequested;
                    //userControl.ExecuteTaskRequested += NewEditBookUC_ExecuteTaskRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ItemGuid,
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

        private void ImportItemsFromFileSideBar_CancelModificationRequested(ImportItemsFromFileSideBar sender, ExecuteRequestedEventArgs e)
        {
            this.RemoveItemToSideBar(sender);
            if (this.Parameters.ParentLibrary == null)
            {
                this.OpenLibraryCollection();
            }
            else
            {
                this.OpenBookCollection(this.Parameters.ParentLibrary);
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


        #region Export items
        public void ExportThisLibrary(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel != null)
                {
                    ExportAllBooksOrLibrariesTask exportAllBooksOrLibrariesTask = new ExportAllBooksOrLibrariesTask(Parameters.MainPage)
                    {
                        UseIntervalAfterFinish = false
                    };
                    exportAllBooksOrLibrariesTask.InitializeWorker(viewModel);
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

        public void ExportAllItems()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ExportAllBooksOrLibrariesTask exportAllBooksOrLibrariesTask = new ExportAllBooksOrLibrariesTask(Parameters.MainPage)
                {
                    UseIntervalAfterFinish = false
                };

                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    exportAllBooksOrLibrariesTask.InitializeWorker();
                }
                else if (FrameContainer.Content is BookCollectionSubPage && Parameters.ParentLibrary != null)
                {
                    exportAllBooksOrLibrariesTask.InitializeWorker(Parameters.ParentLibrary);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Delete Book

        

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
                            ViewModelPage.SelectedItems = new ObservableCollection<object>();

                            var busyLoader = Parameters.MainPage.GetBusyLoader ;
                            if (busyLoader != null)
                            {
                                busyLoader.TbcTitle.Text = "Actualisation du catalogue des livres en cours...";
                            }

                            await this.RefreshItemsGrouping(1, true, ViewModelPage.ResearchItem);

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
        
        internal void NewEditContact(EditMode editMode = EditMode.Create, ContactType? contactType = null, IEnumerable<ContactRole> contactRoles = null, SideBarInterLinkVM parentReferences = null, ContactVM viewModel = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditContactUC item && item.ViewModelPage.EditMode == editMode && item.ViewModelPage.ViewModel.ContactType == contactType) is NewEditContactUC checkedItem)
                {
                    checkedItem.InitializeSideBar(this, editMode, contactType, contactRoles, viewModel);
                    this.SelectItemSideBar(checkedItem);
                }
                else
                {
                    NewEditContactUC userControl = new NewEditContactUC();
                    userControl.InitializeSideBar(this, editMode, contactType, contactRoles, viewModel);

                    if (parentReferences != null)
                    {
                        userControl.ParentReferences = parentReferences;
                    }

                    userControl.CancelModificationRequested += NewEditContactUC_CancelModificationRequested;
                    userControl.ExecuteTaskRequested += NewEditContactUC_ExecuteTaskRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ItemGuid,
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

        private void NewEditContactUC_ExecuteTaskRequested(NewEditContactUC sender, ContactVM originalViewModel, OperationStateVM e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (e.IsSuccess)
                {
                    if (sender.ParentReferences != null)
                    {
                        if (sender.ParentReferences.ParentType == typeof(ContactListUC))
                        {
                            var item = uiServices.GetContactListUCSideBarByGuid(PivotRightSideBar, sender.ParentReferences.ParentGuid);
                            if (item != null)
                            {
                                item.InitializeSideBar(this, item.ViewModelPage.ContactType);
                                SelectItemSideBar(item);
                            }
                        }
                    }
                    this.RemoveItemToSideBar(sender);
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
            this.RemoveItemToSideBar(sender);
        }

        private void DisplayContactsList(ContactType? contactType, IEnumerable<ContactRole> contactRoles = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is ContactListUC) is ContactListUC checkedItem)
                {
                    this.SelectItemSideBar(checkedItem);
                    checkedItem.InitializeSideBar(this, contactType, contactRoles);
                }
                else
                {
                    ContactListUC userControl = new ContactListUC();
                    userControl.InitializeSideBar(this, contactType, contactRoles);
                    userControl.CancelModificationRequested += ContactListUC_CancelModificationRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ItemGuid,
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

        private void ContactListUC_CancelModificationRequested(ContactListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.RemoveItemToSideBar(sender);
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
                    MyTeachingTip.Target = sender;
                    MyTeachingTip.Title = sender.PlaceholderText;
                    MyTeachingTip.Subtitle = "Vous devez d'abord entrer votre mot-clé avant de lancer la recherche.";
                    MyTeachingTip.IsOpen = true;
                    return;
                }

                if (MyTeachingTip.IsOpen)
                {
                    MyTeachingTip.IsOpen = false;
                }

                if (ViewModelPage.ResearchItem == null)
                {
                    ViewModelPage.ResearchItem = new ResearchItemVM()
                    {
                        Term = sender.Text?.Trim(),
                        TermParameter = Code.Search.Terms.Contains,
                        SearchInAuthors = true,
                        SearchInMainTitle = true,
                        SearchInEditors = true,
                        SearchInOtherTitles = true,
                        SearchInCollections = false,
                    };
                }

                if (ViewModelPage.ResearchItem != null)
                {
                    ViewModelPage.ResearchItem.Term = sender.Text?.Trim();
                    if (FrameContainer.Content is LibraryCollectionSubPage)
                    {
                        ViewModelPage.ResearchItem.TypeObject = typeof(BibliothequeVM);
                        
                        Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Recherche en cours de bibliothèques avec le terme « {ViewModelPage.ResearchItem.Term} » ...",
                        });
                    }
                    else if (FrameContainer.Content is BookCollectionSubPage)
                    {
                        ViewModelPage.ResearchItem.TypeObject = typeof(LivreVM);
                        ViewModelPage.ResearchItem.IdLibrary = Parameters?.ParentLibrary?.Id;
                        
                        Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Recherche en cours de livres avec le terme « {ViewModelPage.ResearchItem.Term} » ...",
                        });
                    }
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    await this.RefreshItemsGrouping(1, true, ViewModelPage.ResearchItem);

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, d) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        SearchItems(ViewModelPage.ResearchItem);

                        dispatcherTimer2.Stop();
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SearchItems(ResearchItemVM ResearchItemVM)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is SearchBookUC item) is SearchBookUC checkedItem)
                {
                    checkedItem.InitializeSideBar(this, ResearchItemVM, Parameters?.ParentLibrary?.Id);
                    this.SelectItemSideBar(checkedItem);
                }
                else
                {
                    SearchBookUC userControl = new SearchBookUC();
                    userControl.InitializeSideBar(this, ResearchItemVM, Parameters?.ParentLibrary?.Id);

                    userControl.CancelModificationRequested += SearchBookUC_CancelModificationRequested;
                    userControl.SearchBookRequested += SearchBookUC_SearchBookRequested;
                    userControl.HideSearchBookPanelRequested += SearchBookUC_HideSearchBookPanelRequested;
                    
                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ItemGuid,
                    });
                }

                this.ViewModelPage.IsSplitViewOpen = true;
                this.ViewModelPage.IsGroupBookAppBarBtnEnabled = false;
                this.ViewModelPage.IsSortBookAppBarBtnEnabled = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchBookUC_HideSearchBookPanelRequested(SearchBookUC sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= SearchBookUC_CancelModificationRequested;
                sender.SearchBookRequested -= SearchBookUC_SearchBookRequested;
                sender.HideSearchBookPanelRequested -= SearchBookUC_HideSearchBookPanelRequested;
                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchBookUC_SearchBookRequested(SearchBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ASB_SearchItem.Text = sender.ViewModelPage.ViewModel.Term;

                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Recherche en cours de bibliothèques avec le terme « {ViewModelPage.ResearchItem.Term} » ...",
                    });
                }
                else if (FrameContainer.Content is BookCollectionSubPage)
                {
                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Recherche en cours des livres avec le terme « {sender.ViewModelPage.ViewModel.Term} » ...",
                    });
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    await this.RefreshItemsGrouping(1, true, sender.ViewModelPage.ViewModel);

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, d) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        dispatcherTimer2.Stop();
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
                
                this.ViewModelPage.IsGroupBookAppBarBtnEnabled = false;
                this.ViewModelPage.IsSortBookAppBarBtnEnabled = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchBookUC_CancelModificationRequested(SearchBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= SearchBookUC_CancelModificationRequested;
                sender.SearchBookRequested -= SearchBookUC_SearchBookRequested;
                this.RemoveItemToSideBar(sender);
                ViewModelPage.ResearchItem = null;

                UpdateItemsAfterQuitSearch();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SplitBtnSearch_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            try
            {
                SearchBookUC searchBookUC = uiServices.GetSearchBookUCSideBar(this.PivotRightSideBar);
                if (searchBookUC != null)
                {
                    if (!(this.PivotRightSideBar.SelectedItem is SearchBookUC))
                    {
                        this.SelectItemSideBar(searchBookUC);
                    }
                    else
                    {
                        SearchBookUC_HideSearchBookPanelRequested(searchBookUC, null);
                    }
                }
                else
                {
                    SearchItems(ViewModelPage.ResearchItem);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void MFIQuitSearch_Click(object sender, RoutedEventArgs e)
        {
            CancelSearch();
            UpdateItemsAfterQuitSearch();
        }

        public void CancelSearch()
        {
            try
            {
                ViewModelPage.ResearchItem = null;
                ASB_SearchItem.Text = string.Empty;
                SearchBookUC searchBookUC = uiServices.GetSearchBookUCSideBar(this.PivotRightSideBar);
                if (searchBookUC != null)
                {
                    this.RemoveItemToSideBar(searchBookUC);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void UpdateItemsAfterQuitSearch()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.Content is LibraryCollectionSubPage)
                {
                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Reconstruction du catalogue des bibliothèques en cours ...",
                    });
                }
                else if (FrameContainer.Content is BookCollectionSubPage)
                {
                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Reconstruction du catalogue des livres en cours ...",
                    });
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    await this.RefreshItemsGrouping();

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, d) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        ASB_SearchItem.Text = String.Empty;

                        dispatcherTimer2.Stop();
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Collections
        internal void NewEditCollection(CollectionVM viewModel = null, EditMode editMode = EditMode.Create, SideBarInterLinkVM parentReferences = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCollectionUC item && item.ViewModelPage.EditMode == editMode);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCollectionUC userControl = new NewEditCollectionUC();
                    userControl.InitializeSideBar(Parameters.ParentLibrary.Id, this, viewModel, editMode, parentReferences);

                    if (parentReferences != null)
                    {
                        userControl.ViewModelPage.ParentReferences = parentReferences;
                    }

                    userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
                    userControl.ExecuteTaskRequested += NewEditCollectionUC_ExecuteTaskRequested;

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

        private void NewEditCollectionUC_ExecuteTaskRequested(NewEditCollectionUC sender, CollectionVM originalViewModel, OperationStateVM e)
        {
            try
            {
                if (e.IsSuccess)
                {
                    if (sender.ViewModelPage.ParentReferences != null)
                    {
                        if (sender.ViewModelPage.ParentReferences.ParentType == typeof(CollectionListUC))
                        {
                            var item = uiServices.GetCollectionListUCSideBarByGuid(PivotRightSideBar, sender.ViewModelPage.ParentReferences.ParentGuid);
                            if (item != null)
                            {
                                item.InitializeData(this);
                                SelectItemSideBar(item);
                            }
                        }                        
                    }
                }

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditCollectionUC_Create_CancelModificationRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
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
                    if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                    {
                        return libraryCollectionSubPage.ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                    }
                    else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                    {
                        return bookCollectionSubPage.ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return 1;
                }
            }
        }

        public void CompleteBookInfos(LivreVM viewModel)
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

        private void RemoveAllItemToSideBar()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {
                    for (int i = 0; i < this.CmbxSideBarItemTitle.Items.Count; i++)
                    {
                        if (this.CmbxSideBarItemTitle.Items[i] is SideBarItemHeaderVM headerVM)
                        {
                            ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                            i = 0;
                            continue;
                        }
                    }
                }

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    for (int i = 0; i < this.PivotRightSideBar.Items.Count; i++)
                    {
                        this.PivotRightSideBar.Items.RemoveAt(i);
                        i = 0;
                        continue;
                    }
                }

                this.ViewModelPage.IsSplitViewOpen = false;
                this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
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

        public ImportItemsFromFileSideBar GetImportItemsFromFileSideBar()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is ImportItemsFromFileSideBar);
                    if (itemPivot != null)
                    {
                        return itemPivot as ImportItemsFromFileSideBar;
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
        #endregion
        
        private async void Slider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                if (sender is Slider slider)
                {
                    if (FrameContainer.Content is LibraryCollectionSubPage libraryCollectionSubPage)
                    {
                        await libraryCollectionSubPage.CommonView.RefreshItemsGrouping();
                    }
                    else if (FrameContainer.Content is BookCollectionSubPage bookCollectionSubPage)
                    {
                        await bookCollectionSubPage.CommonView.RefreshItemsGrouping();
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

        public async Task<bool> CloseLibraryAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditBookUC) is NewEditBookUC bookeditor)
                {
                    //bookeditor.Focus(FocusState.Programmatic);
                    var isModificationStateChecked = await bookeditor.CheckModificationsStateAsync();
                    if (!isModificationStateChecked)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return true;
            }
        }

        #region Teaching Tips
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is FlipView flipView)
                {
                    if (flipView.SelectedIndex > -1 && flipView.Items[flipView.SelectedIndex] is FlipViewItem flipViewItem)
                    {
                        if (flipView.SelectedIndex == 0)
                        {
                            TeachTipNewHelp.Title = "Nouvelle bibliothèque";
                            TeachTipNewHelp.Subtitle = "Suivez cette courte vidéo pour apprendre comment créer une nouvelle bibliothèque.";

                            if (flipViewItem.Content is MediaPlayerElement playerElement)
                            {
                                playerElement.MediaPlayer.Play();
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

        private void InitializeAddNewTeachingTip()
        {
            try
            {
                if (FlipViewTeachAddNew.Items.Count == 0)
                {
                    FlipViewItem flipViewItem = new FlipViewItem();

                    if (!flipViewItem.IsSelected)
                        flipViewItem.IsSelected = true;

                    TeachTipNewHelp.Title = "Nouvelle bibliothèque";
                    TeachTipNewHelp.Subtitle = "Suivez cette courte vidéo pour apprendre comment créer une nouvelle bibliothèque.";

                    MediaPlayerElement mediaPlayer = new MediaPlayerElement()
                    {
                        Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/VTutos/AddNewLibrary.mp4")),
                        AutoPlay = true,
                        AreTransportControlsEnabled = false,
                        Stretch = Stretch.UniformToFill,
                    };
                    flipViewItem.Content = mediaPlayer;
                    FlipViewTeachAddNew.Items.Add(flipViewItem);

                    mediaPlayer.MediaPlayer.IsLoopingEnabled = true;
                    mediaPlayer.MediaPlayer.Play();
                }

                //FlipViewPipsPager.NumberOfPages = FlipViewTeachAddNew.Items.Count;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void TeachTipNewHelp_CloseButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            if (dispatcherTimerAddNewItem != null)
            {
                if (dispatcherTimerAddNewItem?.IsEnabled == true)
                {
                    dispatcherTimerAddNewItem?.Stop();
                }

                dispatcherTimerAddNewItem = null;
            }

            foreach (var item in FlipViewTeachAddNew.Items)
            {
                if (item is FlipViewItem flipViewItem)
                {
                    if (flipViewItem.Content is MediaPlayerElement mediaPlayerElement)
                    {
                        if (mediaPlayerElement.MediaPlayer != null)
                        {
                            try
                            {
                                if (mediaPlayerElement.MediaPlayer.PlaybackSession.CanPause)
                                {
                                    mediaPlayerElement.MediaPlayer.Pause();
                                }
                                //mediaPlayerElement.MediaPlayer.Dispose();
                                if (mediaPlayerElement.Source is MediaSource ms)
                                {
                                    ms.Dispose();
                                    ms = null;
                                }
                                mediaPlayerElement.Source = null;
                                //mediaPlayerElement = null;
                            }
                            catch (Exception ex)
                            {
                                MethodBase m = MethodBase.GetCurrentMethod();
                                Logs.Log(ex, m);
                                continue;
                            }

                        }
                        flipViewItem.Content = null;
                    }
                }
            }

            FlipViewTeachAddNew.Items.Clear();
            //FlipViewPipsPager.NumberOfPages = FlipViewTeachAddNew.Items.Count;
        }

        #endregion    
    }
}
