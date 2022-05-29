using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.PrincipalPages;
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

        public NewEditBookPretUCVM ViewModelPage { get; set; } = new NewEditBookPretUCVM();

        public delegate void CancelModificationEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditBookPretUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public BookCollectionPage ParentPage { get; private set; }
        readonly GetAvailableBookExemplariesForPretTask getAvailableBookExemplariesForPretTask = new GetAvailableBookExemplariesForPretTask();

        public NewEditBookPretUC()
        {
            this.InitializeComponent();
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

        public void InitializeSideBar(BookCollectionPage bookCollectionPage, LivreVM livreVM, LivrePretVM livrePretVm, EditMode editMode)
        {
            try
            {
                ParentPage = bookCollectionPage;
                ViewModelPage = new NewEditBookPretUCVM()
                {
                    EditMode = editMode,
                    ParentBook = livreVM,
                    ViewModel = livrePretVm ?? new LivrePretVM()
                    {
                        EtatAvantPret = new LivreEtatVM()
                        {
                            TypeVerification = BookTypeVerification.AvantPret,
                        },
                        EtatApresPret = new LivreEtatVM()
                        {
                            TypeVerification = BookTypeVerification.ApresPret,
                        },
                    },
                };

                ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Prêter un livre" : "Editer un livre")}";

                InitializeActionInfos();
                LoadDataAsync();
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
                TbcInfos.Inlines.Clear();
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter un ou plusieurs" : "d'éditer le")} exemplaire(s) au livre",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                Run runName = new Run()
                {
                    Text = " " + ViewModelPage?.ParentBook?.MainTitle,
                    FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runName);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void HyperlinkBtn_AddNew_Exemplary_Click(object sender, RoutedEventArgs e)
        {
            this.ParentPage.NewBookExemplary(ViewModelPage.ParentBook, new SideBarInterLinkVM()
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

                getAvailableBookExemplariesForPretTask.InitializeWorker(ViewModelPage.ParentBook);
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
                    ParentPage.OpenBookPretSchedule();
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
                        if (this.ParentPage != null)
                        {
                            if (!sender.Text.IsStringNullOrEmptyOrWhiteSpace())
                            {
                                var split = StringHelpers.SplitWord(sender.Text, new string[] { " " });
                                if (split.Length == 1)
                                {
                                    this.ParentPage.NewEditContact(EditMode.Create, ContactType.Human, new ContactRole[] { ContactRole.Adherant }, new SideBarInterLinkVM()
                                    {
                                        ParentGuid = ViewModelPage.ItemGuid,
                                        ParentType = typeof(NewEditBookPretUC),
                                    }, new ContactVM()
                                    {
                                        NomNaissance = split[0],
                                    });
                                }
                                else if (split.Length >= 2)
                                {
                                    this.ParentPage.NewEditContact(EditMode.Create, ContactType.Human, new ContactRole[] { ContactRole.Adherant }, new SideBarInterLinkVM()
                                    {
                                        ParentGuid = ViewModelPage.ItemGuid,
                                        ParentType = typeof(NewEditBookPretUC),
                                    }, new ContactVM()
                                    {
                                        NomNaissance = split[0],
                                        Prenom = split[1],
                                    });
                                }
                            }
                            else
                            {
                                this.ParentPage.NewEditContact(EditMode.Create, ContactType.Human, new ContactRole[] { ContactRole.Adherant }, new SideBarInterLinkVM()
                                {
                                    ParentGuid = ViewModelPage.ItemGuid,
                                    ParentType = typeof(NewEditBookPretUC),
                                }, null);
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

        #region Date
        private void HyperlinkBtn_ClearDatePret_Click(object sender, RoutedEventArgs e)
        {
             ViewModelPage.ViewModel.DatePret = DateTime.Now;
        }

        private void HyperlinkBtn_ClearTimePret_Click(object sender, RoutedEventArgs e)
        {
            ViewModelPage.ViewModel.TimePret = null;
        }

        private void HyperlinkBtn_ClearDateRemise_Click(object sender, RoutedEventArgs e)
        {
            ViewModelPage.ViewModel.DateRemise = null;
        }

        private void HyperlinkBtn_ClearTimeRemise_Click(object sender, RoutedEventArgs e)
        {
            ViewModelPage.ViewModel.TimeRemise = null;
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

                if (ViewModelPage.ViewModel.DateRemise.HasValue)
                {
                    var result = DateTimeOffset.Compare(ViewModelPage.ViewModel.DatePret, ViewModelPage.ViewModel.DateRemise.Value);
                    if (result > 0)
                    {
                        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                        ViewModelPage.ResultMessage = $"La date de prêt ne peut pas être supérieure à la date de remise.";
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                        ViewModelPage.IsResultMessageOpen = true;
                        return false;
                    }
                    else if (result == 0)
                    {
                        if (ViewModelPage.ViewModel.TimePret.HasValue && ViewModelPage.ViewModel.TimeRemise.HasValue)
                        {
                            var timeResult = TimeSpan.Compare(ViewModelPage.ViewModel.TimePret.Value, ViewModelPage.ViewModel.TimeRemise.Value);
                            if (timeResult > 0 || timeResult == 0)
                            {
                                ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                                ViewModelPage.ResultMessage = $"L'heure à laquelle le livre a été prêté ne peut pas être supérieure ou identique à l'heure à laquelle le livre sera remis.";
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                                ViewModelPage.IsResultMessageOpen = true;
                                return false;
                            }
                        }
                    }
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
        public SideBarInterLinkVM ParentReferences { get; set; }

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

        private LivreVM _ParentBook;
        public LivreVM ParentBook
        {
            get => this._ParentBook;
            set
            {
                if (this._ParentBook != value)
                {
                    this._ParentBook = value;
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