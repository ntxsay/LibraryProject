using RostalProjectUWP.Code.Helpers;
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
        private readonly bool IsSubCategorie;
        //private string ArgTitle { get; set; }
        private string ArgName { get; set; }
        private string Value { get; set; }
        public NewCategorieCD()
        {
            this.InitializeComponent();
        }

        public NewCategorieCD(string value, bool isSubCategorie = false)
        {
            this.InitializeComponent();
            IsSubCategorie = isSubCategorie;
            Title = isSubCategorie ? "Ajouter une sous-catégorie" : "Ajouter une catégorie";
            ArgName = isSubCategorie ? "sous-catégorie" : "catégorie";
            Value = value;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    args.Cancel = true;
                    return;
                }

                sender.Tag = Value.Trim();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
