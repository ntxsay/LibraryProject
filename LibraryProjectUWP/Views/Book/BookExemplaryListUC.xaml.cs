﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookExemplaryListUC : PivotItem
    {
        private BackgroundWorker worker;
        public readonly BookExemplaryListParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public BookExemplaryListUCVM ViewModelPage { get; set; } = new BookExemplaryListUCVM();


        public delegate void CancelModificationEventHandler(BookExemplaryListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(BookExemplaryListUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(BookExemplaryListUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;


        public BookExemplaryListUC()
        {
            this.InitializeComponent();
        }

        public BookExemplaryListUC(BookExemplaryListParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Exemplaires d'un livre";
            
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSearchingBookExemplaryWorker();
        }

        #region SearchBooks
        CancellationTokenSource cancellationTokenSourceSearchExemplary = new CancellationTokenSource();
        public void InitializeSearchingBookExemplaryWorker()
        {
            try
            {
                if (worker == null)
                {
                    worker = new BackgroundWorker()
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = true,
                    };

                    //worker.ProgressChanged += Worker_ProgressChanged;
                    worker.DoWork += Worker_DoWork;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                }

                if (worker != null)
                {
                    if (!worker.IsBusy)
                    {
                        cancellationTokenSourceSearchExemplary = new CancellationTokenSource();

                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                Task<IList<LivreExemplaryVM>> task = DbServices.Book.GetBookExemplaryVMAsync(_parameters.BookId, cancellationTokenSourceSearchExemplary.Token);
                task.Wait();

                if (worker.CancellationPending || cancellationTokenSourceSearchExemplary.IsCancellationRequested)
                {
                    if (!cancellationTokenSourceSearchExemplary.IsCancellationRequested)
                    {
                        cancellationTokenSourceSearchExemplary.Cancel();
                    }

                    e.Cancel = true;
                    task.Dispose();
                    return;
                }

                var result = task.Result;
                var state = new WorkerState<LivreExemplaryVM, LivreExemplaryVM>()
                {
                    ResultList = result,
                };

                e.Result = state;
                task.Dispose();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    if (e.Result is WorkerState<LivreExemplaryVM, LivreExemplaryVM> state)
                    {
                        ViewModelPage.ViewModelList = new ObservableCollection<LivreExemplaryVM>(state.ResultList);
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


        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                CreateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                UpdateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private bool IsModelValided()
        {
            try
            {
                //if (ViewModelPage.Value.IsStringNullOrEmptyOrWhiteSpace())
                //{
                //    ViewModelPage.ResultMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
                //    return false;
                //}

                //if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                //{
                //    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentLibrary?.Name?.Trim().ToLower() == ViewModelPage.Value?.Trim().ToLower());
                //    if (isError)
                //    {
                //        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                //        return false;
                //    }
                //}

                //ViewModelPage.ResultMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


       

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (CreateItemRequested != null)
                {
                    CreateItemRequested = null;
                }

                if (UpdateItemRequested != null)
                {
                    UpdateItemRequested = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void MenuFlyout_Opened(object sender, object e)
        {

        }

        private void ASB_SearchEditor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void ASB_SearchEditor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void ASB_SearchEditor_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void ASB_SearchCollection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void ASB_SearchCollection_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void ASB_SearchCollection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    
    public class BookExemplaryListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _Header;
        public string Header
        {
            get => this._Header;
            set
            {
                if (this._Header != value)
                {
                    this._Header = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Glyph = "\uE7BC";
        public string Glyph
        {
            get => _Glyph;
            set
            {
                if (_Glyph != value)
                {
                    _Glyph = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ResultMessage;
        public string ResultMessage
        {
            get => this._ResultMessage;
            set
            {
                if (this._ResultMessage != value)
                {
                    this._ResultMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ArgName;
        public string ArgName
        {
            get => this._ArgName;
            set
            {
                if (this._ArgName != value)
                {
                    this._ArgName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        

        private Brush _ResultMessageForeGround;
        public Brush ResultMessageForeGround
        {
            get => this._ResultMessageForeGround;
            set
            {
                if (this._ResultMessageForeGround != value)
                {
                    this._ResultMessageForeGround = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private BibliothequeVM _ParentLibrary;
        public BibliothequeVM ParentLibrary
        {
            get => this._ParentLibrary;
            set
            {
                if (this._ParentLibrary != value)
                {
                    this._ParentLibrary = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _SelectedSCategorieName;
        public string SelectedSCategorieName
        {
            get => _SelectedSCategorieName;
            set
            {
                if (_SelectedSCategorieName != value)
                {
                    _SelectedSCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedCategorieName;
        public string SelectedCategorieName
        {
            get => _SelectedCategorieName;
            set
            {
                if (_SelectedCategorieName != value)
                {
                    _SelectedCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedSubCategorieName;
        public string SelectedSubCategorieName
        {
            get => _SelectedSubCategorieName;
            set
            {
                if (_SelectedSubCategorieName != value)
                {
                    _SelectedSubCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private CollectionVM _SelectedViewModel;
        public CollectionVM SelectedViewModel
        {
            get => _SelectedViewModel;
            set
            {
                if (_SelectedViewModel != value)
                {
                    _SelectedViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LivreExemplaryVM> _ViewModelList = new ObservableCollection<LivreExemplaryVM>();
        public ObservableCollection<LivreExemplaryVM> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (_ViewModelList != value)
                {
                    this._ViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
