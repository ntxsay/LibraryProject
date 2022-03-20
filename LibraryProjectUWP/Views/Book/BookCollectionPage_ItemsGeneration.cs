using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
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
                            var uncategorizedBooksIdTask = Task.Run(() => DbServices.Categorie.GetUnCategorizedBooksId(_parameters.ParentLibrary.Id));
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

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.None;
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

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.Letter;
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

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.CreationYear;
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


                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Publication.YearParution ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, LivreVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = BookGroupVM.GroupBy.ParutionYear;
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


        private void OrderByCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = BookGroupVM.OrderBy.Croissant;
                this.RefreshItemsGrouping(_parameters.ParentLibrary.Books);
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
                this.RefreshItemsGrouping(_parameters.ParentLibrary.Books);
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
                this.RefreshItemsGrouping(_parameters.ParentLibrary.Books);
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
                this.RefreshItemsGrouping(_parameters.ParentLibrary.Books);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void RefreshItemsGrouping(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ViewModelPage.GroupedBy)
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

        private IEnumerable<LivreVM> GetPaginatedItems(IList<LivreVM> viewModelList, int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                IEnumerable<LivreVM> itemsPage = Enumerable.Empty<LivreVM>();
                //IList<LivreVM> PreSelectedViewModelList = new List<LivreVM>(viewModelList);

                //if (ViewModelPage.SelectedCollections != null && ViewModelPage.SelectedCollections.Any())
                //{
                //    List<LivreVM> vms = new List<LivreVM>();
                //    foreach (LivreVM viewModel in viewModelList)
                //    {
                //        foreach (CollectionVM collectionVM in ViewModelPage.SelectedCollections)
                //        {
                //            if (viewModel.Publication.Collections.Any(s => s.Id == collectionVM.Id))
                //            {
                //                vms.Add(viewModel);
                //                break;
                //            }
                //        }
                //    }

                //    PreSelectedViewModelList = new List<LivreVM>(vms);
                //}

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


        private void InitializePages(IList<LivreVM> viewModelList)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.PagesList.Clear();

                if (viewModelList != null && viewModelList.Any())
                {
                    int nbPageDefault = viewModelList.Count() / ViewModelPage.MaxItemsPerPage;
                    double nbPageExact = viewModelList.Count() / Convert.ToDouble(ViewModelPage.MaxItemsPerPage);
                    int nbPageRounded = nbPageExact > nbPageDefault ? nbPageDefault + 1 : nbPageDefault;
                    ViewModelPage.CountPages = nbPageRounded;

                    for (int i = 0; i < ViewModelPage.CountPages; i++)
                    {
                        ViewModelPage.PagesList.Add(new PageSystemVM()
                        {
                            CurrentPage = i + 1,
                            IsPageSelected = i == 0,
                            BackgroundColor = i == 0 ? Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush : Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush,
                        });
                    }
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
