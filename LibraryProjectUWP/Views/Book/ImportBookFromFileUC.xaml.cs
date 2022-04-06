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
    public sealed partial class ImportBookFromFileUC : PivotItem
    {
        public readonly ImportBookParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public ImportBookFromFileUCVM ViewModelPage { get; set; } = new ImportBookFromFileUCVM();

        public delegate void CancelModificationEventHandler(ImportBookFromFileUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;


        public delegate void ImportDataEventHandler(ImportBookFromFileUC sender, ExecuteRequestedEventArgs e);
        public event ImportDataEventHandler ImportDataRequested;
        public ImportBookFromFileUC()
        {
            this.InitializeComponent();
        }

        public ImportBookFromFileUC(ImportBookParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeText();
        }

        public void InitializeText()
        {
            try
            {
                TbcAfterSearching.Inlines.Clear();

                Run lineCount = new Run()
                {
                    Text = $"{_parameters.ParentPage.ImportBookFileSubPage.SelectedItems.Count} {(_parameters.ParentPage.ImportBookFileSubPage.SelectedItems.Count > 1 ? "lignes ont été sélectionnées" : "ligne a été sélectionnée")}. ",
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

                if (_parameters.ParentPage.ImportBookFileSubPage.SelectedItems.Count == 0)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez sélectionner au moins une ligne.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                ViewModelPage.NewViewModel = new List<LivreVM>(_parameters.ParentPage.ImportBookFileSubPage.SelectedItems);
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

    public class ImportBookFromFileUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> SourceList = LibraryHelpers.Book.Entry.EntrySourceList;
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        public Guid Guid { get; set; } = Guid.NewGuid();

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
