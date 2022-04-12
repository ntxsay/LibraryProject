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

        public async Task GroupItemsBySearch(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GroupItemsBySearch(reloadFromDb, goToPage, resetPage);
                }
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GroupItemsByNone(reloadFromDb, goToPage, resetPage);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GroupItemsByAlphabetic(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GroupItemsByAlphabetic(reloadFromDb, goToPage, resetPage);
                }
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GroupByCreationYear(reloadFromDb, goToPage, resetPage);
                }
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
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.GroupByParutionYear(reloadFromDb, goToPage, resetPage);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void OrderByCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = BookGroupVM.OrderBy.Croissant;
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void OrderByDCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = BookGroupVM.OrderBy.DCroissant;
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void SortByNameXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = BookGroupVM.SortBy.Name;
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void SortByDateCreationXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = BookGroupVM.SortBy.DateCreation;
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task RefreshItemsGrouping(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.RefreshItemsGrouping(reloadFromDb, goToPage, resetPage);
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
    }
}
