using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Code.Services.Tasks;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
using Microsoft.UI.Xaml.Controls;
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
                    if (e.Result is WorkerState<ContactVM, ContactVM> result && result.ResultList != null && result.ResultList.Any())
                    {
                        var GroupingItems = this.OrderItems(result.ResultList)?.Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.NomNaissance.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                        if (GroupingItems != null && GroupingItems.Count() > 0)
                        {
                            if (this.CollectionViewSource.View == null)
                            {
                                this.CollectionViewSource.Source = ViewModelListGroup;
                            }

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
            try
            {
                //if (ViewModelPage.SearchedViewModel != null)
                //{
                //    ViewModelPage.SearchedViewModel = null;
                //}

                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || !ViewModelListGroup.Any())
                {
                    return;
                }

                var FilteredItems = new List<ContactVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelListGroup.Select(s => s.Items).SelectMany(a => a.ToList()).Select(q => q).ToList())
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

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                //bool isValided = IsModelValided();
                //if (!isValided)
                //{
                //    return;
                //}

                UpdateItemRequested?.Invoke(this, args);
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
                        ViewModelPage.SelectedViewModelMessage = $"Afficher « {listView.SelectedItems.Count} contacts »";
                        //if (TtipDeleteCollection.IsOpen)
                        //{
                        //    TtipDeleteCollection.IsOpen = false;
                        //}
                    }
                    else if (listView.SelectedItems.Count == 1)
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Afficher « {ViewModelPage.SelectedViewModel?.DisplayName3} »";
                    }
                    else
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Aucun contact n'est à afficher";
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
                    if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItemOpenSelectedBooks)
                    {
                        if (ViewModelPage.SelectedViewModels != null && ViewModelPage.SelectedViewModels.Count > 0)
                        {
                            flyoutItemOpenSelectedBooks.Text = ViewModelPage.SelectedViewModelMessage;
                            flyoutItemOpenSelectedBooks.IsEnabled = true;
                        }
                        else if (flyoutItemOpenSelectedBooks.Tag is string value)
                        {
                            ViewModelPage.SelectedViewModelMessage = $"Afficher « {value} »";
                            flyoutItemOpenSelectedBooks.Text = ViewModelPage.SelectedViewModelMessage;
                            flyoutItemOpenSelectedBooks.IsEnabled = true;
                        }
                        else
                        {
                            flyoutItemOpenSelectedBooks.Text = ViewModelPage.SelectedViewModelMessage;
                            flyoutItemOpenSelectedBooks.IsEnabled = false;
                        }
                    }


                    if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[2] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Ajouter {ParentPage.ViewModelPage.SelectedItems.Count} livre(s) à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[2] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à ajouter à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = false;
                        }
                    }

                    if (menuFlyout.Items[3] is MenuFlyoutItem flyoutItemDecategorize)
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
                if (ParentPage.ViewModelPage.SelectedItems != null && ParentPage.ViewModelPage.SelectedItems.Any())
                {
                    OperationStateVM result = await DbServices.Collection.DecategorizeBooksAsync(ParentPage.ViewModelPage.SelectedItems.Select(s => s.Id));
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

        private async void NavigateInThisItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.SelectedViewModels != null)
                {
                    ParentPage.ViewModelPage.SelectedContacts = ViewModelPage.SelectedViewModels;
                    await ParentPage.RefreshItemsGrouping();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NavigateInAllItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ParentPage.ViewModelPage.SelectedContacts = null;
                await ParentPage.RefreshItemsGrouping();
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
            catch (Exception)
            {

                throw;
            }
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

        
    }

    public class ContactListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //public SideBarInterLinkVM ParentReferences { get; set; }

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
