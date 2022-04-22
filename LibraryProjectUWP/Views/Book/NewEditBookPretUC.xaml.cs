using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class NewEditBookPretUC : PivotItem
    {
        public readonly ManageBookPretParametersDriverVM _parameters;

        public NewEditBookPretUCVM ViewModelPage { get; set; } = new NewEditBookPretUCVM();

        public delegate void CancelModificationEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        readonly GetAvailableBookExemplariesForPretTask getAvailableBookExemplariesForPretTask = new GetAvailableBookExemplariesForPretTask();

        public NewEditBookPretUC()
        {
            this.InitializeComponent();
        }

        public NewEditBookPretUC(ManageBookPretParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Prêter un livre" : "Editer un livre")}";
            ViewModelPage.ViewModel = parameters?.CurrentViewModel;
            InitializeActionInfos();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();
        }

        public void Close()
        {
            CancelModificationRequested?.Invoke(this, null);
        }

        public void Select()
        {
            try
            {
                if (this.Parent != null && this.Parent is Pivot pivot)
                {
                    pivot.SelectedItem = this;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void InitializeActionInfos()
        {
            try
            {
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter un ou plusieurs" : "d'éditer le")} exemplaire(s) au livre",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (_parameters != null)
                {
                    Run runName = new Run()
                    {
                        Text = " " + _parameters?.ParentBook?.MainTitle,
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runName);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void HyperlinkBtn_AddNew_Exemplary_Click(object sender, RoutedEventArgs e)
        {
            _parameters.ParentPage.NewBookExemplary(_parameters.ParentBook, new SideBarInterLinkVM()
            {
                ParentGuid = ViewModelPage.ItemGuid,
                ParentType = typeof(NewEditBookPretUC)
            });
        }

        private void BtnRefresh_AvailableExemplaries_Click(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();
        }

        #region Contact Source
        public void LoadDataAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.WorkerTextVisibility = Visibility.Visible;
                ViewModelPage.AddExemplaryBtnVisibility = Visibility.Collapsed;
                if (getAvailableBookExemplariesForPretTask.IsWorkerRunning)
                {
                    return;
                }

                getAvailableBookExemplariesForPretTask.InitializeWorker(_parameters.ParentBook);
                getAvailableBookExemplariesForPretTask.AfterTaskCompletedRequested += async (j, e) =>
                {
                    if (e.Result is Tuple<LivreVM, WorkerState<LivreExemplaryVM, LivreExemplaryVM>> result && result.Item2.ResultList != null && result.Item2.ResultList.Any())
                    {
                        ViewModelPage.AvailableExemplaries = new ObservableCollection<LivreExemplaryVM>(result.Item2.ResultList);
                    }

                    IList<ContactVM> itemList = await DbServices.Contact.AllVMAsync();
                    ViewModelPage.ContactViewModelList = itemList?.ToList() ?? new List<ContactVM>();

                    ViewModelPage.WorkerTextVisibility = Visibility.Collapsed;
                    ViewModelPage.AddExemplaryBtnVisibility = Visibility.Visible;
                };
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchContact_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ContactViewModelList == null)
                {
                    return;
                }

                var FilteredItems = new List<ContactVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.ContactViewModelList)
                {
                    if (!value.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) =>
                        {
                            return value.SocietyName.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }

                    if (!value.NomNaissance.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) =>
                        {
                            return value.NomNaissance.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }

                    if (!value.NomUsage.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) =>
                        {
                            return value.NomUsage.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }

                    if (!value.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) =>
                        {
                            return value.Prenom.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }

                    if (!value.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) =>
                        {
                            return value.AutresPrenoms.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }
                }

                if (!FilteredItems.Any())
                {
                    FilteredItems.Add(new ContactVM()
                    {
                        Id = -1,
                        NomNaissance = "Ajouter un contact",
                    });
                }

                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchContact_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is ContactVM value)
                {
                    if (value.Id != -1)
                    {
                        sender.Text = value.SocietyName + " " + value.NomNaissance + " " + value.Prenom;
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

        private void ASB_SearchContact_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is ContactVM viewModel)
                {
                    if (viewModel.Id != -1)
                    {
                        //
                        ViewModelPage.ViewModel.Emprunteur = viewModel;
                        ViewModelPage.ViewModel.IdEmprunteur = viewModel.Id;
                    }
                    else
                    {
                        //Ajoute un nouvel auteur
                        if (_parameters.ParentPage != null)
                        {
                            if (!sender.Text.IsStringNullOrEmptyOrWhiteSpace())
                            {
                                var split = StringHelpers.SplitWord(sender.Text, new string[] { " " });
                                if (split.Length == 1)
                                {
                                    _parameters.ParentPage.NewContact(ContactType.Human, ContactRole.Adherant, split[0], string.Empty, string.Empty, ViewModelPage.ItemGuid);
                                }
                                else if (split.Length >= 2)
                                {
                                    _parameters.ParentPage.NewContact(ContactType.Human, ContactRole.Adherant, split[0], split[1], string.Empty, ViewModelPage.ItemGuid);
                                }
                            }
                            else
                            {
                                _parameters.ParentPage.NewContact(ContactType.Human, ContactRole.Adherant, string.Empty, string.Empty, string.Empty, ViewModelPage.ItemGuid);
                            }
                            sender.Text = String.Empty;
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

        private void MFI_DeleteContactSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelPage.ViewModel.Emprunteur = null;
                ViewModelPage.ViewModel.IdEmprunteur = null;
                ASB_SearchContact.Text = string.Empty;
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
                if (ViewModelPage.ViewModel.Exemplary == null)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner un exemplaire";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.EtatAvantPret.Etat.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'état du livre avant le prêt n'est pas renseigné.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.TimePret != null)
                {
                    var date = new DateTimeOffset(new DateTime(ViewModelPage.ViewModel.DatePret.Year, ViewModelPage.ViewModel.DatePret.Month,
                                                    ViewModelPage.ViewModel.DatePret.Month, ViewModelPage.ViewModel.DatePret.Day,
                                                    ViewModelPage.ViewModel.TimePret.Value.Hours, ViewModelPage.ViewModel.TimePret.Value.Minutes, ViewModelPage.ViewModel.TimePret.Value.Seconds));
                    
                    ViewModelPage.ViewModel.DatePret = date.ToUniversalTime();
                }

                if (ViewModelPage.ViewModel.TimeRemise.HasValue && ViewModelPage.ViewModel.DateRemise.HasValue)
                {
                    var date = new DateTimeOffset(new DateTime(ViewModelPage.ViewModel.DateRemise.Value.Year, ViewModelPage.ViewModel.DateRemise.Value.Month,
                                                    ViewModelPage.ViewModel.DateRemise.Value.Month, ViewModelPage.ViewModel.DateRemise.Value.Day,
                                                    ViewModelPage.ViewModel.TimeRemise.Value.Hours, ViewModelPage.ViewModel.TimeRemise.Value.Minutes, ViewModelPage.ViewModel.TimeRemise.Value.Seconds));

                    ViewModelPage.ViewModel.DateRemise = date;
                }

                ViewModelPage.ViewModel.NoExemplary = ViewModelPage.ViewModel.Exemplary.NoExemplaire;
                ViewModelPage.ViewModel.IdBookExemplary = ViewModelPage.ViewModel.Exemplary.Id;
                ViewModelPage.IsResultMessageOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

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

        
    }

    public class NewEditBookPretUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;
        public NewEditBookPretUCVM()
        {
            chooseYear.Add(DatesHelpers.NoAnswer);
            chooseYear.AddRange(DatesHelpers.ChooseYear());
        }

        public IEnumerable<string> chooseDays = DatesHelpers.ChooseDays();
        public IEnumerable<string> chooseMonths = DatesHelpers.ChooseMonth();
        public List<string> chooseYear = new List<string>();
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        public Guid? ParentGuid { get; set; }

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

        private string _Glyph = "\uE748";
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

        private Visibility _AddExemplaryBtnVisibility;
        public Visibility AddExemplaryBtnVisibility
        {
            get => this._AddExemplaryBtnVisibility;
            set
            {
                if (this._AddExemplaryBtnVisibility != value)
                {
                    this._AddExemplaryBtnVisibility = value;
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

        private Visibility _ContactSourceVisibility;
        public Visibility ContactSourceVisibility
        {
            get => this._ContactSourceVisibility;
            set
            {
                if (this._ContactSourceVisibility != value)
                {
                    this._ContactSourceVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _PriceVisibility;
        public Visibility PriceVisibility
        {
            get => this._PriceVisibility;
            set
            {
                if (this._PriceVisibility != value)
                {
                    this._PriceVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _PretVisibility;
        public Visibility PretVisibility
        {
            get => this._PretVisibility;
            set
            {
                if (this._PretVisibility != value)
                {
                    this._PretVisibility = value;
                    this.OnPropertyChanged();
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

        private InfoBarSeverity _ResultMessageSeverity = InfoBarSeverity.Informational;
        public InfoBarSeverity ResultMessageSeverity
        {
            get => this._ResultMessageSeverity;
            set
            {
                if (this._ResultMessageSeverity != value)
                {
                    this._ResultMessageSeverity = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsResultMessageOpen;
        public bool IsResultMessageOpen
        {
            get => this._IsResultMessageOpen;
            set
            {
                if (this._IsResultMessageOpen != value)
                {
                    this._IsResultMessageOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessageTitle;
        public string ResultMessageTitle
        {
            get => this._ResultMessageTitle;
            set
            {
                if (this._ResultMessageTitle != value)
                {
                    this._ResultMessageTitle = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private LivrePretVM _ViewModel;
        public LivrePretVM ViewModel
        {
            get => this._ViewModel;
            set
            {
                if (this._ViewModel != value)
                {
                    this._ViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private EditMode _EditMode;
        public EditMode EditMode
        {
            get => this._EditMode;
            set
            {
                if (this._EditMode != value)
                {
                    this._EditMode = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LivreExemplaryVM> _AvailableExemplaries = new ObservableCollection<LivreExemplaryVM>();
        public ObservableCollection<LivreExemplaryVM> AvailableExemplaries
        {
            get => this._AvailableExemplaries;
            set
            {
                if (_AvailableExemplaries != value)
                {
                    this._AvailableExemplaries = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<ContactVM> _ContactViewModelList = Enumerable.Empty<ContactVM>();
        public IEnumerable<ContactVM> ContactViewModelList
        {
            get => this._ContactViewModelList;
            set
            {
                if (_ContactViewModelList != value)
                {
                    this._ContactViewModelList = value;
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