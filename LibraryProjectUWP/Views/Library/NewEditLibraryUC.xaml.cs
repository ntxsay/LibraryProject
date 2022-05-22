using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
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

namespace LibraryProjectUWP.Views.Library.Manage
{
    public sealed partial class NewEditLibraryUC : PivotItem
    {
        public BookCollectionPage ParentPage { get; private set; }
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();

        public BibliothequeVM OriginalViewModel { get; private set; }
        readonly EsLibrary eslibrary = new EsLibrary();
        public NewEditLibraryUCVM ViewModelPage { get; set; } = new NewEditLibraryUCVM();

        public delegate void CancelModificationEventHandler(NewEditLibraryUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void ExecuteTaskEventHandler(NewEditLibraryUC sender, BibliothequeVM originalViewModel, OperationStateVM e);
        public event ExecuteTaskEventHandler ExecuteTaskRequested;

        public NewEditLibraryUC()
        {
            this.InitializeComponent();
        }

        public void InitializeSideBar(BookCollectionPage bookCollectionPage, BibliothequeVM bibliothequeVM, EditMode editMode)
        {
            try
            {
                if (bibliothequeVM == null && editMode != EditMode.Create)
                {
                    return;
                }

                ParentPage = bookCollectionPage;
                this.OriginalViewModel = bibliothequeVM; //Attention de ne pas casser lien

                ViewModelPage = new NewEditLibraryUCVM()
                {
                    EditMode = editMode,
                };

                ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une bibliothèque";

                if (ViewModelPage.EditMode == EditMode.Create)
                {
                    ViewModelPage.ViewModel = bibliothequeVM?.DeepCopy() ?? new BibliothequeVM();
                }
                else if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    ViewModelPage.ViewModel = bibliothequeVM.DeepCopy();
                }
                InitializeActionInfos();

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
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une nouvelle" : "d'éditer la")} bibliothèque",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    Run runCategorie = new Run()
                    {
                        Text = " " + OriginalViewModel.Name,
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runCategorie);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                var isModificationStateChecked = await this.CheckModificationsStateAsync();
                if (isModificationStateChecked)
                {
                    CancelModificationRequested?.Invoke(this, args);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void BtnExecuteAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isValided = IsModelValided();
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

        public async Task<bool> CheckModificationsStateAsync()
        {
            try
            {
                var viewModelsEqual = BookHelpers.GetPropertiesChanged(OriginalViewModel, ViewModelPage.ViewModel);
                if (viewModelsEqual.Any())
                {
                    var dialog = new CheckModificationsStateCD(OriginalViewModel, viewModelsEqual)
                    {
                        Title = "Enregistrer vos modifications"
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        OperationStateVM operationResult = null;
                        if (ViewModelPage.EditMode == EditMode.Create)
                        {
                            operationResult = await CreateAsync();
                        }
                        else if (ViewModelPage.EditMode == EditMode.Edit)
                        {
                            operationResult = await UpdateAsync();
                        }

                        return operationResult.IsSuccess;
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return true;
            }
        }

        private bool IsModelValided()
        {
            try
            {
                if (ViewModelPage.ViewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
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
                BibliothequeVM viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Library.CreateAsync(viewModel);
                if (result.IsSuccess)
                {
                    viewModel.Id = result.Id;
                    this.ViewModelPage.ResultMessageTitle = "Succès";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.ViewModelPage.IsResultMessageOpen = true;
                    await eslibrary.SaveLibraryViewModelAsync(OriginalViewModel);
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
                BibliothequeVM viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Library.UpdateAsync(viewModel);
                if (result.IsSuccess)
                {
                    OriginalViewModel.DeepCopy(viewModel);
                    await eslibrary.SaveLibraryViewModelAsync(OriginalViewModel);

                    this.ViewModelPage.ResultMessageTitle = "Succès";
                    this.ViewModelPage.ResultMessage = result.Message;
                    this.ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.ViewModelPage.IsResultMessageOpen = true;
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

    public class NewEditLibraryUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
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

        private string _Glyph = "\uE8F1";
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

        private BibliothequeVM _ViewModel;
        public BibliothequeVM ViewModel
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
