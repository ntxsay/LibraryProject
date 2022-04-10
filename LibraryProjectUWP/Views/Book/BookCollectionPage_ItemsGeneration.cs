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

        public void GroupItemsBySearch(IList<LivreVM> viewModelList, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.GroupItemsBySearch(viewModelList, goToPage, resetPage);
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.GroupItemsByNone(viewModelList, goToPage, resetPage);
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.GroupItemsByAlphabetic(viewModelList, goToPage, resetPage);
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.GroupByCreationYear(viewModelList, goToPage, resetPage);
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.GroupByParutionYear(viewModelList, goToPage, resetPage);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    bookCollectionSpage.RefreshItemsGrouping(viewModelList, goToPage, resetPage);
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
