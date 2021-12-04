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
            Title = parameters.Type == Code.CategorieType.SubCategorie ? $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Renommer")} une sous-catégorie" : $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Renommer")} une catégorie";
            ArgName = parameters.Type == Code.CategorieType.SubCategorie ? "sous-catégorie" : "catégorie";
            Value = parameters.Value;
            Description = parameters.Description;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    TbxErrorMessage.Text = $"Le nom de la {(_parameters.Type == Code.CategorieType.SubCategorie ? "sous-catégorie" : "catégorie")} ne peut pas être vide\nou ne contenir que des espaces blancs.";
                    args.Cancel = true;
                    return;
                }

                if (_parameters.Type == Code.CategorieType.SubCategorie && !_parameters.ParentName.IsStringNullOrEmptyOrWhiteSpace() && _parameters.ParentName.ToLower() == Value.Trim().ToLower())
                {
                    TbxErrorMessage.Text = $"Le nom de la {(_parameters.Type == Code.CategorieType.SubCategorie ? "sous-catégorie" : "catégorie")} ne peut pas avoir \nle même nom que son parent.";
                    args.Cancel = true;
                    return;
                }

                if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == Value.Trim().ToLower()))
                {
                    TbxErrorMessage.Text = $"Cette {(_parameters.Type == Code.CategorieType.SubCategorie ? "sous-catégorie" : "catégorie")} existe déjà.";
                    args.Cancel = true;
                    return;
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
