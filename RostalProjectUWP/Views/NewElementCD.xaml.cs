using RostalProjectUWP.Code;
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

namespace RostalProjectUWP.Views
{
    public sealed partial class NewElementCD : ContentDialog
    {
        public ManagePage ManagePage { get; set; }
        public NewElementCD()
        {
            this.InitializeComponent();
            ManagePage = ManagePage.None;
            this.PrimaryButtonText = "Faites un choix";
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ListView listView)
                {
                    if (listView.SelectedIndex == -1)
                    {
                        ManagePage = ManagePage.None;
                        this.PrimaryButtonText = "Faites un choix";
                    }
                    else if (listView.SelectedIndex == 0)
                    {
                        ManagePage = ManagePage.Library;
                        this.PrimaryButtonText = "Ouvrir";
                    }
                    else if (listView.SelectedIndex == 1)
                    {
                        ManagePage = ManagePage.Book;
                        this.PrimaryButtonText = "Créer";
                    }
                    else if (listView.SelectedIndex == 2)
                    {
                        ManagePage = ManagePage.Contacts;
                        this.PrimaryButtonText = "Créer";
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (ManagePage == ManagePage.None)
                {
                    args.Cancel = true;
                    return;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        
    }
}
