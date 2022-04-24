using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels;
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
    public sealed partial class BookPretListUC : PivotItem
    {
        public readonly BookPretListParametersDriverVM _parameters;
        private CollectionViewSource CollectionViewSource { get; set; } = new CollectionViewSource()
        {
            IsSourceGrouped = true,
            ItemsPath = new PropertyPath("Items")
        };

        public BookPretListUCVM ViewModelPage { get; set; } = new BookPretListUCVM();

        public delegate void CancelModificationEventHandler(BookPretListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(BookPretListUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(BookPretListUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        readonly GetBookPretsTask getBookPretsTask = new GetBookPretsTask();

        public BookPretListUC()
        {
            this.InitializeComponent();
        }

        public BookPretListUC(BookPretListParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Prêts d'un livre";
            InitializeData();
        }

        public void Close()
        {
            CancelModificationRequested?.Invoke(this, null);
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void InitializeData()
        {
            try
            {
                ViewModelPage.WorkerTextVisibility = Visibility.Visible;
                ViewModelPage.DataListVisibility = Visibility.Collapsed;
                if (getBookPretsTask.IsWorkerRunning)
                {
                    return;
                }

                getBookPretsTask.InitializeWorker(_parameters.ParentBook);
                getBookPretsTask.AfterTaskCompletedRequested += (j, e) =>
                {
                    if (e.Result is Tuple<LivreVM, WorkerState<LivrePretVM, LivrePretVM>> result && result.Item2.ResultList != null && result.Item2.ResultList.Any())
                    {
                        var GroupingItems = this.OrderItems(result.Item2.ResultList)?.OrderByDescending(q => q.DateRemise).GroupBy(s => s.PretStatus()).Select(s => s);
                        if (GroupingItems != null && GroupingItems.Count() > 0)
                        {
                            if (this.CollectionViewSource.View == null)
                            {
                                this.CollectionViewSource.Source = ViewModelPage.ViewModelListGroup;
                            }

                            List<LivrePretVMCastVM> LivrePretVMCastVMs = GroupingItems.Select(groupingItem => new LivrePretVMCastVM()
                            {
                                GroupName = groupingItem.Key,
                                Items = new ObservableCollection<LivrePretVM>(groupingItem),
                            }).ToList();

                            ViewModelPage.ViewModelListGroup.Clear();
                            foreach (var item in LivrePretVMCastVMs)
                            {
                                ViewModelPage.ViewModelListGroup.Add(item);
                            }
                        }

                    }
                    ViewModelPage.WorkerTextVisibility = Visibility.Collapsed;
                    ViewModelPage.DataListVisibility = Visibility.Visible;
                };
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        #region Group-Orders
        private IEnumerable<LivrePretVM> OrderItems(IEnumerable<LivrePretVM> Collection, LivrePretGroupVM.OrderBy OrderBy = LivrePretGroupVM.OrderBy.Croissant, LivrePretGroupVM.SortBy SortBy = LivrePretGroupVM.SortBy.DatePret)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == LivrePretGroupVM.SortBy.DatePret)
                {
                    if (OrderBy == LivrePretGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null).OrderBy(o => o.DatePret);
                    }
                    else if (OrderBy == LivrePretGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null).OrderByDescending(o => o.DatePret);
                    }
                }
                

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<LivrePretVM>();
            }
        }


        #endregion

        private void ABBtn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
        }

        private async void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is ListViewItem listViewItem && listViewItem.Tag is LivrePretVM viewModel)
                {
                    var dialog = new ReturnBookExemplaryCD(viewModel)
                    {
                        Title = viewModel.Exemplary.NoExemplaire,
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        InitializeData();
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return;
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

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                _parameters.ParentPage.NewBookPret(_parameters.ParentBook, null, EditMode.Create, new SideBarInterLinkVM() 
                { 
                    ParentType = typeof(BookPretListUC), 
                    ParentGuid = ViewModelPage.ItemGuid 
                });
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

                getBookPretsTask?.DisposeWorker();
                ViewModelPage = null;
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

    public class BookPretListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        //public Guid? ParentGuid { get; set; }

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

        private string _Glyph = "\uE762";
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

        private Visibility _DataListVisibility;
        public Visibility DataListVisibility
        {
            get => this._DataListVisibility;
            set
            {
                if (this._DataListVisibility != value)
                {
                    this._DataListVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _WorkerTextVisibility;
        public Visibility WorkerTextVisibility
        {
            get => this._WorkerTextVisibility;
            set
            {
                if (this._WorkerTextVisibility != value)
                {
                    this._WorkerTextVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LivrePretVMCastVM> _ViewModelListGroup = new ObservableCollection<LivrePretVMCastVM>();
        public ObservableCollection<LivrePretVMCastVM> ViewModelListGroup
        {
            get => this._ViewModelListGroup;
            set
            {
                if (this._ViewModelListGroup != value)
                {
                    this._ViewModelListGroup = value;
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
