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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        private BackgroundWorker workerCountBooks;
        private BackgroundWorker workerResearchBooks;
        private BackgroundWorker workerSearchBooks;
        private BackgroundWorker workerSearchPretsBook;
        private BackgroundWorker workerSearchExemplariesBook;

        CancellationTokenSource cancellationTokenSourceSearchBooks = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceCountBooks = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchPretsBook = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchBookExemplaries = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceResearchBooks = new CancellationTokenSource();

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

        #region CountBooks
        public void InitializeCountBookWorker()
        {
            try
            {
                if (workerCountBooks == null)
                {
                    workerCountBooks = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //worker.ProgressChanged += Worker_ProgressChanged;
                    workerCountBooks.DoWork += WorkerCountBooks_DoWork; ;
                    workerCountBooks.RunWorkerCompleted += WorkerCountBooks_RunWorkerCompleted; ;
                }

                if (workerCountBooks != null)
                {
                    if (!workerCountBooks.IsBusy)
                    {
                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.CountBooks))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.CountBooks,
                                Description = "Recherche du nombre de livre par bibliothèque."
                            });
                        }

                        workerCountBooks.RunWorkerAsync();
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

        private void WorkerCountBooks_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                List<WorkerState<long>> workerStates = new List<WorkerState<long>>();
                //foreach (BibliothequeVM viewModel in ViewModelPage.ViewModelList)
                //{
                //    Task<long> task = DbServices.Book.CountBooksInLibraryAsync(viewModel.Id, cancellationTokenSourceCountBooks.Token);
                //    task.Wait();
                //    var result = task.Result;
                //    workerStates.Add(new WorkerState<long>()
                //    {
                //        Id = viewModel.Id,
                //        Result = result,
                //    });
                //    task.Dispose();

                //    if (worker.CancellationPending)
                //    {
                //        cancellationTokenSourceCountBooks.Cancel();
                //        e.Cancel = true;
                //        return;
                //    }
                //    Thread.Sleep(1000);
                //}
                e.Result = workerStates.ToArray();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }

        private void WorkerCountBooks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    if (e.Result is IEnumerable<WorkerState<long>> workerStates)
                    {
                        //foreach (BibliothequeVM viewModel in ViewModelPage.ViewModelList)
                        //{
                        //    var state = workerStates.SingleOrDefault(w => w.Id == viewModel.Id);
                        //    if (state != null)
                        //    {
                        //        viewModel.CountBooks = state.Result;
                        //    }
                        //}
                    }
                }

                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.CountBooks);
                if (item != null)
                {
                    ViewModelPage.TaskList.Remove(item);
                }

                workerCountBooks.Dispose();
                workerCountBooks = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }

        private void CancelTaskXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is TaskVM taskVM)
                {
                    if (taskVM.Id == EnumTaskId.CountBooks)
                    {
                        cancellationTokenSourceCountBooks?.Cancel();
                        if (workerCountBooks != null && workerCountBooks.IsBusy)
                        {
                            workerCountBooks.CancelAsync();
                        }
                    }
                    else if (taskVM.Id == EnumTaskId.SearchBooks)
                    {
                        cancellationTokenSourceSearchBooks?.Cancel();
                        if (workerSearchBooks != null && workerSearchBooks.IsBusy)
                        {
                            workerSearchBooks.CancelAsync();
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
        #endregion

        #region ResearchBook
        public void InitializeResearchingBookWorker()
        {
            try
            {
                if (workerResearchBooks == null)
                {
                    workerResearchBooks = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    workerResearchBooks.DoWork += WorkerResearchBooks_DoWork;
                    workerResearchBooks.RunWorkerCompleted += WorkerResearchBooks_RunWorkerCompleted;
                }

                if (workerResearchBooks != null)
                {
                    if (!workerResearchBooks.IsBusy)
                    {
                        cancellationTokenSourceResearchBooks = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.ResearchBooks))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.ResearchBooks,
                                Description = $"Recherche de livres avec le terme : \"{ViewModelPage.ResearchBook.Term}\" en cours.."
                            });
                        }

                        workerResearchBooks.RunWorkerAsync();
                        //new ToastContentBuilder()
                        //.AddText($"Exemplaires de {viewModel.MainTitle}")
                        //.AddText($"Nous sommes en train de récupérer les exemplaire du livre {viewModel.MainTitle}, nous vous prions de patienter quelques instants.")
                        //.Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText($"Rechercher des livres")
                        .AddText($"Nous sommes toujours en train de rechercher les livres, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerResearchBooks_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    using (Task<IList<LivreVM>> task = DbServices.Book.SearchBooksVMAsync(ViewModelPage.ResearchBook, cancellationTokenSourceResearchBooks.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceResearchBooks.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceResearchBooks.IsCancellationRequested)
                            {
                                cancellationTokenSourceResearchBooks.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;
                        var state = new WorkerState<LivreVM, LivreVM>()
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

        private async void WorkerResearchBooks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.ResearchBooks);
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
                    if (e.Result is WorkerState<LivreVM, LivreVM> state)
                    {
                        //GroupItemsBySearch(state.ResultList.ToList());
                        //await RefreshItemsGrouping(state.ResultList.ToList());
                        new ToastContentBuilder()
                        .AddText($"Rechercher des livres")
                        .AddText($"La recherche s'est terminé avec {state.ResultList.Count()} livre(s) trouvé.")
                        .Show();
                    }
                }

                workerResearchBooks.Dispose();
                workerResearchBooks = null;
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
