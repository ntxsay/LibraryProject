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
        CancellationTokenSource cancellationTokenSource;

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
                    WorkerBackground.CancelAsync();
                }

                if (cancellationTokenSource != null && cancellationTokenSource.Token.CanBeCanceled)
                {
                    cancellationTokenSource?.Cancel();
                }

                DisposeWorker();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region
        public void InitializeWorker<T>(IEnumerable<T> viewModelList) where T : class
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
                            var parameters = new BusyLoaderParametersVM()
                            {
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
                            };

                            if (typeof(T).IsAssignableFrom(typeof(BibliothequeVM)))
                            {
                                parameters.ProgessText = $"Importation de {viewModelList.Count()} bibliothèque(s) en cours";
                            }
                            else if (typeof(T).IsAssignableFrom(typeof(LivreVM)))
                            {
                                parameters.ProgessText = $"Importation de {viewModelList.Count()} livre(s) en cours";
                            }
                            MainPage.OpenBusyLoader(parameters);
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
                if (sender is BackgroundWorker worker)
                {
                    if (e.Argument is IEnumerable<BibliothequeVM> bibliothequeVMs)
                    {
#warning Implémenter pour la bibliothèque

                    }
                    else if (e.Argument is IEnumerable<LivreVM> viewModelList)
                    {
                        using (Task<List<WorkerState<OperationStateVM, OperationStateVM>>> task = this.ImportBooksAsync(viewModelList, cancellationTokenSource.Token))
                        {
                            task.Wait();
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                e.Cancel = true;
                                return;
                            }

                            e.Result = task.Result.ToArray();
                            cancellationTokenSource.Dispose();
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
            finally
            {
                if (cancellationTokenSource!= null)
                {
                    cancellationTokenSource.Dispose();
                }
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

                // Si erreur
                if (e.Error != null)
                {
                    message = $"Une erreur s'est produite lors de l'export des livres.";
                }
                else if (e.Cancelled)
                {
                    message = $"L'import de livres a été annulé par l'utilisateur.";
                }
                else
                {
                    var viewModelList = e.Result as WorkerState<OperationStateVM, OperationStateVM>[];
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
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task<List<WorkerState<OperationStateVM, OperationStateVM>>> ImportBooksAsync(IEnumerable<BibliothequeVM> viewModelList, CancellationToken cancellationToken = default)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
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
                            OperationStateVM operationResult = await DbServices.Contact.CreateAsync(newViewModel.Auteurs[i]);
                            _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                            {
                                Result = operationResult,
                            });

                            if (!operationResult.IsSuccess)
                            {
                                newViewModel.Auteurs.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                            else
                            {
                                newViewModel.Auteurs[i].Id = operationResult.Id;
                            }
                        }
                    }

                    if (newViewModel.Publication != null)
                    {
                        if (newViewModel.Publication.Collections != null && newViewModel.Publication.Collections.Any())
                        {
                            for (int i = 0; i < newViewModel.Publication.Collections.Count; i++)
                            {
                                OperationStateVM operationResult = await DbServices.Collection.CreateAsync(newViewModel.Publication.Collections[i], IdLibrary);
                                _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                {
                                    Result = operationResult,
                                });

                                if (!operationResult.IsSuccess)
                                {
                                    newViewModel.Publication.Collections.RemoveAt(i);
                                    i = 0;
                                    continue;
                                }
                                else
                                {
                                    newViewModel.Publication.Collections[i].Id = operationResult.Id;
                                }
                            }
                        }

                        if (newViewModel.Publication.Editeurs != null && newViewModel.Publication.Editeurs.Any())
                        {
                            for (int i = 0; i < newViewModel.Publication.Editeurs.Count; i++)
                            {
                                OperationStateVM operationResult = await DbServices.Contact.CreateAsync(newViewModel.Publication.Editeurs[i]);
                                _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                {
                                    Result = operationResult,
                                });

                                if (!operationResult.IsSuccess)
                                {
                                    newViewModel.Publication.Editeurs.RemoveAt(i);
                                    i = 0;
                                    continue;
                                }
                                else
                                {
                                    newViewModel.Publication.Editeurs[i].Id = operationResult.Id;
                                }
                            }
                        }
                    }

                    var bookResult = new WorkerState<OperationStateVM, OperationStateVM>()
                    {
                        ResultList = _subWorkerStates.Select(s => s.Result).ToList(),
                    };

                    OperationStateVM finalResult = await DbServices.Library.CreateAsync(newViewModel);
                    newViewModel.Id = finalResult.Id;
                    bookResult.Result = finalResult;
                    workerStates.Add(bookResult);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        if (WorkerReportsProgress)
                        {
                            int NumberModel = count + 1;
                            double Operation = (double)NumberModel / (double)ModelCount;
                            progressPercentage = Operation * 100;
                            int ProgressValue = Convert.ToInt32(progressPercentage);

                            Thread.Sleep(100);
                            WorkerBackground.ReportProgress(ProgressValue, null);
                            count++;
                        }
                    }
                }

                return workerStates;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        public async Task<List<WorkerState<OperationStateVM, OperationStateVM>>> ImportBooksAsync(IEnumerable<LivreVM> viewModelList, CancellationToken cancellationToken = default)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
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
                            OperationStateVM operationResult = await DbServices.Contact.CreateAsync(newViewModel.Auteurs[i]);
                            _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                            {
                                Result = operationResult,
                            });

                            if (!operationResult.IsSuccess)
                            {
                                newViewModel.Auteurs.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                            else
                            {
                                newViewModel.Auteurs[i].Id = operationResult.Id;
                            }
                        }
                    }

                    if (newViewModel.Publication != null)
                    {
                        if (newViewModel.Publication.Collections != null && newViewModel.Publication.Collections.Any())
                        {
                            for (int i = 0; i < newViewModel.Publication.Collections.Count; i++)
                            {
                                OperationStateVM operationResult = await DbServices.Collection.CreateAsync(newViewModel.Publication.Collections[i], IdLibrary);
                                _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                {
                                    Result = operationResult,
                                });

                                if (!operationResult.IsSuccess)
                                {
                                    newViewModel.Publication.Collections.RemoveAt(i);
                                    i = 0;
                                    continue;
                                }
                                else
                                {
                                    newViewModel.Publication.Collections[i].Id = operationResult.Id;
                                }
                            }
                        }

                        if (newViewModel.Publication.Editeurs != null && newViewModel.Publication.Editeurs.Any())
                        {
                            for (int i = 0; i < newViewModel.Publication.Editeurs.Count; i++)
                            {
                                OperationStateVM operationResult = await DbServices.Contact.CreateAsync(newViewModel.Publication.Editeurs[i]);
                                _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                {
                                    Result = operationResult,
                                });

                                if (!operationResult.IsSuccess)
                                {
                                    newViewModel.Publication.Editeurs.RemoveAt(i);
                                    i = 0;
                                    continue;
                                }
                                else
                                {
                                    newViewModel.Publication.Editeurs[i].Id = operationResult.Id;
                                }
                            }
                        }
                    }

                    var bookResult = new WorkerState<OperationStateVM, OperationStateVM>()
                    {
                        ResultList = _subWorkerStates.Select(s => s.Result).ToList(),
                    };

                    OperationStateVM finalResult = await DbServices.Book.CreateAsync(newViewModel, IdLibrary);
                    bookResult.Result = finalResult;
                    workerStates.Add(bookResult);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        if (WorkerReportsProgress)
                        {
                            int NumberModel = count + 1;
                            double Operation = (double)NumberModel / (double)ModelCount;
                            progressPercentage = Operation * 100;
                            int ProgressValue = Convert.ToInt32(progressPercentage);

                            Thread.Sleep(100);
                            WorkerBackground.ReportProgress(ProgressValue, null);
                            count++;
                        }
                    }
                }

                return workerStates;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        #endregion
    }
}
