using LibraryProjectUWP.Code.Helpers;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookCollectionSubPage
    {
        private BackgroundWorker workerGotoPage;

        #region GoToPage
        public void InitializeGotoPageWorker(int page)
        {
            try
            {
                if (workerGotoPage == null)
                {
                    workerGotoPage = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = false,
                    };

                    workerGotoPage.DoWork += WorkerGotoPage_DoWork;
                    workerGotoPage.RunWorkerCompleted += WorkerGotoPage_RunWorkerCompleted;
                }

                if (workerGotoPage != null)
                {
                    if (!workerGotoPage.IsBusy)
                    {
                        ParentPage.Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Recherche des livres de la page {page}",
                            OpenedLoaderParameter = page,
                            OpenedLoaderCallback = () => workerGotoPage.RunWorkerAsync(page)
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerGotoPage_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker && e.Argument is int page)
                {
                    using (Task task = Task.Run(() => GotoPage(page)))
                    {
                        task.Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void WorkerGotoPage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // Si erreur
                if (e.Error != null)
                {

                }
                else if (e.Cancelled)
                {
                    // Support de l'annulation a été désactivée
                }
                else
                {
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 3),
                };

                dispatcherTimer.Tick += (t, f) =>
                {
                    ParentPage.Parameters.MainPage.CloseBusyLoader();
                    dispatcherTimer.Stop();
                    dispatcherTimer = null;
                };

                dispatcherTimer.Start(); 
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
            finally
            {
                workerGotoPage.DoWork -= WorkerGotoPage_DoWork;
                workerGotoPage.RunWorkerCompleted -= WorkerGotoPage_RunWorkerCompleted;
                workerGotoPage.Dispose();
                workerGotoPage = null;
            }
        }
        #endregion

    }
}
