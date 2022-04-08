using LibraryProjectUWP.Code.Services.Logging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    public sealed partial class ImportBookExcelSubPage : Page
    {
        public BookCollectionPage ParentPage { get; private set; }
        public ImportBookExcelSubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookCollectionPage parameters)
            {
                ParentPage = parameters;
                //InitializeDataAsync(true);
            }
        }

        private void DataGridItems_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ParentPage.ViewModelPage.DataTable != null && ParentPage.ViewModelPage.DataTable.Columns.Count > 0)
                {
                    for (int i = 0; i < ParentPage.ViewModelPage.DataTable.Columns.Count; i++)
                    {
                        DataGridItems.Columns.Add(new DataGridTextColumn()
                        {
                            Header = ParentPage.ViewModelPage.DataTable.Columns[i].ToString(),
                            Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") },
                            IsReadOnly = i == 0,
                        });
                    }

                    var collection = new ObservableCollection<object>();
                    foreach (DataRow row in ParentPage.ViewModelPage.DataTable.Rows)
                    {
                        collection.Add(row.ItemArray);
                    }

                    DataGridItems.ItemsSource = collection;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    this.SelectedItems = dataGrid.SelectedItems.Cast<object>().ToList();
                    var sideBar = ParentPage.GetImportBookFromExcelUC();
                    if (sideBar != null)
                    {
                        if (dataGrid.SelectedItems.Count > 0)
                        {
                            if (dataGrid.SelectedItems[0] is object[] row)
                            {
                                List<object> list = new List<object>();
                                for (int i = 1; i < row.Length; i++)
                                {
                                    list.Add(row[i]);
                                }
                                sideBar.SearchingResult(list.ToArray());
                            }
                        }
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

        public IList<object> SelectedItems { get; set; } = new List<object>();
        
    }
}
