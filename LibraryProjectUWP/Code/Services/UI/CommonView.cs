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
        readonly BookCollectionPage ParentPage;
        readonly BookCollectionSubPage BookCollectionSubView;
        readonly LibraryCollectionSubPage LibraryCollectionSubView;
        readonly EsBook esBook = new EsBook();
        readonly EsAppBaseApi esAppBaseApi = new EsAppBaseApi();
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

        public async Task GroupItemsByNone<T>(int goToPage = 1, bool resetPage = true) where T : class
        {
            try
            {
                var item = await this.GenerateGroupItemAsync<T>(goToPage);
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

        public async Task GroupItemsByAlphabeticAsync<T>(int goToPage = 1, bool resetPage = true) where T : class
        {
            try
            {
                var item = await this.GenerateGroupItemAsync<T>(goToPage);
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

        public async Task GroupByCreationYear<T>(int goToPage = 1, bool resetPage = true) where T : class
        {
            try
            {
                var item = await this.GenerateGroupItemAsync<T>(goToPage);
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

        public async Task GroupByParutionYear<T>(int goToPage = 1, bool resetPage = true) where T : class
        {
            try
            {
                var item = await this.GenerateGroupItemAsync<T>(goToPage);
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

        public async Task<CommonGroupItemVM<T>> GenerateGroupItemAsync<T>(int goToPage = 1) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                CommonGroupItemVM<T> result = null;
                if (ParentPage.ViewModelPage.ResearchItems != null && ParentPage.ViewModelPage.ResearchItems.Any())
                {
                    result = await this.GenerateResearchGroupItemAsync<T>(ParentPage.ViewModelPage.ResearchItems, goToPage);
                }
                else
                {
                    result = await this.GenerateGroupItemAsync_<T>(goToPage);
                }

                if (LibraryCollectionSubView != null)
                {
                    LibraryCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    LibraryCollectionSubView.SetViewModeDataTemplate(ParentPage.ViewModelPage.DataViewMode);
                }
                else if (BookCollectionSubView != null)
                {
                    BookCollectionSubView.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    BookCollectionSubView.SetViewModeDataTemplate(ParentPage.ViewModelPage.DataViewMode);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        private async Task<CommonGroupItemVM<T>> GenerateResearchGroupItemAsync<T>(IEnumerable<ResearchItemVM> searchParams, int goToPage = 1) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems = 0;
                IEnumerable<T> itemsPage = null;

                byte orderedBy = 0;
                byte sortedBy = 0;

                if ((typeof(T).IsAssignableFrom(typeof(Tlibrary)) || typeof(T).IsAssignableFrom(typeof(BibliothequeVM))) && LibraryCollectionSubView != null)
                {
                    var searchedItems = await DbServices.Common.SearchAsync<Tlibrary>(searchParams?.ToArray()) ;
                    if (searchedItems == null || !searchedItems.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    orderedBy = (byte)LibraryCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)LibraryCollectionSubView.ViewModelPage.SortedBy;

                    IEnumerable<Tlibrary> orderModelList = DbServices.Common.Order(searchedItems, orderedBy, sortedBy);
                    if (orderModelList == null || !orderModelList.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = orderModelList.Count();
                    itemsPage = DbServices.Library.GetPaginatedItemsVm(orderModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage).Select(s => (T)(object)s);
                    orderModelList = Enumerable.Empty<Tlibrary>();
                }
                else if ((typeof(T).IsAssignableFrom(typeof(Tbook)) || typeof(T).IsAssignableFrom(typeof(LivreVM))) && BookCollectionSubView != null)
                {
                    var searchedItems = await DbServices.Common.SearchAsync<Tbook>(searchParams?.ToArray(), ParentPage.Parameters?.ParentLibrary?.Id);
                    if (searchedItems == null || !searchedItems.Any())
                    {
                        BookCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    orderedBy = (byte)BookCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)BookCollectionSubView.ViewModelPage.SortedBy;

                    IEnumerable<Tbook> orderModelList = DbServices.Common.Order(searchedItems, orderedBy, sortedBy);
                    if (orderModelList == null || !orderModelList.Any())
                    {
                        BookCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    var filterViewModelList = await DbServices.Book.FilterBooksAsync(orderModelList, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories);
                    if (filterViewModelList == null || !filterViewModelList.Any())
                    {
                        BookCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = filterViewModelList.Count();
                    var results = DbServices.Book.GetPaginatedItemsVm(filterViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    results = await this.CompleteBooksInfoAsync(results);
                    itemsPage = results.Select(s => (T)(object)s);
                    filterViewModelList = Enumerable.Empty<Tbook>();
                    orderModelList = Enumerable.Empty<Tbook>();
                }

                if (ParentPage.IsContainsLibraryCollection(out var libraryCollectionSubPage))
                {
                    libraryCollectionSubPage.ViewModelPage.NbElementDisplayed = itemsPage?.Count() ?? 0;
                }
                else if (ParentPage.IsContainsBookCollection(out var bookCollectionSubPage))
                {
                    bookCollectionSubPage.ViewModelPage.NbElementDisplayed = itemsPage?.Count() ?? 0;
                }

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

        private async Task<CommonGroupItemVM<T>> GenerateGroupItemAsync_<T>(int goToPage = 1) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems = 0;
                IEnumerable<T> itemsPage = null;

                byte orderedBy = 0;
                byte sortedBy = 0;

                if ((typeof(T).IsAssignableFrom(typeof(Tbook)) || typeof(T).IsAssignableFrom(typeof(LivreVM))) && BookCollectionSubView != null)
                {
                    orderedBy = (byte)BookCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)BookCollectionSubView.ViewModelPage.SortedBy;

                    IEnumerable<Tbook> orderModelList = await DbServices.Common.OrderAsync<Tbook>(orderedBy, sortedBy, ParentPage.Parameters.ParentLibrary?.Id);
                    if (orderModelList == null || !orderModelList.Any())
                    {
                        BookCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    var filterViewModelList = await DbServices.Book.FilterBooksAsync(orderModelList, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories);
                    if (filterViewModelList == null || !filterViewModelList.Any())
                    {
                        BookCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = filterViewModelList.Count();
                    var results = DbServices.Book.GetPaginatedItemsVm(filterViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    results = await this.CompleteBooksInfoAsync(results);
                    itemsPage = results.Select(s => (T)(object)s);
                    filterViewModelList = Enumerable.Empty<Tbook>();
                    orderModelList = Enumerable.Empty<Tbook>();
                }
                else if ((typeof(T).IsAssignableFrom(typeof(Tlibrary)) || typeof(T).IsAssignableFrom(typeof(BibliothequeVM))) && LibraryCollectionSubView != null)
                {
                    orderedBy = (byte)LibraryCollectionSubView.ViewModelPage.OrderedBy;
                    sortedBy = (byte)LibraryCollectionSubView.ViewModelPage.SortedBy;
                    IEnumerable<Tlibrary> orderModelList = await DbServices.Common.OrderAsync<Tlibrary>(orderedBy, sortedBy);
                    if (orderModelList == null || !orderModelList.Any())
                    {
                        LibraryCollectionSubView.ViewModelPage.NbElementDisplayed = 0;
                        return null;
                    }

                    countItems = orderModelList.Count();
                    itemsPage = DbServices.Library.GetPaginatedItemsVm(orderModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage).Select(s => (T)(object)s).ToList();
                    orderModelList = Enumerable.Empty<Tlibrary>();
                }

                if (ParentPage.IsContainsLibraryCollection(out var libraryCollectionSubPage))
                {
                    libraryCollectionSubPage.ViewModelPage.NbElementDisplayed = itemsPage?.Count() ?? 0;
                }
                else if (ParentPage.IsContainsBookCollection(out var bookCollectionSubPage))
                {
                    bookCollectionSubPage.ViewModelPage.NbElementDisplayed = itemsPage?.Count() ?? 0;
                }

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

                    var jaquettes = await esAppBaseApi.GetJaquettePathAsync<LivreVM>(book.Guid);
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


        public async Task RefreshItemsGrouping(int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (LibraryCollectionSubView != null)
                {
                    switch (LibraryCollectionSubView.ViewModelPage.GroupedBy)
                    {
                        case LibraryGroupVM.GroupBy.None:
                            await this.GroupItemsByNone<BibliothequeVM>(goToPage, resetPage);
                            break;
                        case LibraryGroupVM.GroupBy.Letter:
                            await this.GroupItemsByAlphabeticAsync<BibliothequeVM>(goToPage, resetPage);
                            break;
                        case LibraryGroupVM.GroupBy.CreationYear:
                            await this.GroupByCreationYear<BibliothequeVM>(goToPage, resetPage);
                            break;
                        default:
                            await this.GroupItemsByNone<BibliothequeVM>(goToPage, resetPage);
                            break;
                    }

                    LibraryCollectionSubView.ViewModelPage.NbItems = await DbServices.Library.CountLibrariesAsync();
                }
                else if (BookCollectionSubView != null)
                {
                    switch (BookCollectionSubView.ViewModelPage.GroupedBy)
                    {
                        case BookGroupVM.GroupBy.None:
                            await this.GroupItemsByNone<LivreVM>(goToPage, resetPage);
                            break;
                        case BookGroupVM.GroupBy.Letter:
                            await this.GroupItemsByAlphabeticAsync<LivreVM>(goToPage, resetPage);
                            break;
                        case BookGroupVM.GroupBy.CreationYear:
                            await this.GroupByCreationYear<LivreVM>(goToPage, resetPage);
                            break;
                        case BookGroupVM.GroupBy.ParutionYear:
                            await this.GroupByParutionYear<LivreVM>(goToPage, resetPage);
                            break;
                        default:
                            await this.GroupItemsByNone<LivreVM>(goToPage, resetPage);
                            break;
                    }

                    BookCollectionSubView.ViewModelPage.NbItems = (int)await DbServices.Book.CountBooksInLibraryAsync(ParentPage.Parameters.ParentLibrary.Id);
                }

                if (ParentPage.ViewModelPage.IsUpdateSubView == true)
                {
                    ParentPage.ViewModelPage.IsUpdateSubView = false;
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
