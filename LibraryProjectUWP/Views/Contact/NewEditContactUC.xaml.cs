using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
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

namespace LibraryProjectUWP.Views.Contact
{
    public sealed partial class NewEditContactUC : PivotItem
    {
        public NewEditContactUCVM ViewModelPage { get; set; } = new NewEditContactUCVM();
        public BookCollectionPage ParentPage { get; private set; }

        public delegate void CancelModificationEventHandler(NewEditContactUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditContactUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditContactUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public NewEditContactUC()
        {
            this.InitializeComponent();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            //InitializeActionInfos();
            //InitializeFieldVisibility();
        }

        public void InitializeSideBar(BookCollectionPage bookCollectionPage, EditMode editMode, ContactType contactType, ContactRole contactRole, string prenom = null, string nomNaissance = null, string societyName = null)
        {
            try
            {
                ParentPage = bookCollectionPage;
                ViewModelPage = new NewEditContactUCVM()
                {
                    EditMode = editMode,
                    ViewModel = new ContactVM()
                    {
                        ContactRole = contactRole,
                        ContactType = contactType,
                        NomNaissance = nomNaissance,
                        Prenom = prenom,
                        SocietyName = societyName,
                    },
                };

                ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Prêter un livre" : "Editer un livre")}";

                InitializeActionInfos();
                InitializeFieldVisibility();
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
                string title = string.Empty;
                string name = string.Empty;
                if (ViewModelPage.ViewModel.ContactType == ContactType.Human)
                {
                    title = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une personne" : "d'éditer")}";
                    name = $"{ViewModelPage?.ViewModel?.TitreCivilite} {ViewModelPage?.ViewModel?.NomNaissance} {ViewModelPage?.ViewModel?.Prenom}";
                    ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une personne";
                    ViewModelPage.Glyph = "\ue77b";
                }
                else if (ViewModelPage.ViewModel.ContactType == ContactType.Society)
                {
                    title = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une société" : "d'éditer une société")}";
                    name = $"{ViewModelPage?.ViewModel?.SocietyName}";
                    ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une société";
                    ViewModelPage.Glyph = "\uE731";
                }

                Run runTitle = new Run()
                {
                    Text = title,
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    Run runName = new Run()
                    {
                        Text = name,
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

        private void InitializeFieldVisibility()
        {
            try
            {
                if (ViewModelPage.ViewModel.ContactType == ContactType.Human)
                {
                    ViewModelPage.AdressVisibility = Visibility.Visible;
                    ViewModelPage.CivilityVisibility = Visibility.Visible;
                    ViewModelPage.SocietyNameVisibility = Visibility.Collapsed;
                    ViewModelPage.AuthorVisibility = Visibility.Collapsed;
                }
                else if (ViewModelPage.ViewModel.ContactType == ContactType.Society)
                {
                    ViewModelPage.SocietyNameVisibility = Visibility.Visible;
                    ViewModelPage.AdressVisibility = Visibility.Visible;
                    ViewModelPage.AuthorVisibility = Visibility.Collapsed;
                    ViewModelPage.CivilityVisibility = Visibility.Collapsed;
                }

                if (ViewModelPage.ViewModel.ContactRole == ContactRole.Adherant)
                {
                    ViewModelPage.AuthorVisibility = Visibility.Collapsed;
                }
                else if (ViewModelPage.ViewModel.ContactRole == ContactRole.EditorHouse)
                {
                   // ViewModelPage.SocietyNameVisibility = Visibility.Visible;
                }
                else if (ViewModelPage.ViewModel.ContactRole == ContactRole.Author)
                {
                    ViewModelPage.AdressVisibility = Visibility.Visible;
                    ViewModelPage.AuthorVisibility = Visibility.Visible;
                }

                var isExist = LibraryHelpers.Contact.ContactRoleDictionary.TryGetValue((byte)ViewModelPage.ViewModel.ContactRole, out string value);
                if (isExist && (cmbxContactRole.SelectedItem == null || cmbxContactRole.SelectedItem is string selectedVal && selectedVal != value))
                {
                    cmbxContactRole.SelectedItem = value;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void CmbxContactRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox && comboBox.SelectedItem is string type)
                {
                    var typeKeyPair = LibraryHelpers.Contact.ContactRoleDictionary.SingleOrDefault(s => s.Value == type);
                    if (!typeKeyPair.Equals(default(KeyValuePair<byte, string>)))
                    {
                        ViewModelPage.ViewModel.ContactRole = (ContactRole)typeKeyPair.Key;
                        InitializeFieldVisibility();
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

        private void MfiClearNaissanceDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.DateNaissance != null)
                {
                    ViewModelPage.ViewModel.DateNaissance = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void MfiClearDeathDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.DateDeces != null)
                {
                    ViewModelPage.ViewModel.DateDeces = null;
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

                CreateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = await IsModelValided();
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


        private async Task<bool> IsModelValided()
        {
            try
            {
                if (ViewModelPage.ViewModel.ContactType == ContactType.Human)
                {
                    ViewModelPage.ViewModel.SocietyName = String.Empty;
                    if (ViewModelPage.ViewModel.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                        ViewModelPage.ResultMessage = $"Le nom du contact ne peut pas être vide\nou ne contenir que des espaces blancs.";
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                        ViewModelPage.IsResultMessageOpen = true;
                        return false;
                    }
                }
                else if (ViewModelPage.ViewModel.ContactType == ContactType.Society)
                {
                    ViewModelPage.ViewModel.TitreCivilite = string.Empty;
                    ViewModelPage.ViewModel.NomNaissance = String.Empty;
                    ViewModelPage.ViewModel.Prenom = String.Empty;

                    if (ViewModelPage.ViewModel.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                        ViewModelPage.ResultMessage = $"Le nom de la société ne peut pas être vide\nou ne contenir que des espaces blancs.";
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                        ViewModelPage.IsResultMessageOpen = true;
                        return false;
                    }
                }

                var isExist = await DbServices.Contact.IsContactExistAsync(ViewModelPage.ViewModel, ViewModelPage.EditMode == EditMode.Edit, ViewModelPage.ViewModel.Id);
                if (isExist.Item1)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Ce contact existe déjà.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
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

    public class NewEditContactUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        public SideBarInterLinkVM ParentReferences { get; set; }

        public readonly IEnumerable<string> contactRole = LibraryHelpers.Contact.ContactRoleList;
        public readonly IEnumerable<string> nationalityList = CountryHelpers.NationalitiesList();
        public readonly IEnumerable<string> civilityList = CivilityHelpers.CiviliteListShorted();

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

        private string _Glyph = "\ue77b";
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

        private Visibility _ContactTypeVisibility = Visibility.Collapsed;
        public Visibility ContactTypeVisibility
        {
            get => this._ContactTypeVisibility;
            set
            {
                if (this._ContactTypeVisibility != value)
                {
                    this._ContactTypeVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _SocietyNameVisibility = Visibility.Collapsed;
        public Visibility SocietyNameVisibility
        {
            get => this._SocietyNameVisibility;
            set
            {
                if (this._SocietyNameVisibility != value)
                {
                    this._SocietyNameVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _AdressVisibility = Visibility.Collapsed;
        public Visibility AdressVisibility
        {
            get => this._AdressVisibility;
            set
            {
                if (this._AdressVisibility != value)
                {
                    this._AdressVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _AuthorVisibility = Visibility.Collapsed;
        public Visibility AuthorVisibility
        {
            get => this._AuthorVisibility;
            set
            {
                if (this._AuthorVisibility != value)
                {
                    this._AuthorVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _CivilityVisibility = Visibility.Collapsed;
        public Visibility CivilityVisibility
        {
            get => this._CivilityVisibility;
            set
            {
                if (this._CivilityVisibility != value)
                {
                    this._CivilityVisibility = value;
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


        private ContactVM _ViewModel;
        public ContactVM ViewModel
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
