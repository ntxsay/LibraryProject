using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Book.SubViews;
using LibraryProjectUWP.Views.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.UI
{
    public  class CommonView
    {
        //readonly MainPage ParentPage;
        readonly BookCollectionPage ParentPage;
        readonly BookCollectionSubPage BookCollectionSubView;
        readonly LibraryCollectionSubPage LibraryCollectionSubView;
        readonly EsBook esBook = new EsBook();
        public CommonView(BookCollectionPage parentPage, BookCollectionSubPage bookCollectionSubView)
        {
            ParentPage = parentPage;
            BookCollectionSubView = bookCollectionSubView;
        }

        public CommonView(BookCollectionPage parentPage, LibraryCollectionSubPage libraryCollectionSubView)
        {
            ParentPage = parentPage;
            LibraryCollectionSubView = libraryCollectionSubView;
        }

        public async Task GroupItemsByNone<T>(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null) where T : class
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync<T>(goToPage, searchParams);
                if (item == null)
                {
                    ClearPageList();
                    return;
                }

                if (typeof(T).IsAssignableFrom(typeof(BibliothequeVM)) && LibraryCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, BibliothequeVM>> GroupingItems = item.ViewModelList.Select(q => (BibliothequeVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                        LibraryCollectionSubView.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.None;
                    }

                    if (resetPage || LibraryCollectionSubView.ViewModelPage.PagesList == null || !LibraryCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        LibraryCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, BibliothequeVM>>();
                }
                else if (typeof(T).IsAssignableFrom(typeof(LivreVM)) && BookCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Select(q => (LivreVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                        BookCollectionSubView.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                    }

                    if (resetPage || BookCollectionSubView.ViewModelPage.PagesList == null || !BookCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        BookCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
                }

                item = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByAlphabeticAsync<T>(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null) where T : class
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync<T>(goToPage, searchParams);
                if (item == null)
                {
                    ClearPageList();
                    return;
                }

                if (typeof(T).IsAssignableFrom(typeof(BibliothequeVM)) && LibraryCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, BibliothequeVM>> GroupingItems = item.ViewModelList.Select(q => (BibliothequeVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.Name?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                        LibraryCollectionSubView.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                    }

                    if (resetPage || LibraryCollectionSubView.ViewModelPage.PagesList == null || !LibraryCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        LibraryCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, BibliothequeVM>>();
                }
                else if (typeof(T).IsAssignableFrom(typeof(LivreVM)) && BookCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Select(q => (LivreVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                        BookCollectionSubView.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                    }

                    if (resetPage || BookCollectionSubView.ViewModelPage.PagesList == null || !BookCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        BookCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
                }

                item = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByCreationYear<T>(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null) where T : class
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync<T>(goToPage, searchParams);
                if (item == null)
                {
                    ClearPageList();
                    return;
                }

                if (typeof(T).IsAssignableFrom(typeof(BibliothequeVM)) && LibraryCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, BibliothequeVM>> GroupingItems = item.ViewModelList.Select(q => (BibliothequeVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                        LibraryCollectionSubView.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
                    }

                    if (resetPage || LibraryCollectionSubView.ViewModelPage.PagesList == null || !LibraryCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        LibraryCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, BibliothequeVM>>();
                }
                else if (typeof(T).IsAssignableFrom(typeof(LivreVM)) && BookCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Select(q => (LivreVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                        BookCollectionSubView.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                    }

                    if (resetPage || BookCollectionSubView.ViewModelPage.PagesList == null || !BookCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        BookCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
                }

                item = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByParutionYear<T>(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null) where T : class
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync<T>(goToPage, searchParams);
                if (item == null)
                {
                    ClearPageList();
                    return;
                }

                if (typeof(T).IsAssignableFrom(typeof(LivreVM)) && BookCollectionSubView != null)
                {
                    IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Select(q => (LivreVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.Publication.YearParution ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                    if (GroupingItems != null && GroupingItems.Any())
                    {
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                        BookCollectionSubView.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                    }

                    if (resetPage || BookCollectionSubView.ViewModelPage.PagesList == null || !BookCollectionSubView.ViewModelPage.PagesList.Any())
                    {
                        var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                        BookCollectionSubView.ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                    }

                    GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
                }

                item = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task<CommonGroupItemVM<T>> GenerateBookGroupItemAsync<T>(int goToPage = 1, ResearchBookVM searchParams = null) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (searchParams != null)
                {
                    return await this.GenerateResearchBookGroupItemAsync<T>(searchParams, goToPage);
                }
                else
                {
                    return await this.GenerateBookGroupItemAsync<T>(goToPage);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private async Task<CommonGroupItemVM<T>> GenerateResearchBookGroupItemAsync<T>(ResearchBookVM searchParams, int goToPage = 1) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems = 0;
                IEnumerable<T> itemsPage = null;
                var searchedBooks = await DbServices.Library.SearchAsync<T>(searchParams);
                if (searchedBooks == null || !searchedBooks.Any())
                {
                    ParentPage.ViewModelPage.NbElementDisplayed = 0;
                    return null;
                }

                byte orderedBy = 0;
                byte sortedBy = 0;

                if (typeof(T).IsAssignableFrom(typeof(Tlibrary)) && LibraryCollectionSubView != null)
                {
                    orderedBy = (byte)LibraryCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)LibraryCollectionSubView.ViewModelPage.SortedBy;
                }
                else if (typeof(T).IsAssignableFrom(typeof(Tbook)) && BookCollectionSubView != null)
                {
                    orderedBy = (byte)BookCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)BookCollectionSubView.ViewModelPage.SortedBy;
                }

                var orderModelList = DbServices.Library.OrderLibraries(searchedBooks, orderedBy, sortedBy);
                if (orderModelList == null || !orderModelList.Any())
                {
                    ParentPage.ViewModelPage.NbElementDisplayed = 0;
                    return null;
                }

                if (typeof(T).IsAssignableFrom(typeof(Tbook)))
                {
                    var filterViewModelList = await DbServices.Book.FilterBooksAsync(orderModelList.Select(s => (Tbook)(object)s), ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories);
                    if (filterViewModelList == null || !filterViewModelList.Any())
                    {
                        ParentPage.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = filterViewModelList.Count();
                    var results = DbServices.Book.GetPaginatedItemsVm(filterViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    results = await this.CompleteBooksInfoAsync(results);
                    itemsPage = results.Select(s => (T)(object)s);
                    filterViewModelList = Enumerable.Empty<Tbook>();

                }
                else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                {
                    countItems = orderModelList.Count();
                    itemsPage = DbServices.Library.GetPaginatedItemsVm(orderModelList.Select(s => (Tlibrary)(object)s), ParentPage.ViewModelPage.MaxItemsPerPage, goToPage).Select(s => (T)(object)s);
                    orderModelList = Enumerable.Empty<T>();
                }

                ParentPage.ViewModelPage.NbElementDisplayed = itemsPage.Count();

                return new CommonGroupItemVM<T>()
                {
                    CountItems = countItems,
                    ViewModelList = itemsPage,
                };
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private async Task<CommonGroupItemVM<T>> GenerateBookGroupItemAsync<T>(int goToPage = 1) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems = 0;
                IEnumerable<T> itemsPage = null;

                long? idLibrary = null;
                byte orderedBy = 0;
                byte sortedBy = 0;

                if (typeof(T).IsAssignableFrom(typeof(Tbook)) || typeof(T).IsAssignableFrom(typeof(LivreVM)) && BookCollectionSubView != null)
                {
                    idLibrary = BookCollectionSubView.IdLibrary;
                    orderedBy = (byte)BookCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)BookCollectionSubView.ViewModelPage.SortedBy;
                }
                else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)) && LibraryCollectionSubView != null)
                {
                    orderedBy = (byte)LibraryCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)LibraryCollectionSubView.ViewModelPage.SortedBy;
                }

                var orderModelList = await DbServices.Library.OrderLibrariesAsync<T>(orderedBy, sortedBy);
                if (orderModelList == null || !orderModelList.Any())
                {
                    ParentPage.ViewModelPage.NbElementDisplayed = 0;
                    return null;
                }

                if (typeof(T).IsAssignableFrom(typeof(Tbook)))
                {
                    var filterViewModelList = await DbServices.Book.FilterBooksAsync(orderModelList.Select(s => (Tbook)(object)s), ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories);
                    if (filterViewModelList == null || !filterViewModelList.Any())
                    {
                        ParentPage.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = filterViewModelList.Count();
                    var results = DbServices.Book.GetPaginatedItemsVm(filterViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    results = await this.CompleteBooksInfoAsync(results);
                    itemsPage = results.Select(s => (T)(object)s);
                    filterViewModelList = Enumerable.Empty<Tbook>();

                }
                else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                {
                    countItems = orderModelList.Count();
                    itemsPage = DbServices.Library.GetPaginatedItemsVm(orderModelList.Select(s => (Tlibrary)(object)s), ParentPage.ViewModelPage.MaxItemsPerPage, goToPage).Select(s => (T)(object)s);
                    orderModelList = Enumerable.Empty<T>();
                }

                ParentPage.ViewModelPage.NbElementDisplayed = itemsPage.Count();

                return new CommonGroupItemVM<T>()
                {
                    CountItems = countItems,
                    ViewModelList = itemsPage,
                };
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        public async Task<IEnumerable<LivreVM>> CompleteBooksInfoAsync(IEnumerable<LivreVM> viewModelList)
        {
            try
            {
                if (viewModelList == null || !viewModelList.Any())
                {
                    return Enumerable.Empty<LivreVM>();
                }

                foreach (var book in viewModelList)
                {
                    long countExemplaries = await DbServices.Book.CountExemplaryInBookAsync(book.Id);
                    book.NbExemplaires = countExemplaries;

                    long countPrets = await DbServices.Book.CountPretInBookAsync(book.Id);
                    book.NbPrets = countPrets;

                    var jaquettes = await esBook.GetBookItemJaquettePathAsync(book);
                    string combinedPath = jaquettes;
                    string jaquetteFile = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : EsGeneral.BookDefaultJaquette;
                    book.JaquettePath = jaquetteFile;
                }

                return viewModelList;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return viewModelList;
            }
        }


        public async Task RefreshItemsGrouping(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (LibraryCollectionSubView != null)
                {
                    switch (LibraryCollectionSubView.ViewModelPage.GroupedBy)
                    {
                        case LibraryGroupVM.GroupBy.None:
                            await this.GroupItemsByNone<Tlibrary>(goToPage, resetPage, searchParams);
                            break;
                        case LibraryGroupVM.GroupBy.Letter:
                            await this.GroupItemsByAlphabeticAsync<Tlibrary>(goToPage, resetPage, searchParams);
                            break;
                        case LibraryGroupVM.GroupBy.CreationYear:
                            await this.GroupByCreationYear<Tlibrary>(goToPage, resetPage, searchParams);
                            break;
                        default:
                            await this.GroupItemsByNone<Tlibrary>(goToPage, resetPage, searchParams);
                            break;
                    }

                    LibraryCollectionSubView.ViewModelPage.NbItems = await DbServices.Library.CountLibrariesAsync();
                }
                else if (BookCollectionSubView != null)
                {
                    switch (BookCollectionSubView.ViewModelPage.GroupedBy)
                    {
                        case BookGroupVM.GroupBy.None:
                            await this.GroupItemsByNone<Tbook>(goToPage, resetPage, searchParams);
                            break;
                        case BookGroupVM.GroupBy.Letter:
                            await this.GroupItemsByAlphabeticAsync<Tbook>(goToPage, resetPage, searchParams);
                            break;
                        case BookGroupVM.GroupBy.CreationYear:
                            await this.GroupByCreationYear<Tbook>(goToPage, resetPage, searchParams);
                            break;
                        case BookGroupVM.GroupBy.ParutionYear:
                            await this.GroupByParutionYear<Tbook>(goToPage, resetPage, searchParams);
                            break;
                        default:
                            await this.GroupItemsByNone<Tbook>(goToPage, resetPage, searchParams);
                            break;
                    }

                    BookCollectionSubView.ViewModelPage.NbItems = (int)await DbServices.Book.CountBooksInLibraryAsync(BookCollectionSubView.IdLibrary);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ClearPageList()
        {
            try
            {
                if (LibraryCollectionSubView != null)
                {
                    if (LibraryCollectionSubView.ViewModelPage.PagesList != null && LibraryCollectionSubView.ViewModelPage.PagesList.Any())
                        LibraryCollectionSubView.ViewModelPage.PagesList.Clear();
                    if (LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel != null && LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                }
                else if (BookCollectionSubView != null)
                {
                    if (BookCollectionSubView.ViewModelPage.PagesList != null && BookCollectionSubView.ViewModelPage.PagesList.Any())
                        BookCollectionSubView.ViewModelPage.PagesList.Clear();
                    if (BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel != null && BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
