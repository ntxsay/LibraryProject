using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Contact
{
    public sealed partial class ContactListUC : PivotItem
    {
        readonly ContactListParametersDriverVM _parameters;
        public ContactListUCVM ViewModelPage { get; set; } = new ContactListUCVM();

        public delegate void CancelModificationEventHandler(ContactListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        private CollectionViewSource CollectionViewSource { get; set; } = new CollectionViewSource()
        {
            IsSourceGrouped = true,
            ItemsPath = new PropertyPath("Items")
        };

        readonly GetContactsListTask getContactsListTask = new GetContactsListTask();

        public ContactListUC()
        {
            this.InitializeComponent();
        }

        public ContactListUC(ContactListParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Les adhérants";
            ViewModelPage.ViewModelList = parameters?.ViewModelList;
            InitializeData(ContactType.Human);
        }

        public void InitializeData(ContactType contactType)
        {
            try
            {
                ViewModelPage.WorkerTextVisibility = Visibility.Visible;
                ViewModelPage.DataListVisibility = Visibility.Collapsed;
                if (getContactsListTask.IsWorkerRunning)
                {
                    return;
                }

                getContactsListTask.InitializeWorker(contactType);
                getContactsListTask.AfterTaskCompletedRequested += (j, e) =>
                {
                    if (e.Result is WorkerState<ContactVM, ContactVM> result && result.ResultList != null && result.ResultList.Any())
                    {
                        var GroupingItems = this.OrderItems(result.ResultList)?.Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.NomNaissance.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                        if (GroupingItems != null && GroupingItems.Count() > 0)
                        {
                            if (this.CollectionViewSource.View == null)
                            {
                                this.CollectionViewSource.Source = ViewModelPage.ViewModelListGroup;
                            }

                            List<ContactGroupCastVM> contactGroupCastVMs = (GroupingItems.Select(groupingItem => new ContactGroupCastVM()
                            {
                                GroupName = groupingItem.Key,
                                Items = new ObservableCollection<ContactVM>(groupingItem),
                            })).ToList();

                            ViewModelPage.ViewModelListGroup.Clear();
                            foreach (var item in contactGroupCastVMs)
                            {
                                ViewModelPage.ViewModelListGroup.Add(item);
                            }
                        }

                    }
                    ViewModelPage.WorkerTextVisibility = Visibility.Collapsed;
                    ViewModelPage.DataListVisibility = Visibility.Visible;
                };
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InitializeDataGroup()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Groups
       
        //public void GroupItemsByLetterNomNaissance()
        //{
        //    try
        //    {
        //        if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
        //        {
        //            return;
        //        }

        //        var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.NomNaissance.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
        //        if (GroupingItems != null && GroupingItems.Count() > 0)
        //        {
        //            List<ContactGroupCastVM> contactGroupCastVMs = (GroupingItems.Select(groupingItem => new ContactGroupCastVM()
        //            {
        //                GroupName = groupingItem.Key,
        //                Items = new ObservableCollection<ContactVM>(groupingItem),
        //            })).ToList();

        //            ViewModelPage.ViewModelListGroup = contactGroupCastVMs;
                    
        //            //_contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.LetterNomNaissance;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        //public void GroupItemsByLetterPrenom()
        //{
        //    try
        //    {
        //        if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
        //        {
        //            return;
        //        }

        //        var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _contactParameters.ParentPage.ViewModelPage.OrderedBy, _contactParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Prenom.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
        //        if (GroupingItems != null && GroupingItems.Count() > 0)
        //        {
        //            ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
        //            _contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.LetterPrenom;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}

        //public void GroupByCreationYear()
        //{
        //    try
        //    {
        //        if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
        //        {
        //            return;
        //        }

        //        var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
        //        if (GroupingItems != null && GroupingItems.Count() > 0)
        //        {
        //            ViewModelPage.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
        //            //_contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.CreationYear;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}
        #endregion

        #region Group-Orders
        private IEnumerable<ContactVM> OrderItems(IEnumerable<ContactVM> Collection, ContactGroupVM.OrderBy OrderBy = ContactGroupVM.OrderBy.Croissant, ContactGroupVM.SortBy SortBy = ContactGroupVM.SortBy.Prenom)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == ContactGroupVM.SortBy.Prenom)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Prenom);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Prenom);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.NomNaissance)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.NomNaissance);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.NomNaissance);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.OrderByDescending(o => o.DateAjout);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<ContactVM>();
            }
        }


        #endregion

        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void ASB_SearchItem_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }
    }

    public class ContactListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        //public Guid? ParentGuid { get; set; }

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

        private string _Glyph = "\ue779";
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

        private string _ArgName;
        public string ArgName
        {
            get => this._ArgName;
            set
            {
                if (this._ArgName != value)
                {
                    this._ArgName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public readonly IEnumerable<string> civilityList = CivilityHelpers.CiviliteListShorted();

        private ContactVM _ViewModel;
        public ContactVM ViewModel
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

        private Brush _ResultMessageForeGround;
        public Brush ResultMessageForeGround
        {
            get => this._ResultMessageForeGround;
            set
            {
                if (this._ResultMessageForeGround != value)
                {
                    this._ResultMessageForeGround = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _DataListVisibility;
        public Visibility DataListVisibility
        {
            get => this._DataListVisibility;
            set
            {
                if (this._DataListVisibility != value)
                {
                    this._DataListVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _WorkerTextVisibility;
        public Visibility WorkerTextVisibility
        {
            get => this._WorkerTextVisibility;
            set
            {
                if (this._WorkerTextVisibility != value)
                {
                    this._WorkerTextVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<ContactVM> _ViewModelList;
        public IEnumerable<ContactVM> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (this._ViewModelList != value)
                {
                    this._ViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ContactGroupCastVM> _ViewModelListGroup = new ObservableCollection<ContactGroupCastVM>();
        public ObservableCollection<ContactGroupCastVM> ViewModelListGroup
        {
            get => this._ViewModelListGroup;
            set
            {
                if (this._ViewModelListGroup != value)
                {
                    this._ViewModelListGroup = value;
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
