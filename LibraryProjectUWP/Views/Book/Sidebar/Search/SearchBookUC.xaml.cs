using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class SearchBookUC : PivotItem
    {

        public SearchBookUCVM ViewModelPage { get; set; } = new SearchBookUCVM();
        public BookCollectionPage ParentPage { get; private set; }

        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        public SideBarInterLinkVM ParentReferences { get; set; }


        public delegate void CancelModificationEventHandler(SearchBookUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void HideSearchBookPanelEventHandler(SearchBookUC sender, RoutedEventArgs e);
        public event HideSearchBookPanelEventHandler HideSearchBookPanelRequested;


        public delegate void SearchBookEventHandler(SearchBookUC sender, ExecuteRequestedEventArgs e);
        public event SearchBookEventHandler SearchBookRequested;

        readonly GetAvailableBookExemplariesForPretTask getAvailableBookExemplariesForPretTask = new GetAvailableBookExemplariesForPretTask();

        public SearchBookUC()
        {
            this.InitializeComponent();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Close()
        {
            CancelModificationRequested?.Invoke(this, null);
        }


        public void InitializeSideBar(BookCollectionPage parentPage, ResearchItemVM viewModel, long? idLibrary = null)
        {
            try
            {
                ParentPage = parentPage;
                ViewModelPage = new SearchBookUCVM()
                {
                    IdLibrary = idLibrary,
                    ViewModel = viewModel,
                };

                if (ViewModelPage.ViewModel.TypeObject == typeof(BibliothequeVM))
                {
                    ViewModelPage.SearchInMainTitleVisibility = Visibility.Visible;
                    ViewModelPage.SearchInOtherTitlesVisibility = Visibility.Collapsed;
                    ViewModelPage.SearchInAuthorsVisibility = Visibility.Collapsed;
                    ViewModelPage.SearchInEditorsVisibility = Visibility.Collapsed;
                    ViewModelPage.SearchInCollectionsVisibility = Visibility.Collapsed;
                }
                else if (ViewModelPage.ViewModel.TypeObject == typeof(LivreVM))
                {
                    ViewModelPage.SearchInMainTitleVisibility = Visibility.Visible;
                    ViewModelPage.SearchInOtherTitlesVisibility = Visibility.Visible;
                    ViewModelPage.SearchInAuthorsVisibility = Visibility.Visible;
                    ViewModelPage.SearchInEditorsVisibility = Visibility.Visible;
                    ViewModelPage.SearchInCollectionsVisibility = Visibility.Visible;
                }

                var keyPair = Search.SearchOnListDictionary.SingleOrDefault(s => s.Key == (byte)ViewModelPage.ViewModel.TermParameter);
                if (!keyPair.Equals(default(KeyValuePair<byte, string>)))
                {
                    CmbxTermsParams.SelectedItem = keyPair.Value;
                }


                InitializeActionInfos();
                this.Bindings.Update();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void InitializeActionInfos()
        {
            try
            {
                TbcInfos.Inlines.Clear();
                if (ViewModelPage.ViewModel.TypeObject == typeof(BibliothequeVM))
                {
                    ViewModelPage.Header = $"Rechercher des bibliothèques";
                    Run runTitle = new Run()
                    {
                        Text = $"Vous êtes en train de rechercher des bibliothèques.",
                        //FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runTitle);
                }
                else if (ViewModelPage.ViewModel.TypeObject == typeof(LivreVM))
                {
                    ViewModelPage.Header = $"Rechercher des livres";
                    Run runTitle = new Run()
                    {
                        Text = $"Vous êtes en train de rechercher des livres.",
                        //FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runTitle);
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

        private void BtnHidePanel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideSearchBookPanelRequested?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void SearchBookXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                SearchBookRequested?.Invoke(this, args);
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
                if (ViewModelPage.ViewModel.Term.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez entrer le terme de la recherche";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.SearchInMainTitle != true && ViewModelPage.ViewModel.SearchInOtherTitles != true &&
                    ViewModelPage.ViewModel.SearchInAuthors != true && ViewModelPage.ViewModel.SearchInCollections != true &&
                    ViewModelPage.ViewModel.SearchInEditors != true)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Vous devez cocher le chemin dans lequel sera recherché le terme.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (SearchBookRequested != null)
                {
                    SearchBookRequested = null;
                }

                if (HideSearchBookPanelRequested != null)
                {
                    HideSearchBookPanelRequested = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox && comboBox.SelectedItem is string value)
                {
                    var keyPair = LibraryHelpers.Book.Search.SearchOnListDictionary.SingleOrDefault(s => s.Value == value);
                    if (!keyPair.Equals(default(KeyValuePair<byte, string>)))
                    {
                        ViewModelPage.ViewModel.TermParameter = (Code.Search.Terms)keyPair.Key;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class SearchBookUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> searchOnList = LibraryHelpers.Book.Search.SearchOnList;

        public long? IdLibrary { get; set; }

        private string _Header;
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

        private string _Glyph = "\uE71E";
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


        private ResearchItemVM _ViewModel;
        public ResearchItemVM ViewModel
        {
            get => this._ViewModel;
            set
            {
                if (this._ViewModel != value)
                {
                    this._ViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchInMainTitleVisibility;
        public Visibility SearchInMainTitleVisibility
        {
            get => _SearchInMainTitleVisibility;
            set
            {
                if (_SearchInMainTitleVisibility != value)
                {
                    _SearchInMainTitleVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchInOtherTitlesVisibility;
        public Visibility SearchInOtherTitlesVisibility
        {
            get => _SearchInOtherTitlesVisibility;
            set
            {
                if (_SearchInOtherTitlesVisibility != value)
                {
                    _SearchInOtherTitlesVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchInAuthorsVisibility;
        public Visibility SearchInAuthorsVisibility
        {
            get => _SearchInAuthorsVisibility;
            set
            {
                if (_SearchInAuthorsVisibility != value)
                {
                    _SearchInAuthorsVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchInCollectionsVisibility;
        public Visibility SearchInCollectionsVisibility
        {
            get => _SearchInCollectionsVisibility;
            set
            {
                if (_SearchInCollectionsVisibility != value)
                {
                    _SearchInCollectionsVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchInEditorsVisibility;
        public Visibility SearchInEditorsVisibility
        {
            get => _SearchInEditorsVisibility;
            set
            {
                if (_SearchInEditorsVisibility != value)
                {
                    _SearchInEditorsVisibility = value;
                    OnPropertyChanged();
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