using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
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
    public class ExportAllBooksTask
    {
        public MainPage MainPage { get; private set; }
        private BackgroundWorker WorkerBackground;
        CancellationTokenSource cancellationTokenSource;
        public bool UseBusyLoader { get; set; } = true;
        public bool CloseBusyLoaderAfterFinish { get; set; } = true;
        public bool UseIntervalAfterFinish { get; set; } = true;
        public bool WorkerReportsProgress { get; set; } = true;
        public TimeSpan IntervalAfterFinish { get; set; } = new TimeSpan(0, 0, 0, 1);
        public bool IsWorkerRunning => WorkerBackground != null && WorkerBackground.IsBusy;
        public bool IsWorkerCancelResquested => WorkerBackground != null && WorkerBackground.CancellationPending;

        public delegate void AfterTaskCompletedEventHandler(ExportAllBooksTask sender, object e);
        public event AfterTaskCompletedEventHandler AfterTaskCompletedRequested;

        public ExportAllBooksTask(MainPage mainPage)
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
                    cancellationTokenSource?.Cancel();
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
        public void InitializeWorker(BibliothequeVM viewModel)
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
                    cancellationTokenSource = new CancellationTokenSource();
                    if (!WorkerBackground.IsBusy)
                    {
                        if (UseBusyLoader)
                        {
                            MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                            {
                                ProgessText = $"Export en cours de l'ensemble des livres de la bibliothèque « {viewModel.Name} ».",
                                CancelButtonText = "Annuler l'export",
                                CancelButtonVisibility = Visibility.Visible,
                                CancelButtonCallback = () =>
                                {
                                    if (WorkerBackground.IsBusy)
                                    {
                                        cancellationTokenSource?.Cancel();
                                        WorkerBackground.CancelAsync();
                                    }
                                },
                                OpenedLoaderCallback = () => WorkerBackground.RunWorkerAsync(viewModel),
                            });
                        }
                        else
                        {
                            WorkerBackground.RunWorkerAsync(viewModel);
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
                if (sender is BackgroundWorker worker && e.Argument is BibliothequeVM viewModel)
                {
                    using (Task<IList<LivreVM>> task = DbServices.Book.GetListOfBooksVmInLibraryAsync(viewModel.Id, cancellationTokenSource.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSource.IsCancellationRequested)
                        {
                            if (!cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Cancel();
                            }

                            e.Result = task.Result?.ToArray();
                            e.Cancel = true;
                            return;
                        }
                        e.Result = task.Result?.ToArray();
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

        private async void WorkerBackground_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                string message = string.Empty;

                // Si erreur
                if (e.Error != null)
                {
                    message = $"Une erreur s'est produite lors de l'export des livres.";
                }
                else if (e.Cancelled)
                {
                    message = $"L'export a été annulé par l'utilisateur.";
                }
                else
                {
                    var viewModelList = e.Result as LivreVM[];
                    message = $"{viewModelList?.Count() ?? 0} {((viewModelList?.Count() ?? 0) > 1 ? "livres ont été exportés" : "livre a été exporté")}.";
                    
                    if (viewModelList != null && viewModelList.Any())
                    {
                        var suggestedFileName = $"Rostalotheque_Livres_All_{DateTime.Now:yyyyMMddHHmmss}";

                        var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                                                        {
                                                            {"JavaScript Object Notation", new List<string>() { ".json" } }
                                                        }, suggestedFileName);
                        if (savedFile == null)
                        {
                            Logs.Log(m, "Le fichier n'a pas pû être créé.");
                            return;
                        }

                        //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                        bool isFileSaved = await Files.Serialization.Json.SerializeAsync(viewModelList, savedFile);// savedFile.Path
                        if (isFileSaved == false)
                        {
                            Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                            return;
                        }
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
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion
    }
}
