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
        private ManagePage ManagePage { get; set; }
        public NewElementCD()
        {
            this.InitializeComponent();
        }

        public NewElementCD(ManagePage managePage)
        {
            this.InitializeComponent();
            ManagePage = managePage;
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
                    }
                    else if (listView.SelectedIndex == 0)
                    {
                        ManagePage = ManagePage.Library;
                    }
                    else if (listView.SelectedIndex == 1)
                    {
                        ManagePage = ManagePage.Book;
                    }
                    else if (listView.SelectedIndex == 2)
                    {
                        ManagePage = ManagePage.Contacts;
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
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        
    }
}
