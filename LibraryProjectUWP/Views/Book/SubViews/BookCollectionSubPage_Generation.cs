using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookCollectionSubPage
    {
        public async Task GroupItemsBySearch(ResearchBookVM searchParams, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var item = await this.GenerateResearchBookGroupItemAsync(searchParams, goToPage);
                if (item == null)
                {
                    if (ViewModelPage.PagesList != null && ViewModelPage.PagesList.Any())
                        ViewModelPage.PagesList.Clear();
                    if (this.ViewModelPage.GroupedRelatedViewModel != null && this.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                    return;
                }

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Résultat de la recherche").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Search;
                }

                if (resetPage || ViewModelPage.PagesList == null || !ViewModelPage.PagesList.Any())
                {
                    var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                item = null;
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByNone(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync(reloadFromDb, goToPage);
                if (item == null)
                {
                    if (ViewModelPage.PagesList != null && ViewModelPage.PagesList.Any())
                        ViewModelPage.PagesList.Clear();
                    if (this.ViewModelPage.GroupedRelatedViewModel != null && this.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                    return;
                }

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }

                if (resetPage || ViewModelPage.PagesList == null || !ViewModelPage.PagesList.Any())
                {
                    var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                item = null;
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByAlphabeticAsync(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync(reloadFromDb, goToPage);
                if (item == null)
                {
                    if (ViewModelPage.PagesList != null && ViewModelPage.PagesList.Any())
                        ViewModelPage.PagesList.Clear();
                    if (this.ViewModelPage.GroupedRelatedViewModel != null && this.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                    return;
                }

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                }

                if (resetPage || ViewModelPage.PagesList == null || !ViewModelPage.PagesList.Any())
                {
                    var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                item = null;
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByCreationYear(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync(reloadFromDb, goToPage);
                if (item == null)
                {
                    if (ViewModelPage.PagesList != null && ViewModelPage.PagesList.Any())
                        ViewModelPage.PagesList.Clear();
                    if (this.ViewModelPage.GroupedRelatedViewModel != null && this.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                    return;
                }

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                }

                if (resetPage || ViewModelPage.PagesList == null || !ViewModelPage.PagesList.Any())
                {
                    var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                item = null;
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByParutionYear(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var item = await this.GenerateBookGroupItemAsync(reloadFromDb, goToPage);
                if (item == null)
                {
                    if (ViewModelPage.PagesList != null && ViewModelPage.PagesList.Any())
                        ViewModelPage.PagesList.Clear();
                    if (this.ViewModelPage.GroupedRelatedViewModel != null && this.ViewModelPage.GroupedRelatedViewModel.Collection != null
                        && this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                    {
                        this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    }
                    return;
                }

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = item.ViewModelList.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Publication.YearParution ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.ParutionYear;
                }

                if (resetPage || ViewModelPage.PagesList == null || !ViewModelPage.PagesList.Any())
                {
                    var pagesList = DbServices.Book.InitializePages(item.CountItems, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                item = null;
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task<BookGroupItemVM> GenerateResearchBookGroupItemAsync(ResearchBookVM searchParams, int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems;
                IEnumerable<LivreVM> itemsPage = null;
                var searchedBooks = await DbServices.Book.SearchBooksAsync(searchParams);
                if (searchedBooks == null || !searchedBooks.Any())
                {
                    return null;
                }

                var orderModelList = DbServices.Book.OrderBooks(searchedBooks, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (orderModelList == null || !orderModelList.Any())
                {
                    return null;
                }

                var filterViewModelList = await DbServices.Book.FilterBooksAsync(orderModelList, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories);
                if (filterViewModelList == null || !filterViewModelList.Any())
                {
                    return null;
                }

                countItems = filterViewModelList.Count();
                itemsPage = DbServices.Book.GetPaginatedItemsVm(filterViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                filterViewModelList = Enumerable.Empty<Tbook>();

                itemsPage = await this.CompleteBooksInfoAsync(itemsPage);
                return new BookGroupItemVM()
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

        private async Task<BookGroupItemVM> GenerateBookGroupItemAsync(bool reloadFromDb = true, int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                int countItems;
                IEnumerable<LivreVM> itemsPage = null;
                if (reloadFromDb || this.ViewModelPage.GroupedRelatedViewModel.Collection == null || !this.ViewModelPage.GroupedRelatedViewModel.Collection.Any())
                {
                    var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage.Parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                                                ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                    if (filteredViewModelList == null || !filteredViewModelList.Any())
                    {
                        return null;
                    }

                    countItems = filteredViewModelList.Count();
                    itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    filteredViewModelList = Enumerable.Empty<Tbook>();
                }
                else
                {
                    List<LivreVM> localViewModelList = this.ViewModelPage.GroupedRelatedViewModel.Collection.SelectMany(s => s.ToList()).Select(q => q).ToList();
                    var orderedLocalViewModelList = this.OrderItems(localViewModelList, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                    itemsPage = DbServices.Book.GetPaginatedItems(orderedLocalViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);
                    countItems = (int)await DbServices.Book.CountBooksInLibraryAsync(ParentPage.Parameters.ParentLibrary.Id);
                    localViewModelList.Clear();
                    orderedLocalViewModelList = Enumerable.Empty<LivreVM>();
                }

                itemsPage = await this.CompleteBooksInfoAsync(itemsPage);
                ParentPage.ViewModelPage.NbElementDisplayed = itemsPage.Count();
                
                return new BookGroupItemVM()
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


        public async Task RefreshItemsGrouping(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ParentPage.ViewModelPage.GroupedBy)
                {
                    case BookGroupVM.GroupBy.None:
                        await this.GroupItemsByNone(reloadFromDb, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Letter:
                        await this.GroupItemsByAlphabeticAsync(reloadFromDb, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.CreationYear:
                        await this.GroupByCreationYear(reloadFromDb, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.ParutionYear:
                        await this.GroupByParutionYear(reloadFromDb, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Search:
                        await this.GroupItemsBySearch(searchParams, goToPage, resetPage);
                        break;
                    default:
                        await this.GroupItemsByNone(reloadFromDb, goToPage, resetPage);
                        break;
                }

                ParentPage.ViewModelPage.NbBooks = (int)await DbServices.Book.CountBooksInLibraryAsync(ParentPage.Parameters.ParentLibrary.Id);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }




    }
}
