using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Excel;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Author;
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
        private readonly ExcelServices excelServices;
        public readonly ImportBookParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public ImportBookFromExcelUCVM ViewModelPage { get; set; } = new ImportBookFromExcelUCVM();

        public delegate void CancelModificationEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;


        public delegate void ImportDataEventHandler(ImportBookFromExcelUC sender, ExecuteRequestedEventArgs e);
        public event ImportDataEventHandler ImportDataRequested;
        public ImportBookFromExcelUC()
        {
            this.InitializeComponent();
        }

        public ImportBookFromExcelUC(ImportBookParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            excelServices = new ExcelServices(parameters.ExcelFile);
        }

        private async void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var worksheetsName = await excelServices.GetExcelSheetsName();
                ViewModelPage.WorkSheetsName = new ObservableCollection<string>(worksheetsName);

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void SearchDataInFileXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (ViewModelPage.SelectedWorkSheetName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner une feuille";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return;
                }

                if (ViewModelPage.TableRange.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner une plage de cellule pour délimiter votre tableau.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return;
                }

                var rr = await excelServices.ImportExcelToDatatable(ViewModelPage.SelectedWorkSheetName, ViewModelPage.TableRange, ViewModelPage.IsTableRangeContainsHeader);
                ViewModelPage.DataTable = rr;
                SearchingResult();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchingResult()
        {
            try
            {
                ViewModelPage.ItemstVisibility = Visibility.Collapsed;
                if (ViewModelPage.DataTable == null)
                {
                    return;
                }

                Run lineCount = new Run()
                {
                    Text = $"{ViewModelPage.DataTable.Rows.Count} {(ViewModelPage.DataTable.Rows.Count > 1 ? "lignes ont été trouvées" : "ligne a été trouvée")}. ",
                    FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(lineCount);
                
                if (ViewModelPage.DataTable.Rows.Count == 0)
                {
                    return;
                }

                ViewModelPage.ItemstVisibility = Visibility.Visible;
                Run runTitle = new Run()
                {
                    Text = $"La première ligne a été sélectionnée afin de nous aider à importer vos livres.",
                    //FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(runTitle);

                ViewModelPage.Titles.Clear();
                ViewModelPage.Auteurs.Clear();
                ViewModelPage.MaisonsEdition.Clear();
                ViewModelPage.Langues.Clear();
                ViewModelPage.DateParution.Clear();
                ViewModelPage.Formats.Clear();
                ViewModelPage.NumberOfPages.Clear();
                ViewModelPage.Collections.Clear();
                for (int i = 0; i < ViewModelPage.DataTable.Columns.Count; i++)
                {
                    var item = new BookImportDataTableVM()
                    {
                        ColumnIndex = i,
                        ColumnName = ViewModelPage.DataTable.Columns[i].ColumnName,
                        RowName = ViewModelPage.DataTable.Rows[0].ItemArray[i]?.ToString(),
                    };

                    ViewModelPage.Titles.Add(item);
                    ViewModelPage.Auteurs.Add(item);
                    ViewModelPage.Langues.Add(item);
                    ViewModelPage.MaisonsEdition.Add(item);
                    ViewModelPage.Formats.Add(item);
                    ViewModelPage.DateParution.Add(item);
                    ViewModelPage.NumberOfPages.Add(item);
                    ViewModelPage.Collections.Add(item);
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

        private async void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = await IsModelValided();
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



        private async Task<bool> IsModelValided()
        {
            try
            {
                if (ViewModelPage.SelectedTitle == null)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner la donnée qui correspond au titre.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }
                List<LivreVM> list = new List<LivreVM>();
                for (int i = 0; i < ViewModelPage.DataTable.Rows.Count; i++)
                {
                    var viewModel = new LivreVM()
                    {
                        Publication = new LivrePublicationVM(),
                        MainTitle = ViewModelPage.DataTable.Rows[i].ItemArray[ViewModelPage.SelectedTitle.ColumnIndex].ToString(),
                    };

                    if (ViewModelPage.SelectedAuteur != null)
                    {
                        List<ContactVM> authorViewModelList = new List<ContactVM>();
                        var authors = ViewModelPage.DataTable.Rows[i].ItemArray[ViewModelPage.SelectedAuteur.ColumnIndex].ToString();
                        if (!authors.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            var authorsList = StringHelpers.SplitWord(authors, new string[] { "," });
                            if (authorsList != null && authorsList.Length > 0)
                            {
                                foreach (var author in authorsList)
                                {
                                    ContactVM authorVm = new ContactVM()
                                    {
                                        ContactType = ContactType.Author,
                                        TitreCivilite = CivilityHelpers.NonSpecifie,
                                    };

                                    var split = StringHelpers.SplitWord(author, new string[] { " " });
                                    
                                    if (split.Length == 1)
                                    {
                                        authorVm.Prenom = split[0];
                                    }
                                    else if (split.Length >= 2)
                                    {
                                        authorVm.Prenom = split[0];
                                        authorVm.NomNaissance = split[1];
                                    }

                                    authorViewModelList.Add(authorVm);
                                }
                            }
                            
                        }
                        viewModel.Auteurs = new ObservableCollection<ContactVM>(authorViewModelList);
                    }

                    if (ViewModelPage.SelectedCollection != null)
                    {
                        List<CollectionVM> collectionViewModelList = new List<CollectionVM>();
                        var collections = ViewModelPage.DataTable.Rows[i].ItemArray[ViewModelPage.SelectedCollection.ColumnIndex].ToString();
                        if (!collections.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            var collectionList = StringHelpers.SplitWord(collections, new string[] { "," });
                            if (collectionList != null && collectionList.Length > 0)
                            {
                                foreach (var collection in collectionList)
                                {
                                    CollectionVM collectionVm = new CollectionVM()
                                    {
                                        Name = collection,
                                    };
                                    collectionViewModelList.Add(collectionVm);
                                }
                            }

                        }
                        viewModel.Publication.Collections = new ObservableCollection<CollectionVM>(collectionViewModelList);
                    }

                    list.Add(viewModel);
                }

                ViewModelPage.NewViewModel = list;
                ViewModelPage.IsResultMessageOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

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

        public Guid Guid { get; set; } = Guid.NewGuid();

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


        private Visibility _ItemstVisibility = Visibility.Collapsed;
        public Visibility ItemstVisibility
        {
            get => this._ItemstVisibility;
            set
            {
                if (this._ItemstVisibility != value)
                {
                    this._ItemstVisibility = value;
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
