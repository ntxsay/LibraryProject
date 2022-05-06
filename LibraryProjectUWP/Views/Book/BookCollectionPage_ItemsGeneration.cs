﻿using LibraryProjectUWP.Code;
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

        public void GroupItemsBy(string busyLoaderMessage, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            try
            {
                this.GenerateItemsWithBusyLoader(busyLoaderMessage, goToPage, resetPage, searchParams);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void OrderItemsBy(BookGroupVM.OrderBy orderBy, string busyLoaderMessage, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            try
            {
                this.ViewModelPage.OrderedBy = orderBy;
                this.GenerateItemsWithBusyLoader(busyLoaderMessage, goToPage, resetPage, searchParams);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SortItemsBy(BookGroupVM.SortBy sortBy, string busyLoaderMessage, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            try
            {
                this.ViewModelPage.SortedBy = sortBy;
                this.GenerateItemsWithBusyLoader(busyLoaderMessage, goToPage, resetPage, searchParams);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GenerateItemsWithBusyLoader(string busyLoaderMessage, int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            try
            {
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
                    await this.RefreshItemsGrouping(goToPage, resetPage, searchParams);

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

        public async Task RefreshItemsGrouping(int goToPage = 1, bool resetPage = true, ResearchBookVM searchParams = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var libraryCollectionSpage = this.LibraryCollectionSubPage;
                if (libraryCollectionSpage != null)
                {
                    await libraryCollectionSpage.CommonView.RefreshItemsGrouping(goToPage, resetPage, searchParams);
                }
                else
                {
                    var bookCollectionSpage = this.BookCollectionSubPage;
                    if (bookCollectionSpage != null)
                    {
                        await bookCollectionSpage.CommonView.RefreshItemsGrouping(goToPage, resetPage, searchParams);
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
