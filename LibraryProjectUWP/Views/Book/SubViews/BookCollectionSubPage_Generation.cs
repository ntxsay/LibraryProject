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

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookCollectionSubPage
    {
        public async Task GroupItemsBySearch(int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage._parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id), 
                    ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (filteredViewModelList == null || !filteredViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = itemsPage.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }

                if (resetPage)
                {
                    var pagesList = DbServices.Book.InitializePages(filteredViewModelList.Count(), ParentPage.ViewModelPage.MaxItemsPerPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                filteredViewModelList = Enumerable.Empty<Tbook>();
                itemsPage = Enumerable.Empty<LivreVM>();
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByNone(int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage._parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id), 
                    ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (filteredViewModelList == null || !filteredViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = itemsPage.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection.Clear();
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }

                if (resetPage)
                {
                    var pagesList = DbServices.Book.InitializePages(filteredViewModelList.Count(), ParentPage.ViewModelPage.MaxItemsPerPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                filteredViewModelList = Enumerable.Empty<Tbook>();
                itemsPage = Enumerable.Empty<LivreVM>();
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByAlphabetic(int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage._parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                    ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (filteredViewModelList == null || !filteredViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = itemsPage.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                }

                if (resetPage)
                {
                    var pagesList = DbServices.Book.InitializePages(filteredViewModelList.Count(), ParentPage.ViewModelPage.MaxItemsPerPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                filteredViewModelList = Enumerable.Empty<Tbook>();
                itemsPage = Enumerable.Empty<LivreVM>();
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByCreationYear(int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage._parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                    ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (filteredViewModelList == null || !filteredViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = itemsPage.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                }

                if (resetPage)
                {
                    var pagesList = DbServices.Book.InitializePages(filteredViewModelList.Count(), ParentPage.ViewModelPage.MaxItemsPerPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                filteredViewModelList = Enumerable.Empty<Tbook>();
                itemsPage = Enumerable.Empty<LivreVM>();
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupByParutionYear(int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var filteredViewModelList = await DbServices.Book.FilterBooksAsync(ParentPage._parameters.ParentLibrary.Id, ParentPage.ViewModelPage.SelectedCollections?.Select(s => s.Id),
                    ParentPage.ViewModelPage.DisplayUnCategorizedBooks, ParentPage.ViewModelPage.SelectedSCategories, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy);
                if (filteredViewModelList == null || !filteredViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = DbServices.Book.GetPaginatedItemsVm(filteredViewModelList, ParentPage.ViewModelPage.MaxItemsPerPage, goToPage);

                IEnumerable<IGrouping<string, LivreVM>> GroupingItems = itemsPage.Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Publication.YearParution ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.ParutionYear;
                }

                if (resetPage)
                {
                    var pagesList = DbServices.Book.InitializePages(filteredViewModelList.Count(), ParentPage.ViewModelPage.MaxItemsPerPage);
                    ViewModelPage.PagesList = new ObservableCollection<PageSystemVM>(pagesList);
                }

                filteredViewModelList = Enumerable.Empty<Tbook>();
                itemsPage = Enumerable.Empty<LivreVM>();
                GroupingItems = Enumerable.Empty<IGrouping<string, LivreVM>>();
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

        public async Task RefreshItemsGrouping(int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ParentPage.ViewModelPage.GroupedBy)
                {
                    case BookGroupVM.GroupBy.None:
                        await this.GroupItemsByNone(goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Letter:
                        await this.GroupItemsByAlphabetic(goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.CreationYear:
                        await this.GroupByCreationYear(goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.ParutionYear:
                        await this.GroupByParutionYear(goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Search:
                        await this.GroupItemsBySearch(goToPage, resetPage);
                        break;
                    default:
                        await this.GroupItemsByNone(goToPage, resetPage);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }




    }
}
