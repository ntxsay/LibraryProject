using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Book.Manage
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageBookCategorie : Page
    {
        private ManageBookPage _parentPage;
        private LivreVM ViewModel { get; set; }

        public ManageBookCategorie()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ManageBookParentChildVM parameters)
            {
                ViewModel = parameters.ViewModel;
                _parentPage = parameters.ParentPage;
                if (ViewModel != null)
                {
                    ViewModel.Categories.Add(new CategorieLivreVM()
                    {
                        Name = "dddddddddddddd",
                        SubCategorieLivres = new ObservableCollection<CategorieLivreVM>()
                        {
                            new CategorieLivreVM() { Name = "ggggg"}
                        }
                    });
                }
                
            }
        }

        private async void AddNewCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                string newCategorie = string.Empty;
                var dialog = new NewCategorieCD(newCategorie, false);

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    newCategorie = dialog.Tag?.ToString();
                   ViewModel.Categories.Add(new CategorieLivreVM() 
                   { 
                       Name = newCategorie, 
                       SubCategorieLivres = new ObservableCollection<CategorieLivreVM>()
                   });
                }
                else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private void AddNewSubCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }
    }
}
