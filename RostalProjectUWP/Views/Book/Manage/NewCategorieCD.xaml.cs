using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.ViewModels;
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

namespace RostalProjectUWP.Views.Book.Manage
{
    public sealed partial class NewCategorieCD : ContentDialog
    {
        private readonly ManageCategorieDialogParametersVM _parameters;
        private readonly ManageSubCategorieDialogParametersVM _subparameters;
        public string ErrorMessage { get; set; }
        private string ArgName { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public NewCategorieCD()
        {
            this.InitializeComponent();
        }

        public NewCategorieCD(ManageCategorieDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            Title =  $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} une catégorie";
            this.PrimaryButtonText = (parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Mettre à jour");
            ArgName = "catégorie";
            Value = parameters.Value;
            Description = parameters.Description;
        }

        public NewCategorieCD(ManageSubCategorieDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _subparameters = parameters;
            Title = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} une sous-catégorie";
            this.PrimaryButtonText = (parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Mettre à jour");
            ArgName = "sous-catégorie";
            Value = parameters.Value;
            Description = parameters.Description;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    TbxErrorMessage.Text = $"Le nom de la {(_subparameters != null ? "sous-catégorie" : "catégorie")} ne peut pas être vide\nou ne contenir que des espaces blancs.";
                    args.Cancel = true;
                    return;
                }

                if (_subparameters != null && !_subparameters.Categorie.Name.IsStringNullOrEmptyOrWhiteSpace() && _subparameters.Categorie.Name.ToLower() == Value.Trim().ToLower())
                {
                    TbxErrorMessage.Text = $"Le nom de la sous-catégorie ne peut pas avoir \nle même nom que son parent.";
                    args.Cancel = true;
                    return;
                }

                if (_parameters != null)
                {
                    if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == Value.Trim().ToLower()))
                    {
                        TbxErrorMessage.Text = $"Cette catégorie existe déjà.";
                        args.Cancel = true;
                        return;
                    }
                }
                else if (_subparameters != null)
                {
                    if (_subparameters.ViewModelList != null && _subparameters.ViewModelList.Any(a => a.Name.ToLower() == Value.Trim().ToLower()))
                    {
                        TbxErrorMessage.Text = $"Cette sous-catégorie existe déjà.";
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
