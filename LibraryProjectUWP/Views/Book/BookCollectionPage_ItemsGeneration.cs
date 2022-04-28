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

        public void GroupItemsBy(BookGroupVM.GroupBy groupBy, string busyLoaderMessage, bool reloadFromDb = true, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            try
            {
                this.ViewModelPage.GroupedBy = groupBy;
                Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                {
                    ProgessText = busyLoaderMessage,
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
                        await this.RefreshItemsGrouping(reloadFromDb, goToPage, resetPage, searchParams);
                    }

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
    }
}
