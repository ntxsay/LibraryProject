using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
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
        public IEnumerable<LivreVM> PrepareBooks(IList<LivreVM> viewModelList)
        {
            try
            {
                List<LivreVM> PreSelectedViewModelList = new List<LivreVM>();

                if (ViewModelPage.SelectedCollections != null && ViewModelPage.SelectedCollections.Any() || ViewModelPage.DisplayUnCategorizedBooks == true ||
                    ViewModelPage.SelectedSCategories != null && ViewModelPage.SelectedSCategories.Any())
                {
                    List<LivreVM> vms = new List<LivreVM>();
                    foreach (LivreVM viewModel in viewModelList)
                    {
                        if (ViewModelPage.SelectedCollections != null && ViewModelPage.SelectedCollections.Any())
                        {
                            foreach (CollectionVM collectionVM in ViewModelPage.SelectedCollections)
                            {
                                if (viewModel.Publication.Collections.Any(s => s.Id == collectionVM.Id))
                                {
                                    if (!vms.Contains(viewModel))
                                    {
                                        vms.Add(viewModel);
                                    }
                                    break;
                                }
                            }
                        }

                        if (ViewModelPage.DisplayUnCategorizedBooks == false && ViewModelPage.SelectedSCategories != null && ViewModelPage.SelectedSCategories.Any())
                        {
                            foreach (var item in ViewModelPage.SelectedSCategories)
                            {
                                if (item is CategorieLivreVM categorie)
                                {
                                    if (categorie.BooksId.Any(f => f == viewModel.Id))
                                    {
                                        if (!vms.Contains(viewModel))
                                        {
                                            vms.Add(viewModel);
                                        }
                                        break;
                                    }
                                }
                                else if (item is SubCategorieLivreVM subCategorie)
                                {
                                    var result = subCategorie.BooksId.Any(a => a == viewModel.Id);
                                    if (result == true)
                                    {
                                        if (!vms.Contains(viewModel))
                                        {
                                            vms.Add(viewModel);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else if (ViewModelPage.DisplayUnCategorizedBooks == true)
                        {
                            var uncategorizedBooksIdTask = Task.Run(() => DbServices.Categorie.GetUnCategorizedBooksId(ParentPage._parameters.ParentLibrary.Id));
                            uncategorizedBooksIdTask.Wait();
                            var uncategorizedBooksId = uncategorizedBooksIdTask.Result;

                            if (uncategorizedBooksId.Any(f => f == viewModel.Id))
                            {
                                if (!vms.Contains(viewModel))
                                {
                                    vms.Add(viewModel);
                                }
                            }
                        }
                    }

                    if (vms.Count > 0)
                    {
                        PreSelectedViewModelList.AddRange(vms);
                    }

                    vms.Clear();
                    vms = null;
                }

                return PreSelectedViewModelList == null || PreSelectedViewModelList.Count == 0 ? viewModelList : PreSelectedViewModelList.Distinct();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<LivreVM>();
            }
        }

        public void GroupItemsBySearch(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var preparedViewModelList = this.PrepareBooks(viewModelList);
                if (preparedViewModelList == null || !preparedViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(preparedViewModelList.ToList(), goToPage);

                var GroupingItems = this.OrderItems(itemsPage, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Résultat de la recherche").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }

                if (resetPage)
                {
                    this.InitializePages(preparedViewModelList.ToList());
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupItemsByNone(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var preparedViewModelList = this.PrepareBooks(viewModelList);
                if (preparedViewModelList == null || !preparedViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(preparedViewModelList.ToList(), goToPage);

                var GroupingItems = this.OrderItems(itemsPage, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
                }

                if (resetPage)
                {
                    this.InitializePages(preparedViewModelList.ToList());
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupItemsByAlphabetic(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var preparedViewModelList = this.PrepareBooks(viewModelList);
                if (preparedViewModelList == null || !preparedViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(preparedViewModelList.ToList(), goToPage);

                var GroupingItems = this.OrderItems(itemsPage, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
                }

                if (resetPage)
                {
                    this.InitializePages(preparedViewModelList.ToList());
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var preparedViewModelList = this.PrepareBooks(viewModelList);
                if (preparedViewModelList == null || !preparedViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(preparedViewModelList.ToList(), goToPage);

                var GroupingItems = this.OrderItems(itemsPage, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
                }

                if (resetPage)
                {
                    this.InitializePages(preparedViewModelList.ToList());
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByParutionYear(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var preparedViewModelList = this.PrepareBooks(viewModelList);
                if (preparedViewModelList == null || !preparedViewModelList.Any())
                {
                    return;
                }

                IEnumerable<LivreVM> itemsPage = GetPaginatedItems(preparedViewModelList.ToList(), goToPage);


                var GroupingItems = this.OrderItems(itemsPage, ParentPage.ViewModelPage.OrderedBy, ParentPage.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Publication.YearParution ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    ParentPage.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.ParutionYear;
                }

                if (resetPage)
                {
                    this.InitializePages(preparedViewModelList.ToList());
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

        public void RefreshItemsGrouping(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ParentPage.ViewModelPage.GroupedBy)
                {
                    case BookGroupVM.GroupBy.None:
                        this.GroupItemsByNone(viewModelList, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Letter:
                        this.GroupItemsByAlphabetic(viewModelList, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.CreationYear:
                        this.GroupByCreationYear(viewModelList, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.ParutionYear:
                        this.GroupByParutionYear(viewModelList, goToPage, resetPage);
                        break;
                    case BookGroupVM.GroupBy.Search:
                        this.GroupItemsBySearch(viewModelList, goToPage, resetPage);
                        break;
                    default:
                        this.GroupItemsByNone(viewModelList, goToPage, resetPage);
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
