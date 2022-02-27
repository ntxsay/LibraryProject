using LibraryProjectUWP.Code.Services.Db;
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
        #region SearchBooks
        CancellationTokenSource cancellationTokenSourceSearchBook = new CancellationTokenSource();
        private void InitializeSearchingBookWorker(LivreVM viewModel)
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
                        if (state.Item2.ResultList == null || !state.Item2.ResultList.Any())
                        {
                            new ToastContentBuilder()
                        .AddText($"Exemplaires de {state.Item1.MainTitle}")
                        .AddText($"Il n'y a pas d'exemplaires à afficher pour ce livre en ce moment.")
                        .Show();
                            return;
                        }
                        
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
