using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.PrincipalPages;
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

namespace LibraryProjectUWP.Views.Common
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ImportItemsFromTablePage : Page
    {
        public DataTable DataTable { get; private set; }
        public BookCollectionPage BookCollectionPage { get; private set; }
        public IList<object> SelectedItems { get; set; } = new List<object>();
        public ImportItemsFromTablePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Tuple<BookCollectionPage, DataTable, IEnumerable<BibliothequeVM>> parameters)
            {
                BookCollectionPage = parameters.Item1;
                DataTable = parameters.Item2;
                Title = "Importer des bibliothèques depuis un fichier";
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataTable != null && DataTable.Columns.Count > 0)
                {
                    for (int i = 0; i < DataTable.Columns.Count; i++)
                    {
                        DataGridItems.Columns.Add(new DataGridTextColumn()
                        {
                            Header = DataTable.Columns[i].ToString(),
                            Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") },
                            IsReadOnly = i == 0,
                        });
                    }

                    var collection = new ObservableCollection<object>();
                    foreach (DataRow row in DataTable.Rows)
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
                if (sender is DataGrid dataGrid && dataGrid.SelectedItems.Count > 0)
                {
                    this.SelectedItems = new List<object>(dataGrid.SelectedItems.Cast<object>());
                    var sideBar = BookCollectionPage.GetImportItemsFromFileSideBar();
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (BookCollectionPage != null)
            {
                if (BookCollectionPage.Parameters.ParentLibrary == null)
                {
                    BookCollectionPage.OpenLibraryCollection();
                }
                else
                {
                    BookCollectionPage.OpenBookCollection(BookCollectionPage.Parameters.ParentLibrary);
                }
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
                                                                typeof(ImportItemsFromTablePage), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImportItemsFromTablePage parent && e.NewValue is string title)
            {
                parent.header.Title = title.Trim();
            }
        }

    }
}
