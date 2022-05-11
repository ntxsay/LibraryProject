using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
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
    public class GetContactsListTask
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
        
        CancellationTokenSource cancellationTokenSource;

        public delegate void AfterTaskCompletedEventHandler(GetContactsListTask sender, RunWorkerCompletedEventArgs e);
        public event AfterTaskCompletedEventHandler AfterTaskCompletedRequested;

        public GetContactsListTask()
        {
            UseBusyLoader = false;
            CloseBusyLoaderAfterFinish = false;
        }

        public GetContactsListTask(MainPage mainPage)
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

                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
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
                    cancellationTokenSource?.Cancel();
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

        #region
        public void InitializeWorker(ContactType contactType)
        {
            try
            {
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
                        cancellationTokenSource = new CancellationTokenSource();
                        if(UseBusyLoader)
                        {
                            MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                            {
                                ProgessText = $"Recherche en cours de contacts. Veuillez patienter quelques instants.",
                                CancelButtonText = "Annuler la recherche",
                                CancelButtonVisibility = Visibility.Visible,
                                CancelButtonCallback = () =>
                                {
                                    if (WorkerBackground.IsBusy)
                                    {
                                        WorkerBackground.CancelAsync();
                                    }
                                },
                                OpenedLoaderCallback = () => WorkerBackground.RunWorkerAsync(contactType),
                            });
                        }
                        else
                        {
                            WorkerBackground.RunWorkerAsync(contactType);
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
                if (sender is BackgroundWorker worker && e.Argument is ContactType contactType)
                {
                    using (Task<IList<ContactVM>> task = DbServices.Contact.MultipleVMAsync(contactType))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSource.IsCancellationRequested)
                        {
                            if (!cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;
                        var state = new WorkerState<ContactVM, ContactVM>()
                        {
                            ResultList = result,
                        };

                        e.Result = state;
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

        private void WorkerBackground_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (UseBusyLoader)
                {
                    //Progress bar/text
                    var busyLoader = MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des contacts chargés.";
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

        private void WorkerBackground_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string message = string.Empty;

                // Si erreur
                if (e.Error != null)
                {
                    message = $"Une erreur s'est produite lors de la recherche d'exemplaire(s) emprunté(s)";
                }
                else if (e.Cancelled)
                {
                    message = $"La recherche d'exemplaire(s) emprunté(s) a été annulée par l'utilisateur.";
                }
                else
                {
                    if (e.Result is WorkerState<ContactVM, ContactVM> result)
                    {
                        message = $"{result.ResultList?.Count() ?? 0} contact(s) récupérés";
                    }
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
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
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
