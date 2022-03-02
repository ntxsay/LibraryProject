using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
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

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        private BackgroundWorker workerSearchExemplariesBook;
        private BackgroundWorker workerCompleteInfoBook;
        CancellationTokenSource cancellationTokenSourceSearchBook = new CancellationTokenSource();
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

                        if (!ViewModelPage.TaskList.Any(a => a.Id == BookCollectionPageVM.CompleteInfoBookExemplaryTaskId))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = BookCollectionPageVM.SearchBookExemplaryTaskId,
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
                            LivreVM iBook = iGroupingBook as LivreVM;
                            if (iBook != null && iBook.Id == book.Id)
                            {
                                iBook.NbExemplaires = result;
                                iBook.JaquettePath = jaquetteFile;
                                break;
                            }
                        }
                    }
                }

                

                //if (e.Argument is LivreVM viewModel)
                //{
                //    using (Task<IList<LivreExemplaryVM>> task = DbServices.Book.GetBookExemplaryVMAsync(viewModel.Id, cancellationTokenSourceSearchBook.Token))
                //    {
                //        task.Wait();

                //        if (worker.CancellationPending || cancellationTokenSourceSearchBook.IsCancellationRequested)
                //        {
                //            if (!cancellationTokenSourceSearchBook.IsCancellationRequested)
                //            {
                //                cancellationTokenSourceSearchBook.Cancel();
                //            }

                //            e.Cancel = true;
                //            return;
                //        }

                //        var result = task.Result;
                //        var state = new WorkerState<LivreExemplaryVM, LivreExemplaryVM>()
                //        {
                //            ResultList = result,
                //        };

                //        e.Result = new Tuple<LivreVM, WorkerState<LivreExemplaryVM, LivreExemplaryVM>>(viewModel, state);
                //    }
                //}
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void WorkerCompleteInfoBook_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == BookCollectionPageVM.CompleteInfoBookExemplaryTaskId);
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

                workerCompleteInfoBook.Dispose();
                workerCompleteInfoBook = null;
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
                        cancellationTokenSourceSearchBook = new CancellationTokenSource();

                        if (!ViewModelPage.TaskList.Any(a => a.Id == BookCollectionPageVM.SearchBookExemplaryTaskId))
                        {
                            ViewModelPage.TaskList.Add(new TaskVM()
                            {
                                Id = BookCollectionPageVM.SearchBookExemplaryTaskId,
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
                    using (Task<IList<LivreExemplaryVM>> task = DbServices.Book.GetBookExemplaryVMAsync(viewModel.Id, cancellationTokenSourceSearchBook.Token))
                    {
                        task.Wait();

                        if (worker.CancellationPending || cancellationTokenSourceSearchBook.IsCancellationRequested)
                        {
                            if (!cancellationTokenSourceSearchBook.IsCancellationRequested)
                            {
                                cancellationTokenSourceSearchBook.Cancel();
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
                var item = ViewModelPage.TaskList.SingleOrDefault(a => a.Id == BookCollectionPageVM.SearchBookExemplaryTaskId);
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
    }
}
