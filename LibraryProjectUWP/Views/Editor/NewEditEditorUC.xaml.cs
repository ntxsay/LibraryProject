using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Publishers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace LibraryProjectUWP.Views.Editor
{
    public sealed partial class NewEditEditorUC : PivotItem
    {
        public readonly ManageEditorParametersDriverVM _parameters;

        public NewEditEditorUCVM ViewModelPage { get; set; } = new NewEditEditorUCVM();

        public delegate void CancelModificationEventHandler(NewEditEditorUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditEditorUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditEditorUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public NewEditEditorUC()
        {
            this.InitializeComponent();
        }

        public NewEditEditorUC(ManageEditorParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} un éditeur";
            ViewModelPage.ViewModel = parameters?.CurrentViewModel;
            InitializeActionInfos();
        }

        private void InitializeActionInfos()
        {
            try
            {
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter un " : "d'éditer l'")}éditeur",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (_parameters != null)
                {
                    if (ViewModelPage.EditMode == EditMode.Edit)
                    {
                        Run runCategorie = new Run()
                        {
                            Text = " " + _parameters?.CurrentViewModel?.Name,
                            FontWeight = FontWeights.Medium,
                        };
                        TbcInfos.Inlines.Add(runCategorie);
                    }
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
                if (ViewModelPage.ViewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessage = $"Le nom de l'éditeur ne peut pas être vide\nou ne contenir que des espaces blancs.";
                    ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                    return false;
                }

                if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.ViewModel.Name.Trim().ToLower()))
                {
                    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentViewModel?.Name?.Trim().ToLower() == ViewModelPage.ViewModel.Name?.Trim().ToLower());
                    if (isError)
                    {
                        ViewModelPage.ResultMessage = $"Cet éditeur existe déjà.";
                        ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                        return false;
                    }
                }

                ViewModelPage.ResultMessage = string.Empty;
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

    public class NewEditEditorUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        private string _ArgName;
        public string ArgName
        {
            get => this._ArgName;
            set
            {
                if (this._ArgName != value)
                {
                    this._ArgName = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private PublisherVM _ViewModel;
        public PublisherVM ViewModel
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
