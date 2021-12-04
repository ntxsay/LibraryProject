using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Library.Manage
{
    public sealed partial class NewLibraryCD : ContentDialog
    {
        private readonly ManageLibraryDialogParametersVM _parameters;
        public string Value { get; set; }
        public string Description { get; set; }
        public NewLibraryCD()
        {
            this.InitializeComponent();
        }

        public NewLibraryCD(ManageLibraryDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            Title = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Renommer")} une bibliothèque";
            Value = parameters.Value;
            Description = parameters.Description;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    TbxErrorMessage.Text = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
                    args.Cancel = true;
                    return;
                }

                if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == Value.Trim().ToLower()))
                {
                    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.Value?.Trim().ToLower() == Value.Trim().ToLower());
                    if (isError)
                    {
                        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                        args.Cancel = true;
                        return;
                    }
                }

                TbxErrorMessage.Text = string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
