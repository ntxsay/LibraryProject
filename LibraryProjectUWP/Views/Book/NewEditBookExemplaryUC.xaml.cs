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
                if (ViewModelPage.ViewModel.DateAcquisition == null)
                {
                    BtnDateAcquisition.Flyout.Hide();
                    MyTeachingTip.Target = BtnDateAcquisition;
                    MyTeachingTip.Title = "Date de parution";
                    MyTeachingTip.Subtitle = "Sélectionnez tout d'abord une date puis cliquez de nouveau sur ce bouton.";
                    MyTeachingTip.IsOpen = true;

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

                //else if (ViewModelPage.ViewModel.NomNaissance.IsStringNullOrEmptyOrWhiteSpace())
                //{
                //    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                //    ViewModelPage.ResultMessage = $"Le nom de naissance de l'auteur ne peut pas être vide ou ne contenir que des espaces blancs.";
                //    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                //    ViewModelPage.IsResultMessageOpen = true;
                //    return false;
                //}
                //else if (ViewModelPage.ViewModel.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                //{
                //    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                //    ViewModelPage.ResultMessage = $"Le prénom de l'auteur ne peut pas être vide ou ne contenir que des espaces blancs.";
                //    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                //    ViewModelPage.IsResultMessageOpen = true;
                //    return false;
                //}

                //if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(c => c.TitreCivilite.ToLower() == ViewModelPage.ViewModel.TitreCivilite.Trim().ToLower() && c.NomNaissance.ToLower() == ViewModelPage.ViewModel.NomNaissance.Trim().ToLower() && c.Prenom.ToLower() == ViewModelPage.ViewModel.Prenom.Trim().ToLower() &&
                //                                                  c.NomUsage.ToLower() == ViewModelPage.ViewModel.NomUsage.Trim().ToLower() && c.AutresPrenoms.ToLower() == ViewModelPage.ViewModel.AutresPrenoms.Trim().ToLower()))
                //{
                //    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentViewModel.TitreCivilite.ToLower() == ViewModelPage.ViewModel.TitreCivilite.Trim().ToLower() && _parameters.CurrentViewModel.NomNaissance.ToLower() == ViewModelPage.ViewModel.NomNaissance.Trim().ToLower() && _parameters.CurrentViewModel.Prenom.ToLower() == ViewModelPage.ViewModel.Prenom.Trim().ToLower() &&
                //                                                  _parameters.CurrentViewModel.NomUsage.ToLower() == ViewModelPage.ViewModel.NomUsage.Trim().ToLower() && _parameters.CurrentViewModel.AutresPrenoms.ToLower() == ViewModelPage.ViewModel.AutresPrenoms.Trim().ToLower());
                //    if (isError)
                //    {
                //        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                //        ViewModelPage.ResultMessage = $"Cet auteur existe déjà.";
                //        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                //        ViewModelPage.IsResultMessageOpen = true;
                //        return false;
                //    }
                //}

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
