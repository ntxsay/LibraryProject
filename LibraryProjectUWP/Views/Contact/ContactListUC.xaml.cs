﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.PrincipalPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Contact
{
    public sealed partial class ContactListUC : PivotItem
    {
        public BookCollectionPage ParentPage { get; private set; }
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        public ContactListUCVM ViewModelPage { get; set; } = new ContactListUCVM();

        public delegate void CancelModificationEventHandler(ContactListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(ContactListUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(ContactListUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public ObservableCollection<ContactGroupCastVM> ViewModelListGroup { get; set; } = new ObservableCollection<ContactGroupCastVM>();
        private CollectionViewSource CollectionViewSource { get; set; } = new CollectionViewSource()
        {
            IsSourceGrouped = true,
            ItemsPath = new PropertyPath("Items")
        };

        readonly GetContactsListTask getContactsListTask = new GetContactsListTask();

        public ContactListUC()
        {
            if (this.CollectionViewSource.View == null)
            {
                this.CollectionViewSource.Source = ViewModelListGroup;
            }

            this.InitializeComponent();
        }

        public void InitializeSideBar(BookCollectionPage bookCollectionPage, ContactType? contactType = null, IEnumerable<ContactRole> contactRoleList = null)
        {
            try
            {
                ParentPage = bookCollectionPage;

                ViewModelPage = new ContactListUCVM()
                {
                    ContactType = contactType,
                    ContactRoles = contactRoleList != null && contactRoleList.Any() ? new ObservableCollection<ContactRole>(contactRoleList) : new ObservableCollection<ContactRole>(),
                };

                if (contactType != null && (contactRoleList == null || contactRoleList != null && !contactRoleList.Any()))
                {
                    switch (contactType)
                    {
                        case ContactType.Human:
                            ViewModelPage.Header = $"Les contacts";
                            TbcInfos.Text = "Vous naviguez parmis tous les contacts";
                            break;
                        case ContactType.Society:
                            ViewModelPage.Header = $"Les sociétés";
                            TbcInfos.Text = "Vous naviguez parmis toutes les sociétés";
                            break;
                        default:
                            break;
                    }
                }
                else if (contactType == null && contactRoleList != null && contactRoleList.Any())
                {
                    if (contactRoleList.Count() == 1)
                    {
                        switch (contactRoleList.FirstOrDefault())
                        {
                            case ContactRole.Adherant:
                                ViewModelPage.Header = $"Afficher les adhérants";
                                TbcInfos.Text = "Vous naviguez parmis tous les adhérants";
                                break;
                            case ContactRole.Author:
                                ViewModelPage.Header = $"Afficher les auteurs";
                                TbcInfos.Text = "Vous naviguez parmis les auteurs";
                                break;
                            case ContactRole.Translator:
                                break;
                            case ContactRole.EditorHouse:
                                ViewModelPage.Header = $"Afficher les éditeurs";
                                TbcInfos.Text = "Vous naviguez parmis les éditeurs et maisons d'édition";
                                break;
                            case ContactRole.Illustrator:
                                break;
                        }
                    }
                    else
                    {
                        ViewModelPage.Header = $"Afficher les contacts";
                        TbcInfos.Text = "Vous naviguez parmis des contacts ayant divers rôles";
                    }
                    
                }
                else
                {
                    ViewModelPage.Header = $"Afficher les contacts";
                    TbcInfos.Text = "Vous naviguez parmis tous les contacts";
                }


                ViewModelPage.WorkerTextVisibility = Visibility.Visible;
                ViewModelPage.DataListVisibility = Visibility.Collapsed;
                if (getContactsListTask.IsWorkerRunning)
                {
                    return;
                }

                getContactsListTask.InitializeWorker(contactType, contactRoleList);
                getContactsListTask.AfterTaskCompletedRequested += (j, e) =>
                {
                    if (this.CollectionViewSource.View == null)
                    {
                        this.CollectionViewSource.Source = ViewModelListGroup;
                    }

                    if (e.Result is WorkerState<ContactVM, ContactVM> result && result.ResultList != null && result.ResultList.Any())
                    {
                        ViewModelPage.contactList = result.ResultList;
                        var GroupingItems = result.ResultList.OrderBy(o => o.DisplayName3)?.Where(w => !w.DisplayName3.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DisplayName3.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                        if (GroupingItems != null && GroupingItems.Count() > 0)
                        {
                            List<ContactGroupCastVM> contactGroupCastVMs = GroupingItems.Select(groupingItem => new ContactGroupCastVM()
                            {
                                GroupName = groupingItem.Key,
                                Items = new ObservableCollection<ContactVM>(groupingItem),
                            }).ToList();

                            ViewModelListGroup.Clear();
                            foreach (var item in contactGroupCastVMs)
                            {
                                ViewModelListGroup.Add(item);
                            }
                        }
                    }
                    else
                    {
                        ViewModelListGroup.Clear();
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

        private void ABBtn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            InitializeSideBar(ParentPage, ViewModelPage.ContactType, ViewModelPage.ContactRoles);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (CreateItemRequested != null)
                {
                    CreateItemRequested = null;
                }

                if (UpdateItemRequested != null)
                {
                    UpdateItemRequested = null;
                }

                getContactsListTask?.DisposeWorker();
                ViewModelPage = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region SearchItems
        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.contactList == null || !ViewModelPage.contactList.Any())
                {
                    return;
                }

                var FilteredItems = new List<ContactVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.contactList)
                {
                    if (value.DisplayName3.IsStringNullOrEmptyOrWhiteSpace()) continue;

                    var found = splitSearchTerm.All((key) =>
                    {
                        return value.DisplayName3.ToLower().Contains(key.ToLower());
                    });

                    if (found)
                    {
                        FilteredItems.Add(value);
                    }
                }
                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }

        private void ASB_SearchItem_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is ContactVM value)
                {
                    sender.Text = value.DisplayName3;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is ContactVM viewModel)
                {
                    foreach (var item in ListViewZoomInView.Items)
                    {
                        if (item is ContactVM itemViewModel && itemViewModel.Id == viewModel.Id)
                        {
                            if (ListViewZoomInView.SelectedItem != item)
                            {
                                ListViewZoomInView.SelectedItem = item;
                            }

                            ListViewZoomInView.ScrollIntoView(ListViewZoomInView.SelectedItem);
                            return;
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
        #endregion
        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is ContactVM contactVM)
                {
                    ParentPage.NewEditContact(EditMode.Edit, null, null, new SideBarInterLinkVM()
                    {
                        ParentGuid = this.ItemGuid,
                        ParentType = typeof(ContactListUC)
                    }, contactVM);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ABBtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.SelectedViewModel != null && ViewModelPage.SelectedViewModel is ContactVM viewModel)
                {
                    ParentPage.NewEditContact(EditMode.Edit, null, null, new SideBarInterLinkVM()
                    {
                        ParentGuid = this.ItemGuid,
                        ParentType = typeof(ContactListUC)
                    }, viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void MenuFlyoutDisplayContacts_Opened(object sender, object e)
        {
            try
            {
                if (ViewModelPage.ContactRoles != null && ViewModelPage.ContactRoles.Count > 0)
                {
                    TmfiAdherants.IsChecked = ViewModelPage.ContactRoles.Any(a => a == ContactRole.Adherant);
                    TmfiAuthors.IsChecked = ViewModelPage.ContactRoles.Any(a => a == ContactRole.Author);
                    TmfiEditors.IsChecked = ViewModelPage.ContactRoles.Any(a => a == ContactRole.EditorHouse);
                    TmfiAllContacts.IsChecked = false;
                }
                else
                {
                    TmfiAllContacts.IsChecked = true;
                    TmfiAdherants.IsChecked = false;
                    TmfiAuthors.IsChecked = false;
                    TmfiEditors.IsChecked = false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MFI_DisplayAdherant_Click(object sender, RoutedEventArgs e)
        {
            InitializeSideBar(ParentPage, null, new ContactRole[] { ContactRole.Adherant });
        }

        private void MFI_DisplayAuthors_Click(object sender, RoutedEventArgs e)
        {
            InitializeSideBar(ParentPage, null, new ContactRole[] { ContactRole.Author });

        }

        private void MFI_DisplayEditors_Click(object sender, RoutedEventArgs e)
        {
            InitializeSideBar(ParentPage, null, new ContactRole[] { ContactRole.EditorHouse });
        }

        private void MFI_DisplayAllContacts_Click(object sender, RoutedEventArgs e)
        {
            InitializeSideBar(ParentPage, null, null);
        }

        private void ListViewZoomInView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ListView listView)
                {
                    var cast = listView.SelectedItems?.Select(s => (ContactVM)s);
                    this.ViewModelPage.SelectedViewModels = new ObservableCollection<ContactVM>(cast);
                    if (listView.SelectedItems.Count > 1)
                    {
                        ViewModelPage.SearchAuthorsMessage = $"Rechercher les livres de ces auteurs";
                        ViewModelPage.SearchEditorsMessage = $"Rechercher les livres de ces éditeurs";
                        //if (TtipDeleteCollection.IsOpen)
                        //{
                        //    TtipDeleteCollection.IsOpen = false;
                        //}
                    }
                    else if (listView.SelectedItems.Count == 1)
                    {
                        ViewModelPage.SearchAuthorsMessage = $"Rechercher les livres de cet auteur";
                        ViewModelPage.SearchEditorsMessage = $"Rechercher les livres de cet éditeur";
                    }
                    else
                    {
                        ViewModelPage.SearchAuthorsMessage = $"Aucun livre à rechercher";
                        ViewModelPage.SearchAuthorsMessage = $"Aucun livre à rechercher";
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void MenuFlyout_ListViewItemContext_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    if (ViewModelPage.SelectedViewModels != null && ViewModelPage.SelectedViewModels.Count > 0)
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem menuFlyoutItemSearchAuthors)
                        {
                            menuFlyoutItemSearchAuthors.Text = ViewModelPage.SearchAuthorsMessage;
                            menuFlyoutItemSearchAuthors.IsEnabled = true;
                        }

                        if (menuFlyout.Items[1] is MenuFlyoutItem menuFlyoutItemSearchEditors)
                        {
                            menuFlyoutItemSearchEditors.Text = ViewModelPage.SearchEditorsMessage;
                            menuFlyoutItemSearchEditors.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem menuFlyoutItemSearchAuthors)
                        {
                            if (menuFlyoutItemSearchAuthors.CommandParameter is ContactVM contactVM)
                            {
                                menuFlyoutItemSearchAuthors.IsEnabled = true;
                                ViewModelPage.SearchAuthorsMessage = $"Rechercher les livres de cet auteur";
                            }
                            else
                            {
                                menuFlyoutItemSearchAuthors.IsEnabled = false;
                            }
                            menuFlyoutItemSearchAuthors.Text = ViewModelPage.SearchAuthorsMessage;
                        }

                        if (menuFlyout.Items[1] is MenuFlyoutItem menuFlyoutItemSearchEditors)
                        {
                            if (menuFlyoutItemSearchEditors.CommandParameter is ContactVM contactVM)
                            {
                                menuFlyoutItemSearchEditors.IsEnabled = true;
                                ViewModelPage.SearchEditorsMessage = $"Rechercher les livres de cet éditeur";
                            }
                            else
                            {
                                menuFlyoutItemSearchEditors.IsEnabled = false;
                            }
                            menuFlyoutItemSearchEditors.Text = ViewModelPage.SearchEditorsMessage;
                        }
                    }

                    if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[3] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Ajouter {ParentPage.ViewModelPage.SelectedItems.Count} livre(s) à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[3] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à ajouter à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = false;
                        }
                    }

                    if (menuFlyout.Items[4] is MenuFlyoutItem flyoutItemDecategorize)
                    {
                        if (flyoutItemDecategorize.Tag is ContactVM collectionVM)
                        {
                            //if (collectionVM.BooksId != null && collectionVM.BooksId.Any())
                            //{
                            //    flyoutItemDecategorize.Text = $"Retirer {collectionVM.BooksId.Count} livre(s) de « {collectionVM.Name} »";
                            //    flyoutItemDecategorize.IsEnabled = true;
                            //}
                            //else
                            //{
                            //    flyoutItemDecategorize.Text = $"Aucun livre à retirer de « {collectionVM.Name} »";
                            //    flyoutItemDecategorize.IsEnabled = false;
                            //}
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

        private void MenuFlyout_DeleteCmB_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItemDelete)
                    {
                        if (ViewModelPage.SelectedViewModels.Count > 1)
                        {
                            flyoutItemDelete.Text = $"Supprimer « {ViewModelPage.SelectedViewModels.Count} contacts »";
                        }
                        else if (ViewModelPage.SelectedViewModels.Count == 1)
                        {
                            flyoutItemDelete.Text = $"Supprimer « {ViewModelPage.SelectedViewModels[0].DisplayName3} »";
                        }
                        else
                        {
                            flyoutItemDelete.Text = $"Aucun contact n'est à supprimer";
                        }
                    }

                    if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[1] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Décatégoriser {ParentPage.ViewModelPage.SelectedItems.Count} livre(s)";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[1] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à retirer d'une collection";
                            flyoutItem.IsEnabled = false;
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

        private async void AddBooksToCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                {
                    if (args.Parameter is ContactVM viewModel)
                    {
                        //var creationResult = await DbServices.Collection.CreateCollectionConnectorAsync(ParentPage.ViewModelPage.SelectedItems.Select(s => s.Id), viewModel);
                        //if (creationResult.IsSuccess)
                        //{
                        //    ViewModelPage.ResultMessageTitle = "Succès";
                        //    ViewModelPage.ResultMessage = creationResult.Message;
                        //    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        //    ViewModelPage.IsResultMessageOpen = true;

                        //    await ParentPage.UpdateLibraryCollectionAsync();
                        //}
                        //else
                        //{
                        //    //Erreur
                        //    ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        //    ViewModelPage.ResultMessage = creationResult.Message;
                        //    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        //    ViewModelPage.IsResultMessageOpen = true;
                        //    return;
                        //}
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

        private async void DecategorizeBooksFromCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                OperationStateVM result = null;
                if (args.Parameter is ContactVM viewModel)
                {
                    //result = await DbServices.Collection.DecategorizeBooksAsync(viewModel.BooksId);
                }

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await ParentPage.UpdateLibraryCollectionAsync();
                    }
                    else
                    {
                        //Erreur
                        ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        ViewModelPage.IsResultMessageOpen = true;
                        return;
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


        private async void MenuFlyoutItem_DecategorizeBooksFromCollection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any() && ParentPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                {
                    OperationStateVM result = await DbServices.Collection.DecategorizeBooksAsync(collection.Select(s => s.Id));
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await ParentPage.UpdateLibraryCollectionAsync();
                    }
                    else
                    {
                        //Erreur
                        ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        ViewModelPage.IsResultMessageOpen = true;
                        return;
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

        private void ResearchBooksFromAuthorsXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ParentPage.IsContainsBookCollection(out _))
                {
                    List<ContactVM> list = new List<ContactVM>();
                    if (ViewModelPage.SelectedViewModels != null && ViewModelPage.SelectedViewModels.Any())
                    {
                        list.AddRange(ViewModelPage.SelectedViewModels);
                    }
                    else if (args.Parameter is ContactVM contactVM)
                    {
                        list.Add(contactVM);
                    }

                    if (list != null && list.Any())
                    {
                        ParentPage.LaunchSearch(list.Select(s => new ResearchItemVM()
                        {
                            IdLibrary = ParentPage.Parameters.ParentLibrary.Id,
                            SearchInAuthors = true,
                            Term = s.DisplayName3,
                            TermParameter = Search.Terms.Equals,
                        }), false, false);
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ResearchBooksFromEditorsXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ParentPage.IsContainsBookCollection(out _))
                {
                    List<ContactVM> list = new List<ContactVM>();
                    if (ViewModelPage.SelectedViewModels != null && ViewModelPage.SelectedViewModels.Any())
                    {
                        list.AddRange(ViewModelPage.SelectedViewModels);
                    }
                    else if (args.Parameter is ContactVM contactVM)
                    {
                        list.Add(contactVM);
                    }

                    if (list != null && list.Any())
                    {
                        ParentPage.LaunchSearch(list.Select(s => new ResearchItemVM()
                        {
                            IdLibrary = ParentPage.Parameters.ParentLibrary.Id,
                            SearchInEditors = true,
                            Term = s.DisplayName3,
                            TermParameter = Search.Terms.Equals,
                        }), false, false);
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        

        private void ABBtnExport_Click(object sender, RoutedEventArgs e)
        {

        }


        private void MFI_CreatePerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentPage.NewEditContact(EditMode.Create, ContactType.Human, null, new SideBarInterLinkVM()
                {
                    ParentGuid = this.ItemGuid,
                    ParentType = typeof(ContactListUC)
                }, null);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void MFI_CreateSociety_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentPage.NewEditContact(EditMode.Create, ContactType.Society, null, new SideBarInterLinkVM()
                {
                    ParentGuid = this.ItemGuid,
                    ParentType = typeof(ContactListUC)
                }, null);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
    }

    public class ContactListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //public SideBarInterLinkVM ParentReferences { get; set; }
        public IEnumerable<ContactVM> contactList;
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

        private ContactType? _ContactType;
        public ContactType? ContactType
        {
            get => this._ContactType;
            set
            {
                if (this._ContactType != value)
                {
                    this._ContactType = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ContactRole> _ContactRoles = new ObservableCollection<ContactRole>();
        public ObservableCollection<ContactRole> ContactRoles
        {
            get => _ContactRoles;
            set
            {
                if (_ContactRoles != value)
                {
                    _ContactRoles = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly IEnumerable<string> civilityList = CivilityHelpers.CiviliteListShorted();

        private ContactVM _SelectedViewModel;
        public ContactVM SelectedViewModel
        {
            get => _SelectedViewModel;
            set
            {
                if (_SelectedViewModel != value)
                {
                    _SelectedViewModel = value;
                    OnPropertyChanged();
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

        private string _SelectedViewModelMessage;
        public string SelectedViewModelMessage
        {
            get => this._SelectedViewModelMessage;
            set
            {
                if (this._SelectedViewModelMessage != value)
                {
                    this._SelectedViewModelMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _SearchAuthorsMessage;
        public string SearchAuthorsMessage
        {
            get => this._SearchAuthorsMessage;
            set
            {
                if (this._SearchAuthorsMessage != value)
                {
                    this._SearchAuthorsMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _SearchEditorsMessage;
        public string SearchEditorsMessage
        {
            get => this._SearchEditorsMessage;
            set
            {
                if (this._SearchEditorsMessage != value)
                {
                    this._SearchEditorsMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ContactVM> _SelectedViewModels = new ObservableCollection<ContactVM>();
        public ObservableCollection<ContactVM> SelectedViewModels
        {
            get => this._SelectedViewModels;
            set
            {
                if (_SelectedViewModels != value)
                {
                    this._SelectedViewModels = value;
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
