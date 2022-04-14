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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {

        public void GroupItemsBySearch(ResearchBookVM searchParams, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Recherche en cours des livres...",
                });

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.GroupItemsBySearch(searchParams, goToPage, resetPage);
                    }

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, e) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
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

        public void GroupItemsByNone(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Dégroupement en cours des livres...",
                });

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.GroupItemsByNone(reloadFromDb, goToPage, resetPage);
                    }

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, e) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        dispatcherTimer2.Stop();
                        //dispatcherTimer2 = null;
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                    //dispatcherTimer = null;
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

        public void GroupItemsByAlphabetic(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Groupement en cours des livres par lettre...",
                });

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.GroupItemsByAlphabeticAsync(reloadFromDb, goToPage, resetPage);
                    }

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, e) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        dispatcherTimer2.Stop();
                        //dispatcherTimer2 = null;
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                    //dispatcherTimer = null;
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


        public void GroupByCreationYear(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Groupement en cours des livres par année de création...",
                });

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.GroupByCreationYear(reloadFromDb, goToPage, resetPage);
                    }

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, e) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        dispatcherTimer2.Stop();
                        //dispatcherTimer2 = null;
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                    //dispatcherTimer = null;
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
        public void GroupByParutionYear(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true)
        {
            try
            {
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = $"Groupement en cours des livres par année de parution...",
                });

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += async (t, f) =>
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.GroupByParutionYear(reloadFromDb, goToPage, resetPage);
                    }

                    DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 2),
                    };

                    dispatcherTimer2.Tick += (s, e) =>
                    {
                        Parameters.MainPage.CloseBusyLoader();
                        dispatcherTimer2.Stop();
                        //dispatcherTimer2 = null;
                    };
                    dispatcherTimer2.Start();

                    dispatcherTimer.Stop();
                    //dispatcherTimer = null;
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

        private void TMFI_OrderByCroissant_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ToggleMenuFlyoutItem item)
                {
                    if (this.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.Croissant)
                    {
                        if (!item.IsChecked)
                        {
                            item.IsChecked = true;
                        }
                        return;
                    }

                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Organisation en cours des livres par ordre croissant...",
                    });

                    DispatcherTimer dispatcherTimer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 1),
                    };

                    dispatcherTimer.Tick += async (t, f) =>
                    {
                        ViewModelPage.OrderedBy = BookGroupVM.OrderBy.Croissant;
                        await this.RefreshItemsGrouping();

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
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void TMFI_OrderByDCroissant_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ToggleMenuFlyoutItem item)
                {
                    if (this.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        if (!item.IsChecked)
                        {
                            item.IsChecked = true;
                        }
                        return;
                    }

                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Organisation en cours des livres par ordre décroissant...",
                    });

                    DispatcherTimer dispatcherTimer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 1),
                    };

                    dispatcherTimer.Tick += async (t, f) =>
                    {
                        ViewModelPage.OrderedBy = BookGroupVM.OrderBy.DCroissant;
                        await this.RefreshItemsGrouping();

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
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void TMFI_SortByName_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ToggleMenuFlyoutItem item)
                {
                    if (this.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        if (!item.IsChecked)
                        {
                            item.IsChecked = true;
                        }
                        return;
                    }

                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Organisation en cours des livres par nom...",
                    });

                    DispatcherTimer dispatcherTimer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 1),
                    };

                    dispatcherTimer.Tick += async (t, f) =>
                    {
                        ViewModelPage.SortedBy = BookGroupVM.SortBy.Name;
                        await this.RefreshItemsGrouping();

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
                }
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void TMFI_SortByDateCreation_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ToggleMenuFlyoutItem item)
                {
                    if (this.ViewModelPage.OrderedBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        if (!item.IsChecked)
                        {
                            item.IsChecked = true;
                        }
                        return;
                    }

                    Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                    {
                        ProgessText = $"Organisation en cours des livres par date de création...",
                    });

                    DispatcherTimer dispatcherTimer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 1),
                    };

                    dispatcherTimer.Tick += async (t, f) =>
                    {
                        ViewModelPage.SortedBy = BookGroupVM.SortBy.DateCreation;
                        await this.RefreshItemsGrouping();

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
                }
                await this.RefreshItemsGrouping(false);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task RefreshItemsGrouping(bool reloadFromDb = true, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var bookCollectionSpage = this.BookCollectionSubPage;
                if (bookCollectionSpage != null)
                {
                    await bookCollectionSpage.RefreshItemsGrouping(reloadFromDb, goToPage, resetPage, searchParams);
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
