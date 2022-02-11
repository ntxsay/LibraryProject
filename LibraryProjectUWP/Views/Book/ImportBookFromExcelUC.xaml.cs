using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Excel;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public sealed partial class ImportBookFromExcelUC : PivotItem
    {
        private readonly ExcelServices excelServices;
        public readonly ImportBookParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public ImportBookFromExcelUCVM ViewModelPage { get; set; } = new ImportBookFromExcelUCVM();

        public delegate void CancelModificationEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;


        public delegate void ImportDataEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event ImportDataEventHandler ImportDataRequested;
        public ImportBookFromExcelUC()
        {
            this.InitializeComponent();
        }

        public ImportBookFromExcelUC(ImportBookParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            excelServices = new ExcelServices(parameters.ExcelFile);
        }

        private async void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var worksheetsName = await excelServices.GetExcelSheetsName();
                ViewModelPage.WorkSheetsName = new ObservableCollection<string>(worksheetsName);

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void SearchDataInFileXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (ViewModelPage.SelectedWorkSheetName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner une feuille";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return;
                }

                if (ViewModelPage.TableRange.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner une plage de cellule pou délimiter votre tableau.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return;
                }
                var rr = await excelServices.ImportExcelToDatatable(ViewModelPage.SelectedWorkSheetName, ViewModelPage.TableRange, ViewModelPage.IsTableRangeContainsHeader);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Date Acquisition
        private void MfiClearRemiseDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.DateRemiseLivre != null)
                {
                    ViewModelPage.ViewModel.DateRemiseLivre = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void MfiClearParutionDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.DateAcquisition != null)
                {
                    ViewModelPage.ViewModel.DateAcquisition = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void TmfiDayKnow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleMenuFlyoutItem toggle)
                {
                    if (toggle.IsChecked)
                    {
                        if (ViewModelPage.ViewModel.IsMoisAcquisitionKnow == false || ViewModelPage.ViewModel.DateAcquisition == null)
                        {
                            ViewModelPage.ViewModel.IsJourAcquisitionKnow = false;
                            ViewModelPage.ViewModel.IsJourAcquisitionVisible = false;
                            toggle.IsChecked = false;
                        }
                        else
                        {
                            ViewModelPage.ViewModel.IsJourAcquisitionKnow = true;
                            ViewModelPage.ViewModel.IsJourAcquisitionVisible = true;
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.IsJourAcquisitionKnow = false;
                        ViewModelPage.ViewModel.IsJourAcquisitionVisible = false;
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

        private void TmfiMonthKnow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleMenuFlyoutItem toggle)
                {
                    if (toggle.IsChecked)
                    {
                        if (ViewModelPage.ViewModel.DateAcquisition == null)
                        {
                            ViewModelPage.ViewModel.IsJourAcquisitionVisible = true;
                            ViewModelPage.ViewModel.IsMoisAcquisitionVisible = true;
                            ViewModelPage.ViewModel.IsMoisAcquisitionKnow = false;
                            ViewModelPage.ViewModel.IsJourAcquisitionKnow = false;
                        }
                        else
                        {
                            ViewModelPage.ViewModel.IsMoisAcquisitionVisible = true;
                            ViewModelPage.ViewModel.IsMoisAcquisitionKnow = true;
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.IsJourAcquisitionKnow = false;
                        ViewModelPage.ViewModel.IsJourAcquisitionVisible = false;
                        ViewModelPage.ViewModel.IsMoisAcquisitionKnow = false;
                        ViewModelPage.ViewModel.IsMoisAcquisitionVisible = false;
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

        private void DP_DateAcquisition_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            try
            {
                if (args.NewDate != null)
                {
                    ViewModelPage.ViewModel.IsJourAcquisitionVisible = true;
                    ViewModelPage.ViewModel.IsMoisAcquisitionVisible = true;
                    ViewModelPage.ViewModel.IsJourAcquisitionKnow = true;
                    ViewModelPage.ViewModel.IsMoisAcquisitionKnow = true;
                }
                else
                {
                    ViewModelPage.ViewModel.IsJourAcquisitionVisible = true;
                    ViewModelPage.ViewModel.IsMoisAcquisitionVisible = true;
                    ViewModelPage.ViewModel.IsJourAcquisitionKnow = false;
                    ViewModelPage.ViewModel.IsMoisAcquisitionKnow = false;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DateParution_MenuFlyout_Opening(object sender, object e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

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

        private async void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = await IsModelValided();
                if (!isValided)
                {
                    return;
                }

                ImportDataRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }



        private async Task<bool> IsModelValided()
        {
            try
            {
                var rr = await excelServices.ImportExcelToDatatable(ViewModelPage.SelectedWorkSheetName, ViewModelPage.TableRange);
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

                if (ImportDataRequested != null)
                {
                    ImportDataRequested = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }

    public class ImportBookFromExcelUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        public Guid Guid { get; set; } = Guid.NewGuid();

        private string _Header = "Importer un fichier Excel";
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

        private string _Glyph = "\uE8B5";
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

        private ObservableCollection<string> _WorkSheetsName = new ObservableCollection<string>();
        public ObservableCollection<string> WorkSheetsName
        {
            get => this._WorkSheetsName;
            set
            {
                if (this._WorkSheetsName != value)
                {
                    this._WorkSheetsName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _SelectedWorkSheetName;
        public string SelectedWorkSheetName
        {
            get => this._SelectedWorkSheetName;
            set
            {
                if (this._SelectedWorkSheetName != value)
                {
                    this._SelectedWorkSheetName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _TableRange;
        public string TableRange
        {
            get => this._TableRange;
            set
            {
                if (this._TableRange != value)
                {
                    this._TableRange = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsTableRangeContainsHeader;
        public bool IsTableRangeContainsHeader
        {
            get => this._IsTableRangeContainsHeader;
            set
            {
                if (this._IsTableRangeContainsHeader != value)
                {
                    this._IsTableRangeContainsHeader = value;
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
