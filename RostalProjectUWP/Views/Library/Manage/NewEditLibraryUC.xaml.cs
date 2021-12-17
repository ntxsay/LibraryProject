using RostalProjectUWP.Code;
using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels.General;
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

namespace RostalProjectUWP.Views.Library.Manage
{
    public sealed partial class NewEditLibraryUC : UserControl
    {
        public readonly ManageLibraryDialogParametersVM _parameters;
        
        public NewEditLibraryUCVM ViewModelPage { get; set; } = new NewEditLibraryUCVM();

        public delegate void CancelModificationEventHandler(NewEditLibraryUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditLibraryUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditLibraryUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public NewEditLibraryUC()
        {
            this.InitializeComponent();
        }

        public NewEditLibraryUC(ManageLibraryDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} une bibliothèque";
            ViewModelPage.NamePlaceHolderText = "Nom de la bibliothèque";
            ViewModelPage.DescriptionPlaceHolderText = "Description facultative de la bibliothèque";
            ViewModelPage.Value = parameters?.CurrentLibrary?.Name;
            ViewModelPage.Description = parameters?.CurrentLibrary?.Description;
            InitializeActionInfos();
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
                
                if (_parameters != null)
                {
                    if (ViewModelPage.EditMode == EditMode.Edit)
                    {
                        Run runCategorie = new Run()
                        {
                            Text = " " + _parameters?.CurrentLibrary?.Name,
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
                if (ViewModelPage.Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ErrorMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
                    return false;
                }

                if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                {
                    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentLibrary?.Name?.Trim().ToLower() == ViewModelPage.Value?.Trim().ToLower());
                    if (isError)
                    {
                        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                        return false;
                    }
                }

                ViewModelPage.ErrorMessage = string.Empty;
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
    }

    public class NewEditLibraryUCVM : INotifyPropertyChanged
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

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => this._ErrorMessage;
            set
            {
                if (this._ErrorMessage != value)
                {
                    this._ErrorMessage = value;
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

        private string _Value;
        public string Value
        {
            get => this._Value;
            set
            {
                if (this._Value != value)
                {
                    this._Value = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Description;
        public string Description
        {
            get => this._Description;
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _NamePlaceHolderText;
        public string NamePlaceHolderText
        {
            get => this._NamePlaceHolderText;
            set
            {
                if (this._NamePlaceHolderText != value)
                {
                    this._NamePlaceHolderText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _DescriptionPlaceHolderText;
        public string DescriptionPlaceHolderText
        {
            get => this._DescriptionPlaceHolderText;
            set
            {
                if (this._DescriptionPlaceHolderText != value)
                {
                    this._DescriptionPlaceHolderText = value;
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
