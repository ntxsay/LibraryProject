using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
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

namespace LibraryProjectUWP.Views.Contact
{
    public sealed partial class NewEditContactUC : PivotItem
    {
        public NewEditContactUCVM ViewModelPage { get; set; } = new NewEditContactUCVM();
        public ContactVM OriginalViewModel { get; private set; }
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        public SideBarInterLinkVM ParentReferences { get; set; }

        public BookCollectionPage ParentPage { get; private set; }

        public delegate void CancelModificationEventHandler(NewEditContactUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void ExecuteTaskEventHandler(NewEditContactUC sender, ContactVM originalViewModel, OperationStateVM e);
        public event ExecuteTaskEventHandler ExecuteTaskRequested;


        public NewEditContactUC()
        {
            this.InitializeComponent();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void InitializeSideBar(BookCollectionPage bookCollectionPage, EditMode editMode, ContactType? contactType = null, IEnumerable<ContactRole> contactRoles = null, ContactVM contactVM = null)
        {
            try
            {
                if (contactVM == null && editMode != EditMode.Create && contactType == null)
                {
                    return;
                }

                ParentPage = bookCollectionPage;
                this.OriginalViewModel = contactVM; //Attention de ne pas casser lien

                ViewModelPage = new NewEditContactUCVM()
                {
                    EditMode = editMode,
                };

                if (ViewModelPage.EditMode == EditMode.Create)
                {
                    ViewModelPage.ViewModel = contactVM?.DeepCopy() ?? new ContactVM();
                    ViewModelPage.ViewModel.ContactRoles = contactRoles != null && contactRoles.Any() ?  new ObservableCollection<ContactRole>(contactRoles) : new ObservableCollection<ContactRole>() { ContactRole.Adherant };
                    ViewModelPage.ViewModel.ContactType = (ContactType)contactType;
                }
                else if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    ViewModelPage.ViewModel = contactVM.DeepCopy();
                }

                InitializeActionInfos();
                InitializeFieldVisibility();
                InitializeRoleText();
                InitializeMenuFlyout();

                if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    this.Bindings.Update();
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
                TbcInfos.Inlines.Clear();

                string title = string.Empty;
                string name = string.Empty;
                if (ViewModelPage.ViewModel.ContactType == ContactType.Human)
                {
                    title = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une personne" : "d'éditer")} ";
                    name = $"{ViewModelPage?.ViewModel?.DisplayName3}";
                    ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une personne";
                    ViewModelPage.Glyph = "\ue77b";
                }
                else if (ViewModelPage.ViewModel.ContactType == ContactType.Society)
                {
                    title = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une société" : "d'éditer une société")} ";
                    name = $"{ViewModelPage?.ViewModel?.DisplayName3}";
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

                if (ViewModelPage.ViewModel.ContactRoles !=null && ViewModelPage.ViewModel.ContactRoles.Any())
                {
                    foreach (var role in ViewModelPage.ViewModel.ContactRoles)
                    {
                        switch (role)
                        {
                            case ContactRole.Adherant:
                                break;
                            case ContactRole.Author:
                                if (ViewModelPage.ViewModel.ContactType == ContactType.Human)
                                    ViewModelPage.AuthorVisibility = Visibility.Visible;
                                break;
                            case ContactRole.EditorHouse:
                                break;
                            case ContactRole.Translator:
                                break;
                            case ContactRole.Illustrator:
                                break;
                            default:
                                break;
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

        private void InitializeRoleText()
        {
            try
            {
                string rolesList = string.Empty;

                var rolesDictio = LibraryHelpers.Contact.ContactRoleDictionary;
                foreach (var _role in rolesDictio)
                {
                    if (ViewModelPage.ViewModel.ContactRoles.Any(a => (byte)a == _role.Key))
                    {
                        rolesList += $"{_role.Value}, ";
                    }
                }

                rolesList = rolesList.Trim();
                if (rolesList.EndsWith(','))
                {
                    rolesList = rolesList.Remove(rolesList.Length - 1, 1);
                }

                TbkRolesNames.Text = rolesList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InitializeMenuFlyout()
        {
            try
            {
                MenuFlyoutRoles.Items.Clear();
                var rolesDictio = LibraryHelpers.Contact.ContactRoleDictionary;
                foreach (var role in rolesDictio)
                {
                    ToggleMenuFlyoutItem toggleMenuFlyoutItemRole = new ToggleMenuFlyoutItem()
                    {
                        Text = role.Value,
                        Tag = role,
                    };
                    toggleMenuFlyoutItemRole.Click += ToggleMenuFlyoutItemRole_Click;
                    MenuFlyoutRoles.Items.Add(toggleMenuFlyoutItemRole);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MenuFlyoutRoles_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    string rolesList = string.Empty;

                    if (!ViewModelPage.ViewModel.ContactRoles.Any())
                    {
                        menuFlyout.Items.Where(w => w is ToggleMenuFlyoutItem toggleMenuFlyoutItem && toggleMenuFlyoutItem.IsChecked).Select(s => (ToggleMenuFlyoutItem)s).ToList().ForEach(d => d.IsChecked = false);
                    }
                    else
                    {
                        foreach (var item in menuFlyout.Items)
                        {
                            if (item is ToggleMenuFlyoutItem toggleMenuFlyoutItem && toggleMenuFlyoutItem.Tag is KeyValuePair<byte, string> role)
                            {
                                if (ViewModelPage.ViewModel.ContactRoles.Any(a => (byte)a == role.Key))
                                {
                                    if (!toggleMenuFlyoutItem.IsChecked)
                                    {
                                        toggleMenuFlyoutItem.IsChecked = true;
                                    }
                                    rolesList += $"{role.Value}, ";
                                }
                                else
                                {
                                    if (toggleMenuFlyoutItem.IsChecked)
                                    {
                                        toggleMenuFlyoutItem.IsChecked = false;
                                    }
                                }
                            }
                        }
                    }

                    rolesList = rolesList.Trim();
                    if (rolesList.EndsWith(','))
                    {
                        rolesList = rolesList.Remove(rolesList.Length - 1, 1);
                    }

                    TbkRolesNames.Text = rolesList;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ToggleMenuFlyoutItemRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleMenuFlyoutItem toggleMenuFlyoutItem && toggleMenuFlyoutItem.Tag is KeyValuePair<byte, string> role)
                {
                    if (toggleMenuFlyoutItem.IsChecked)
                    {
                        if (!ViewModelPage.ViewModel.ContactRoles.Any(a => (byte)a == role.Key))
                        {
                            ViewModelPage.ViewModel.ContactRoles.Add((ContactRole)role.Key);
                        }
                    }
                    else
                    {
                        if (ViewModelPage.ViewModel.ContactRoles.Any(a => (byte)a == role.Key))
                        {
                            ViewModelPage.ViewModel.ContactRoles.Remove((ContactRole)role.Key);
                        }
                    }
                }

                InitializeRoleText();
                InitializeFieldVisibility();
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

        private async void BtnExecuteAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isValided = await IsModelValided();
                if (!isValided)
                {
                    return;
                }

                OperationStateVM result = null;
                if (ViewModelPage.EditMode == EditMode.Create)
                {
                    result = await CreateAsync();
                }
                else if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    result = await UpdateAsync();
                }

                ExecuteTaskRequested?.Invoke(this, OriginalViewModel, result);
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

        private async Task<OperationStateVM> CreateAsync()
        {
            try
            {
                ContactVM viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Contact.CreateAsync(viewModel);
                if (result.IsSuccess)
                {
                    viewModel.Id = result.Id;
                    this.ViewModelPage.ResultMessageTitle = "Succès";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.ViewModelPage.IsResultMessageOpen = true;
#warning Ajouter le système afin de sauvegarder 
                    //await esBook.SaveBookViewModelAsync(OriginalViewModel);
                }
                else
                {
                    //Erreur
                    this.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.ViewModelPage.IsResultMessageOpen = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private async Task<OperationStateVM> UpdateAsync()
        {
            try
            {
                ContactVM viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Contact.UpdateAsync(viewModel);
                if (result.IsSuccess)
                {
                    this.ViewModelPage.ResultMessageTitle = "Succès";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.ViewModelPage.IsResultMessageOpen = true;
#warning Ajouter le système afin de sauvegarder 
                    //await esBook.SaveBookViewModelAsync(OriginalViewModel);
                }
                else
                {
                    //Erreur
                    this.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.ViewModelPage.IsResultMessageOpen = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
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

                if (ExecuteTaskRequested != null)
                {
                    ExecuteTaskRequested = null;
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
