using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
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
    public class DeleteManyBooksTask
    {
        public MainPage MainPage { get; private set; }
        private BackgroundWorker WorkerBackground;
        public bool UseBusyLoader { get; set; } = true;
        public bool CloseBusyLoaderAfterFinish { get; set; } = true;
        public bool UseIntervalAfterFinish { get; set; } = true;
        public bool WorkerReportsProgress { get; set; } = true;
        public TimeSpan IntervalAfterFinish { get; set; } = new TimeSpan(0, 0, 0, 1);
        public bool IsWorkerRunning => WorkerBackground != null && WorkerBackground.IsBusy;
        public bool IsWorkerCancelResquested => WorkerBackground != null && WorkerBackground.CancellationPending;

        public delegate void AfterTaskCompletedEventHandler(DeleteManyBooksTask sender, object e);
        public event AfterTaskCompletedEventHandler AfterTaskCompletedRequested;

        public DeleteManyBooksTask(MainPage mainPage)
        {
            MainPage = mainPage;
        }

        public void DisposeWorker()
        {
            try
            {
                if (WorkerBackground != null && !WorkerBackground.IsBusy)
                {
                    WorkerBackground.Dispose();
                    WorkerBackground = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void CancelWorker()
        {
            try
            {
                if (WorkerBackground != null && WorkerBackground.IsBusy)
                {
                    WorkerBackground.CancelAsync();
                    DisposeWorker();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Delete Books
        public void InitializeWorker(IEnumerable<LivreVM> viewModelList)
        {
            try
            {
                if (viewModelList == null || !viewModelList.Any())
                {
                    return;
                }

                if (WorkerBackground == null)
                {
                    WorkerBackground = new BackgroundWorker()
                    {
                        WorkerReportsProgress = WorkerReportsProgress,
                        WorkerSupportsCancellation = true,
                    };

                    WorkerBackground.ProgressChanged += WorkerBackground_ProgressChanged;
                    WorkerBackground.DoWork += WorkerBackground_DoWork;
                    WorkerBackground.RunWorkerCompleted += WorkerBackground_RunWorkerCompleted;
                }

                if (WorkerBackground != null)
                {
                    if (!WorkerBackground.IsBusy)
                    {
                        if (UseBusyLoader)
                        {
                            MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                            {
                                ProgessText = $"Suppression en cours de {viewModelList.Count()} livre(s).",
                                CancelButtonText = "Annuler la suppression",
                                CancelButtonVisibility = Visibility.Visible,
                                CancelButtonCallback = () =>
                                {
                                    if (WorkerBackground.IsBusy)
                                    {
                                        WorkerBackground.CancelAsync();
                                    }
                                },
                                OpenedLoaderCallback = () => WorkerBackground.RunWorkerAsync(viewModelList),
                            });
                        }
                        else
                        {
                            WorkerBackground.RunWorkerAsync(viewModelList);
                        }
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

        private void WorkerBackground_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker && e.Argument is IEnumerable<LivreVM> viewModelList)
                {
                    int ModelCount = viewModelList.Count();
                    double progressPercentage;
                    int count = 0;

                    List<OperationStateVM> workerStates = new List<OperationStateVM>();
                    foreach (var viewModel in viewModelList)
                    {
                        using (Task<OperationStateVM> task = DbServices.Book.DeleteAsync(viewModel.Id))
                        {
                            task.Wait();
                            workerStates.Add(task.Result);

                            if (worker.CancellationPending == true)
                            {
                                e.Result = workerStates.ToArray();
                                e.Cancel = true;
                                return;
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
                                    worker.ReportProgress(ProgressValue, viewModel);
                                    count++;
                                }
                            }
                        }
                    }
                    e.Result = workerStates.ToArray();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }

        private void WorkerBackground_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (UseBusyLoader)
                {
                    if (e.UserState != null && e.UserState is LivreVM viewModel)
                    {
                        //Progress bar/text
                        var busyLoader = MainPage.GetBusyLoader;
                        if (busyLoader != null)
                        {
                            busyLoader.TbcTitle.Text = $"Suppression en cours du livre « {viewModel.MainTitle} ».\n{e.ProgressPercentage} % des livres supprimés.";
                            if (busyLoader.BtnCancel.Visibility != Visibility.Visible)
                                busyLoader.BtnCancel.Visibility = Visibility.Visible;
                        }
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

        private void WorkerBackground_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string message = string.Empty;

                // Si erreur
                if (e.Error != null)
                {
                    message = $"Une erreur s'est produite lors de la suppression de livres.";
                }
                else if (e.Cancelled)
                {
                    message = $"La suppression a été annulée par l'utilisateur.";
                }
                else
                {
                    var viewModelList = e.Result as OperationStateVM[];
                    message = $"{viewModelList?.Count() ?? 0} {((viewModelList?.Count() ?? 0) > 1 ? "livres ont été supprimés" : "livre a été supprimé")}.";
                }

                if (UseBusyLoader)
                {
                    var busyLoader = MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = message;

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
                        AfterTaskCompletedRequested?.Invoke(this, e);
                        if (CloseBusyLoaderAfterFinish && UseBusyLoader)
                        {
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
                        }

                        dispatcherTimer.Stop();
                    };

                    dispatcherTimer.Start();
                }
                else
                {
                    if (CloseBusyLoaderAfterFinish && UseBusyLoader)
                    {
                        MainPage.CloseBusyLoader();
                    }
                    AfterTaskCompletedRequested?.Invoke(this, e);
                }

                WorkerBackground.Dispose();
                WorkerBackground = null;
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
