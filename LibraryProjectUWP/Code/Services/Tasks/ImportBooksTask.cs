﻿using LibraryProjectUWP.Code.Services.Db;
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
    public class ImportBooksTask
    {
        public MainPage MainPage { get; private set; }
        private BackgroundWorker WorkerBackground;
        public long IdLibrary { get; set; }
        public bool UseBusyLoader { get; set; } = true;
        public bool CloseBusyLoaderAfterFinish { get; set; } = true;
        public bool UseIntervalAfterFinish { get; set; } = true;
        public bool WorkerReportsProgress { get; set; } = true;
        public TimeSpan IntervalAfterFinish { get; set; } = new TimeSpan(0, 0, 0, 1);
        public bool IsWorkerRunning => WorkerBackground != null && WorkerBackground.IsBusy;
        public bool IsWorkerCancelResquested => WorkerBackground != null && WorkerBackground.CancellationPending;

        public delegate void AfterTaskCompletedEventHandler(ImportBooksTask sender, object e);
        public event AfterTaskCompletedEventHandler AfterTaskCompletedRequested;

        public ImportBooksTask(MainPage mainPage, long idLibrary)
        {
            MainPage = mainPage;
            IdLibrary = idLibrary;
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

        #region
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
                                ProgessText = $"Importation de {viewModelList.Count()} livre(s) en cours",
                                CancelButtonText = "Annuler l'importation",
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

                    List<WorkerState<OperationStateVM, OperationStateVM>> workerStates = new List<WorkerState<OperationStateVM, OperationStateVM>>();
                    foreach (var newViewModel in viewModelList)
                    {
                        List<WorkerState<OperationStateVM, OperationStateVM>> _subWorkerStates = new List<WorkerState<OperationStateVM, OperationStateVM>>();
                        if (newViewModel.Auteurs != null && newViewModel.Auteurs.Any())
                        {
                            for (int i = 0; i < newViewModel.Auteurs.Count; i++)
                            {
                                using (Task<OperationStateVM> task = DbServices.Contact.CreateAsync(newViewModel.Auteurs[i]))
                                {
                                    task.Wait();
                                    _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                    {
                                        Result = task.Result,
                                    });

                                    if (!task.Result.IsSuccess)
                                    {
                                        newViewModel.Auteurs.RemoveAt(i);
                                        i = 0;
                                        continue;
                                    }
                                    else
                                    {
                                        newViewModel.Auteurs[i].Id = task.Result.Id;
                                    }
                                }
                            }
                        }

                        if (newViewModel.Publication != null)
                        {
                            if (newViewModel.Publication.Collections != null && newViewModel.Publication.Collections.Any())
                            {
                                for (int i = 0; i < newViewModel.Publication.Collections.Count; i++)
                                {
                                    using (Task<OperationStateVM> task = DbServices.Collection.CreateAsync(newViewModel.Publication.Collections[i], IdLibrary))
                                    {
                                        task.Wait();
                                        _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                        {
                                            Result = task.Result,
                                        });

                                        if (!task.Result.IsSuccess)
                                        {
                                            newViewModel.Publication.Collections.RemoveAt(i);
                                            i = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            newViewModel.Publication.Collections[i].Id = task.Result.Id;
                                        }
                                    }
                                }
                            }

                            if (newViewModel.Publication.Editeurs != null && newViewModel.Publication.Editeurs.Any())
                            {
                                for (int i = 0; i < newViewModel.Publication.Editeurs.Count; i++)
                                {
                                    using (Task<OperationStateVM> task = DbServices.Contact.CreateAsync(newViewModel.Publication.Editeurs[i]))
                                    {
                                        task.Wait();
                                        _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                        {
                                            Result = task.Result,
                                        });

                                        if (!task.Result.IsSuccess)
                                        {
                                            newViewModel.Publication.Editeurs.RemoveAt(i);
                                            i = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            newViewModel.Publication.Editeurs[i].Id = task.Result.Id;
                                        }
                                    }
                                }
                            }
                        }

                        var bookResult = new WorkerState<OperationStateVM, OperationStateVM>()
                        {
                            ResultList = _subWorkerStates.Select(s => s.Result).ToList(),
                        };

                        using (Task<OperationStateVM> task = DbServices.Book.CreateAsync(newViewModel, IdLibrary))
                        {
                            task.Wait();
                            bookResult.Result = task.Result;
                            workerStates.Add(bookResult);
                        }

                        if (worker.CancellationPending)
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
                    e.Result = workerStates?.ToArray();
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
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des livres ont été importés.\nCette opération peut prendre un certain temps";
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
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                string message = string.Empty;
                var viewModelList = e.Result as WorkerState<OperationStateVM, OperationStateVM>[];

                // Si erreur
                if (e.Error != null)
                {
                    message = $"Une erreur s'est produite.\n{viewModelList?.Count(w => w.Result.IsSuccess == false) ?? 0} erreur(s) et {viewModelList.Select(s => s.ResultList).SelectMany(w => w.ToList()).Select(q => q).Count(c => c.IsSuccess == false)} avertissement(s).";
                }
                else if (e.Cancelled)
                {
                    message = $"L'export a été annulé par l'utilisateur.";
                }
                else
                {
                    message = $"L'import des livres s'est terminé avec {viewModelList?.Count(w => w.Result.IsSuccess == false) ?? 0} erreur(s) et {viewModelList?.Select(s => s.ResultList)?.SelectMany(w => w.ToList())?.Select(q => q)?.Count(c => c.IsSuccess == false) ?? 0} avertissement(s)";
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
                        if (CloseBusyLoaderAfterFinish)
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
                    if (CloseBusyLoaderAfterFinish)
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
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion
    }
}
