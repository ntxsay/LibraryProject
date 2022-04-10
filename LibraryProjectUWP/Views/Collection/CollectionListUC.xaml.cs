﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
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
using System.Threading;
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

namespace LibraryProjectUWP.Views.Collection
{
    public sealed partial class CollectionListUC : PivotItem
    {
        public CollectionListParametersDriverVM _parameters;

        public CollectionListUCVM ViewModelPage { get; set; } = new CollectionListUCVM();


        public delegate void CancelModificationEventHandler(CollectionListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;


        public CollectionListUC()
        {
            this.InitializeComponent();
        }

        public CollectionListUC(CollectionListParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Collections";
            ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
            //if (parameters.ViewModelList != null && parameters.ViewModelList.Any())
            //{
            //    ViewModelPage.CollectionViewModelList = new ObservableCollection<CollectionVM>(parameters.ViewModelList);
            //}
            InitializeActionInfos();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_parameters.ParentPage.ViewModelPage.SelectedCollections != null && 
                    _parameters.ParentPage.ViewModelPage.SelectedCollections.Any())
                {
                    foreach (var item in MyListView.Items)
                    {
                        foreach (var viewModel in _parameters.ParentPage.ViewModelPage.SelectedCollections)
                        {
                            if (item is CollectionVM _viewModel && _viewModel.Id == viewModel.Id && !MyListView.SelectedItems.Contains(item))
                            {
                                MyListView.SelectedItems.Add(item);
                                break;
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

        private void InitializeActionInfos()
        {
            try
            {
                
            }
            catch (Exception)
            {

                throw;
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

                ViewModelPage = null;
                _parameters = null;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private async void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await _parameters.ParentPage.NewCollectionAsync(String.Empty, ViewModelPage.ItemGuid, typeof(CollectionListUC));
        }

        private async void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is CollectionVM viewModel)
                {
                    await _parameters.ParentPage.EditCollection(viewModel, ViewModelPage.ItemGuid, typeof(CollectionListUC));
                }
                else
                {
                    await _parameters.ParentPage.EditCollection(ViewModelPage.SelectedViewModel, ViewModelPage.ItemGuid, typeof(CollectionListUC));
                }
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
                //if (ViewModelPage.Value.IsStringNullOrEmptyOrWhiteSpace())
                //{
                //    ViewModelPage.ResultMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
                //    return false;
                //}

                //if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                //{
                //    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentLibrary?.Name?.Trim().ToLower() == ViewModelPage.Value?.Trim().ToLower());
                //    if (isError)
                //    {
                //        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                //        return false;
                //    }
                //}

                //ViewModelPage.ResultMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                //if (ViewModelPage.SearchedViewModel != null)
                //{
                //    ViewModelPage.SearchedViewModel = null;
                //}

                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || _parameters.ParentLibrary.Books == null || !_parameters.ParentLibrary.Books.Any())
                {
                    return;
                }

                var FilteredItems = new List<CollectionVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in _parameters.ParentLibrary.Collections)
                {
                    if (value.Name.IsStringNullOrEmptyOrWhiteSpace()) continue;

                    var found = splitSearchTerm.All((key) =>
                    {
                        return value.Name.ToLower().Contains(key.ToLower());
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
                if (args.SelectedItem != null && args.SelectedItem is CollectionVM value)
                {
                    sender.Text = value.Name;
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
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is CollectionVM viewModel)
                {
                    foreach (var item in MyListView.Items)
                    {
                        if (item is CollectionVM itemViewModel && itemViewModel.Id == viewModel.Id)
                        {
                            if (MyListView.SelectedItem != item)
                            {
                                MyListView.SelectedItem = item;
                            }
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



        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is CollectionVM viewModel)
                {
                    DependencyObject selectedItem = MyListView.ContainerFromItem(viewModel);
                    if (selectedItem is ListViewItem listViewItem)
                    {
                        var textblock = new TextBlock()
                        {
                            TextWrapping = TextWrapping.Wrap,
                        };
                        Run run1 = new Run()
                        {
                            Text = $"Êtes-vous sûr de vouloir supprimer la collection « ",
                            //FontWeight = FontWeights.Medium,
                        };
                        Run run2 = new Run()
                        {
                            Text = viewModel.Name,
                            Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                            FontWeight = FontWeights.Medium,
                        };
                        Run run3 = new Run()
                        {
                            Text = $" » ?",
                            //FontWeight = FontWeights.Medium,
                        };

                        Run run4 = new Run()
                        {
                            Text = $"Veuillez noter que cette action entraînera la suppression de cette collection dans les livres concernés.",
                            Foreground = new SolidColorBrush(Colors.OrangeRed),
                        };
                        textblock.Inlines.Add(run1);
                        textblock.Inlines.Add(run2);
                        textblock.Inlines.Add(run3);
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(run4);

                        TtipDeleteCollection.Target = listViewItem;
                        TtipDeleteCollection.Title = "Supprimer une collection";
                        TtipDeleteCollection.Content = textblock;
                        TtipDeleteCollection.IsOpen = true;
                    }
                }
                else
                {
                    if (this.ViewModelPage.SelectedViewModels.Count > 1)
                    {
                        var textblock = new TextBlock()
                        {
                            TextWrapping = TextWrapping.Wrap,
                        };
                        Run run1 = new Run()
                        {
                            Text = $"Êtes-vous sûr de vouloir supprimer les collections sélectionnées ?",
                            //FontWeight = FontWeights.Medium,
                        };

                        Run run2 = new Run()
                        {
                            Text = $"Veuillez noter que cette action entraînera la suppression de ces collections dans les livres concernés.",
                            Foreground = new SolidColorBrush(Colors.OrangeRed),
                        };
                        textblock.Inlines.Add(run1);
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(run2);

                        TtipDeleteCollection.Target = ABBDelete;
                        TtipDeleteCollection.Title = "Supprimer des collections";
                        TtipDeleteCollection.Content = textblock;
                        TtipDeleteCollection.IsOpen = true;
                    }
                    else if (this.ViewModelPage.SelectedViewModels.Count == 1)
                    {
                        DependencyObject selectedItem = MyListView.ContainerFromItem(MyListView.SelectedItem);
                        if (selectedItem is ListViewItem listViewItem)
                        {
                            var textblock = new TextBlock()
                            {
                                TextWrapping = TextWrapping.Wrap,
                            };
                            Run run1 = new Run()
                            {
                                Text = $"Êtes-vous sûr de vouloir supprimer la collection « ",
                                //FontWeight = FontWeights.Medium,
                            };
                            Run run2 = new Run()
                            {
                                Text = ViewModelPage.SelectedViewModel?.Name,
                                Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                FontWeight = FontWeights.Medium,
                            };
                            Run run3 = new Run()
                            {
                                Text = $" » ?",
                                //FontWeight = FontWeights.Medium,
                            };

                            Run run4 = new Run()
                            {
                                Text = $"Veuillez noter que cette action entraînera la suppression de cette collection dans les livres concernés.",
                                Foreground = new SolidColorBrush(Colors.OrangeRed),
                            };
                            textblock.Inlines.Add(run1);
                            textblock.Inlines.Add(run2);
                            textblock.Inlines.Add(run3);
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(run4);

                            TtipDeleteCollection.Target = listViewItem;
                            TtipDeleteCollection.Title = "Supprimer une collection";
                            TtipDeleteCollection.Content = textblock;
                            TtipDeleteCollection.IsOpen = true;
                        }


                    }
                    else if (this.ViewModelPage.SelectedViewModels.Count == 1)
                    {
                        DependencyObject selectedItem = MyListView.ContainerFromItem(MyListView.SelectedItem);
                        if (selectedItem is ListViewItem listViewItem)
                        {
                            var textblock = new TextBlock()
                            {
                                TextWrapping = TextWrapping.Wrap,
                            };
                            Run run1 = new Run()
                            {
                                Text = $"Êtes-vous sûr de vouloir supprimer la collection « ",
                                //FontWeight = FontWeights.Medium,
                            };
                            Run run2 = new Run()
                            {
                                Text = ViewModelPage.SelectedViewModel?.Name,
                                Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                FontWeight = FontWeights.Medium,
                            };
                            Run run3 = new Run()
                            {
                                Text = $" » ?",
                                //FontWeight = FontWeights.Medium,
                            };

                            Run run4 = new Run()
                            {
                                Text = $"Veuillez noter que cette action entraînera la suppression de cette collection dans les livres concernés.",
                                Foreground = new SolidColorBrush(Colors.OrangeRed),
                            };
                            textblock.Inlines.Add(run1);
                            textblock.Inlines.Add(run2);
                            textblock.Inlines.Add(run3);
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(run4);

                            TtipDeleteCollection.Target = listViewItem;
                            TtipDeleteCollection.Title = "Supprimer une collection";
                            TtipDeleteCollection.Content = textblock;
                            TtipDeleteCollection.IsOpen = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ListView listView)
                {
                    var cast = listView.SelectedItems.Cast<CollectionVM>();
                    this.ViewModelPage.SelectedViewModels = new ObservableCollection<CollectionVM>(cast);
                    if (listView.SelectedItems.Count > 1)
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Afficher « {listView.SelectedItems.Count} collections »";
                        if (TtipDeleteCollection.IsOpen)
                        {
                            TtipDeleteCollection.IsOpen = false;
                        }
                    }
                    else if (listView.SelectedItems.Count == 1)
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Afficher « {ViewModelPage.SelectedViewModel?.Name} »";
                    }
                    else
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Aucune collection n'est à afficher";
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void BtnDeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                foreach (var id in ViewModelPage.SelectedViewModels.Select(s => s.Id))
                {
                    var result = await DbServices.Collection.DeleteAsync(id);
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;
                        var item = _parameters.ParentLibrary.Collections.SingleOrDefault(s => s.Id == id);
                        if (item != null)
                        {
                            _parameters.ParentLibrary.Collections.Remove(item);
                        }

                        Thread.Sleep(500);
                    }
                    else
                    {
                        //Erreur
                        ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        ViewModelPage.ResultMessage += "\n" + result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        ViewModelPage.IsResultMessageOpen = true;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


        private void ExportAllCollectionToJsonXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }


        private void BtnDeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            TtipDeleteCollection.IsOpen = false;
        }

        private async void NavigateInThisItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.SelectedViewModels != null)
                {
                    _parameters.ParentPage.ViewModelPage.SelectedCollections = ViewModelPage.SelectedViewModels;
                    await _parameters.ParentPage.RefreshItemsGrouping();
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
                _parameters.ParentPage.ViewModelPage.SelectedCollections = null;
                await _parameters.ParentPage.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void MenuFlyout_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    if (_parameters.ParentPage.ViewModelPage.SelectedItems != null && _parameters.ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Ajouter {_parameters.ParentPage.ViewModelPage.SelectedItems.Count} livre(s) à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à ajouter à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = false;
                        }
                    }

                    if (menuFlyout.Items[1] is MenuFlyoutItem flyoutItemDecategorize)
                    {
                        if (flyoutItemDecategorize.Tag is CollectionVM collectionVM)
                        {
                            if (collectionVM.BooksId != null && collectionVM.BooksId.Any())
                            {
                                flyoutItemDecategorize.Text = $"Retirer {collectionVM.BooksId.Count} livre(s) de « {collectionVM.Name} »";
                                flyoutItemDecategorize.IsEnabled = true;
                            }
                            else
                            {
                                flyoutItemDecategorize.Text = $"Aucun livre à retirer de « {collectionVM.Name} »";
                                flyoutItemDecategorize.IsEnabled = false;
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

        private void ListViewItem_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            try
            {

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
                if (_parameters.ParentPage.ViewModelPage.SelectedItems != null && _parameters.ParentPage.ViewModelPage.SelectedItems.Any())
                {
                    if (args.Parameter is CollectionVM viewModel)
                    {
                        var creationResult = await DbServices.Collection.CreateCollectionConnectorAsync(_parameters.ParentPage.ViewModelPage.SelectedItems.Select(s => s.Id), viewModel);
                        if (creationResult.IsSuccess)
                        {
                            ViewModelPage.ResultMessageTitle = "Succès";
                            ViewModelPage.ResultMessage = creationResult.Message;
                            ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                            ViewModelPage.IsResultMessageOpen = true;

                            await _parameters.ParentPage.UpdateLibraryCollectionAsync();
                        }
                        else
                        {
                            //Erreur
                            ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                            ViewModelPage.ResultMessage = creationResult.Message;
                            ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                            ViewModelPage.IsResultMessageOpen = true;
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

        private async void DecategorizeBooksFromCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                OperationStateVM result = null;
                if (args.Parameter is CollectionVM viewModel)
                {
                    result = await DbServices.Collection.DecategorizeBooksAsync(viewModel.BooksId);
                }

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await _parameters.ParentPage.UpdateLibraryCollectionAsync();
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
                            flyoutItemDelete.Text = $"Supprimer « {ViewModelPage.SelectedViewModels.Count} collections »";
                        }
                        else if (ViewModelPage.SelectedViewModels.Count == 1)
                        {
                            flyoutItemDelete.Text = $"Supprimer « {ViewModelPage.SelectedViewModels[0].Name} »";
                        }
                        else
                        {
                            flyoutItemDelete.Text = $"Aucune collection n'est à supprimer";
                        }
                    }

                    if (_parameters.ParentPage.ViewModelPage.SelectedItems != null && _parameters.ParentPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[1] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Décatégoriser {_parameters.ParentPage.ViewModelPage.SelectedItems.Count} livre(s)";
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

        private async void MenuFlyoutItem_DecategorizeBooksFromCollection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_parameters.ParentPage.ViewModelPage.SelectedItems != null && _parameters.ParentPage.ViewModelPage.SelectedItems.Any())
                {
                    OperationStateVM result = await DbServices.Collection.DecategorizeBooksAsync(_parameters.ParentPage.ViewModelPage.SelectedItems.Select(s => s.Id));
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await _parameters.ParentPage.UpdateLibraryCollectionAsync();
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
    }

    public class CollectionListUCVM : INotifyPropertyChanged
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

        private string _Glyph = "\uE81E";
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


        private BibliothequeVM _ParentLibrary;
        public BibliothequeVM ParentLibrary
        {
            get => this._ParentLibrary;
            set
            {
                if (this._ParentLibrary != value)
                {
                    this._ParentLibrary = value;
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

        private CollectionVM _SelectedViewModel;
        public CollectionVM SelectedViewModel
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

        private ObservableCollection<CollectionVM> _CollectionViewModelList = new ObservableCollection<CollectionVM>();
        [Obsolete]
        public ObservableCollection<CollectionVM> CollectionViewModelList
        {
            get => this._CollectionViewModelList;
            set
            {
                if (_CollectionViewModelList != value)
                {
                    this._CollectionViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CollectionVM> _SelectedViewModels = new ObservableCollection<CollectionVM>();
        public ObservableCollection<CollectionVM> SelectedViewModels
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
