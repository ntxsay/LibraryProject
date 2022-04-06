using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
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

namespace LibraryProjectUWP.Views.Book.SubViews
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ImportBookFileSubPage : Page
    {
        public ImportBookFileSubPage()
        {
            this.InitializeComponent();
        }

        public BookSubPageParametersDriverVM ParametersDriverVM { get; private set; }
        public IList<LivreVM> SelectedItems { get; set; } = new List<LivreVM>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookSubPageParametersDriverVM parameters)
            {
                ParametersDriverVM = parameters;
                //InitializeDataAsync(true);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridItems_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    this.SelectedItems = dataGrid.SelectedItems.Cast<LivreVM>().ToList();
                    var sideBar = ParametersDriverVM.ParentPage.GetImportBookFromFileUC();
                    if (sideBar != null)
                    {
                        sideBar.InitializeText();
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
    }
}
