using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class NewEditBookExemplaryUC : PivotItem
    {
        public readonly ManageBookExemplaryParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public NewEditBookExemplaryUCVM ViewModelPage { get; set; } = new NewEditBookExemplaryUCVM();

        public delegate void CancelModificationEventHandler(NewEditBookExemplaryUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditBookExemplaryUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditBookExemplaryUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;
        public NewEditBookExemplaryUC()
        {
            this.InitializeComponent();
        }

        public NewEditBookExemplaryUC(ManageBookExemplaryParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} un exemplaire";
            ViewModelPage.ViewModel = parameters?.CurrentViewModel;
            InitializeActionInfos();
        }

        private async void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
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
                        Text = " " + _parameters?.Parent?.MainTitle,
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


        #region Contact Source
        private async Task LoadDataAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var itemList = await DbServices.Contact.AllVMAsync();
                ViewModelPage.ContactViewModelList = itemList?.ToList() ?? new List<ContactVM>(); ;
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

        private async void ASB_SearchContact_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is ContactVM viewModel)
                {
                    if (viewModel.Id != -1)
                    {
                        //
                        ViewModelPage.ViewModel.ContactSource = viewModel;
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
                                    await _parameters.ParentPage.NewFreeContactAsync(split[0], string.Empty, ViewModelPage.Guid);
                                }
                                else if (split.Length >= 2)
                                {
                                    await _parameters.ParentPage.NewFreeContactAsync(split[0], split[1], ViewModelPage.Guid);
                                }
                            }
                            else
                            {
                                await _parameters.ParentPage.NewFreeContactAsync(string.Empty, string.Empty, ViewModelPage.Guid);
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
        #endregion

        private void CmbxTypeAcquisition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox combobox && combobox.SelectedItem is string type)
                {
                    if (type == LibraryHelpers.Book.Entry.Achat)
                    {
                        ViewModelPage.PretVisibility = Visibility.Collapsed;
                        ViewModelPage.PriceVisibility = Visibility.Visible;
                        ViewModelPage.ContactSourceVisibility = Visibility.Visible;
                    }
                    else if (type == LibraryHelpers.Book.Entry.Don)
                    {
                        ViewModelPage.PretVisibility = Visibility.Collapsed;
                        ViewModelPage.PriceVisibility = Visibility.Collapsed;
                        ViewModelPage.ContactSourceVisibility = Visibility.Visible;
                    }
                    else if (type == LibraryHelpers.Book.Entry.Pret)
                    {
                        ViewModelPage.PretVisibility = Visibility.Visible;
                        ViewModelPage.PriceVisibility = Visibility.Visible;
                        ViewModelPage.ContactSourceVisibility = Visibility.Visible;
                    }
                    else if (type == LibraryHelpers.Book.Entry.Autre)
                    {
                        ViewModelPage.PretVisibility = Visibility.Collapsed;
                        ViewModelPage.PriceVisibility = Visibility.Collapsed;
                        ViewModelPage.ContactSourceVisibility = Visibility.Visible;
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
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                if (CreateItemRequested != null)
                {
                    CreateItemRequested(this, args);
                }
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
                if (ViewModelPage.ViewModel.NbExemplaire <= 0)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Le nombre d'exemplaire ne peut pas être inférieur ou égal à 0.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.Etat.Etat.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'état de l'exemplaire n'est pas renseigné.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.Source == LibraryHelpers.Book.Entry.Achat)
                {
                    if (!ViewModelPage.ViewModel.IsPriceUnavailable && ViewModelPage.ViewModel.Price >= 0 &&
                        ViewModelPage.ViewModel.DeviceName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                        ViewModelPage.ResultMessage = $"Vous devez spécifier le type de monnaie.";
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                        ViewModelPage.IsResultMessageOpen = true;
                        return false;
                    }
                }
                else if (ViewModelPage.ViewModel.Source == LibraryHelpers.Book.Entry.Don)
                {

                }
                else if (ViewModelPage.ViewModel.Source == LibraryHelpers.Book.Entry.Pret)
                {
                    if (!ViewModelPage.ViewModel.IsPriceUnavailable && ViewModelPage.ViewModel.Price >= 0 &&
                        ViewModelPage.ViewModel.DeviceName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                        ViewModelPage.ResultMessage = $"Vous devez spécifier le type de monnaie.";
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                        ViewModelPage.IsResultMessageOpen = true;
                        return false;
                    }
                }
                else if (ViewModelPage.ViewModel.Source == LibraryHelpers.Book.Entry.Autre)
                {

                }

                if (ViewModelPage.ViewModel.Etat.Etat.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez spécifier l'état des livres.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                ViewModelPage.ViewModel.Etat.Observations = ViewModelPage.ViewModel.Observations;

                if (!ViewModelPage.ViewModel.MonthAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.MonthAcquisition != DatesHelpers.NoAnswer &&
                    ViewModelPage.ViewModel.YearAcquisition.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ViewModel.YearAcquisition == DatesHelpers.NoAnswer)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez spécifier l'année d'acquisition pour valider le mois.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }
                else if (!ViewModelPage.ViewModel.DayAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.DayAcquisition != DatesHelpers.NoAnswer &&
                    ViewModelPage.ViewModel.MonthAcquisition.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ViewModel.MonthAcquisition == DatesHelpers.NoAnswer)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez spécifier le mois d'acquisition pour valider le jour.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }
                else
                {
                    if (!ViewModelPage.ViewModel.DayAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.DayAcquisition != DatesHelpers.NoAnswer &&
                    !ViewModelPage.ViewModel.MonthAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.MonthAcquisition != DatesHelpers.NoAnswer &&
                    !ViewModelPage.ViewModel.YearAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.YearAcquisition != DatesHelpers.NoAnswer)
                    {
                        var day = Convert.ToInt32(ViewModelPage.ViewModel.DayAcquisition);
                        var month = DatesHelpers.ChooseMonth().ToList().IndexOf(ViewModelPage.ViewModel.MonthAcquisition);
                        var year = Convert.ToInt32(ViewModelPage.ViewModel.YearAcquisition);
                        var isDateCorrect = DateTime.TryParseExact($"{day:00}/{month:00}/{year:0000}", "dd/MM/yyyy", new CultureInfo("fr-FR"), DateTimeStyles.AssumeLocal, out DateTime dateAcquisition);
                        if (!isDateCorrect)
                        {
                            ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                            ViewModelPage.ResultMessage = $"La date d'acquisition n'est pas valide.";
                            ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                            ViewModelPage.IsResultMessageOpen = true;
                            return false;
                        }
                        else
                        {
                            ViewModelPage.ViewModel.DateAcquisition = dateAcquisition.ToString("dd/MM/yyyy");
                        }
                    }
                    else if (!ViewModelPage.ViewModel.MonthAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.MonthAcquisition != DatesHelpers.NoAnswer &&
                            !ViewModelPage.ViewModel.YearAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.YearAcquisition != DatesHelpers.NoAnswer)
                    {
                        var month = DatesHelpers.ChooseMonth().ToList().IndexOf(ViewModelPage.ViewModel.MonthAcquisition);
                        var year = Convert.ToInt32(ViewModelPage.ViewModel.YearAcquisition);
                        ViewModelPage.ViewModel.DateAcquisition = $"{month:00}/{year:0000}";
                    }
                    else if (!ViewModelPage.ViewModel.YearAcquisition.IsStringNullOrEmptyOrWhiteSpace() && ViewModelPage.ViewModel.YearAcquisition != DatesHelpers.NoAnswer)
                    {
                        ViewModelPage.ViewModel.DateAcquisition = $"{ViewModelPage.ViewModel.YearAcquisition}";
                    }
                }

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

    public class NewEditBookExemplaryUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;
        public NewEditBookExemplaryUCVM()
        {
            chooseYear.Add(DatesHelpers.NoAnswer);
            chooseYear.AddRange(DatesHelpers.ChooseYear());
        }

        public IEnumerable<string> chooseDays = DatesHelpers.ChooseDays();
        public IEnumerable<string> chooseMonths = DatesHelpers.ChooseMonth();
        public List<string> chooseYear = new List<string>();
        public Guid Guid { get; set; } = Guid.NewGuid();

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

        private string _Glyph = "\uE71B";
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


        private LivreExemplaryVM _ViewModel;
        public LivreExemplaryVM ViewModel
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
