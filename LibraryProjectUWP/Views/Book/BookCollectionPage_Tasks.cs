using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book.SubViews;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        private BackgroundWorker WorkerImportBooksFromExcel;
        private BackgroundWorker workerSearchPretsBook;
        private BackgroundWorker workerSearchExemplariesBook;

        CancellationTokenSource cancellationTokenSourceCountBooks = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchPretsBook = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchBookExemplaries = new CancellationTokenSource();

        public void OpenBookCollection()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(BookCollectionSubPage), this);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenImportBookFromExcel()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(ImportBookExcelSubPage), this);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void OpenImportBookFromFile(IEnumerable<LivreVM> viewModelList)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.NavigateToView(typeof(ImportBookFileSubPage), new BookSubPageParametersDriverVM()
                {
                    ParentPage = this,
                    ViewModelList = new ObservableCollection<LivreVM>(viewModelList),
                });
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region SearchBookExemplaries
        public void InitializeSearchingBookWorker(LivreVM viewModel)
        {
            try
            {
                if (workerSearchExemplariesBook == null)
                {
                    workerSearchExemplariesBook = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //workerSearchExemplariesBook.ProgressChanged += workerSearchExemplariesBook_ProgressChanged;
                    workerSearchExemplariesBook.DoWork += WorkerSearchExemplariesBook_DoWork;
                    workerSearchExemplariesBook.RunWorkerCompleted += WorkerSearchExemplariesBook_RunWorkerCompleted;
                }

                if (workerSearchExemplariesBook != null)
                {
                    if (!workerSearchExemplariesBook.IsBusy)
                    {
                        cancellationTokenSourceSearchBookExemplaries = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.SearchBookExemplary))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.SearchBookExemplary,
                                Description = $"Récupération des exemplaires du livre {viewModel.MainTitle}"
                            });
                        }

                        workerSearchExemplariesBook.RunWorkerAsync(viewModel);
                        new ToastContentBuilder()
                        .AddText($"Exemplaires de {viewModel.MainTitle}")
                        .AddText($"Nous sommes en train de récupérer les exemplaire du livre {viewModel.MainTitle}, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText($"Exemplaires de {viewModel.MainTitle}")
                        .AddText($"Nous sommes toujours en train de récupérer les exemplaire du livre {viewModel.MainTitle}, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerSearchExemplariesBook_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                if (e.Argument is LivreVM viewModel)
                {
                    using (Task<IList<LivreExemplaryVM>> task = DbServices.Book.GetBookExemplaryVMAsync(viewModel.Id, cancellationTokenSourceSearchBookExemplaries.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceSearchBookExemplaries.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceSearchBookExemplaries.IsCancellationRequested)
                            {
                                cancellationTokenSourceSearchBookExemplaries.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;
                        var state = new WorkerState<LivreExemplaryVM, LivreExemplaryVM>()
                        {
                            ResultList = result,
                        };

                        e.Result = new Tuple<LivreVM, WorkerState<LivreExemplaryVM, LivreExemplaryVM>>(viewModel, state);
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

        private void WorkerSearchExemplariesBook_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.SearchBookExemplary);
                if (item != null)
                {
                    ViewModelPage.TaskList.Remove(item);
                }

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
                    if (e.Result is Tuple<LivreVM, WorkerState<LivreExemplaryVM, LivreExemplaryVM>> state)
                    {
                        //if (state.Item2.ResultList == null || !state.Item2.ResultList.Any())
                        //{
                        //    new ToastContentBuilder()
                        //.AddText($"Exemplaires de {state.Item1.MainTitle}")
                        //.AddText($"Il n'y a pas d'exemplaires à afficher pour ce livre en ce moment.")
                        //.Show();
                        //    return;
                        //}
                        
                        BookExemplaryList(state.Item1, state.Item2.ResultList);
                    }
                }

                workerSearchExemplariesBook.Dispose();
                workerSearchExemplariesBook = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region SearchPretsBooks
        public void InitializeSearchingBookPretsWorker(LivreVM viewModel)
        {
            try
            {
                if (workerSearchPretsBook == null)
                {
                    workerSearchPretsBook = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //workerSearchPretsBook.ProgressChanged += workerSearchExemplariesBook_ProgressChanged;
                    workerSearchPretsBook.DoWork += WorkerSearchPretsBook_DoWork;
                    workerSearchPretsBook.RunWorkerCompleted += WorkerSearchPretsBookBook_RunWorkerCompleted;
                }

                if (workerSearchPretsBook != null)
                {
                    if (!workerSearchPretsBook.IsBusy)
                    {
                        cancellationTokenSourceSearchPretsBook = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.SearchBookPret))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.SearchBookPret,
                                Description = $"Récupération des exemplaires du livre {viewModel.MainTitle}"
                            });
                        }

                        workerSearchPretsBook.RunWorkerAsync(viewModel);
                        new ToastContentBuilder()
                        .AddText($"Exemplaires de {viewModel.MainTitle}")
                        .AddText($"Nous sommes en train de récupérer les exemplaire du livre {viewModel.MainTitle}, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText($"Exemplaires de {viewModel.MainTitle}")
                        .AddText($"Nous sommes toujours en train de récupérer les exemplaire du livre {viewModel.MainTitle}, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerSearchPretsBook_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                if (e.Argument is LivreVM viewModel)
                {
                    using (Task<IList<LivrePretVM>> task = DbServices.BookPret.GetBookPretVMAsync(viewModel.Id, BookPretFrom.Book, cancellationTokenSourceSearchPretsBook.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceSearchPretsBook.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceSearchPretsBook.IsCancellationRequested)
                            {
                                cancellationTokenSourceSearchPretsBook.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;
                        var state = new WorkerState<LivrePretVM, LivrePretVM>()
                        {
                            ResultList = result,
                        };

                        e.Result = new Tuple<LivreVM, WorkerState<LivrePretVM, LivrePretVM>>(viewModel, state);
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

        private void WorkerSearchPretsBookBook_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.SearchBookPret);
                if (item != null)
                {
                    ViewModelPage.TaskList.Remove(item);
                }

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
                    if (e.Result is Tuple<LivreVM, WorkerState<LivrePretVM, LivrePretVM>> state)
                    {
                        //if (state.Item2.ResultList == null || !state.Item2.ResultList.Any())
                        //{
                        //    new ToastContentBuilder()
                        //.AddText($"Exemplaires de {state.Item1.MainTitle}")
                        //.AddText($"Il n'y a pas d'exemplaires à afficher pour ce livre en ce moment.")
                        //.Show();
                        //    return;
                        //}

                        BookPretList(state.Item1, state.Item2.ResultList);
                    }
                }

                workerSearchPretsBook.Dispose();
                workerSearchPretsBook = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Import
        public void InitializeImportBooksWorker(IEnumerable<LivreVM> viewModelList)
        {
            try
            {
                if (viewModelList == null|| !viewModelList.Any())
                {
                    return;
                }

                if (WorkerImportBooksFromExcel == null)
                {
                    WorkerImportBooksFromExcel = new BackgroundWorker()
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true,
                    };

                    WorkerImportBooksFromExcel.ProgressChanged += WorkerImportBooksFromExcel_ProgressChanged;
                    WorkerImportBooksFromExcel.DoWork += WorkerImportBooksFromExcel_DoWork; ;
                    WorkerImportBooksFromExcel.RunWorkerCompleted += WorkerImportBooksFromExcel_RunWorkerCompleted; ;
                }

                if (WorkerImportBooksFromExcel != null)
                {
                    if (!WorkerImportBooksFromExcel.IsBusy)
                    {
                        this.Parameters.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Importation de {viewModelList.Count()} livre(s) en cours",
                            CancelButtonText = "Annuler l'importation",
                            CancelButtonVisibility = Visibility.Visible,
                            CancelButtonCallback = () =>
                            {
                                if (WorkerImportBooksFromExcel.IsBusy)
                                {
                                    WorkerImportBooksFromExcel.CancelAsync();
                                }
                            },
                            OpenedLoaderCallback = () => WorkerImportBooksFromExcel.RunWorkerAsync(viewModelList),
                        });
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerImportBooksFromExcel_DoWork(object sender, DoWorkEventArgs e)
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
                                foreach (var collection in newViewModel.Publication.Collections)
                                {
                                    using (Task<OperationStateVM> task = DbServices.Collection.CreateAsync(collection, Parameters.ParentLibrary.Id))
                                    {
                                        task.Wait();
                                        _subWorkerStates.Add(new WorkerState<OperationStateVM, OperationStateVM>()
                                        {
                                            Result = task.Result,
                                        });
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

                        using (Task<OperationStateVM> task = DbServices.Book.CreateAsync(newViewModel, Parameters.ParentLibrary.Id))
                        {
                            task.Wait();
                            bookResult.Result = task.Result;
                            workerStates.Add(bookResult);
                        }

                        var NumberModel = count + 1;
                        double Operation = (double)NumberModel / (double)ModelCount;
                        progressPercentage = Operation * 100;
                        int ProgressValue = Convert.ToInt32(progressPercentage);
                        
                        if (worker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(500);
                            worker.ReportProgress(ProgressValue, bookResult);
                        }
                        count++;

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

        private void WorkerImportBooksFromExcel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState != null && e.UserState is WorkerState<OperationStateVM, OperationStateVM> workerUserState)
                {
                    //Progress bar/text
                    var busyLoader = Parameters.MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des livres importés. {(workerUserState.Result.IsSuccess ? 0 : 1)} erreur(s), {workerUserState.ResultList.Where(w => w.IsSuccess == false).Count()} avertissement(s)";
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

        private void WorkerImportBooksFromExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var busyLoader = Parameters.MainPage.GetBusyLoader;
                if (busyLoader != null)
                {
                    // Si erreur
                    if (e.Error != null)
                    {
                        busyLoader.TbcTitle.Text = $"L'import s'est terminé avec l'erreur :\n\"{e.Error.Message}\"\n\nActualisation du catalogue des livres en cours...";
                    }
                    else if (e.Cancelled)
                    {
                        busyLoader.TbcTitle.Text = $"L'import a été annulé par l'utilisateur\nActualisation du catalogue des livres en cours...";
                    }
                    else
                    {

                        if (e.Result != null && e.Result is WorkerState<OperationStateVM, OperationStateVM>[] workerUserState)
                        {
                            busyLoader.TbcTitle.Text = $"L'import des livres s'est terminé avec {workerUserState.Count(w => w.Result.IsSuccess == false)} erreur(s) et {workerUserState.Select(s => s.ResultList).SelectMany(w => w.ToList()).Select(q => q).Count(c => c.IsSuccess == false)} avertissement(s)\nActualisation du catalogue des livres en cours...";
                        }
                    }

                    if (busyLoader.BtnCancel.Visibility != Visibility.Collapsed)
                        busyLoader.BtnCancel.Visibility = Visibility.Collapsed;
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += (t, f) =>
                {
                    this.OpenBookCollection();
                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();

                WorkerImportBooksFromExcel.Dispose();
                WorkerImportBooksFromExcel = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }
        #endregion

        private void CancelTaskXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is TaskVM taskVM)
                {
                    if (taskVM.Id == EnumTaskId.CountBooks)
                    {
                        cancellationTokenSourceCountBooks?.Cancel();
                        if (WorkerImportBooksFromExcel != null && WorkerImportBooksFromExcel.IsBusy)
                        {
                            WorkerImportBooksFromExcel.CancelAsync();
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
    }
}
