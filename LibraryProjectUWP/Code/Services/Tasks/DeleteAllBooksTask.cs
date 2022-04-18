using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Code.Services.Tasks
{
    public class DeleteAllBooksTask
    {
        public MainPage MainPage { get; private set; }
        private BackgroundWorker WorkerDeleteBooks;
        public bool UseBusyLoader { get; set; } = true;
        public bool UseIntervalAfterFinish { get; set; } = true;
        public bool WorkerReportsProgress { get; set; } = true;
        public TimeSpan IntervalAfterFinish { get; set; } = new TimeSpan(0, 0, 0, 1);
        public bool IsWorkerRunning => WorkerDeleteBooks != null && WorkerDeleteBooks.IsBusy;
        public bool IsWorkerCancelResquested => WorkerDeleteBooks != null && WorkerDeleteBooks.CancellationPending;

        public delegate void AfterTaskCompletedEventHandler(DeleteAllBooksTask sender, object e);
        public event AfterTaskCompletedEventHandler AfterTaskCompletedRequested;

        public DeleteAllBooksTask(MainPage mainPage)
        {
            MainPage = mainPage;
        }

        #region Delete Books
        public void InitializeDeleteBooksWorker(BibliothequeVM viewModel)
        {
            try
            {
                if (WorkerDeleteBooks == null)
                {
                    WorkerDeleteBooks = new BackgroundWorker()
                    {
                        WorkerReportsProgress = WorkerReportsProgress,
                        WorkerSupportsCancellation = true,
                    };

                    WorkerDeleteBooks.ProgressChanged += WorkerDeleteBooks_ProgressChanged;
                    WorkerDeleteBooks.DoWork += WorkerDeleteBooks_DoWork;
                    WorkerDeleteBooks.RunWorkerCompleted += WorkerDeleteBooks_RunWorkerCompleted;
                }

                if (WorkerDeleteBooks != null)
                {
                    if (!WorkerDeleteBooks.IsBusy)
                    {
                        if (UseBusyLoader)
                        {
                            MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                            {
                                ProgessText = $"Suppression en cours de l'ensemble des livres de la bibliothèque {viewModel.Name}.",
                                CancelButtonText = "Annuler la suppression",
                                CancelButtonVisibility = Visibility.Visible,
                                CancelButtonCallback = () =>
                                {
                                    if (WorkerDeleteBooks.IsBusy)
                                    {
                                        WorkerDeleteBooks.CancelAsync();
                                    }
                                },
                                OpenedLoaderCallback = () => WorkerDeleteBooks.RunWorkerAsync(viewModel),
                            });
                        }
                        else
                        {
                            WorkerDeleteBooks.RunWorkerAsync(viewModel);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerDeleteBooks_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker && e.Argument is BibliothequeVM viewModel)
                {
                    using (Task<IList<long>> task = DbServices.Book.GetListOfIdBooksInLibraryAsync(viewModel.Id))
                    {
                        task.Wait();

                        int ModelCount = task.Result.Count();
                        double progressPercentage;
                        int count = 0;

                        List<OperationStateVM> workerStates = new List<OperationStateVM>();
                        foreach (var idBook in task.Result)
                        {
                            using (Task<OperationStateVM> taskDelete = DbServices.Book.DeleteAsync(idBook))
                            {
                                taskDelete.Wait();
                                workerStates.Add(taskDelete.Result);

                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    break;
                                }
                                else
                                {
                                    if (WorkerReportsProgress)
                                    {
                                        var NumberModel = count + 1;
                                        double Operation = (double)NumberModel / (double)ModelCount;
                                        progressPercentage = Operation * 100;
                                        int ProgressValue = Convert.ToInt32(progressPercentage);

                                        Thread.Sleep(100);
                                        worker.ReportProgress(ProgressValue, null);
                                        count++;
                                    }
                                }
                            }
                        }
                        e.Result = workerStates.ToArray();
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

        private void WorkerDeleteBooks_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (UseBusyLoader)
                {
                    //Progress bar/text
                    var busyLoader = MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des livres supprimés.";
                        if (busyLoader.BtnCancel.Visibility != Visibility.Visible)
                            busyLoader.BtnCancel.Visibility = Visibility.Visible;
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

        private void WorkerDeleteBooks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (UseBusyLoader)
                {
                    var busyLoader = MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        // Si erreur
                        if (e.Error != null)
                        {
                            if (e.Result != null && e.Result is OperationStateVM[] workerUserState)
                            {
                                busyLoader.TbcTitle.Text = $"{workerUserState.Count(w => w.IsSuccess == true)} {(workerUserState.Count(w => w.IsSuccess == true) > 1 ? "livres" : "livre")} sur {workerUserState.Count()} {(workerUserState.Count() > 1 ? "ont été supprimés" : "a été supprimé")}, une erreur s'est produite lors de la suppresion.\nActualisation du catalogue des livres en cours...";
                            }
                            else
                            {
                                busyLoader.TbcTitle.Text = $"Une erreur s'est produite lors de la suppresion.\nActualisation du catalogue des livres en cours...";
                            }
                        }
                        else if (e.Cancelled)
                        {
                            if (e.Result != null && e.Result is OperationStateVM[] workerUserState)
                            {
                                busyLoader.TbcTitle.Text = $"La suppression a été annulée par l'utilisateur. {workerUserState.Count(w => w.IsSuccess == true)} {(workerUserState.Count(w => w.IsSuccess == true) > 1 ? "livres ont été supprimés" : "livre a été supprimé")}.\nActualisation du catalogue des livres en cours...";
                            }
                            else
                            {
                                busyLoader.TbcTitle.Text = $"La suppression a été annulée par l'utilisateur.\nActualisation du catalogue des livres en cours...";
                            }
                        }
                        else
                        {
                            if (e.Result != null && e.Result is OperationStateVM[] workerUserState)
                            {
                                busyLoader.TbcTitle.Text = $"{workerUserState.Count(w => w.IsSuccess == true)} {(workerUserState.Count(w => w.IsSuccess == true) > 1 ? "livres" : "livre")} sur {workerUserState.Count()} {(workerUserState.Count() > 1 ? "ont été supprimés" : "a été supprimé")}.\nActualisation du catalogue des livres en cours...";
                            }
                        }

                        if (busyLoader.BtnCancel.Visibility != Visibility.Collapsed)
                            busyLoader.BtnCancel.Visibility = Visibility.Collapsed;
                    }
                }
                
                if (UseIntervalAfterFinish)
                {
                    DispatcherTimer dispatcherTimer = new DispatcherTimer()
                    {
                        Interval = IntervalAfterFinish,
                    };

                    dispatcherTimer.Tick += (t, f) =>
                    {
                        AfterTaskCompletedRequested?.Invoke(this, null);

                        DispatcherTimer dispatcherTimer2 = new DispatcherTimer()
                        {
                            Interval = new TimeSpan(0, 0, 0, 2),
                        };

                        dispatcherTimer2.Tick += (s, i) =>
                        {
                            MainPage.CloseBusyLoader();
                            dispatcherTimer2.Stop();
                        };
                        dispatcherTimer2.Start();

                        dispatcherTimer.Stop();
                    };

                    dispatcherTimer.Start();
                }
                else
                {
                    AfterTaskCompletedRequested?.Invoke(this, e);
                }

                WorkerDeleteBooks.Dispose();
                WorkerDeleteBooks = null;
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
