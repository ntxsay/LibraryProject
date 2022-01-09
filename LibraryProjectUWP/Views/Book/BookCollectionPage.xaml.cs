using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book.Collection;
using LibraryProjectUWP.Views.Contact;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using LibraryProjectUWP.Views.Author;
using LibraryProjectUWP.Views.Collection;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.Views.Editor;
using LibraryProjectUWP.ViewModels.Publishers;
using System.Collections.ObjectModel;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookCollectionPage : Page
    {
        public BookCollectionPageVM ViewModelPage { get; set; } = new BookCollectionPageVM();
        private LibraryToBookNavigationDriverVM _parameters;
        public BookCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryToBookNavigationDriverVM parameters)
            {
                _parameters = parameters;
                ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void ReloadDataXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await LoadDataAsync();
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

        private async Task LoadDataAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var bookList = await DbServices.Book.AllVMAsync();
                ViewModelPage.ViewModelList = bookList?.ToList() ?? new List<LivreVM>(); ;
                await InitializeDataAsync();
            }
            catch (Exception ex)
            {
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
                    EsBook esBook = new EsBook();
                    foreach (var book in ViewModelPage.ViewModelList)
                    {
                        string combinedPath = await esBook.GetBookItemJaquettePathAsync(book);
                        book.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    }
                }

                ViewModelPage.SearchingLibraryVisibility = Visibility.Collapsed;
                NavigateToView(typeof(BookCollectionGdViewPage), new BookCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
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
                if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                {
                    NavigateToView(typeof(BookCollectionGdViewPage), new BookCollectionParentChildParamsVM()
                    {
                        ParentPage = this,
                        ViewModelList = ViewModelPage.ViewModelList,
                    });
                }

                this.ViewModelPage.SelectedItems = new List<LivreVM>();
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
                if (FramePartialView.Content is BookCollectionGdViewPage)
                {
                    NavigateToView(typeof(BookCollectionDgViewPage), new BookCollectionParentChildParamsVM()
                    {
                        ParentPage = this,
                        ViewModelList = ViewModelPage.ViewModelList,
                    });
                }

                this.ViewModelPage.SelectedItems = new List<LivreVM>();
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

        public void GroupItemsByNone()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    if (FramePartialView.Content is BookCollectionGdViewPage BookCollectionGdViewPage)
                    {
                        BookCollectionGdViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
                    else if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                    {
                        BookCollectionDgViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
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
        public void GroupItemsByAlphabetic()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    if (FramePartialView.Content is BookCollectionGdViewPage BookCollectionGdViewPage)
                    {
                        BookCollectionGdViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
                    else if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                    {
                        BookCollectionDgViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
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

        public void GroupByCreationYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    if (FramePartialView.Content is BookCollectionGdViewPage BookCollectionGdViewPage)
                    {
                        BookCollectionGdViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
                    else if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                    {
                        BookCollectionDgViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
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

        public void GroupByParutionYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s =>  s.DateParution?.Year.ToString() ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    if (FramePartialView.Content is BookCollectionGdViewPage BookCollectionGdViewPage)
                    {
                        BookCollectionGdViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
                    else if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                    {
                        BookCollectionDgViewPage.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    }
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

        public void RefreshItemsGrouping()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ViewModelPage.GroupedBy)
                {
                    case BookGroupVM.GroupBy.None:
                        this.GroupItemsByNone();
                        break;
                    case BookGroupVM.GroupBy.Letter:
                        this.GroupItemsByAlphabetic();
                        break;
                    case BookGroupVM.GroupBy.CreationYear:
                        this.GroupByCreationYear();
                        break;
                    case BookGroupVM.GroupBy.ParutionYear:
                        this.GroupByParutionYear();
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

        #region Search
        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var FilteredItems = new List<LivreVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.ViewModelList)
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

        private void SearchViewModel(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                if (FramePartialView.Content is BookCollectionGdViewPage BookCollectionGdViewPage)
                {
                    BookCollectionGdViewPage.SearchViewModel(viewModel);
                }
                else if (FramePartialView.Content is BookCollectionDgViewPage BookCollectionDgViewPage)
                {
                    //BookCollectionDgViewPage.SearchViewModel(viewModel);
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

        private void Lv_SelectedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Btn_SelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_UnSelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_OpenAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayCategorieListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(BookCategorieUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    BookCategorieUC userControl = new BookCategorieUC(new BookCategorieParametersDriverVM()
                    {
                        ParentPage = this,
                        ParentLibrary = ViewModelPage.ParentLibrary,
                    });


                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

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
                        ViewModelList = ViewModelPage.ViewModelList,
                        CurrentViewModel = new LivreVM()
                        {

                        }
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditBookUC_CreateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditBookUC_CreateItemRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    LivreVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Book.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        ViewModelPage.ViewModelList.Add(newViewModel);
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                        return;
                    }
                }

                //sender.CancelModificationRequested -= NewEditContactUC_CancelModificationRequested;
                //sender.CreateItemRequested -= NewEditContactUC_CreateItemRequested;

                //if (FramePartialView.Content is BookCollectionGdViewPage bookCollectionGdViewPage)
                //{
                //    bookCollectionGdViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionGdViewPage.ViewModelPage.SplitViewContent = null;
                //}
                //else if (FramePartialView.Content is BookCollectionDgViewPage bookCollectionDgViewPage)
                //{
                //    bookCollectionDgViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionDgViewPage.ViewModelPage.SplitViewContent = null;
                //}
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditBookUC_CancelModificationRequested(NewEditBookUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditBookUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditBookUC_CreateItemRequested;

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
                        ViewModelList = ViewModelPage.ViewModelList,
                        CurrentViewModel = viewModel,
                    });

                    userControl.CancelModificationRequested += NewEditBookUC_Edit_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditBookUC_Edit_UpdateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
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

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
                        sender._parameters.CurrentViewModel.MainTitle = updatedViewModel.MainTitle;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditBookUC_Edit_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditBookUC_Edit_UpdateItemRequested;

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
                if (ViewModelPage.ViewModelList != null)
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
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(ViewModelPage.ViewModelList, savedFile);// savedFile.Path
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
        private async void NewContactXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(NewEditContactUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var contactsList = await DbServices.Contact.AllVMAsync();
                    ViewModelPage.ContactViewModelList = contactsList?.ToList();
                    NewEditContactUC userControl = new NewEditContactUC(new ManageContactParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ViewModelList = ViewModelPage.ContactViewModelList,
                        CurrentViewModel = new ContactVM()
                        {
                            TitreCivilite = CivilityHelpers.MPoint,
                        }
                    });

                    userControl.CancelModificationRequested += NewEditContactUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditContactUC_CreateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
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
                        ViewModelPage.ContactViewModelList.Add(newViewModel);
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                        return;
                    }
                }

                //sender.CancelModificationRequested -= NewEditContactUC_CancelModificationRequested;
                //sender.CreateItemRequested -= NewEditContactUC_CreateItemRequested;

                //if (FramePartialView.Content is BookCollectionGdViewPage bookCollectionGdViewPage)
                //{
                //    bookCollectionGdViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionGdViewPage.ViewModelPage.SplitViewContent = null;
                //}
                //else if (FramePartialView.Content is BookCollectionDgViewPage bookCollectionDgViewPage)
                //{
                //    bookCollectionDgViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionDgViewPage.ViewModelPage.SplitViewContent = null;
                //}
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

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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


                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
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
            await this.NewAuthorAsync();
        }

        internal async Task NewAuthorAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(NewEditAuthorUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var authorsList = await DbServices.Author.AllVMAsync();
                    ViewModelPage.AuthorViewModelList = authorsList?.ToList();
                    NewEditAuthorUC userControl = new NewEditAuthorUC(new ManageAuthorParametersDriverVM()
                    {
                        BookParentPage = this,
                        EditMode = Code.EditMode.Create,
                        ViewModelList = ViewModelPage.AuthorViewModelList,
                        CurrentViewModel = new AuthorVM()
                        {
                            TitreCivilite = CivilityHelpers.MPoint,
                        }
                    });

                    userControl.CancelModificationRequested += NewEditAuthorUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditAuthorUC_CreateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditAuthorUC_CreateItemRequested(NewEditAuthorUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    AuthorVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Author.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        //ViewModelPage.AuthorViewModelList.Add(newViewModel);
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                        return;
                    }
                }

                //sender.CancelModificationRequested -= NewEditContactUC_CancelModificationRequested;
                //sender.CreateItemRequested -= NewEditContactUC_CreateItemRequested;

                //if (FramePartialView.Content is BookCollectionGdViewPage bookCollectionGdViewPage)
                //{
                //    bookCollectionGdViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionGdViewPage.ViewModelPage.SplitViewContent = null;
                //}
                //else if (FramePartialView.Content is BookCollectionDgViewPage bookCollectionDgViewPage)
                //{
                //    bookCollectionDgViewPage.ViewModelPage.IsSplitViewOpen = false;
                //    bookCollectionDgViewPage.ViewModelPage.SplitViewContent = null;
                //}
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditAuthorUC_CancelModificationRequested(NewEditAuthorUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DisplayAuthorListXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }


        #endregion

        #region Collection
        private async void NewCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await NewCollectionAsync();
        }

        internal async Task NewCollectionAsync()
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
                        CurrentViewModel = new CollectionVM()
                    });

                    userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditCollectionUC_Create_CreateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
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

                    var creationResult = await DbServices.Collection.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
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

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
                    IList<CollectionVM> itemList = await DbServices.Collection.AllVMAsync();
                    CollectionListUC userControl = new CollectionListUC(new CollectionListParametersDriverVM()
                    {
                        ViewModelList = itemList?.ToList(), //ViewModelPage.ContactViewModelList,
                    });

                    userControl.CancelModificationRequested += CollectionListUC_CancelModificationRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
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

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
            await NewEditorAsync();
        }

        internal async Task NewEditorAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditEditorUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Editors.AllVMAsync();
                    NewEditEditorUC userControl = new NewEditEditorUC(new ManageEditorParametersDriverVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ViewModelList = itemList,
                        CurrentViewModel = new PublisherVM()
                    });

                    userControl.CancelModificationRequested += NewEditEditorUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditEditorUC_Create_CreateItemRequested;

                    this.PivotRightSideBar.Items.Add(userControl);
                    this.PivotRightSideBar.SelectedItem = userControl;
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditEditorUC_Create_CreateItemRequested(NewEditEditorUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    PublisherVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Editors.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                        return;
                    }
                }

                sender.ViewModelPage.ViewModel = new PublisherVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditEditorUC_Create_CancelModificationRequested(NewEditEditorUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditEditorUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditEditorUC_Create_CreateItemRequested;

                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }
                this.PivotRightSideBar.Items.Remove(sender);
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
    }

    public class BookCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        private List<AuthorVM> _AuthorViewModelList;
        public List<AuthorVM> AuthorViewModelList
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
