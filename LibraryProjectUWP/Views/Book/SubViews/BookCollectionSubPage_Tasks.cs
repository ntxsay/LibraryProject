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

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookCollectionSubPage
    {
        private BackgroundWorker workerGotoPage;

        #region Complete Info Book
        public void InitializeGotoPageWorker(int page)
        {
            try
            {
                ParentPage.Parameters.MainPage.OpenLoading();

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
                        workerGotoPage.RunWorkerAsync(page);
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

                ParentPage.Parameters.MainPage.CloseLoading();
                workerGotoPage.DoWork -= WorkerGotoPage_DoWork;
                workerGotoPage.RunWorkerCompleted -= WorkerGotoPage_RunWorkerCompleted;
                workerGotoPage.Dispose();
                workerGotoPage = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

    }
}
