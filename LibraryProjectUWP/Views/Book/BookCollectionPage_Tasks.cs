using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BackgroundWorker workerSearchBooks;
        private BackgroundWorker workerSearchPretsBook;
        private BackgroundWorker workerSearchExemplariesBook;
        private BackgroundWorker workerCompleteInfoBook;

        CancellationTokenSource cancellationTokenSourceSearchBooks = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceCountBooks = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchPretsBook = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceSearchBookExemplaries = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSourceCompleteInfoBook = new CancellationTokenSource();

        #region Complete Info Book
        public void InitializeCompleteInfoBookWorker()
        {
            try
            {
                if (workerCompleteInfoBook == null)
                {
                    workerCompleteInfoBook = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //workerSearchExemplariesBook.ProgressChanged += workerSearchExemplariesBook_ProgressChanged;
                    workerCompleteInfoBook.DoWork += WorkerCompleteInfoBook_DoWork;
                    workerCompleteInfoBook.RunWorkerCompleted += WorkerCompleteInfoBook_RunWorkerCompleted;
                }

                if (workerCompleteInfoBook != null)
                {
                    if (!workerCompleteInfoBook.IsBusy)
                    {
                        cancellationTokenSourceCompleteInfoBook = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.CompleteInfoBook))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.CompleteInfoBook,
                                Description = $"Finalisation de la structure des livres"
                            });
                        }

                        workerCompleteInfoBook.RunWorkerAsync();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }        

        private void WorkerCompleteInfoBook_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                foreach (var book in _parameters.ParentLibrary.Books)
                {
                    using (Task<long> task = DbServices.Book.CountExemplaryInBookAsync(book.Id, cancellationTokenSourceCompleteInfoBook.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceCompleteInfoBook.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceCompleteInfoBook.IsCancellationRequested)
                            {
                                cancellationTokenSourceCompleteInfoBook.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;
                        string jaquetteFile = EsGeneral.BookDefaultJaquette;

                        using (var taskJaquettes = esBook.GetBookItemJaquettePathAsync(book))
                        {
                            taskJaquettes.Wait();

                            string combinedPath = taskJaquettes.Result;
                            jaquetteFile = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : EsGeneral.BookDefaultJaquette;
                        }

                        book.NbExemplaires = result;
                        book.JaquettePath = jaquetteFile;

                        foreach (IGrouping<string, LivreVM> iGroupingBook in ViewModelPage.GroupedRelatedViewModel.Collection)
                        {
                            if (iGroupingBook is LivreVM iBook && iBook.Id == book.Id)
                            {
                                iBook.NbExemplaires = result;
                                iBook.JaquettePath = jaquetteFile;
                                break;
                            }
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

        private async void WorkerCompleteInfoBook_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.CompleteInfoBook);
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
                    
                }
                ViewModelPage.BackgroundImagePath = await esBook.GetBookCollectionBackgroundImagePathAsync();
                await InitializeBackgroundImagesync();

                workerCompleteInfoBook.Dispose();
                workerCompleteInfoBook = null;

                await InitializeDataAsync(true);

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion


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

        #region SearchBooks
        public void InitializeSearchingBookWorker(BibliothequeVM viewModel)
        {
            try
            {
                if (workerSearchBooks == null)
                {
                    workerSearchBooks = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //worker.ProgressChanged += Worker_ProgressChanged;
                    workerSearchBooks.DoWork += WorkerSearchBooks_DoWork;
                    workerSearchBooks.RunWorkerCompleted += WorkerSearchBooks_RunWorkerCompleted;
                }

                if (workerSearchBooks != null)
                {
                    if (!workerSearchBooks.IsBusy)
                    {
                        cancellationTokenSourceSearchBooks = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == EnumTaskId.SearchBooks))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = EnumTaskId.SearchBooks,
                                Description = $"Récupération des livres de la bibliothèque {viewModel.Name}"
                            });
                        }

                        workerSearchBooks.RunWorkerAsync(viewModel);
                        new ToastContentBuilder()
                        .AddText($"Bibliothèque {viewModel.Name}")
                        .AddText("Nous sommes en train de récupérer vos livres, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText($"Bibliothèque {viewModel.Name}")
                        .AddText("Nous sommes toujours en train de récupérer vos livres, nous vous prions de patienter quelques instants.")
                        .Show();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerSearchBooks_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                if (e.Argument is BibliothequeVM viewModel)
                {
                    using (Task<IList<LivreVM>> task = DbServices.Book.MultipleVmWithIdLibraryAsync(viewModel.Id, cancellationTokenSourceSearchBooks.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceSearchBooks.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceSearchBooks.IsCancellationRequested)
                            {
                                cancellationTokenSourceSearchBooks.Cancel();
                            }

                            e.Cancel = true;
                            return;
                        }

                        var result = task.Result;

                        //EsBook esBook = new EsBook();
                        //foreach (var book in result)
                        //{
                        //    using (var taskJaquettes = esBook.GetBookItemJaquettePathAsync(book))
                        //    {
                        //        taskJaquettes.Wait();

                        //        if (worker.CancellationPending || cancellationTokenSourceSearchBook.IsCancellationRequested)
                        //        {
                        //            if (!cancellationTokenSourceSearchBook.IsCancellationRequested)
                        //            {
                        //                cancellationTokenSourceSearchBook.Cancel();
                        //            }

                        //            e.Cancel = true;
                        //            return;
                        //        }

                        //        string combinedPath = taskJaquettes.Result;
                        //        book.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                        //    }
                        //}

                        var state = new WorkerState<LivreVM, LivreVM>()
                        {
                            Id = viewModel.Id,
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

        private void WorkerSearchBooks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == EnumTaskId.SearchBooks);
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
                        _parameters.ParentLibrary.Books.Clear();
                        _parameters.ParentLibrary.Books.AddRange(state.ResultList);
                        this.InitializeCompleteInfoBookWorker();
                    }
                }

                workerSearchBooks.Dispose();
                workerSearchBooks = null;
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


    }
}
