using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Excel;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class ImportBookFromExcelUC : PivotItem
    {
        private SyncfusionXlsServices syncfusionXlsServices;

        public ImportBookFromExcelUCVM ViewModelPivotItem { get; set; } = new ImportBookFromExcelUCVM();

        public delegate void CancelModificationEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void ImportDataEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event ImportDataEventHandler ImportDataRequested;
        
        public ImportBookFromExcelUC()
        {
            this.InitializeComponent();
        }

        private async void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        private async void Hyperlink_PickAnotherFile_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var storageFile = await Files.OpenStorageFileAsync(Files.ExcelExtensions);
                if (storageFile == null)
                {
                    Logs.Log(m, $"Vous devez sélectionner un fichier de type Microsoft Excel.");
                    return;
                }

                ViewModelPivotItem.ParentPage.ImportBookFromExcelFile(storageFile);
                ViewModelPivotItem.FileStorage = storageFile;
                syncfusionXlsServices = new SyncfusionXlsServices(storageFile);
                await InitializeDataAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                syncfusionXlsServices = new SyncfusionXlsServices(ViewModelPivotItem.FileStorage);
                var worksheetsName = await syncfusionXlsServices.GetExcelSheetsName();
                ViewModelPivotItem.WorkSheetsName = new ObservableCollection<string>(worksheetsName);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox && comboBox.SelectedItem is string workSheetName)
                {
                    var rr = await syncfusionXlsServices.ImportExcelToDatatable(workSheetName);
                    ViewModelPivotItem.ParentPage.ViewModelPage.DataTable = rr;
                    ViewModelPivotItem.ParentPage.OpenImportBookFromExcel();
                    SearchingResult();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Btn_RemoveSelectedData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Parent is Grid parentGrid)
                {
                    if (parentGrid.Children.FirstOrDefault(f => f is ComboBox) is ComboBox comboBox)
                    {
                        comboBox.SelectedItem = null;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SearchingResult(object[] row = null)
        {
            try
            {
                TbcAfterSearching.Inlines.Clear();
                if (row == null || row.Length == 0)
                {
                    if (ViewModelPivotItem.ItemsVisibility != Visibility.Collapsed)
                    {
                        ViewModelPivotItem.ItemsVisibility = Visibility.Collapsed;
                    }

                    Run lineCount1 = new Run()
                    {
                        Text = $"Sélectionnez au moins une ligne pour préparer l'importation de vos livres.",
                        FontWeight = FontWeights.Medium,
                    };
                    TbcAfterSearching.Inlines.Add(lineCount1);
                    return;
                }


                Run lineCount = new Run()
                {
                    Text = $"{ViewModelPivotItem.ParentPage.ImportBookExcelSubPage.SelectedItems.Count} {(ViewModelPivotItem.ParentPage.ImportBookExcelSubPage.SelectedItems.Count > 1 ? "lignes ont été sélectionnées" : "ligne a été sélectionnée")}. ",
                    FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(lineCount);
                
                Run runTitle = new Run()
                {
                    Text = $"La première ligne a été sélectionnée afin de nous aider à importer vos livres.",
                    //FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(runTitle);

                ViewModelPivotItem.Titles.Clear();
                ViewModelPivotItem.Auteurs.Clear();
                ViewModelPivotItem.MaisonsEdition.Clear();
                ViewModelPivotItem.Langues.Clear();
                ViewModelPivotItem.DateParution.Clear();
                ViewModelPivotItem.Formats.Clear();
                ViewModelPivotItem.NumberOfPages.Clear();
                ViewModelPivotItem.Collections.Clear();
                for (int i = 0; i < row.Length; i++)
                {
                    var item = new BookImportDataTableVM()
                    {
                        ColumnIndex = i + 1,
                        ColumnName = ViewModelPivotItem.ParentPage.ViewModelPage.DataTable.Columns[i + 1].ColumnName,
                        RowName = row[i]?.ToString(),
                    };

                    ViewModelPivotItem.Titles.Add(item);
                    ViewModelPivotItem.Auteurs.Add(item);
                    ViewModelPivotItem.Langues.Add(item);
                    ViewModelPivotItem.MaisonsEdition.Add(item);
                    ViewModelPivotItem.Formats.Add(item);
                    ViewModelPivotItem.DateParution.Add(item);
                    ViewModelPivotItem.NumberOfPages.Add(item);
                    ViewModelPivotItem.Collections.Add(item);
                }

                if (ViewModelPivotItem.ItemsVisibility != Visibility.Visible)
                {
                    ViewModelPivotItem.ItemsVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                ImportDataRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private bool IsModelValided()
        {
            try
            {
                if (ViewModelPivotItem.SelectedTitle == null)
                {
                    ViewModelPivotItem.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPivotItem.ResultMessage = $"Vous devez sélectionner la donnée qui correspond au titre.";
                    ViewModelPivotItem.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPivotItem.IsResultMessageOpen = true;
                    return false;
                }
                List<LivreVM> list = new List<LivreVM>();
                IList<object> selectedItems = ViewModelPivotItem.ParentPage.ImportBookExcelSubPage.SelectedItems;
                if (selectedItems != null)
                {
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        if (selectedItems[i] is object[] row)
                        {
                            var title = row[ViewModelPivotItem.SelectedTitle.ColumnIndex].ToString();
                            var viewModel = new LivreVM()
                            {
                                Publication = new LivrePublicationVM(),
                                MainTitle = title,
                            };

                            if (ViewModelPivotItem.SelectedAuteur != null)
                            {
                                var authors = row[ViewModelPivotItem.SelectedAuteur.ColumnIndex].ToString();
                                if (!authors.IsStringNullOrEmptyOrWhiteSpace())
                                {
                                    var authorsVm = DbServices.Contact.CreateViewModel(authors, ContactType.Human, ContactRole.Author, ',');
                                    if (authorsVm != null && authorsVm.Any())
                                    {
                                        viewModel.Auteurs = new ObservableCollection<ContactVM>(authorsVm);
                                    }
                                }
                            }

                            if (ViewModelPivotItem.SelectedMaisonEdition != null)
                            {
                                var contacts = row[ViewModelPivotItem.SelectedMaisonEdition.ColumnIndex].ToString();
                                if (!contacts.IsStringNullOrEmptyOrWhiteSpace())
                                {
                                    var contactVMs = DbServices.Contact.CreateViewModel(contacts, ContactType.Society, ContactRole.EditorHouse, ',');
                                    if (contactVMs != null && contactVMs.Any())
                                    {
                                        viewModel.Publication.Editeurs = new ObservableCollection<ContactVM>(contactVMs);
                                    }
                                }
                            }

                            if (ViewModelPivotItem.SelectedCollection != null)
                            {
                                List<CollectionVM> collectionViewModelList = new List<CollectionVM>();
                                var collections = row[ViewModelPivotItem.SelectedCollection.ColumnIndex].ToString();
                                if (!collections.IsStringNullOrEmptyOrWhiteSpace())
                                {
                                    var collectionsVm = DbServices.Collection.CreateViewModel(ViewModelPivotItem.ParentPage.Parameters.ParentLibrary.Id, collections, ',');
                                    if (collectionsVm != null && collectionsVm.Any())
                                    {
                                        viewModel.Publication.Collections = new ObservableCollection<CollectionVM>(collectionsVm);
                                    }
                                }
                            }

                            list.Add(viewModel);
                        }

                    }
                }

                ViewModelPivotItem.NewViewModel = list;
                ViewModelPivotItem.IsResultMessageOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (ImportDataRequested != null)
                {
                    ImportDataRequested = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class ImportBookFromExcelUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        //public Guid? ParentGuid { get; set; }

        private string _Header = "Importer un fichier Excel";
        public string Header
        {
            get => this._Header;
            set
            {
                if (this._Header != value)
                {
                    this._Header = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Glyph = "\uE8B5";
        public string Glyph
        {
            get => _Glyph;
            set
            {
                if (_Glyph != value)
                {
                    _Glyph = value;
                    OnPropertyChanged();
                }
            }
        }

        private StorageFile _FileStorage;
        public StorageFile FileStorage
        {
            get => this._FileStorage;
            set
            {
                if (this._FileStorage != value)
                {
                    this._FileStorage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookCollectionPage _ParentPage;
        public BookCollectionPage ParentPage
        {
            get => this._ParentPage;
            set
            {
                if (this._ParentPage != value)
                {
                    this._ParentPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _WorkSheetsName = new ObservableCollection<string>();
        public ObservableCollection<string> WorkSheetsName
        {
            get => this._WorkSheetsName;
            set
            {
                if (this._WorkSheetsName != value)
                {
                    this._WorkSheetsName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _SelectedWorkSheetName;
        public string SelectedWorkSheetName
        {
            get => this._SelectedWorkSheetName;
            set
            {
                if (this._SelectedWorkSheetName != value)
                {
                    this._SelectedWorkSheetName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _TableRange;
        public string TableRange
        {
            get => this._TableRange;
            set
            {
                if (this._TableRange != value)
                {
                    this._TableRange = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DataTable _DataTable;
        [Obsolete]
        public DataTable DataTable
        {
            get => this._DataTable;
            set
            {
                if (this._DataTable != value)
                {
                    this._DataTable = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsTableRangeContainsHeader;
        public bool IsTableRangeContainsHeader
        {
            get => this._IsTableRangeContainsHeader;
            set
            {
                if (this._IsTableRangeContainsHeader != value)
                {
                    this._IsTableRangeContainsHeader = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _Titles = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> Titles
        {
            get => this._Titles;
            set
            {
                if (this._Titles != value)
                {
                    this._Titles = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedTitle;
        public BookImportDataTableVM SelectedTitle
        {
            get => this._SelectedTitle;
            set
            {
                if (this._SelectedTitle != value)
                {
                    this._SelectedTitle = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _Auteurs = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> Auteurs
        {
            get => this._Auteurs;
            set
            {
                if (this._Auteurs != value)
                {
                    this._Auteurs = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedAuteur;
        public BookImportDataTableVM SelectedAuteur
        {
            get => this._SelectedAuteur;
            set
            {
                if (this._SelectedAuteur != value)
                {
                    this._SelectedAuteur = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _MaisonsEdition = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> MaisonsEdition
        {
            get => this._MaisonsEdition;
            set
            {
                if (this._MaisonsEdition != value)
                {
                    this._MaisonsEdition = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedMaisonEdition;
        public BookImportDataTableVM SelectedMaisonEdition
        {
            get => this._SelectedMaisonEdition;
            set
            {
                if (this._SelectedMaisonEdition != value)
                {
                    this._SelectedMaisonEdition = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _Langues = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> Langues
        {
            get => this._Langues;
            set
            {
                if (this._Langues != value)
                {
                    this._Langues = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedLangue;
        public BookImportDataTableVM SelectedLangue
        {
            get => this._SelectedLangue;
            set
            {
                if (this._SelectedLangue != value)
                {
                    this._SelectedLangue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _DateParution = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> DateParution
        {
            get => this._DateParution;
            set
            {
                if (this._DateParution != value)
                {
                    this._DateParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedDateParution;
        public BookImportDataTableVM SelectedDateParution
        {
            get => this._SelectedDateParution;
            set
            {
                if (this._SelectedDateParution != value)
                {
                    this._SelectedDateParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _NumberOfPages = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> NumberOfPages
        {
            get => this._NumberOfPages;
            set
            {
                if (this._NumberOfPages != value)
                {
                    this._NumberOfPages = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedNumberOfPages;
        public BookImportDataTableVM SelectedNumberOfPages
        {
            get => this._SelectedNumberOfPages;
            set
            {
                if (this._SelectedNumberOfPages != value)
                {
                    this._SelectedNumberOfPages = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _Formats = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> Formats
        {
            get => this._Formats;
            set
            {
                if (this._Formats != value)
                {
                    this._Formats = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedFormat;
        public BookImportDataTableVM SelectedFormat
        {
            get => this._SelectedFormat;
            set
            {
                if (this._SelectedFormat != value)
                {
                    this._SelectedFormat = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<BookImportDataTableVM> _Collections = new ObservableCollection<BookImportDataTableVM>();
        public ObservableCollection<BookImportDataTableVM> Collections
        {
            get => this._Collections;
            set
            {
                if (this._Collections != value)
                {
                    this._Collections = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BookImportDataTableVM _SelectedCollection;
        public BookImportDataTableVM SelectedCollection
        {
            get => this._SelectedCollection;
            set
            {
                if (this._SelectedCollection != value)
                {
                    this._SelectedCollection = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private Visibility _ItemsVisibility = Visibility.Collapsed;
        public Visibility ItemsVisibility
        {
            get => this._ItemsVisibility;
            set
            {
                if (this._ItemsVisibility != value)
                {
                    this._ItemsVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private List<LivreVM> _NewViewModel = new List<LivreVM>();
        public List<LivreVM> NewViewModel
        {
            get => this._NewViewModel;
            set
            {
                if (this._NewViewModel != value)
                {
                    this._NewViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessage;
        public string ResultMessage
        {
            get => this._ResultMessage;
            set
            {
                if (this._ResultMessage != value)
                {
                    this._ResultMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private InfoBarSeverity _ResultMessageSeverity = InfoBarSeverity.Informational;
        public InfoBarSeverity ResultMessageSeverity
        {
            get => this._ResultMessageSeverity;
            set
            {
                if (this._ResultMessageSeverity != value)
                {
                    this._ResultMessageSeverity = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsResultMessageOpen;
        public bool IsResultMessageOpen
        {
            get => this._IsResultMessageOpen;
            set
            {
                if (this._IsResultMessageOpen != value)
                {
                    this._IsResultMessageOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessageTitle;
        public string ResultMessageTitle
        {
            get => this._ResultMessageTitle;
            set
            {
                if (this._ResultMessageTitle != value)
                {
                    this._ResultMessageTitle = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
