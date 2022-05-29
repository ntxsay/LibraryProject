using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Excel;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.PrincipalPages;
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

namespace LibraryProjectUWP.Views.Common
{
    public sealed partial class ImportItemsFromFileSideBar : PivotItem
    {
        public BookCollectionPage BookCollectionPage { get; private set; }
        public Type Type { get; private set; }
        IEnumerable<BibliothequeVM> bibliothequeVMs;
        IEnumerable<LivreVM> livreVMs;
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();

        public ImportItemsFromFileSideBarVM ViewModelPage { get; set; }

        public delegate void CancelModificationEventHandler(ImportItemsFromFileSideBar sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;


        public delegate void ImportDataEventHandler(ImportItemsFromFileSideBar sender, object originalViewModel, OperationStateVM e);
        public event ImportDataEventHandler ImportDataRequested;
        public ImportItemsFromFileSideBar()
        {
            this.InitializeComponent();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void InitializeSideBar<T, U>(U page, IEnumerable<T> objectList, StorageFile file) where T : class where U : Page
        {
            try
            {
                if (objectList == null && !objectList.Any())
                {
                    return;
                }

                Type = typeof(T);
                if (Type == typeof(BibliothequeVM))
                {
                    bibliothequeVMs = objectList.Select(s => (BibliothequeVM)(object)s);
                }
                else if (Type == typeof(LivreVM))
                {
                    livreVMs = objectList.Select(s => (LivreVM)(object)s);
                }

                if (page is BookCollectionPage bookCollectionPage)
                {
                    BookCollectionPage = bookCollectionPage;
                }

                ViewModelPage = new ImportItemsFromFileSideBarVM()
                {
                    File = file,
                };

                //ViewModelPage.Header = $"{(ViewModelPage.EditMode == EditMode.Create ? "Ajouter" : "Editer")} un livre";

                InitializeText();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void InitializeText()
        {
            try
            {
                TbcInfos.Inlines.Clear();
                Run lineT1 = new Run();

                if (Type == typeof(BibliothequeVM))
                {
                    lineT1.Text = "Vous êtes en train d'importer des bibliothèques à partir du fichier «";
                }
                else if (Type == typeof(LivreVM))
                {
                    lineT1.Text = "Vous êtes en train d'importer des livres à partir du fichier «";
                }
                TbcInfos.Inlines.Add(lineT1);

                TbcInfos.Inlines.Add(new Run()
                {
                    Text = ViewModelPage.File?.Name,
                    FontWeight = FontWeights.SemiBold,
                });
                TbcInfos.Inlines.Add(new Run()
                {
                    Text = "»."
                });

                TbcAfterSearching.Inlines.Clear();

                string countSelectedLines = string.Empty;

                if (BookCollectionPage != null)
                {
                    countSelectedLines = $"{BookCollectionPage.ImportItemsFromTablePage.SelectedItems.Count} {(BookCollectionPage.ImportItemsFromTablePage.SelectedItems.Count > 1 ? "lignes ont été sélectionnées" : "ligne a été sélectionnée")}. ";
                }

                Run lineCount = new Run()
                {
                    Text = countSelectedLines,
                    FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(lineCount);


                ViewModelPage.ItemstVisibility = Visibility.Visible;
                Run runTitle = new Run()
                {
                    Text = $"La première ligne a été sélectionnée afin de nous aider à importer vos livres.",
                    //FontWeight = FontWeights.Medium,
                };
                TbcAfterSearching.Inlines.Add(runTitle);
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

                if (Type == typeof(BibliothequeVM))
                {
                    List<BibliothequeVM> list = new List<BibliothequeVM>();
                    foreach (object[] item in ViewModelPage.NewViewModel)
                    {
                        var range = bibliothequeVMs.Where(w => w.Name == item[1].ToString());
                        if (range != null && range.Any())
                        {
                            list.AddRange(range);
                        }
                    }

                    if (list != null && list.Any())
                    {
                        if (BookCollectionPage != null)
                        {
                            ImportBooksOrLibrariesTask importBooksTask = new ImportBooksOrLibrariesTask(BookCollectionPage.Parameters.MainPage);
                            importBooksTask.AfterTaskCompletedRequested += ImportBooksTask_AfterTaskCompletedRequested;
                            importBooksTask.InitializeWorker(list);
                        }
                    }
                }
                else if (Type == typeof(LivreVM))
                {

                }

                


                //ImportDataRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ImportBooksTask_AfterTaskCompletedRequested(ImportBooksOrLibrariesTask sender, object e)
        {
            if (BookCollectionPage != null)
            {
                BookCollectionPage.ViewModelPage.IsUpdateSubView = true;
                if (BookCollectionPage.Parameters.ParentLibrary == null)
                {
                    BookCollectionPage.OpenLibraryCollection();
                }
                else
                {
                    BookCollectionPage.OpenBookCollection(BookCollectionPage.Parameters.ParentLibrary);
                    if (BookCollectionPage.IsContainsBookCollection(out var bookCollectionSubPage))
                    {
                        bookCollectionSubPage.UpdateBinding();
                    }
                }

            }
        }

        private bool IsModelValided()
        {
            try
            {
                if (BookCollectionPage != null && BookCollectionPage.ImportItemsFromTablePage.SelectedItems.Count == 0)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner au moins une ligne.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                ViewModelPage.NewViewModel = new List<object>(BookCollectionPage.ImportItemsFromTablePage.SelectedItems);
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

    public class ImportItemsFromFileSideBarVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        public StorageFile File { get; set; }
        private string _Header = "Importer un fichier";
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

        private List<object> _NewViewModel = new List<object>();
        public List<object> NewViewModel
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
