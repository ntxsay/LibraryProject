using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.UI;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.PrincipalPages;
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

namespace LibraryProjectUWP.Views.Collection
{
    public sealed partial class NewEditCollectionUC : PivotItem
    {
        public NewEditCollectionUCVM ViewModelPage { get; set; } = new NewEditCollectionUCVM();
        public BookCollectionPage ParentPage { get; private set; }
        public CollectionVM OriginalViewModel { get; private set; }

        public delegate void CancelModificationEventHandler(NewEditCollectionUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void ExecuteTaskEventHandler(NewEditCollectionUC sender, CollectionVM originalViewModel, OperationStateVM e);
        public event ExecuteTaskEventHandler ExecuteTaskRequested;


        public NewEditCollectionUC()
        {
            this.InitializeComponent();
        }

        public void InitializeSideBar(long idLibrary, BookCollectionPage bookCollectionPage, CollectionVM viewModel, EditMode editMode, SideBarInterLinkVM parentReferences)
        {
            try
            {
                if (viewModel == null && editMode != EditMode.Create)
                {
                    return;
                }

                ParentPage = bookCollectionPage;
                this.OriginalViewModel = viewModel;

                ViewModelPage = new NewEditCollectionUCVM()
                {
                    EditMode = editMode,
                    IdLibrary = idLibrary,
                    ParentReferences = parentReferences,
                };

                ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une collection";
                if (ViewModelPage.EditMode == EditMode.Create)
                {
                    ViewModelPage.ViewModel = viewModel ?? new CollectionVM()
                    {
                        IdLibrary = idLibrary,
                    };
                }
                else if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    ViewModelPage.ViewModel = viewModel.DeepCopy();
                }

                InitializeActionInfos();

                if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    this.Bindings.Update();
                }

                //InitializeViewModelList();
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
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une " : "d'éditer la")} collection",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (ViewModelPage.EditMode == EditMode.Edit)
                {
                    Run runtitle = new Run()
                    {
                        Text = " " + OriginalViewModel.Name,
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runtitle);
                }
            }
            catch (Exception)
            {

                throw;
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

        private async Task<OperationStateVM> CreateAsync()
        {
            try
            {
                CollectionVM viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Collection.CreateAsync(viewModel, ViewModelPage.IdLibrary);
                if (result.IsSuccess)
                {
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

        private async Task<OperationStateVM> UpdateAsync()
        {
            try
            {
                var viewModel = this.ViewModelPage.ViewModel;

                OperationStateVM result = await DbServices.Collection.UpdateAsync(viewModel);
                if (result.IsSuccess)
                {
                    OriginalViewModel.Copy(this.ViewModelPage.ViewModel);

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


        private bool IsModelValided()
        {
            try
            {
                if (ViewModelPage.ViewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Le nom de la collection ne peut pas être vide ou ne contenir que des espaces blancs.";
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

    public class NewEditCollectionUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public long IdLibrary { get; set; }
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

        private string _Glyph = "\uE81E";
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

        private CollectionVM _ViewModel;
        public CollectionVM ViewModel
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
